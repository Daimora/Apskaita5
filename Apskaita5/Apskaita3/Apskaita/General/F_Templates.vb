Public Class F_Templates
    Public Obj As HelperLists.TemplateJournalEntryInfoList
    Public loading As Boolean = True

    Private Sub F_Templates_Activated(ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles Me.Activated

        If Me.WindowState = FormWindowState.Maximized AndAlso MyCustomSettings.AutoSizeForm Then _
            Me.WindowState = FormWindowState.Normal

        If loading Then
            loading = False
            Exit Sub
        End If

        If Not PrepareCache(Me, GetType(HelperLists.TemplateJournalEntryInfoList)) Then Exit Sub

        ReloadObjList()

    End Sub

    Private Sub F_Templates_FormClosing(ByVal sender As Object, _
        ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        GetFormLayout(Me)
    End Sub

    Private Sub F_Templates_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        If Not PrepareCache(Me, GetType(HelperLists.TemplateJournalEntryInfoList)) Then Exit Sub

        Using busy As New StatusBusy
            Try
                Obj = HelperLists.TemplateJournalEntryInfoList.GetList
                TemplateJournalEntryListBindingSource.DataSource = Obj.GetFilteredList
            Catch ex As Exception
                ShowError(ex)
                DisableAllControls(Me)
            End Try
        End Using

        TemplateJournalEntryListDataGridView.Select()

        SetFormLayout(Me)

        InitializeMenu(Of HelperLists.TemplateJournalEntryInfo)()

    End Sub


    Private Sub Nauja_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles NewTemplateButton.Click
        NewItem(Nothing)
    End Sub


    Private Sub InitializeMenu(Of T As HelperLists.TemplateJournalEntryInfo)()

        Dim w As New ToolStripHelper(Of T)(TemplateJournalEntryListDataGridView, _
            ContextMenuStrip1, "", True)

        w.AddMenuItemHandler(ChangeItem_MenuItem, New DelegateContainer(Of T)(AddressOf ChangeItem))
        w.AddMenuItemHandler(DeleteItem_MenuItem, New DelegateContainer(Of T)(AddressOf DeleteItem))
        w.AddMenuItemHandler(NewItem_MenuItem, New DelegateContainer(Of T)(AddressOf NewItem))

        w.AddButtonHandler("Keisti", "Keisti bendrojo žurnalo šabloną.", _
            New DelegateContainer(Of T)(AddressOf ChangeItem))
        w.AddButtonHandler("Ištrinti", "Pašalinti bendrojo žurnalo šabloną iš duomenų bazės.", _
            New DelegateContainer(Of T)(AddressOf DeleteItem))

    End Sub

    Private Sub ChangeItem(ByVal item As HelperLists.TemplateJournalEntryInfo)
        If item Is Nothing Then Exit Sub
        MDIParent1.LaunchForm(GetType(F_Template), False, False, item.ID, item.ID)
    End Sub

    Private Sub DeleteItem(ByVal item As HelperLists.TemplateJournalEntryInfo)

        If item Is Nothing Then Exit Sub

        For Each frm As Form In MDIParent1.MdiChildren
            If TypeOf frm Is F_Template AndAlso DirectCast(frm, F_Template).ObjectID = item.ID Then
                MsgBox("Negalima pašalinti duomenų, kol jie yra redaguojami. Uždarykite redagavimo formą.", _
                    MsgBoxStyle.Exclamation, "Klaida")
                frm.Activate()
                Exit Sub
            End If
        Next

        If Not YesOrNo("Ar tikrai norite pašalinti bendrojo žurnalo šabloną iš duomenų bazės?") Then Exit Sub

        Using busy As New StatusBusy

            Using bm As New BindingsManager(TemplateJournalEntryListBindingSource, _
                Nothing, Nothing, False, True)
                Try
                    General.TemplateJournalEntry.DeleteTemplateJournalEntry(item.ID)
                    Obj.Remove(item)
                Catch ex As Exception
                    ShowError(ex)
                    Exit Sub
                End Try
            End Using

        End Using

        MsgBox("Bendojo žurnalo šablonas sėkmingai pašalintas iš įmonės duomenų bazės.", _
            MsgBoxStyle.Information, "Info")

    End Sub

    Private Sub NewItem(ByVal item As HelperLists.TemplateJournalEntryInfo)
        MDIParent1.LaunchForm(GetType(F_Template), False, False, 0)
    End Sub


    Private Sub ReloadObjList()

        Using busy As New StatusBusy
            Using bm As New BindingsManager(TemplateJournalEntryListBindingSource, Nothing, Nothing, False, True)

                Try
                    Obj = HelperLists.TemplateJournalEntryInfoList.GetList
                Catch ex As Exception
                    ShowError(ex)
                    DisableAllControls(Me)
                End Try

                bm.SetNewDataSource(Obj.GetFilteredList)

            End Using
        End Using

    End Sub

End Class