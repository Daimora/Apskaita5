<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class F_Accounts
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
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(F_Accounts))
        Me.Panel2 = New System.Windows.Forms.Panel
        Me.CopyToClipboardButton = New System.Windows.Forms.Button
        Me.SaveFileButton = New System.Windows.Forms.Button
        Me.OpenFileAccButton = New AccControls.AccButton
        Me.PasteAccButton = New AccControls.AccButton
        Me.nCancelButton = New System.Windows.Forms.Button
        Me.ApplyButton = New System.Windows.Forms.Button
        Me.nOkButton = New System.Windows.Forms.Button
        Me.AccountListBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.AccountListDataGridView = New System.Windows.Forms.DataGridView
        Me.DataGridViewTextBoxColumn4 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.DataGridViewTextBoxColumn1 = New AccControls.DataGridViewAccTextBoxColumn
        Me.DataGridViewTextBoxColumn2 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.DataGridViewTextBoxColumn3 = New AccControls.DataGridViewAccGridComboBoxColumn
        Me.Panel2.SuspendLayout()
        CType(Me.AccountListBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.AccountListDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel2
        '
        Me.Panel2.AutoSize = True
        Me.Panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Panel2.Controls.Add(Me.CopyToClipboardButton)
        Me.Panel2.Controls.Add(Me.SaveFileButton)
        Me.Panel2.Controls.Add(Me.OpenFileAccButton)
        Me.Panel2.Controls.Add(Me.PasteAccButton)
        Me.Panel2.Controls.Add(Me.nCancelButton)
        Me.Panel2.Controls.Add(Me.ApplyButton)
        Me.Panel2.Controls.Add(Me.nOkButton)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel2.Location = New System.Drawing.Point(0, 428)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(718, 39)
        Me.Panel2.TabIndex = 2
        '
        'CopyToClipboardButton
        '
        Me.CopyToClipboardButton.Image = Global.ApskaitaWUI.My.Resources.Resources.Actions_edit_copy_icon_16p
        Me.CopyToClipboardButton.Location = New System.Drawing.Point(12, 6)
        Me.CopyToClipboardButton.Name = "CopyToClipboardButton"
        Me.CopyToClipboardButton.Size = New System.Drawing.Size(35, 25)
        Me.CopyToClipboardButton.TabIndex = 76
        Me.CopyToClipboardButton.UseVisualStyleBackColor = True
        '
        'SaveFileButton
        '
        Me.SaveFileButton.Image = Global.ApskaitaWUI.My.Resources.Resources.Actions_document_save_icon_16p
        Me.SaveFileButton.Location = New System.Drawing.Point(135, 6)
        Me.SaveFileButton.Name = "SaveFileButton"
        Me.SaveFileButton.Size = New System.Drawing.Size(35, 25)
        Me.SaveFileButton.TabIndex = 75
        Me.SaveFileButton.UseVisualStyleBackColor = True
        '
        'OpenFileAccButton
        '
        Me.OpenFileAccButton.BorderStyleDown = System.Windows.Forms.Border3DStyle.Flat
        Me.OpenFileAccButton.BorderStyleNormal = System.Windows.Forms.Border3DStyle.Flat
        Me.OpenFileAccButton.BorderStyleUp = System.Windows.Forms.Border3DStyle.Flat
        Me.OpenFileAccButton.ButtonStyle = AccControls.rsButtonStyle.DropDownWithSep
        Me.OpenFileAccButton.Checked = False
        Me.OpenFileAccButton.DropDownSepWidth = 12
        Me.OpenFileAccButton.FocusRectangle = False
        Me.OpenFileAccButton.Image = Global.ApskaitaWUI.My.Resources.Resources.folder_open_icon_16p
        Me.OpenFileAccButton.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.OpenFileAccButton.ImagePadding = 2
        Me.OpenFileAccButton.Location = New System.Drawing.Point(94, 6)
        Me.OpenFileAccButton.Name = "OpenFileAccButton"
        Me.OpenFileAccButton.Size = New System.Drawing.Size(35, 25)
        Me.OpenFileAccButton.TabIndex = 74
        Me.OpenFileAccButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.OpenFileAccButton.TextPadding = 2
        '
        'PasteAccButton
        '
        Me.PasteAccButton.BorderStyleDown = System.Windows.Forms.Border3DStyle.Flat
        Me.PasteAccButton.BorderStyleNormal = System.Windows.Forms.Border3DStyle.Flat
        Me.PasteAccButton.BorderStyleUp = System.Windows.Forms.Border3DStyle.Flat
        Me.PasteAccButton.ButtonStyle = AccControls.rsButtonStyle.DropDownWithSep
        Me.PasteAccButton.Checked = False
        Me.PasteAccButton.DropDownSepWidth = 12
        Me.PasteAccButton.FocusRectangle = False
        Me.PasteAccButton.Image = Global.ApskaitaWUI.My.Resources.Resources.Paste_icon_16p
        Me.PasteAccButton.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.PasteAccButton.ImagePadding = 2
        Me.PasteAccButton.Location = New System.Drawing.Point(53, 6)
        Me.PasteAccButton.Name = "PasteAccButton"
        Me.PasteAccButton.Size = New System.Drawing.Size(35, 25)
        Me.PasteAccButton.TabIndex = 70
        Me.PasteAccButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.PasteAccButton.TextPadding = 2
        '
        'nCancelButton
        '
        Me.nCancelButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.nCancelButton.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.nCancelButton.Location = New System.Drawing.Point(628, 11)
        Me.nCancelButton.Name = "nCancelButton"
        Me.nCancelButton.Size = New System.Drawing.Size(78, 25)
        Me.nCancelButton.TabIndex = 68
        Me.nCancelButton.Text = "Atšaukti"
        Me.nCancelButton.UseVisualStyleBackColor = True
        '
        'ApplyButton
        '
        Me.ApplyButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ApplyButton.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ApplyButton.Location = New System.Drawing.Point(544, 11)
        Me.ApplyButton.Name = "ApplyButton"
        Me.ApplyButton.Size = New System.Drawing.Size(78, 25)
        Me.ApplyButton.TabIndex = 67
        Me.ApplyButton.Text = "Išsaugoti"
        Me.ApplyButton.UseVisualStyleBackColor = True
        '
        'nOkButton
        '
        Me.nOkButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.nOkButton.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.nOkButton.Location = New System.Drawing.Point(460, 11)
        Me.nOkButton.Name = "nOkButton"
        Me.nOkButton.Size = New System.Drawing.Size(78, 25)
        Me.nOkButton.TabIndex = 66
        Me.nOkButton.Text = "OK"
        Me.nOkButton.UseVisualStyleBackColor = True
        '
        'AccountListBindingSource
        '
        Me.AccountListBindingSource.DataSource = GetType(ApskaitaObjects.General.Account)
        '
        'AccountListDataGridView
        '
        Me.AccountListDataGridView.AllowUserToOrderColumns = True
        Me.AccountListDataGridView.AutoGenerateColumns = False
        Me.AccountListDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.AccountListDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells
        Me.AccountListDataGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.AccountListDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.AccountListDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.AccountListDataGridView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.DataGridViewTextBoxColumn4, Me.DataGridViewTextBoxColumn1, Me.DataGridViewTextBoxColumn2, Me.DataGridViewTextBoxColumn3})
        Me.AccountListDataGridView.DataSource = Me.AccountListBindingSource
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.AccountListDataGridView.DefaultCellStyle = DataGridViewCellStyle5
        Me.AccountListDataGridView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AccountListDataGridView.Location = New System.Drawing.Point(0, 0)
        Me.AccountListDataGridView.Name = "AccountListDataGridView"
        Me.AccountListDataGridView.RowHeadersWidth = 20
        Me.AccountListDataGridView.Size = New System.Drawing.Size(718, 428)
        Me.AccountListDataGridView.TabIndex = 3
        '
        'DataGridViewTextBoxColumn4
        '
        Me.DataGridViewTextBoxColumn4.DataPropertyName = "Class"
        Me.DataGridViewTextBoxColumn4.HeaderText = "Klasė"
        Me.DataGridViewTextBoxColumn4.Name = "DataGridViewTextBoxColumn4"
        Me.DataGridViewTextBoxColumn4.ReadOnly = True
        Me.DataGridViewTextBoxColumn4.Width = 63
        '
        'DataGridViewTextBoxColumn1
        '
        Me.DataGridViewTextBoxColumn1.AllowNegative = False
        Me.DataGridViewTextBoxColumn1.DataPropertyName = "ID"
        Me.DataGridViewTextBoxColumn1.DecimalLength = 0
        DataGridViewCellStyle2.Format = "##"
        Me.DataGridViewTextBoxColumn1.DefaultCellStyle = DataGridViewCellStyle2
        Me.DataGridViewTextBoxColumn1.HeaderText = "Sąskaitos Nr."
        Me.DataGridViewTextBoxColumn1.MaxInputLength = 15
        Me.DataGridViewTextBoxColumn1.Name = "DataGridViewTextBoxColumn1"
        Me.DataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DataGridViewTextBoxColumn1.Width = 108
        '
        'DataGridViewTextBoxColumn2
        '
        Me.DataGridViewTextBoxColumn2.DataPropertyName = "Name"
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DataGridViewTextBoxColumn2.DefaultCellStyle = DataGridViewCellStyle3
        Me.DataGridViewTextBoxColumn2.HeaderText = "Sąskaitos pavadinimas"
        Me.DataGridViewTextBoxColumn2.MaxInputLength = 255
        Me.DataGridViewTextBoxColumn2.Name = "DataGridViewTextBoxColumn2"
        Me.DataGridViewTextBoxColumn2.Width = 161
        '
        'DataGridViewTextBoxColumn3
        '
        Me.DataGridViewTextBoxColumn3.CloseOnSingleClick = True
        Me.DataGridViewTextBoxColumn3.ComboDataGridView = Nothing
        Me.DataGridViewTextBoxColumn3.DataPropertyName = "AssociatedReportItem"
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DataGridViewTextBoxColumn3.DefaultCellStyle = DataGridViewCellStyle4
        Me.DataGridViewTextBoxColumn3.EmptyValueString = ""
        Me.DataGridViewTextBoxColumn3.FilterPropertyName = ""
        Me.DataGridViewTextBoxColumn3.HeaderText = "Atskaitomybės eil."
        Me.DataGridViewTextBoxColumn3.InstantBinding = True
        Me.DataGridViewTextBoxColumn3.Name = "DataGridViewTextBoxColumn3"
        Me.DataGridViewTextBoxColumn3.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DataGridViewTextBoxColumn3.ValueMember = ""
        Me.DataGridViewTextBoxColumn3.Width = 134
        '
        'F_Accounts
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(718, 467)
        Me.Controls.Add(Me.AccountListDataGridView)
        Me.Controls.Add(Me.Panel2)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "F_Accounts"
        Me.ShowInTaskbar = False
        Me.Text = "Sąskaitų planas"
        Me.Panel2.ResumeLayout(False)
        CType(Me.AccountListBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.AccountListDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents nCancelButton As System.Windows.Forms.Button
    Friend WithEvents ApplyButton As System.Windows.Forms.Button
    Friend WithEvents nOkButton As System.Windows.Forms.Button
    Friend WithEvents AccountListBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents AccountListDataGridView As System.Windows.Forms.DataGridView
    Friend WithEvents PasteAccButton As AccControls.AccButton
    Friend WithEvents OpenFileAccButton As AccControls.AccButton
    Friend WithEvents SaveFileButton As System.Windows.Forms.Button
    Friend WithEvents CopyToClipboardButton As System.Windows.Forms.Button
    Friend WithEvents DataGridViewTextBoxColumn4 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn1 As AccControls.DataGridViewAccTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn2 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn3 As AccControls.DataGridViewAccGridComboBoxColumn
End Class
