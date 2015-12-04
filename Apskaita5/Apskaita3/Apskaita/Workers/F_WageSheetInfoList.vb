Imports ApskaitaObjects.Workers
Imports ApskaitaObjects.ActiveReports
Public Class F_WageSheetInfoList
    Implements ISupportsPrinting

    Private Obj As WageSheetInfoList

    Private Sub F_WageSheetInfoList_Activated(ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles Me.Activated
        If Me.WindowState = FormWindowState.Maximized AndAlso MyCustomSettings.AutoSizeForm Then _
            Me.WindowState = FormWindowState.Normal
    End Sub

    Private Sub F_WageSheetInfoList_FormClosing(ByVal sender As Object, _
        ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        GetDataGridViewLayOut(WageSheetInfoListDataGridView)
        GetFormLayout(Me)
    End Sub

    Private Sub F_WageSheetInfoList_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        RefreshButton.Enabled = WageSheetInfoList.CanGetObject
        NewButton.Enabled = WageSheet.CanAddObject

        DateFromDateTimePicker.Value = Today.Subtract(New TimeSpan(90, 0, 0, 0))

        AddDGVColumnSelector(WageSheetInfoListDataGridView)
        SetDataGridViewLayOut(WageSheetInfoListDataGridView)
        SetFormLayout(Me)

        InitializeMenu(Of WageSheetInfo)()

    End Sub


    Private Sub RefreshButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles RefreshButton.Click
        DoRefresh(DateFromDateTimePicker.Value.Date, DateToDateTimePicker.Value.Date)
    End Sub

    Private Sub ShowPayedOutCheckBox_CheckedChanged(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ShowPayedOutCheckBox.CheckedChanged
        If Obj Is Nothing Then Exit Sub
        Obj.ApplyFilter(ShowPayedOutCheckBox.Checked)
    End Sub

    Private Sub NewButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles NewButton.Click
        NewItem(Nothing)
    End Sub

    Private Sub InitializeMenu(Of T As WageSheetInfo)()

        Dim w As New ToolStripHelper(Of T)(WageSheetInfoListDataGridView, _
            ContextMenuStrip1, "", True)

        w.AddMenuItemHandler(ChangeItem_MenuItem, New DelegateContainer(Of T)(AddressOf ChangeItem))
        w.AddMenuItemHandler(DeleteItem_MenuItem, New DelegateContainer(Of T)(AddressOf DeleteItem))
        w.AddMenuItemHandler(NewItem_MenuItem, New DelegateContainer(Of T)(AddressOf NewItem))

        w.AddButtonHandler("Keisti", "Keisti darbo užmokesčio žiniaraščio duomenis.", _
            New DelegateContainer(Of T)(AddressOf ChangeItem))
        w.AddButtonHandler("Ištrinti", "Pašalinti darbo užmokesčio žiniaraščio duomenis iš duomenų bazės.", _
            New DelegateContainer(Of T)(AddressOf DeleteItem))

    End Sub

    Private Sub ChangeItem(ByVal item As WageSheetInfo)
        If item Is Nothing Then Exit Sub
        MDIParent1.LaunchForm(GetType(F_WageSheet), False, False, item.ID, item.ID)
    End Sub

    Private Sub DeleteItem(ByVal item As WageSheetInfo)

        If item Is Nothing Then Exit Sub

        For Each frm As Form In MDIParent1.MdiChildren
            If TypeOf frm Is F_WageSheet AndAlso DirectCast(frm, F_WageSheet).ObjectID = item.ID Then
                MsgBox("Negalima pašalinti duomenų, kol jie yra redaguojami. Uždarykite redagavimo formą.", _
                    MsgBoxStyle.Exclamation, "Klaida")
                frm.Activate()
                Exit Sub
            End If
        Next

        If Not YesOrNo("Ar tikrai norite pašalinti darbo užmokesčio žiniaraščio duomenis?") Then Exit Sub

        Try
            Using busy As New StatusBusy
                WageSheet.DeleteWageSheet(item.ID)
            End Using
        Catch ex As Exception
            ShowError(ex)
            Exit Sub
        End Try

        If Not YesOrNo("Darbo užmokesčio žiniaraščio duomenys sėkmingai pašalinti. Atnaujinti sąrašą?") Then Exit Sub

        DoRefresh(Obj.DateFrom, Obj.DateTo)

    End Sub

    Private Sub NewItem(ByVal item As WageSheetInfo)

        Dim cYear, cMonth As Integer
        Using frm As New F_NewSheet("Naujas darbo užmokesčio žiniaraštis")
            frm.ShowDialog()
            If Not frm.Result Then Exit Sub
            cYear = frm.Year
            cMonth = frm.Month
        End Using

        MDIParent1.LaunchForm(GetType(F_WageSheet), False, False, 0, cYear, cMonth)

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


    Private Sub DoRefresh(ByVal dateFrom As Date, ByVal dateTo As Date)

        Using bm As New BindingsManager(WageSheetInfoListBindingSource, _
            Nothing, Nothing, False, True)

            Try
                Obj = LoadObject(Of WageSheetInfoList)(Nothing, "GetWageSheetInfoList", _
                    True, dateFrom, dateTo)
            Catch ex As Exception
                ShowError(ex)
                Exit Sub
            End Try

            Obj.ApplyFilter(ShowPayedOutCheckBox.Checked)

            bm.SetNewDataSource(Obj.GetFilteredList())

            TotalSumAfterDeductionsAccBox.DecimalValue = Obj.TotalSumAfterDeductions
            TotalSumPayedOutAccBox.DecimalValue = Obj.TotalSumPayedOut

        End Using

        WageSheetInfoListDataGridView.Select()

    End Sub

End Class