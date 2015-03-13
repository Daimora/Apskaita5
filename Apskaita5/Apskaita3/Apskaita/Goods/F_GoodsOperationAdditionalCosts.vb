Imports ApskaitaObjects.Goods
Public Class F_GoodsOperationAdditionalCosts
    Implements IObjectEditForm

    Private Obj As GoodsOperationAdditionalCosts = Nothing
    Private Loading As Boolean = True
    Private OperationID As Integer = 0
    Private GoodsID As Integer = 0
    Private WarehouseID As Integer = 0
    Private ChildObject As Documents.InvoiceAttachedObject = Nothing


    Public ReadOnly Property ObjectID() As Integer Implements IObjectEditForm.ObjectID
        Get
            If Not ChildObject Is Nothing Then Return DirectCast(ChildObject.ValueObject, _
                GoodsOperationAdditionalCosts).ID
            If Not Obj Is Nothing Then Return Obj.ID
            Return 0
        End Get
    End Property

    Public ReadOnly Property ObjectType() As System.Type Implements IObjectEditForm.ObjectType
        Get
            Return GetType(GoodsOperationAdditionalCosts)
        End Get
    End Property


    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Sub New(ByVal nOperationID As Integer)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        OperationID = nOperationID

    End Sub

    Public Sub New(ByVal nGoodsID As Integer, ByVal nWarehouseID As Integer)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        GoodsID = nGoodsID
        WarehouseID = nWarehouseID

    End Sub

    Public Sub New(ByRef nChildObject As Documents.InvoiceAttachedObject)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        If nChildObject Is Nothing Then Throw New ArgumentNullException("ChildObject negali būti null.")

        If nChildObject.Type <> Documents.InvoiceAttachedObjectType.GoodsConsignmentAdditionalCosts Then _
            Throw New Exception("Klaida. Objekto tipas nėra prekių savikainos padidinimas.")

        ' Add any initialization after the InitializeComponent() call.
        ChildObject = nChildObject

    End Sub


    Private Sub F_GoodsOperationAdditionalCosts_Activated(ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles Me.Activated

        If Me.WindowState = FormWindowState.Maximized AndAlso MyCustomSettings.AutoSizeForm Then _
            Me.WindowState = FormWindowState.Normal

        If Loading Then
            Loading = False
            Exit Sub
        End If

        If Not PrepareCache(Me, GetType(HelperLists.AccountInfoList)) Then Exit Sub

    End Sub

    Private Sub F_GoodsOperationAdditionalCosts_FormClosing(ByVal sender As Object, _
        ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        If Not Obj Is Nothing AndAlso TypeOf Obj Is IIsDirtyEnough AndAlso _
            DirectCast(Obj, IIsDirtyEnough).IsDirtyEnough Then
            Dim answ As String = Ask("Išsaugoti duomenis?", New ButtonStructure("Taip"), _
                New ButtonStructure("Ne"), New ButtonStructure("Atšaukti"))
            If answ <> "Taip" AndAlso answ <> "Ne" Then
                e.Cancel = True
                Exit Sub
            End If
            If answ = "Taip" Then
                If Not SaveObj() Then
                    e.Cancel = True
                    Exit Sub
                End If
            End If
        End If

        If Not Obj Is Nothing AndAlso Obj.IsDirty Then CancelObj()

        GetFormLayout(Me)
        GetDataGridViewLayOut(ConsignmentsDataGridView)

    End Sub

    Private Sub F_GoodsOperationAdditionalCosts_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        If ChildObject Is Nothing Then

            If Not GoodsOperationAdditionalCosts.CanGetObject AndAlso OperationID > 0 Then
                MsgBox("Klaida. Jūsų teisių nepakanka šiai informacijai gauti.", _
                    MsgBoxStyle.Exclamation, "Klaida.")
                DisableAllControls(Me)
                Exit Sub
            ElseIf Not GoodsOperationAdditionalCosts.CanAddObject AndAlso Not OperationID > 0 Then
                MsgBox("Klaida. Jūsų teisių nepakanka prekių duomenims tvarkyti.", _
                    MsgBoxStyle.Exclamation, "Klaida.")
                DisableAllControls(Me)
                Exit Sub
            End If

        End If

        If Not SetDataSources() Then Exit Sub

        If ChildObject Is Nothing Then

            If OperationID > 0 Then

                Try
                    Obj = LoadObject(Of GoodsOperationAdditionalCosts)(Nothing, "GetGoodsOperationAdditionalCosts", _
                        True, OperationID)
                Catch ex As Exception
                    ShowError(ex)
                    DisableAllControls(Me)
                    Exit Sub
                End Try

            Else

                Try
                    Obj = LoadObject(Of GoodsOperationAdditionalCosts)(Nothing, "NewGoodsOperationAdditionalCosts", _
                        True, GoodsID, WarehouseID)
                Catch ex As Exception
                    ShowError(ex)
                    DisableAllControls(Me)
                    Exit Sub
                End Try

            End If

            GoodsOperationAdditionalCostsBindingSource.DataSource = Obj

            ConfigureLimitationButton(Obj)

        Else

            DirectCast(ChildObject.ValueObject, GoodsOperationAdditionalCosts).BeginEdit()
            GoodsOperationAdditionalCostsBindingSource.DataSource = _
                DirectCast(ChildObject.ValueObject, GoodsOperationAdditionalCosts)

            ConfigureLimitationButton(DirectCast(ChildObject.ValueObject, GoodsOperationAdditionalCosts))
            DateDateTimePicker.Enabled = False

        End If

        AddDGVColumnSelector(ConsignmentsDataGridView)

        SetDataGridViewLayOut(ConsignmentsDataGridView)
        SetFormLayout(Me)

        ConfigureButtons()

        Dim h As New EditableDataGridViewHelper(ConsignmentsDataGridView)

    End Sub


    Private Sub OkButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles nOkButton.Click

        If Not ChildObject Is Nothing Then

            If Not DirectCast(ChildObject.ValueObject, GoodsOperationAdditionalCosts).IsValid Then
                If Not YesOrNo("Redaguojamuose prekių savikainos padidinimo duomenyse yra klaidų:" & vbCrLf _
                    & DirectCast(ChildObject.ValueObject, GoodsOperationAdditionalCosts).GetAllBrokenRules _
                    & vbCrLf & vbCrLf & "Ar tikrai norite uždaryti formą?") Then Exit Sub
            End If

            If DirectCast(ChildObject.ValueObject, GoodsOperationAdditionalCosts).IsDirty Then
                Me.DialogResult = Windows.Forms.DialogResult.OK
            Else
                Me.DialogResult = Windows.Forms.DialogResult.Cancel
            End If
            BindingsManager.UnBind(True, False, New BindingSource() {GoodsOperationAdditionalCostsBindingSource})
            If Me.DialogResult = Windows.Forms.DialogResult.OK Then
                DirectCast(ChildObject.ValueObject, GoodsOperationAdditionalCosts).ApplyEdit()
            Else
                DirectCast(ChildObject.ValueObject, GoodsOperationAdditionalCosts).CancelEdit()
            End If
            Me.Close()

        ElseIf Not Obj Is Nothing Then

            If SaveObj() Then Me.Close()

        End If

    End Sub

    Private Sub ApplyButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ApplyButton.Click
        If Not ChildObject Is Nothing OrElse Obj Is Nothing Then Exit Sub
        If SaveObj() Then ConfigureButtons()
    End Sub

    Private Sub CancelButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles nCancelButton.Click

        If Not ChildObject Is Nothing Then

            BindingsManager.UnBind(True, True, New BindingSource() {GoodsOperationAdditionalCostsBindingSource})
            DirectCast(ChildObject.ValueObject, GoodsOperationAdditionalCosts).CancelEdit()
            Me.DialogResult = Windows.Forms.DialogResult.Cancel
            Me.Close()

        ElseIf Not Obj Is Nothing Then

            CancelObj()

        End If

    End Sub


    Private Sub RefreshJournalEntryInfoButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles RefreshJournalEntryInfoButton.Click

        If Not ChildObject Is Nothing OrElse Obj Is Nothing Then Exit Sub

        Dim JEList As ActiveReports.JournalEntryInfoList

        Try
            JEList = LoadObject(Of ActiveReports.JournalEntryInfoList)(Nothing, "GetList", _
                True, Obj.Date, Obj.Date, "", -1, -1, DocumentType.None, False, "", "")
        Catch ex As Exception
            ShowError(ex)
            Exit Sub
        End Try

        JournalEntryInfoComboBox.DataSource = JEList

    End Sub

    Private Sub AttachJournalEntryInfoButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles AttachJournalEntryInfoButton.Click

        If Not ChildObject Is Nothing OrElse Obj Is Nothing Then Exit Sub

        If JournalEntryInfoComboBox.SelectedItem Is Nothing Then Exit Sub

        Try
            Obj.LoadAssociatedJournalEntry(CType(JournalEntryInfoComboBox.SelectedItem, _
                ActiveReports.JournalEntryInfo))
        Catch ex As Exception
            ShowError(ex)
            Exit Sub
        End Try

    End Sub

    Private Sub AccountPurchasesIsClosedCheckBox_CheckStateChanged(ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles AccountPurchasesIsClosedCheckBox.CheckStateChanged

        If Obj Is Nothing AndAlso ChildObject Is Nothing Then Exit Sub

        If Not ChildObject Is Nothing Then
            DirectCast(ChildObject.ValueObject, GoodsOperationAdditionalCosts). _
                AccountPurchasesIsClosed = AccountPurchasesIsClosedCheckBox.Checked
            TotalGoodsValueChangeAccTextBox.ReadOnly = DirectCast(ChildObject.ValueObject, _
                GoodsOperationAdditionalCosts).TotalGoodsValueChangeIsReadOnly
        Else
            Obj.AccountPurchasesIsClosed = AccountPurchasesIsClosedCheckBox.Checked
            TotalGoodsValueChangeAccTextBox.ReadOnly = Obj.TotalGoodsValueChangeIsReadOnly
        End If

    End Sub

    Private Sub LimitationsButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles LimitationsButton.Click

        If Not ChildObject Is Nothing Then
            MsgBox(ChildObject.ChronologyValidator.LimitsExplanation, MsgBoxStyle.Information, "")
        ElseIf Not Obj Is Nothing Then
            MsgBox(Obj.OperationLimitations.LimitsExplanation & vbCrLf & vbCrLf & _
                Obj.OperationLimitations.GetBackgroundDescription, MsgBoxStyle.Information, "")
        End If

    End Sub

    Private Sub ViewJournalEntryButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ViewJournalEntryButton.Click

        If Obj Is Nothing OrElse Not Obj.JournalEntryID > 0 Then Exit Sub

        MDIParent1.LaunchForm(GetType(F_GeneralLedgerEntry), False, False, _
            Obj.JournalEntryID, Obj.JournalEntryID)

    End Sub


    Private Function SaveObj() As Boolean

        If Obj Is Nothing OrElse Not Obj.IsDirty Then Return True

        If Not Obj.IsValid Then
            MsgBox("Formoje yra klaidų:" & vbCrLf & Obj.GetAllBrokenRules, MsgBoxStyle.Exclamation, "Klaida.")
            Return False
        End If

        Dim Question, Answer As String
        If Not String.IsNullOrEmpty(Obj.GetAllWarnings.Trim) Then
            Question = "DĖMESIO. Duomenyse gali būti klaidų: " & vbCrLf & Obj.GetAllWarnings & vbCrLf
        Else
            Question = ""
        End If
        If Obj.IsNew Then
            Question = Question & "Ar tikrai norite įtraukti naujus duomenis?"
            Answer = "Nauji duomenys sėkmingai įtraukti."
        Else
            Question = Question & "Ar tikrai norite pakeisti duomenis?"
            Answer = "Duomenys sėkmingai pakeisti."
        End If

        If Not YesOrNo(Question) Then Return False

        Using bm As New BindingsManager(GoodsOperationAdditionalCostsBindingSource, _
            ConsignmentsBindingSource, Nothing, True, False)

            Try
                Obj = LoadObject(Of GoodsOperationAdditionalCosts)(Obj, "Save", False)
            Catch ex As Exception
                ShowError(ex)
                Return False
            End Try

            bm.SetNewDataSource(Obj)

        End Using

        MsgBox(Answer, MsgBoxStyle.Information, "Info")

        Return True

    End Function

    Private Sub CancelObj()
        If Obj Is Nothing OrElse Obj.IsNew Then Exit Sub
        Using bm As New BindingsManager(GoodsOperationAdditionalCostsBindingSource, _
            ConsignmentsBindingSource, Nothing, True, True)
        End Using
    End Sub

    Private Function SetDataSources() As Boolean

        If Not PrepareCache(Me, GetType(HelperLists.AccountInfoList)) Then Return False

        Try

            LoadAccountInfoListToGridCombo(AccountGoodsNetCostsAccGridComboBox, False, 6)

        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
            Return False
        End Try

        Return True

    End Function

    Private Sub ConfigureButtons()

        Dim current As GoodsOperationAdditionalCosts = Nothing
        If Not ChildObject Is Nothing Then
            current = DirectCast(ChildObject.ValueObject, GoodsOperationAdditionalCosts)
        ElseIf Not Obj Is Nothing Then
            current = Obj
        End If

        TotalGoodsValueChangeAccTextBox.ReadOnly = current Is Nothing OrElse current.TotalGoodsValueChangeIsReadOnly
        TotalValueChangeAccTextBox.ReadOnly = current Is Nothing OrElse current.TotalValueChangeIsReadOnly
        AccountGoodsNetCostsAccGridComboBox.Enabled = Not current Is Nothing AndAlso _
            Not current.AccountGoodsNetCostsIsReadOnly
        AccountPurchasesIsClosedCheckBox.Enabled = Not current Is Nothing AndAlso _
            Not current.AccountPurchasesIsClosedIsReadOnly
        DateDateTimePicker.Enabled = Not current Is Nothing AndAlso Not current.DateIsReadOnly
        DescriptionTextBox.ReadOnly = current Is Nothing OrElse current.DescriptionIsReadOnly

        ApplyButton.Enabled = ChildObject Is Nothing AndAlso Not Obj Is Nothing
        nCancelButton.Enabled = Not ChildObject Is Nothing OrElse (Not Obj Is Nothing AndAlso Not Obj.IsNew)

        ConsignmentsDataGridView.ReadOnly = current Is Nothing OrElse _
            Not current.OperationLimitations.FinancialDataCanChange

        EditedBaner.Visible = Not current Is Nothing AndAlso Not current.IsNew

        AttachJournalEntryInfoButton.Enabled = Not current Is Nothing AndAlso _
            Not current.AssociatedJournalEntryIsReadOnly
        JournalEntryInfoComboBox.Enabled = Not current Is Nothing AndAlso _
            Not current.AssociatedJournalEntryIsReadOnly
        RefreshJournalEntryInfoButton.Enabled = Not current Is Nothing AndAlso _
            Not current.AssociatedJournalEntryIsReadOnly

        ViewJournalEntryButton.Visible = Not Obj Is Nothing

    End Sub

    Private Sub ConfigureLimitationButton(ByVal asset As GoodsOperationAdditionalCosts)

        If Not asset.OperationLimitations.FinancialDataCanChange OrElse _
            asset.OperationLimitations.MaxDateApplicable OrElse _
            asset.OperationLimitations.MinDateApplicable Then
            LimitationsButton.Visible = True
            LimitationsButton.Image = My.Resources.Action_lock_icon_32p
        Else
            LimitationsButton.Visible = False
        End If

    End Sub

End Class