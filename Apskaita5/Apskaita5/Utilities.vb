Imports System.Configuration
Imports System.Globalization
Public Module Utilities

    Public Delegate Sub SaveCommonSettingsLocal(ByVal settingsXmlString As String)
    Public Delegate Function GetCommonSettingsLocal() As String

    Friend _SaveCommonSettingsLocal As SaveCommonSettingsLocal = Nothing
    Friend _GetCommonSettingsLocal As GetCommonSettingsLocal = Nothing

    Public Sub AttachLocalSettingsMethods(ByVal nSaveCommonSettingsLocal As SaveCommonSettingsLocal, _
        ByVal nGetCommonSettingsLocal As GetCommonSettingsLocal)

        _SaveCommonSettingsLocal = nSaveCommonSettingsLocal
        _GetCommonSettingsLocal = nGetCommonSettingsLocal

    End Sub

    ''' <summary>
    ''' Returns all common information about current company and it's settings.
    ''' </summary>
    Public Function GetCurrentCompany() As ApskaitaObjects.Settings.CompanyInfo
        If Not IsLoggedInDB() Then _
            Throw New Exception("Klaida. Vartotojas neprisijungęs prie jokios įmonės.")
        If Not ApplicationContext.GlobalContext.Contains(KeyCompanyInfo) _
            OrElse Not TypeOf ApplicationContext.GlobalContext.Item(KeyCompanyInfo) _
            Is ApskaitaObjects.Settings.CompanyInfo Then Throw New Exception("Klaida. Nerasti bendri įmonės duomenys.")

        Return DirectCast(ApplicationContext.GlobalContext.Item(KeyCompanyInfo),  _
            ApskaitaObjects.Settings.CompanyInfo)

    End Function

    ''' <summary>
    ''' Returns TRUE if the current user is in the role.
    ''' </summary>
    Public Function UserIsInAdminRole() As Boolean
        Return ApplicationContext.User.IsInRole(AccDataAccessLayer.ProjectConstants.Name_AdminRole)
    End Function


    Public Function GetAllBrokenRulesForList(ByVal list As Csla.Core.IExtendedBindingList) As String

        If list Is Nothing OrElse list.Count < 1 Then Return ""

        If Not TypeOf list.Item(0) Is IGetErrorForListItem Then
            Throw New NotImplementedException(String.Format(My.Resources.Common_TypeDoesNotImplementInterface, _
                list.Item(0).GetType.FullName, GetType(IGetErrorForListItem).FullName))
        End If

        Dim result As String = ""

        Dim currentError As String
        For Each item As IGetErrorForListItem In list
            currentError = item.GetErrorString
            result = AddWithNewLine(result, currentError, False)
        Next

        Return result

    End Function

    Public Function GetAllWarningsForList(ByVal list As Csla.Core.IExtendedBindingList) As String

        If list Is Nothing OrElse list.Count < 1 Then Return ""

        If Not TypeOf list.Item(0) Is IGetErrorForListItem Then
            Throw New NotImplementedException(String.Format(My.Resources.Common_TypeDoesNotImplementInterface, _
                list.Item(0).GetType.FullName, GetType(IGetErrorForListItem).FullName))
        End If

        Dim result As String = ""

        Dim currentWarning As String
        For Each item As IGetErrorForListItem In list
            currentWarning = item.GetWarningString
            result = AddWithNewLine(result, currentWarning, False)
        Next

        Return result

    End Function


#Region " Enum conversion methods "

    Private _commonEnumMap As List(Of EnumMapItem) = Nothing

    Private Structure EnumMapItem
        Public EnumValue As [Enum]
        Public HumanReadableString As String
        Public DatabaseCode As Integer
        Public DatabaseStringCode As String
        Public Sub New(ByVal nEnumValue As [Enum], ByVal nHumanReadableString As String, _
            ByVal nDatabaseCode As Integer, Optional ByVal nDatabaseStringCode As String = "")
            EnumValue = nEnumValue
            HumanReadableString = nHumanReadableString
            DatabaseCode = nDatabaseCode
            DatabaseStringCode = nDatabaseStringCode
        End Sub
        Public Function IsMatch(ByVal ObjType As Type) As Boolean
            Return ObjType Is EnumValue.GetType
        End Function
        Public Function IsMatch(ByVal Obj As [Enum]) As Boolean
            Return Obj.GetType Is EnumValue.GetType AndAlso _
                 System.Enum.GetName(Obj.GetType, Obj) = _
                 System.Enum.GetName(Obj.GetType, EnumValue)
        End Function
        Public Function IsMatch(ByVal ObjType As Type, ByVal DbCode As Integer) As Boolean
            Return ObjType Is EnumValue.GetType AndAlso DbCode = DatabaseCode
        End Function
        Public Function IsMatch(ByVal ObjType As Type, ByVal DbStringCode As String, ByVal NullValue As Object) As Boolean
            Return ObjType Is EnumValue.GetType AndAlso DbStringCode.Trim.ToLower = DatabaseStringCode.Trim.ToLower
        End Function
        Public Function IsMatch(ByVal ObjType As Type, _
            ByVal HumanReadableValue As String) As Boolean
            Return ObjType Is EnumValue.GetType AndAlso _
                HumanReadableValue.Trim.ToLower = HumanReadableString.Trim.ToLower
        End Function
    End Structure

    Private Sub InitializeCommonEnumMap()

        _commonEnumMap = New List(Of EnumMapItem)

        '*** OPERATION CHRONOLOGY TYPE ***

        _commonEnumMap.Add(New EnumMapItem(OperationChronologyType.Overall, _
            "Iš visų operacijų", 0))
        _commonEnumMap.Add(New EnumMapItem(OperationChronologyType.LastBefore, _
            "Paskutinė prieš", 1))
        _commonEnumMap.Add(New EnumMapItem(OperationChronologyType.FirstAfter, _
            "Pirma po", 2))

        '*** FINANCIAL STATEMENT ITEM TYPE ***

        _commonEnumMap.Add(New EnumMapItem(General.FinancialStatementItemType.StatementOfFinancialPosition, _
            "Balanso ataskaitos eilutė", 0))
        _commonEnumMap.Add(New EnumMapItem(General.FinancialStatementItemType.StatementOfComprehensiveIncome, _
            "Pelno (nuostolio) ataskaitos eilutė", 1))
        _commonEnumMap.Add(New EnumMapItem(General.FinancialStatementItemType.HeaderStatementOfFinancialPosition, _
            "Balanso ataskaitos antraštės eilutė", 2))
        _commonEnumMap.Add(New EnumMapItem(General.FinancialStatementItemType.HeaderStatementOfComprehensiveIncome, _
            "Pelno (nuostolio) ataskaitos antraštės eilutė", 3))
        _commonEnumMap.Add(New EnumMapItem(General.FinancialStatementItemType.HeaderGeneral, _
            "Finansinių ataskaitų rinkinio antraštės eilutė", 4))

        '*** TAX TARIF TYPE ***

        _commonEnumMap.Add(New EnumMapItem(TaxRateType.GPM, _
            "GPM", 0, "GPM"))
        _commonEnumMap.Add(New EnumMapItem(TaxRateType.PSDForCompany, _
            "PSD įmonei", 1, "PSDP"))
        _commonEnumMap.Add(New EnumMapItem(TaxRateType.PSDForPerson, _
            "PSD išskaič.", 2, "PSDI"))
        _commonEnumMap.Add(New EnumMapItem(TaxRateType.SodraForCompany, _
            "SODRA įmonei", 3, "SODRAP"))
        _commonEnumMap.Add(New EnumMapItem(TaxRateType.SodraForPerson, _
            "SODRA išskaič.", 4, "SODRAI"))
        _commonEnumMap.Add(New EnumMapItem(TaxRateType.Vat, _
            "PVM", 5, "PVM"))

        '*** DEFAULT RATE TYPE ***

        _commonEnumMap.Add(New EnumMapItem(General.DefaultRateType.GpmWage, _
            "GPM darbo santykių", 0))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultRateType.GuaranteeFund, _
            "Garantinis fondas", 1))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultRateType.PsdEmployee, _
            "PSD darbuotojui", 2))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultRateType.PsdEmployer, _
            "PSD darbdaviui", 3))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultRateType.SodraEmployee, _
            "SODRA darbuotojui", 4))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultRateType.SodraEmployer, _
            "SODRA darbdaviui", 5))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultRateType.Vat, _
            "PVM", 6))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultRateType.WageRateDeviations, _
            "Darbo užm. nenorm. sąlyg.", 7))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultRateType.WageRateNight, _
            "Darbo užm. naktinis", 8))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultRateType.WageRateOvertime, _
            "Darbo užm. viršv.", 9))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultRateType.WageRatePublicHolidays, _
            "Darbo užm. švenčių", 10))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultRateType.WageRateRestTime, _
            "Darbo užm. poilsio", 11))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultRateType.WageRateSickLeave, _
            "Darbo užm. nedarbing.", 12))

        '*** DEFAULT ACCOUNT TYPE ***

        _commonEnumMap.Add(New EnumMapItem(General.DefaultAccountType.Bank, _
            "Banko", 0, "BK"))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultAccountType.Buyers, _
            "Pirkėjų", 1, "PR"))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultAccountType.Suppliers, _
            "Tiekėjų", 2, "TK"))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultAccountType.Till, _
            "Kasos", 3, "KS"))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultAccountType.VatPayable, _
            "Mokėtinas PVM", 4, "PV"))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultAccountType.VatReceivable, _
            "Gautinas PVM", 5, "PG"))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultAccountType.WageGpmPayable, _
            "Mokėtinas GPM darbo sant.", 6, "GP"))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultAccountType.WageGuaranteeFundPayable, _
            "Mokėtinos garant. įmokos", 7, "GR"))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultAccountType.WageImprestPayable, _
            "Mokėtini darbo užm. avansai", 8, "DA"))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultAccountType.WagePayable, _
            "Mokėtinas darbo užm.", 9, "DU"))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultAccountType.WagePsdPayable, _
            "Mokėtinas PSD darbo sant.", 10, "SS"))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultAccountType.WageSodraPayable, _
            "Mokėtina SODRA darbo sant.", 11, "SD"))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultAccountType.WageWithdraw, _
            "Išskaitų iš darbo užm.", 12, "IS"))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultAccountType.WagePsdPayableToVMI, _
            "Mokėtinas PSD darbo sant. VMI", 13, "SV"))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultAccountType.ClosingSummary, _
            "Uždarymo suvestinė", 14, "SU"))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultAccountType.CurrentProfit, _
            "Pelnas (nuostolis) einamasis", 15, "PL"))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultAccountType.FormerProfit, _
            "Pelnas (nuostolis) ankstesnis", 16, "PA"))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultAccountType.OtherGpmPayable, _
            "Mokėtinas GPM kitas", 17, "OG"))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultAccountType.OtherPsdPayable, _
            "Mokėtinas PSD kitas", 18, "OP"))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultAccountType.OtherSodraPayable, _
            "Mokėtina SODRA kita", 19, "OS"))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultAccountType.GoodsSalesNetCosts, _
            "Prekių pardavimo sąn.", 20, "GS"))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultAccountType.GoodsPurchases, _
            "Prekių pirkimai", 21, "GQ"))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultAccountType.GoodsDiscounts, _
            "Prekių nuolaidos", 22, "GD"))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultAccountType.GoodsValueReduction, _
            "Prekių nukainojimas", 23, "GV"))
        _commonEnumMap.Add(New EnumMapItem(General.DefaultAccountType.HolidayReserve, _
            "Atostogų rezervas", 24, "HR"))

        '*** GOODS OPERATION TYPE ***

        _commonEnumMap.Add(New EnumMapItem(Goods.GoodsOperationType.Acquisition, _
            "Įsigijimas", 1))
        _commonEnumMap.Add(New EnumMapItem(Goods.GoodsOperationType.Transfer, _
            "Perleidimas", 2))
        _commonEnumMap.Add(New EnumMapItem(Goods.GoodsOperationType.Discard, _
            "Nurašymas", 3))
        _commonEnumMap.Add(New EnumMapItem(Goods.GoodsOperationType.Inventorization, _
            "Inventorizacija", 4))
        _commonEnumMap.Add(New EnumMapItem(Goods.GoodsOperationType.ConsignmentDiscount, _
            "Gauta Nuolaida", 5))
        _commonEnumMap.Add(New EnumMapItem(Goods.GoodsOperationType.ConsignmentAdditionalCosts, _
            "Papildomi Kaštai", 6))
        _commonEnumMap.Add(New EnumMapItem(Goods.GoodsOperationType.PriceCut, _
            "Nukainojimas", 7))
        _commonEnumMap.Add(New EnumMapItem(Goods.GoodsOperationType.AccountSalesNetCostsChange, _
            "Pardavimo Savik. Sąsk. Pakeitimas", 9))
        _commonEnumMap.Add(New EnumMapItem(Goods.GoodsOperationType.AccountPurchasesChange, _
            "Pardavimo Sąsk. Pakeitimas", 10))
        _commonEnumMap.Add(New EnumMapItem(Goods.GoodsOperationType.AccountDiscountsChange, _
            "Nuolaidų Sąsk. Pakeitimas", 11))
        _commonEnumMap.Add(New EnumMapItem(Goods.GoodsOperationType.AccountValueReductionChange, _
            "Nukainojimo Sąsk. Pakeitimas", 12))
        _commonEnumMap.Add(New EnumMapItem(Goods.GoodsOperationType.ValuationMethodChange, _
            "Vertinimo Metodo Pakeitimas", 13))
        _commonEnumMap.Add(New EnumMapItem(Goods.GoodsOperationType.TransferOfBalance, _
            "Likučių perkėlimas", 14))

        '*** GOODS COMPLEX OPERATION TYPE ***

        _commonEnumMap.Add(New EnumMapItem(Goods.GoodsComplexOperationType.InternalTransfer, _
            "Vidinis judėjimas", 1))
        _commonEnumMap.Add(New EnumMapItem(Goods.GoodsComplexOperationType.Production, _
            "Gamyba", 2))
        _commonEnumMap.Add(New EnumMapItem(Goods.GoodsComplexOperationType.Inventorization, _
            "Inventorizacija", 3))
        _commonEnumMap.Add(New EnumMapItem(Goods.GoodsComplexOperationType.BulkDiscard, _
            "Masinis nurašymas", 4))
        _commonEnumMap.Add(New EnumMapItem(Goods.GoodsComplexOperationType.BulkPriceCut, _
            "Masinis nukainojimas", 5))
        _commonEnumMap.Add(New EnumMapItem(Goods.GoodsComplexOperationType.TransferOfBalance, _
            "Likučių perkėlimas", 6))

        '*** PRODUCTION COMPONENT TYPE ***

        _commonEnumMap.Add(New EnumMapItem(Goods.ProductionComponentType.Component, _
            "Komplektuojančios prekės/žaliavos", 0, "a"))
        _commonEnumMap.Add(New EnumMapItem(Goods.ProductionComponentType.Costs, _
            "Gamybos savikainos sąnaudos", 1, "s"))

        '*** DOCUMENT SERIAL TYPE ***

        _commonEnumMap.Add(New EnumMapItem(Settings.DocumentSerialType.Invoice, _
            "Sąskaita - faktūra", 0, "sf"))
        _commonEnumMap.Add(New EnumMapItem(Settings.DocumentSerialType.LabourContract, _
            "Darbo sutartis", 1, "ds"))
        _commonEnumMap.Add(New EnumMapItem(Settings.DocumentSerialType.TillIncomeOrder, _
            "Kasos pajamų orderis", 2, "kpo"))
        _commonEnumMap.Add(New EnumMapItem(Settings.DocumentSerialType.TillSpendingsOrder, _
            "Kasos išlaidų orderis", 3, "kio"))

        '*** DOCUMENT TYPE ***

        _commonEnumMap.Add(New EnumMapItem(DocumentType.ImprestSheet, _
            "Avanso žiniaraštis", 0, "av"))
        _commonEnumMap.Add(New EnumMapItem(DocumentType.WageSheet, _
            "Darbo užmokesčio žiniaraštis", 1, "du"))
        _commonEnumMap.Add(New EnumMapItem(DocumentType.TillSpendingOrder, _
            "Kasos išlaidų orderis", 2, "kio"))
        _commonEnumMap.Add(New EnumMapItem(DocumentType.TillIncomeOrder, _
            "Kasos pajamų orderis", 3, "kpo"))
        _commonEnumMap.Add(New EnumMapItem(DocumentType.InvoiceMade, _
            "Išrašyta sąskaita - faktūra", 4, "sf"))
        _commonEnumMap.Add(New EnumMapItem(DocumentType.InvoiceReceived, _
            "Gauta sąskaita - faktūra", 5, "sg"))
        _commonEnumMap.Add(New EnumMapItem(DocumentType.GoodsInternalTransfer, _
            "Prekių vidinis judėjimas", 6, "a"))
        _commonEnumMap.Add(New EnumMapItem(DocumentType.Amortization, _
            "Amortizacija", 7, "amo"))
        _commonEnumMap.Add(New EnumMapItem(DocumentType.BankOperation, _
            "Banko operacija", 8, "b"))
        _commonEnumMap.Add(New EnumMapItem(DocumentType.GoodsProduction, _
            "Gamyba", 9, "g"))
        _commonEnumMap.Add(New EnumMapItem(DocumentType.LongTermAssetDiscard, _
            "Ilgalaikio turto nurašymas", 10, "t"))
        _commonEnumMap.Add(New EnumMapItem(DocumentType.GoodsWriteOff, _
            "Atsargų nurašymas", 11, "tn"))
        _commonEnumMap.Add(New EnumMapItem(DocumentType.ClosingEntries, _
            "5/6 klasių uždarymas", 12, "uz"))
        _commonEnumMap.Add(New EnumMapItem(DocumentType.GoodsRevalue, _
            "Atsargų nukainojimas (atstatymas)", 13, "v"))
        _commonEnumMap.Add(New EnumMapItem(DocumentType.TransferOfBalance, _
            "Likučių perkėlimas", 14, "lik"))
        _commonEnumMap.Add(New EnumMapItem(DocumentType.AdvanceReport, _
            "Avanso apyskaita", 15, "ap"))
        _commonEnumMap.Add(New EnumMapItem(DocumentType.LongTermAssetAccountChange, _
            "Ilgalaikio turto apsk. sąsk. pakeitimas", 16, "tsp"))
        _commonEnumMap.Add(New EnumMapItem(DocumentType.Offset, "Užskaita", 17, "sk"))
        _commonEnumMap.Add(New EnumMapItem(DocumentType.None, "Nėra", 18, ""))
        _commonEnumMap.Add(New EnumMapItem(DocumentType.GoodsInventorization, _
            "Atsargų inventorizacija", 19, "gi"))
        _commonEnumMap.Add(New EnumMapItem(DocumentType.GoodsAccountChange, _
            "Atsargų apskaitos sąsk. pakeitimas", 20, "ga"))
        _commonEnumMap.Add(New EnumMapItem(DocumentType.AccumulatedCosts, _
            "Sukauptos sąnaudos", 21, "ac"))
        _commonEnumMap.Add(New EnumMapItem(DocumentType.HolidayReserve, _
            "Atostogų rezervo pažyma", 22, "hr"))

        '*** GOODS ACCOUNTING METHOD ***

        _commonEnumMap.Add(New EnumMapItem(Goods.GoodsAccountingMethod.Persistent, _
            "Nuolat apskaitomos", 0))
        _commonEnumMap.Add(New EnumMapItem(Goods.GoodsAccountingMethod.Periodic, _
            "Periodiškai apskaitomos", 1))

        '*** GOODS VALUATION METHOD ***

        _commonEnumMap.Add(New EnumMapItem(Goods.GoodsValuationMethod.FIFO, _
            "FIFO", 0))
        _commonEnumMap.Add(New EnumMapItem(Goods.GoodsValuationMethod.LIFO, _
            "LIFO", 1))
        _commonEnumMap.Add(New EnumMapItem(Goods.GoodsValuationMethod.Average, _
            "Vidurkių", 2))

        '*** BOOK ENTRY TYPE ***

        _commonEnumMap.Add(New EnumMapItem(BookEntryType.Debetas, _
            "Debetas", 0, "Debetas"))
        _commonEnumMap.Add(New EnumMapItem(BookEntryType.Kreditas, _
            "Kreditas", 1, "Kreditas"))

        '*** WAGE TYPE ***

        _commonEnumMap.Add(New EnumMapItem(Workers.WageType.Position, _
            "Pareiginis", 0, "p"))
        _commonEnumMap.Add(New EnumMapItem(Workers.WageType.Hourly, _
            "Valandinis", 1, "v"))

        '*** WORKER STATUS TYPE ***

        _commonEnumMap.Add(New EnumMapItem(Workers.WorkerStatusType.Employed, _
            "Įsidarbino", 0, "d"))
        _commonEnumMap.Add(New EnumMapItem(Workers.WorkerStatusType.ExtraPay, _
            "Priedas", 1, "i"))
        _commonEnumMap.Add(New EnumMapItem(Workers.WorkerStatusType.Fired, _
            "Atleistas", 2, "n"))
        _commonEnumMap.Add(New EnumMapItem(Workers.WorkerStatusType.Holiday, _
            "Kasmetinės atostogos", 3, "a"))
        _commonEnumMap.Add(New EnumMapItem(Workers.WorkerStatusType.HolidayCorrection, _
            "Atostogų korekcija", 4, "o"))
        _commonEnumMap.Add(New EnumMapItem(Workers.WorkerStatusType.NPD, _
            "Taikytinas NPD", 5, "p"))
        _commonEnumMap.Add(New EnumMapItem(Workers.WorkerStatusType.PNPD, _
            "Taikytinas PNPD", 6, "r"))
        _commonEnumMap.Add(New EnumMapItem(Workers.WorkerStatusType.Wage, _
            "Darbo užmokestis", 7, "u"))
        _commonEnumMap.Add(New EnumMapItem(Workers.WorkerStatusType.WorkLoad, _
            "Darbo krūvis (etatai)", 8, "k"))
        _commonEnumMap.Add(New EnumMapItem(Workers.WorkerStatusType.Position, _
            "Pareigos", 9, "f"))

        '*** BONUS TYPE ***

        _commonEnumMap.Add(New EnumMapItem(Workers.BonusType.k, _
            "Ketvirtinė", 0, "k"))
        _commonEnumMap.Add(New EnumMapItem(Workers.BonusType.m, _
            "Metinė", 1, "m"))

        '*** LONG TERM ASSET OPERATION TYPE ***

        _commonEnumMap.Add(New EnumMapItem(Assets.LtaOperationType.AccountChange, _
            "Apsk. sąsk. pakeitimas", 0, "aac"))
        _commonEnumMap.Add(New EnumMapItem(Assets.LtaOperationType.AcquisitionValueIncrease, _
            "Įsig. vertės padid.", 1, "avi"))
        _commonEnumMap.Add(New EnumMapItem(Assets.LtaOperationType.Amortization, _
            "Skaičiuoti amort.", 2, "amo"))
        _commonEnumMap.Add(New EnumMapItem(Assets.LtaOperationType.AmortizationPeriod, _
            "Naujas amort. laik.", 3, "alk"))
        _commonEnumMap.Add(New EnumMapItem(Assets.LtaOperationType.Discard, _
            "Nurašyti", 4, "nur"))
        _commonEnumMap.Add(New EnumMapItem(Assets.LtaOperationType.Transfer, _
            "Perleisti", 5, "per"))
        _commonEnumMap.Add(New EnumMapItem(Assets.LtaOperationType.UsingStart, _
            "Pradėti naudoti", 7, "nau"))
        _commonEnumMap.Add(New EnumMapItem(Assets.LtaOperationType.UsingEnd, _
            "Baigti naudoti", 6, "nau"))
        _commonEnumMap.Add(New EnumMapItem(Assets.LtaOperationType.ValueChange, _
            "Vertės pakeitimas", 8, "avd"))

        '*** LONG TERM ASSET ACCOUNT CHANGE TYPE ***

        _commonEnumMap.Add(New EnumMapItem(Assets.LtaAccountChangeType.AcquisitionAccount, _
            "Savikainos", 0, "aqs"))
        _commonEnumMap.Add(New EnumMapItem(Assets.LtaAccountChangeType.AmortizationAccount, _
            "Amortizacijos (kontrar.)", 1, "amr"))
        _commonEnumMap.Add(New EnumMapItem(Assets.LtaAccountChangeType.ValueDecreaseAccount, _
            "Vertės sumažėjimo", 2, "vld"))
        _commonEnumMap.Add(New EnumMapItem(Assets.LtaAccountChangeType.ValueIncreaseAccount, _
            "Vertės padidėjimo", 3, "vli"))
        _commonEnumMap.Add(New EnumMapItem(Assets.LtaAccountChangeType.ValueIncreaseAmortizationAccount, _
            "Vertės padidėjimo amort.", 4, "vam"))

        '*** CASH ACCOUNT TYPE ***

        _commonEnumMap.Add(New EnumMapItem(Documents.CashAccountType.BankAccount, _
            "Banko sąskaita", 0))
        _commonEnumMap.Add(New EnumMapItem(Documents.CashAccountType.PseudoBankAccount, _
            "Pseudo banko sąskaita", 1))
        _commonEnumMap.Add(New EnumMapItem(Documents.CashAccountType.Till, _
            "Kasa", 2))

        '*** WORK TIME TYPE ***

        _commonEnumMap.Add(New EnumMapItem(Workers.WorkTimeType.NightWork, "Darbas naktį", 0))
        _commonEnumMap.Add(New EnumMapItem(Workers.WorkTimeType.OtherExcluded, "Kitas neįskaitomas į darbo laiką", 1))
        _commonEnumMap.Add(New EnumMapItem(Workers.WorkTimeType.OtherIncluded, "Kitas įskaitomas į darbo laiką", 2))
        _commonEnumMap.Add(New EnumMapItem(Workers.WorkTimeType.OvertimeWork, "Viršvalandžiai", 3))
        _commonEnumMap.Add(New EnumMapItem(Workers.WorkTimeType.PublicHolidaysAndRestDayWork, "Darbas švenčių ir poilsio dienomis", 4))
        _commonEnumMap.Add(New EnumMapItem(Workers.WorkTimeType.Truancy, "Pravaikšta", 5))
        _commonEnumMap.Add(New EnumMapItem(Workers.WorkTimeType.UnusualWork, "Darbas ypatingomis sąlygomis", 6))
        _commonEnumMap.Add(New EnumMapItem(Workers.WorkTimeType.DownTime, "Prastova", 7))
        _commonEnumMap.Add(New EnumMapItem(Workers.WorkTimeType.SickDays, "Liga", 8))
        _commonEnumMap.Add(New EnumMapItem(Workers.WorkTimeType.AnnualHolidays, "Kasmetinės atostogos", 9))
        _commonEnumMap.Add(New EnumMapItem(Workers.WorkTimeType.OtherHolidays, "Kitos atostogos", 10))

        '*** INDIRECT RELATION TYPE ***

        _commonEnumMap.Add(New EnumMapItem(IndirectRelationType.PayoutToResident, _
            "Išmoka fiziniam asmeniui", 0))
        _commonEnumMap.Add(New EnumMapItem(IndirectRelationType.LongTermAssetsOperation, _
            "Operacija su ilgalaikiu turtu", 1))
        _commonEnumMap.Add(New EnumMapItem(IndirectRelationType.LongTermAssetsPurchase, _
            "Ilgalaikio turto įsigijimas", 2))
        _commonEnumMap.Add(New EnumMapItem(IndirectRelationType.GoodsOperation, _
            "Operacija su prekėmis", 3))
        _commonEnumMap.Add(New EnumMapItem(IndirectRelationType.AdvanceReport, _
            "Avanso apyskaita", 4))

    End Sub

    Public Function ConvertEnumDatabaseCode(Of T)(ByVal DatabaseCode As Integer) As T

        If Not GetType(T).IsEnum Then Throw New ArgumentException( _
            "Klaida. Funkcija ApskaitaObjects.Utilities.ConvertEnumDatabaseCode " & _
            "gali konvertuoti tik enumeracijas, o nurodyto objekto tipas - " & _
            GetType(T).FullName & " .")

        If _commonEnumMap Is Nothing Then InitializeCommonEnumMap()

        For Each item As EnumMapItem In _commonEnumMap
            If item.IsMatch(GetType(T), DatabaseCode) Then Return _
                DirectCast(DirectCast(item.EnumValue, Object), T)
        Next

        Throw New Exception("Enumeracijos tipo " & GetType(T).FullName & _
            " vertė, duomenų bazėje pažymėta kodu " & DatabaseCode.ToString & ", nežinoma.")

    End Function

    Public Function ConvertEnumDatabaseCode(ByVal EnumValue As [Enum]) As Integer

        If _commonEnumMap Is Nothing Then InitializeCommonEnumMap()

        For Each item As EnumMapItem In _commonEnumMap
            If item.IsMatch(EnumValue) Then Return item.DatabaseCode
        Next

        Throw New Exception("Enumeracijos tipo " & EnumValue.ToString & _
            " kodas duomenų bazėje nežinomas.")

    End Function

    Public Function ConvertEnumDatabaseStringCode(Of T)(ByVal DatabaseStringCode As String) As T

        If Not GetType(T).IsEnum Then Throw New ArgumentException( _
            "Klaida. Funkcija ApskaitaObjects.Utilities.ConvertEnumDatabaseCode " & _
            "gali konvertuoti tik enumeracijas, o nurodyto objekto tipas - " & _
            GetType(T).FullName & " .")

        If _commonEnumMap Is Nothing Then InitializeCommonEnumMap()

        For Each item As EnumMapItem In _commonEnumMap
            If item.IsMatch(GetType(T), DatabaseStringCode, Nothing) Then _
                Return DirectCast(DirectCast(item.EnumValue, Object), T)
        Next

        Throw New Exception("Enumeracijos tipo " & GetType(T).ToString & _
            " vertė, duomenų bazėje pažymėta kodu " & DatabaseStringCode & ", nežinoma.")

    End Function

    Public Function ConvertEnumDatabaseStringCode(ByVal EnumValue As [Enum]) As String

        If _commonEnumMap Is Nothing Then InitializeCommonEnumMap()

        For Each item As EnumMapItem In _commonEnumMap
            If item.IsMatch(EnumValue) Then Return item.DatabaseStringCode
        Next

        Throw New Exception("Enumeracijos tipo " & EnumValue.ToString & _
            " kodas duomenų bazėje nežinomas.")

    End Function

    Public Function ConvertEnumHumanReadable(Of T)(ByVal HumanReadableString As String) As T

        If Not GetType(T).IsEnum Then Throw New ArgumentException( _
            "Klaida. Funkcija ApskaitaObjects.Utilities.ConvertEnumHumanReadable " & _
            "gali konvertuoti tik enumeracijas, o nurodyto objekto tipas - " & _
            GetType(T).ToString & " .")

        If _commonEnumMap Is Nothing Then InitializeCommonEnumMap()

        For Each item As EnumMapItem In _commonEnumMap
            If item.IsMatch(GetType(T), HumanReadableString) Then _
                Return DirectCast(DirectCast(item.EnumValue, Object), T)
        Next

        Throw New Exception("Enumeracijos tipo " & GetType(T).ToString & _
            " vertė, įvardinta kaip " & HumanReadableString.ToString & ", nežinoma.")

    End Function

    Public Function ConvertEnumHumanReadable(ByVal EnumValue As [Enum]) As String

        If _commonEnumMap Is Nothing Then InitializeCommonEnumMap()

        For Each item As EnumMapItem In _commonEnumMap
            If item.IsMatch(EnumValue) Then Return item.HumanReadableString
        Next

        Throw New Exception("Enumeracijos tipo " & EnumValue.ToString & _
            " pavadinimas ""žmonių kalba"" nežinomas.")

    End Function

    Public Function ConvertEnumDatabaseCodeToHumanReadable(Of T)(ByVal DatabaseCode As Integer) As String

        If _commonEnumMap Is Nothing Then InitializeCommonEnumMap()

        For Each item As EnumMapItem In _commonEnumMap
            If item.IsMatch(GetType(T), DatabaseCode) Then Return item.HumanReadableString
        Next

        Throw New Exception("Enumeracijos tipo " & GetType(T).ToString & _
            " vertė, duomenų bazėje pažymėta kodu " & DatabaseCode.ToString & ", nežinoma.")

    End Function

    Public Function GetEnumValuesHumanReadableList(ByVal EnumType As Type, _
        ByVal AddEmptyString As Boolean, ByVal ParamArray OnlySelectedValues As [Enum]()) As List(Of String)

        Dim result As New List(Of String)

        For Each v As [Enum] In [Enum].GetValues(EnumType)
            If OnlySelectedValues.Length < 1 OrElse Not Array.IndexOf(OnlySelectedValues, v) < 0 Then
                result.Add(ConvertEnumHumanReadable(v))
            End If
        Next

        result.Sort()
        If AddEmptyString Then result.Insert(0, "")

        Return result

    End Function

#End Region

End Module
