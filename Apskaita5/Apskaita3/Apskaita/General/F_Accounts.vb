Public Class F_Accounts
    Implements ISupportsPrinting

    Private Obj As General.AccountList
    Private IsLoading As Boolean = True

    Private ObjIsRebinding As Boolean = True

    Private Sub F_F_Accounts_Closing(ByVal sender As Object, _
        ByVal e As FormClosingEventArgs) Handles Me.FormClosing

        If Not Obj Is Nothing AndAlso Obj.IsDirtyEnough Then
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

        GetDataGridViewLayOut(AccountListDataGridView)
        GetFormLayout(Me)

    End Sub

    Private Sub F_F_Accounts_Activated(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Activated

        If Me.WindowState = FormWindowState.Maximized AndAlso MyCustomSettings.AutoSizeForm Then _
            Me.WindowState = FormWindowState.Normal

        If IsLoading Then
            IsLoading = False
            Exit Sub
        End If

        If Not PrepareCache(Me, GetType(HelperLists.AssignableCRItemList)) Then Exit Sub

    End Sub

    Private Sub F_Accounts_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        If Not SetDataSources() Then Exit Sub

        Try

            Obj = LoadObject(Of General.AccountList)(Nothing, "GetAccountList", False)

        Catch ex As Exception
            ShowError(New Exception("Klaida. Nepavyko gauti sąskaitų plano duomenų.", ex))
            DisableAllControls(Me)
            ObjIsRebinding = False
            Exit Sub
        End Try

        Obj.BeginEdit()
        AccountListBindingSource.DataSource = Obj

        Dim CM2 As New ContextMenu()
        Dim CMItem1 As New MenuItem("Overwrite", AddressOf PasteAccButton_Click)
        CM2.MenuItems.Add(CMItem1)
        Dim CMItem2 As New MenuItem("Informacija apie formatą", AddressOf PasteAccButton_Click)
        CM2.MenuItems.Add(CMItem2)
        PasteAccButton.ContextMenu = CM2

        Dim CM1 As New ContextMenu()
        Dim CMItem3 As New MenuItem("Overwrite", AddressOf OpenFileButton_Click)
        CM1.MenuItems.Add(CMItem3)
        OpenFileAccButton.ContextMenu = CM1

        nOkButton.Enabled = General.AccountList.CanEditObject
        ApplyButton.Enabled = General.AccountList.CanEditObject

        SetDataGridViewLayOut(AccountListDataGridView)
        SetFormLayout(Me)
        ObjIsRebinding = False

        Dim h As New EditableDataGridViewHelper(AccountListDataGridView)

    End Sub


    Private Sub OkButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles nOkButton.Click
        If Not SaveObj() Then Exit Sub
        Me.Hide()
        Me.Close()
    End Sub

    Private Sub ApplyButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ApplyButton.Click
        SaveObj()
    End Sub

    Private Sub CancelButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles nCancelButton.Click
        CancelObj()
    End Sub


    Private Sub CopyToClipboardButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles CopyToClipboardButton.Click
        Try
            Using busy As New StatusBusy
                Clipboard.SetText(Obj.SaveToString(), TextDataFormat.UnicodeText)
            End Using
            MsgBox("Duomenys sėkminga nukopijuotį į clipboard'ą.", MsgBoxStyle.Information, "Info")
        Catch ex As Exception
            ShowError(ex)
        End Try
    End Sub

    Private Sub PasteAccButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles PasteAccButton.Click

        If Obj Is Nothing OrElse Not General.AccountList.CanEditObject Then Exit Sub

        If GetSenderText(sender).Trim.ToLower.Contains("informacija") Then
            MsgBox(General.Account.GetPasteStringColumnsDescription, MsgBoxStyle.Information, "Info")
            Clipboard.SetText(String.Join(vbTab, General.Account.GetPasteStringColumns))
            Exit Sub
        End If

        If GetSenderText(sender).Trim.ToLower.Contains("overwrite") AndAlso Obj.Count > 0 AndAlso _
            Not YesOrNo("Ar tikrai norite perrašyti visą sąskaitų planą?") Then Exit Sub

        Try
            Using busy As New StatusBusy
                Obj.LoadAccountListFromString(Clipboard.GetText, GetSenderText(sender).Trim.ToLower.Contains("overwrite"))
            End Using
        Catch ex As Exception
            ShowError(ex)
            Exit Sub
        End Try

    End Sub

    Private Sub OpenFileButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles OpenFileAccButton.Click

        If Obj Is Nothing Then Exit Sub

        Dim filePath As String

        Using openFile As New OpenFileDialog
            openFile.InitialDirectory = AppPath()
            openFile.Filter = "Sąskaitų plano duomenys (*.txt)|*.txt|Visi failai|*.*"
            openFile.Multiselect = False
            If openFile.ShowDialog() <> Windows.Forms.DialogResult.OK Then Exit Sub
            filePath = openFile.FileName
        End Using

        If StringIsNullOrEmpty(filePath) Then Exit Sub

        If Not IO.File.Exists(filePath) Then
            MsgBox("Klaida. Failas '" & filePath & "' neegzistuoja.", MsgBoxStyle.Exclamation, "Klaida")
            Exit Sub
        End If

        Try
            Using busy As New StatusBusy
                Obj.LoadAccountListFromFile(filePath, GetSenderText(sender).Trim.ToLower.Contains("overwrite"))
            End Using
        Catch ex As Exception
            ShowError(ex)
        End Try

    End Sub

    Private Sub SaveFileButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles SaveFileButton.Click

        If Obj Is Nothing Then Exit Sub

        Dim filePath As String

        Using saveFile As New SaveFileDialog
            saveFile.Filter = "Sąskaitų plano duomenys (*.txt)|*.txt|Visi failai|*.*"
            saveFile.AddExtension = True
            saveFile.DefaultExt = ".txt"
            If saveFile.ShowDialog() <> Windows.Forms.DialogResult.OK Then Exit Sub
            filePath = saveFile.FileName
        End Using

        If StringIsNullOrEmpty(filePath) Then Exit Sub

        Try
            Using busy As New StatusBusy
                Obj.SaveToFile(filePath)
            End Using
            MsgBox("Failas sėkmingai išsaugotas.", MsgBoxStyle.Information, "Info")
        Catch ex As Exception
            ShowError(ex)
        End Try

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
            MsgBox("Formoje yra klaidų:" & vbCrLf & Obj.GetAllBrokenRules, MsgBoxStyle.Exclamation, "Klaida.")
            Return False
        End If

        Dim Question, Answer As String
        If Not String.IsNullOrEmpty(Obj.GetAllWarnings.Trim) Then
            Question = "DĖMESIO. Duomenyse gali būti klaidų: " & vbCrLf _
                & Obj.GetAllWarnings.Trim & vbCrLf
        Else
            Question = ""
        End If
        Question = Question & "Ar tikrai norite pakeisti duomenis?"
        Answer = "Duomenys sėkmingai pakeisti."

        If Not YesOrNo(Question) Then Return False

        Using bm As New BindingsManager(AccountListBindingSource, Nothing, Nothing, True, False)

            Obj.ApplyEdit()

            Try
                Obj = LoadObject(Of General.AccountList)(Obj, "Save", False)
            Catch ex As Exception
                ShowError(ex)
                Return False
            End Try

            Obj.BeginEdit()

            bm.SetNewDataSource(Obj)

        End Using

        MsgBox(Answer, MsgBoxStyle.Information, "Info")

        Return True

    End Function

    Private Sub CancelObj()
        If Obj Is Nothing Then Exit Sub
        Using bm As New BindingsManager(AccountListBindingSource, Nothing, Nothing, True, True)
            Obj.CancelEdit()
            Obj.BeginEdit()
        End Using
    End Sub


    Private Function SetDataSources() As Boolean

        If Not PrepareCache(Me, GetType(HelperLists.AssignableCRItemList)) Then Return False

        Try
            LoadAssignableCRItemListToGridCombo(DataGridViewTextBoxColumn3, True)
        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
            Return False
        End Try

        Return True

    End Function

End Class