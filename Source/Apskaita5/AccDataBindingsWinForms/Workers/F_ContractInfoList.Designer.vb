<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Friend Class F_ContractInfoList
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(F_ContractInfoList))
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.RefreshButton = New System.Windows.Forms.Button
        Me.AddWorkersWithoutContractCheckBox = New System.Windows.Forms.CheckBox
        Me.NotOriginalDataCheckBox = New System.Windows.Forms.CheckBox
        Me.AtDateCheckBox = New System.Windows.Forms.CheckBox
        Me.ContractInfoListBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ChangeItem_MenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.DeleteItem_MenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.NewItem_MenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.NewItemUpdate_MenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ItemGeneral_MenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ContextMenuStrip2 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ChangeSubItem_MenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.DeleteSubItem_MenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ContractInfoListObjectTreeView = New BrightIdeasSoftware.TreeListView
        Me.OlvColumn5 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn4 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn1 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn2 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn3 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn6 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn7 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn8 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn9 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn10 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn11 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn12 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn13 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn14 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn15 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn16 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn17 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn18 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn19 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn20 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn21 = New BrightIdeasSoftware.OLVColumn
        Me.OlvColumn22 = New BrightIdeasSoftware.OLVColumn
        Me.ProgressFiller2 = New AccControlsWinForms.ProgressFiller
        Me.ProgressFiller1 = New AccControlsWinForms.ProgressFiller
        Me.AtDateAccDatePicker = New AccControlsWinForms.AccDatePicker
        Me.Panel1.SuspendLayout()
        CType(Me.ContractInfoListBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.ContextMenuStrip2.SuspendLayout()
        CType(Me.ContractInfoListObjectTreeView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.AutoSize = True
        Me.Panel1.Controls.Add(Me.AtDateAccDatePicker)
        Me.Panel1.Controls.Add(Me.RefreshButton)
        Me.Panel1.Controls.Add(Me.AddWorkersWithoutContractCheckBox)
        Me.Panel1.Controls.Add(Me.NotOriginalDataCheckBox)
        Me.Panel1.Controls.Add(Me.AtDateCheckBox)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(901, 54)
        Me.Panel1.TabIndex = 0
        '
        'RefreshButton
        '
        Me.RefreshButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.RefreshButton.Image = Global.AccDataBindingsWinForms.My.Resources.Resources.Button_Reload_icon_24p
        Me.RefreshButton.Location = New System.Drawing.Point(862, 8)
        Me.RefreshButton.Margin = New System.Windows.Forms.Padding(0)
        Me.RefreshButton.Name = "RefreshButton"
        Me.RefreshButton.Size = New System.Drawing.Size(33, 33)
        Me.RefreshButton.TabIndex = 4
        Me.RefreshButton.UseVisualStyleBackColor = True
        '
        'AddWorkersWithoutContractCheckBox
        '
        Me.AddWorkersWithoutContractCheckBox.AutoSize = True
        Me.AddWorkersWithoutContractCheckBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AddWorkersWithoutContractCheckBox.Location = New System.Drawing.Point(256, 34)
        Me.AddWorkersWithoutContractCheckBox.Name = "AddWorkersWithoutContractCheckBox"
        Me.AddWorkersWithoutContractCheckBox.Size = New System.Drawing.Size(239, 17)
        Me.AddWorkersWithoutContractCheckBox.TabIndex = 3
        Me.AddWorkersWithoutContractCheckBox.Text = "Įtraukti darbuotojus be darbo sutarčių"
        Me.AddWorkersWithoutContractCheckBox.UseVisualStyleBackColor = True
        '
        'NotOriginalDataCheckBox
        '
        Me.NotOriginalDataCheckBox.AutoSize = True
        Me.NotOriginalDataCheckBox.Checked = True
        Me.NotOriginalDataCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.NotOriginalDataCheckBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.NotOriginalDataCheckBox.Location = New System.Drawing.Point(256, 11)
        Me.NotOriginalDataCheckBox.Name = "NotOriginalDataCheckBox"
        Me.NotOriginalDataCheckBox.Size = New System.Drawing.Size(197, 17)
        Me.NotOriginalDataCheckBox.TabIndex = 2
        Me.NotOriginalDataCheckBox.Text = "Galiojantys sutarčių duomenys"
        Me.NotOriginalDataCheckBox.UseVisualStyleBackColor = True
        '
        'AtDateCheckBox
        '
        Me.AtDateCheckBox.AutoSize = True
        Me.AtDateCheckBox.Checked = True
        Me.AtDateCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.AtDateCheckBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AtDateCheckBox.Location = New System.Drawing.Point(9, 11)
        Me.AtDateCheckBox.Name = "AtDateCheckBox"
        Me.AtDateCheckBox.Size = New System.Drawing.Size(107, 17)
        Me.AtDateCheckBox.TabIndex = 0
        Me.AtDateCheckBox.Text = "Tik dirbantys :"
        Me.AtDateCheckBox.UseVisualStyleBackColor = True
        '
        'ContractInfoListBindingSource
        '
        Me.ContractInfoListBindingSource.DataSource = GetType(ApskaitaObjects.ActiveReports.ContractInfo)
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ChangeItem_MenuItem, Me.DeleteItem_MenuItem, Me.ToolStripSeparator1, Me.NewItem_MenuItem, Me.NewItemUpdate_MenuItem, Me.ItemGeneral_MenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(230, 120)
        '
        'ChangeItem_MenuItem
        '
        Me.ChangeItem_MenuItem.Name = "ChangeItem_MenuItem"
        Me.ChangeItem_MenuItem.Size = New System.Drawing.Size(229, 22)
        Me.ChangeItem_MenuItem.Text = "Keisti"
        '
        'DeleteItem_MenuItem
        '
        Me.DeleteItem_MenuItem.Name = "DeleteItem_MenuItem"
        Me.DeleteItem_MenuItem.Size = New System.Drawing.Size(229, 22)
        Me.DeleteItem_MenuItem.Text = "Ištrinti"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(226, 6)
        '
        'NewItem_MenuItem
        '
        Me.NewItem_MenuItem.Name = "NewItem_MenuItem"
        Me.NewItem_MenuItem.Size = New System.Drawing.Size(229, 22)
        Me.NewItem_MenuItem.Text = "Nauja sutartis"
        '
        'NewItemUpdate_MenuItem
        '
        Me.NewItemUpdate_MenuItem.Name = "NewItemUpdate_MenuItem"
        Me.NewItemUpdate_MenuItem.Size = New System.Drawing.Size(229, 22)
        Me.NewItemUpdate_MenuItem.Text = "Naujas sutarties pakeitimas"
        '
        'ItemGeneral_MenuItem
        '
        Me.ItemGeneral_MenuItem.Name = "ItemGeneral_MenuItem"
        Me.ItemGeneral_MenuItem.Size = New System.Drawing.Size(229, 22)
        Me.ItemGeneral_MenuItem.Text = "Bendri darbuotojo duomenys"
        '
        'ContextMenuStrip2
        '
        Me.ContextMenuStrip2.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ChangeSubItem_MenuItem, Me.DeleteSubItem_MenuItem})
        Me.ContextMenuStrip2.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip2.Size = New System.Drawing.Size(108, 48)
        '
        'ChangeSubItem_MenuItem
        '
        Me.ChangeSubItem_MenuItem.Name = "ChangeSubItem_MenuItem"
        Me.ChangeSubItem_MenuItem.Size = New System.Drawing.Size(107, 22)
        Me.ChangeSubItem_MenuItem.Text = "Keisti"
        '
        'DeleteSubItem_MenuItem
        '
        Me.DeleteSubItem_MenuItem.Name = "DeleteSubItem_MenuItem"
        Me.DeleteSubItem_MenuItem.Size = New System.Drawing.Size(107, 22)
        Me.DeleteSubItem_MenuItem.Text = "Ištrinti"
        '
        'ContractInfoListObjectTreeView
        '
        Me.ContractInfoListObjectTreeView.AllColumns.Add(Me.OlvColumn5)
        Me.ContractInfoListObjectTreeView.AllColumns.Add(Me.OlvColumn4)
        Me.ContractInfoListObjectTreeView.AllColumns.Add(Me.OlvColumn1)
        Me.ContractInfoListObjectTreeView.AllColumns.Add(Me.OlvColumn2)
        Me.ContractInfoListObjectTreeView.AllColumns.Add(Me.OlvColumn3)
        Me.ContractInfoListObjectTreeView.AllColumns.Add(Me.OlvColumn6)
        Me.ContractInfoListObjectTreeView.AllColumns.Add(Me.OlvColumn7)
        Me.ContractInfoListObjectTreeView.AllColumns.Add(Me.OlvColumn8)
        Me.ContractInfoListObjectTreeView.AllColumns.Add(Me.OlvColumn9)
        Me.ContractInfoListObjectTreeView.AllColumns.Add(Me.OlvColumn10)
        Me.ContractInfoListObjectTreeView.AllColumns.Add(Me.OlvColumn11)
        Me.ContractInfoListObjectTreeView.AllColumns.Add(Me.OlvColumn12)
        Me.ContractInfoListObjectTreeView.AllColumns.Add(Me.OlvColumn13)
        Me.ContractInfoListObjectTreeView.AllColumns.Add(Me.OlvColumn14)
        Me.ContractInfoListObjectTreeView.AllColumns.Add(Me.OlvColumn15)
        Me.ContractInfoListObjectTreeView.AllColumns.Add(Me.OlvColumn16)
        Me.ContractInfoListObjectTreeView.AllColumns.Add(Me.OlvColumn17)
        Me.ContractInfoListObjectTreeView.AllColumns.Add(Me.OlvColumn18)
        Me.ContractInfoListObjectTreeView.AllColumns.Add(Me.OlvColumn19)
        Me.ContractInfoListObjectTreeView.AllColumns.Add(Me.OlvColumn20)
        Me.ContractInfoListObjectTreeView.AllColumns.Add(Me.OlvColumn21)
        Me.ContractInfoListObjectTreeView.AllColumns.Add(Me.OlvColumn22)
        Me.ContractInfoListObjectTreeView.AllowColumnReorder = True
        Me.ContractInfoListObjectTreeView.CausesValidation = False
        Me.ContractInfoListObjectTreeView.CellEditUseWholeCell = False
        Me.ContractInfoListObjectTreeView.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.OlvColumn5, Me.OlvColumn9, Me.OlvColumn10, Me.OlvColumn11, Me.OlvColumn12, Me.OlvColumn14, Me.OlvColumn15, Me.OlvColumn16, Me.OlvColumn17, Me.OlvColumn18, Me.OlvColumn19, Me.OlvColumn20, Me.OlvColumn21, Me.OlvColumn22})
        Me.ContractInfoListObjectTreeView.Cursor = System.Windows.Forms.Cursors.Default
        Me.ContractInfoListObjectTreeView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ContractInfoListObjectTreeView.HasCollapsibleGroups = False
        Me.ContractInfoListObjectTreeView.HeaderWordWrap = True
        Me.ContractInfoListObjectTreeView.HideSelection = False
        Me.ContractInfoListObjectTreeView.IncludeColumnHeadersInCopy = True
        Me.ContractInfoListObjectTreeView.Location = New System.Drawing.Point(0, 54)
        Me.ContractInfoListObjectTreeView.Name = "ContractInfoListObjectTreeView"
        Me.ContractInfoListObjectTreeView.RenderNonEditableCheckboxesAsDisabled = True
        Me.ContractInfoListObjectTreeView.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu
        Me.ContractInfoListObjectTreeView.ShowCommandMenuOnRightClick = True
        Me.ContractInfoListObjectTreeView.ShowGroups = False
        Me.ContractInfoListObjectTreeView.ShowItemToolTips = True
        Me.ContractInfoListObjectTreeView.Size = New System.Drawing.Size(901, 492)
        Me.ContractInfoListObjectTreeView.TabIndex = 2
        Me.ContractInfoListObjectTreeView.UseCompatibleStateImageBehavior = False
        Me.ContractInfoListObjectTreeView.UseFilterIndicator = True
        Me.ContractInfoListObjectTreeView.UseFiltering = True
        Me.ContractInfoListObjectTreeView.UseHotItem = True
        Me.ContractInfoListObjectTreeView.UseNotifyPropertyChanged = True
        Me.ContractInfoListObjectTreeView.UseTranslucentHotItem = True
        Me.ContractInfoListObjectTreeView.View = System.Windows.Forms.View.Details
        Me.ContractInfoListObjectTreeView.VirtualMode = True
        '
        'OlvColumn5
        '
        Me.OlvColumn5.AspectName = "PersonName"
        Me.OlvColumn5.CellEditUseWholeCell = True
        Me.OlvColumn5.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn5.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn5.IsEditable = False
        Me.OlvColumn5.Text = "Darbuotojas"
        Me.OlvColumn5.ToolTipText = ""
        Me.OlvColumn5.Width = 150
        '
        'OlvColumn4
        '
        Me.OlvColumn4.AspectName = "PersonID"
        Me.OlvColumn4.CellEditUseWholeCell = True
        Me.OlvColumn4.DisplayIndex = 1
        Me.OlvColumn4.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn4.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn4.IsEditable = False
        Me.OlvColumn4.IsVisible = False
        Me.OlvColumn4.Text = "Asmens ID"
        Me.OlvColumn4.ToolTipText = ""
        Me.OlvColumn4.Width = 100
        '
        'OlvColumn1
        '
        Me.OlvColumn1.AspectName = "ID"
        Me.OlvColumn1.CellEditUseWholeCell = True
        Me.OlvColumn1.DisplayIndex = 2
        Me.OlvColumn1.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn1.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn1.IsEditable = False
        Me.OlvColumn1.IsVisible = False
        Me.OlvColumn1.Text = "ID"
        Me.OlvColumn1.ToolTipText = ""
        Me.OlvColumn1.Width = 100
        '
        'OlvColumn2
        '
        Me.OlvColumn2.AspectName = "InsertDate"
        Me.OlvColumn2.CellEditUseWholeCell = True
        Me.OlvColumn2.DisplayIndex = 3
        Me.OlvColumn2.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn2.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn2.IsEditable = False
        Me.OlvColumn2.IsVisible = False
        Me.OlvColumn2.Text = "Įtraukta DB"
        Me.OlvColumn2.ToolTipText = ""
        Me.OlvColumn2.Width = 100
        '
        'OlvColumn3
        '
        Me.OlvColumn3.AspectName = "UpdateDate"
        Me.OlvColumn3.CellEditUseWholeCell = True
        Me.OlvColumn3.DisplayIndex = 4
        Me.OlvColumn3.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn3.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn3.IsEditable = False
        Me.OlvColumn3.IsVisible = False
        Me.OlvColumn3.Text = "Pakeista DB"
        Me.OlvColumn3.ToolTipText = ""
        Me.OlvColumn3.Width = 100
        '
        'OlvColumn6
        '
        Me.OlvColumn6.AspectName = "PersonCode"
        Me.OlvColumn6.CellEditUseWholeCell = True
        Me.OlvColumn6.DisplayIndex = 5
        Me.OlvColumn6.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn6.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn6.IsEditable = False
        Me.OlvColumn6.IsVisible = False
        Me.OlvColumn6.Text = "Asmens Kodas"
        Me.OlvColumn6.ToolTipText = ""
        Me.OlvColumn6.Width = 100
        '
        'OlvColumn7
        '
        Me.OlvColumn7.AspectName = "PersonSodraCode"
        Me.OlvColumn7.CellEditUseWholeCell = True
        Me.OlvColumn7.DisplayIndex = 6
        Me.OlvColumn7.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn7.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn7.IsEditable = False
        Me.OlvColumn7.IsVisible = False
        Me.OlvColumn7.Text = "Sodros Kodas"
        Me.OlvColumn7.ToolTipText = ""
        Me.OlvColumn7.Width = 100
        '
        'OlvColumn8
        '
        Me.OlvColumn8.AspectName = "PersonAddress"
        Me.OlvColumn8.CellEditUseWholeCell = True
        Me.OlvColumn8.DisplayIndex = 7
        Me.OlvColumn8.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn8.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn8.IsEditable = False
        Me.OlvColumn8.IsVisible = False
        Me.OlvColumn8.Text = "Adresas"
        Me.OlvColumn8.ToolTipText = ""
        Me.OlvColumn8.Width = 100
        '
        'OlvColumn9
        '
        Me.OlvColumn9.AspectName = "Serial"
        Me.OlvColumn9.CellEditUseWholeCell = True
        Me.OlvColumn9.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn9.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn9.IsEditable = False
        Me.OlvColumn9.Text = "Serija"
        Me.OlvColumn9.ToolTipText = ""
        Me.OlvColumn9.Width = 43
        '
        'OlvColumn10
        '
        Me.OlvColumn10.AspectName = "Number"
        Me.OlvColumn10.CellEditUseWholeCell = True
        Me.OlvColumn10.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn10.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn10.IsEditable = False
        Me.OlvColumn10.Text = "Numeris"
        Me.OlvColumn10.ToolTipText = ""
        Me.OlvColumn10.Width = 57
        '
        'OlvColumn11
        '
        Me.OlvColumn11.AspectName = "Date"
        Me.OlvColumn11.AspectToStringFormat = "{0:yyyy-MM-dd}"
        Me.OlvColumn11.CellEditUseWholeCell = True
        Me.OlvColumn11.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn11.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn11.IsEditable = False
        Me.OlvColumn11.Text = "Data"
        Me.OlvColumn11.ToolTipText = ""
        Me.OlvColumn11.Width = 82
        '
        'OlvColumn12
        '
        Me.OlvColumn12.AspectName = "DateTermination"
        Me.OlvColumn12.CellEditUseWholeCell = True
        Me.OlvColumn12.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn12.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn12.IsEditable = False
        Me.OlvColumn12.Text = "Nutraukimo Data"
        Me.OlvColumn12.ToolTipText = ""
        Me.OlvColumn12.Width = 92
        '
        'OlvColumn13
        '
        Me.OlvColumn13.AspectName = "Content"
        Me.OlvColumn13.CellEditUseWholeCell = True
        Me.OlvColumn13.DisplayIndex = 12
        Me.OlvColumn13.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn13.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn13.IsEditable = False
        Me.OlvColumn13.IsVisible = False
        Me.OlvColumn13.Text = "Turinys"
        Me.OlvColumn13.ToolTipText = ""
        Me.OlvColumn13.Width = 100
        '
        'OlvColumn14
        '
        Me.OlvColumn14.AspectName = "Position"
        Me.OlvColumn14.CellEditUseWholeCell = True
        Me.OlvColumn14.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn14.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn14.IsEditable = False
        Me.OlvColumn14.Text = "Pareigos"
        Me.OlvColumn14.ToolTipText = ""
        Me.OlvColumn14.Width = 100
        '
        'OlvColumn15
        '
        Me.OlvColumn15.AspectName = "WorkLoad"
        Me.OlvColumn15.CellEditUseWholeCell = True
        Me.OlvColumn15.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn15.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn15.IsEditable = False
        Me.OlvColumn15.Text = "Krūvis"
        Me.OlvColumn15.ToolTipText = ""
        Me.OlvColumn15.Width = 54
        '
        'OlvColumn16
        '
        Me.OlvColumn16.AspectName = "Wage"
        Me.OlvColumn16.CellEditUseWholeCell = True
        Me.OlvColumn16.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn16.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn16.IsEditable = False
        Me.OlvColumn16.Text = "Darbo Užmokestis"
        Me.OlvColumn16.ToolTipText = ""
        Me.OlvColumn16.Width = 98
        '
        'OlvColumn17
        '
        Me.OlvColumn17.AspectName = "WageTypeHumanReadable"
        Me.OlvColumn17.CellEditUseWholeCell = True
        Me.OlvColumn17.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn17.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn17.IsEditable = False
        Me.OlvColumn17.Text = "DU Tipas"
        Me.OlvColumn17.ToolTipText = ""
        Me.OlvColumn17.Width = 77
        '
        'OlvColumn18
        '
        Me.OlvColumn18.AspectName = "ExtraPay"
        Me.OlvColumn18.CellEditUseWholeCell = True
        Me.OlvColumn18.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn18.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn18.IsEditable = False
        Me.OlvColumn18.Text = "Priedas"
        Me.OlvColumn18.ToolTipText = ""
        Me.OlvColumn18.Width = 76
        '
        'OlvColumn19
        '
        Me.OlvColumn19.AspectName = "NPD"
        Me.OlvColumn19.CellEditUseWholeCell = True
        Me.OlvColumn19.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn19.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn19.IsEditable = False
        Me.OlvColumn19.Text = "NPD"
        Me.OlvColumn19.ToolTipText = ""
        Me.OlvColumn19.Width = 100
        '
        'OlvColumn20
        '
        Me.OlvColumn20.AspectName = "PNPD"
        Me.OlvColumn20.CellEditUseWholeCell = True
        Me.OlvColumn20.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn20.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn20.IsEditable = False
        Me.OlvColumn20.Text = "PNPD"
        Me.OlvColumn20.ToolTipText = ""
        Me.OlvColumn20.Width = 100
        '
        'OlvColumn21
        '
        Me.OlvColumn21.AspectName = "AnnualHoliday"
        Me.OlvColumn21.CellEditUseWholeCell = True
        Me.OlvColumn21.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn21.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn21.IsEditable = False
        Me.OlvColumn21.Text = "Kasmet. Atostogos"
        Me.OlvColumn21.ToolTipText = ""
        Me.OlvColumn21.Width = 100
        '
        'OlvColumn22
        '
        Me.OlvColumn22.AspectName = "HolidayCorrection"
        Me.OlvColumn22.CellEditUseWholeCell = True
        Me.OlvColumn22.HeaderFont = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.OlvColumn22.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.OlvColumn22.IsEditable = False
        Me.OlvColumn22.Text = "Atostogų Korekcija"
        Me.OlvColumn22.ToolTipText = ""
        Me.OlvColumn22.Width = 100
        '
        'ProgressFiller2
        '
        Me.ProgressFiller2.Location = New System.Drawing.Point(187, 81)
        Me.ProgressFiller2.Name = "ProgressFiller2"
        Me.ProgressFiller2.Size = New System.Drawing.Size(168, 47)
        Me.ProgressFiller2.TabIndex = 6
        Me.ProgressFiller2.Visible = False
        '
        'ProgressFiller1
        '
        Me.ProgressFiller1.Location = New System.Drawing.Point(395, 67)
        Me.ProgressFiller1.Name = "ProgressFiller1"
        Me.ProgressFiller1.Size = New System.Drawing.Size(175, 61)
        Me.ProgressFiller1.TabIndex = 5
        Me.ProgressFiller1.Visible = False
        '
        'AtDateAccDatePicker
        '
        Me.AtDateAccDatePicker.BoldedDates = Nothing
        Me.AtDateAccDatePicker.Location = New System.Drawing.Point(111, 8)
        Me.AtDateAccDatePicker.MaxDate = New Date(9998, 12, 31, 0, 0, 0, 0)
        Me.AtDateAccDatePicker.MinDate = New Date(1753, 1, 1, 0, 0, 0, 0)
        Me.AtDateAccDatePicker.Name = "AtDateAccDatePicker"
        Me.AtDateAccDatePicker.ReadOnly = False
        Me.AtDateAccDatePicker.ShowWeekNumbers = True
        Me.AtDateAccDatePicker.Size = New System.Drawing.Size(129, 20)
        Me.AtDateAccDatePicker.TabIndex = 1
        '
        'F_ContractInfoList
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(901, 546)
        Me.Controls.Add(Me.ContractInfoListObjectTreeView)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.ProgressFiller2)
        Me.Controls.Add(Me.ProgressFiller1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "F_ContractInfoList"
        Me.ShowInTaskbar = False
        Me.Text = "Darbo sutarčių registras"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.ContractInfoListBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ContextMenuStrip2.ResumeLayout(False)
        CType(Me.ContractInfoListObjectTreeView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents AtDateCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents NotOriginalDataCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents AddWorkersWithoutContractCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents RefreshButton As System.Windows.Forms.Button
    Friend WithEvents ContractInfoListBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ChangeItem_MenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DeleteItem_MenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NewItem_MenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NewItemUpdate_MenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ItemGeneral_MenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ContextMenuStrip2 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ChangeSubItem_MenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DeleteSubItem_MenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents ContractInfoListObjectTreeView As BrightIdeasSoftware.TreeListView
    Friend WithEvents OlvColumn1 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn2 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn3 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn4 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn5 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn6 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn7 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn8 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn9 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn10 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn11 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn12 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn13 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn14 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn15 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn16 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn17 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn18 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn19 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn20 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn21 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents OlvColumn22 As BrightIdeasSoftware.OLVColumn
    Friend WithEvents ProgressFiller2 As AccControlsWinForms.ProgressFiller
    Friend WithEvents ProgressFiller1 As AccControlsWinForms.ProgressFiller
    Friend WithEvents AtDateAccDatePicker As AccControlsWinForms.AccDatePicker
End Class
