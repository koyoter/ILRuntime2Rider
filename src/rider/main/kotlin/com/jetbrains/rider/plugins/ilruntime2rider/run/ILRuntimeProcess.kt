package com.jetbrains.rider.plugins.ilruntime2rider.run

/** An ILRuntime host discovered via UDP broadcast. */
data class ILRuntimeProcess(
    val projectName: String,
    val machineName: String,
    val processId: Int,
    val port: Int,
    val ip: String
)
