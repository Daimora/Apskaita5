Imports ApskaitaObjects.ActiveReports
Imports ApskaitaObjects.HelperLists
Imports ApskaitaObjects.Goods
Public Class F_GoodsTurnoverInfoList
    Implements ISupportsPrinting

    Private Obj As GoodsTurnoverInfoList
    Private Loading As Boolean = True


    Private Sub F_GoodsTurnoverInfoList_Activated(ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles Me.Activated

        If Me.WindowState = FormWindowState.Maximized AndAlso MyCustomSettings.AutoSizeForm Then _
            Me.WindowState = FormWindowState.Normal

        If Loading Then
            Loading = False
            Exit Sub
        End If

        If Not PrepareCache(Me, GetType(HelperLists.GoodsGroupInfoList), _
            GetType(HelperLists.WarehouseInfoList)) Then Exit Sub

    End Sub

    Private Sub F_GoodsTurnoverInfoList_FormClosing(ByVal sender As Object, _
        ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        GetDataGridViewLayOut(GoodsTurnoverInfoListDataGridView)
        GetFormLayout(Me)
    End Sub

    Private Sub F_GoodsTurnoverInfoList_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        If Not SetDataSources() Then Exit Sub

        DateFromDateTimePicker.Value = Today.Subtract(New TimeSpan(30, 0, 0, 0))

        AddDGVColumnSelector(GoodsTurnoverInfoListDataGridView)

        SetDataGridViewLayOut(GoodsTurnoverInfoListDataGridView)
        SetFormLayout(Me)

    End Sub


    Private Sub RefreshButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles RefreshButton.Click

        Dim warehouse As WarehouseInfo = Nothing
        Try
            warehouse = DirectCast(WarehouseInfoAccGridComboBox.SelectedValue, WarehouseInfo)
        Catch ex As Exception
        End Try
        Dim group As GoodsGroupInfo = Nothing
        Try
            group = DirectCast(GoodsGroupInfoAccGridComboBox.SelectedValue, GoodsGroupInfo)
        Catch ex As Exception
        End Try

        DoRefresh(DateFromDateTimePicker.Value.Date, DateToDateTimePicker.Value.Date, group, warehouse)

    End Sub

    Private Sub GoodsTurnoverInfoListDataGridView_CellDoubleClick(ByVal sender As System.Object, _
        ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) _
        Handles GoodsTurnoverInfoListDataGridView.CellDoubleClick

        If e.RowIndex < 0 OrElse Obj Is Nothing Then Exit Sub

        Dim tmp As GoodsTurnoverInfo = Nothing
        Try
            tmp = CType(GoodsTurnoverInfoListDataGridView.Rows(e.RowIndex).DataBoundItem, GoodsTurnoverInfo)
        Catch ex As Exception
            ShowError(ex)
            Exit Sub
        End Try
        If tmp Is Nothing Then
            MsgBox("Klaida. Nepavyko nustatyti pasirinktos prekės.", _
                MsgBoxStyle.Exclamation, "Klaida.")
            Exit Sub
        End If


        Dim ats As String = Ask("", New ButtonStructure("Operacijos", "Žiūrėti operacijų sąrašą."), _
            New ButtonStructure("Keisti", "Keisti bendrus prekės duomenis."), _
            New ButtonStructure("Ištrinti", "Ištrinti prekės duomenis iš duomenų bazės."), _
            New ButtonStructure("Atšaukti", "Nieko nedaryti."))

        If ats <> "Operacijos" AndAlso ats <> "Keisti" AndAlso ats <> "Ištrinti" Then Exit Sub

        If ats = "Operacijos" Then

            Dim wd As Integer = 0
            If Not Obj.Warehouse Is Nothing AndAlso Obj.Warehouse.ID > 0 Then wd = Obj.Warehouse.ID

            MDIParent1.LaunchForm(GetType(F_GoodsOperationInfoListParent), False, False, 0, _
                Obj.DateFrom, Obj.DateTo, tmp.ID, wd)

        ElseIf ats = "Ištrinti" Then

            If Not YesOrNo("Ar tikrai norite pašalinti pasirinktos prekės duomenis iš duomenų bazės?") Then Exit Sub

            Try
                Using busy As New StatusBusy
                    Goods.GoodsItem.DeleteGoodsItem(tmp.ID)
                End Using
            Catch ex As Exception
                ShowError(ex)
                Exit Sub
            End Try

            If Not YesOrNo("Prekės duomenys sėkmingai pašalinti iš duomenų bazės." _
                & vbCrLf & "Atnaujinti ataskaitą?") Then Exit Sub

            DoRefresh(Obj.DateFrom, Obj.DateTo, Obj.Group, Obj.Warehouse)

        Else

            MDIParent1.LaunchForm(GetType(F_GoodsItem), False, False, tmp.ID, tmp.ID)

        End If

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


    Private Sub DoRefresh(ByVal nDateFrom As Date, ByVal nDateTo As Date, _
        ByVal nGroup As GoodsGroupInfo, ByVal nWarehouse As WarehouseInfo)

        Using bm As New BindingsManager(GoodsTurnoverInfoListBindingSource, _
            Nothing, Nothing, False, True)

            Try
                Obj = LoadObject(Of ActiveReports.GoodsTurnoverInfoList)(Nothing, _
                    "GetGoodsTurnoverInfoList", True, nDateFrom, nDateTo, nGroup, _
                    nWarehouse, NameOrCodeFragmentTextBox.Text)
            Catch ex As Exception
                ShowError(ex)
                Exit Sub
            End Try

            bm.SetNewDataSource(Obj.GetSortedList)

        End Using

        GoodsTurnoverInfoListDataGridView.Select()

    End Sub

    Private Function SetDataSources() As Boolean

        If Not PrepareCache(Me, GetType(HelperLists.GoodsGroupInfoList), _
            GetType(HelperLists.WarehouseInfoList)) Then Exit Function

        Try

            LoadGoodsGroupInfoListToGridCombo(GoodsGroupInfoAccGridComboBox, True)
            LoadWarehouseInfoListToGridCombo(WarehouseInfoAccGridComboBox, True)

        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
            Return False
        End Try

        Return True

    End Function

End Class