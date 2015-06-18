Namespace Documents

    <Serializable()> _
    Public Class InvoiceMadeItemList
        Inherits BusinessListBase(Of InvoiceMadeItemList, InvoiceMadeItem)

#Region " Business Methods "

        Private _IsLoading As Boolean = False
        Private _LanguageCode As String = LanguageCodeLith.Trim.ToUpper
        Private _CurrencyRate As Double = 1
        Private _CurrencyCode As String = GetCurrentCompany.BaseCurrency
        Private _DefaultVatRate As Double = 21


        Friend ReadOnly Property LanguageCode() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _LanguageCode.Trim
            End Get
        End Property

        Friend ReadOnly Property CurrencyRate() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_CurrencyRate, 6)
            End Get
        End Property

        Friend ReadOnly Property CurrencyCode() As String
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _CurrencyCode.Trim
            End Get
        End Property

        Friend ReadOnly Property DefaultVatRate() As Double
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return CRound(_DefaultVatRate)
            End Get
        End Property


        Friend Sub UpdateLanguage(ByVal nLanguageCode As String)

            _LanguageCode = nLanguageCode

            RaiseListChangedEvents = False

            For Each o As InvoiceMadeItem In Me
                o.UpdateLanguage()
            Next

            RaiseListChangedEvents = True

            Me.ResetBindings()

        End Sub

        Friend Sub UpdateCurrencyRate(ByVal nCurrencyRate As Double, ByVal nCurrencyCode As String)

            RaiseListChangedEvents = False

            _CurrencyRate = nCurrencyRate
            _CurrencyCode = nCurrencyCode
            For Each o As InvoiceMadeItem In Me
                o.UpdateCurrencyRate(nCurrencyRate)
            Next

            RaiseListChangedEvents = True

            Me.ResetBindings()

        End Sub


        Friend Sub UpdateDate(ByVal nDate As Date)

            RaiseListChangedEvents = False

            For Each o As InvoiceMadeItem In Me
                o.SetAttachedObjectInvoiceDate(nDate)
            Next

            RaiseListChangedEvents = True

            Me.ResetBindings()

        End Sub

        Friend Sub MarkAsCopy()

            RaiseListChangedEvents = False
            _IsLoading = True

            Me.AllowNew = True
            Me.AllowRemove = True

            For i As Integer = Me.Count To 1 Step -1
                If Not Me.Item(i - 1).CanBeCopied Then
                    Me.RemoveAt(i - 1)
                Else
                    Me.Item(i - 1).MarkAsCopy()
                End If
            Next
            Me.DeletedList.Clear()

            _IsLoading = False
            RaiseListChangedEvents = True

            Me.ResetBindings()

        End Sub


        Friend Function GetInvoiceItemInfoList() As List(Of InvoiceInfo.InvoiceItemInfo)

            Dim result As New List(Of InvoiceInfo.InvoiceItemInfo)

            For Each i As InvoiceMadeItem In Me
                result.Add(i.GetInvoiceItemInfo())
            Next

            Return result

        End Function

        Friend Function GetChronologyValidators() As IChronologicValidator()

            Dim result As New List(Of IChronologicValidator)
            For Each i As InvoiceMadeItem In Me
                If Not i.AttachedObjectValue Is Nothing Then _
                    result.Add(i.AttachedObjectValue.ChronologyValidator)
            Next
            Return result.ToArray

        End Function


        Protected Overrides Function AddNewCore() As Object
            Dim newItem As InvoiceMadeItem = InvoiceMadeItem.NewInvoiceMadeItem()
            Me.Add(newItem)
            newItem.ResetBindings()
            Return newItem
        End Function

        Protected Overrides Sub RemoveItem(ByVal index As Integer)
            If Not _IsLoading Then
                If index < 0 OrElse index >= Me.Count Then Throw New IndexOutOfRangeException( _
                    "Index out of range in InvoiceMadeItemList.RemoveItem. Index=" & index.ToString _
                    & "; Count=" & Me.Count.ToString & ".")
                If Not Me.Item(index).AttachedObjectValue Is Nothing AndAlso _
                    Not Me.Item(index).AttachedObjectValue.ChronologyValidator.FinancialDataCanChange Then _
                    Throw New Exception("Klaida. Pašalinti sąskaitos faktūros eilutę neleidžiama:" _
                    & vbCrLf & Me.Item(index).AttachedObjectValue.ChronologyValidator.FinancialDataCanChangeExplanation)
            End If
            MyBase.RemoveItem(index)
        End Sub


        Public Function GetAllBrokenRules() As String
            Dim result As String = GetAllBrokenRulesForList(Me)

            'Dim GeneralErrorString As String = ""
            'SomeGeneralValidationSub(GeneralErrorString)
            'AddWithNewLine(result, GeneralErrorString, False)

            Return result
        End Function

        Public Function GetAllWarnings() As String

            Dim result As String = ""
            For Each o As InvoiceMadeItem In Me
                result = AddWithNewLine(result, o.GetWarningString, False)
            Next

            'Dim GeneralErrorString As String = ""
            'SomeGeneralValidationSub(GeneralErrorString)
            'AddWithNewLine(result, GeneralErrorString, False)

            Return result
        End Function

#End Region

#Region " Factory Methods "

        Friend Shared Function NewInvoiceMadeItemList() As InvoiceMadeItemList
            Return New InvoiceMadeItemList(Nothing, 1, GetCurrentCompany.BaseCurrency, LanguageCodeLith)
        End Function

        Friend Shared Function NewInvoiceMadeItemList(ByVal info As InvoiceInfo.InvoiceInfo, _
            ByVal pCurrencyRate As Double, ByVal pCurrencyCode As String, _
            ByVal pLanguageCode As String) As InvoiceMadeItemList
            Return New InvoiceMadeItemList(info, pCurrencyRate, pCurrencyCode, pLanguageCode)
        End Function

        Friend Shared Function GetInvoiceMadeItemList(ByVal InvoiceID As Integer, _
            ByVal pCurrencyRate As Double, ByVal pLanguageCode As String, _
            ByVal pDefaultVatRate As Double, ByVal baseChronologyValidator As SimpleChronologicValidator) As InvoiceMadeItemList
            Return New InvoiceMadeItemList(InvoiceID, pCurrencyRate, _
                pLanguageCode, pDefaultVatRate, baseChronologyValidator)
        End Function

        Friend Shared Function DoomyInvoiceMadeItemList() As InvoiceMadeItemList

            Dim result As InvoiceMadeItemList = New InvoiceMadeItemList

            result._CurrencyCode = "USD"
            result._CurrencyRate = 2.34267
            result._LanguageCode = "EN"
            result._DefaultVatRate = 21.0

            result.RaiseListChangedEvents = False

            Dim r As New Random

            For i As Integer = 1 To r.Next(5, 16)
                result.Add(InvoiceMadeItem.DoomyInvoiceMadeItem(r))
            Next

            result.RaiseListChangedEvents = True

            Return result

        End Function


        Private Sub New()
            ' require use of factory methods
            MarkAsChild()
            Me.AllowEdit = True
            Me.AllowNew = True
            Me.AllowRemove = True
        End Sub

        Private Sub New(ByVal info As InvoiceInfo.InvoiceInfo, ByVal pCurrencyRate As Double, _
            ByVal pCurrencyCode As String, ByVal pLanguageCode As String)
            ' require use of factory methods
            MarkAsChild()
            Me.AllowEdit = True
            Me.AllowNew = True
            Me.AllowRemove = True
            Create(info, pCurrencyRate, pCurrencyCode, pLanguageCode)
        End Sub

        Private Sub New(ByVal InvoiceID As Integer, ByVal pCurrencyRate As Double, _
            ByVal pLanguageCode As String, ByVal pDefaultVatRate As Double, _
            ByVal baseChronologyValidator As SimpleChronologicValidator)
            ' require use of factory methods
            MarkAsChild()
            Me.AllowEdit = True
            Me.AllowNew = baseChronologyValidator.FinancialDataCanChange
            Me.AllowRemove = baseChronologyValidator.FinancialDataCanChange
            Fetch(InvoiceID, pCurrencyRate, pLanguageCode, pDefaultVatRate, baseChronologyValidator)
        End Sub

#End Region

#Region " Data Access "

        Private Sub Create(ByVal info As InvoiceInfo.InvoiceInfo, _
            ByVal pCurrencyRate As Double, ByVal pCurrencyCode As String, _
            ByVal pLanguageCode As String)

            RaiseListChangedEvents = False
            _IsLoading = True

            Me._CurrencyCode = pCurrencyCode
            Me._CurrencyRate = pCurrencyRate
            Me._DefaultVatRate = GetCurrentCompany.Rates.GetRate(General.DefaultRateType.Vat)
            Me._LanguageCode = pLanguageCode

            If Not info Is Nothing Then
                For Each i As InvoiceInfo.InvoiceItemInfo In info.InvoiceItems
                    Add(InvoiceMadeItem.NewInvoiceMadeItem(i, pCurrencyRate))
                Next
            End If

            _IsLoading = False
            RaiseListChangedEvents = True

        End Sub

        Private Sub Fetch(ByVal InvoiceID As Integer, ByVal pCurrencyRate As Double, _
            ByVal pLanguageCode As String, ByVal pDefaultVatRate As Double, _
            ByVal baseChronologyValidator As SimpleChronologicValidator)

            Dim myComm As New SQLCommand("FetchInvoiceMadeItemList")
            myComm.AddParam("?MD", InvoiceID)

            Using myData As DataTable = myComm.Fetch

                RaiseListChangedEvents = False
                _IsLoading = True

                _LanguageCode = pLanguageCode
                _CurrencyRate = pCurrencyRate
                _DefaultVatRate = pDefaultVatRate

                For Each dr As DataRow In myData.Rows
                    Add(InvoiceMadeItem.GetInvoiceMadeItem(dr, pCurrencyRate, baseChronologyValidator))
                Next

                _IsLoading = False
                RaiseListChangedEvents = True

            End Using

        End Sub

        Friend Sub CheckRules(ByVal parentChronologyValidator As IChronologicValidator)
            For Each item As InvoiceMadeItem In DeletedList
                If Not item.IsNew Then item.CheckIfCanDelete(parentChronologyValidator)
            Next
            For Each item As InvoiceMadeItem In Me
                If item.IsDirty Then item.CheckIfCanUpdate(Not parentChronologyValidator. _
                    FinancialDataCanChange, parentChronologyValidator)
            Next
            If Not IsValid Then Throw New Exception("Duomenyse yra klaidų: " & GetAllBrokenRules())
        End Sub

        Friend Sub Update(ByVal parent As InvoiceMade)

            RaiseListChangedEvents = False
            _IsLoading = True

            For Each item As InvoiceMadeItem In DeletedList
                If Not item.IsNew Then item.DeleteSelf()
            Next
            DeletedList.Clear()

            For Each item As InvoiceMadeItem In Me
                If item.IsNew Then
                    item.Insert(parent)
                ElseIf item.IsDirty Then
                    item.Update(parent)
                End If
            Next

            _IsLoading = False
            RaiseListChangedEvents = True

        End Sub

#End Region

    End Class

End Namespace