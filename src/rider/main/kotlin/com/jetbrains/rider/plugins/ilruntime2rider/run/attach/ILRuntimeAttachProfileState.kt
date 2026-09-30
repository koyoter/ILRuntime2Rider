package com.jetbrains.rider.plugins.ilruntime2rider.run.attach

import com.intellij.execution.configurations.GeneralCommandLine
import com.intellij.execution.runners.ExecutionEnvironment
import com.intellij.openapi.application.ApplicationManager
import com.jetbrains.rd.util.lifetime.Lifetime
import com.jetbrains.rider.debugger.DebuggerWorkerProcessHandler
import com.jetbrains.rider.model.debuggerWorker.DebuggerStartInfoBase
import com.jetbrains.rider.model.debuggerWorker.DebuggerWorkerModel
import com.jetbrains.rider.plugins.ilruntime2rider.debuggerWorker.ILRuntimeStartInfo
import com.jetbrains.rider.plugins.ilruntime2rider.debuggerWorker.Process
import com.jetbrains.rider.plugins.ilruntime2rider.debuggerWorker.iLRuntimeDebuggerWorkerModel
import com.jetbrains.rider.plugins.ilruntime2rider.run.ILRuntimeProcess
import com.jetbrains.rider.plugins.ilruntime2rider.run.ILRuntimeProcessFinder
import com.jetbrains.rider.run.configurations.remote.MonoConnectRemoteProfileState
import com.jetbrains.rider.run.configurations.remote.RemoteConfiguration

class ILRuntimeAttachProfileState(
    remoteConfiguration: RemoteConfiguration,
    executionEnvironment: ExecutionEnvironment,
    private val process: ILRuntimeProcess
) : MonoConnectRemoteProfileState(remoteConfiguration, executionEnvironment) {

    override suspend fun createModelStartInfo(lifetime: Lifetime): DebuggerStartInfoBase {
        return ILRuntimeStartInfo(process.port, process.ip, process.projectName)
    }

    override suspend fun createDebuggerWorker(
        workerCmd: GeneralCommandLine,
        protocolModel: DebuggerWorkerModel,
        protocolServerPort: Int,
        projectLifetime: Lifetime
    ): DebuggerWorkerProcessHandler {
        val nested = projectLifetime.createNested()
        val model = protocolModel.iLRuntimeDebuggerWorkerModel
        ILRuntimeProcessFinder.onProcessChanged.advise(nested) { discovered ->
            if (discovered != null) {
                // RdSignal.fire asserts the UI/protocol thread; the discovery
                // loop runs on a pooled thread.
                ApplicationManager.getApplication().invokeLater {
                    model.signal.fire(Process(discovered.ip, discovered.port, discovered.projectName))
                }
            }
        }
        return super.createDebuggerWorker(workerCmd, protocolModel, protocolServerPort, projectLifetime)
    }
}
