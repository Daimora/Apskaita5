Imports ApskaitaObjects.General
Imports ApskaitaObjects.HelperLists
Public Class F_Company
    Private WithEvents Obj As General.Company
    Private Loading As Boolean = True

    Private Sub F_Company_Activated(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Activated

        If Me.WindowState = FormWindowState.Maximized AndAlso MyCustomSettings.AutoSizeForm Then _
            Me.WindowState = FormWindowState.Normal

        If Loading Then
            Loading = False
            Exit Sub
        End If

        PrepareCache(Me, GetType(CompanyRegionalInfoList), GetType(AccountInfoList))

    End Sub

    Private Sub F_Company_Closing(ByVal sender As System.Object, _
        ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing

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
        GetDataGridViewLayOut(CompanyRegionalInfoListDataGridView)

    End Sub

    Private Sub F_Company_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        If Not PrepareCache(Me, GetType(CompanyRegionalInfoList), GetType(AccountInfoList)) Then Exit Sub

        Try

            LoadAccountInfoListToGridCombo(DataGridViewTextBoxColumn4, True)

            Obj = LoadObject(Of Company)(Nothing, "GetCompany", True)

        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
            Exit Sub
        End Try

        CompanyBindingSource.DataSource = Obj
        CompanyRegionalInfoListBindingSource.DataSource = GetBindingSourceForCachedList( _
            GetType(CompanyRegionalInfoList))

        SaveCompanyButton.Enabled = General.Company.CanEditObject
        OpenImageButton.Enabled = General.Company.CanEditObject
        ClearLogoButton.Enabled = General.Company.CanEditObject
        SaveCompanyButton.Enabled = General.Company.CanEditObject

        AddDGVColumnSelector(CompanyRegionalInfoListDataGridView)
        SetFormLayout(Me)
        SetDataGridViewLayOut(CompanyRegionalInfoListDataGridView)

        Dim h As New EditableDataGridViewHelper(AccountsDataGridView)
        Dim g As New EditableDataGridViewHelper(DefaultTaxRatesDataGridView)

    End Sub


    Private Sub OkButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles nOkButton.Click
        If SaveObj() Then Me.Close()
    End Sub

    Private Sub SaveCompanyButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles SaveCompanyButton.Click
        SaveObj()
    End Sub

    Private Sub CancelButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles nCancelButton.Click
        CancelObj()
    End Sub


    Private Sub OpenImageButton_Click(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles OpenImageButton.Click
        Using opf As New OpenFileDialog
            opf.Multiselect = False
            If opf.ShowDialog() <> Windows.Forms.DialogResult.OK Then Exit Sub
            If IO.File.Exists(opf.FileName) Then
                Try
                    Obj.HeadPersonSignature = DirectCast(Bitmap.FromFile(opf.FileName).Clone, Image)
                Catch ex As Exception
                    ShowError(New Exception("Klaida. Neatpažintas paveikslėlio formatas: " & ex.Message, ex))
                End Try
            End If
        End Using
    End Sub

    Private Sub ClearLogoButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ClearLogoButton.Click
        Obj.HeadPersonSignature = Nothing
    End Sub

    Private Sub NewRegionDataButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles NewRegionDataButton.Click
        MDIParent1.LaunchForm(GetType(F_CompanyRegionalData), False, False, 0)
    End Sub


    Private Sub CompanyRegionalInfoListDataGridView_CellContentClick(ByVal sender As System.Object, _
        ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) _
        Handles CompanyRegionalInfoListDataGridView.CellDoubleClick

        If e.RowIndex < 0 Then Exit Sub

        Dim SelectedObj As HelperLists.CompanyRegionalInfo = Nothing
        Try
            SelectedObj = DirectCast(CompanyRegionalInfoListDataGridView.Rows(e.RowIndex).DataBoundItem, _
                HelperLists.CompanyRegionalInfo)
        Catch ex As Exception
        End Try
        If SelectedObj Is Nothing Then Exit Sub

        For Each frm As Form In MDIParent1.MdiChildren
            If TypeOf frm Is F_CompanyRegionalData AndAlso SelectedObj.ID >= 0 AndAlso _
                DirectCast(frm, F_CompanyRegionalData).ObjectID = SelectedObj.ID Then
                frm.BringToFront()
                frm.Activate()
                Exit Sub
            End If
        Next

        Dim Answ As String
        If SelectedObj.ID > 0 Then

            Answ = Ask("", New ButtonStructure("Keisti", "Keisti regioninius duomenis."), _
                New ButtonStructure("Ištrinti", "Pašalinti regioninius duomenis iš duomenų bazės."), _
                New ButtonStructure("Atšaukti", "Nieko nedaryti."))

        Else
            Answ = Ask("", New ButtonStructure("Keisti", "Keisti regioninius duomenis."), _
                New ButtonStructure("Atšaukti", "Nieko nedaryti."))
        End If

        If Answ <> "Keisti" AndAlso Answ <> "Ištrinti" Then Exit Sub

        If Answ = "Keisti" Then

            MDIParent1.LaunchForm(GetType(F_CompanyRegionalData), False, False, 0, SelectedObj.ID)

        Else

            If Not YesOrNo("Ar tikrai norite pašalinti pasirinktus įmonės regioninius " & _
                "duomenis iš duomenų bazės?") Then Exit Sub

            Using busy As New StatusBusy
                Try
                    General.CompanyRegionalData.DeleteCompanyRegionalData(SelectedObj.ID)
                    CompanyRegionalInfoList.GetList()
                Catch ex As Exception
                    ShowError(ex)
                    Exit Sub
                End Try
            End Using

            MsgBox("Pasirinkti įmonės regioniniai duomenys sėkmingai pašalinti iš duomenų bazės.", _
                MsgBoxStyle.Information, "Info")

        End If

    End Sub



    Private Function SaveObj() As Boolean

        If Not Obj.IsDirty Then Return True

        If Not Obj.IsValid Then
            MsgBox("Formoje yra klaidų:" & vbCrLf & Obj.BrokenRulesCollection.ToString( _
                Csla.Validation.RuleSeverity.Error), MsgBoxStyle.Exclamation, "Klaida.")
            Return False
        End If

        Dim Question, Answer As String
        If Obj.BrokenRulesCollection.WarningCount > 0 Then
            Question = "DĖMESIO. Duomenyse gali būti klaidų: " & vbCrLf _
                & Obj.BrokenRulesCollection.ToString(Csla.Validation.RuleSeverity.Warning) & vbCrLf
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

        Using busy As New StatusBusy

            Using bm As New BindingsManager(CompanyBindingSource, AccountsBindingSource, _
                DefaultTaxRatesBindingSource, True, False)

                Try
                    Obj = LoadObject(Of Company)(Obj, "Save", False)
                Catch ex As Exception
                    ShowError(ex)
                    Return False
                End Try

                bm.SetNewDataSource(Obj)

            End Using

        End Using

        MsgBox("Duomenys sėkmingai pakeisti.", MsgBoxStyle.Information, "Info")

        Return True

    End Function

    Private Sub CancelObj()
        Using bm As New BindingsManager(CompanyBindingSource, AccountsBindingSource, _
            DefaultTaxRatesBindingSource, True, True)
        End Using
    End Sub

End Class