@file:Suppress("EXPERIMENTAL_API_USAGE","EXPERIMENTAL_UNSIGNED_LITERALS","PackageDirectoryMismatch","UnusedImport","unused","LocalVariableName","CanBeVal","PropertyName","EnumEntryName","ClassName","ObjectPropertyName","UnnecessaryVariable","SpellCheckingInspection")
package com.jetbrains.rider.plugins.ilruntime2rider.debuggerWorker

import com.jetbrains.rd.framework.*
import com.jetbrains.rd.framework.base.*
import com.jetbrains.rd.framework.impl.*

import com.jetbrains.rd.util.lifetime.*
import com.jetbrains.rd.util.reactive.*
import com.jetbrains.rd.util.string.*
import com.jetbrains.rd.util.*
import kotlin.time.Duration
import kotlin.reflect.KClass
import kotlin.jvm.JvmStatic



/**
 * #### Generated from [ILRuntimeDebuggerWorkerModel.kt:10]
 */
class ILRuntimeDebuggerWorkerModel private constructor(
    private val _signal: RdSignal<Process>
) : RdExtBase() {
    //companion
    
    companion object : ISerializersOwner {
        
        override fun registerSerializersCore(serializers: ISerializers)  {
            val classLoader = javaClass.classLoader
            serializers.register(LazyCompanionMarshaller(RdId(-8176282977667242034), classLoader, "com.jetbrains.rider.plugins.ilruntime2rider.debuggerWorker.ILRuntimeStartInfo"))
            serializers.register(LazyCompanionMarshaller(RdId(597109246684), classLoader, "com.jetbrains.rider.plugins.ilruntime2rider.debuggerWorker.Process"))
            serializers.register(LazyCompanionMarshaller(RdId(8214639194174048170), classLoader, "com.jetbrains.rider.plugins.ilruntime2rider.debuggerWorker.ILRuntimeStartInfoBase_Unknown"))
        }
        
        
        
        
        
        const val serializationHash = 7698449258580600213L
        
    }
    override val serializersOwner: ISerializersOwner get() = ILRuntimeDebuggerWorkerModel
    override val serializationHash: Long get() = ILRuntimeDebuggerWorkerModel.serializationHash
    
    //fields
    val signal: ISignal<Process> get() = _signal
    //methods
    //initializer
    init {
        bindableChildren.add("signal" to _signal)
    }
    
    //secondary constructor
    internal constructor(
    ) : this(
        RdSignal<Process>(Process)
    )
    
    //equals trait
    //hash code trait
    //pretty print
    override fun print(printer: PrettyPrinter)  {
        printer.println("ILRuntimeDebuggerWorkerModel (")
        printer.indent {
            print("signal = "); _signal.print(printer); println()
        }
        printer.print(")")
    }
    //deepClone
    override fun deepClone(): ILRuntimeDebuggerWorkerModel   {
        return ILRuntimeDebuggerWorkerModel(
            _signal.deepClonePolymorphic()
        )
    }
    //contexts
    //threading
    override val extThreading: ExtThreadingKind get() = ExtThreadingKind.Default
}
val com.jetbrains.rider.model.debuggerWorker.DebuggerWorkerModel.iLRuntimeDebuggerWorkerModel get() = getOrCreateExtension("iLRuntimeDebuggerWorkerModel", ::ILRuntimeDebuggerWorkerModel)



/**
 * #### Generated from [ILRuntimeDebuggerWorkerModel.kt:18]
 */
class ILRuntimeStartInfo (
    port: Int,
    host: String?,
    projectName: String?
) : ILRuntimeStartInfoBase (
    port,
    host,
    projectName
) {
    //companion
    
    companion object : IMarshaller<ILRuntimeStartInfo> {
        override val _type: KClass<ILRuntimeStartInfo> = ILRuntimeStartInfo::class
        override val id: RdId get() = RdId(-8176282977667242034)
        
        @Suppress("UNCHECKED_CAST")
        override fun read(ctx: SerializationCtx, buffer: AbstractBuffer): ILRuntimeStartInfo  {
            val port = buffer.readInt()
            val host = buffer.readNullable { buffer.readString() }
            val projectName = buffer.readNullable { buffer.readString() }
            return ILRuntimeStartInfo(port, host, projectName)
        }
        
        override fun write(ctx: SerializationCtx, buffer: AbstractBuffer, value: ILRuntimeStartInfo)  {
            buffer.writeInt(value.port)
            buffer.writeNullable(value.host) { buffer.writeString(it) }
            buffer.writeNullable(value.projectName) { buffer.writeString(it) }
        }
        
        
    }
    //fields
    //methods
    //initializer
    //secondary constructor
    //equals trait
    override fun equals(other: Any?): Boolean  {
        if (this === other) return true
        if (other == null || other::class != this::class) return false
        
        other as ILRuntimeStartInfo
        
        if (port != other.port) return false
        if (host != other.host) return false
        if (projectName != other.projectName) return false
        
        return true
    }
    //hash code trait
    override fun hashCode(): Int  {
        var __r = 0
        __r = __r*31 + port.hashCode()
        __r = __r*31 + if (host != null) host.hashCode() else 0
        __r = __r*31 + if (projectName != null) projectName.hashCode() else 0
        return __r
    }
    //pretty print
    override fun print(printer: PrettyPrinter)  {
        printer.println("ILRuntimeStartInfo (")
        printer.indent {
            print("port = "); port.print(printer); println()
            print("host = "); host.print(printer); println()
            print("projectName = "); projectName.print(printer); println()
        }
        printer.print(")")
    }
    
    override fun toString() = PrettyPrinter().singleLine().also { print(it) }.toString()
    //deepClone
    //contexts
    //threading
}


/**
 * #### Generated from [ILRuntimeDebuggerWorkerModel.kt:12]
 */
abstract class ILRuntimeStartInfoBase (
    val port: Int,
    val host: String?,
    val projectName: String?
) : com.jetbrains.rider.model.debuggerWorker.DebuggerStartInfoBase (
) {
    //companion
    
    companion object : IAbstractDeclaration<ILRuntimeStartInfoBase> {
        override fun readUnknownInstance(ctx: SerializationCtx, buffer: AbstractBuffer, unknownId: RdId, size: Int): ILRuntimeStartInfoBase  {
            val objectStartPosition = buffer.position
            val port = buffer.readInt()
            val host = buffer.readNullable { buffer.readString() }
            val projectName = buffer.readNullable { buffer.readString() }
            val unknownBytes = ByteArray(objectStartPosition + size - buffer.position)
            buffer.readByteArrayRaw(unknownBytes)
            return ILRuntimeStartInfoBase_Unknown(port, host, projectName, unknownId, unknownBytes)
        }
        
        
    }
    //fields
    //methods
    //initializer
    //secondary constructor
    //equals trait
    //hash code trait
    //pretty print
    //deepClone
    //contexts
    //threading
}


class ILRuntimeStartInfoBase_Unknown (
    port: Int,
    host: String?,
    projectName: String?,
    override val unknownId: RdId,
    val unknownBytes: ByteArray
) : ILRuntimeStartInfoBase (
    port,
    host,
    projectName
), IUnknownInstance {
    //companion
    
    companion object : IMarshaller<ILRuntimeStartInfoBase_Unknown> {
        override val _type: KClass<ILRuntimeStartInfoBase_Unknown> = ILRuntimeStartInfoBase_Unknown::class
        override val id: RdId get() = RdId(8214639194174048170)
        
        @Suppress("UNCHECKED_CAST")
        override fun read(ctx: SerializationCtx, buffer: AbstractBuffer): ILRuntimeStartInfoBase_Unknown  {
            throw NotImplementedError("Unknown instances should not be read via serializer")
        }
        
        override fun write(ctx: SerializationCtx, buffer: AbstractBuffer, value: ILRuntimeStartInfoBase_Unknown)  {
            buffer.writeInt(value.port)
            buffer.writeNullable(value.host) { buffer.writeString(it) }
            buffer.writeNullable(value.projectName) { buffer.writeString(it) }
            buffer.writeByteArrayRaw(value.unknownBytes)
        }
        
        
    }
    //fields
    //methods
    //initializer
    //secondary constructor
    //equals trait
    override fun equals(other: Any?): Boolean  {
        if (this === other) return true
        if (other == null || other::class != this::class) return false
        
        other as ILRuntimeStartInfoBase_Unknown
        
        if (port != other.port) return false
        if (host != other.host) return false
        if (projectName != other.projectName) return false
        
        return true
    }
    //hash code trait
    override fun hashCode(): Int  {
        var __r = 0
        __r = __r*31 + port.hashCode()
        __r = __r*31 + if (host != null) host.hashCode() else 0
        __r = __r*31 + if (projectName != null) projectName.hashCode() else 0
        return __r
    }
    //pretty print
    override fun print(printer: PrettyPrinter)  {
        printer.println("ILRuntimeStartInfoBase_Unknown (")
        printer.indent {
            print("port = "); port.print(printer); println()
            print("host = "); host.print(printer); println()
            print("projectName = "); projectName.print(printer); println()
        }
        printer.print(")")
    }
    
    override fun toString() = PrettyPrinter().singleLine().also { print(it) }.toString()
    //deepClone
    //contexts
    //threading
}


/**
 * #### Generated from [ILRuntimeDebuggerWorkerModel.kt:24]
 */
data class Process (
    val host: String,
    val port: Int,
    val projectName: String
) : IPrintable {
    //companion
    
    companion object : IMarshaller<Process> {
        override val _type: KClass<Process> = Process::class
        override val id: RdId get() = RdId(597109246684)
        
        @Suppress("UNCHECKED_CAST")
        override fun read(ctx: SerializationCtx, buffer: AbstractBuffer): Process  {
            val host = buffer.readString()
            val port = buffer.readInt()
            val projectName = buffer.readString()
            return Process(host, port, projectName)
        }
        
        override fun write(ctx: SerializationCtx, buffer: AbstractBuffer, value: Process)  {
            buffer.writeString(value.host)
            buffer.writeInt(value.port)
            buffer.writeString(value.projectName)
        }
        
        
    }
    //fields
    //methods
    //initializer
    //secondary constructor
    //equals trait
    override fun equals(other: Any?): Boolean  {
        if (this === other) return true
        if (other == null || other::class != this::class) return false
        
        other as Process
        
        if (host != other.host) return false
        if (port != other.port) return false
        if (projectName != other.projectName) return false
        
        return true
    }
    //hash code trait
    override fun hashCode(): Int  {
        var __r = 0
        __r = __r*31 + host.hashCode()
        __r = __r*31 + port.hashCode()
        __r = __r*31 + projectName.hashCode()
        return __r
    }
    //pretty print
    override fun print(printer: PrettyPrinter)  {
        printer.println("Process (")
        printer.indent {
            print("host = "); host.print(printer); println()
            print("port = "); port.print(printer); println()
            print("projectName = "); projectName.print(printer); println()
        }
        printer.print(")")
    }
    //deepClone
    //contexts
    //threading
}
