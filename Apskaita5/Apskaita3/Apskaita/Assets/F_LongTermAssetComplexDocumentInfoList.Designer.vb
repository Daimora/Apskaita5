Imports ApskaitaObjects.ActiveReports

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class F_LongTermAssetComplexDocumentInfoList
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(F_LongTermAssetComplexDocumentInfoList))
        Me.DateToDateTimePicker = New System.Windows.Forms.DateTimePicker
        Me.Label1 = New System.Windows.Forms.Label
        Me.RefreshButton = New System.Windows.Forms.Button
        Me.DateFromDateTimePicker = New System.Windows.Forms.DateTimePicker
        Me.Label10 = New System.Windows.Forms.Label
        Me.LongTermAssetComplexDocumentInfoListBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.LongTermAssetComplexDocumentInfoListDataGridView = New System.Windows.Forms.DataGridView
        Me.DataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.DataGridViewTextBoxColumn3 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.DataGridViewTextBoxColumn2 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.DataGridViewTextBoxColumn4 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.DataGridViewTextBoxColumn8 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.DataGridViewTextBoxColumn6 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.DataGridViewTextBoxColumn7 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ChangeItem_MenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.DeleteItem_MenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.NewItem_MenuItem = New System.Windows.Forms.ToolStripMenuItem
        CType(Me.LongTermAssetComplexDocumentInfoListBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LongTermAssetComplexDocumentInfoListDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'DateToDateTimePicker
        '
        Me.DateToDateTimePicker.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DateToDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DateToDateTimePicker.Location = New System.Drawing.Point(288, 3)
        Me.DateToDateTimePicker.Name = "DateToDateTimePicker"
        Me.DateToDateTimePicker.Size = New System.Drawing.Size(188, 20)
        Me.DateToDateTimePicker.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(257, 6)
        Me.Label1.Margin = New System.Windows.Forms.Padding(3, 6, 3, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(25, 13)
        Me.Label1.TabIndex = 81
        Me.Label1.Text = "Iki:"
        '
        'RefreshButton
        '
        Me.RefreshButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.RefreshButton.Image = Global.ApskaitaWUI.My.Resources.Resources.Button_Reload_icon_24p
        Me.RefreshButton.Location = New System.Drawing.Point(499, 0)
        Me.RefreshButton.Margin = New System.Windows.Forms.Padding(0, 0, 0, 3)
        Me.RefreshButton.Name = "RefreshButton"
        Me.RefreshButton.Size = New System.Drawing.Size(35, 32)
        Me.RefreshButton.TabIndex = 2
        Me.RefreshButton.UseVisualStyleBackColor = True
        '
        'DateFromDateTimePicker
        '
        Me.DateFromDateTimePicker.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DateFromDateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.DateFromDateTimePicker.Location = New System.Drawing.Point(43, 3)
        Me.DateFromDateTimePicker.Name = "DateFromDateTimePicker"
        Me.DateFromDateTimePicker.Size = New System.Drawing.Size(188, 20)
        Me.DateFromDateTimePicker.TabIndex = 0
        '
        'Label10
        '
        Me.Label10.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.Location = New System.Drawing.Point(3, 6)
        Me.Label10.Margin = New System.Windows.Forms.Padding(3, 6, 3, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(34, 13)
        Me.Label10.TabIndex = 78
        Me.Label10.Text = "Nuo:"
        '
        'LongTermAssetComplexDocumentInfoListBindingSource
        '
        Me.LongTermAssetComplexDocumentInfoListBindingSource.DataSource = GetType(ApskaitaObjects.ActiveReports.LongTermAssetComplexDocumentInfo)
        '
        'LongTermAssetComplexDocumentInfoListDataGridView
        '
        Me.LongTermAssetComplexDocumentInfoListDataGridView.AllowUserToAddRows = False
        Me.LongTermAssetComplexDocumentInfoListDataGridView.AllowUserToDeleteRows = False
        Me.LongTermAssetComplexDocumentInfoListDataGridView.AllowUserToOrderColumns = True
        Me.LongTermAssetComplexDocumentInfoListDataGridView.AutoGenerateColumns = False
        Me.LongTermAssetComplexDocumentInfoListDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.LongTermAssetComplexDocumentInfoListDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.LongTermAssetComplexDocumentInfoListDataGridView.CausesValidation = False
        Me.LongTermAssetComplexDocumentInfoListDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn1, Me.DataGridViewTextBoxColumn3, Me.DataGridViewTextBoxColumn2, Me.DataGridViewTextBoxColumn4, Me.DataGridViewTextBoxColumn8, Me.DataGridViewTextBoxColumn6, Me.DataGridViewTextBoxColumn7})
        Me.LongTermAssetComplexDocumentInfoListDataGridView.DataSource = Me.LongTermAssetComplexDocumentInfoListBindingSource
        Me.LongTermAssetComplexDocumentInfoListDataGridView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LongTermAssetComplexDocumentInfoListDataGridView.Location = New System.Drawing.Point(0, 35)
        Me.LongTermAssetComplexDocumentInfoListDataGridView.Name = "LongTermAssetComplexDocumentInfoListDataGridView"
        Me.LongTermAssetComplexDocumentInfoListDataGridView.ReadOnly = True
        Me.LongTermAssetComplexDocumentInfoListDataGridView.RowHeadersVisible = False
        Me.LongTermAssetComplexDocumentInfoListDataGridView.Size = New System.Drawing.Size(750, 469)
        Me.LongTermAssetComplexDocumentInfoListDataGridView.TabIndex = 1
        '
        'DataGridViewTextBoxColumn1
        '
        Me.DataGridViewTextBoxColumn1.DataPropertyName = "ID"
        Me.DataGridViewTextBoxColumn1.HeaderText = "ID"
        Me.DataGridViewTextBoxColumn1.Name = "DataGridViewTextBoxColumn1"
        Me.DataGridViewTextBoxColumn1.ReadOnly = True
        Me.DataGridViewTextBoxColumn1.Visible = False
        Me.DataGridViewTextBoxColumn1.Width = 43
        '
        'DataGridViewTextBoxColumn3
        '
        Me.DataGridViewTextBoxColumn3.DataPropertyName = "Date"
        DataGridViewCellStyle1.Format = "d"
        DataGridViewCellStyle1.NullValue = Nothing
        Me.DataGridViewTextBoxColumn3.DefaultCellStyle = DataGridViewCellStyle1
        Me.DataGridViewTextBoxColumn3.HeaderText = "Data"
        Me.DataGridViewTextBoxColumn3.Name = "DataGridViewTextBoxColumn3"
        Me.DataGridViewTextBoxColumn3.ReadOnly = True
        Me.DataGridViewTextBoxColumn3.Width = 55
        '
        'DataGridViewTextBoxColumn2
        '
        Me.DataGridViewTextBoxColumn2.DataPropertyName = "OperationType"
        Me.DataGridViewTextBoxColumn2.HeaderText = "Tipas"
        Me.DataGridViewTextBoxColumn2.Name = "DataGridViewTextBoxColumn2"
        Me.DataGridViewTextBoxColumn2.ReadOnly = True
        Me.DataGridViewTextBoxColumn2.Width = 58
        '
        'DataGridViewTextBoxColumn4
        '
        Me.DataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.DataGridViewTextBoxColumn4.DataPropertyName = "Content"
        Me.DataGridViewTextBoxColumn4.HeaderText = "Turinys (aprašas)"
        Me.DataGridViewTextBoxColumn4.Name = "DataGridViewTextBoxColumn4"
        Me.DataGridViewTextBoxColumn4.ReadOnly = True
        '
        'DataGridViewTextBoxColumn8
        '
        Me.DataGridViewTextBoxColumn8.DataPropertyName = "CorrespondingAccount"
        Me.DataGridViewTextBoxColumn8.HeaderText = "Koresp. sąsk."
        Me.DataGridViewTextBoxColumn8.Name = "DataGridViewTextBoxColumn8"
        Me.DataGridViewTextBoxColumn8.ReadOnly = True
        Me.DataGridViewTextBoxColumn8.Width = 96
        '
        'DataGridViewTextBoxColumn6
        '
        Me.DataGridViewTextBoxColumn6.DataPropertyName = "AttachedJournalEntryID"
        Me.DataGridViewTextBoxColumn6.HeaderText = "Susietos BŽ ID"
        Me.DataGridViewTextBoxColumn6.Name = "DataGridViewTextBoxColumn6"
        Me.DataGridViewTextBoxColumn6.ReadOnly = True
        Me.DataGridViewTextBoxColumn6.Visible = False
        Me.DataGridViewTextBoxColumn6.Width = 103
        '
        'DataGridViewTextBoxColumn7
        '
        Me.DataGridViewTextBoxColumn7.DataPropertyName = "AttachedJournalEntry"
        Me.DataGridViewTextBoxColumn7.HeaderText = "Susietas BŽ"
        Me.DataGridViewTextBoxColumn7.Name = "DataGridViewTextBoxColumn7"
        Me.DataGridViewTextBoxColumn7.ReadOnly = True
        Me.DataGridViewTextBoxColumn7.Width = 89
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.AutoSize = True
        Me.TableLayoutPanel1.ColumnCount = 9
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle)
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33444!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle)
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33444!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle)
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33112!))
        Me.TableLayoutPanel1.Controls.Add(Me.Label10, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.DateFromDateTimePicker, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.RefreshButton, 6, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.DateToDateTimePicker, 4, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 3, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(750, 35)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ChangeItem_MenuItem, Me.DeleteItem_MenuItem, Me.NewItem_MenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(111, 70)
        '
        'ChangeItem_MenuItem
        '
        Me.ChangeItem_MenuItem.Name = "ChangeItem_MenuItem"
        Me.ChangeItem_MenuItem.Size = New System.Drawing.Size(110, 22)
        Me.ChangeItem_MenuItem.Text = "Keisti"
        '
        'DeleteItem_MenuItem
        '
        Me.DeleteItem_MenuItem.Name = "DeleteItem_MenuItem"
        Me.DeleteItem_MenuItem.Size = New System.Drawing.Size(110, 22)
        Me.DeleteItem_MenuItem.Text = "Ištrinti"
        '
        'NewItem_MenuItem
        '
        Me.NewItem_MenuItem.Name = "NewItem_MenuItem"
        Me.NewItem_MenuItem.Size = New System.Drawing.Size(110, 22)
        Me.NewItem_MenuItem.Text = "Naujas"
        '
        'F_LongTermAssetComplexDocumentInfoList
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(750, 504)
        Me.Controls.Add(Me.LongTermAssetComplexDocumentInfoListDataGridView)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "F_LongTermAssetComplexDocumentInfoList"
        Me.ShowInTaskbar = False
        Me.Text = "Kompleksiniai operacijų su ilgalaikiu turtu dokumentai"
        CType(Me.LongTermAssetComplexDocumentInfoListBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LongTermAssetComplexDocumentInfoListDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents DateToDateTimePicker As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents RefreshButton As System.Windows.Forms.Button
    Friend WithEvents DateFromDateTimePicker As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents LongTermAssetComplexDocumentInfoListBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents LongTermAssetComplexDocumentInfoListDataGridView As System.Windows.Forms.DataGridView
    Friend WithEvents DataGridViewTextBoxColumn1 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn3 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn2 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn4 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn5 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn8 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn6 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn7 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ChangeItem_MenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DeleteItem_MenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NewItem_MenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
