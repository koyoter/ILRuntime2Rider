package com.jetbrains.rider.plugins.ilruntime2rider.run.attach

import com.intellij.execution.Executor
import com.intellij.execution.configurations.RunConfiguration
import com.intellij.execution.configurations.RunProfileState
import com.intellij.execution.configurations.WithoutOwnBeforeRunSteps
import com.intellij.execution.executors.DefaultDebugExecutor
import com.intellij.execution.runners.ExecutionEnvironment
import com.intellij.execution.runners.RunConfigurationWithSuppressedDefaultRunAction
import com.intellij.openapi.application.ApplicationManager
import com.intellij.openapi.project.Project
import com.jetbrains.rider.plugins.ilruntime2rider.ILRuntimeConstants
import com.jetbrains.rider.plugins.ilruntime2rider.run.ILRuntimeProcess
import com.jetbrains.rider.plugins.ilruntime2rider.run.attach.ILRuntimeAttachProfileState
import com.jetbrains.rider.plugins.ilruntime2rider.ui.ILRuntimeProcessSelectionDialog
import com.jetbrains.rider.run.configurations.remote.DotNetRemoteConfiguration
import com.jetbrains.rider.run.configurations.remote.RemoteConfiguration

class ILRuntimeAttachRunConfiguration(
    project: Project,
    factory: ILRuntimeAttachRunFactory,
) : DotNetRemoteConfiguration(project, factory, ILRuntimeConstants.DEBUGGER_NAME),
    RunConfigurationWithSuppressedDefaultRunAction,
    RemoteConfiguration,
    WithoutOwnBeforeRunSteps {

    override fun hideDisabledExecutorButtons(): Boolean = true

    override fun clone(): RunConfiguration = super.clone() as ILRuntimeAttachRunConfiguration

    override fun getState(executor: Executor, environment: ExecutionEnvironment): RunProfileState? {
        if (executor.id != DefaultDebugExecutor.EXECUTOR_ID) return null

        var selectedProcess: com.jetbrains.rider.plugins.ilruntime2rider.run.ILRuntimeProcess? = null
        ApplicationManager.getApplication().invokeAndWait {
            val dialog = ILRuntimeProcessSelectionDialog(project)
            if (dialog.showAndGet()) {
                selectedProcess = dialog.selectedProcess
            }
        }
        return selectedProcess?.let { ILRuntimeAttachProfileState(this, environment, it) }
    }

    override var listenPortForConnections: Boolean = false
}
