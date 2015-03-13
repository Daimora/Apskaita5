Imports ApskaitaObjects.ActiveReports
Imports ApskaitaObjects.General
Public Class F_Persons
    Implements ISupportsPrinting

    Private Obj As PersonInfoItemList
    Private Loading As Boolean = True


    Private Sub F_Persons_Activated(ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles Me.Activated

        If Me.WindowState = FormWindowState.Maximized AndAlso MyCustomSettings.AutoSizeForm Then _
            Me.WindowState = FormWindowState.Normal

        If Loading Then
            Loading = False
            Exit Sub
        End If

        If Not PrepareCache(Me, GetType(HelperLists.PersonGroupInfoList)) Then Exit Sub

    End Sub

    Private Sub F_Persons_FormClosing(ByVal sender As Object, _
        ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        GetDataGridViewLayOut(PersonInfoItemListDataGridView)
        GetFormLayout(Me)
    End Sub

    Private Sub F_Persons_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        If Not PrepareCache(Me, GetType(HelperLists.PersonGroupInfoList), _
            GetType(HelperLists.PersonInfoList)) Then Exit Sub

        If Not SetDataSources() Then Exit Sub

        AddDGVColumnSelector(PersonInfoItemListDataGridView)

        SetDataGridViewLayOut(PersonInfoItemListDataGridView)
        SetFormLayout(Me)

        InitializeMenu(Of PersonInfoItem)()

    End Sub


    Private Sub RefreshButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles RefreshButton.Click

        Using bm As New BindingsManager(PersonInfoItemListBindingSource, Nothing, Nothing, False, True)

            Try
                Obj = LoadObject(Of ActiveReports.PersonInfoItemList)(Nothing, "GetList", True, _
                    ShowClientsCheckBox.Checked, ShowsuppliersCheckBox.Checked, _
                    ShowWorkersCheckBox.Checked, LikeStringTextBox.Text.Trim)
            Catch ex As Exception
                ShowError(ex)
                Exit Sub
            End Try

            bm.SetNewDataSource(Obj.GetSortedList)

        End Using

    End Sub


    Private Sub InitializeMenu(Of T As PersonInfoItem)()

        Dim w As New ToolStripHelper(Of T)(PersonInfoItemListDataGridView, _
            ContextMenuStrip1, "", True)

        w.AddMenuItemHandler(ChangeItem_MenuItem, New DelegateContainer(Of T)(AddressOf ChangeItem))
        w.AddMenuItemHandler(DeleteItem_MenuItem, New DelegateContainer(Of T)(AddressOf DeleteItem))

        w.AddButtonHandler("Keisti", "Keisti kontrahento duomenis.", _
            New DelegateContainer(Of T)(AddressOf ChangeItem))
        w.AddButtonHandler("Ištrinti", "Pašalinti kontrahento duomenis iš duomenų bazės.", _
            New DelegateContainer(Of T)(AddressOf DeleteItem))

    End Sub

    Private Sub ChangeItem(ByVal item As PersonInfoItem)
        If item Is Nothing Then Exit Sub
        MDIParent1.LaunchForm(GetType(F_Person), False, False, item.ID, item.ID)
    End Sub

    Private Sub DeleteItem(ByVal item As PersonInfoItem)

        If item Is Nothing Then Exit Sub

        For Each frm As Form In MDIParent1.MdiChildren
            If TypeOf frm Is F_Person AndAlso DirectCast(frm, F_Person).ObjectID = item.ID Then
                MsgBox("Negalima pašalinti duomenų, kol jie yra redaguojami. Uždarykite redagavimo formą.", _
                    MsgBoxStyle.Exclamation, "Klaida")
                frm.Activate()
                Exit Sub
            End If
        Next

        If Not YesOrNo("Ar tikrai norite pašalinti pasirinkto kontrahento " & _
                "duomenis iš duomenų bazės?") Then Exit Sub

        Using busy As New StatusBusy
            Try
                Person.DeletePerson(item.ID)
            Catch ex As Exception
                ShowError(ex)
                Exit Sub
            End Try
        End Using

        MsgBox("Kontrahento duomenys sėkmingai pašalinti iš duomenų bazės.", _
            MsgBoxStyle.Information, "Info")

        Using bm As New BindingsManager(PersonInfoItemListBindingSource, Nothing, Nothing, False, True)

            Try
                Obj = LoadObject(Of ActiveReports.PersonInfoItemList)(Nothing, "GetList", True, _
                    Obj.ShowClients, Obj.ShowSuppliers, Obj.ShowWorkers, Obj.LikeString)
            Catch ex As Exception
                ShowError(ex)
                Exit Sub
            End Try

            bm.SetNewDataSource(Obj.GetSortedList)

        End Using

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

        Try


        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
            Return False
        End Try

        Return True

    End Function

End Class