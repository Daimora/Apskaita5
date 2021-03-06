Namespace HelperLists

    ''' <summary>
    ''' Represents a <see cref="General.Person">person's</see> value object.
    ''' </summary>
    ''' <remarks>Values are stored in the database table asmenys.</remarks>
    <Serializable()> _
    Public NotInheritable Class PersonInfo
        Inherits ReadOnlyBase(Of PersonInfo)
        Implements IComparable, IValueObject

#Region " Business Methods "

        Private _ID As Integer = 0
        Private _Name As String = ""
        Private _Code As String = ""
        Private _Address As String = ""
        Private _Bank As String = ""
        Private _BankAccount As String = ""
        Private _CodeVAT As String = ""
        Private _CodeSODRA As String = ""
        Private _Email As String = ""
        Private _AccountAgainstBankBuyer As Long = 0
        Private _AccountAgainstBankSupplyer As Long = 0
        Private _ContactInfo As String = ""
        Private _InternalCode As String = ""
        Private _LanguageCode As String = LanguageCodeLith
        Private _LanguageName As String = GetLanguageName(LanguageCodeLith, False)
        Private _CurrencyCode As String = GetCurrentCompany.BaseCurrency
        Private _IsNaturalPerson As Boolean = False
        Private _IsObsolete As Boolean = False
        Private _IsClient As Boolean = False
        Private _IsSupplier As Boolean = False
        Private _IsWorker As Boolean = False


        ''' <summary>
        ''' Whether an object is a place holder (does not represent a real person).
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly Property IsEmpty() As Boolean _
            Implements IValueObject.IsEmpty
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return Not _ID > 0
            End Get
        End Property

        ''' <summary>
        ''' Gets an ID of the person (assigned automaticaly by DB AUTOINCREMENT).
        ''' Returns 0 for a new person.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="General.Person.ID">Person.ID</see> property.
        ''' Value is stored in the database field asmenys.ID.</remarks>
        Public ReadOnly Property ID() As Integer
            Get
                Return _ID
            End Get
        End Property

        ''' <summary>
        ''' Gets an official name of the person.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="General.Person.Name">Person.Name</see> property.
        ''' Value is stored in the database field asmenys.Pavad.</remarks>
        Public ReadOnly Property Name() As String
            Get
                Return _Name
            End Get
        End Property

        ''' <summary>
        ''' Gets an official registration/personal code of the person.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="General.Person.Code">Person.Code</see> property.
        ''' Value is stored in the database field asmenys.Kodas.</remarks>
        Public ReadOnly Property Code() As String
            Get
                Return _Code
            End Get
        End Property

        ''' <summary>
        ''' Gets an address of the person.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="General.Person.Address">Person.Address</see> property.
        ''' Value is stored in the database field asmenys.Adresas.</remarks>
        Public ReadOnly Property Address() As String
            Get
                Return _Address
            End Get
        End Property

        ''' <summary>
        ''' Gets a name of the bank used by the person.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="General.Person.Bank">Person.Bank</see> property.
        ''' Value is stored in the database field asmenys.Bank.</remarks>
        Public ReadOnly Property Bank() As String
            Get
                Return _Bank
            End Get
        End Property

        ''' <summary>
        ''' Gets a bank account number used by the person.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="General.Person.BankAccount">Person.BankAccount</see> property.
        ''' Value is stored in the database field asmenys.B_Sask.</remarks>
        Public ReadOnly Property BankAccount() As String
            Get
                Return _BankAccount
            End Get
        End Property

        ''' <summary>
        ''' Gets a VAT payer code of the person.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="General.Person.CodeVAT">Person.CodeVAT</see> property.
        ''' Value is stored in the database field asmenys.SP_kodas.</remarks>
        Public ReadOnly Property CodeVAT() As String
            Get
                Return _CodeVAT
            End Get
        End Property

        ''' <summary>
        ''' Gets a SODRA (social security) code of the person.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="General.Person.CodeSODRA">Person.CodeSODRA</see> property.
        ''' Only applicable to natural persons.
        ''' Value is stored in the database field asmenys.SD_kodas.</remarks>
        Public ReadOnly Property CodeSODRA() As String
            Get
                Return _CodeSODRA
            End Get
        End Property

        ''' <summary>
        ''' Gets an email address of the person.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="General.Person.Email">Person.Email</see> property.
        ''' Value is stored in the database field asmenys.E_Mail.</remarks>
        Public ReadOnly Property Email() As String
            Get
                Return _Email
            End Get
        End Property

        ''' <summary>
        ''' Gets any other person info, e.g. phone number, etc.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="General.Person.ContactInfo">Person.ContactInfo</see> property.
        ''' Value is stored in the database field asmenys.ContactInfo.</remarks>
        Public ReadOnly Property ContactInfo() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ContactInfo.Trim
            End Get
        End Property

        ''' <summary>
        ''' Gets an internal code of the person for company's uses.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="General.Person.InternalCode">Person.InternalCode</see> property.
        ''' Value is stored in the database field asmenys.InternalCode.</remarks>
        Public ReadOnly Property InternalCode() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _InternalCode.Trim
            End Get
        End Property

        ''' <summary>
        ''' Gets default language ISO 639-1 code used by the person.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="General.Person.LanguageCode">Person.LanguageCode</see> property.
        ''' Value is stored in the database field asmenys.LanguageCode.</remarks>
        Public ReadOnly Property LanguageCode() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _LanguageCode.Trim
            End Get
        End Property

        ''' <summary>
        ''' Gets default language used by the person.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="General.Person.LanguageName">Person.LanguageName</see> property.
        ''' Value is stored in the database field asmenys.LanguageCode.</remarks>
        Public ReadOnly Property LanguageName() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _LanguageName.Trim
            End Get
        End Property

        ''' <summary>
        ''' Gets default currency code used by the person.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="General.Person.CurrencyCode">Person.CurrencyCode</see> property.
        ''' Value is stored in the database field asmenys.CurrencyCode.</remarks>
        Public ReadOnly Property CurrencyCode() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _CurrencyCode.Trim
            End Get
        End Property

        ''' <summary>
        ''' Gets an account for buyers' debts.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="General.Person.AccountAgainstBankBuyer">Person.AccountAgainstBankBuyer</see> property.
        ''' Used when importing bank operations of type 'money received'. Credits this account, debits bank account.
        ''' Value is stored in the database field asmenys.B_Kor.</remarks>
        Public ReadOnly Property AccountAgainstBankBuyer() As Long
            Get
                Return _AccountAgainstBankBuyer
            End Get
        End Property

        ''' <summary>
        ''' Gets an account for suppliers' debts.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="General.Person.AccountAgainstBankSupplyer">Person.AccountAgainstBankSupplyer</see> property.
        ''' Used when importing bank operations of type 'money transfered'. Debits this account, credits bank account.
        ''' Value is stored in the database field asmenys.B_Kor_Tiek.</remarks>
        Public ReadOnly Property AccountAgainstBankSupplyer() As Long
            Get
                Return _AccountAgainstBankSupplyer
            End Get
        End Property

        ''' <summary>
        ''' Gets if the person is a natural person, i.e. not a company.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="General.Person.IsNaturalPerson">Person.IsNaturalPerson</see> property.
        ''' Value is stored in the database field asmenys.IsNaturalPerson.</remarks>
        Public ReadOnly Property IsNaturalPerson() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _IsNaturalPerson
            End Get
        End Property

        ''' <summary>
        ''' Gets if the person is no longer in use, i.e. not supposed to be displayed in combos.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="General.Person.IsObsolete">Person.IsObsolete</see> property.
        ''' Value is stored in the database field asmenys.IsObsolete.</remarks>
        Public ReadOnly Property IsObsolete() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _IsObsolete
            End Get
        End Property

        ''' <summary>
        ''' Gets a user friendly full name of a person.
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly Property NameUserFriendly() As String
            Get
                Return Me.ToString
            End Get
        End Property

        ''' <summary>
        ''' Whether a person is a client of the company.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="General.Person.IsClient">Person.IsClient</see> property.
        ''' Value is stored in the database field asmenys.IsClient.</remarks>
        Public ReadOnly Property IsClient() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _IsClient
            End Get
        End Property

        ''' <summary>
        ''' Whether a person is a supplier of the company.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="General.Person.IsSupplier">Person.IsSupplier</see> property.
        ''' Value is stored in the database field asmenys.IsSupplier.</remarks>
        Public ReadOnly Property IsSupplier() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _IsSupplier
            End Get
        End Property

        ''' <summary>
        ''' Whether a person is a worker of the company.
        ''' </summary>
        ''' <remarks>Corresponds to <see cref="General.Person.IsWorker">Person.IsWorker</see> property.
        ''' Value is stored in the database field asmenys.IsWorker.</remarks>
        Public ReadOnly Property IsWorker() As Boolean
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _IsWorker
            End Get
        End Property


        Public Shared Operator =(ByVal a As PersonInfo, ByVal b As PersonInfo) As Boolean

            Dim aId, bId As Integer
            If a Is Nothing OrElse a.IsEmpty Then
                aId = 0
            Else
                aId = a.ID
            End If
            If b Is Nothing OrElse b.IsEmpty Then
                bId = 0
            Else
                bId = b.ID
            End If

            Return aId = bId

        End Operator

        Public Shared Operator <>(ByVal a As PersonInfo, ByVal b As PersonInfo) As Boolean
            Return Not a = b
        End Operator

        Public Shared Operator >(ByVal a As PersonInfo, ByVal b As PersonInfo) As Boolean

            Dim aToString, bToString As String
            If a Is Nothing OrElse a.IsEmpty Then
                aToString = ""
            Else
                aToString = a.ToString
            End If
            If b Is Nothing OrElse b.IsEmpty Then
                bToString = ""
            Else
                bToString = b.ToString
            End If

            Return aToString > bToString

        End Operator

        Public Shared Operator <(ByVal a As PersonInfo, ByVal b As PersonInfo) As Boolean

            Dim aToString, bToString As String
            If a Is Nothing OrElse a.IsEmpty Then
                aToString = ""
            Else
                aToString = a.ToString
            End If
            If b Is Nothing OrElse b.IsEmpty Then
                bToString = ""
            Else
                bToString = b.ToString
            End If

            Return aToString < bToString

        End Operator

        Public Function CompareTo(ByVal obj As Object) As Integer Implements System.IComparable.CompareTo
            Dim tmp As PersonInfo = TryCast(obj, PersonInfo)
            If Me = tmp Then Return 0
            If Me > tmp Then Return 1
            Return -1
        End Function


        Friend Function GetValueObjectIdString() As String
            If Me.IsEmpty Then Return ""
            Return _ID.ToString(Globalization.CultureInfo.InvariantCulture)
        End Function


        Protected Overrides Function GetIdValue() As Object
            Return _ID
        End Function

        Public Overrides Function ToString() As String
            If Not _ID > 0 AndAlso StringIsNullOrEmpty(_Name) Then Return ""
            If Not _ID > 0 Then Return String.Format( _
                My.Resources.HelperLists_PersonInfo_NewPersonToString, _Name, _Code)
            Return String.Format("{0} ({1})", _Name, _Code)
        End Function

#End Region

#Region " Factory Methods "

        Private Shared _Empty As PersonInfo = Nothing

        ''' <summary>
        ''' Gets an empty PersonInfo (placeholder).
        ''' </summary>
        Public Shared Function Empty() As PersonInfo
            If _Empty Is Nothing Then
                _Empty = New PersonInfo
            End If
            Return _Empty
        End Function

        ''' <summary>
        ''' Gets a doomy person info with made up data for demonstration pusposes.
        ''' </summary>
        ''' <remarks></remarks>
        Friend Shared Function DoomyPersonInfo() As PersonInfo

            Dim result As New PersonInfo
            result._AccountAgainstBankBuyer = 0
            result._AccountAgainstBankSupplyer = 0
            result._Address = My.Resources.HelperLists_PersonInfo_DoomyPersonAddress
            result._Bank = My.Resources.HelperLists_PersonInfo_DoomyPersonBankName
            result._BankAccount = My.Resources.HelperLists_PersonInfo_DoomyPersonBankAccount
            result._Code = My.Resources.HelperLists_PersonInfo_DoomyPersonCode
            result._CodeSODRA = My.Resources.HelperLists_PersonInfo_DoomyPersonCodeSodra
            result._CodeVAT = My.Resources.HelperLists_PersonInfo_DoomyPersonCodeVat
            result._ContactInfo = My.Resources.HelperLists_PersonInfo_DoomyPersonContactInfo
            result._CurrencyCode = "USD"
            result._Email = My.Resources.HelperLists_PersonInfo_DoomyPersonEmail
            result._InternalCode = "KON_VK0001"
            result._IsClient = True
            result._IsNaturalPerson = True
            result._IsObsolete = False
            result._IsSupplier = True
            result._IsWorker = True
            result._LanguageCode = LanguageCodeLith.Trim.ToUpper
            result._LanguageName = GetLanguageName(LanguageCodeLith.Trim.ToUpper)
            result._Name = My.Resources.HelperLists_PersonInfo_DoomyPersonName
            result._ID = Integer.MaxValue

            Return result

        End Function

        ''' <summary>
        ''' Gets an existing person info by a database query.
        ''' </summary>
        ''' <param name="dr">DataRow containing account data.</param>
        ''' <param name="offset">An offset by which data is "pushed" to compare to original 
        ''' position in query result. Used when person info data is returned along with other data,
        ''' e.g. as part of <see cref="General.JournalEntry">JournalEntry</see> data.</param>
        Friend Shared Function GetPersonInfo(ByVal dr As DataRow, ByVal offset As Integer) As PersonInfo
            Return New PersonInfo(dr, offset)
        End Function

        ''' <summary>
        ''' Gets a person info with a known ID. 
        ''' </summary>
        ''' <param name="personID">Known person <see cref="ID">ID</see>.</param>
        ''' <param name="fetchData">Whether to fetch person data or to leave it empty.</param>
        ''' <remarks>Used for mass operations for person assignements 
        ''' when the actual person data (name, code, etc.) is not of importance.</remarks>
        Friend Shared Function GetPersonInfoChild(ByVal personID As Integer, ByVal fetchData As Boolean) As PersonInfo
            Return New PersonInfo(personID, fetchData)
        End Function

        Friend Shared Function GetPersonInfo(ByVal prs As General.Person) As PersonInfo
            Return New PersonInfo(prs)
        End Function


        Private Sub New()
            ' require use of factory methods
        End Sub

        Private Sub New(ByVal dr As DataRow, ByVal offset As Integer)
            Fetch(dr, offset)
        End Sub

        Private Sub New(ByVal personID As Integer, ByVal fetchData As Boolean)
            _ID = personID
            If fetchData Then Fetch(personID)
        End Sub

        Private Sub New(ByVal prs As General.Person)
            Create(prs)
        End Sub

#End Region

#Region " Data Access "

        Private Sub Create(ByVal prs As General.Person)
            _AccountAgainstBankBuyer = prs.AccountAgainstBankBuyer
            _AccountAgainstBankSupplyer = prs.AccountAgainstBankSupplyer
            _Address = prs.Address
            _Bank = prs.Bank
            _BankAccount = prs.BankAccount
            _Code = prs.Code
            _CodeSODRA = prs.CodeSODRA
            _CodeVAT = prs.CodeVAT
            _ContactInfo = prs.ContactInfo
            _CurrencyCode = prs.CurrencyCode
            _Email = prs.Email
            _ID = prs.ID
            _InternalCode = prs.InternalCode
            _IsClient = prs.IsClient
            _IsNaturalPerson = prs.IsNaturalPerson
            _IsObsolete = prs.IsObsolete
            _IsSupplier = prs.IsSupplier
            _IsWorker = prs.IsWorker
            _LanguageCode = prs.LanguageCode
            _LanguageName = prs.LanguageName
            _Name = prs.Name
        End Sub

        Private Sub Fetch(ByVal personID As Integer)

            Dim myComm As New SQLCommand("FetchPersonInfoByID")
            myComm.AddParam("?CD", personID)

            Using myData As DataTable = myComm.Fetch

                If myData.Rows.Count < 1 Then Throw New Exception(String.Format( _
                    My.Resources.Common_ObjectNotFound, My.Resources.HelperLists_PersonInfo_TypeName, _
                    personID.ToString()))

                Fetch(myData.Rows(0), 0)

            End Using

        End Sub

        Private Sub Fetch(ByVal dr As DataRow, ByVal offset As Integer)

            _ID = CIntSafe(dr.Item(0 + offset), 0)
            _Name = CStrSafe(dr.Item(1 + offset)).Trim
            _Code = CStrSafe(dr.Item(2 + offset)).Trim
            _Address = CStrSafe(dr.Item(3 + offset)).Trim
            _CodeVAT = CStrSafe(dr.Item(4 + offset)).Trim
            _BankAccount = CStrSafe(dr.Item(5 + offset)).Trim
            _Bank = CStrSafe(dr.Item(6 + offset)).Trim
            _AccountAgainstBankBuyer = CLongSafe(dr.Item(7 + offset), 0)
            _AccountAgainstBankSupplyer = CLongSafe(dr.Item(8 + offset), 0)
            _Email = CStrSafe(dr.Item(9 + offset)).Trim
            _CodeSODRA = CStrSafe(dr.Item(10 + offset)).Trim
            _ContactInfo = CStrSafe(dr.Item(11 + offset)).Trim
            _InternalCode = CStrSafe(dr.Item(12 + offset)).Trim
            _IsObsolete = ConvertDbBoolean(CIntSafe(dr.Item(13 + offset), 0))
            _IsNaturalPerson = ConvertDbBoolean(CIntSafe(dr.Item(14 + offset), 0))
            _LanguageCode = CStrSafe(dr.Item(15 + offset)).Trim
            _LanguageName = GetLanguageName(_LanguageCode, False)
            _CurrencyCode = CStrSafe(dr.Item(16 + offset)).Trim
            _IsClient = ConvertDbBoolean(CIntSafe(dr.Item(17 + offset), 0))
            _IsSupplier = ConvertDbBoolean(CIntSafe(dr.Item(18 + offset), 0))
            _IsWorker = ConvertDbBoolean(CIntSafe(dr.Item(19 + offset), 0))

        End Sub

#End Region

    End Class

End Namespace