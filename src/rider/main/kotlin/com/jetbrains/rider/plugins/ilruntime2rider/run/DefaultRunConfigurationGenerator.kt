package com.jetbrains.rider.plugins.ilruntime2rider.run

import com.intellij.execution.RunManager
import com.intellij.execution.configurations.ConfigurationTypeUtil
import com.intellij.execution.configurations.UnknownConfigurationType
import com.intellij.openapi.project.Project
import com.jetbrains.rider.plugins.ilruntime2rider.ILRuntimeConstants
import com.jetbrains.rider.plugins.ilruntime2rider.run.attach.ILRuntimeAttachRunFactory
import com.jetbrains.rider.plugins.ilruntime2rider.run.attach.ILRuntimeAttachRunConfigurationType

class DefaultRunConfigurationGenerator(project: Project) {
    init {
        val runManager = RunManager.getInstance(project)
        val configurationName = ILRuntimeConstants.DEBUGGER_NAME

        val staleSettings = runManager.allSettings.filter {
            it.type is UnknownConfigurationType && it.name == configurationName
        }
        for (setting in staleSettings) {
            runManager.removeConfiguration(setting)
        }

        if (!runManager.allSettings.any {
                it.type is ILRuntimeAttachRunConfigurationType && it.factory is ILRuntimeAttachRunFactory
            }) {
            val configurationType = ConfigurationTypeUtil.findConfigurationType(ILRuntimeAttachRunConfigurationType::class.java)
            val runConfiguration = runManager.createRunConfiguration(
                configurationName,
                configurationType.attachFactory
            )
            runManager.addConfiguration(runConfiguration, false)
        }

        // Select the attach configuration if nothing is selected
        if (runManager.selectedConfiguration == null) {
            val runConfiguration = runManager.findConfigurationByName(configurationName)
            if (runConfiguration != null) {
                runManager.selectedConfiguration = runConfiguration
            }
        }
    }
}
