Imports System.Globalization
Imports System.Threading
Imports AccControls.PublicFunctions
Imports AccDataAccessLayer.Security
Imports AccDataAccessLayer
Imports NetOffice
Imports Excel = NetOffice.ExcelApi
Imports NetOffice.ExcelApi.Enums
Public Class MDIParent1

    Private m_ChildFormNumber As Integer = 0
    Shared MT As Mutex
    Public InstanceGuid As Guid = Guid.NewGuid


    Private Sub MDIParent1_FormClosing(ByVal sender As Object, _
        ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If Not My.Application.IsInternalShutdown Then _
            If Not YesOrNo("Ar tikrai norite baigti darbą su programa?") _
                Then e.Cancel = True
        MyCustomSettings.Save()
        Try
            MT.WaitOne()
            MT.ReleaseMutex()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub MDIParent1_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        If My.Application.IsInternalShutdown Then Exit Sub

        MT = New Mutex(True, "Apskaita210")

        AddHandler AccDataAccessLayer.CacheManager.BaseTypeCacheIsAdded, _
            AddressOf CachedListsLoaders.CachedListChanged
        Dim WebServiceTimeOut As Integer = 1000 * 60 * 25
        Csla.ApplicationContext.LocalContext.Add("WebServiceTimeOut", WebServiceTimeOut)

        Dim DBlist As DatabaseInfoList = Nothing
        Try
            DBlist = DatabaseInfoList.GetDatabaseList
            DatabasesToMenu()
        Catch ex As Exception
            ShowError(ex)
        End Try

        Me.ToolStrip1.Visible = MyCustomSettings.ShowToolStrip

        LogInPrimaryToGUI()

        My.Application.ChangeCulture("lt-LT")
        My.Application.Culture.CurrentCulture.DateTimeFormat.DateSeparator = "-"
        My.Application.Culture.CurrentCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd"

        If Not DBlist Is Nothing AndAlso Not DBlist.ErrorsString Is Nothing _
            AndAlso Not String.IsNullOrEmpty(DBlist.ErrorsString.Trim) Then _
            MsgBox("ĮSPĖJIMAS. Nepavyko gauti duomenų apie įmones kai kuriose duomenų bazėse. " _
                & "Gali būti kad duomenys jose yra sugadinti." & vbCrLf & vbCrLf & DBlist.ErrorsString.Trim, _
                MsgBoxStyle.Exclamation, "Įspėjimas")

        If MyCustomSettings.AutoUpdate AndAlso My.Computer.Network.IsAvailable Then _
            AccWebCrawler.CheckForAvailableUpdatesAsync(MyCustomSettings.UpdateUrl _
            & LastUpdateFileName, AddressOf OnUpdateDataReceived)

        If OpenCompanyMenuItem.DropDownItems.Count = 1 Then _
            OpenCompanyMenuItem.DropDownItems(0).PerformClick()

    End Sub

#Region "***ADMINISTRAVIMAS***"

    Private Sub NewCompanyMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles NewCompanyMenuItem.Click

        If IsLoggedInDB() Then
            If Not YesOrNo("Ar tikrai norite baigti darbą su " & GetCurrentCompany.Name & " ?") _
                Then Exit Sub
            Try
                Using busy As New StatusBusy
                    AccPrincipal.Login("", New CustomCacheManager)
                    BOMapper.LogOffToGUI()
                End Using
            Catch ex As Exception
                ShowError(ex)
                Exit Sub
            End Try
        End If

        LaunchForm(GetType(F_NewCompany), True, True, 0)

    End Sub

    Private Sub DropCompanyMenuItem_Click(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles DropCompanyMenuItem.Click

        If IsLoggedInDB() Then
            If Not YesOrNo("Ar tikrai norite baigti darbą su " & GetCurrentCompany.Name & " ?") _
                Then Exit Sub
            Try
                Using busy As New StatusBusy
                    AccPrincipal.Login("", New CustomCacheManager)
                    BOMapper.LogOffToGUI()
                End Using
            Catch ex As Exception
                ShowError(ex)
                Exit Sub
            End Try
        End If

        LaunchForm(GetType(F_DeleteDatabase), True, True, 0)

    End Sub

    Private Sub ExitMenuItem_Click(ByVal sender As Object, _
        ByVal e As EventArgs) Handles ExitMenuItem.Click
        My.Application.IsInternalShutdown = True
        Global.System.Windows.Forms.Application.Exit()
    End Sub

    Private Sub ProgramSetupMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ProgramSetupMenuItem.Click
        LaunchForm(GetType(F_SettingsLocal), True, False, 0)
    End Sub

    ''' <summary>
    ''' Handles clicks on company selection menu items and performs login to a database.
    ''' </summary>
    Public Sub Baze(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim Klaida As Boolean = False

        If IsLoggedInDB() Then

            If Not YesOrNo("Ar tikrai norite baigti darbą su " & GetCurrentCompany.Name & " ?") _
                Then Exit Sub

            Try
                Using busy As New StatusBusy
                    AccPrincipal.Login("", New CustomCacheManager)
                    BOMapper.LogOffToGUI()
                End Using
            Catch ex As Exception
                ShowError(ex)
                Exit Sub
            End Try

        End If

        If GetCurrentIdentity.IsLocalUser Then

            Try
                If AccPrincipal.Login(CType(sender.tag, DatabaseInfo).DatabaseName, New CustomCacheManager, "") Then
                    BOMapper.LogInToGUI()
                    Exit Sub
                End If
            Catch ex As Exception
            End Try

            Using frm As New F_LoginSecondary(CType(sender.tag, DatabaseInfo))
                frm.ShowDialog()
                If frm.IsLogonSuccesfull Then BOMapper.LogInToGUI()
            End Using

        Else


            Try
                Using busy As New StatusBusy
                    AccPrincipal.Login(CType(sender.tag, DatabaseInfo).DatabaseName, New CustomCacheManager)
                    BOMapper.LogInToGUI()
                End Using
            Catch ex As Exception
                ShowError(ex)
            End Try

        End If

    End Sub

    Private Sub UserAdministrationMenuItem_Click(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles UserAdministrationMenuItem.Click

        If IsLoggedInDB() Then
            If Not YesOrNo("Ar tikrai norite baigti darbą su " & GetCurrentCompany.Name & " ?") _
                Then Exit Sub
            Try
                Using busy As New StatusBusy
                    AccPrincipal.Login("", New CustomCacheManager)
                    BOMapper.LogOffToGUI()
                End Using
            Catch ex As Exception
                ShowError(ex)
                Exit Sub
            End Try

        End If

        LaunchForm(GetType(F_User), True, True, 0)

    End Sub

    Private Sub LogOffMenuItem_Click(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles LogOffMenuItem.Click

        If Not IsLoggedInDB() Then Exit Sub

        If Not YesOrNo("Ar tikrai norite baigti darbą su " & GetCurrentCompany.Name & " ?") _
                Then Exit Sub

        Try
            Using busy As New StatusBusy
                AccPrincipal.Login("", New CustomCacheManager)
                BOMapper.LogOffToGUI()
            End Using
        Catch ex As Exception
            ShowError(ex)
            Exit Sub
        End Try

    End Sub

    Private Sub BackupMenuItem_Click_1(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles BackupMenuItem.Click
        LaunchForm(GetType(F_BackUp), True, False, 0)
    End Sub

    Private Sub ChangePasswordMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ChangePasswordMenuItem.Click
        LaunchForm(GetType(F_ChangePassword), True, True, 0)
    End Sub

    Private Sub UpgradeDBMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles UpgradeDBMenuItem.Click
        LaunchForm(GetType(F_DatabaseStructureError), True, False, 0)
    End Sub

    Private Sub QueryBrowserMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles QueryBrowserMenuItem.Click
        LaunchForm(GetType(F_RawSqlFetch), False, False, 0)
    End Sub

    Private Sub DatabaseStructureEditor_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles DatabaseStructureEditor_MenuItem.Click
        LaunchForm(GetType(F_DatabaseStructureEditor), False, False, 0)
    End Sub

    Private Sub RoleStructureEditor_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles RoleStructureEditor_MenuItem.Click
        LaunchForm(GetType(F_DatabaseTableAccessRoleList), False, False, 0)
    End Sub

#End Region

#Region "***DUOMENYS***"

    Private Sub AccountsListMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles AccountsListMenuItem.Click
        LaunchForm(GetType(F_Accounts), True, False, 0)
    End Sub

    Private Sub CompanyInfoMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles CompanyInfoMenuItem.Click
        LaunchForm(GetType(F_Company), True, False, 0)
    End Sub

    Private Sub PersonsMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles PersonsMenuItem.Click, PersonsButton.Click
        LaunchForm(GetType(F_Persons), True, False, 0)
    End Sub

    Private Sub NewPerson_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles NewPerson_MenuItem.Click, _
        NewPersonButton.Click
        LaunchForm(GetType(F_Person), False, False, 0)
    End Sub

    Private Sub PersonGroupsMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles PersonGroupsMenuItem.Click
        LaunchForm(GetType(F_PersonGroups), True, False, 0)
    End Sub

    Private Sub ImportedPersonList_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ImportedPersonList_MenuItem.Click
        LaunchForm(GetType(F_ImportedPersonList), True, False, 0)
    End Sub

    Private Sub GeneralLedgerMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles GeneralLedgerMenuItem.Click, GeneralLedgerButton.Click
        LaunchForm(GetType(F_GeneralLedger), False, False, 0)
    End Sub

    Private Sub BookEntriesTurnoverMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles BookEntriesTurnoverMenuItem.Click, BookEntriesTurnoverButton.Click
        LaunchForm(GetType(F_AccountTurnoverInfo), False, False, 0)
    End Sub

    Private Sub NewJournalEntryMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles NewJournalEntryMenuItem.Click, _
        NewJournalEntryToolStripButton.Click
        LaunchForm(GetType(F_GeneralLedgerEntry), False, False, 0)
    End Sub

    Private Sub AccumulativeCosts_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles AccumulativeCosts_MenuItem.Click
        LaunchForm(GetType(F_AccumulativeCosts), False, False, 0)
    End Sub

    Private Sub JournalEntryTemplatesMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles JournalEntryTemplatesMenuItem.Click
        LaunchForm(GetType(F_Templates), True, False, 0)
    End Sub

    Private Sub TransferOfBalanceMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles TransferOfBalanceMenuItem.Click
        LaunchForm(GetType(F_TransferOfBalance), True, False, 0)
    End Sub

    Private Sub ServerSideSettingsMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ServerSideSettingsMenuItem.Click
        LaunchForm(GetType(F_GeneralSettings), True, False, 0)
    End Sub

    Private Sub DocumentSerialList_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles DocumentSerialList_MenuItem.Click
        LaunchForm(GetType(F_DocumentSerialList), True, False, 0)
    End Sub

#End Region

#Region "***DOKUMENTAI***"

    Private Sub CashAccountList_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles CashAccountList_MenuItem.Click
        LaunchForm(GetType(F_CashAccountList), True, False, 0)
    End Sub

    Private Sub BankTransferMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles BankTransferMenuItem.Click
        LaunchForm(GetType(F_BankOperation), False, False, 0)
    End Sub

    Private Sub InvoiceMadeMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles InvoiceMadeMenuItem.Click, MakeInvoiceButton.Click
        LaunchForm(GetType(F_InvoiceMade), False, False, 0)
    End Sub

    Private Sub InvoiceListMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles InvoiceListMenuItem.Click, InvoiceListButton.Click
        LaunchForm(GetType(F_InvoiceInfoList), False, False, 0)
    End Sub

    Private Sub InvoiceReceivedMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles InvoiceReceivedMenuItem.Click, RegisterInvoiceButton.Click
        LaunchForm(GetType(F_InvoiceReceived), False, False, 0)
    End Sub

    Private Sub TillIncomeOrderMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles TillIncomeOrderMenuItem.Click, TillIncomeOrderButton.Click
        LaunchForm(GetType(F_TillIncomeOrder), False, False, 0)
    End Sub

    Private Sub TillSpendingsOrderMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles TillSpendingsOrderMenuItem.Click, TillSpendingsOrderButton.Click
        LaunchForm(GetType(F_TillSpendingsOrder), False, False, 0)
    End Sub

    Private Sub CashOperationInfoListMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles CashOperationInfoListMenuItem.Click, TillButton.Click
        LaunchForm(GetType(F_CashOperationInfoList), False, False, 0)
    End Sub

    Private Sub BankImportMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles BankImportMenuItem.Click
        LaunchForm(GetType(F_BankOperationItemList), False, False, 0)
    End Sub

    Private Sub ServiceListMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ServiceListMenuItem.Click, ServiceInfoListButton.Click
        LaunchForm(GetType(F_ServiceInfoList), False, False, 0)
    End Sub

    Private Sub NewService_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles NewService_MenuItem.Click, NewServiceButton.Click
        LaunchForm(GetType(F_Service), False, False, 0)
    End Sub

    Private Sub AdvanceReportListMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles AdvanceReportListMenuItem.Click
        LaunchForm(GetType(F_AdvanceReportInfoList), False, False, 0)
    End Sub

    Private Sub AdvanceReport_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles AdvanceReport_MenuItem.Click
        LaunchForm(GetType(F_AdvanceReport), False, False, 0)
    End Sub

    Private Sub NewOffset_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles NewOffset_MenuItem.Click
        LaunchForm(GetType(F_Offset), False, False, 0)
    End Sub

#End Region

#Region "***IRANKIAI***"

    Private Sub ConsolidatedReportMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ConsolidatedReportMenuItem.Click
        LaunchForm(GetType(F_FinancialStatementsInfo), False, False, 0)
    End Sub

    Private Sub ConsolidatedReportStructureMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ConsolidatedReportStructureMenuItem.Click
        LaunchForm(GetType(F_ConsolidatedReportsStructure), False, False, 0)
    End Sub

    Private Sub CurrencyRateChangeImpactCalculator_MenuItem_Click_1(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles CurrencyRateChangeImpactCalculator_MenuItem.Click
        LaunchForm(GetType(F_CurrencyRateChangeImpactCalculator), False, False, 0)
    End Sub

    Private Sub DebtsTableMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles DebtsTableMenuItem.Click
        LaunchForm(GetType(F_DebtInfoList), False, False, 0)
    End Sub

    Private Sub UnsettledPersonInfoList_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles UnsettledPersonInfoList_MenuItem.Click
        LaunchForm(GetType(F_UnsettledPersonInfoList), False, False, 0)
    End Sub

    Private Sub ServiceTurnoverInfoList_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ServiceTurnoverInfoList_MenuItem.Click
        LaunchForm(GetType(F_ServiceTurnoverInfoList), False, False, 0)
    End Sub

#End Region

#Region "***HELPAI***"

    Private Sub ExampleAccountListMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ExampleAccountListMenuItem.Click
        LaunchForm(GetType(PVZ_SP), False, False, 0)
    End Sub

    Private Sub AmortizationListMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles AmortizationListMenuItem.Click
        'jei useris nori ziureti PMI nustatytas nusidevejimu normas
        LaunchForm(GetType(Nusidevejimai), False, False, 0)
    End Sub

    Private Sub AboutMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles AboutMenuItem.Click
        'jei useris nori suzinoti apie programa
        LaunchForm(GetType(Apie), False, True, 0)
    End Sub

    Private Sub HelpMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles HelpMenuItem.Click
        Try
            Process.Start("https://apskaita5.codeplex.com/")
        Catch ex As Exception
        End Try
        'Try
        '    Process.Start(AppPath() & "\Apskaita_help.CHM")
        'Catch ex As Exception
        '    ShowError(ex)
        'End Try
    End Sub

    Private Sub CheckIfUpdateAvailable_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles CheckIfUpdateAvailable_MenuItem.Click

        Dim AvailableUpdateDate As Date

        Using frm As New AccWebCrawler.F_LaunchWebCrawler(MyCustomSettings.UpdateUrl _
            & LastUpdateFileName, IO.Path.Combine(AppPath(), LastUpdateFileName), _
            "Ieškoma programos atnaujinimo...", System.Text.Encoding.Unicode, _
            System.Text.Encoding.Unicode, Nothing)

            If frm.ShowDialog <> Windows.Forms.DialogResult.OK Then Exit Sub

            If frm.result Is Nothing OrElse Not TypeOf frm.result Is Date _
                OrElse DirectCast(frm.result, Date) = Date.MinValue Then
                MsgBox("Naujų programos atnaujinimų nėra, t.y. programa naujausios versijos.", _
                    MsgBoxStyle.Information, "")
                Exit Sub
            End If

            AvailableUpdateDate = DirectCast(frm.result, Date)

        End Using

        DownloadAndInstallUpdate(AvailableUpdateDate)

    End Sub

#End Region

#Region "***TURTO MODULIS***"

    Private Sub LTAPurchaseMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles LTAPurchaseMenuItem.Click
        LaunchForm(GetType(F_LongTermAsset), False, False, 0)
    End Sub

    Private Sub LTAListMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles LTAListMenuItem.Click
        LaunchForm(GetType(F_LongTermAssetInfoList), False, False, 0)
    End Sub

    Private Sub LongTermAssetCustomGroupList_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles LongTermAssetCustomGroupList_MenuItem.Click
        LaunchForm(GetType(F_LongTermAssetCustomGroupList), True, False, 0)
    End Sub

    Private Sub LTAMassOperationMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles LTAMassOperationMenuItem.Click
        LaunchForm(GetType(F_LongTermAssetComplexDocumentInfoList), False, False, 0)
    End Sub


    Private Sub NewGoodsItem_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles NewGoodsItem_MenuItem.Click, NewGoodsItemButton.Click
        LaunchForm(GetType(F_GoodsItem), False, False, 0)
    End Sub

    Private Sub ImportedGoodsItemList_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ImportedGoodsItemList_MenuItem.Click
        LaunchForm(GetType(F_ImportedGoodsItemList), True, False, 0)
    End Sub

    Private Sub GoodsTurnoverInfoList_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles GoodsTurnoverInfoList_MenuItem.Click, GoodsTurnoverInfoButton.Click
        LaunchForm(GetType(F_GoodsTurnoverInfoList), False, False, 0)
    End Sub

    Private Sub ProductionCalculationSheetList_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ProductionCalculationSheetList_MenuItem.Click
        LaunchForm(GetType(F_ProductionCalculationItemList), False, False, 0)
    End Sub

    Private Sub GoodsGroupListMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles GoodsGroupListMenuItem.Click
        LaunchForm(GetType(F_GoodsGroupList), True, False, 0)
    End Sub

    Private Sub WarehouseList_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles WarehouseList_MenuItem.Click
        LaunchForm(GetType(F_WarehouseList), True, False, 0)
    End Sub

    Private Sub GoodsOperationInfoList_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles GoodsOperationInfoList_MenuItem.Click
        LaunchForm(GetType(F_GoodsOperationInfoListParent), False, False, 0)
    End Sub

    Private Sub NewGoodsOperation_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles NewGoodsOperation_MenuItem.Click
        LaunchForm(GetType(F_NewGoodsOperationSelector), True, True, 0)
    End Sub

    Private Sub GoodsComplexOperationTransferOfBalance_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles GoodsComplexOperationTransferOfBalance_MenuItem.Click
        LaunchForm(GetType(F_GoodsComplexOperationTransferOfBalance), True, False, 0)
    End Sub

    Private Sub NewProductionCalculation_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles NewProductionCalculation_MenuItem.Click
        LaunchForm(GetType(F_ProductionCalculation), False, False, 0)
    End Sub

#End Region

#Region "***DARBUOTOJŲ MODULIS***"

    Private Sub WorkerStatusMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles WorkerStatusMenuItem.Click
        LaunchForm(GetType(F_LabourContractInfoList), False, False, 0)
    End Sub

    Private Sub WorkersVDUInfo_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles WorkersVDUInfo_MenuItem.Click
        LaunchForm(GetType(F_WorkersVDUInfo), False, False, 0)
    End Sub

    Private Sub WorkerHolidayInfo_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles WorkerHolidayInfo_MenuItem.Click
        LaunchForm(GetType(F_WorkerHolidayInfo), False, False, 0)
    End Sub

    Private Sub HolidayPayReserve_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles HolidayPayReserve_MenuItem.Click
        LaunchForm(GetType(F_HolidayPayReserve), False, False, 0)
    End Sub

    Friend Sub NewWageSheetMenuItem_Click(ByVal sender As System.Object, _
            ByVal e As System.EventArgs) Handles NewWageSheetMenuItem.Click

        Dim cYear, cMonth As Integer
        Using frm As New F_NewSheet("Naujas darbo užmokesčio žiniaraštis")
            frm.ShowDialog()
            If Not frm.Result Then Exit Sub
            cYear = frm.Year
            cMonth = frm.Month
        End Using

        LaunchForm(GetType(F_WageSheet), False, False, 0, cYear, cMonth)

    End Sub

    Private Sub WageSheetInfoListMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles WageSheetInfoListMenuItem.Click
        LaunchForm(GetType(F_WageSheetInfoList), False, False, 0)
    End Sub

    Friend Sub NewImprestSheetMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles NewImprestSheetMenuItem.Click

        Dim cYear, cMonth As Integer
        Using frm As New F_NewSheet("Naujas avanso žiniaraštis")
            frm.ShowDialog()
            If Not frm.Result Then Exit Sub
            cYear = frm.Year
            cMonth = frm.Month
        End Using

        LaunchForm(GetType(F_ImprestSheet), False, False, 0, cYear, cMonth)

    End Sub

    Private Sub ImprestSheetInfoListMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ImprestSheetInfoListMenuItem.Click
        LaunchForm(GetType(F_ImprestSheetInfoList), False, False, 0)
    End Sub

    Private Sub PayOutNaturalPersonListMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles PayOutNaturalPersonListMenuItem.Click
        LaunchForm(GetType(F_PayOutNaturalPersonList), False, False, 0)
    End Sub

    Private Sub DeclarationMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles DeclarationMenuItem.Click
        LaunchForm(GetType(F_Declarations), False, False, 0)
    End Sub

    Private Sub WorkerInfoCardMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles WorkerInfoCardMenuItem.Click
        LaunchForm(GetType(F_WorkerInfoCard), False, False, 0)
    End Sub

    Private Sub WorkTimeClassList_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles WorkTimeClassList_MenuItem.Click
        LaunchForm(GetType(F_WorkTimeClassList), True, False, 0)
    End Sub

    Private Sub WorkTimeSheet_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles WorkTimeSheet_MenuItem.Click
        LaunchForm(GetType(F_WorkTimeSheet), False, False, 0)
    End Sub

    Private Sub WorkTimeSheetInfoList_MenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles WorkTimeSheetInfoList_MenuItem.Click
        LaunchForm(GetType(F_WorkTimeSheetInfoList), False, False, 0)
    End Sub

#End Region

#Region "***TECHNINIAI***"

    ' CHILD FORMS ADMINISTRATION METHODS

    Friend Sub LaunchForm(ByVal FormType As Type, ByVal SingleForm As Boolean, _
        ByVal ShowAsDialog As Boolean, ByVal ObjectId As Long, ByVal ParamArray ParamObjects As Object())

        Dim FormIndex As Integer = 0

        If SingleForm Then
            For Each frm As Form In Me.MdiChildren
                If frm.GetType Is FormType Then
                    frm.BringToFront()
                    frm.Activate()
                    Exit Sub
                End If
            Next
        ElseIf ObjectId > 0 Then
            For Each frm As Form In Me.MdiChildren
                If frm.GetType Is FormType AndAlso TypeOf frm Is IObjectEditForm _
                    AndAlso DirectCast(frm, IObjectEditForm).ObjectID = ObjectId Then
                    frm.BringToFront()
                    frm.Activate()
                    Exit Sub
                End If
            Next
        Else
            For Each frm As Form In Me.MdiChildren
                If frm.GetType Is FormType Then
                    FormIndex += 1
                End If
            Next
        End If

        Dim newForm As Form
        If ParamObjects Is Nothing OrElse ParamObjects.Length < 1 Then
            newForm = DirectCast(Activator.CreateInstance(FormType), Form)
        Else
            newForm = DirectCast(Activator.CreateInstance(FormType, ParamObjects), Form)
        End If

        If ShowAsDialog Then
            newForm.ShowDialog(Me)
        Else
            newForm.MdiParent = Me
            If FormIndex > 0 Then newForm.Text = newForm.Text & " (" & FormIndex.ToString & ")"
            'AddHandler newForm.FormClosed, AddressOf BringMdiChildrenToMenu
            'BringMdiChildrenToMenu(newForm, Nothing)
            newForm.Show()
        End If

    End Sub

    Friend Sub ChangeFormText(ByVal TargetFrm As Form)

        'If TargetFrm.Tag Is Nothing OrElse Not TypeOf TargetFrm.Tag Is Guid Then Exit Sub

        'For i As Integer = WindowsMenuItem.DropDownItems.Count To 1 Step -1
        '    If Not WindowsMenuItem.DropDownItems(i - 1).Tag Is Nothing AndAlso _
        '        TypeOf WindowsMenuItem.DropDownItems(i - 1).Tag Is Guid AndAlso _
        '        DirectCast(WindowsMenuItem.DropDownItems(i - 1).Tag, Guid) _
        '        = DirectCast(TargetFrm.Tag, Guid) Then

        '        WindowsMenuItem.DropDownItems(i - 1).Text = TargetFrm.Text
        '        Exit Sub

        '    End If
        'Next

    End Sub

    'Private Sub BringMdiChildrenToMenu(ByVal sender As Object, ByVal e As FormClosedEventArgs)

    '    For i As Integer = WindowsMenuItem.DropDownItems.Count To 1 Step -1
    '        If Not WindowsMenuItem.DropDownItems(i - 1) Is CascadeToolStripMenuItem AndAlso _
    '            Not WindowsMenuItem.DropDownItems(i - 1) Is TileVerticalToolStripMenuItem AndAlso _
    '            Not WindowsMenuItem.DropDownItems(i - 1) Is TileHorizontalToolStripMenuItem AndAlso _
    '            Not WindowsMenuItem.DropDownItems(i - 1) Is CloseAllToolStripMenuItem Then

    '            RemoveHandler WindowsMenuItem.DropDownItems(i - 1).Click, AddressOf ChildFormMenuItem_Click
    '            WindowsMenuItem.DropDownItems.RemoveAt(i - 1)

    '        End If
    '    Next

    '    If Me.MdiChildren.Length < 1 Then Exit Sub

    '    WindowsMenuItem.DropDownItems.Add(New ToolStripSeparator)

    '    For Each frm As Form In Me.MdiChildren

    '        If e Is Nothing OrElse sender.Text.Trim <> frm.Text.Trim Then

    '            Dim t As ToolStripItem = WindowsMenuItem.DropDownItems.Add(frm.Text, _
    '                Nothing, AddressOf ChildFormMenuItem_Click)

    '            If frm.Tag Is Nothing OrElse Not TypeOf frm.Tag Is Guid Then frm.Tag = Guid.NewGuid

    '            t.Tag = frm.Tag

    '        End If

    '    Next

    '    If Not e Is Nothing Then RemoveHandler DirectCast(sender, Form).FormClosed, _
    '        AddressOf BringMdiChildrenToMenu

    'End Sub

    Private Sub CascadeToolStripMenuItem_Click(ByVal sender As Object, _
            ByVal e As EventArgs) Handles CascadeToolStripMenuItem.Click
        Me.LayoutMdi(MdiLayout.Cascade)
    End Sub

    Private Sub TileVerticleToolStripMenuItem_Click(ByVal sender As Object, _
            ByVal e As EventArgs) Handles TileVerticalToolStripMenuItem.Click
        Me.LayoutMdi(MdiLayout.TileVertical)
    End Sub

    Private Sub TileHorizontalToolStripMenuItem_Click(ByVal sender As Object, _
            ByVal e As EventArgs) Handles TileHorizontalToolStripMenuItem.Click
        Me.LayoutMdi(MdiLayout.TileHorizontal)
    End Sub

    Private Sub ArrangeIconsToolStripMenuItem_Click(ByVal sender As Object, _
            ByVal e As EventArgs)
        Me.LayoutMdi(MdiLayout.ArrangeIcons)
    End Sub

    Private Sub CloseAllToolStripMenuItem_Click(ByVal sender As Object, _
            ByVal e As EventArgs) Handles CloseAllToolStripMenuItem.Click
        ' Close all child forms of the parent.
        For Each ChildForm As Form In Me.MdiChildren
            ChildForm.Close()
        Next
    End Sub

    ' PRINTING INFRASTRUCTURE METHODS

    Private Sub PrintToolStripSplitButton_ButtonClick(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles PrintToolStripSplitButton.ButtonClick
        If Me.ActiveMdiChild Is Nothing OrElse Not TypeOf Me.ActiveMdiChild Is ISupportsPrinting Then Exit Sub
        Dim ActiveChild As ISupportsPrinting = DirectCast(Me.ActiveMdiChild, ISupportsPrinting)
        ActiveChild.OnPrintClick(sender, e)
    End Sub

    Private Sub PreviewToolStripSplitButton_ButtonClick(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles PreviewToolStripSplitButton.ButtonClick
        If Me.ActiveMdiChild Is Nothing OrElse Not TypeOf Me.ActiveMdiChild Is ISupportsPrinting Then Exit Sub
        Dim ActiveChild As ISupportsPrinting = DirectCast(Me.ActiveMdiChild, ISupportsPrinting)
        ActiveChild.OnPrintPreviewClick(sender, e)
    End Sub

    Private Sub EmailToolStripSplitButton_ButtonClick(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles EmailToolStripSplitButton.ButtonClick
        If Me.ActiveMdiChild Is Nothing OrElse Not TypeOf Me.ActiveMdiChild Is ISupportsPrinting Then Exit Sub
        Dim ActiveChild As ISupportsPrinting = DirectCast(Me.ActiveMdiChild, ISupportsPrinting)
        ActiveChild.OnMailClick(sender, e)
    End Sub

    Private Sub MainForm_MdiChildActivate(ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles Me.MdiChildActivate

        If Me.ActiveMdiChild Is Nothing OrElse Not TypeOf Me.ActiveMdiChild Is ISupportsPrinting Then

            PrintToolStripSplitButton.Enabled = False
            PreviewToolStripSplitButton.Enabled = False
            EmailToolStripSplitButton.Enabled = False
            PrintToolStripSplitButton.DropDown = Nothing
            PreviewToolStripSplitButton.DropDown = Nothing
            EmailToolStripSplitButton.DropDown = Nothing

        Else

            Dim ActiveChild As ISupportsPrinting = DirectCast(Me.ActiveMdiChild, ISupportsPrinting)
            PrintToolStripSplitButton.Enabled = True
            PreviewToolStripSplitButton.Enabled = True
            EmailToolStripSplitButton.Enabled = ActiveChild.SupportsEmailing
            PrintToolStripSplitButton.DropDown = ActiveChild.GetPrintDropDownItems
            PreviewToolStripSplitButton.DropDown = ActiveChild.GetPrintPreviewDropDownItems
            EmailToolStripSplitButton.DropDown = ActiveChild.GetMailDropDownItems

        End If

        ExcelButton.Enabled = Not Me.ActiveMdiChild Is Nothing _
            AndAlso FormContainsDataGridView(Me.ActiveMdiChild)

    End Sub


    Private Sub EksportExcelMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles EksportExcelMenuItem.Click, ExcelButton.Click

        If Not Me.MdiChildren.Length > 0 OrElse Me.ActiveMdiChild Is Nothing OrElse _
            Not TypeOf Me.ActiveMdiChild.ActiveControl Is DataGridView OrElse _
            Not DirectCast(Me.ActiveMdiChild.ActiveControl, DataGridView).Rows.Count > 0 Then

            MsgBox("Klaida. Nepasirinkta kopijuojama lentelė arba lentelėje nėra eilučių.", _
                MsgBoxStyle.Exclamation, "Klaida")
            Exit Sub

        End If

        Try
            Using busy As New StatusBusy
                ExportGridToExcel(DirectCast(Me.ActiveMdiChild.ActiveControl, DataGridView))
            End Using
        Catch ex As Exception
            ShowError(New Exception("Eksporto į excelį procesas nutrūko dėl klaidos: " _
                & vbCrLf & ex.Message, ex))
        End Try

    End Sub

    Private Sub MakeGlassToolStripButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MakeGlassToolStripButton.Click
        If MakeGlassToolStripButton.Checked Then
            Me.Opacity = 0.5
        Else
            Me.Opacity = 1
        End If
    End Sub

    Private Sub ShowToolStripMenuItem_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles ShowToolStripMenuItem.Click
        Try
            MyCustomSettings.ShowToolStrip = ShowToolStripMenuItem.Checked
            MyCustomSettings.Save()
        Catch ex As Exception
            ShowError(New Exception("Klaida. Nepavyko išsaugoti programos nustatymų: " & ex.Message, ex))
        End Try
        Me.ToolStrip1.Visible = ShowToolStripMenuItem.Checked
    End Sub

    Private Sub ExportGridToExcel(ByVal grid As DataGridView)

        Dim excelApplication As New Excel.Application()
        excelApplication.DisplayAlerts = False

        ' add a new workbook
        Dim workBook As Excel.Workbook = excelApplication.Workbooks.Add()
        Dim workSheet As Excel.Worksheet = DirectCast(workBook.Worksheets(1), Excel.Worksheet)

        '  the given thread culture in all latebinding calls are stored in NetOffice.Settings.
        '  you can change the culture. default is en-us.
        Dim cultureInfo As CultureInfo = NetOffice.Settings.ThreadCulture

        Dim i, j, colCounter As Integer

        workSheet.Cells.Font.Name = "Times New Roman"
        workSheet.Cells.Font.Size = 10
        workSheet.Cells.VerticalAlignment = XlVAlign.xlVAlignTop

        colCounter = 1
        For j = 1 To grid.Columns.Count
            If grid.Columns(j - 1).Visible Then
                workSheet.Cells.Item(1, colCounter).Value = grid.Columns(j - 1).HeaderText
                colCounter += 1
            End If
        Next
        workSheet.Rows.Item(1).Font.Bold = True
        workSheet.Rows.Item(1).WrapText = True
        workSheet.Rows.Item(1).VerticalAlignment = XlVAlign.xlVAlignBottom
        workSheet.Rows.Item(1).HorizontalAlignment = XlHAlign.xlHAlignCenter

        colCounter = 1
        For j = 1 To grid.Columns.Count

            If grid.Columns(j - 1).Visible Then

                For i = 1 To grid.Rows.Count

                    If grid.Item(j - 1, i - 1).Value Is Nothing Then

                        workSheet.Cells.Item(i + 1, colCounter).Value = ""

                    ElseIf Not grid.Item(j - 1, i - 1).Value.GetType.IsValueType _
                        AndAlso Not TypeOf grid.Item(j - 1, i - 1).Value Is String Then

                        workSheet.Cells.Item(i + 1, colCounter).Value = grid.Item(j - 1, i - 1).Value.ToString

                    Else

                        workSheet.Cells.Item(i + 1, colCounter).Value = grid.Item(j - 1, i - 1).Value

                    End If

                Next

                colCounter += 1

            End If

        Next

        colCounter = 1
        Dim t As Type
        For j = 1 To grid.Columns.Count

            If grid.Columns(j - 1).Visible Then

                t = grid.Columns(j - 1).ValueType
                If t Is Nothing Then t = GetType(String)

                If t Is GetType(DateTime) Then
                    workSheet.Columns.Item(colCounter).NumberFormat = _
                        Settings.ThreadCulture.DateTimeFormat.ShortDatePattern

                ElseIf t Is GetType(Integer) OrElse t Is GetType(Double) OrElse _
                    t Is GetType(Decimal) OrElse t Is GetType(Single) _
                    OrElse t Is GetType(UInt16) OrElse t Is GetType(UInt32) _
                    OrElse t Is GetType(UInt64) OrElse t Is GetType(UInteger) _
                    OrElse t Is GetType(Byte) OrElse t Is GetType(Int16) _
                    OrElse t Is GetType(Int32) OrElse t Is GetType(Int64) _
                    OrElse t Is GetType(System.SByte) Then

                    If grid.Columns(j - 1).DefaultCellStyle.Format Is Nothing OrElse _
                        (Not grid.Columns(j - 1).DefaultCellStyle.Format.Contains(".") AndAlso _
                        Not grid.Columns(j - 1).DefaultCellStyle.Format.Contains(",")) Then

                        workSheet.Columns.Item(colCounter).NumberFormat = "#"

                    Else

                        Dim f As String = ""
                        If grid.Columns(j - 1).DefaultCellStyle.Format.Contains(",") Then
                            f = "#" & cultureInfo.NumberFormat.CurrencyGroupSeparator & "##0"
                        Else
                            f = "#0"
                        End If
                        If grid.Columns(j - 1).DefaultCellStyle.Format.Contains(".") Then
                            f = f & cultureInfo.NumberFormat.CurrencyDecimalSeparator & _
                                String.Empty.PadRight(grid.Columns(j - 1).DefaultCellStyle.Format.Trim. _
                                Length - grid.Columns(j - 1).DefaultCellStyle.Format.Trim.LastIndexOf(".") - 1, "0"c)
                        End If
                        workSheet.Columns.Item(colCounter).NumberFormat = f

                    End If

                Else
                    workSheet.Columns.Item(colCounter).NumberFormat = "@"

                End If

                If Not t.IsValueType Then
                    workSheet.Columns.Item(colCounter).WrapText = True
                    workSheet.Columns.Item(colCounter).ColumnWidth = 45
                Else
                    workSheet.Columns(colCounter).AutoFit()
                End If

                colCounter += 1

            End If

        Next

        For i = 1 To grid.Rows.Count + 1
            workSheet.Rows.Item(i).AutoFit()
        Next

        excelApplication.WindowState = XlWindowState.xlMaximized
        excelApplication.Visible = True

    End Sub

    Private Function FormContainsDataGridView(ByVal frm As Control) As Boolean
        If TypeOf frm Is DataGridView Then Return True
        For Each c As Control In frm.Controls
            If FormContainsDataGridView(c) Then Return True
        Next
        Return False
    End Function

    Private Sub OnUpdateDataReceived(ByVal sender As Object, _
        ByVal e As System.Net.DownloadDataCompletedEventArgs)

        Dim AvailableUpdateDate As Date = AccWebCrawler.ResolveAvailableUpdatesResult( _
            MyCustomSettings.UpdateUrl & LastUpdateFileName, IO.Path.Combine(AppPath(), LastUpdateFileName), _
            System.Text.Encoding.Unicode, System.Text.Encoding.Unicode, Nothing, e, False)
        If AvailableUpdateDate <> Date.MinValue Then DownloadAndInstallUpdate(AvailableUpdateDate)

    End Sub

    Private Sub DownloadAndInstallUpdate(ByVal AvailableUpdateDate As Date)

        If Not YesOrNo("Rastas programos atnaujinimas, paskelbtas inete " _
            & AvailableUpdateDate.ToShortDateString & "." & vbCrLf _
            & "Atnaujinti programą?") Then Exit Sub

        Dim FileToDownload As String = MyCustomSettings.UpdateFileName
        If MyCustomSettings.IsPortableInstalation Then FileToDownload = MyCustomSettings.UpdateFileNamePortable

        Using frm As New AccWebCrawler.F_LaunchWebCrawler(MyCustomSettings.UpdateUrl _
            & FileToDownload, IO.Path.Combine(System.IO.Path.GetTempPath, FileToDownload), _
            "Parsisiunčiamas atnaujinimas...")
            If frm.ShowDialog <> Windows.Forms.DialogResult.OK Then Exit Sub
        End Using

        Dim ShellProcess As New Process
        ShellProcess.StartInfo.FileName = IO.Path.Combine(System.IO.Path.GetTempPath, FileToDownload)
        If MyCustomSettings.IsPortableInstalation Then ShellProcess.StartInfo.Arguments _
            = "-o""" & AppPath() & """"
        ShellProcess.StartInfo.UseShellExecute = True
        ShellProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal
        My.Application.IsInternalShutdown = True
        ShellProcess.Start()
        Global.System.Windows.Forms.Application.Exit()

    End Sub

#End Region

End Class
