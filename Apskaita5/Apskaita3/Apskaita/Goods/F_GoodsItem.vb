Imports ApskaitaObjects.Goods
Imports ApskaitaObjects.HelperLists
Public Class F_GoodsItem
    Implements IObjectEditForm

    Private Obj As GoodsItem = Nothing
    Private Loading As Boolean = True
    Private GoodsID As Integer = 0


    Public ReadOnly Property ObjectID() As Integer Implements IObjectEditForm.ObjectID
        Get
            If Not Obj Is Nothing Then Return Obj.ID
            Return 0
        End Get
    End Property

    Public ReadOnly Property ObjectType() As System.Type Implements IObjectEditForm.ObjectType
        Get
            Return GetType(GoodsItem)
        End Get
    End Property


    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Sub New(ByVal nGoodsID As Integer)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        GoodsID = nGoodsID

    End Sub


    Private Sub F_GoodsItem_Activated(ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles Me.Activated

        If Me.WindowState = FormWindowState.Maximized AndAlso MyCustomSettings.AutoSizeForm Then _
            Me.WindowState = FormWindowState.Normal

        If Loading Then
            Loading = False
            Exit Sub
        End If

        If Not PrepareCache(Me, GetType(WarehouseInfoList), _
            GetType(GoodsGroupInfoList), GetType(AccountInfoList), _
            GetType(CompanyRegionalInfoList), GetType(TaxRateInfoList)) Then Exit Sub

    End Sub

    Private Sub F_GoodsItem_FormClosing(ByVal sender As Object, _
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

    Private Sub F_GoodsItem_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        If Not Goods.GoodsItem.CanGetObject Then
            MsgBox("Klaida. Jūsų teisių nepakanka šiai informacijai gauti.", _
                MsgBoxStyle.Exclamation, "Klaida.")
            DisableAllControls(Me)
            Exit Sub
        ElseIf Not GoodsID > 0 AndAlso Not Goods.GoodsItem.CanAddObject Then
            MsgBox("Klaida. Jūsų teisių nepakanka naujai prekei įvesti.", _
                MsgBoxStyle.Exclamation, "Klaida.")
            DisableAllControls(Me)
            Exit Sub
        End If

        If Not SetDataSources() Then Exit Sub

        Try

            If GoodsID > 0 Then

                Try
                    Obj = LoadObject(Of GoodsItem)(Nothing, "GetGoodsItem", True, GoodsID)
                Catch ex As Exception
                    ShowError(ex)
                    DisableAllControls(Me)
                    Exit Sub
                End Try

            Else

                Obj = GoodsItem.NewGoodsItem

            End If

        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
        End Try

        GoodsItemBindingSource.DataSource = Obj

        SetFormLayout(Me)
        SetDataGridViewLayOut(RegionalContentsDataGridView)
        SetDataGridViewLayOut(RegionalPricesDataGridView)

        If Not Obj.IsNew AndAlso Not GoodsItem.CanEditObject Then
            MsgBox("Klaida. Jūsų teisių nepakanka duomenims redaguoti.", _
                MsgBoxStyle.Exclamation, "Klaida.")
            DisableAllControls(Me)
            Exit Sub
        End If

        AccountValueReductionAccGridComboBox.Enabled = Not Obj.PriceCutsExist
        AccountPurchasesAccGridComboBox.Enabled = (Obj.OldAccountingMethod <> _
            GoodsAccountingMethod.Periodic OrElse Not Obj.IsInUse)
        AccountSalesNetCostsAccGridComboBox.Enabled = (Obj.OldAccountingMethod <> _
            GoodsAccountingMethod.Periodic OrElse Not Obj.IsInUse)
        AccountDiscountsAccGridComboBox.Enabled = (Obj.OldAccountingMethod <> _
            GoodsAccountingMethod.Periodic OrElse Not Obj.IsInUse)
        AccountingMethodHumanReadableComboBox.Enabled = Not Obj.IsInUse
        DefaultValuationMethodHumanReadableComboBox.Enabled = Not Obj.IsInUse
        EditedBaner.Visible = Not Obj.IsNew

        Dim h As New EditableDataGridViewHelper(RegionalContentsDataGridView)
        Dim g As New EditableDataGridViewHelper(RegionalPricesDataGridView)

    End Sub


    Private Sub OkButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles nOkButton.Click
        If Obj Is Nothing Then Exit Sub
        If Not SaveObj() Then Exit Sub
        Me.Hide()
        Me.Close()
    End Sub

    Private Sub ApplyButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ApplyButton.Click
        If Obj Is Nothing Then Exit Sub
        If Not SaveObj() Then Exit Sub
        EditedBaner.Visible = Not Obj.IsNew
        If Not Obj.IsNew AndAlso Not GoodsItem.CanEditObject Then
            MsgBox("Klaida. Jūsų teisių nepakanka duomenims redaguoti.", _
                MsgBoxStyle.Exclamation, "Klaida.")
            DisableAllControls(Me)
            Exit Sub
        End If
    End Sub

    Private Sub CancelButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles nCancelButton.Click
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

        Using bm As New BindingsManager(GoodsItemBindingSource, RegionalPricesSortedBindingSource, _
            RegionalContentsSortedBindingSource, True, False)

            Try
                Obj = LoadObject(Of GoodsItem)(Obj, "Save", False)
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

        Using bm As New BindingsManager(GoodsItemBindingSource, RegionalPricesSortedBindingSource, _
            RegionalContentsSortedBindingSource, True, True)
        End Using

    End Sub

    Private Function SetDataSources() As Boolean

        If Not PrepareCache(Me, GetType(WarehouseInfoList), _
            GetType(GoodsGroupInfoList), GetType(AccountInfoList), _
            GetType(CompanyRegionalInfoList), GetType(TaxRateInfoList)) Then Return False

        Try

            LoadEnumHumanReadableListToComboBox(AccountingMethodHumanReadableComboBox, _
                GetType(GoodsAccountingMethod), False)
            LoadEnumHumanReadableListToComboBox(DefaultValuationMethodHumanReadableComboBox, _
                GetType(GoodsValuationMethod), False)
            LoadEnumLocalizedListToComboBox(TradedTypeHumanReadableComboBox, _
                GetType(Documents.TradedItemType), False)

            LoadTaxRateListToCombo(DefaultVatRatePurchaseComboBox, TaxRateType.Vat)
            LoadTaxRateListToCombo(DefaultVatRateSalesComboBox, TaxRateType.Vat)

            LoadAccountInfoListToGridCombo(AccountPurchasesAccGridComboBox, True, 6)
            LoadAccountInfoListToGridCombo(AccountSalesNetCostsAccGridComboBox, True, 6)
            LoadAccountInfoListToGridCombo(AccountDiscountsAccGridComboBox, True, 6)
            LoadAccountInfoListToGridCombo(AccountValueReductionAccGridComboBox, True, 2)

            LoadLanguageListToComboBox(DataGridViewTextBoxColumn3, True)
            LoadCurrencyCodeListToComboBox(DataGridViewTextBoxColumn8, True)

            LoadGoodsGroupInfoListToGridCombo(GroupAccGridComboBox, True)
            LoadWarehouseInfoListToGridCombo(DefaultWarehouseAccGridComboBox, True)

        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
            Return False
        End Try

        Return True

    End Function

End Class