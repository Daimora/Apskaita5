Imports ApskaitaObjects.HelperLists
Imports ApskaitaObjects.Workers
Public Class F_PayOutNaturalPerson
    Implements IObjectEditForm

    Private Obj As PayOutNaturalPerson
    Private _PayOutNaturalPersonID As Integer = 0
    Private Loading As Boolean = True


    Public ReadOnly Property ObjectID() As Integer _
        Implements IObjectEditForm.ObjectID
        Get
            If Not Obj Is Nothing Then Return Obj.ID
            Return 0
        End Get
    End Property

    Public ReadOnly Property ObjectType() As System.Type _
        Implements IObjectEditForm.ObjectType
        Get
            Return GetType(PayOutNaturalPerson)
        End Get
    End Property


    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Sub New(ByVal PayOutNaturalPersonID As Integer)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        _PayOutNaturalPersonID = PayOutNaturalPersonID

    End Sub


    Private Sub F_PayOutNaturalPerson_Activated(ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles Me.Activated

        If Me.WindowState = FormWindowState.Maximized AndAlso MyCustomSettings.AutoSizeForm Then _
            Me.WindowState = FormWindowState.Normal

        If Loading Then
            Loading = False
            Exit Sub
        End If

        If Not PrepareCache(Me, GetType(TaxRateInfoList), _
            GetType(CodeInfoList)) Then Exit Sub

    End Sub

    Private Sub F_PayOutNaturalPerson_FormClosing(ByVal sender As Object, _
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

    End Sub

    Private Sub F_PayOutNaturalPerson_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        If Not SetDataSources Then Exit Sub

        If _PayOutNaturalPersonID > 0 Then
            Try
                Obj = LoadObject(Of PayOutNaturalPerson)(Nothing, "GetPayOutNaturalPerson", _
                    True, _PayOutNaturalPersonID)
            Catch ex As Exception
                ShowError(ex)
                DisableAllControls(Me)
                Exit Sub
            End Try
        Else
            Obj = PayOutNaturalPerson.NewPayOutNaturalPerson
        End If

        PayOutNaturalPersonBindingSource.DataSource = Obj

        IOkButton.Enabled = ((Obj.IsNew AndAlso PayOutNaturalPerson.CanAddObject) _
            OrElse (Not Obj.IsNew AndAlso PayOutNaturalPerson.CanEditObject))
        IApplyButton.Enabled = ((Obj.IsNew AndAlso PayOutNaturalPerson.CanAddObject) _
            OrElse (Not Obj.IsNew AndAlso PayOutNaturalPerson.CanEditObject))
        RefreshJournalEntryListButton.Enabled = ((Obj.IsNew AndAlso PayOutNaturalPerson.CanAddObject) _
            OrElse (Not Obj.IsNew AndAlso PayOutNaturalPerson.CanEditObject))
        LoadJournalEntryButton.Enabled = ((Obj.IsNew AndAlso PayOutNaturalPerson.CanAddObject) _
            OrElse (Not Obj.IsNew AndAlso PayOutNaturalPerson.CanEditObject))

        SetFormLayout(Me)

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


    Private Sub RefreshJournalEntryListButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles RefreshJournalEntryListButton.Click

        Dim JEList As ActiveReports.JournalEntryInfoList

        Try
            JEList = LoadObject(Of ActiveReports.JournalEntryInfoList)(Nothing, "GetList", _
                True, JournalEntryListDateDateTimePicker.Value, JournalEntryListDateDateTimePicker.Value, _
                "", -1, -1, DocumentType.None, False, "", "")
        Catch ex As Exception
            ShowError(ex)
            Exit Sub
        End Try

        Dim DS As New Csla.FilteredBindingList(Of ActiveReports.JournalEntryInfo)(JEList, _
            AddressOf DocumentTypeFilter)
        Dim AllowedDocTypes(2) As DocumentType
        AllowedDocTypes(0) = DocumentType.BankOperation
        AllowedDocTypes(1) = DocumentType.None
        AllowedDocTypes(2) = DocumentType.TillSpendingOrder
        DS.ApplyFilter("DocType", AllowedDocTypes)

        JournalEntryListComboBox.DataSource = Nothing
        JournalEntryListComboBox.DataSource = DS
        JournalEntryListComboBox.SelectedIndex = -1

    End Sub

    Private Sub LoadJournalEntryButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles LoadJournalEntryButton.Click

        If JournalEntryListComboBox.SelectedItem Is Nothing Then
            MsgBox("Klaida. Nepasirinkta BŽ operacija.", MsgBoxStyle.Exclamation, "Klaida.")
            Exit Sub
        End If

        Using busy As New StatusBusy
            Obj.LoadAssociatedJournalEntry(CType(JournalEntryListComboBox.SelectedItem, _
                ActiveReports.JournalEntryInfo))
        End Using

    End Sub

    Private Sub ViewJournalEntryButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ViewJournalEntryButton.Click

        If Obj Is Nothing Then Exit Sub

        If Obj.JournalEntryID > 0 Then

            MDIParent1.LaunchForm(GetType(F_GeneralLedgerEntry), False, False, _
                Obj.JournalEntryID, Obj.JournalEntryID)

        Else

            MDIParent1.LaunchForm(GetType(F_GeneralLedgerEntry), False, False, _
                0, Obj.GetNewJournalEntry)

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

            Using bm As New BindingsManager(PayOutNaturalPersonBindingSource, Nothing, Nothing, True, False)

                Try
                    Obj = LoadObject(Of PayOutNaturalPerson)(Obj, "Save", False)
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
        Using bm As New BindingsManager(PayOutNaturalPersonBindingSource, Nothing, Nothing, True, True)
        End Using
    End Sub

    Private Function SetDataSources() As Boolean

        If Not PrepareCache(Me, GetType(TaxRateInfoList), _
            GetType(CodeInfoList)) Then Return False

        Try

            LoadTaxRateListToCombo(RateGPMComboBox, TaxRateType.GPM)
            LoadTaxRateListToCombo(RatePSDForPersonComboBox, TaxRateType.PSDForPerson)
            LoadTaxRateListToCombo(RatePSDForCompanyComboBox, TaxRateType.PSDForCompany)
            LoadTaxRateListToCombo(RateSODRAForPersonComboBox, TaxRateType.SodraForPerson)
            LoadTaxRateListToCombo(RateSODRAForCompanyComboBox, TaxRateType.SodraForCompany)
            LoadCodeInfoListToGridCombo(CodeVMIAccGridComboBox, _
                ApskaitaObjects.Settings.CodeType.GpmDeclaration, True, True)
            LoadCodeInfoListToGridCombo(CodeSODRAAccGridComboBox, _
                ApskaitaObjects.Settings.CodeType.GpmDeclaration, True, True)

        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
            Return False
        End Try

        Return True

    End Function

    Public Function DocumentTypeFilter(ByVal item As Object, ByVal filterValue As Object) As Boolean

        If filterValue Is Nothing OrElse Not TypeOf filterValue Is DocumentType() _
            OrElse Not CType(filterValue, DocumentType()).Length > 0 Then Return True

        Dim flt() As DocumentType = CType(filterValue, DocumentType())

        If Not Array.IndexOf(flt, CType(item, DocumentType)) < 0 Then Return True

        Return False

    End Function

End Class