Imports ApskaitaObjects.Settings
Public Class F_GeneralSettings

    Private Obj As CommonSettings


    Private Sub F_GeneralSettings_Closing(ByVal sender As System.Object, _
        ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing

        If Not Obj Is Nothing AndAlso TypeOf Obj Is IIsDirtyEnough AndAlso _
            DirectCast(Obj, IIsDirtyEnough).IsDirtyEnough Then
            Dim answ As String = Ask("Išsaugoti duomenis?", New ButtonStructure("Taip"), _
                New ButtonStructure("Ne"), New ButtonStructure("Atšaukti"))
            If answ <> "Taip" AndAlso answ <> "Ne" Then
                e.Cancel = True
                Exit Sub
            End If
            If answ = "Taip" Then
                If Not SaveObj(False) Then
                    e.Cancel = True
                    Exit Sub
                End If
            Else
                CancelObj(False)
            End If
        End If

        GetFormLayout(Me)

    End Sub

    Private Sub F_GeneralSettings_Activated(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Activated
        If Me.WindowState = FormWindowState.Maximized AndAlso MyCustomSettings.AutoSizeForm Then _
            Me.WindowState = FormWindowState.Normal
    End Sub

    Private Sub F_GeneralSettings_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        TaxTypeDataGridViewColumn.DataSource = _
            EnumValueAttribute.GetLocalizedNameList(GetType(TaxRateType))
        CodeTypeDataGridViewColumn.DataSource = _
            EnumValueAttribute.GetLocalizedNameList(GetType(CodeType))
        NameTypeDataGridViewColumn.DataSource = _
            EnumValueAttribute.GetLocalizedNameList(GetType(NameType))

        Try
            Using busy As New StatusBusy
                Obj = ApskaitaObjects.Settings.CommonSettings.GetCommonSettings()
            End Using
        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
            Exit Sub
        End Try

        CommonSettingsBindingSource.DataSource = Obj

        nOkButton.Enabled = ApskaitaObjects.Settings.CommonSettings.CanEditObject
        ApplyButton.Enabled = ApskaitaObjects.Settings.CommonSettings.CanEditObject

        SetFormLayout(Me)

    End Sub


    Private Sub OkButton_Click(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles nOkButton.Click
        If Not SaveObj(False) Then Exit Sub
        Me.Hide()
        Me.Close()
    End Sub

    Private Sub ApplyButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ApplyButton.Click
        SaveObj(True)
    End Sub

    Private Sub CancelButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles nCancelButton.Click
        CancelObj(True)
    End Sub


    Private Sub TaxRatesDataGridView_RowLeave(ByVal sender As Object, _
        ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) _
        Handles TaxRatesDataGridView.RowLeave, CodesDataGridView.RowLeave, _
        DefaultWorkTimesDataGridView.RowLeave, PublicHolidaysDataGridView.RowLeave, _
        NamesDataGridView.RowLeave

        Dim grid As DataGridView = Nothing
        Try
            grid = DirectCast(sender, DataGridView)
        Catch ex As Exception
        End Try
        If grid Is Nothing Then Exit Sub

        If grid.Rows(e.RowIndex).IsNewRow AndAlso e.RowIndex = grid.Rows.Count - 1 Then _
            grid.CancelEdit()

    End Sub


    Private Function SaveObj(ByVal rebindObject As Boolean) As Boolean

        If Not Obj.IsValid Then
            MsgBox("Formoje yra klaidų:" & vbCrLf & Obj.GetAllBrokenRules, _
                MsgBoxStyle.Exclamation, "Klaida.")
            Return False
        End If

        If Not Obj.IsDirty Then Return True

        If Not YesOrNo("Ar tikrai norite pakeisti bendrus aplinkos nustatymus?") Then Return False

        CommonSettingsBindingSource.RaiseListChangedEvents = False
        TaxRatesBindingSource.RaiseListChangedEvents = False
        CodesSortedBindingSource.RaiseListChangedEvents = False
        NamesSortedBindingSource.RaiseListChangedEvents = False
        PublicHolidaysSortedBindingSource.RaiseListChangedEvents = False
        DefaultWorkTimesSortedBindingSource.RaiseListChangedEvents = False

        TaxRatesBindingSource.EndEdit()
        CodesSortedBindingSource.EndEdit()
        NamesSortedBindingSource.EndEdit()
        PublicHolidaysSortedBindingSource.EndEdit()
        DefaultWorkTimesSortedBindingSource.EndEdit()
        CommonSettingsBindingSource.EndEdit()

        Try
            Using busy As New StatusBusy
                Obj = Obj.Clone.Save
            End Using
        Catch ex As Exception
            ShowError(ex)
            CommonSettingsBindingSource.RaiseListChangedEvents = True
            TaxRatesBindingSource.RaiseListChangedEvents = True
            CodesSortedBindingSource.RaiseListChangedEvents = True
            NamesSortedBindingSource.RaiseListChangedEvents = True
            PublicHolidaysSortedBindingSource.RaiseListChangedEvents = True
            DefaultWorkTimesSortedBindingSource.RaiseListChangedEvents = True
            Return False
        End Try

        If rebindObject Then
            RebindObj()
        Else
            CommonSettingsBindingSource.RaiseListChangedEvents = True
            TaxRatesBindingSource.RaiseListChangedEvents = True
            CodesSortedBindingSource.RaiseListChangedEvents = True
            NamesSortedBindingSource.RaiseListChangedEvents = True
            PublicHolidaysSortedBindingSource.RaiseListChangedEvents = True
            DefaultWorkTimesSortedBindingSource.RaiseListChangedEvents = True
        End If

        MsgBox("Bendri aplinkos nustatymai sėkmingai pakeisti.", MsgBoxStyle.Information, "Info")

        Return True

    End Function

    Private Sub CancelObj(ByVal rebindObject As Boolean)

        CommonSettingsBindingSource.RaiseListChangedEvents = False
        TaxRatesBindingSource.RaiseListChangedEvents = False
        CodesSortedBindingSource.RaiseListChangedEvents = False
        NamesSortedBindingSource.RaiseListChangedEvents = False
        PublicHolidaysSortedBindingSource.RaiseListChangedEvents = False
        DefaultWorkTimesSortedBindingSource.RaiseListChangedEvents = False

        TaxRatesBindingSource.CancelEdit()
        CodesSortedBindingSource.CancelEdit()
        NamesSortedBindingSource.CancelEdit()
        PublicHolidaysSortedBindingSource.CancelEdit()
        DefaultWorkTimesSortedBindingSource.CancelEdit()
        CommonSettingsBindingSource.CancelEdit()

        If rebindObject Then
            RebindObj()
        Else
            CommonSettingsBindingSource.RaiseListChangedEvents = True
            TaxRatesBindingSource.RaiseListChangedEvents = True
            CodesSortedBindingSource.RaiseListChangedEvents = True
            NamesSortedBindingSource.RaiseListChangedEvents = True
            PublicHolidaysSortedBindingSource.RaiseListChangedEvents = True
            DefaultWorkTimesSortedBindingSource.RaiseListChangedEvents = True
        End If

    End Sub

    Private Sub RebindObj()

        CommonSettingsBindingSource.RaiseListChangedEvents = False
        TaxRatesBindingSource.RaiseListChangedEvents = False
        CodesSortedBindingSource.RaiseListChangedEvents = False
        NamesSortedBindingSource.RaiseListChangedEvents = False
        PublicHolidaysSortedBindingSource.RaiseListChangedEvents = False
        DefaultWorkTimesSortedBindingSource.RaiseListChangedEvents = False

        CommonSettingsBindingSource.DataSource = Nothing
        TaxRatesBindingSource.DataSource = CommonSettingsBindingSource
        CodesSortedBindingSource.DataSource = CommonSettingsBindingSource
        NamesSortedBindingSource.DataSource = CommonSettingsBindingSource
        PublicHolidaysSortedBindingSource.DataSource = CommonSettingsBindingSource
        DefaultWorkTimesSortedBindingSource.DataSource = CommonSettingsBindingSource

        CommonSettingsBindingSource.DataSource = Obj

        CommonSettingsBindingSource.RaiseListChangedEvents = True
        TaxRatesBindingSource.RaiseListChangedEvents = True
        CodesSortedBindingSource.RaiseListChangedEvents = True
        NamesSortedBindingSource.RaiseListChangedEvents = True
        PublicHolidaysSortedBindingSource.RaiseListChangedEvents = True
        DefaultWorkTimesSortedBindingSource.RaiseListChangedEvents = True

        CommonSettingsBindingSource.ResetBindings(False)
        TaxRatesBindingSource.ResetBindings(False)
        CodesSortedBindingSource.ResetBindings(False)
        NamesSortedBindingSource.ResetBindings(False)
        PublicHolidaysSortedBindingSource.ResetBindings(False)
        DefaultWorkTimesSortedBindingSource.ResetBindings(False)

    End Sub

End Class