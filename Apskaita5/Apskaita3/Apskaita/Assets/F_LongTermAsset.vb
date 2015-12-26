Imports ApskaitaObjects.Assets
Imports ApskaitaObjects.HelperLists
Imports ApskaitaObjects.Documents.InvoiceAdapters

Public Class F_LongTermAsset
    Implements IObjectEditForm

    Private Obj As LongTermAsset = Nothing
    Private Loading As Boolean = True
    Private LongTermAssetID As Integer = -1
    Private ChildObject As AssetAcquisitionInvoiceAdapter = Nothing


    Public ReadOnly Property ObjectID() As Integer Implements IObjectEditForm.ObjectID
        Get
            If Not ChildObject Is Nothing Then Return DirectCast(ChildObject.ValueObject, LongTermAsset).ID
            If Not Obj Is Nothing Then Return Obj.ID
            Return 0
        End Get
    End Property

    Public ReadOnly Property ObjectType() As System.Type Implements IObjectEditForm.ObjectType
        Get
            Return GetType(LongTermAsset)
        End Get
    End Property


    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Sub New(ByVal nLongTermAssetID As Integer)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        LongTermAssetID = nLongTermAssetID

    End Sub

    Public Sub New(ByRef nChildObject As AssetAcquisitionInvoiceAdapter)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        If nChildObject Is Nothing Then Throw New ArgumentNullException("ChildObject negali būti null.")

        ' Add any initialization after the InitializeComponent() call.
        ChildObject = nChildObject

    End Sub


    Private Sub F_LongTermAsset_Activated(ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles Me.Activated

        If Me.WindowState = FormWindowState.Maximized AndAlso MyCustomSettings.AutoSizeForm Then _
            Me.WindowState = FormWindowState.Normal

        If Loading Then
            Loading = False
            Exit Sub
        End If

        If Not PrepareCache(Me, GetType(AccountInfoList), _
            GetType(LongTermAssetCustomGroupInfoList), GetType(NameInfoList)) Then Exit Sub

    End Sub

    Private Sub F_LongTermAsset_FormClosing(ByVal sender As Object, _
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

    End Sub

    Private Sub F_LongTermAsset_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        If ChildObject Is Nothing Then

            If Not LongTermAsset.CanGetObject AndAlso LongTermAssetID > 0 Then
                MsgBox("Klaida. Jūsų teisių nepakanka šiai informacijai gauti.", _
                    MsgBoxStyle.Exclamation, "Klaida.")
                DisableAllControls(Me)
                Exit Sub
            ElseIf Not LongTermAsset.CanAddObject AndAlso Not LongTermAssetID > 0 Then
                MsgBox("Klaida. Jūsų teisių nepakanka ilgalaikio turto duomenims tvarkyti.", _
                    MsgBoxStyle.Exclamation, "Klaida.")
                DisableAllControls(Me)
                Exit Sub
            End If

        End If

        If Not SetDataSources() Then Exit Sub

        If ChildObject Is Nothing Then

            If LongTermAssetID > 0 Then

                Try
                    Obj = LoadObject(Of LongTermAsset)(Nothing, "GetLongTermAsset", _
                        True, LongTermAssetID)
                Catch ex As Exception
                    ShowError(ex)
                    DisableAllControls(Me)
                    Exit Sub
                End Try

            Else

                Obj = LongTermAsset.GetNewLongTermAsset

            End If

            LongTermAssetBindingSource.DataSource = Obj

        Else

            ChildObject.AcquisitionOperation.BeginEdit()
            LongTermAssetBindingSource.DataSource = ChildObject.AcquisitionOperation

        End If

        SetFormLayout(Me)

        ConfigureButtons()

    End Sub


    Private Sub OkButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles nOkButton.Click

        If Not ChildObject Is Nothing Then

            If Not ChildObject.AcquisitionOperation.IsValid Then
                If Not YesOrNo("Redaguojamuose turto įsigijimo duomenyse yra klaidų:" & vbCrLf _
                    & ChildObject.AcquisitionOperation. _
                    BrokenRulesCollection.ToString(Csla.Validation.RuleSeverity.Error) _
                    & vbCrLf & vbCrLf & "Ar tikrai norite uždaryti formą?") Then Exit Sub
            End If

            BindingsManager.UnBind(True, False, New BindingSource() {LongTermAssetBindingSource})
            ChildObject.AcquisitionOperation.ApplyEdit()
            Me.DialogResult = Windows.Forms.DialogResult.OK
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

            BindingsManager.UnBind(True, True, New BindingSource() {LongTermAssetBindingSource})
            ChildObject.AcquisitionOperation.CancelEdit()
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
                True, Obj.AcquisitionDate, Obj.AcquisitionDate, "", -1, -1, _
                DocumentType.None, False, "", "")
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
            Obj.LoadAssociatedJournalEntry(CType(JournalEntryInfoComboBox.SelectedItem,  _
                ActiveReports.JournalEntryInfo))
        Catch ex As Exception
            ShowError(ex)
            Exit Sub
        End Try

    End Sub

    Private Sub LimitationsButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles LimitationsButton.Click

        If Not ChildObject Is Nothing Then
            MsgBox(ChildObject.ChronologyValidator.LimitsExplanation, MsgBoxStyle.Information, "")
        ElseIf Not Obj Is Nothing Then
            MsgBox(Obj.ChronologyValidator.LimitsExplanation, MsgBoxStyle.Information, "")
        End If

    End Sub

    Private Sub ViewJournalEntryButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ViewJournalEntryButton.Click

        If Obj Is Nothing OrElse Not Obj.AcquisitionJournalEntryID > 0 Then Exit Sub

        MDIParent1.LaunchForm(GetType(F_GeneralLedgerEntry), False, False, _
            Obj.AcquisitionJournalEntryID, Obj.AcquisitionJournalEntryID)

    End Sub


    Private Function SaveObj() As Boolean

        If Obj Is Nothing OrElse Not Obj.IsDirty Then Return True

        If Not Obj.IsValid Then
            MsgBox("Formoje yra klaidų:" & vbCrLf & Obj.BrokenRulesCollection.ToString( _
                Csla.Validation.RuleSeverity.Error), MsgBoxStyle.Exclamation, "Klaida.")
            Return False
        End If

        Dim Question, Answer As String
        If Obj.BrokenRulesCollection.WarningCount > 0 Then
            Question = "DĖMESIO. Duomenyse gali būti klaidų: " & vbCrLf _
                & Obj.BrokenRulesCollection.ToString(Csla.Validation.RuleSeverity.Warning) & vbCrLf
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

        Using bm As New BindingsManager(LongTermAssetBindingSource, Nothing, Nothing, True, False)

            Try
                Obj = LoadObject(Of LongTermAsset)(Obj, "Save", False)
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
        Using bm As New BindingsManager(LongTermAssetBindingSource, Nothing, Nothing, True, True)
        End Using
    End Sub

    Private Function SetDataSources() As Boolean

        If Not PrepareCache(Me, GetType(HelperLists.AccountInfoList), _
            GetType(LongTermAssetCustomGroupInfoList), GetType(NameInfoList)) Then Return False

        Try

            LoadAccountInfoListToGridCombo(AccountAcquisitionAccGridComboBox, True, 1, 2)
            LoadAccountInfoListToGridCombo(AccountAccumulatedAmortizationAccGridComboBox, True, 1, 2)
            LoadAccountInfoListToGridCombo(AccountValueDecreaseAccGridComboBox, True, 1, 2)
            LoadAccountInfoListToGridCombo(AccountValueIncreaseAccGridComboBox, True, 1, 2)
            LoadAccountInfoListToGridCombo(AccountRevaluedPortionAmmortizationAccGridComboBox, True, 1, 2)
            LoadNameInfoListToCombo(LegalGroupComboBox, _
                ApskaitaObjects.Settings.NameType.LongTermAssetLegalGroup, True)

            CustomGroupInfoAccComboBox.DisplayMember = "Name"
            CustomGroupInfoAccComboBox.ValueMember = "GetMe"
            CustomGroupInfoAccComboBox.DataSource = _
                LongTermAssetCustomGroupInfoList.GetList()

        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
            Return False
        End Try

        Return True

    End Function

    Private Sub ConfigureButtons()

        Dim asset As LongTermAsset = Nothing

        If Not ChildObject Is Nothing Then
            asset = DirectCast(ChildObject.ValueObject, LongTermAsset)
        Else
            asset = Obj
        End If

        If asset Is Nothing Then
            DisableAllControls(Me)
            LimitationsButton.Visible = False
            Exit Sub
        End If

        If asset.AllDataIsReadOnly Then
            DisableAllControls(Me)
            LimitationsButton.Visible = False
            Exit Sub
        End If

        AccountAcquisitionAccGridComboBox.Enabled = Not asset.FinancialDataIsReadOnly
        AccountValueDecreaseAccGridComboBox.Enabled = Not asset.FinancialDataIsReadOnly
        AccountAccumulatedAmortizationAccGridComboBox.Enabled = Not asset.FinancialDataIsReadOnly
        AccountValueIncreaseAccGridComboBox.Enabled = Not asset.FinancialDataIsReadOnly
        AccountRevaluedPortionAmmortizationAccGridComboBox.Enabled = Not asset.FinancialDataIsReadOnly
        AcquisitionAccountValuePerUnitAccTextBox.ReadOnly = asset.FinancialDataIsReadOnly
        AmortizationAccountValuePerUnitAccTextBox.ReadOnly = asset.FinancialDataIsReadOnly
        ValueDecreaseAccountValuePerUnitAccTextBox.ReadOnly = asset.FinancialDataIsReadOnly
        ValueIncreaseAccountValuePerUnitAccTextBox.ReadOnly = asset.FinancialDataIsReadOnly
        ValueIncreaseAmmortizationAccountValuePerUnitAccTextBox.ReadOnly = asset.FinancialDataIsReadOnly
        AmmountAccTextBox.ReadOnly = asset.FinancialDataIsReadOnly
        AcquisitionAccountValueCorrectionNumericUpDown.ReadOnly = asset.FinancialDataIsReadOnly
        AmortizationAccountValueCorrectionNumericUpDown.ReadOnly = asset.FinancialDataIsReadOnly
        ValueDecreaseAccountValueCorrectionNumericUpDown.ReadOnly = asset.FinancialDataIsReadOnly
        ValueIncreaseAccountValueCorrectionNumericUpDown.ReadOnly = asset.FinancialDataIsReadOnly
        ValueIncreaseAmmortizationAccountValueCorrectionNumericUpDown.ReadOnly = asset.FinancialDataIsReadOnly

        AcquisitionDateDateTimePicker.Enabled = Not asset.AcquisitionDateIsReadOnly

        Me.AmortizationCalculatedForMonthsAccTextBox.ReadOnly = asset.AmortizationDataIsReadOnly
        Me.DefaultAmortizationPeriodAccTextBox.ReadOnly = asset.AmortizationDataIsReadOnly
        Me.LiquidationUnitValueAccTextBox.ReadOnly = asset.AmortizationDataIsReadOnly
        Me.ContinuedUsageCheckBox.Enabled = Not asset.AmortizationDataIsReadOnly

        ApplyButton.Enabled = ChildObject Is Nothing
        nCancelButton.Enabled = Not asset.IsNew

        EditedBaner.Visible = Not asset.IsNew

        JournalEntryInfoComboBox.Enabled = ChildObject Is Nothing
        RefreshJournalEntryInfoButton.Enabled = ChildObject Is Nothing
        AttachJournalEntryInfoButton.Enabled = ChildObject Is Nothing

        ViewJournalEntryButton.Enabled = Not asset.IsNew

        If asset.FinancialDataIsReadOnly AndAlso Not asset.AmortizationDataIsReadOnly Then
            LimitationsButton.Visible = True
            LimitationsButton.Image = My.Resources.Action_lock_icon_24p
        ElseIf asset.FinancialDataIsReadOnly Then
            LimitationsButton.Visible = True
            LimitationsButton.Image = My.Resources.Action_lock_full_icon_24p
        Else
            LimitationsButton.Visible = False
        End If

    End Sub

End Class