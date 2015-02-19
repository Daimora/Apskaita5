Imports ApskaitaObjects.General
Imports ApskaitaObjects.HelperLists
Public Class F_Person
    Implements IObjectEditForm

    Private Obj As Person
    Private Loading As Boolean = True
    Private _PersonID As Integer = 0
    Private nClientCode As String = ""
    Private nClientName As String = ""
    Private nClientBankAccount As String = ""
    Private nClientBankName As String = ""
    Private nClientAddress As String = ""
    Private nIsClientAndSupplier As Boolean = False
    Private nClientInfo As InvoiceInfo.ClientInfo = Nothing


    Public ReadOnly Property ObjectID() As Integer Implements IObjectEditForm.ObjectID
        Get
            If Obj Is Nothing Then Return 0
            Return Obj.ID
        End Get
    End Property

    Public ReadOnly Property ObjectType() As System.Type Implements IObjectEditForm.ObjectType
        Get
            Return GetType(Person)
        End Get
    End Property


    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

    End Sub

    Public Sub New(ByVal PersonID As Integer)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        _PersonID = PersonID

    End Sub

    Public Sub New(ByVal cClientCode As String, ByVal cClientName As String, _
        ByVal cClientBankAccount As String, ByVal cClientBankName As String)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        nClientCode = cClientCode
        nClientName = cClientName
        nClientBankAccount = cClientBankAccount
        nClientBankName = cClientBankName
        nClientAddress = "Nenurodytas"
        nIsClientAndSupplier = True

    End Sub

    Public Sub New(ByVal cClientInfo As InvoiceInfo.ClientInfo)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        nClientInfo = cClientInfo

    End Sub


    Private Sub F_Person_Activated(ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles Me.Activated

        If Me.WindowState = FormWindowState.Maximized AndAlso MyCustomSettings.AutoSizeForm Then _
            Me.WindowState = FormWindowState.Normal

        If Loading Then
            Loading = False
            Exit Sub
        End If

        If Not PrepareCache(Me, GetType(HelperLists.CompanyRegionalInfoList), _
            GetType(HelperLists.AccountInfoList)) Then Exit Sub

    End Sub

    Private Sub F_Person_FormClosing(ByVal sender As Object, _
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

    Private Sub F_Person_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        If Not SetDataSources() Then Exit Sub

        Try
            If _PersonID > 0 Then
                Obj = LoadObject(Of Person)(Nothing, "GetPerson", True, _PersonID)
                Me.Text = "Kontrahento duomenys " & Obj.Name
            Else
                Obj = LoadObject(Of Person)(Nothing, "NewPerson", True)
                If Not nClientInfo Is Nothing Then
                    Obj.AddWithPersonInfoData(nClientInfo)
                Else
                    Obj.Name = Me.nClientName
                    Obj.Code = Me.nClientCode
                    Obj.BankAccount = Me.nClientBankAccount
                    Obj.Bank = Me.nClientBankName
                    Obj.Address = Me.nClientAddress
                    Obj.IsClient = Me.nIsClientAndSupplier
                    Obj.IsSupplier = Me.nIsClientAndSupplier
                    Obj.AccountAgainstBankBuyer = GetCurrentCompany.GetDefaultAccount(DefaultAccountType.Buyers)
                    Obj.AccountAgainstBankSupplyer = GetCurrentCompany.GetDefaultAccount(DefaultAccountType.Suppliers)
                End If
                Me.Text = "Naujo kontrahento duomenys"
            End If
        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
            Exit Sub
        End Try

        PersonBindingSource.DataSource = Obj

        IOkButton.Enabled = Person.CanEditObject
        IApplyButton.Enabled = Person.CanEditObject

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


    Private Sub FetchFromWebButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles FetchFromWebButton.Click

        If Not My.Computer.Network.IsAvailable Then
            MsgBox("Klaida. Nėra ryšio su tinklu.", MsgBoxStyle.Exclamation, "Klaida.")
            Exit Sub
        End If

        If String.IsNullOrEmpty(Obj.Code.Trim) Then
            MsgBox("Klaida. Neįvestas asmens/įmonės kodas.", MsgBoxStyle.Exclamation, "Klaida.")
            Exit Sub
        End If

        If Not Obj.IsNew Then
            If Not YesOrNo("DĖMESIO!!!. Šio asmens duomenys jau yra įtraukti į įmonės duomenų " & _
                "bazę. Ar tikrai norite įkrauti naujus duomenis iš JAR ir VMI?") Then Exit Sub
        End If

        Using frm As New AccWebCrawler.F_LaunchWebCrawler(Obj.Code.Trim)
            If frm.ShowDialog <> Windows.Forms.DialogResult.OK _
                OrElse frm.result Is Nothing Then Exit Sub
            Obj.Name = DirectCast(frm.result, AccWebCrawler.PersonInfo).Name
            Obj.Address = DirectCast(frm.result, AccWebCrawler.PersonInfo).Address
            Obj.CodeVAT = DirectCast(frm.result, AccWebCrawler.PersonInfo).VatCode
        End Using

    End Sub

    Private Sub CopyPersonButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles CopyPersonButton.Click

        If Obj Is Nothing Then Exit Sub

        Dim info As InvoiceInfo.ClientInfo = Nothing
        Try
            Using busy As New StatusBusy
                info = Obj.GetPersonInfo()
            End Using
        Catch ex As Exception
            ShowError(New Exception("Klaida. Nepavyko generuoti ClientInfo objekto: " _
                & ex.Message, ex))
            Exit Sub
        End Try

        If info Is Nothing Then
            MsgBox("Klaida. Dėl nežinomų priežasčių nepavyko generuoti ClientInfo objekto.", _
                MsgBoxStyle.Exclamation, "Klaida")
            Exit Sub
        End If

        Try
            Using busy As New StatusBusy
                System.Windows.Forms.Clipboard.SetText(InvoiceInfo.Factory. _
                    ToXmlString(Of InvoiceInfo.ClientInfo)(info), TextDataFormat.UnicodeText)
            End Using
        Catch ex As Exception
            ShowError(New Exception("Klaida. Nepavyko serializuoti ClientInfo objekto: " _
                & ex.Message, ex))
            Exit Sub
        End Try

        MsgBox("Asmens duomenys sėkmingai nukopijuoti į ClipBoard'ą.", MsgBoxStyle.Information, "Info")

    End Sub

    Private Sub PastePersonButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles PastePersonButton.Click

        Dim clipboardText As String = System.Windows.Forms.Clipboard.GetText(TextDataFormat.UnicodeText)

        If clipboardText Is Nothing OrElse String.IsNullOrEmpty(clipboardText.Trim) Then

            MsgBox("Klaida. ClipBoard'as tuščias, t.y. nebuvo nukopijuoti jokie asmens duomenys.", _
                MsgBoxStyle.Exclamation, "Klaida")
            Exit Sub

        End If

        Dim info As InvoiceInfo.ClientInfo = Nothing
        Try
            Using busy As New StatusBusy
                info = InvoiceInfo.Factory.FromXmlString(Of InvoiceInfo.ClientInfo)(clipboardText)
            End Using
        Catch ex As Exception
            ShowError(New Exception("Klaida. Nepavyko atkurti asmens duomenų objekto. " _
                & "Teigtina, kad prieš tai į ClipBoard'ą buvo nukopijuota ne asmens duomenys, " _
                & "o šiaip kažkoks tekstas." & vbCrLf & "Klaidos tekstas: " & ex.Message, ex))
            Exit Sub
        End Try

        If info Is Nothing Then
            MsgBox("Klaida. Dėl nežinomų priežasčių nepavyko atkurti asmens duomenų objekto.", _
                MsgBoxStyle.Exclamation, "Klaida")
            Exit Sub
        End If

        If Not PrepareCache(Me, GetType(HelperLists.PersonInfoList)) Then Exit Sub
        Dim persons As HelperLists.PersonInfoList = HelperLists.PersonInfoList.GetList
        If Not persons.GetPersonInfo(info.Code) Is Nothing Then
            MsgBox("Klaida. Toks asmuo jau yra registruotas: " & info.Name _
                & ", įmonės kodas " & info.Code & ".", MsgBoxStyle.Exclamation, "Klaida")
            Exit Sub
        End If

        If Not Obj Is Nothing AndAlso TypeOf Obj Is IIsDirtyEnough AndAlso _
            DirectCast(Obj, IIsDirtyEnough).IsDirtyEnough Then
            Dim answ As String = Ask("Išsaugoti duomenis?", New ButtonStructure("Taip"), _
                New ButtonStructure("Ne"), New ButtonStructure("Atšaukti"))
            If answ <> "Taip" AndAlso answ <> "Ne" Then Exit Sub
            If answ = "Taip" Then
                If Not SaveObj() Then Exit Sub
            End If
        End If

        Try
            Using busy As New StatusBusy

                Using bm As New BindingsManager(PersonBindingSource, AssignedToGroupsBindingSource, _
                    Nothing, True, True)

                    Obj = LoadObject(Of Person)(Nothing, "NewPerson", True)
                    Obj.AddWithPersonInfoData(info)
                    bm.SetNewDataSource(Obj)

                End Using

            End Using
        Catch ex As Exception
            ShowError(New Exception("Klaida. Nepavyko įkrauti kopijuojamų asmens duomenų: " _
                & ex.Message, ex))
            Exit Sub
        End Try

    End Sub

    Private Sub OpenPersonFileButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles OpenPersonFileButton.Click

        Dim clipboardText As String = ""

        Using ofd As New OpenFileDialog
            If ofd.ShowDialog <> Windows.Forms.DialogResult.OK Then Exit Sub
            If ofd.FileName Is Nothing OrElse String.IsNullOrEmpty(ofd.FileName.Trim) _
                OrElse Not IO.File.Exists(ofd.FileName) Then Exit Sub
            Try
                clipboardText = IO.File.ReadAllText(ofd.FileName, New System.Text.UnicodeEncoding)
            Catch ex As Exception
                ShowError(New Exception("Klaida. Nepavyko atidaryti failo: " & ex.Message, ex))
                Exit Sub
            End Try
        End Using

        If clipboardText Is Nothing OrElse String.IsNullOrEmpty(clipboardText.Trim) Then
            MsgBox("Klaida. Failas tuščias.", MsgBoxStyle.Exclamation, "Klaida")
            Exit Sub
        End If

        Dim info As InvoiceInfo.ClientInfo = Nothing
        Try
            Using busy As New StatusBusy
                info = InvoiceInfo.Factory.FromXmlString(Of InvoiceInfo.ClientInfo)(clipboardText)
            End Using
        Catch ex As Exception
            ShowError(New Exception("Klaida. Nepavyko atkurti asmens duomenų objekto. " _
                & "Teigtina, kad faile saugomi ne asmens duomenys." & vbCrLf _
                & "Klaidos tekstas: " & ex.Message, ex))
            Exit Sub
        End Try

        If info Is Nothing Then
            MsgBox("Klaida. Dėl nežinomų priežasčių nepavyko atkurti asmens duomenų objekto.", _
                MsgBoxStyle.Exclamation, "Klaida")
            Exit Sub
        End If

        If Not PrepareCache(Me, GetType(HelperLists.PersonInfoList)) Then Exit Sub
        Dim persons As HelperLists.PersonInfoList = HelperLists.PersonInfoList.GetList
        If Not persons.GetPersonInfo(info.Code) Is Nothing Then
            MsgBox("Klaida. Toks asmuo jau yra registruotas: " & info.Name _
                & ", įmonės kodas " & info.Code & ".", MsgBoxStyle.Exclamation, "Klaida")
            Exit Sub
        End If

        If Not Obj Is Nothing AndAlso TypeOf Obj Is IIsDirtyEnough AndAlso _
            DirectCast(Obj, IIsDirtyEnough).IsDirtyEnough Then
            Dim answ As String = Ask("Išsaugoti duomenis?", New ButtonStructure("Taip"), _
                New ButtonStructure("Ne"), New ButtonStructure("Atšaukti"))
            If answ <> "Taip" AndAlso answ <> "Ne" Then Exit Sub
            If answ = "Taip" Then
                If Not SaveObj() Then Exit Sub
            End If
        End If

        Try
            Using busy As New StatusBusy

                Using bm As New BindingsManager(PersonBindingSource, AssignedToGroupsBindingSource, _
                    Nothing, True, True)

                    Obj = LoadObject(Of Person)(Nothing, "NewPerson", True)
                    Obj.AddWithPersonInfoData(info)
                    bm.SetNewDataSource(Obj)

                End Using

            End Using
        Catch ex As Exception
            ShowError(New Exception("Klaida. Nepavyko įkrauti asmens duomenų: " _
                & ex.Message, ex))
            Exit Sub
        End Try

    End Sub

    Private Sub SavePersonFileButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles SavePersonFileButton.Click

        If Obj Is Nothing Then Exit Sub

        Dim info As InvoiceInfo.ClientInfo = Nothing
        Try
            Using busy As New StatusBusy
                info = Obj.GetPersonInfo()
            End Using
        Catch ex As Exception
            ShowError(New Exception("Klaida. Nepavyko generuoti ClientInfo objekto: " _
                & ex.Message, ex))
            Exit Sub
        End Try

        If info Is Nothing Then
            MsgBox("Klaida. Dėl nežinomų priežasčių nepavyko generuoti ClientInfo objekto.", _
                MsgBoxStyle.Exclamation, "Klaida")
            Exit Sub
        End If

        Dim FileName As String

        Using sfd As New SaveFileDialog
            sfd.ValidateNames = True
            If sfd.ShowDialog <> Windows.Forms.DialogResult.OK Then Exit Sub
            If sfd.FileName Is Nothing OrElse String.IsNullOrEmpty(sfd.FileName.Trim) Then Exit Sub
            FileName = sfd.FileName
        End Using

        Try
            Using busy As New StatusBusy
                IO.File.WriteAllText(FileName, InvoiceInfo.Factory.ToXmlString _
                    (Of InvoiceInfo.ClientInfo)(info), New System.Text.UnicodeEncoding)
            End Using
        Catch ex As Exception
            ShowError(New Exception("Klaida. Nepavyko serializuoti ir išsaugoti ClientInfo objekto: " _
                & ex.Message, ex))
            Exit Sub
        End Try

        MsgBox("Asmens duomenys sėkmingai nukopijuoti į failą.", MsgBoxStyle.Information, "Info")

    End Sub


    Private Function SaveObj() As Boolean

        If Not Obj.IsDirty Then Return True

        If Not Obj.IsValid Then
            MsgBox("Formoje yra klaidų:" & vbCrLf & Obj.BrokenRulesCollection.ToString( _
                Csla.Validation.RuleSeverity.Error), MsgBoxStyle.Exclamation, "Klaida.")
            Return False
        End If

        Dim Question, Answer As String
        If Not String.IsNullOrEmpty(Obj.GetAllWarnings.Trim) Then
            Question = "DĖMESIO. Duomenyse gali būti klaidų: " & vbCrLf & Obj.GetAllWarnings & vbCrLf
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

            Using bm As New BindingsManager(PersonBindingSource, AssignedToGroupsBindingSource, Nothing, True, False)

                Try
                    Obj = LoadObject(Of Person)(Obj, "Save", False)
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
        Using bm As New BindingsManager(PersonBindingSource, AssignedToGroupsBindingSource, Nothing, True, True)
        End Using
    End Sub

    Private Function SetDataSources() As Boolean

        If Not PrepareCache(Me, GetType(HelperLists.CompanyRegionalInfoList), _
            GetType(HelperLists.AccountInfoList)) Then Return False

        Try

            LoadLanguageListToComboBox(LanguageNameComboBox, False)
            LoadCurrencyCodeListToComboBox(CurrencyCodeComboBox, False)
            LoadAccountInfoListToGridCombo(AccountAgainstBankBuyerAccGridComboBox, True, 2, 5)
            LoadAccountInfoListToGridCombo(AccountAgainstBankSupplyerAccGridComboBox, True, 2, 4, 6)

        Catch ex As Exception
            ShowError(ex)
            DisableAllControls(Me)
            Return False
        End Try

        Return True

    End Function

    Friend Sub RefreshPersonGroupList()
        If Obj Is Nothing Then Exit Sub
        Obj.AssignedToGroups.RefreshPersonGroupInfoList(PersonGroupInfoList.GetList, True)
    End Sub

End Class