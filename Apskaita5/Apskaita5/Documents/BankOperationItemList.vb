Imports ApskaitaObjects.HelperLists
Namespace Documents

    <Serializable()> _
    Public Class BankOperationItemList
        Inherits BusinessListBase(Of BankOperationItemList, BankOperationItem)
        Implements IIsDirtyEnough

#Region " Business Methods "

        Private _Account As CashAccountInfo = Nothing
        Private _ImportsSource As BankOperationImportSourceType = BankOperationImportSourceType.PasteString
        Private _BalanceStart As Double = 0
        Private _BalanceEnd As Double = 0
        Private _Income As Double = 0
        Private _Spendings As Double = 0
        Private _DateStart As Date = Today
        Private _DateEnd As Date = Today


        Public ReadOnly Property Account() As CashAccountInfo
            Get
                Return _Account
            End Get
        End Property

        Public ReadOnly Property ImportsSource() As String
            Get
                Return ConvertEnumHumanReadable(_ImportsSource)
            End Get
        End Property

        Public ReadOnly Property BalanceStart() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_BalanceStart)
            End Get
        End Property

        Public ReadOnly Property BalanceEnd() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_BalanceEnd)
            End Get
        End Property

        Public ReadOnly Property Income() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_Income)
            End Get
        End Property

        Public ReadOnly Property Spendings() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_Spendings)
            End Get
        End Property

        Public ReadOnly Property DateStart() As Date
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DateStart
            End Get
        End Property

        Public ReadOnly Property DateEnd() As Date
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DateEnd
            End Get
        End Property


        Public ReadOnly Property IsDirtyEnough() As Boolean Implements IIsDirtyEnough.IsDirtyEnough
            Get
                If Me.Count < 1 Then Return False
                For Each i As BankOperationItem In Me
                    If Not i.ExistsInDatabase Then Return True
                Next
                Return False
            End Get
        End Property


        Public Function GetDescription() As String
            If _ImportsSource = BankOperationImportSourceType.LITAS_ESIS Then
                Return "Duomenys įkrauti iš LITAS-ESIS formato failo - banko išrašo. " _
                    & "Banko išrašo periodas nuo " & _DateStart.ToShortDateString _
                    & " iki " & _DateEnd.ToShortDateString & "." & vbCrLf & "Balansas pradžioje - " _
                    & DblParser(_BalanceStart) & " " & _Account.CurrencyCode.Trim.ToUpper _
                    & "." & vbCrLf & "Įplaukos per išrašo laikotarpį - " & DblParser(_Income) & " " _
                    & _Account.CurrencyCode.Trim.ToUpper & ". Išlaidos per išrašo laikotarpį - " _
                    & DblParser(_Spendings) & " " & _Account.CurrencyCode.Trim.ToUpper _
                    & "." & vbCrLf & "Balansas pabaigoje - " & DblParser(_BalanceEnd) & " " _
                    & _Account.CurrencyCode.Trim.ToUpper & "."
            Else
                Return "Duomenys įkrauti iš copy - paste'o."
            End If
        End Function

        Public Function GetAllBrokenRules() As String
            Dim result As String = GetAllBrokenRulesForList(Me)
            Return result
        End Function

        Public Function GetAllWarnings() As String
            Dim result As String = GetAllWarningsForList(Me)
            Return result
        End Function


        Public Sub UpdateWithBankOperation(ByVal savedBankOperation As BankOperation)
            For Each i As BankOperationItem In Me
                i.IdentifyWithBankOperation(savedBankOperation, _Account)
            Next
        End Sub


        Public Overrides Function Save() As BankOperationItemList

            If Not Me.Count > 0 Then Throw New Exception("Klaida. Neįvesta nė viena eilutė.")
            If Not Me.IsValid Then Throw New Exception("Įvestuose duomenyse yra klaidų: " _
                & GetAllBrokenRules())

            Return MyBase.Save()

        End Function

#End Region

#Region " Authorization Rules "

        Public Shared Function CanGetObject() As Boolean
            Return False
        End Function

        Public Shared Function CanAddObject() As Boolean
            Return BankOperation.CanAddObject
        End Function

        Public Shared Function CanEditObject() As Boolean
            Return False
        End Function

        Public Shared Function CanDeleteObject() As Boolean
            Return False
        End Function

#End Region

#Region " Factory Methods "

        ' used to implement automatic sort in datagridview
        <NonSerialized()> _
        Private _SortedList As Csla.SortedBindingList(Of BankOperationItem) = Nothing

        Public Shared Function GetListFromLitasEsisFile(ByVal FileName As String, _
            ByVal nAccount As CashAccountInfo, ByVal BankDocumentPrefix As String, _
            ByVal IgnoreWrongIBAN As Boolean) As BankOperationItemList

            If FileName Is Nothing OrElse String.IsNullOrEmpty(FileName.Trim) Then _
                Throw New Exception("Klaida. Nenurodytas failas.")
            If Not IO.File.Exists(FileName) Then Throw New Exception( _
                "Klaida. Failas '" & FileName & "' nerastas.")
            If nAccount Is Nothing OrElse Not nAccount.ID > 0 Then Throw New Exception( _
                "Klaida. Nenurodyta banko sąskaita, į kurią importuojami duomenys.")
            If nAccount.Type <> CashAccountType.PseudoBankAccount AndAlso _
                nAccount.Type <> CashAccountType.BankAccount Then Throw New Exception( _
                "Klaida. Nurodyta sąskaita nėra banko sąskaita.")

            Dim readText() As String = IO.File.ReadAllLines(FileName, _
                System.Text.Encoding.GetEncoding("windows-1257")) ' "ISO-8859-13"

            Dim nBalanceStart As Double = 0
            Dim nBalanceEnd As Double = 0
            Dim nDateStart As Date = Today
            Dim nDateEnd As Date = Today

            Dim OperationString As String = ""
            For Each s As String In readText
                If GetElement(s, 0).Trim = "010" Then

                    If Not String.IsNullOrEmpty(OperationString.Trim) Then _
                        OperationString = OperationString.Trim & "##%%OpSeparator%%##"
                    OperationString = OperationString.Trim & s

                ElseIf GetElement(s, 0).Trim = "000" Then

                    If Not String.IsNullOrEmpty(GetElement(s, 17).Trim) AndAlso _
                        GetElement(s, 17).Trim.ToUpper <> nAccount.CurrencyCode.Trim.ToUpper Then _
                        Throw New Exception("Klaida. Faile nurodyta sąskaitos valiuta " _
                            & GetElement(s, 17).Trim.ToUpper & " nesutampa su pasirinktos " _
                            & "sąskaitos valiuta " & nAccount.CurrencyCode.Trim.ToUpper & ".")
                    If Not IgnoreWrongIBAN AndAlso Not String.IsNullOrEmpty(GetElement(s, 16).Trim) AndAlso _
                        GetElement(s, 16).Trim.ToUpper <> nAccount.BankAccountNumber.Trim.ToUpper Then _
                        Throw New Exception("Klaida. Faile nurodytas IBAN sąskaitos numeris " _
                            & GetElement(s, 16).Trim.ToUpper & " nesutampa su pasirinktos " _
                            & "sąskaitos IBAN numeriu " & nAccount.BankAccountNumber.Trim.ToUpper & ".")

                ElseIf GetElement(s, 0).Trim = "020" Then

                    If GetElement(s, 1).Trim.ToLower = "likutispr" Then
                        nBalanceStart = CRound(CLongSafe(GetElement(s, 4), 0) / 100)
                        Dim tm As String = GetElement(s, 2).Trim
                        If tm.Length = 8 Then nDateStart = New Date(CIntSafe(tm.Substring(0, 4)), _
                            CIntSafe(tm.Substring(4, 2)), CIntSafe(tm.Substring(6, 2)))
                    ElseIf GetElement(s, 1).Trim.ToLower = "likutispb" Then
                        nBalanceEnd = CRound(CLongSafe(GetElement(s, 4), 0) / 100)
                        Dim tm As String = GetElement(s, 2).Trim
                        If tm.Length = 8 Then nDateEnd = New Date(CIntSafe(tm.Substring(0, 4)), _
                            CIntSafe(tm.Substring(4, 2)), CIntSafe(tm.Substring(6, 2)))
                    End If

                End If
            Next

            If String.IsNullOrEmpty(OperationString.Trim) Then Throw New Exception( _
                "Klaida. Failas nėra banko duomenų failas LITAS-ESIS formatu arba išraše nėra įplaukų.")

            Dim result As BankOperationItemList = DataPortal.Fetch(Of BankOperationItemList) _
                (New Criteria(nAccount, OperationString, BankOperationImportSourceType.LITAS_ESIS, _
                 BankDocumentPrefix, nBalanceStart, nBalanceEnd, nDateStart, nDateEnd))

            Return result

        End Function

        Public Shared Function GetListFromPasteString(ByVal PasteString As String, _
            ByVal nAccount As CashAccountInfo, ByVal BankDocumentPrefix As String) As BankOperationItemList

            If nAccount Is Nothing OrElse Not nAccount.ID > 0 Then Throw New Exception( _
                "Klaida. Nenurodyta banko sąskaita, į kurią importuojami duomenys.")
            If nAccount.Type <> CashAccountType.PseudoBankAccount AndAlso _
                nAccount.Type <> CashAccountType.BankAccount Then Throw New Exception( _
                "Klaida. Nurodyta sąskaita nėra banko sąskaita.")
            If PasteString Is Nothing OrElse String.IsNullOrEmpty(PasteString.Trim) Then _
                Throw New Exception("Klaida. Nėra ką paste'inti. Turi būti paste'inami 13 stulpelių. " _
                    & vbCrLf & " Stulpeliai turi būti išdėstyti tokia seka: " _
                    & "[data], [dokumento numeris], [asmens kodas], [asmens pavadinimas], " _
                    & "[turinys/aprašas], [pajamos("""")/išlaidos(""x"")], [valiuta], " _
                    & "[originali suma], [suma LTL], [suma sąskaitos valiuta, i.e. " _
                    & nAccount.CurrencyCode.Trim.ToUpper & "], [unikalus kodas], " _
                    & "[asmens (banko) sąskaitos numeris], [asmens banko pavadinimas].")

            Dim lineDelim As Char() = {ControlChars.Cr, ControlChars.Lf}
            Dim colDelim As Char() = {ControlChars.Tab}

            Dim lines As String() = PasteString.Split(lineDelim, StringSplitOptions.None)

            If lines(0).Split(colDelim, StringSplitOptions.None).Length <> 13 Then _
                Throw New Exception("Klaida. Turi būti paste'inami 13 stulpelių, o yra " _
                    & lines(0).Split(colDelim, StringSplitOptions.None).Length.ToString & "." _
                    & vbCrLf & " Stulpeliai turi būti išdėstyti tokia seka: " _
                    & "[data], [dokumento numeris], [asmens kodas], [asmens pavadinimas], " _
                    & "[turinys/aprašas], [pajamos("""")/išlaidos(""x"")], [valiuta], " _
                    & "[originali suma], [suma LTL], [suma sąskaitos valiuta, i.e. " _
                    & nAccount.CurrencyCode.Trim.ToUpper & "], [unikalus kodas], " _
                    & "[asmens (banko) sąskaitos numeris], [asmens banko pavadinimas].")

            Dim OperationString As String = ""
            For Each s As String In lines
                If s.Split(colDelim, StringSplitOptions.None).Length = 13 Then
                    If Not String.IsNullOrEmpty(OperationString.Trim) Then _
                        OperationString = OperationString.Trim & "##%%OpSeparator%%##"
                    OperationString = OperationString.Trim & s
                End If
            Next

            Dim result As BankOperationItemList = DataPortal.Fetch(Of BankOperationItemList) _
                (New Criteria(nAccount, OperationString, BankOperationImportSourceType.PasteString, _
                BankDocumentPrefix, 0, 0, Today, Today))

            Return result

        End Function

        Public Function GetSortedList() As Csla.SortedBindingList(Of BankOperationItem)
            If _SortedList Is Nothing Then _SortedList = New Csla.SortedBindingList(Of BankOperationItem)(Me)
            Return _SortedList
        End Function

        Private Sub New()
            ' require use of factory methods
            Me.AllowEdit = True
            Me.AllowNew = False
            Me.AllowRemove = True
        End Sub

#End Region

#Region " Data Access "

        <Serializable()> _
        Private Class Criteria
            Private _Account As CashAccountInfo = Nothing
            Private _ImportsSource As BankOperationImportSourceType = BankOperationImportSourceType.PasteString
            Private _OperationString As String = ""
            Private _BankDocumentPrefix As String = ""
            Private _BalanceStart As Double = 0
            Private _BalanceEnd As Double = 0
            Private _DateStart As Date = Today
            Private _DateEnd As Date = Today
            Public ReadOnly Property Account() As CashAccountInfo
                Get
                    Return _Account
                End Get
            End Property
            Public ReadOnly Property ImportsSource() As BankOperationImportSourceType
                Get
                    Return _ImportsSource
                End Get
            End Property
            Public ReadOnly Property OperationString() As String
                Get
                    Return _OperationString
                End Get
            End Property
            Public ReadOnly Property BankDocumentPrefix() As String
                Get
                    Return _BankDocumentPrefix
                End Get
            End Property
            Public ReadOnly Property BalanceStart() As Double
                Get
                    Return CRound(_BalanceStart)
                End Get
            End Property
            Public ReadOnly Property BalanceEnd() As Double
                Get
                    Return CRound(_BalanceEnd)
                End Get
            End Property
            Public ReadOnly Property DateStart() As Date
                Get
                    Return _DateStart
                End Get
            End Property
            Public ReadOnly Property DateEnd() As Date
                Get
                    Return _DateEnd
                End Get
            End Property
            Public Sub New(ByVal nAccount As CashAccountInfo, ByVal nOperationString As String, _
                ByVal nImportsSource As BankOperationImportSourceType, _
                ByVal nBankDocumentPrefix As String, ByVal nBalanceStart As Double, _
                ByVal nBalanceEnd As Double, ByVal nDateStart As Date, ByVal nDateEnd As Date)
                _Account = nAccount
                _OperationString = nOperationString
                _ImportsSource = nImportsSource
                _BankDocumentPrefix = nBankDocumentPrefix
                _BalanceStart = nBalanceStart
                _BalanceEnd = nBalanceEnd
                _DateStart = nDateStart
                _DateEnd = nDateEnd
            End Sub
        End Class


        Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

            If Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka naujų duomenų registravimui.")

            Dim OperationStrings() As String = criteria.OperationString.Split( _
                New String() {"##%%OpSeparator%%##"}, StringSplitOptions.RemoveEmptyEntries)

            RaiseListChangedEvents = False

            _Account = criteria.Account
            _ImportsSource = criteria.ImportsSource
            _BalanceStart = criteria.BalanceStart
            _BalanceEnd = criteria.BalanceEnd
            _DateStart = criteria.DateStart
            _DateEnd = criteria.DateEnd

            For Each s As String In OperationStrings
                Add(BankOperationItem.GetBankOperationItem(s, criteria.Account, _
                    criteria.ImportsSource, criteria.BankDocumentPrefix))
            Next

            If criteria.Account.EnforceUniqueOperationID Then
                For Each i As BankOperationItem In Me
                    i.CheckIfUniqueCodeIsEnforcable(criteria.Account)
                Next
            End If

            Dim BankPersonInfo As PersonInfo = Nothing
            If criteria.Account.ManagingPersonID > 0 Then

                Dim myComm As New SQLCommand("FetchPersonInfoByID")
                myComm.AddParam("?CD", criteria.Account.ManagingPersonID)

                Using myData As DataTable = myComm.Fetch
                    If myData.Rows.Count > 0 Then BankPersonInfo _
                        = PersonInfo.GetPersonInfo(myData.Rows(0), 0)
                End Using

            End If

            _Income = 0
            _Spendings = 0
            For Each i As BankOperationItem In Me
                i.RecognizeItem(criteria.Account, BankPersonInfo)
                If i.Inflow Then
                    _Income += i.SumInAccountBank
                Else
                    _Spendings += i.SumInAccountBank
                End If
            Next

            _Income = CRound(_Income)
            _Spendings = CRound(_Spendings)

            RaiseListChangedEvents = True

        End Sub

        Protected Overrides Sub DataPortal_Update()

            If Not CanAddObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka naujų duomenų registravimui.")

            CheckUniqueCodeConstraintsWithinList()

            For Each item As BankOperationItem In Me
                item.PrepareForInsert(_Account)
            Next

            RaiseListChangedEvents = False

            DeletedList.Clear()

            DatabaseAccess.TransactionBegin()

            For Each item As BankOperationItem In Me
                item.Insert()
            Next

            DatabaseAccess.TransactionCommit()

            RaiseListChangedEvents = True

        End Sub

        Private Sub CheckUniqueCodeConstraintsWithinList()

            If Not _Account.EnforceUniqueOperationID Then Exit Sub

            Dim i, j As Integer
            Dim operationsWithoutUniqueCode As New List(Of String)
            Dim operationsWithDuplicateUniqueCode As New List(Of String)
            For i = 1 To Me.Count

                If String.IsNullOrEmpty(Me.Item(i - 1).UniqueCode.Trim) Then
                    operationsWithoutUniqueCode.Add(Me.Item(i - 1).ToString)
                End If

                For j = i + 1 To Me.Count
                    If Me.Item(i - 1).UniqueCode.Trim.ToUpper = Me.Item(j - 1).UniqueCode.Trim.ToUpper Then
                        operationsWithDuplicateUniqueCode.Add(Me.Item(i - 1).ToString _
                            & " -> " & Me.Item(j - 1).ToString)
                    End If
                Next

            Next

            Dim exceptionMessage As String = ""

            If operationsWithoutUniqueCode.Count > 0 Then
                exceptionMessage = String.Format("Duomenyse ne visur nurodyti unikalūs operacijų kodai, " _
                    & "o pasirinktoje sąskaitoje reikalaujama garantuoti kodų unikalumą. Operacijos be unikalaus kodo:{0}{1}", _
                    vbCrLf, String.Concat(operationsWithoutUniqueCode.ToArray))
            End If
            If operationsWithDuplicateUniqueCode.Count > 0 Then
                exceptionMessage = exceptionMessage & vbCrLf & String.Format("Duomenyse pasikartoja unikalūs operacijų kodai, " _
                    & "o pasirinktoje sąskaitoje reikalaujama garantuoti kodų unikalumą. Operacijos su pasikartojančiu unikaliu kodu:{0}{1}", _
                    vbCrLf, String.Concat(operationsWithDuplicateUniqueCode.ToArray))
                exceptionMessage = exceptionMessage.Trim
            End If

            If Not String.IsNullOrEmpty(exceptionMessage.Trim) Then
                Throw New Exception(String.Format("{0} {1}", "Klaida.", exceptionMessage))
            End If

        End Sub

#End Region

    End Class

End Namespace