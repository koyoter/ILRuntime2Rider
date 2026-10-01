package com.jetbrains.rider.plugins.ilruntime2rider.ui

import com.intellij.notification.NotificationType
import com.intellij.openapi.project.Project
import com.intellij.openapi.rd.defineNestedLifetime
import com.intellij.openapi.ui.DialogWrapper
import com.intellij.ui.JBColor
import com.intellij.ui.PortField
import com.intellij.ui.components.JBLabel
import com.intellij.ui.components.JBScrollPane
import com.intellij.ui.table.JBTable
import com.intellij.util.ui.ColumnInfo
import com.intellij.util.ui.ListTableModel
import com.jetbrains.rd.util.lifetime.Lifetime
import com.jetbrains.rd.util.reactive.Property
import com.jetbrains.rider.plugins.ilruntime2rider.ILRuntimeConstants
import com.jetbrains.rider.plugins.ilruntime2rider.run.ILRuntimeProcess
import com.jetbrains.rider.plugins.ilruntime2rider.run.ILRuntimeProcessFinder
import java.awt.Component
import java.awt.Dimension
import java.awt.FlowLayout
import java.util.Comparator
import javax.swing.JButton
import javax.swing.JComponent
import javax.swing.JPanel
import javax.swing.ListSelectionModel
import javax.swing.table.TableCellRenderer

class ILRuntimeProcessSelectionDialog(project: Project) : DialogWrapper(project) {

    private var processTable: JBTable? = null
    private val processes = ArrayList<ILRuntimeProcess>(64)
    private val tableModel: ListTableModel<ILRuntimeProcess>
    private val hasChanged = Property(false)

    var selectedProcess: ILRuntimeProcess? = null
        private set

    init {
        title = ILRuntimeConstants.DEBUGGER_NAME
        ILRuntimeProcessFinder.removeStale(ILRuntimeConstants.PROCESS_STALE_ON_OPEN_MS.toLong())
        val dialogLifetime = disposable.defineNestedLifetime()
        tableModel = createTableModel()

        init()

        hasChanged.set(true)
        ILRuntimeProcessFinder.onError.view(dialogLifetime) { _, errorText ->
            if (errorText.isNotEmpty()) {
                ILRuntimeNotifier.showToast(project, "Error", errorText, NotificationType.ERROR)
            }
        }
        ILRuntimeProcessFinder.processes.view(dialogLifetime) { elementLifetime, _ ->
            hasChanged.set(true)
            elementLifetime.onTermination { hasChanged.set(true) }
        }
        hasChanged.view(dialogLifetime) { _, changed ->
            if (!changed) return@view
            processes.clear()
            processes.addAll(ILRuntimeProcessFinder.processes)
            processes.sortWith(
                compareByDescending<ILRuntimeProcess> { ILRuntimeConstants.isLocalAddress(it.ip) }
                    .thenComparing({ it.projectName }, String.CASE_INSENSITIVE_ORDER)
                    .thenComparingInt { it.processId }
            )
            tableModel.items = processes
            hasChanged.set(false)
        }
    }

    private fun createTableModel(): ListTableModel<ILRuntimeProcess> {
        val projectNameCol = object : ColumnInfo<ILRuntimeProcess, String>("Project Name") {
            override fun valueOf(process: ILRuntimeProcess) = process.projectName
        }
        val machineNameCol = object : ColumnInfo<ILRuntimeProcess, String>("Machine Name") {
            override fun valueOf(process: ILRuntimeProcess) = process.machineName
        }
        val processIdCol = object : ColumnInfo<ILRuntimeProcess, Int>("Process ID") {
            override fun valueOf(process: ILRuntimeProcess) = process.processId
            override fun getComparator() = Comparator.comparingInt<ILRuntimeProcess> { it.processId }
        }
        val portCol = object : ColumnInfo<ILRuntimeProcess, Int>("Port") {
            override fun valueOf(process: ILRuntimeProcess) = process.port
            override fun getComparator() = Comparator.comparingInt<ILRuntimeProcess> { it.port }
        }
        val ipCol = object : ColumnInfo<ILRuntimeProcess, String>("IP Address") {
            override fun valueOf(process: ILRuntimeProcess) = process.ip
        }
        return ListTableModel(projectNameCol, ipCol, portCol, machineNameCol, processIdCol)
    }

    override fun createNorthPanel(): JComponent {
        val topPanel = JPanel(FlowLayout(FlowLayout.LEFT))
        val portField = PortField(ILRuntimeProcessFinder.port)
        val refreshButton = JButton("Refresh")

        refreshButton.addActionListener {
            processes.clear()
            tableModel.items = processes
            ILRuntimeProcessFinder.listen(portField.value as? Int ?: ILRuntimeConstants.PORT)
        }

        topPanel.add(JBLabel("Port:"))
        topPanel.add(portField)
        topPanel.add(refreshButton)
        return topPanel
    }

    override fun doOKAction() {
        val row = processTable?.selectedRow ?: -1
        if (row >= 0) {
            selectedProcess = tableModel.getItem(processTable!!.convertRowIndexToModel(row))
        }
        super.doOKAction()
    }

    override fun createCenterPanel(): JComponent {
        val table = object : JBTable(tableModel) {
            override fun prepareRenderer(renderer: TableCellRenderer, row: Int, column: Int): Component {
                val component = super.prepareRenderer(renderer, row, column)
                if (!isRowSelected(row)) {
                    val item = tableModel.getItem(convertRowIndexToModel(row))
                    component.foreground =
                        if (item == ILRuntimeProcessFinder.attachedProcess) JBColor.GREEN else this.foreground
                }
                return component
            }
        }
        table.setSelectionMode(ListSelectionModel.SINGLE_SELECTION)
        table.emptyText.text = "No ILRuntime processes discovered"
        // OK stays disabled until a process is selected.
        okAction.isEnabled = false
        table.selectionModel.addListSelectionListener {
            okAction.isEnabled = table.selectedRow >= 0
        }
        processTable = table
        val scrollPane = JBScrollPane(table)
        scrollPane.preferredSize = Dimension(600, 300)
        return scrollPane
    }
}
