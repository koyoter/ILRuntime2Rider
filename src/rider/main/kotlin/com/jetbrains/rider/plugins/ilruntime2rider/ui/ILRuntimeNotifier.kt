package com.jetbrains.rider.plugins.ilruntime2rider.ui

import com.intellij.notification.NotificationGroupManager
import com.intellij.notification.NotificationType
import com.intellij.notification.Notifications
import com.intellij.openapi.project.Project

object ILRuntimeNotifier {
    private const val NOTIFICATION_GROUP_ID = "ILRuntime2Rider.Notifications"

    fun showToast(project: Project?, title: String, content: String, type: NotificationType = NotificationType.INFORMATION) {
        val notification = NotificationGroupManager.getInstance()
            .getNotificationGroup(NOTIFICATION_GROUP_ID)
            .createNotification(title, content, type)
        Notifications.Bus.notify(notification, project)
    }
}
