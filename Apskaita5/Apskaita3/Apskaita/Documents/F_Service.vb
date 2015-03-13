Imports ApskaitaObjects.Documents
Imports ApskaitaObjects.HelperLists
Public Class F_Service
    Implements IObjectEditForm

    Private Obj As Service
    Private EditObjectID As Integer = -1
    Private Loading As Boolean = True


    Public Sub New(ByVal ServiceID As Integer)
        InitializeComponent()
        EditObjectID = ServiceID
    End Sub

    Public Sub New()
        InitializeComponent()
    End Sub


    Public ReadOnly Property ObjectID() As Integer Implements IObjectEditForm.ObjectID
        Get
            If Obj Is Nothing Then Return 0
            Return Obj.ID
        End Get
    End Property

    Public ReadOnly Property ObjectType() As System.Type Implements IObjectEditForm.ObjectType
        Get
            Return GetType(Service)
        End Get
    End Property


    Private Sub F_Service_Activated(ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles Me.Activated

        If Me.WindowState = FormWindowState.Maximized AndAlso MyCustomSettings.AutoSizeForm Then _
            Me.WindowState = FormWindowState.Normal

        If Loading Then
            Loading = False
            Exit Sub
        End If

        If Not PrepareCache(Me, GetType(HelperLists.AccountInfoList), _
            GetType(HelperLists.CompanyRegionalInfoList), GetType(Settings.CommonSettings)) Then Exit Sub

    End Sub

    Private Sub F_Service_FormClosing(ByVal sender As Object, _
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

        GetFormLayout(Me)
        GetDataGridViewLayOut(RegionalContentsDataGridView)
        GetDataGridViewLayOut(RegionalPricesDataGridView)

    End Sub

    Private Sub F_Service_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        If Not Documents.Service.CanGetObject Then
            MsgBox("Klaida. Jūsų teisių nepakanka šiai informacijai gauti.", _
                MsgBoxStyle.Exclamation, "Klaida.")
            DisableAllControls(Me)
            Exit Sub
        ElseIf Not EditObjectID > 0 AndAlso Not Documents.Service.CanAddObject Then
            MsgBox("Klaida. Jūsų teisių nepakanka naujam dokumentui įvesti.", _
                MsgBoxStyle.Exclamation, "Klaida.")
            DisableAllControls(Me)
            Exit Sub
        End If

        If Not PrepareCache(Me, GetType(HelperLists.AccountInfoList), _
            GetType(HelperLists.CompanyRegionalInfoList), GetType(Settings.CommonSettings)) Then Exit Sub

        If Not SetDataSources() Then Exit Sub

        Try

            If EditObjectID > 0 Then

                Try
                    Obj = LoadObject(Of Service)(Nothing, "GetService", False, EditObjectID)
                Catch ex As Exception
                    ShowError(ex)
                    DisableAllControls(Me)
                    Exit Sub
                End Try

            Else

                Obj = Service.NewService

            End If

        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
        End Try

        ServiceBindingSource.DataSource = Obj

        SetFormLayout(Me)
        SetDataGridViewLayOut(RegionalContentsDataGridView)
        SetDataGridViewLayOut(RegionalPricesDataGridView)

        If Not Obj.IsNew AndAlso Not Documents.TillIncomeOrder.CanEditObject Then
            MsgBox("Klaida. Jūsų teisių nepakanka duomenims redaguoti.", _
                MsgBoxStyle.Exclamation, "Klaida.")
            DisableAllControls(Me)
            Exit Sub
        End If

        Dim h As New EditableDataGridViewHelper(RegionalContentsDataGridView)
        Dim g As New EditableDataGridViewHelper(RegionalPricesDataGridView)

    End Sub


    Private Sub OkButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles IOkButton.Click
        If Obj Is Nothing Then Exit Sub
        If Not SaveObj() Then Exit Sub
        Me.Hide()
        Me.Close()
    End Sub

    Private Sub ApplyButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles IApplyButton.Click
        If Obj Is Nothing Then Exit Sub
        If Not SaveObj() Then Exit Sub
        If Not Obj.IsNew AndAlso Not Service.CanEditObject Then
            MsgBox("Klaida. Jūsų teisių nepakanka duomenims redaguoti.", _
                MsgBoxStyle.Exclamation, "Klaida.")
            DisableAllControls(Me)
            Exit Sub
        End If
    End Sub

    Private Sub CancelButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ICancelButton.Click
        If Obj Is Nothing Then Exit Sub
        CancelObj()
    End Sub


    Private Function SaveObj() As Boolean

        If Obj Is Nothing Then Exit Function

        If Not Obj.IsDirty Then Return True

        If Not Obj.IsValid Then
            MsgBox("Formoje yra klaidų:" & vbCrLf & Obj.GetAllBrokenRules, MsgBoxStyle.Exclamation, "Klaida.")
            Return False
        End If

        Dim Question, Answer As String
        If Not String.IsNullOrEmpty(Obj.GetAllWarnings.Trim) Then
            Question = "DĖMESIO. Duomenyse gali būti klaidų: " & vbCrLf _
                & Obj.GetAllWarnings & vbCrLf
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

        Using bm As New BindingsManager(ServiceBindingSource, RegionalPricesSortedBindingSource, _
            RegionalContentsSortedBindingSource, True, False)

            Try
                Obj = LoadObject(Of Service)(Obj, "Save", False)
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

        If Obj Is Nothing OrElse Obj.IsNew OrElse Not Obj.IsDirty Then Exit Sub

        Using bm As New BindingsManager(ServiceBindingSource, RegionalPricesSortedBindingSource, _
            RegionalContentsSortedBindingSource, True, True)
        End Using

    End Sub

    Private Function SetDataSources() As Boolean

        Try

            LoadEnumHumanReadableListToComboBox(TypeHumanReadableComboBox, GetType(TradedItemType), False)
            LoadCurrencyCodeListToComboBox(DataGridViewTextBoxColumn2, True)

            LoadTaxRateListToCombo(RateVatPurchaseComboBox, TaxTarifType.Vat)
            LoadTaxRateListToCombo(RateVatSalesComboBox, TaxTarifType.Vat)
            LoadLanguageListToComboBox(DataGridViewTextBoxColumn7, True)
            LoadAccountInfoListToGridCombo(AccountPurchaseAccGridComboBox, True, 2, 3, 6)
            LoadAccountInfoListToGridCombo(AccountSalesAccGridComboBox, True, 2, 3, 4, 5)
            LoadAccountInfoListToGridCombo(AccountVatPurchaseAccGridComboBox, True, 2, 6)
            LoadAccountInfoListToGridCombo(AccountVatSalesAccGridComboBox, True, 4)

        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
            Return False
        End Try

        Return True

    End Function

End Class