Imports ApskaitaObjects.General
Public Class F_CompanyRegionalData
    Implements IObjectEditForm

    Private Obj As CompanyRegionalData
    Private Loading As Boolean = True
    Private _RegionalDataID As Integer = -1
    Private InvoiceFormDataSet As AccControls.ReportData

    Public ReadOnly Property ObjectID() As Integer Implements IObjectEditForm.ObjectID
        Get
            If Obj Is Nothing Then Return 0
            Return Obj.ID
        End Get
    End Property

    Public ReadOnly Property ObjectType() As System.Type Implements IObjectEditForm.ObjectType
        Get
            Return GetType(CompanyRegionalData)
        End Get
    End Property


    Public Sub New(ByVal RegionalDataID As Integer)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        _RegionalDataID = RegionalDataID

    End Sub

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

    End Sub


    Private Sub F_CompanyRegionalData_Activated(ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles Me.Activated

        If Me.WindowState = FormWindowState.Maximized AndAlso MyCustomSettings.AutoSizeForm Then _
            Me.WindowState = FormWindowState.Normal

        If Loading Then
            Loading = False
            Exit Sub
        End If

    End Sub

    Private Sub F_CompanyRegionalData_FormClosing(ByVal sender As Object, _
        ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

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

        DisposeBindingSources(InvoiceFormReportViewer)
        InvoiceFormReportViewer.Dispose()
        If Not InvoiceFormDataSet Is Nothing Then InvoiceFormDataSet.Dispose()

    End Sub

    Private Sub F_CompanyRegionalData_Load(ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles Me.Load

        LanguageNameComboBox.DataSource = GetLanguageList()

        If Not _RegionalDataID < 0 Then
            Try
                Obj = LoadObject(Of CompanyRegionalData)(Nothing, "GetCompanyRegionalData", True, _RegionalDataID)
            Catch ex As Exception
                ShowError(ex)
                DisableAllControls(Me)
                Exit Sub
            End Try
        Else
            Obj = CompanyRegionalData.NewCompanyRegionalData
        End If

        CompanyRegionalDataBindingSource.DataSource = Obj

        Dim DoomyInvoice As Documents.InvoiceMade = Documents.InvoiceMade.DoomyInvoiceMade
        InvoiceFormDataSet = MapObjToReport(DoomyInvoice, "", 1, 0)
        UpdateReportDataSetWithRegionalData(InvoiceFormDataSet, Obj)

        InitializeReportViewer(InvoiceFormReportViewer, InvoiceFormDataSet, Obj.InvoiceForm)

        IOkButton.Enabled = CompanyRegionalData.CanEditObject
        OpenImageButton.Enabled = CompanyRegionalData.CanEditObject
        ClearLogoButton.Enabled = CompanyRegionalData.CanEditObject
        IApplyButton.Enabled = CompanyRegionalData.CanEditObject
        LanguageNameComboBox.Enabled = Obj.IsNew

        SetFormLayout(Me)

        If Not Obj.InvoiceForm Is Nothing AndAlso Obj.InvoiceForm.Length > 50 Then _
            Me.InvoiceFormReportViewer.RefreshReport()

    End Sub

    Private Sub OnReportRenderingComplete(ByVal sender As Object, _
        ByVal e As Microsoft.Reporting.WinForms.RenderingCompleteEventArgs) _
        Handles InvoiceFormReportViewer.RenderingComplete

        Dim Viewer As Microsoft.Reporting.WinForms.ReportViewer = _
            CType(sender, Microsoft.Reporting.WinForms.ReportViewer)
        Try
            Viewer.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout)
            Viewer.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent
            Viewer.ZoomPercent = 75
        Catch ex As Exception
        End Try

    End Sub

    Private Sub InvoiceFormReportViewer_ReportError(ByVal sender As Object, _
        ByVal e As Microsoft.Reporting.WinForms.ReportErrorEventArgs) _
        Handles InvoiceFormReportViewer.ReportError
        e.Handled = True
    End Sub

    Private Sub IOkButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles IOkButton.Click
        If SaveObj() Then
            Me.Hide()
            Me.Close()
        End If
    End Sub

    Private Sub IApplyButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles IApplyButton.Click
        SaveObj()
    End Sub

    Private Sub ICancelButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ICancelButton.Click
        CancelObj()
    End Sub

    Private Sub OpenImageButton_Click(ByVal sender As System.Object, _
                ByVal e As System.EventArgs) Handles OpenImageButton.Click
        Using opf As New OpenFileDialog
            opf.Multiselect = False
            If opf.ShowDialog() <> Windows.Forms.DialogResult.OK Then Exit Sub
            If IO.File.Exists(opf.FileName) Then
                Try
                    Obj.LogoImage = DirectCast(Bitmap.FromFile(opf.FileName).Clone, Image)
                Catch ex As Exception
                    ShowError(New Exception("Klaida. Neatpažintas paveikslėlio formatas: " & ex.Message, ex))
                End Try
            End If
        End Using
    End Sub

    Private Sub ClearLogoButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ClearLogoButton.Click
        Obj.LogoImage = Nothing
    End Sub

    Private Sub OpenInvoiceFormButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles OpenInvoiceFormButton.Click
        If Not OpenForm(Obj.InvoiceForm) Then Exit Sub
        InitializeReportViewer(InvoiceFormReportViewer, InvoiceFormDataSet, Obj.InvoiceForm)
    End Sub

    Private Sub ClearInvoiceFormButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ClearInvoiceFormButton.Click
        Obj.InvoiceForm = Nothing
        InitializeReportViewer(InvoiceFormReportViewer, InvoiceFormDataSet, Obj.InvoiceForm)
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

        Using bm As New BindingsManager(CompanyRegionalDataBindingSource, Nothing, Nothing, True, False)

            Try
                Using busy As New StatusBusy
                    Obj = LoadObject(Of CompanyRegionalData)(Obj, "Save", False)
                End Using
            Catch ex As Exception
                ShowError(ex)
                Return False
            End Try

            bm.SetNewDataSource(Obj)

        End Using

        MsgBox("Duomenys sėkmingai pakeisti.", MsgBoxStyle.Information, "Info")

        Return True

    End Function

    Private Sub CancelObj()
        Using bm As New BindingsManager(CompanyRegionalDataBindingSource, Nothing, Nothing, True, True)
        End Using
    End Sub

    Private Sub DisposeBindingSources(ByRef RP As Microsoft.Reporting.WinForms.ReportViewer)
        For Each ds As Microsoft.Reporting.WinForms.ReportDataSource _
            In RP.LocalReport.DataSources
            CType(ds.Value, BindingSource).Dispose()
        Next
        If RP.LocalReport.DataSources.Count > 0 Then _
            InvoiceFormReportViewer.LocalReport.DataSources.Clear()
    End Sub

    Private Sub InitializeReportViewer(ByRef RP As Microsoft.Reporting.WinForms.ReportViewer, _
        ByVal FDS As AccControls.ReportData, ByVal ReportByteArray As Byte())

        DisposeBindingSources(RP)

        RP.Reset()

        If Not ReportByteArray Is Nothing AndAlso ReportByteArray.Length > 50 Then

            Dim TblGeneral As BindingSource = New BindingSource(FDS, "TableGeneral")
            Dim Tbl1 As BindingSource = New BindingSource(FDS, "Table1")
            Dim Tbl2 As BindingSource = New BindingSource(FDS, "Table2")
            Dim Tbl3 As BindingSource = New BindingSource(FDS, "Table3")
            Dim Tbl4 As BindingSource = New BindingSource(FDS, "Table4")
            Dim Tbl5 As BindingSource = New BindingSource(FDS, "Table5")
            Dim Tbl6 As BindingSource = New BindingSource(FDS, "Table6")
            Dim TblCompany As BindingSource = New BindingSource(FDS, "TableCompany")

            Dim NewSourceGeneral As New Microsoft.Reporting.WinForms.ReportDataSource
            NewSourceGeneral.Value = TblGeneral
            NewSourceGeneral.Name = "ReportData_TableGeneral"

            Dim NewSourceCompany As New Microsoft.Reporting.WinForms.ReportDataSource
            NewSourceCompany.Value = TblCompany
            NewSourceCompany.Name = "ReportData_TableCompany"

            RP.LocalReport.DataSources.Add(NewSourceGeneral)
            RP.LocalReport.DataSources.Add(NewSourceCompany)

            For i As Integer = 1 To 6

                Dim NewSource As New Microsoft.Reporting.WinForms.ReportDataSource

                If i = 1 Then
                    NewSource.Value = Tbl1
                    NewSource.Name = "ReportData_Table1"
                ElseIf i = 2 Then
                    NewSource.Value = Tbl2
                    NewSource.Name = "ReportData_Table2"
                ElseIf i = 3 Then
                    NewSource.Value = Tbl3
                    NewSource.Name = "ReportData_Table3"
                ElseIf i = 4 Then
                    NewSource.Value = Tbl4
                    NewSource.Name = "ReportData_Table4"
                ElseIf i = 5 Then
                    NewSource.Value = Tbl5
                    NewSource.Name = "ReportData_Table5"
                Else
                    NewSource.Value = Tbl6
                    NewSource.Name = "ReportData_Table6"
                End If

                RP.LocalReport.DataSources.Add(NewSource)

            Next

            Dim FileStream As New IO.MemoryStream(ReportByteArray)
            RP.LocalReport.LoadReportDefinition(FileStream)

            RP.RefreshReport()

        End If

    End Sub

    Private Function OpenForm(ByRef FormObj As Byte()) As Boolean

        Using opf As New OpenFileDialog
            opf.Multiselect = False
            opf.Filter = "Report Files (*.rdlc)|*.rdlc|All Files (*.*)|*.*"

            If opf.ShowDialog() <> System.Windows.Forms.DialogResult.OK Then Return False

            If Not String.IsNullOrEmpty(opf.FileName) AndAlso IO.File.Exists(opf.FileName) Then
                Try
                    Using fs As New IO.FileStream(opf.FileName, IO.FileMode.Open)
                        Dim length As Integer = Convert.ToInt32(fs.Length - 1)
                        Dim bytes(length) As Byte
                        fs.Read(bytes, 0, length + 1)
                        FormObj = bytes
                        fs.Close()
                    End Using
                Catch ex As Exception
                    ShowError(ex)
                    Return False
                End Try
            Else
                Return False
            End If
        End Using

        Return True
    End Function

End Class