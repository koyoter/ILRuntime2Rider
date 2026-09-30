package model.debuggerWorker

import com.jetbrains.rd.generator.nova.*
import com.jetbrains.rd.generator.nova.PredefinedType.*
import com.jetbrains.rd.generator.nova.csharp.CSharp50Generator
import com.jetbrains.rd.generator.nova.kotlin.Kotlin11Generator
import com.jetbrains.rider.model.nova.debugger.main.DebuggerWorkerModel

@Suppress("unused")
object ILRuntimeDebuggerWorkerModel : Ext(DebuggerWorkerModel) {

    private val ILRuntimeStartInfoBase = basestruct extends DebuggerWorkerModel.debuggerStartInfoBase {
        field("port", int)
        field("host", string.nullable)
        field("projectName", string.nullable)
    }

    private val ILRuntimeStartInfo = structdef extends ILRuntimeStartInfoBase {}

    init {
        setting(Kotlin11Generator.Namespace, "com.jetbrains.rider.plugins.ilruntime2rider.debuggerWorker")
        setting(CSharp50Generator.Namespace, "JetBrains.Debugger.Model.Plugins.ILRuntime2Rider")

        signal("signal", structdef("process") {
            field("host", string)
            field("port", int)
            field("projectName", string)
        })
    }
}
