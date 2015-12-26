Imports ApskaitaObjects.HelperLists
Public Class F_IndirectRelationInfoList

    Private Obj As IndirectRelationInfoList = Nothing
    Private Loading As Boolean = True
    Private JournalEntryID As Integer = 0


    Public ReadOnly Property ObjectID() As Integer
        Get
            If Not Obj Is Nothing Then Return Obj.ID
            Return 0
        End Get
    End Property

    Public ReadOnly Property ObjectType() As System.Type
        Get
            Return GetType(IndirectRelationInfoList)
        End Get
    End Property


    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Sub New(ByVal nJournalEntryID As Integer)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        JournalEntryID = nJournalEntryID

    End Sub


    Private Sub F_IndirectRelationInfoList_Activated(ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles Me.Activated

        If Me.WindowState = FormWindowState.Maximized AndAlso MyCustomSettings.AutoSizeForm Then _
            Me.WindowState = FormWindowState.Normal

        If Loading Then
            Loading = False
            Exit Sub
        End If

    End Sub

    Private Sub F_IndirectRelationInfoList_FormClosing(ByVal sender As Object, _
        ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        GetFormLayout(Me)
        GetDataGridViewLayOut(IndirectRelationInfoListDataGridView)
    End Sub

    Private Sub F_IndirectRelationInfoList_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        If Not IndirectRelationInfoList.CanGetObject Then
            MsgBox("Klaida. Jūsų teisių nepakanka šiai informacijai gauti.", _
                MsgBoxStyle.Exclamation, "Klaida.")
            DisableAllControls(Me)
            Exit Sub
        End If

        Try

            If JournalEntryID > 0 Then

                Try
                    Obj = LoadObject(Of IndirectRelationInfoList)(Nothing, _
                        "GetIndirectRelationInfoList", True, JournalEntryID)
                Catch ex As Exception
                    ShowError(ex)
                    DisableAllControls(Me)
                    Exit Sub
                End Try

            End If

        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
        End Try

        SetFormLayout(Me)
        SetDataGridViewLayOut(IndirectRelationInfoListDataGridView)

        If Not Obj Is Nothing Then

            IndirectRelationInfoListBindingSource.DataSource = Obj
            ContentTextBox.Text = Obj.Content
            CorrespondingAccountsTextBox.Text = Obj.CorrespondingAccounts
            DateTextBox.Text = Obj.Date.ToString("yyyy-MM-dd")
            DocNumberTextBox.Text = Obj.DocNumber
            DocTypeHumanReadableTextBox.Text = Obj.DocTypeHumanReadable
            IDTextBox.Text = Obj.ID.ToString
            InsertDateTextBox.Text = Obj.InsertDate.ToString("yyyy-MM-dd HH:mm:ss")
            PersonCodeTextBox.Text = Obj.PersonCode
            PersonNameTextBox.Text = Obj.PersonName
            SumAccTextBox.DecimalValue = Obj.Sum
            UpdateDateTextBox.Text = Obj.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss")

        End If

    End Sub


    Private Sub IOkButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles IOkButton.Click
        Me.Hide()
        Me.Close()
    End Sub


    Private Sub IndirectRelationInfoListDataGridView_CellDoubleClick(ByVal sender As System.Object, _
        ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) _
        Handles IndirectRelationInfoListDataGridView.CellDoubleClick

        If e.RowIndex < 0 Then Exit Sub

        Dim selectedItem As IndirectRelationInfo = Nothing
        Try
            selectedItem = DirectCast(IndirectRelationInfoListDataGridView.Rows(e.RowIndex). _
                DataBoundItem, IndirectRelationInfo)
        Catch ex As Exception
        End Try
        If selectedItem Is Nothing Then Exit Sub

        If selectedItem.Type = IndirectRelationType.PayoutToResident Then

            MDIParent1.LaunchForm(GetType(F_PayOutNaturalPerson), False, False, _
                selectedItem.ID, selectedItem.ID)

        ElseIf selectedItem.Type = IndirectRelationType.LongTermAssetsPurchase Then

            MDIParent1.LaunchForm(GetType(F_LongTermAsset), False, False, _
                selectedItem.ID, selectedItem.ID)

        ElseIf selectedItem.Type = IndirectRelationType.LongTermAssetsOperation Then

            OpenOldAssetOperationInEditForm(selectedItem.ID, _
                Assets.OperationInfo.OperationInfoFetchType.OperationID)

        ElseIf selectedItem.Type = IndirectRelationType.GoodsOperation Then

            Select Case selectedItem.GoodsOperationType
                Case Goods.GoodsOperationType.Acquisition
                    MDIParent1.LaunchForm(GetType(F_GoodsOperationAcquisition), False, False, _
                        selectedItem.ID, selectedItem.ID)
                Case Goods.GoodsOperationType.ConsignmentAdditionalCosts
                    MDIParent1.LaunchForm(GetType(F_GoodsOperationAdditionalCosts), False, False, _
                        selectedItem.ID, selectedItem.ID)
                Case Goods.GoodsOperationType.ConsignmentDiscount
                    MDIParent1.LaunchForm(GetType(F_GoodsOperationDiscount), False, False, _
                        selectedItem.ID, selectedItem.ID)
                Case Goods.GoodsOperationType.Discard
                    MDIParent1.LaunchForm(GetType(F_GoodsOperationDiscard), False, False, _
                        selectedItem.ID, selectedItem.ID)
                Case Goods.GoodsOperationType.PriceCut
                    MDIParent1.LaunchForm(GetType(F_GoodsOperationPriceCut), False, False, _
                        selectedItem.ID, selectedItem.ID, False)
                Case Goods.GoodsOperationType.Transfer
                    MDIParent1.LaunchForm(GetType(F_GoodsOperationTransfer), False, False, _
                        selectedItem.ID, selectedItem.ID)
                Case Goods.GoodsOperationType.ValuationMethodChange
                    MDIParent1.LaunchForm(GetType(F_GoodsOperationValuationMethod), False, False, _
                        selectedItem.ID, selectedItem.ID, False)
                Case Goods.GoodsOperationType.AccountDiscountsChange, _
                    Goods.GoodsOperationType.AccountPurchasesChange, _
                    Goods.GoodsOperationType.AccountSalesNetCostsChange, _
                    Goods.GoodsOperationType.AccountValueReductionChange
                    MDIParent1.LaunchForm(GetType(F_GoodsOperationAccountChange), False, False, _
                        selectedItem.ID, selectedItem.ID)
            End Select

        End If


    End Sub

End Class