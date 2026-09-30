package com.jetbrains.rider.plugins.ilruntime2rider

import com.intellij.openapi.diagnostic.Logger
import java.net.InetAddress
import java.net.NetworkInterface

object ILRuntimeConstants {
    private val log = Logger.getInstance(ILRuntimeConstants::class.java)

    /** Default UDP port ILRuntime hosts broadcast their presence on. */
    const val PORT = 56000

    /** Delay between process-discovery receive attempts, ms. */
    const val PROCESS_REFRESH_INTERVAL_MS = 200

    const val DEBUGGER_NAME = "Attach To ILRuntime"
    const val DEBUGGER_DESCRIPTION = "Attach to a running ILRuntime host process"

    /** True if [ipAddress] is this machine's loopback or one of its local NIC addresses. */
    fun isLocalAddress(ipAddress: String?): Boolean {
        if (ipAddress.isNullOrBlank()) return false

        return try {
            val target = InetAddress.getByName(ipAddress)
            if (target.isLoopbackAddress) return true

            val interfaces = NetworkInterface.getNetworkInterfaces() ?: return false
            interfaces.asSequence()
                .filter { it.isUp && !it.isLoopback && !it.isVirtual }
                .flatMap { it.inetAddresses.asSequence() }
                .any { it == target }
        } catch (e: Exception) {
            log.warn("Failed to check whether $ipAddress is a local address: ${e.message}")
            false
        }
    }
}
