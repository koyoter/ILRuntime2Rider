package com.jetbrains.rider.plugins.ilruntime2rider.run

import com.google.common.base.Charsets
import com.intellij.openapi.application.ApplicationManager
import com.intellij.openapi.diagnostic.Logger
import com.jetbrains.rd.util.reactive.IProperty
import com.jetbrains.rd.util.reactive.Property
import com.jetbrains.rd.util.reactive.ViewableSet
import com.jetbrains.rider.plugins.ilruntime2rider.ILRuntimeConstants
import java.io.ByteArrayInputStream
import java.io.DataInputStream
import java.net.DatagramPacket
import java.net.DatagramSocket
import java.net.SocketTimeoutException
import java.util.concurrent.ConcurrentHashMap

/**
 * Listens for UDP broadcasts from ILRuntime hosts and accumulates the discovered
 * processes. Wire format per datagram: utf8 string (int16 LE length prefix),
 * utf8 string, int32 LE process id, int32 LE debugger port.
 */
object ILRuntimeProcessFinder {
    private val log = Logger.getInstance(ILRuntimeProcessFinder::class.java)

    private val socketBuffer = ByteArray(65535)
    private val stringBuffer = ByteArray(1024)
    private var socket: DatagramSocket? = null

    @Volatile
    private var receiveLoopStarted = false

    val processes = ViewableSet<ILRuntimeProcess>()
    val onError: IProperty<String> = Property("")
    val onProcessChanged: IProperty<ILRuntimeProcess?> = Property(null)

    private val lastSeen = ConcurrentHashMap<ILRuntimeProcess, Long>()

    /** The process this plugin instance is currently attached to; exempt from TTL expiry. */
    @Volatile
    var attachedProcess: ILRuntimeProcess? = null

    var port: Int = ILRuntimeConstants.PORT
        private set

    init {
        listen(port)
    }

    /** Rebinds the discovery socket to [newPort] and drops the accumulated process list. */
    fun listen(newPort: Int) {
        port = newPort
        try {
            processes.clear()
            lastSeen.clear()
            socket?.close()
            socket = DatagramSocket(newPort).apply { soTimeout = ILRuntimeConstants.PROCESS_REFRESH_INTERVAL_MS }
            ensureReceiveLoop()
        } catch (e: Exception) {
            onError.set(e.message ?: e.javaClass.simpleName)
        }
    }

    /** Drops entries that stopped broadcasting for more than [thresholdMs], keeping [attachedProcess]. */
    fun removeStale(thresholdMs: Long) {
        val now = System.currentTimeMillis()
        for ((process, seen) in lastSeen) {
            if (now - seen <= thresholdMs) continue
            if (process == attachedProcess && isHostAlive(process)) continue
            if (process == attachedProcess) attachedProcess = null
            lastSeen.remove(process)
            processes.remove(process)
        }
    }

    // Broadcasts stop while a debugger is attached, so the attached entry is kept while its host
    // process lives. Only local hosts can be liveness-checked here; remote ones stay exempt.
    private fun isHostAlive(process: ILRuntimeProcess): Boolean =
        !ILRuntimeConstants.isLocalAddress(process.ip) || ProcessHandle.of(process.processId.toLong()).isPresent

    // A single long-lived receive loop: recreating it per [listen] call would race
    // concurrent receivers over the shared buffers.
    private fun ensureReceiveLoop() {
        if (receiveLoopStarted) return
        receiveLoopStarted = true
        ApplicationManager.getApplication().executeOnPooledThread {
            try {
                while (true) {
                    val received = try {
                        receiveOnce()
                    } catch (e: SocketTimeoutException) {
                        false
                    } catch (e: Exception) {
                        log.warn("ILRuntime process discovery receive failed: ${e.message}")
                        false
                    }
                    try {
                        removeStale(ILRuntimeConstants.PROCESS_TTL_MS.toLong())
                    } catch (e: Exception) {
                        // The sweep fires dialog refresh advises; never let one kill this thread,
                        // receiveLoopStarted would stay true and discovery would never restart.
                        log.warn("ILRuntime process discovery sweep failed: ${e.message}")
                    }
                    // Sleep only when idle: a fresh broadcast must reach the UI without delay.
                    if (!received) {
                        try {
                            Thread.sleep(ILRuntimeConstants.PROCESS_REFRESH_INTERVAL_MS.toLong())
                        } catch (e: InterruptedException) {
                            Thread.currentThread().interrupt()
                            break
                        }
                    }
                }
            } finally {
                receiveLoopStarted = false
            }
        }
    }

    /** Waits for and processes one datagram. Returns false only when idle (no socket). */
    private fun receiveOnce(): Boolean {
        val currentSocket = socket ?: return false
        val packet = DatagramPacket(socketBuffer, socketBuffer.size)
        currentSocket.receive(packet)
        val ip = packet.address?.hostAddress ?: return true

        val input = DataInputStream(ByteArrayInputStream(socketBuffer))
        val projectName = readUtf8String(input) ?: return true
        val machineName = readUtf8String(input) ?: return true
        val processId = readIntLe(input)
        val debuggerPort = readIntLe(input)

        val process = ILRuntimeProcess(projectName, machineName, processId, debuggerPort, ip)
        lastSeen[process] = System.currentTimeMillis()
        processes.add(process)
        onProcessChanged.set(null)
        onProcessChanged.set(process)
        return true
    }

    private fun readUtf8String(input: DataInputStream): String? {
        val length = readShortLe(input)
        if (length <= 0) return null
        val bytesRead = input.read(stringBuffer, 0, length)
        if (bytesRead < length) {
            log.warn("Expected $length bytes for string, but only got $bytesRead")
            return null
        }
        return String(stringBuffer, 0, length, Charsets.UTF_8)
    }

    private fun readShortLe(input: DataInputStream): Int {
        val first = input.read()
        val second = input.read()
        if (first == -1 || second == -1) return -1
        return first or (second shl 8)
    }

    private fun readIntLe(input: DataInputStream): Int {
        val b0 = input.read()
        val b1 = input.read()
        val b2 = input.read()
        val b3 = input.read()
        return b0 or (b1 shl 8) or (b2 shl 16) or (b3 shl 24)
    }
}
