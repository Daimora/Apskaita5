Imports ApskaitaObjects.Assets
Public Class F_LongTermAssetComplexDocumentInfoList
    Implements ISupportsPrinting

    Private Obj As LongTermAssetComplexDocumentInfoList

    Private Sub F_LongTermAssetComplexDocumentInfoList_Activated(ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles Me.Activated

        If Me.WindowState = FormWindowState.Maximized AndAlso MyCustomSettings.AutoSizeForm Then _
            Me.WindowState = FormWindowState.Normal

    End Sub

    Private Sub F_LongTermAssetComplexDocumentInfoList_FormClosing(ByVal sender As Object, _
        ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        GetDataGridViewLayOut(LongTermAssetComplexDocumentInfoListDataGridView)
        GetFormLayout(Me)
    End Sub

    Private Sub F_LongTermAssetComplexDocumentInfoList_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        AddDGVColumnSelector(LongTermAssetComplexDocumentInfoListDataGridView)

        SetDataGridViewLayOut(LongTermAssetComplexDocumentInfoListDataGridView)
        SetFormLayout(Me)

        InitializeMenu(Of LongTermAssetComplexDocumentInfo)()

    End Sub


    Private Sub RefreshButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles RefreshButton.Click

        Using bm As New BindingsManager(LongTermAssetComplexDocumentInfoListBindingSource, _
            Nothing, Nothing, False, True)

            Try
                Obj = LoadObject(Of LongTermAssetComplexDocumentInfoList)(Nothing, _
                    "GetLongTermAssetComplexDocumentInfoList", True, DateFromDateTimePicker.Value, _
                    DateToDateTimePicker.Value)
            Catch ex As Exception
                ShowError(ex)
                Exit Sub
            End Try

            bm.SetNewDataSource(Obj.GetSortedList)

        End Using

    End Sub

    Private Sub NewOperationButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles NewOperationButton.Click
        NewItem(Nothing)
    End Sub


    Private Sub InitializeMenu(Of T As LongTermAssetComplexDocumentInfo)()

        Dim w As New ToolStripHelper(Of T)(LongTermAssetComplexDocumentInfoListDataGridView, _
            ContextMenuStrip1, "", True)

        w.AddMenuItemHandler(ChangeItem_MenuItem, New DelegateContainer(Of T)(AddressOf ChangeItem))
        w.AddMenuItemHandler(DeleteItem_MenuItem, New DelegateContainer(Of T)(AddressOf DeleteItem))
        w.AddMenuItemHandler(NewItem_MenuItem, New DelegateContainer(Of T)(AddressOf NewItem))

        w.AddButtonHandler("Keisti", "Keisti IT kompleksinį dokumentą.", _
            New DelegateContainer(Of T)(AddressOf ChangeItem))
        w.AddButtonHandler("Ištrinti", "Pašalinti IT kompleksinį dokumentą iš duomenų bazės.", _
            New DelegateContainer(Of T)(AddressOf DeleteItem))

    End Sub

    Private Sub ChangeItem(ByVal item As LongTermAssetComplexDocumentInfo)
        If item Is Nothing Then Exit Sub
        MDIParent1.LaunchForm(GetType(F_LongTermAssetComplexDocument), False, False, item.ID, item.ID)
    End Sub

    Private Sub DeleteItem(ByVal item As LongTermAssetComplexDocumentInfo)

        If item Is Nothing Then Exit Sub

        For Each frm As Form In MDIParent1.MdiChildren
            If TypeOf frm Is F_LongTermAssetComplexDocument AndAlso _
                DirectCast(frm, F_LongTermAssetComplexDocument).ObjectID = item.ID Then
                MsgBox("Negalima pašalinti duomenų, kol jie yra redaguojami. Uždarykite redagavimo formą.", _
                    MsgBoxStyle.Exclamation, "Klaida")
                frm.Activate()
                Exit Sub
            End If
        Next

        If Not YesOrNo("Ar tikrai norite pašalinti kompleksinį IT dokumentą iš duomenų bazės?") Then Exit Sub

        Using busy As New StatusBusy

            Using bm As New BindingsManager(LongTermAssetComplexDocumentInfoListBindingSource, _
                Nothing, Nothing, False, True)
                Try
                    LongTermAssetComplexDocument.DeleteLongTermAssetComplexDocument(item.ID)
                    Obj.Remove(item)
                Catch ex As Exception
                    ShowError(ex)
                    Exit Sub
                End Try
            End Using

        End Using

        MsgBox("Kompleksinis IT dokumentas sėkmingai pašalintas iš įmonės duomenų bazės.", _
            MsgBoxStyle.Information, "Info")

    End Sub

    Private Sub NewItem(ByVal item As LongTermAssetComplexDocumentInfo)
        MDIParent1.LaunchForm(GetType(F_LongTermAssetComplexDocument), False, False, 0)
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

End Class