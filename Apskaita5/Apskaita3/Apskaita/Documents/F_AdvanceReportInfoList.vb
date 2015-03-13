Imports ApskaitaObjects.HelperLists
Imports ApskaitaObjects.ActiveReports
Public Class F_AdvanceReportInfoList
    Implements ISupportsPrinting

    Private Obj As AdvanceReportInfoList
    Private Loading As Boolean = True

    Private Sub F_AdvanceReportInfoList_Activated(ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles Me.Activated

        If Me.WindowState = FormWindowState.Maximized AndAlso MyCustomSettings.AutoSizeForm Then _
            Me.WindowState = FormWindowState.Normal

        If Loading Then
            Loading = False
            Exit Sub
        End If

        If Not PrepareCache(Me, GetType(HelperLists.PersonInfoList)) Then Exit Sub

    End Sub

    Private Sub F_AdvanceReportInfoList_FormClosing(ByVal sender As Object, _
        ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        GetDataGridViewLayOut(AdvanceReportInfoListDataGridView)
        GetFormLayout(Me)
    End Sub

    Private Sub F_AdvanceReportInfoList_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        If Not PrepareCache(Me, GetType(HelperLists.PersonInfoList)) Then Exit Sub

        If Not SetDataSources() Then Exit Sub

        DateFromDateTimePicker.Value = Today.Subtract(New TimeSpan(30, 0, 0, 0))

        AddDGVColumnSelector(AdvanceReportInfoListDataGridView)

        SetDataGridViewLayOut(AdvanceReportInfoListDataGridView)
        SetFormLayout(Me)

        InitializeMenu(Of AdvanceReportInfo)()

    End Sub


    Private Sub RefreshButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles RefreshButton.Click

        Using bm As New BindingsManager(AdvanceReportInfoListBindingSource, _
            Nothing, Nothing, False, True)

            Try
                Obj = LoadObject(Of AdvanceReportInfoList)(Nothing, _
                    "GetAdvanceReportInfoList", True, DateFromDateTimePicker.Value.Date, _
                    DateToDateTimePicker.Value.Date, LoadObjectFromCombo(Of PersonInfo) _
                    (PersonInfoAccGridComboBox, "ID"))
            Catch ex As Exception
                ShowError(ex)
                Exit Sub
            End Try

            bm.SetNewDataSource(Obj)

        End Using

        AdvanceReportInfoListDataGridView.Select()

    End Sub


    Private Sub InitializeMenu(Of T As AdvanceReportInfo)()

        Dim w As New ToolStripHelper(Of T)(AdvanceReportInfoListDataGridView, _
            ContextMenuStrip1, "", True)

        w.AddMenuItemHandler(ChangeItem_MenuItem, New DelegateContainer(Of T)(AddressOf ChangeItem))
        w.AddMenuItemHandler(DeleteItem_MenuItem, New DelegateContainer(Of T)(AddressOf DeleteItem))
        w.AddMenuItemHandler(NewItem_MenuItem, New DelegateContainer(Of T)(AddressOf NewItem))
        w.AddMenuItemHandler(AttachedOrder_MenuItem, New DelegateContainer(Of T)(AddressOf AttachedOrder))

        w.AddButtonHandler("Keisti", "Keisti avanso apyskaitos duomenis.", _
            New DelegateContainer(Of T)(AddressOf ChangeItem))
        w.AddButtonHandler("Orderis", "Atidaryti susietą/naują kasos orderį.", _
            New DelegateContainer(Of T)(AddressOf AttachedOrder))
        w.AddButtonHandler("Ištrinti", "Pašalinti avanso apyskaitos duomenis iš duomenų bazės.", _
            New DelegateContainer(Of T)(AddressOf DeleteItem))

    End Sub

    Private Sub ChangeItem(ByVal item As AdvanceReportInfo)
        If item Is Nothing Then Exit Sub
        MDIParent1.LaunchForm(GetType(F_AdvanceReport), False, False, item.ID, item.ID)
    End Sub

    Private Sub DeleteItem(ByVal item As AdvanceReportInfo)

        If item Is Nothing Then Exit Sub

        For Each frm As Form In MDIParent1.MdiChildren
            If TypeOf frm Is F_AdvanceReport AndAlso DirectCast(frm, F_AdvanceReport).ObjectID = item.ID Then
                MsgBox("Negalima pašalinti duomenų, kol jie yra redaguojami. Uždarykite redagavimo formą.", _
                    MsgBoxStyle.Exclamation, "Klaida")
                frm.Activate()
                Exit Sub
            End If
        Next

        If Not YesOrNo("Ar tikrai norite pašalinti dokumento duomenis iš duomenų bazės?") Then Exit Sub

        Using busy As New StatusBusy
            Try
                Documents.AdvanceReport.DeleteAdvanceReport(item.ID)
            Catch ex As Exception
                ShowError(ex)
                Exit Sub
            End Try
        End Using

        If Not YesOrNo("Dokumento duomenys sėkmingai pašalinti iš įmonės duomenų bazės. " _
            & "Atnaujinti sąrašą?") Then Exit Sub

        Using bm As New BindingsManager(AdvanceReportInfoListBindingSource, _
            Nothing, Nothing, False, True)

            Try
                Obj = LoadObject(Of AdvanceReportInfoList)(Nothing, _
                    "GetAdvanceReportInfoList", True, Obj.DateFrom, Obj.DateTo, Obj.Person)
            Catch ex As Exception
                ShowError(ex)
                Exit Sub
            End Try

            bm.SetNewDataSource(Obj)

        End Using

    End Sub

    Private Sub NewItem(ByVal item As AdvanceReportInfo)
        MDIParent1.LaunchForm(GetType(F_AdvanceReport), False, False, 0)
    End Sub

    Private Sub AttachedOrder(ByVal item As AdvanceReportInfo)

        If item.TillOrderID > 0 Then
            If item.IsIncomeTillOrder Then
                MDIParent1.LaunchForm(GetType(F_TillIncomeOrder), False, False, _
                    item.TillOrderID, item.TillOrderID)
            Else
                MDIParent1.LaunchForm(GetType(F_TillSpendingsOrder), False, False, _
                    item.TillOrderID, item.TillOrderID)
            End If
        Else
            If item.IsIncomeTillOrder Then
                MDIParent1.LaunchForm(GetType(F_TillIncomeOrder), False, False, 0, item)
            Else
                MDIParent1.LaunchForm(GetType(F_TillSpendingsOrder), False, False, 0, item)
            End If
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


    Private Function SetDataSources() As Boolean

        If Not PrepareCache(Me, GetType(HelperLists.PersonInfoList)) Then Return False

        Try
            LoadPersonInfoListToGridCombo(PersonInfoAccGridComboBox, True, False, False, True)
        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
            Return False
        End Try

        Return True

    End Function

End Class