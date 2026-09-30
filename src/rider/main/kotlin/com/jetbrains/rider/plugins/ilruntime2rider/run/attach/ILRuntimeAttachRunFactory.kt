package com.jetbrains.rider.plugins.ilruntime2rider.run.attach

import com.intellij.execution.configurations.ConfigurationType
import com.intellij.icons.AllIcons
import com.intellij.openapi.project.Project
import com.jetbrains.rider.plugins.ilruntime2rider.ILRuntimeConstants
import com.jetbrains.rider.run.configurations.DotNetConfigurationFactoryBase

open class ILRuntimeAttachRunFactory(type: ConfigurationType) :
    DotNetConfigurationFactoryBase<ILRuntimeAttachRunConfiguration>(type) {

    override fun createTemplateConfiguration(project: Project) = ILRuntimeAttachRunConfiguration(project, this)
    override fun getName() = ILRuntimeConstants.DEBUGGER_NAME
    override fun getIcon() = AllIcons.RunConfigurations.Applet
    override fun getId() = "ILRuntimeDebug"
}
