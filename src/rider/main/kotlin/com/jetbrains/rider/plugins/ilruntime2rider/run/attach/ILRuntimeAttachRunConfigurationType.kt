package com.jetbrains.rider.plugins.ilruntime2rider.run.attach

import com.intellij.execution.configurations.ConfigurationTypeBase
import com.intellij.icons.AllIcons
import com.jetbrains.rider.plugins.ilruntime2rider.ILRuntimeConstants

class ILRuntimeAttachRunConfigurationType : ConfigurationTypeBase(
    "ILRUNTIME_DEBUG_RUN_CONFIGURATION",
    ILRuntimeConstants.DEBUGGER_NAME, ILRuntimeConstants.DEBUGGER_DESCRIPTION,
    AllIcons.RunConfigurations.Application
) {
    val attachFactory = ILRuntimeAttachRunFactory(this)

    init {
        addFactory(attachFactory)
    }
}
