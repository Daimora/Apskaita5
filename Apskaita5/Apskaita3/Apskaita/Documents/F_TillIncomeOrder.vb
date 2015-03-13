Imports ApskaitaObjects.Documents
Imports ApskaitaObjects.Settings
Public Class F_TillIncomeOrder
    Implements ISupportsPrinting, IObjectEditForm

    Private Obj As TillIncomeOrder
    Private TillIncomeOrderID As Integer = -1
    Private _AdvanceReportInfo As ActiveReports.AdvanceReportInfo = Nothing
    Private Loading As Boolean = True

    Public ReadOnly Property ObjectID() As Integer Implements IObjectEditForm.ObjectID
        Get
            If Not Obj Is Nothing Then Return Obj.ID
            Return 0
        End Get
    End Property

    Public ReadOnly Property ObjectType() As System.Type Implements IObjectEditForm.ObjectType
        Get
            Return GetType(TillIncomeOrder)
        End Get
    End Property


    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Sub New(ByVal nTillIncomeOrderID As Integer)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        TillIncomeOrderID = nTillIncomeOrderID

    End Sub

    Public Sub New(ByVal nAdvanceReportInfo As ActiveReports.AdvanceReportInfo)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        _AdvanceReportInfo = nAdvanceReportInfo

    End Sub


    Private Sub F_TillIncomeOrder_Activated(ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles Me.Activated

        If Me.WindowState = FormWindowState.Maximized AndAlso MyCustomSettings.AutoSizeForm Then _
            Me.WindowState = FormWindowState.Normal

        If Loading Then
            Loading = False
            Exit Sub
        End If

        If Not PrepareCache(Me, GetType(HelperLists.DocumentSerialInfoList), _
            GetType(HelperLists.PersonInfoList), GetType(HelperLists.AccountInfoList), _
            GetType(HelperLists.CashAccountInfoList)) Then Exit Sub

    End Sub

    Private Sub F_TillIncomeOrder_FormClosing(ByVal sender As Object, _
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

        GetDataGridViewLayOut(BookEntryItemsDataGridView)
        GetFormLayout(Me)

    End Sub

    Private Sub F_TillIncomeOrder_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        If TillIncomeOrderID > 0 AndAlso Not Documents.TillIncomeOrder.CanGetObject Then
            MsgBox("Klaida. Jūsų teisių nepakanka šiai informacijai gauti.", _
                MsgBoxStyle.Exclamation, "Klaida.")
            DisableAllControls(Me)
            Exit Sub
        ElseIf Not TillIncomeOrderID > 0 AndAlso Not Documents.TillIncomeOrder.CanAddObject Then
            MsgBox("Klaida. Jūsų teisių nepakanka naujam dokumentui įvesti.", _
                MsgBoxStyle.Exclamation, "Klaida.")
            DisableAllControls(Me)
            Exit Sub
        End If

        If Not SetDataSources() Then Exit Sub

        If TillIncomeOrderID > 0 Then

            Try
                Obj = LoadObject(Of TillIncomeOrder)(Nothing, "GetTillIncomeOrder", _
                    True, TillIncomeOrderID)
            Catch ex As Exception
                ShowError(ex)
                DisableAllControls(Me)
                Exit Sub
            End Try

        Else

            Obj = TillIncomeOrder.NewTillIncomeOrder

            If Not _AdvanceReportInfo Is Nothing AndAlso _AdvanceReportInfo.ID > 0 Then
                Try
                    Obj.LoadAdvanceReport(_AdvanceReportInfo)
                Catch ex As Exception
                    ShowError(ex)
                End Try
            End If

        End If

        TillIncomeOrderBindingSource.DataSource = Obj

        SetDataGridViewLayOut(BookEntryItemsDataGridView)
        SetFormLayout(Me)

        If Not Obj.IsNew AndAlso Not Documents.TillIncomeOrder.CanEditObject Then
            MsgBox("Klaida. Jūsų teisių nepakanka duomenims redaguoti.", _
                MsgBoxStyle.Exclamation, "Klaida.")
            DisableAllControls(Me)
            BookEntryItemsDataGridView.Enabled = True
            BookEntryItemsDataGridView.ReadOnly = True
            Exit Sub
        End If

        ConfigureButtons()

        Dim h As New EditableDataGridViewHelper(BookEntryItemsDataGridView)

    End Sub


    Private Sub OkButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles IOkButton.Click

        If Not SaveObj() Then Exit Sub
        Me.Hide()
        Me.Close()

    End Sub

    Private Sub ApplyButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles IApplyButton.Click
        If Not SaveObj() Then Exit Sub
        If Not Obj.IsNew AndAlso Not TillIncomeOrder.CanEditObject Then
            MsgBox("Klaida. Jūsų teisių nepakanka duomenims redaguoti.", _
                MsgBoxStyle.Exclamation, "Klaida.")
            DisableAllControls(Me)
            Exit Sub
        End If
    End Sub

    Private Sub CancelButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ICancelButton.Click
        CancelObj()
    End Sub


    Private Sub RefreshAdvanceReportInfoListButton_Click(ByVal sender As System.Object, _
           ByVal e As System.EventArgs) Handles RefreshAdvanceReportInfoListButton.Click

        Dim AdvanceReportList As ActiveReports.AdvanceReportInfoList

        Try
            AdvanceReportList = LoadObject(Of ActiveReports.AdvanceReportInfoList) _
                (Nothing, "GetAdvanceReportInfoList", True, AdvanceReportInfoDateTimePicker.Value.Date, _
                AdvanceReportInfoDateTimePicker.Value.Date, Nothing)
        Catch ex As Exception
            ShowError(ex)
            Exit Sub
        End Try

        AdvanceReportInfoComboBox.DataSource = AdvanceReportList

    End Sub

    Private Sub AddJournalEntryInfoButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles AddAdvanceReportInfoButton.Click

        If AdvanceReportInfoComboBox.SelectedItem Is Nothing Then Exit Sub

        Try
            Obj.LoadAdvanceReport(CType(AdvanceReportInfoComboBox.SelectedItem, _
                ActiveReports.AdvanceReportInfo))
        Catch ex As Exception
            ShowError(ex)
            Exit Sub
        End Try

    End Sub

    Private Sub RemoveJournalEntryInfoButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles RemoveAdvanceReportInfoButton.Click

        Try
            Obj.ClearAdvanceReport()
        Catch ex As Exception
            ShowError(ex)
            Exit Sub
        End Try

    End Sub

    Private Sub GetCurrencyRatesButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles GetCurrencyRatesButton.Click

        If Obj Is Nothing OrElse Obj.AccountCurrency Is Nothing _
            OrElse String.IsNullOrEmpty(Obj.AccountCurrency.Trim) _
            OrElse Obj.AccountCurrency.Trim.ToUpper = GetCurrentCompany.BaseCurrency Then Exit Sub

        If Not YesOrNo("Gauti valiutos kursą?") Then Exit Sub

        Dim result As AccWebCrawler.CurrencyRateList = _
            FetchCurrencyRate(Obj.AccountCurrency.Trim.ToUpper, Obj.Date)
        If Not result Is Nothing Then Obj.CurrencyRateInAccount = _
            result.GetCurrencyRate(Obj.Date, Obj.AccountCurrency.Trim.ToUpper)

    End Sub

    Private Sub RefreshNumberButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles RefreshNumberButton.Click

        If Obj Is Nothing OrElse Obj.DocumentSerial Is Nothing OrElse _
            String.IsNullOrEmpty(Obj.DocumentSerial.Trim) Then Exit Sub

        If Not Obj.IsNew Then
            If Not YesOrNo("DĖMESIO. Jūs redaguojate jau įtrauktą į duomenų bazę dokumentą. " _
                & "Ar tikrai norite suteikti jam naują numerį?") Then Exit Sub
        End If

        Using busy As New StatusBusy
            Try
                Obj.DocumentNumber = CommandLastDocumentNumber.TheCommand( _
                    DocumentSerialType.TillIncomeOrder, Obj.DocumentSerial.Trim, Obj.Date, _
                    Obj.AddDateToNumberOptionWasUsed) + 1
            Catch ex As Exception
                ShowError(ex)
            End Try
        End Using

    End Sub

    Private Sub DateDateTimePicker_CloseUp(ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles DateDateTimePicker.CloseUp

        If Obj Is Nothing OrElse Not Obj.IsNew OrElse Loading OrElse _
            Not MyCustomSettings.AutoReloadData OrElse Not Obj.AddDateToNumberOptionWasUsed _
            OrElse Obj.DocumentSerial Is Nothing OrElse String.IsNullOrEmpty(Obj.DocumentSerial.Trim) Then Exit Sub

        Obj.Date = DateDateTimePicker.Value

        Using busy As New StatusBusy
            Try
                Obj.DocumentNumber = CommandLastDocumentNumber.TheCommand( _
                    DocumentSerialType.TillIncomeOrder, Obj.DocumentSerial, Obj.Date, _
                    Obj.AddDateToNumberOptionWasUsed) + 1
            Catch ex As Exception
                ShowError(ex)
            End Try
        End Using

    End Sub

    Private Sub DocumentSerialComboBox_DropDownClosed(ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles DocumentSerialComboBox.DropDownClosed

        If Obj Is Nothing OrElse Not Obj.IsNew OrElse Loading OrElse _
            Not MyCustomSettings.AutoReloadData Then Exit Sub

        If DocumentSerialComboBox.SelectedItem Is Nothing OrElse _
            String.IsNullOrEmpty(DocumentSerialComboBox.SelectedItem.ToString.Trim) Then
            Exit Sub
        Else
            Obj.DocumentSerial = DocumentSerialComboBox.SelectedItem.ToString.Trim
        End If

        Using busy As New StatusBusy
            Try
                Obj.DocumentNumber = CommandLastDocumentNumber.TheCommand( _
                    DocumentSerialType.TillIncomeOrder, Obj.DocumentSerial, Obj.Date, _
                    Obj.AddDateToNumberOptionWasUsed) + 1
            Catch ex As Exception
                ShowError(ex)
            End Try
        End Using

    End Sub

    Private Sub LimitationsButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles LimitationsButton.Click
        If Obj Is Nothing Then Exit Sub
        MsgBox(Obj.ChronologicValidator.LimitsExplanation, MsgBoxStyle.Information, "")
    End Sub

    Private Sub ViewJournalEntryButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ViewJournalEntryButton.Click
        If Obj Is Nothing OrElse Not Obj.ID > 0 Then Exit Sub
        MDIParent1.LaunchForm(GetType(F_GeneralLedgerEntry), False, False, Obj.ID, Obj.ID)
    End Sub

    Private Sub OpenAdvanceReportButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles OpenAdvanceReportButton.Click
        If Obj Is Nothing OrElse Not Obj.AdvanceReportID > 0 Then Exit Sub
        MDIParent1.LaunchForm(GetType(F_AdvanceReport), False, False, _
            Obj.AdvanceReportID, Obj.AdvanceReportID)
    End Sub


    Public Function GetMailDropDownItems() As System.Windows.Forms.ToolStripDropDown _
       Implements ISupportsPrinting.GetMailDropDownItems
        Return Nothing
    End Function

    Public Function GetPrintDropDownItems() As System.Windows.Forms.ToolStripDropDown _
        Implements ISupportsPrinting.GetPrintDropDownItems
        Return Nothing
    End Function

    Public Function GetPrintPreviewDropDownItems() As System.Windows.Forms.ToolStripDropDown _
        Implements ISupportsPrinting.GetPrintPreviewDropDownItems
        Return Nothing
    End Function

    Public Sub OnMailClick(ByVal sender As Object, ByVal e As System.EventArgs) _
        Implements ISupportsPrinting.OnMailClick
        If Obj Is Nothing Then Exit Sub

        Using frm As New F_SendObjToEmail(Obj, 0)
            frm.ShowDialog()
        End Using

    End Sub

    Public Sub OnPrintClick(ByVal sender As Object, ByVal e As System.EventArgs) _
        Implements ISupportsPrinting.OnPrintClick
        If Obj Is Nothing Then Exit Sub
        Try
            PrintObject(Obj, False, 0)
        Catch ex As Exception
            ShowError(ex)
        End Try
    End Sub

    Public Sub OnPrintPreviewClick(ByVal sender As Object, ByVal e As System.EventArgs) _
        Implements ISupportsPrinting.OnPrintPreviewClick
        If Obj Is Nothing Then Exit Sub
        Try
            PrintObject(Obj, True, 0)
        Catch ex As Exception
            ShowError(ex)
        End Try
    End Sub

    Public Function SupportsEmailing() As Boolean _
        Implements ISupportsPrinting.SupportsEmailing
        Return True
    End Function



    Private Function SaveObj() As Boolean

        If Not Obj.IsDirty Then Return True

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

        Using bm As New BindingsManager(TillIncomeOrderBindingSource, _
            BookEntryItemsSortedBindingSource, Nothing, True, False)

            Try
                Obj = LoadObject(Of TillIncomeOrder)(Obj, "Save", False)
            Catch ex As Exception
                ShowError(ex)
                Return False
            End Try

            bm.SetNewDataSource(Obj)

        End Using

        ConfigureButtons()

        MsgBox(Answer, MsgBoxStyle.Information, "Info")

        Return True

    End Function

    Private Sub CancelObj()
        If Obj Is Nothing OrElse Obj.IsNew OrElse Not Obj.IsDirty Then Exit Sub
        Using bm As New BindingsManager(TillIncomeOrderBindingSource, _
            BookEntryItemsSortedBindingSource, Nothing, True, True)
        End Using
    End Sub

    Private Function SetDataSources() As Boolean

        If Not PrepareCache(Me, GetType(HelperLists.DocumentSerialInfoList), _
            GetType(HelperLists.PersonInfoList), GetType(HelperLists.AccountInfoList), _
            GetType(HelperLists.CashAccountInfoList)) Then Return False

        Try

            LoadDocumentSerialInfoListToCombo(DocumentSerialComboBox, _
                Settings.DocumentSerialType.TillIncomeOrder, True, False)
            LoadCashAccountInfoListToGridCombo(AccountAccGridComboBox, True)
            LoadPersonInfoListToGridCombo(PayerAccGridComboBox, True, True, False, True)
            LoadAccountInfoListToGridCombo(AccountCurrencyRateChangeImpactAccGridComboBox, _
                True, 5, 6)
            LoadAccountInfoListToGridCombo(DataGridViewTextBoxColumn2, True, 2, 3, 4, 5)

        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
            Return False
        End Try

        Return True

    End Function

    Private Sub ConfigureButtons()

        If Obj Is Nothing Then Exit Sub

        AccountAccGridComboBox.Enabled = Obj.ChronologicValidator.FinancialDataCanChange
        SumAccTextBox.ReadOnly = Not Obj.ChronologicValidator.FinancialDataCanChange
        CurrencyRateInAccountAccTextBox.ReadOnly = Not Obj.ChronologicValidator.FinancialDataCanChange
        AccountCurrencyRateChangeImpactAccGridComboBox.Enabled = Obj.ChronologicValidator.FinancialDataCanChange
        CurrencyRateChangeImpactAccTextBox.ReadOnly = Not Obj.ChronologicValidator.FinancialDataCanChange
        GetCurrencyRatesButton.Enabled = Obj.ChronologicValidator.FinancialDataCanChange
        BookEntryItemsDataGridView.AllowUserToAddRows = Obj.ChronologicValidator.FinancialDataCanChange
        BookEntryItemsDataGridView.AllowUserToDeleteRows = Obj.ChronologicValidator.FinancialDataCanChange
        BookEntryItemsDataGridView.ReadOnly = Not Obj.ChronologicValidator.FinancialDataCanChange

        LimitationsButton.Visible = Not String.IsNullOrEmpty(Obj.ChronologicValidator.LimitsExplanation.Trim)

    End Sub

End Class