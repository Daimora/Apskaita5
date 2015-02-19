Namespace ActiveReports

    <Serializable()> _
Public Class Declaration
        Inherits ReadOnlyBase(Of Declaration)

#Region " Business Methods "

        Private _ID As Guid = Guid.NewGuid

        ' manual (de)serializacion of dataset (to string) is used because 
        ' current dataset serialization by MS does not work with web services (at least)
        <NonSerialized()> _
        Private _DeclarationDataSet As DataSet = Nothing
        Private _DeclarationString As String

        Private _DeclarationDateCulture As System.Globalization.DateTimeFormatInfo
        Private _NumberGroupSeparator As String
        Private _NumberDecimalSeparator As String

        Private _DeclarationType As DeclarationType
        Private _Warning As String = ""


        Public ReadOnly Property ID() As Guid
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _ID
            End Get
        End Property

        Public ReadOnly Property DeclarationDataSet() As DataSet
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                If _DeclarationDataSet Is Nothing AndAlso (_DeclarationString Is Nothing _
                    OrElse String.IsNullOrEmpty(_DeclarationString.Trim)) Then Return Nothing

                If _DeclarationDataSet Is Nothing Then

                    Dim SR As IO.StringReader = New IO.StringReader(_DeclarationString)
                    _DeclarationDataSet = New DataSet("DeclarationDataSet")
                    _DeclarationDataSet.ReadXml(SR)
                    SR.Dispose()

                    For Each DT As DataTable In _DeclarationDataSet.Tables
                        If DT.Rows.Count = 1 AndAlso Not DT.Rows(0).Item(0) Is Nothing _
                            AndAlso DT.Rows(0).Item(0).ToString.Trim = "%#DELETE#" Then DT.Rows.RemoveAt(0)
                    Next

                End If

                Return _DeclarationDataSet
            End Get
        End Property

        Public ReadOnly Property DeclarationType() As DeclarationType
            <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
            Get
                Return _DeclarationType
            End Get
        End Property

        Public ReadOnly Property Warning() As String
            Get
                Return _Warning
            End Get
        End Property


        Public Sub SaveToFFData(ByVal FileName As String, ByVal PreparatorName As String)

            If _DeclarationDataSet Is Nothing AndAlso (_DeclarationString Is Nothing _
                OrElse String.IsNullOrEmpty(_DeclarationString.Trim)) Then _
                Throw New Exception("Klaida. Nėra suformuotos deklaracijos.")

            ' clear temporary file if present
            Dim TempFileName As String = AppPath() & FILENAMEFFDATATEMP
            If IO.File.Exists(TempFileName) Then IO.File.Delete(TempFileName)

            ' Set culture params that were used when parsing declaration's
            ' numbers and dates to string
            Dim oldCulture As Globalization.CultureInfo = _
                DirectCast(System.Threading.Thread.CurrentThread.CurrentCulture.Clone, Globalization.CultureInfo)

            Dim d As Double = 4569157.2

            System.Threading.Thread.CurrentThread.CurrentCulture = _
                New Globalization.CultureInfo("lt-LT", False)
            System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat = _DeclarationDateCulture
            System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = _
                _NumberDecimalSeparator
            System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator = _
                _NumberGroupSeparator

            Dim FFDataFormatDataSet As DataSet = Nothing
            If _DeclarationType = ApskaitaObjects.DeclarationType.SAM_1 Then
                FFDataFormatDataSet = GetFFDataForSAM_1(PreparatorName)
            ElseIf _DeclarationType = ApskaitaObjects.DeclarationType.SAM_Aut_1 Then
                FFDataFormatDataSet = GetFFDataForSAM_Aut_1(PreparatorName)
            ElseIf _DeclarationType = ApskaitaObjects.DeclarationType.FR0572 Then
                FFDataFormatDataSet = GetFFDataForFR0572_2(PreparatorName)
            ElseIf _DeclarationType = ApskaitaObjects.DeclarationType.FR0572_3 Then
                FFDataFormatDataSet = GetFFDataForFR0572_3(PreparatorName)
            ElseIf _DeclarationType = ApskaitaObjects.DeclarationType.FR0572_4 Then
                FFDataFormatDataSet = GetFFDataForFR0572_4(PreparatorName)
            ElseIf _DeclarationType = ApskaitaObjects.DeclarationType.FR0573_3 OrElse _
                _DeclarationType = ApskaitaObjects.DeclarationType.FR0573 OrElse _
                _DeclarationType = ApskaitaObjects.DeclarationType.FR0573_4 Then
                FFDataFormatDataSet = GetFFDataForFR0573_3(PreparatorName)
            ElseIf _DeclarationType = ApskaitaObjects.DeclarationType.SD13_1 OrElse _
                _DeclarationType = ApskaitaObjects.DeclarationType.SD13_2 OrElse _
                _DeclarationType = ApskaitaObjects.DeclarationType.SD13_5 Then
                FFDataFormatDataSet = GetFFDataForSD13(PreparatorName)
            ElseIf _DeclarationType = ApskaitaObjects.DeclarationType.SAM_2 OrElse _
                _DeclarationType = ApskaitaObjects.DeclarationType.SAM_3 OrElse _
                _DeclarationType = ApskaitaObjects.DeclarationType.SAM_4 Then
                FFDataFormatDataSet = GetFFDataForSAM_2(PreparatorName)
            Else
                Throw New NotImplementedException("Klaida. Deklaracijos tipo '" & _
                    _DeclarationType.ToString & "' eksportas į ffdata formatą nepalaikomas.")
            End If

            If FFDataFormatDataSet Is Nothing Then _
                Throw New Exception("Klaida. Dėl nežinomų priežasčių nepavyko " & _
                    "konvertuoti deklaracijos duomenų į ffdata formatą.")

            Using FFDataFileStream As IO.FileStream = New IO.FileStream(FileName, IO.FileMode.Create)
                FFDataFormatDataSet.WriteXml(FFDataFileStream)
                FFDataFileStream.Close()
            End Using

            FFDataFormatDataSet.Dispose()

            System.Threading.Thread.CurrentThread.CurrentCulture = oldCulture

        End Sub

        Private Function GetFFDataForSAM_1(ByVal PreparatorName As String) As DataSet

            Dim i As Integer
            Dim DDS As DataSet = DeclarationDataSet

            Dim CurrentUser As AccDataAccessLayer.Security.AccIdentity = GetCurrentIdentity()

            ' Add 3SD appendixes to the ffdata xml structure if needed
            ' and copy form structure to the temp file
            If CInt(DDS.Tables("Specific").Rows(0).Item(5)) > 1 Then
                Dim myDoc As New Xml.XmlDocument
                myDoc.Load(AppPath() & FILENAMEFFDATASAM01)
                For i = 1 To CInt(DDS.Tables("Specific").Rows(0).Item(5)) - 1
                    Dim AddSD As Xml.XmlElement = DirectCast(myDoc.ChildNodes(1).ChildNodes(0). _
                        ChildNodes(1).ChildNodes(2).Clone, Xml.XmlElement)
                    AddSD.Attributes(1).Value = (i + 3).ToString
                    myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(1).AppendChild(AddSD)
                    Dim AddPg As Xml.XmlElement = DirectCast(myDoc.ChildNodes(1).ChildNodes(0). _
                        ChildNodes(0).ChildNodes(0).ChildNodes(2).Clone, Xml.XmlElement)
                    myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(0).ChildNodes(0).AppendChild(AddPg)
                Next
                myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(1).Attributes(0).Value = _
                    (2 + CInt(DDS.Tables("Specific").Rows(0).Item(5))).ToString
                myDoc.Save(AppPath() & FILENAMEFFDATATEMP)
            Else
                IO.File.Copy(AppPath() & FILENAMEFFDATASAM01, AppPath() & FILENAMEFFDATATEMP)
            End If

            ' read ffdata xml structure to dataset
            Dim FormDataSet As New DataSet
            Using FormFileStream As IO.FileStream = New IO.FileStream( _
                AppPath() & FILENAMEFFDATATEMP, IO.FileMode.Open)
                FormDataSet.ReadXml(FormFileStream)
                FormFileStream.Close()
            End Using

            FormDataSet.Tables(0).Rows(0).Item(3) = CurrentUser.Name
            FormDataSet.Tables(0).Rows(0).Item(4) = GetDateInFFDataFormat(Today)
            FormDataSet.Tables(1).Rows(0).Item(2) = AppPath() & FILENAMEMXFDSAM01

            Dim SD3F As Boolean = False
            Dim SpecificDataRow As DataRow = DDS.Tables("Specific").Rows(0)
            For i = 1 To FormDataSet.Tables(8).Rows.Count
                If FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "FormCode" Then _
                    SD3F = (FormDataSet.Tables(8).Rows(i - 1).Item(1).ToString = "SAM3SD")
                If FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "InsurerName" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString( _
                        DDS.Tables("General").Rows(0).Item(0).ToString, 68).ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "InsurerCode" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = DDS.Tables("General").Rows(0).Item(3)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "PreparatorDetails" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString( _
                        PreparatorName, 68).ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "InsurerPhone" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString(DDS.Tables("General").Rows(0).Item(8).ToString, 15)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "JuridicalPersonCode" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = DDS.Tables("General").Rows(0).Item(1)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "InsurerAddress" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString( _
                        DDS.Tables("General").Rows(0).Item(2).ToString, 68).ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "RecipientDepName" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(1).ToString.ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "DocDate" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetDateInFFDataFormat(CDate(SpecificDataRow.Item(0)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "TaxRateRep" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat(CDbl(SpecificDataRow.Item(4)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "CycleYear" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(2)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "CycleQuarter" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(3)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "Appendixes1" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = 1
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "Apdx1PageCount" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = 1
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "Appendixes2" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = 1
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "Apdx2PageCount" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(5)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "PageTotal" And SD3F Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(5)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "Apdx2PersonCount" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(7)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "Apdx2InsIncomeSum" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat(CDbl(SpecificDataRow.Item(8)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "Apdx2PaymentSum" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat(CDbl(SpecificDataRow.Item(9)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "ManagerJobPosition" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = "DIREKTORIUS"
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "ManagerFullName" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString( _
                        DDS.Tables("General").Rows(0).Item(9).ToString, 68).ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "PersonCountQuarterStart" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(10)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "PersonCountStarted" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(11)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "PersonCountEnded" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(12)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "PersonCountQuarterEnd" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(13)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "IncomeSum" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(14)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "ChargedDebtYearStart" Then
                    If String.IsNullOrEmpty(SpecificDataRow.Item(15).ToString.Trim) Then
                        FormDataSet.Tables(8).Rows(i - 1).Item(1) = ""
                    Else
                        FormDataSet.Tables(8).Rows(i - 1).Item(1) = _
                            GetNumberInFFDataFormat(CDbl(SpecificDataRow.Item(15)))
                    End If
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "ChargedDebtQuarterEnd" Then
                    If String.IsNullOrEmpty(SpecificDataRow.Item(16).ToString.Trim) Then
                        FormDataSet.Tables(8).Rows(i - 1).Item(1) = ""
                    Else
                        FormDataSet.Tables(8).Rows(i - 1).Item(1) = _
                            GetNumberInFFDataFormat(CDbl(SpecificDataRow.Item(16)))
                    End If
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "ChargedMonthly" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(17)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "ChargedMonth1" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(18)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "ChargedMonth2" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(19)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "ChargedMonth3" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(20)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "ChargedYearTotal" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(21)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "ChargedTotal" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(22)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "TransferQuarterEnd" Then
                    If String.IsNullOrEmpty(SpecificDataRow.Item(23).ToString.Trim) Then
                        FormDataSet.Tables(8).Rows(i - 1).Item(1) = ""
                    Else
                        FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                            CDbl(SpecificDataRow.Item(23)))
                    End If
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "TransferMonthly" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(24)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "TransferMonth1" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(25)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "TransferMonth2" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(26)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "TransferMonth3" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(27)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "TransferYearTotal" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(28)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "DebtQuarterEnd" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(29)))
                End If
            Next

            Dim j, p As Integer
            Dim PageIncome As Double = 0
            Dim PagePayments As Double = 0
            Dim DetailsDataTable As DataTable = DDS.Tables("Details")
            For i = 1 To CInt(SpecificDataRow.Item(5))
                p = 9 * (i - 1)
                For j = 1 To Math.Min(9, CInt(SpecificDataRow.Item(7)) - p)
                    If i = 1 Then
                        FormDataSet.Tables(8).Rows(87 + 7 * (j - 1)).Item(1) = j
                        FormDataSet.Tables(8).Rows(88 + 7 * (j - 1)).Item(1) = _
                            DetailsDataTable.Rows(p + j - 1).Item(2)
                        FormDataSet.Tables(8).Rows(89 + 7 * (j - 1)).Item(1) = _
                            DetailsDataTable.Rows(p + j - 1).Item(3).ToString.Trim.Substring(0, 2)
                        FormDataSet.Tables(8).Rows(90 + 7 * (j - 1)).Item(1) = _
                            GetNumericPart(DetailsDataTable.Rows(p + j - 1).Item(3).ToString.Trim)
                        FormDataSet.Tables(8).Rows(91 + 7 * (j - 1)).Item(1) = _
                            GetNumberInFFDataFormat(CDbl(DetailsDataTable.Rows(p + j - 1).Item(4)))
                        FormDataSet.Tables(8).Rows(92 + 7 * (j - 1)).Item(1) = _
                            GetNumberInFFDataFormat(CDbl(DetailsDataTable.Rows(p + j - 1).Item(5)))
                        FormDataSet.Tables(8).Rows(93 + 7 * (j - 1)).Item(1) = _
                            DetailsDataTable.Rows(p + j - 1).Item(1).ToString.ToUpper
                    Else
                        FormDataSet.Tables(8).Rows(87 + 7 * (j - 1) + (i - 1) * 72).Item(1) = j
                        FormDataSet.Tables(8).Rows(88 + 7 * (j - 1) + (i - 1) * 72).Item(1) = _
                            DetailsDataTable.Rows(p + j - 1).Item(2)
                        FormDataSet.Tables(8).Rows(89 + 7 * (j - 1) + (i - 1) * 72).Item(1) = _
                            DetailsDataTable.Rows(p + j - 1).Item(3).ToString.Trim.Substring(0, 2)
                        FormDataSet.Tables(8).Rows(90 + 7 * (j - 1) + (i - 1) * 72).Item(1) = _
                            GetNumericPart(DetailsDataTable.Rows(p + j - 1).Item(3).ToString.Trim)
                        FormDataSet.Tables(8).Rows(91 + 7 * (j - 1) + (i - 1) * 72).Item(1) = _
                            GetNumberInFFDataFormat(CDbl(DetailsDataTable.Rows(p + j - 1).Item(4)))
                        FormDataSet.Tables(8).Rows(92 + 7 * (j - 1) + (i - 1) * 72).Item(1) = _
                            GetNumberInFFDataFormat(CDbl(DetailsDataTable.Rows(p + j - 1).Item(5)))
                        FormDataSet.Tables(8).Rows(93 + 7 * (j - 1) + (i - 1) * 72).Item(1) = _
                            DetailsDataTable.Rows(p + j - 1).Item(1).ToString.ToUpper
                    End If
                    PageIncome = PageIncome + CDbl(DetailsDataTable.Rows(p + j - 1).Item(4))
                    PagePayments = PagePayments + CDbl(DetailsDataTable.Rows(p + j - 1).Item(5))
                Next
                If i = 1 Then
                    FormDataSet.Tables(8).Rows(150).Item(1) = GetNumberInFFDataFormat(PageIncome)
                    FormDataSet.Tables(8).Rows(151).Item(1) = GetNumberInFFDataFormat(PagePayments)
                Else
                    FormDataSet.Tables(8).Rows(150 + (i - 1) * 72).Item(1) = GetNumberInFFDataFormat(PageIncome)
                    FormDataSet.Tables(8).Rows(151 + (i - 1) * 72).Item(1) = GetNumberInFFDataFormat(PagePayments)
                End If
                PageIncome = 0
                PagePayments = 0
            Next

            Return FormDataSet

        End Function

        Private Function GetFFDataForSAM_Aut_1(ByVal PreparatorName As String) As DataSet

            Dim i As Integer
            Dim DDS As DataSet = DeclarationDataSet

            Dim CurrentUser As AccDataAccessLayer.Security.AccIdentity = GetCurrentIdentity()

            ' delete 3SD appendix
            Dim myDoc As New Xml.XmlDocument
            myDoc.Load(AppPath() & FILENAMEFFDATASAM01)
            myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(1).RemoveChild( _
                myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(1).ChildNodes(2))
            myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(0).ChildNodes(0).RemoveChild( _
                myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(0).ChildNodes(0).ChildNodes(2))
            myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(1).Attributes(0).Value = "2"
            myDoc.Save(AppPath() & FILENAMEFFDATATEMP)

            ' read ffdata xml structure to dataset
            Dim FormDataSet As New DataSet
            Using FormFileStream As IO.FileStream = New IO.FileStream( _
                AppPath() & FILENAMEFFDATATEMP, IO.FileMode.Open)
                FormDataSet.ReadXml(FormFileStream)
                FormFileStream.Close()
            End Using

            FormDataSet.Tables(0).Rows(0).Item(3) = CurrentUser.Name
            FormDataSet.Tables(0).Rows(0).Item(4) = GetDateInFFDataFormat(Today)
            FormDataSet.Tables(1).Rows(0).Item(2) = AppPath() & FILENAMEMXFDSAM01

            Dim SpecificDataRow As DataRow = DDS.Tables("Specific").Rows(0)
            For i = 1 To FormDataSet.Tables(8).Rows.Count
                If FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "InsurerName" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString( _
                        DDS.Tables("General").Rows(0).Item(0).ToString, 68).ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "InsurerCode" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = DDS.Tables("General").Rows(0).Item(3)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "PreparatorDetails" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString( _
                        PreparatorName, 68).ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "InsurerPhone" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString(DDS.Tables("General").Rows(0).Item(8).ToString, 15)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "JuridicalPersonCode" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = DDS.Tables("General").Rows(0).Item(1)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "InsurerAddress" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString( _
                        DDS.Tables("General").Rows(0).Item(2).ToString, 68).ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "RecipientDepName" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(1).ToString.ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "DocDate" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetDateInFFDataFormat(CDate(SpecificDataRow.Item(0)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "TaxRateRep" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat(CDbl(SpecificDataRow.Item(4)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "CycleYear" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(2)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "CycleQuarter" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(3)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "Appendixes1" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = 1
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "Apdx1PageCount" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = 1
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "Appendixes2" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = 0
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "Apdx2PageCount" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = 0
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "PageTotal" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = 1
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "ManagerJobPosition" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = "DIREKTORIUS"
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "ManagerFullName" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString( _
                        DDS.Tables("General").Rows(0).Item(9).ToString, 68).ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "IncomeSum" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(14)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "ChargedDebtYearStart" Then
                    If String.IsNullOrEmpty(SpecificDataRow.Item(15).ToString.Trim) Then
                        FormDataSet.Tables(8).Rows(i - 1).Item(1) = ""
                    Else
                        FormDataSet.Tables(8).Rows(i - 1).Item(1) = _
                            GetNumberInFFDataFormat(CDbl(SpecificDataRow.Item(15)))
                    End If
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "ChargedDebtQuarterEnd" Then
                    If String.IsNullOrEmpty(SpecificDataRow.Item(16).ToString.Trim) Then
                        FormDataSet.Tables(8).Rows(i - 1).Item(1) = ""
                    Else
                        FormDataSet.Tables(8).Rows(i - 1).Item(1) = _
                            GetNumberInFFDataFormat(CDbl(SpecificDataRow.Item(16)))
                    End If
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "ChargedMonthly" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(17)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "ChargedMonth1" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(18)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "ChargedMonth2" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(19)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "ChargedMonth3" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(20)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "ChargedYearTotal" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(21)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "ChargedTotal" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(22)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "TransferQuarterEnd" Then
                    If String.IsNullOrEmpty(SpecificDataRow.Item(23).ToString.Trim) Then
                        FormDataSet.Tables(8).Rows(i - 1).Item(1) = ""
                    Else
                        FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                            CDbl(SpecificDataRow.Item(23)))
                    End If
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "TransferMonthly" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(24)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "TransferMonth1" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(25)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "TransferMonth2" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(26)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "TransferMonth3" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(27)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "TransferYearTotal" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(28)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "DebtQuarterEnd" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(29)))
                End If
            Next

            Return FormDataSet

        End Function

        Private Function GetFFDataForFR0572_2(ByVal PreparatorName As String) As DataSet

            Dim DDS As DataSet = DeclarationDataSet
            Dim CurrentUser As AccDataAccessLayer.Security.AccIdentity = GetCurrentIdentity()

            ' read ffdata xml structure to dataset
            Dim FormDataSet As New DataSet
            Using FormFileStream As IO.FileStream = New IO.FileStream( _
                AppPath() & FILENAMEFFDATAFR0572_2, IO.FileMode.Open)
                FormDataSet.ReadXml(FormFileStream)
                FormFileStream.Close()
            End Using

            FormDataSet.Tables(0).Rows(0).Item(2) = CurrentUser.Name
            FormDataSet.Tables(0).Rows(0).Item(3) = GetDateInFFDataFormat(Today)
            FormDataSet.Tables(1).Rows(0).Item(2) = AppPath() & FILENAMEMXFDFR0572_2

            Dim SpecificDataRow As DataRow = DDS.Tables("Specific").Rows(0)
            For i As Integer = 1 To FormDataSet.Tables(8).Rows.Count
                If FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_MM_ID" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = DDS.Tables("General").Rows(0).Item(1)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_MM_Pavad" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString( _
                        DDS.Tables("General").Rows(0).Item(0).ToString, 45).ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_MM_Adresas" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString( _
                        DDS.Tables("General").Rows(0).Item(2).ToString, 45).ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_MM_Tel" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString( _
                        DDS.Tables("General").Rows(0).Item(8).ToString, 12)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_MM_Epastas" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString( _
                        DDS.Tables("General").Rows(0).Item(7).ToString, 35).ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_UzpildData" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetDateInFFDataFormat( _
                        CDate(SpecificDataRow.Item(1)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_ML_Metai" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(2).ToString
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_ML_Menuo" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(3).ToString
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_MM_SavKodas" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(0).ToString
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E11" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(4).ToString
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E18" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        Convert.ToDouble(SpecificDataRow.Item(5)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E19" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        Convert.ToDouble(SpecificDataRow.Item(6)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E20" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        Convert.ToDouble(SpecificDataRow.Item(7)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E21" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        Convert.ToDouble(SpecificDataRow.Item(8)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E22" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        Convert.ToDouble(SpecificDataRow.Item(9)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E23" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        Convert.ToDouble(SpecificDataRow.Item(10)))
                End If
            Next

            Return FormDataSet

        End Function

        Private Function GetFFDataForFR0572_3(ByVal PreparatorName As String) As DataSet

            Dim DDS As DataSet = DeclarationDataSet
            Dim i As Integer
            Dim CurrentUser As AccDataAccessLayer.Security.AccIdentity = GetCurrentIdentity()

            If DDS.Tables("Details").Rows.Count < 1 Then
                Dim myDoc As New Xml.XmlDocument
                myDoc.Load(AppPath() & FILENAMEFFDATAFR0572_3)
                myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(0).ChildNodes(0).RemoveChild( _
                    myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(0).ChildNodes(0).ChildNodes(1))
                myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(1).RemoveChild( _
                    myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(1).ChildNodes(1))
                myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(1).Attributes(0).Value = "1"
                myDoc.Save(AppPath() & FILENAMEFFDATATEMP)

            ElseIf DDS.Tables("Details").Rows.Count > 4 Then
                Dim myDoc As New Xml.XmlDocument
                myDoc.Load(AppPath() & FILENAMEFFDATAFR0572_3)

                For i = 1 To Convert.ToInt32(Math.Ceiling(DDS.Tables("Details").Rows.Count / 4) - 1)
                    Dim AddPg1 As Xml.XmlElement = DirectCast(myDoc.ChildNodes(1).ChildNodes(0). _
                        ChildNodes(0).ChildNodes(0).ChildNodes(1).Clone, Xml.XmlElement)
                    myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(0).ChildNodes(0).AppendChild(AddPg1)
                    Dim AddPg2 As Xml.XmlElement = DirectCast(myDoc.ChildNodes(1).ChildNodes(0). _
                        ChildNodes(1).ChildNodes(1).Clone, Xml.XmlElement)
                    AddPg2.Attributes(1).Value = (i + 2).ToString
                    myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(1).AppendChild(AddPg2)
                Next
                myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(1).Attributes(0).Value = _
                    Convert.ToInt32(Math.Ceiling(DDS.Tables("Details").Rows.Count / 4) + 1).ToString
                myDoc.Save(AppPath() & FILENAMEFFDATATEMP)

            Else
                IO.File.Copy(AppPath() & FILENAMEFFDATAFR0572_3, AppPath() & FILENAMEFFDATATEMP)
            End If

            ' read ffdata xml structure to dataset
            Dim FormDataSet As New DataSet
            Using FormFileStream As IO.FileStream = New IO.FileStream( _
                AppPath() & FILENAMEFFDATATEMP, IO.FileMode.Open)
                FormDataSet.ReadXml(FormFileStream)
                FormFileStream.Close()
            End Using

            FormDataSet.Tables(0).Rows(0).Item(3) = CurrentUser.Name
            FormDataSet.Tables(0).Rows(0).Item(4) = GetDateInFFDataFormat(Today)
            FormDataSet.Tables(1).Rows(0).Item(2) = AppPath() & FILENAMEMXFDFR0572_3

            Dim SpecificDataRow As DataRow = DDS.Tables("Specific").Rows(0)

            For i = 1 To FormDataSet.Tables(8).Rows.Count
                If FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_MM_ID" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = DDS.Tables("General").Rows(0).Item(1)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_MM_Pavad" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString(DDS.Tables("General").Rows(0).Item(0).ToString, 45).ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_MM_Adresas" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString(DDS.Tables("General").Rows(0).Item(2).ToString, 45).ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_MM_Tel" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString(DDS.Tables("General").Rows(0).Item(8).ToString, 12)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_MM_Epastas" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString(DDS.Tables("General").Rows(0).Item(7).ToString, 35).ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_UzpildData" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetDateInFFDataFormat( _
                        CDate(SpecificDataRow.Item(1)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_ML_Metai" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(2).ToString
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_ML_Menuo" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(3)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_MM_SavKodas" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(0).ToString
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E11" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(4).ToString
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E12" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(5).ToString
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E13" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(6).ToString
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E18" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(7)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E19" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(8)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E20" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(9)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E21" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(10)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E22" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(11)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E23" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(12)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E24" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(13)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E25" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(14)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E26" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(15)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E27" _
                    AndAlso CInt(SpecificDataRow.Item(2)) = 2009 Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(16)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E28" _
                    AndAlso CInt(SpecificDataRow.Item(2)) = 2009 Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(17)))
                End If
            Next

            Dim DetailsDataTable As DataTable = DDS.Tables("Details")
            Dim PageIncome, PageGPM, PagePSD As Double
            Dim p As Integer
            For i = 1 To Convert.ToInt32(Math.Ceiling(DetailsDataTable.Rows.Count / 4))
                PageIncome = 0
                PageGPM = 0
                PagePSD = 0
                p = (i - 1) * 4
                For j As Integer = 1 To Math.Min(4, DetailsDataTable.Rows.Count - p)
                    FormDataSet.Tables(8).Rows(28 + 71 * (i - 1) + 15 * (j - 1)).Item(1) = p + j
                    FormDataSet.Tables(8).Rows(29 + 71 * (i - 1) + 15 * (j - 1)).Item(1) = _
                        DetailsDataTable.Rows(p + j - 1).Item(1).ToString.ToUpper
                    FormDataSet.Tables(8).Rows(30 + 71 * (i - 1) + 15 * (j - 1)).Item(1) = _
                        DetailsDataTable.Rows(p + j - 1).Item(2)
                    FormDataSet.Tables(8).Rows(31 + 71 * (i - 1) + 15 * (j - 1)).Item(1) = _
                        DetailsDataTable.Rows(p + j - 1).Item(3)
                    FormDataSet.Tables(8).Rows(32 + 71 * (i - 1) + 15 * (j - 1)).Item(1) = _
                        GetNumberInFFDataFormat(CDbl(DetailsDataTable.Rows(p + j - 1).Item(4)))
                    FormDataSet.Tables(8).Rows(35 + 71 * (i - 1) + 15 * (j - 1)).Item(1) = _
                        GetNumberInFFDataFormat(CDbl(DetailsDataTable.Rows(p + j - 1).Item(5)))
                    FormDataSet.Tables(8).Rows(37 + 71 * (i - 1) + 15 * (j - 1)).Item(1) = _
                        DetailsDataTable.Rows(p + j - 1).Item(6)
                    FormDataSet.Tables(8).Rows(38 + 71 * (i - 1) + 15 * (j - 1)).Item(1) = _
                        DetailsDataTable.Rows(p + j - 1).Item(7)
                    If DetailsDataTable.Columns.Count > 8 AndAlso CInt(SpecificDataRow.Item(2)) = 2009 Then
                        FormDataSet.Tables(8).Rows(40 + 71 * (i - 1) + 15 * (j - 1)).Item(1) = _
                            GetNumberInFFDataFormat(CDbl(DetailsDataTable.Rows(p + j - 1).Item(8)))
                        PagePSD = PagePSD + CDbl(DetailsDataTable.Rows(p + j - 1).Item(8))
                    End If
                    PageIncome = PageIncome + CDbl(DetailsDataTable.Rows(p + j - 1).Item(4))
                    PageGPM = PageGPM + CDbl(DetailsDataTable.Rows(p + j - 1).Item(5))
                Next
                FormDataSet.Tables(8).Rows(88 + 71 * (i - 1)).Item(1) = GetNumberInFFDataFormat(PageIncome)
                FormDataSet.Tables(8).Rows(89 + 71 * (i - 1)).Item(1) = GetNumberInFFDataFormat(PageGPM)
                FormDataSet.Tables(8).Rows(90 + 71 * (i - 1)).Item(1) = GetNumberInFFDataFormat(PagePSD)
            Next

            Return FormDataSet

        End Function

        Private Function GetFFDataForFR0572_4(ByVal PreparatorName As String) As DataSet

            Dim DDS As DataSet = DeclarationDataSet
            Dim i As Integer
            Dim CurrentUser As AccDataAccessLayer.Security.AccIdentity = GetCurrentIdentity()

            If DDS.Tables("Details").Rows.Count < 1 Then
                Dim myDoc As New Xml.XmlDocument
                myDoc.Load(AppPath() & FILENAMEFFDATAFR0572_4)
                myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(0).ChildNodes(0).RemoveChild( _
                    myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(0).ChildNodes(0).ChildNodes(1))
                myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(1).RemoveChild( _
                    myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(1).ChildNodes(1))
                myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(1).Attributes(0).Value = "1"
                myDoc.Save(AppPath() & FILENAMEFFDATATEMP)

            ElseIf DDS.Tables("Details").Rows.Count > 4 Then
                Dim myDoc As New Xml.XmlDocument
                myDoc.Load(AppPath() & FILENAMEFFDATAFR0572_4)

                For i = 1 To Convert.ToInt32(Math.Ceiling(DDS.Tables("Details").Rows.Count / 4) - 1)
                    Dim AddPg1 As Xml.XmlElement = DirectCast(myDoc.ChildNodes(1).ChildNodes(0). _
                        ChildNodes(0).ChildNodes(0).ChildNodes(1).Clone(), Xml.XmlElement)
                    myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(0).ChildNodes(0).AppendChild(AddPg1)
                    Dim AddPg2 As Xml.XmlElement = DirectCast(myDoc.ChildNodes(1).ChildNodes(0). _
                        ChildNodes(1).ChildNodes(1).Clone, Xml.XmlElement)
                    AddPg2.Attributes(1).Value = (i + 2).ToString
                    myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(1).AppendChild(AddPg2)
                Next
                myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(1).Attributes(0).Value = _
                    Convert.ToInt32(Math.Ceiling(DDS.Tables("Details").Rows.Count / 4) + 1).ToString
                myDoc.Save(AppPath() & FILENAMEFFDATATEMP)

            Else
                IO.File.Copy(AppPath() & FILENAMEFFDATAFR0572_4, AppPath() & FILENAMEFFDATATEMP)
            End If

            ' read ffdata xml structure to dataset
            Dim FormDataSet As New DataSet
            Using FormFileStream As IO.FileStream = New IO.FileStream( _
                AppPath() & FILENAMEFFDATATEMP, IO.FileMode.Open)
                FormDataSet.ReadXml(FormFileStream)
                FormFileStream.Close()
            End Using

            FormDataSet.Tables(0).Rows(0).Item(3) = CurrentUser.Name
            FormDataSet.Tables(0).Rows(0).Item(4) = GetDateInFFDataFormat(Today)
            FormDataSet.Tables(1).Rows(0).Item(2) = AppPath() & FILENAMEMXFDFR0572_4

            Dim SpecificDataRow As DataRow = DDS.Tables("Specific").Rows(0)

            For i = 1 To FormDataSet.Tables(8).Rows.Count
                If FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_MM_ID" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = DDS.Tables("General").Rows(0).Item(1)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_MM_Pavad" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString(DDS.Tables("General").Rows(0).Item(0).ToString, 45).ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_MM_Adresas" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString(DDS.Tables("General").Rows(0).Item(2).ToString, 45).ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_MM_Tel" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString(DDS.Tables("General").Rows(0).Item(8).ToString, 12)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_MM_Epastas" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString(DDS.Tables("General").Rows(0).Item(7).ToString, 35).ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_UzpildData" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetDateInFFDataFormat( _
                        CDate(SpecificDataRow.Item(1)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_ML_Metai" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(2).ToString
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_ML_Menuo" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(3)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_MM_SavKodas" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(0).ToString
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E11" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(4).ToString
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E12" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(5).ToString
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E13" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(6).ToString
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E18" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(7)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E19" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(8)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E20" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(9)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E21" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(10)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E22" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(11)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E23" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(12)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E24" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(13)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E25" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(14)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E26" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(15)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E27" _
                    AndAlso CInt(SpecificDataRow.Item(2)) = 2009 Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(16)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E28" _
                    AndAlso CInt(SpecificDataRow.Item(2)) = 2009 Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(17)))
                End If
            Next

            Dim DetailsDataTable As DataTable = DDS.Tables("Details")
            Dim PageIncome, PageGPM, PagePSD As Double
            Dim p As Integer
            For i = 1 To Convert.ToInt32(Math.Ceiling(DetailsDataTable.Rows.Count / 4))
                PageIncome = 0
                PageGPM = 0
                PagePSD = 0
                p = (i - 1) * 4
                For j As Integer = 1 To Math.Min(4, DetailsDataTable.Rows.Count - p)
                    FormDataSet.Tables(8).Rows(28 + 75 * (i - 1) + 16 * (j - 1)).Item(1) = p + j
                    FormDataSet.Tables(8).Rows(29 + 75 * (i - 1) + 16 * (j - 1)).Item(1) = _
                        DetailsDataTable.Rows(p + j - 1).Item(1).ToString.ToUpper
                    FormDataSet.Tables(8).Rows(30 + 75 * (i - 1) + 16 * (j - 1)).Item(1) = _
                        DetailsDataTable.Rows(p + j - 1).Item(2)
                    FormDataSet.Tables(8).Rows(31 + 75 * (i - 1) + 16 * (j - 1)).Item(1) = _
                        DetailsDataTable.Rows(p + j - 1).Item(3)
                    FormDataSet.Tables(8).Rows(32 + 75 * (i - 1) + 16 * (j - 1)).Item(1) = _
                        GetNumberInFFDataFormat(CDbl(DetailsDataTable.Rows(p + j - 1).Item(4)))
                    FormDataSet.Tables(8).Rows(35 + 75 * (i - 1) + 16 * (j - 1)).Item(1) = _
                        GetNumberInFFDataFormat(CDbl(DetailsDataTable.Rows(p + j - 1).Item(5)))
                    FormDataSet.Tables(8).Rows(37 + 75 * (i - 1) + 16 * (j - 1)).Item(1) = _
                        DetailsDataTable.Rows(p + j - 1).Item(6)
                    FormDataSet.Tables(8).Rows(38 + 75 * (i - 1) + 16 * (j - 1)).Item(1) = _
                        DetailsDataTable.Rows(p + j - 1).Item(7)
                    FormDataSet.Tables(8).Rows(43 + 75 * (i - 1) + 16 * (j - 1)).Item(1) = "1"
                    If DetailsDataTable.Columns.Count > 8 AndAlso CInt(SpecificDataRow.Item(2)) = 2009 Then
                        FormDataSet.Tables(8).Rows(40 + 75 * (i - 1) + 16 * (j - 1)).Item(1) = _
                            GetNumberInFFDataFormat(CDbl(DetailsDataTable.Rows(p + j - 1).Item(8)))
                        PagePSD = PagePSD + CDbl(DetailsDataTable.Rows(p + j - 1).Item(8))
                    End If
                    PageIncome = PageIncome + CRound(CDbl(DetailsDataTable.Rows(p + j - 1).Item(4)), 2)
                    PageGPM = PageGPM + CRound(CDbl(DetailsDataTable.Rows(p + j - 1).Item(5)), 0)
                Next
                FormDataSet.Tables(8).Rows(92 + 75 * (i - 1)).Item(1) = GetNumberInFFDataFormat(PageIncome)
                FormDataSet.Tables(8).Rows(93 + 75 * (i - 1)).Item(1) = GetNumberInFFDataFormat(PageGPM)
                FormDataSet.Tables(8).Rows(94 + 75 * (i - 1)).Item(1) = GetNumberInFFDataFormat(PagePSD)
            Next

            Return FormDataSet

        End Function

        Private Function GetFFDataForFR0573_3(ByVal PreparatorName As String) As DataSet

            Dim DDS As DataSet = DeclarationDataSet
            Dim i As Integer
            Dim CurrentUser As AccDataAccessLayer.Security.AccIdentity = GetCurrentIdentity()

            If DDS.Tables("Details").Rows.Count > 1 Then

                Dim myDoc As New Xml.XmlDocument

                If _DeclarationType = ApskaitaObjects.DeclarationType.FR0573_3 Then
                    myDoc.Load(AppPath() & FILENAMEFFDATAFR0573_3)
                ElseIf _DeclarationType = ApskaitaObjects.DeclarationType.FR0573 Then
                    myDoc.Load(AppPath() & FILENAMEFFDATAFR0573_2)
                Else
                    myDoc.Load(AppPath() & FILENAMEFFDATAFR0573_4)
                End If

                For i = 1 To Convert.ToInt32(Math.Ceiling(DDS.Tables("Details").Rows.Count / 5) - 1)
                    Dim AddSD As Xml.XmlElement = DirectCast(myDoc.ChildNodes(1).ChildNodes(0). _
                        ChildNodes(1).ChildNodes(1).Clone, Xml.XmlElement)
                    AddSD.Attributes(1).Value = (i + 2).ToString
                    myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(1).AppendChild(AddSD)
                    Dim AddPg As Xml.XmlElement = DirectCast(myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(0). _
                        ChildNodes(0).ChildNodes(0).Clone, Xml.XmlElement)
                    myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(0).ChildNodes(0).AppendChild(AddPg)
                Next
                myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(1).Attributes(0).Value = _
                    (2 + Math.Ceiling(DDS.Tables("Details").Rows.Count / 5) - 1).ToString
                myDoc.Save(AppPath() & FILENAMEFFDATATEMP)
            Else
                If _DeclarationType = ApskaitaObjects.DeclarationType.FR0573_3 Then
                    IO.File.Copy(AppPath() & FILENAMEFFDATAFR0573_3, AppPath() & FILENAMEFFDATATEMP)
                ElseIf _DeclarationType = ApskaitaObjects.DeclarationType.FR0573 Then
                    IO.File.Copy(AppPath() & FILENAMEFFDATAFR0573_2, AppPath() & FILENAMEFFDATATEMP)
                Else
                    IO.File.Copy(AppPath() & FILENAMEFFDATAFR0573_4, AppPath() & FILENAMEFFDATATEMP)
                End If
            End If

            ' read ffdata xml structure to dataset
            Dim FormDataSet As New DataSet
            Using FormFileStream As IO.FileStream = New IO.FileStream( _
                AppPath() & FILENAMEFFDATATEMP, IO.FileMode.Open)
                FormDataSet.ReadXml(FormFileStream)
                FormFileStream.Close()
            End Using

            FormDataSet.Tables(0).Rows(0).Item(3) = CurrentUser.Name
            FormDataSet.Tables(0).Rows(0).Item(4) = GetDateInFFDataFormat(Today)
            If _DeclarationType = ApskaitaObjects.DeclarationType.FR0573 Then
                FormDataSet.Tables(1).Rows(0).Item(2) = AppPath() & FILENAMEMXFDFR0573_2
            ElseIf _DeclarationType = ApskaitaObjects.DeclarationType.FR0573_3 Then
                FormDataSet.Tables(1).Rows(0).Item(2) = AppPath() & FILENAMEMXFDFR0573_3
            ElseIf _DeclarationType = ApskaitaObjects.DeclarationType.FR0573_4 Then
                FormDataSet.Tables(1).Rows(0).Item(2) = AppPath() & FILENAMEMXFDFR0573_4
            End If


            Dim SpecificDataRow As DataRow = DDS.Tables("Specific").Rows(0)
            For i = 1 To FormDataSet.Tables(8).Rows.Count ' bendri duomenys
                If FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_MM_ID" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = DDS.Tables("General").Rows(0).Item(1)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_MM_Pavad" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString( _
                        DDS.Tables("General").Rows(0).Item(0).ToString, 43).ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_MM_Adresas" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString( _
                        DDS.Tables("General").Rows(0).Item(2).ToString, 43).ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_MM_Tel" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString( _
                        DDS.Tables("General").Rows(0).Item(8).ToString, 15)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_MM_Epastas" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString( _
                        DDS.Tables("General").Rows(0).Item(7).ToString, 35).ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_UzpildData" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetDateInFFDataFormat( _
                        CDate(SpecificDataRow.Item(1)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_ML_Metai" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(2).ToString
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E12" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = Math.Ceiling(DDS.Tables("Details").Rows.Count / 5)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E13" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = DDS.Tables("Details").Rows.Count
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "B_MM_SavKodas" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(0).ToString
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E16" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat(CDbl(SpecificDataRow.Item(7)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E17" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(10).ToString
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "E18" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(10).ToString
                End If
            Next

            Dim DetailsDataTable As DataTable = DDS.Tables("Details")
            Dim j, p As Integer
            Dim PageIncome As Double = 0
            Dim PagePayments As Double = 0

            ' both FR0573(2), FR0573(3) and FR0573(4) forms contain the same data
            ' but different xml structure; the difference is defined by PgSt variable
            ' which makes differents 'steps' when looping through xml
            Dim PgSt As Integer
            If _DeclarationType = ApskaitaObjects.DeclarationType.FR0573_3 OrElse _
                _DeclarationType = ApskaitaObjects.DeclarationType.FR0573_4 Then
                PgSt = 84
            Else
                PgSt = 79
            End If

            If _DeclarationType = ApskaitaObjects.DeclarationType.FR0573_4 Then

                For i = 1 To Convert.ToInt32(Math.Ceiling(DetailsDataTable.Rows.Count / 5))
                    p = 5 * (i - 1)
                    PageIncome = 0
                    PagePayments = 0
                    For j = 1 To Math.Min(5, DetailsDataTable.Rows.Count - p)
                        FormDataSet.Tables(8).Rows(20 + PgSt * (i - 1) + 15 * (j - 1)).Item(1) = j
                        FormDataSet.Tables(8).Rows(21 + PgSt * (i - 1) + 15 * (j - 1)).Item(1) = _
                            DetailsDataTable.Rows(p + j - 1).Item(1)
                        FormDataSet.Tables(8).Rows(22 + PgSt * (i - 1) + 15 * (j - 1)).Item(1) = "1"
                        FormDataSet.Tables(8).Rows(23 + PgSt * (i - 1) + 15 * (j - 1)).Item(1) = _
                            DetailsDataTable.Rows(p + j - 1).Item(2).ToString.ToUpper
                        FormDataSet.Tables(8).Rows(24 + PgSt * (i - 1) + 15 * (j - 1)).Item(1) = _
                            DetailsDataTable.Rows(p + j - 1).Item(3).ToString
                        FormDataSet.Tables(8).Rows(25 + PgSt * (i - 1) + 15 * (j - 1)).Item(1) = _
                                DetailsDataTable.Rows(p + j - 1).Item(4).ToString
                        FormDataSet.Tables(8).Rows(26 + PgSt * (i - 1) + 15 * (j - 1)).Item(1) = _
                            GetNumberInFFDataFormat(CDbl(DetailsDataTable.Rows(p + j - 1).Item(5)))
                        FormDataSet.Tables(8).Rows(27 + PgSt * (i - 1) + 15 * (j - 1)).Item(1) = _
                            GetNumberInFFDataFormat(CDbl(DetailsDataTable.Rows(p + j - 1).Item(6)))
                        FormDataSet.Tables(8).Rows(31 + PgSt * (i - 1) + 15 * (j - 1)).Item(1) = _
                            GetNumberInFFDataFormat(CDbl(DetailsDataTable.Rows(p + j - 1).Item(7)))
                        FormDataSet.Tables(8).Rows(32 + PgSt * (i - 1) + 15 * (j - 1)).Item(1) = _
                            DetailsDataTable.Rows(p + j - 1).Item(8).ToString
                        FormDataSet.Tables(8).Rows(33 + PgSt * (i - 1) + 15 * (j - 1)).Item(1) = _
                            DetailsDataTable.Rows(p + j - 1).Item(9).ToString
                        FormDataSet.Tables(8).Rows(34 + PgSt * (i - 1) + 15 * (j - 1)).Item(1) = _
                            DetailsDataTable.Rows(p + j - 1).Item(9).ToString
                        PageIncome = PageIncome + CDbl(DetailsDataTable.Rows(p + j - 1).Item(7))
                        PagePayments = PagePayments + CInt(DetailsDataTable.Rows(p + j - 1).Item(9))
                    Next
                    FormDataSet.Tables(8).Rows(95 + PgSt * (i - 1)).Item(1) = GetNumberInFFDataFormat(PageIncome)
                    FormDataSet.Tables(8).Rows(96 + PgSt * (i - 1)).Item(1) = GetNumberInFFDataFormat(PagePayments)
                    FormDataSet.Tables(8).Rows(97 + PgSt * (i - 1)).Item(1) = GetNumberInFFDataFormat(PagePayments)
                Next

            Else

                For i = 1 To Convert.ToInt32(Math.Ceiling(DetailsDataTable.Rows.Count / 5))
                    p = 5 * (i - 1)
                    PageIncome = 0
                    PagePayments = 0
                    For j = 1 To Math.Min(5, DetailsDataTable.Rows.Count - p)
                        FormDataSet.Tables(8).Rows(21 + PgSt * (i - 1) + 14 * (j - 1)).Item(1) = j
                        FormDataSet.Tables(8).Rows(22 + PgSt * (i - 1) + 14 * (j - 1)).Item(1) = _
                            DetailsDataTable.Rows(p + j - 1).Item(1)
                        FormDataSet.Tables(8).Rows(23 + PgSt * (i - 1) + 14 * (j - 1)).Item(1) = _
                            DetailsDataTable.Rows(p + j - 1).Item(2).ToString.ToUpper
                        FormDataSet.Tables(8).Rows(24 + PgSt * (i - 1) + 14 * (j - 1)).Item(1) = _
                            DetailsDataTable.Rows(p + j - 1).Item(3).ToString
                        FormDataSet.Tables(8).Rows(25 + PgSt * (i - 1) + 14 * (j - 1)).Item(1) = _
                                DetailsDataTable.Rows(p + j - 1).Item(4).ToString
                        FormDataSet.Tables(8).Rows(26 + PgSt * (i - 1) + 14 * (j - 1)).Item(1) = _
                            GetNumberInFFDataFormat(CDbl(DetailsDataTable.Rows(p + j - 1).Item(5)))
                        FormDataSet.Tables(8).Rows(27 + PgSt * (i - 1) + 14 * (j - 1)).Item(1) = _
                            GetNumberInFFDataFormat(CDbl(DetailsDataTable.Rows(p + j - 1).Item(6)))
                        FormDataSet.Tables(8).Rows(31 + PgSt * (i - 1) + 14 * (j - 1)).Item(1) = _
                            GetNumberInFFDataFormat(CDbl(DetailsDataTable.Rows(p + j - 1).Item(7)))
                        FormDataSet.Tables(8).Rows(32 + PgSt * (i - 1) + 14 * (j - 1)).Item(1) = _
                            DetailsDataTable.Rows(p + j - 1).Item(8).ToString
                        FormDataSet.Tables(8).Rows(33 + PgSt * (i - 1) + 14 * (j - 1)).Item(1) = _
                            DetailsDataTable.Rows(p + j - 1).Item(9).ToString
                        FormDataSet.Tables(8).Rows(34 + PgSt * (i - 1) + 14 * (j - 1)).Item(1) = _
                            DetailsDataTable.Rows(p + j - 1).Item(9).ToString
                        PageIncome = PageIncome + CDbl(DetailsDataTable.Rows(p + j - 1).Item(7))
                        PagePayments = PagePayments + CInt(DetailsDataTable.Rows(p + j - 1).Item(9))
                    Next
                    FormDataSet.Tables(8).Rows(91 + PgSt * (i - 1)).Item(1) = GetNumberInFFDataFormat(PageIncome)
                    FormDataSet.Tables(8).Rows(92 + PgSt * (i - 1)).Item(1) = GetNumberInFFDataFormat(PagePayments)
                    FormDataSet.Tables(8).Rows(93 + PgSt * (i - 1)).Item(1) = GetNumberInFFDataFormat(PagePayments)
                Next

            End If

            Return FormDataSet

        End Function

        Private Function GetFFDataForSD13(ByVal PreparatorName As String) As DataSet

            Dim DDS As DataSet = DeclarationDataSet
            Dim i As Integer
            Dim CurrentUser As AccDataAccessLayer.Security.AccIdentity = GetCurrentIdentity()

            Dim PageCount As Integer = 2
            If DDS.Tables("Details").Rows.Count > 6 Then

                Dim myDoc As New Xml.XmlDocument
                If _DeclarationType = ApskaitaObjects.DeclarationType.SD13_1 Then
                    myDoc.Load(AppPath() & FILENAMEFFDATASD13_1)
                ElseIf _DeclarationType = ApskaitaObjects.DeclarationType.SD13_2 Then
                    myDoc.Load(AppPath() & FILENAMEFFDATASD13_2)
                Else
                    myDoc.Load(AppPath() & FILENAMEFFDATASD13_5)
                End If

                PageCount = Convert.ToInt32(Math.Ceiling((DDS.Tables("Details").Rows.Count - 6) / 5) + 2)
                For i = 1 To PageCount - 2
                    Dim AddPg As Xml.XmlElement = DirectCast(myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(0). _
                        ChildNodes(0).ChildNodes(1).Clone, Xml.XmlElement)
                    myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(0).ChildNodes(0).AppendChild(AddPg)
                    Dim AddTb As Xml.XmlElement = DirectCast(myDoc.ChildNodes(1).ChildNodes(0). _
                        ChildNodes(1).ChildNodes(1).Clone, Xml.XmlElement)
                    AddTb.Attributes(1).Value = (i + 2).ToString
                    myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(1).AppendChild(AddTb)
                Next
                myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(1).Attributes(0).Value = PageCount.ToString
                myDoc.Save(AppPath() & FILENAMEFFDATATEMP)
            Else
                If _DeclarationType = ApskaitaObjects.DeclarationType.SD13_1 Then
                    IO.File.Copy(AppPath() & FILENAMEFFDATASD13_1, AppPath() & FILENAMEFFDATATEMP)
                ElseIf _DeclarationType = ApskaitaObjects.DeclarationType.SD13_2 Then
                    IO.File.Copy(AppPath() & FILENAMEFFDATASD13_2, AppPath() & FILENAMEFFDATATEMP)
                Else
                    IO.File.Copy(AppPath() & FILENAMEFFDATASD13_5, AppPath() & FILENAMEFFDATATEMP)
                End If
            End If

            ' read ffdata xml structure to dataset
            Dim FormDataSet As New DataSet
            Using FormFileStream As IO.FileStream = New IO.FileStream( _
                AppPath() & FILENAMEFFDATATEMP, IO.FileMode.Open)
                FormDataSet.ReadXml(FormFileStream)
                FormFileStream.Close()
            End Using

            FormDataSet.Tables(0).Rows(0).Item(3) = CurrentUser.Name
            FormDataSet.Tables(0).Rows(0).Item(4) = GetDateInFFDataFormat(Today)
            If _DeclarationType = ApskaitaObjects.DeclarationType.SD13_1 Then
                FormDataSet.Tables(1).Rows(0).Item(2) = AppPath() & FILENAMEMXFDSD13_1
            ElseIf _DeclarationType = ApskaitaObjects.DeclarationType.SD13_2 Then
                FormDataSet.Tables(1).Rows(0).Item(2) = AppPath() & FILENAMEMXFDSD13_2
            Else
                FormDataSet.Tables(1).Rows(0).Item(2) = AppPath() & FILENAMEMXFDSD13_5
            End If

            Dim SpecificDataRow As DataRow = DDS.Tables("Specific").Rows(0)

            For i = 1 To FormDataSet.Tables(8).Rows.Count ' bendri duomenys
                If FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "InsurerName" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString( _
                        DDS.Tables("General").Rows(0).Item(0).ToString, 68).ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "InsurerCode" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = DDS.Tables("General").Rows(0).Item(3)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "JuridicalPersonCode" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = DDS.Tables("General").Rows(0).Item(1)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "InsurerPhone" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString(DDS.Tables("General").Rows(0).Item(8).ToString, 15)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "InsurerAddress" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString( _
                        DDS.Tables("General").Rows(0).Item(2).ToString, 68).ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "RecipientDepName" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(1).ToString
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "DocDate" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetDateInFFDataFormat( _
                        CDate(SpecificDataRow.Item(0)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "DocNumber" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = "1"
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "TaxRateInsurer" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(6)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "TaxRatePerson" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(5)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "TaxRateTotal" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(7)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "PersonCountTotal" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(2).ToString
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "InsIncomeTotal" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(3)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "PaymentTotal" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat( _
                        CDbl(SpecificDataRow.Item(4)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "ManagerJobPosition" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = "DIREKTORIUS"
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "ManagerFullName" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = DDS.Tables("General").Rows(0).Item(9).ToString.Trim.ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "PreparatorDetails" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = PreparatorName.Trim.ToUpper
                End If
            Next

            If Not DDS.Tables("Details").Rows.Count > 0 Then Return FormDataSet

            Dim DetailsDataTable As DataTable = DDS.Tables("Details")
            Dim j As Integer
            Dim PageIncome As Double = 0
            Dim PagePayments As Double = 0

            ' first person appears on the title page (not an appendix)
            FormDataSet.Tables(8).Rows(18).Item(1) = 1
            FormDataSet.Tables(8).Rows(19).Item(1) = DetailsDataTable.Rows(0).Item(1)
            FormDataSet.Tables(8).Rows(20).Item(1) = DetailsDataTable.Rows(0).Item(2)
            FormDataSet.Tables(8).Rows(21).Item(1) = DetailsDataTable.Rows(0).Item(3)
            FormDataSet.Tables(8).Rows(22).Item(1) = GetDateInFFDataFormat(CDate(DetailsDataTable.Rows(0).Item(4)))
            FormDataSet.Tables(8).Rows(23).Item(1) = GetNumberInFFDataFormat(CDbl(DetailsDataTable.Rows(0).Item(5)))
            FormDataSet.Tables(8).Rows(24).Item(1) = GetNumberInFFDataFormat(CDbl(DetailsDataTable.Rows(0).Item(6)))
            FormDataSet.Tables(8).Rows(25).Item(1) = DetailsDataTable.Rows(0).Item(7)
            FormDataSet.Tables(8).Rows(26).Item(1) = DetailsDataTable.Rows(0).Item(8)
            FormDataSet.Tables(8).Rows(27).Item(1) = DetailsDataTable.Rows(0).Item(9)

            For i = 1 To PageCount - 1
                PageIncome = 0
                PagePayments = 0
                For j = 1 To Math.Min(5, DetailsDataTable.Rows.Count - (i - 1) * 5 - 1)
                    FormDataSet.Tables(8).Rows(38 + (j - 1) * 10 + (i - 1) * 59).Item(1) = (i - 1) * 5 + j + 1
                    FormDataSet.Tables(8).Rows(39 + (j - 1) * 10 + (i - 1) * 59).Item(1) = _
                        DetailsDataTable.Rows((i - 1) * 5 + j).Item(1)
                    FormDataSet.Tables(8).Rows(40 + (j - 1) * 10 + (i - 1) * 59).Item(1) = _
                        DetailsDataTable.Rows((i - 1) * 5 + j).Item(2)
                    FormDataSet.Tables(8).Rows(41 + (j - 1) * 10 + (i - 1) * 59).Item(1) = _
                        DetailsDataTable.Rows((i - 1) * 5 + j).Item(3)
                    FormDataSet.Tables(8).Rows(42 + (j - 1) * 10 + (i - 1) * 59).Item(1) = _
                        GetDateInFFDataFormat(CDate(DetailsDataTable.Rows((i - 1) * 5 + j).Item(4)))
                    FormDataSet.Tables(8).Rows(43 + (j - 1) * 10 + (i - 1) * 59).Item(1) = _
                        GetNumberInFFDataFormat(CDbl(DetailsDataTable.Rows((i - 1) * 5 + j).Item(5)))
                    FormDataSet.Tables(8).Rows(44 + (j - 1) * 10 + (i - 1) * 59).Item(1) = _
                        GetNumberInFFDataFormat(CDbl(DetailsDataTable.Rows((i - 1) * 5 + j).Item(6)))
                    FormDataSet.Tables(8).Rows(45 + (j - 1) * 10 + (i - 1) * 59).Item(1) = _
                        DetailsDataTable.Rows((i - 1) * 5 + j).Item(7)
                    FormDataSet.Tables(8).Rows(46 + (j - 1) * 10 + (i - 1) * 59).Item(1) = _
                        DetailsDataTable.Rows((i - 1) * 5 + j).Item(8)
                    FormDataSet.Tables(8).Rows(47 + (j - 1) * 10 + (i - 1) * 59).Item(1) = _
                        DetailsDataTable.Rows((i - 1) * 5 + j).Item(9)
                    PageIncome = PageIncome + CDbl(DetailsDataTable.Rows((i - 1) * 5 + j).Item(5))
                    PagePayments = PagePayments + CDbl(DetailsDataTable.Rows((i - 1) * 5 + j).Item(6))
                Next
                FormDataSet.Tables(8).Rows(88 + (i - 1) * 59).Item(1) = GetNumberInFFDataFormat(PageIncome)
                FormDataSet.Tables(8).Rows(89 + (i - 1) * 59).Item(1) = GetNumberInFFDataFormat(PagePayments)
            Next

            Return FormDataSet

        End Function

        Private Function GetFFDataForSAM_2(ByVal PreparatorName As String) As DataSet

            Dim i As Integer
            Dim DDS As DataSet = DeclarationDataSet
            Dim CurrentUser As AccDataAccessLayer.Security.AccIdentity = GetCurrentIdentity()

            ' Add 3SD appendixes to the ffdata xml structure if needed
            ' and copy form structure to the temp file
            If CInt(DDS.Tables("Specific").Rows(0).Item(6)) > 1 Then
                Dim myDoc As New Xml.XmlDocument
                If _DeclarationType = ApskaitaObjects.DeclarationType.SAM_2 Then
                    myDoc.Load(AppPath() & FILENAMEFFDATASAM02)
                ElseIf _DeclarationType = ApskaitaObjects.DeclarationType.SAM_3 Then
                    myDoc.Load(AppPath() & FILENAMEFFDATASAM03)
                Else
                    myDoc.Load(AppPath() & FILENAMEFFDATASAM04)
                End If
                For i = 1 To Convert.ToInt32(Math.Ceiling(CInt(DDS.Tables("Specific").Rows(0).Item(6)) / 9) - 1)
                    Dim AddSD As Xml.XmlElement = DirectCast(myDoc.ChildNodes(1).ChildNodes(0). _
                        ChildNodes(1).ChildNodes(1).Clone, Xml.XmlElement)
                    AddSD.Attributes(1).Value = (i + 2).ToString
                    AddSD.ChildNodes(0).ChildNodes(2).InnerText = (i + 1).ToString
                    AddSD.ChildNodes(0).ChildNodes(3).InnerText = _
                        Math.Ceiling(CInt(DDS.Tables("Specific").Rows(0).Item(6)) / 9).ToString
                    myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(1).AppendChild(AddSD)
                    Dim AddPg As Xml.XmlElement = DirectCast(myDoc.ChildNodes(1).ChildNodes(0). _
                        ChildNodes(0).ChildNodes(0).ChildNodes(1).Clone, Xml.XmlElement)
                    myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(0).ChildNodes(0).AppendChild(AddPg)
                Next
                myDoc.ChildNodes(1).ChildNodes(0).ChildNodes(1).Attributes(0).Value = _
                    (Math.Ceiling(CInt(DDS.Tables("Specific").Rows(0).Item(6)) / 9) + 1).ToString
                myDoc.Save(AppPath() & FILENAMEFFDATATEMP)
            Else
                If _DeclarationType = ApskaitaObjects.DeclarationType.SAM_2 Then
                    IO.File.Copy(AppPath() & FILENAMEFFDATASAM02, AppPath() & FILENAMEFFDATATEMP)
                ElseIf _DeclarationType = ApskaitaObjects.DeclarationType.SAM_3 Then
                    IO.File.Copy(AppPath() & FILENAMEFFDATASAM03, AppPath() & FILENAMEFFDATATEMP)
                Else
                    IO.File.Copy(AppPath() & FILENAMEFFDATASAM04, AppPath() & FILENAMEFFDATATEMP)
                End If
            End If

            ' read ffdata xml structure to dataset
            Dim FormDataSet As New DataSet
            Using FormFileStream As IO.FileStream = New IO.FileStream( _
                AppPath() & FILENAMEFFDATATEMP, IO.FileMode.Open)
                FormDataSet.ReadXml(FormFileStream)
                FormFileStream.Close()
            End Using

            FormDataSet.Tables(0).Rows(0).Item(3) = CurrentUser.Name
            FormDataSet.Tables(0).Rows(0).Item(4) = GetDateInFFDataFormat(Today)
            If _DeclarationType = ApskaitaObjects.DeclarationType.SAM_2 Then
                FormDataSet.Tables(1).Rows(0).Item(2) = AppPath() & FILENAMEMXFDSAM02
            ElseIf _DeclarationType = ApskaitaObjects.DeclarationType.SAM_3 Then
                FormDataSet.Tables(1).Rows(0).Item(2) = AppPath() & FILENAMEMXFDSAM03
            Else
                FormDataSet.Tables(1).Rows(0).Item(2) = AppPath() & FILENAMEMXFDSAM04
            End If

            Dim SpecificDataRow As DataRow = DDS.Tables("Specific").Rows(0)
            For i = 1 To FormDataSet.Tables(8).Rows.Count
                If FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "InsurerName" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString( _
                        DDS.Tables("General").Rows(0).Item(0).ToString, 68).ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "InsurerCode" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = DDS.Tables("General").Rows(0).Item(3)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "PreparatorDetails" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString( _
                        PreparatorName, 68).ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "InsurerPhone" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString( _
                        DDS.Tables("General").Rows(0).Item(8).ToString, 15)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "JuridicalPersonCode" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = DDS.Tables("General").Rows(0).Item(1)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "InsurerAddress" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString( _
                        DDS.Tables("General").Rows(0).Item(2).ToString, 68).ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "RecipientDepName" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(1).ToString.ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "DocDate" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetDateInFFDataFormat(CDate(SpecificDataRow.Item(0)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "TaxRateRep" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat(CDbl(SpecificDataRow.Item(4)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "CycleYear" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(2)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "CycleMonth" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(3)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "Appendixes2" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = 1
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "Apdx2PageCount" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = Math.Ceiling(CInt(SpecificDataRow.Item(6)) / 9).ToString
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "Apdx2PersonCount" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = SpecificDataRow.Item(6)
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "Apdx2InsIncomeSum" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat(CDbl(SpecificDataRow.Item(7)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "Apdx2PaymentSum" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetNumberInFFDataFormat(CDbl(SpecificDataRow.Item(8)))
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "ManagerJobPosition" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = "DIREKTORIUS"
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "ManagerFullName" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = GetLimitedLengthString( _
                        DDS.Tables("General").Rows(0).Item(9).ToString, 68).ToUpper
                ElseIf FormDataSet.Tables(8).Rows(i - 1).Item(0).ToString = "ApdxPageCountTotal" Then
                    FormDataSet.Tables(8).Rows(i - 1).Item(1) = Math.Ceiling(CInt(SpecificDataRow.Item(6)) / 9).ToString
                End If
            Next


            Dim j, p As Integer
            Dim PageIncome, PagePayments As Double
            Dim DetailsDataTable As DataTable = DDS.Tables("Details")
            For i = 1 To Convert.ToInt32(Math.Ceiling(DetailsDataTable.Rows.Count / 9))
                PageIncome = 0
                PagePayments = 0
                p = 9 * (i - 1)

                For j = 1 To Math.Min(9, DetailsDataTable.Rows.Count - p)
                    FormDataSet.Tables(8).Rows(38 + 7 * (j - 1) + (i - 1) * 72).Item(1) = p + j
                    FormDataSet.Tables(8).Rows(39 + 7 * (j - 1) + (i - 1) * 72).Item(1) = _
                        DetailsDataTable.Rows(p + j - 1).Item(1)
                    FormDataSet.Tables(8).Rows(40 + 7 * (j - 1) + (i - 1) * 72).Item(1) = _
                        DetailsDataTable.Rows(p + j - 1).Item(2)
                    FormDataSet.Tables(8).Rows(41 + 7 * (j - 1) + (i - 1) * 72).Item(1) = _
                        DetailsDataTable.Rows(p + j - 1).Item(3)
                    FormDataSet.Tables(8).Rows(42 + 7 * (j - 1) + (i - 1) * 72).Item(1) = _
                        GetNumberInFFDataFormat(CDbl(DetailsDataTable.Rows(p + j - 1).Item(5)))
                    FormDataSet.Tables(8).Rows(43 + 7 * (j - 1) + (i - 1) * 72).Item(1) = _
                        GetNumberInFFDataFormat(CDbl(DetailsDataTable.Rows(p + j - 1).Item(6)))
                    FormDataSet.Tables(8).Rows(44 + 7 * (j - 1) + (i - 1) * 72).Item(1) = _
                        DetailsDataTable.Rows(p + j - 1).Item(4).ToString.ToUpper

                    PageIncome = PageIncome + CDbl(DetailsDataTable.Rows(p + j - 1).Item(5))
                    PagePayments = PagePayments + CDbl(DetailsDataTable.Rows(p + j - 1).Item(6))
                Next

                FormDataSet.Tables(8).Rows(101 + (i - 1) * 72).Item(1) = GetNumberInFFDataFormat(PageIncome)
                FormDataSet.Tables(8).Rows(102 + (i - 1) * 72).Item(1) = GetNumberInFFDataFormat(PagePayments)
            Next

            Return FormDataSet

        End Function


        Protected Overrides Function GetIdValue() As Object
            Return _ID
        End Function

#End Region

#Region " Authorization Rules "

        Protected Overrides Sub AddAuthorizationRules()

            ' TODO: add authorization rules
            'AuthorizationRules.AllowRead("", "")

        End Sub

        Public Shared Function CanGetObject() As Boolean
            Return ApplicationContext.User.IsInRole("Workers.Declaration1")
        End Function

#End Region

#Region " Factory Methods "

        Public Shared Function GetDeclaration(ByVal nDeclarationType As DeclarationType, _
                ByVal nDate As Date, ByVal nDateFrom As Date, ByVal nDateTo As Date, _
                ByVal nSODRADepartment As String, ByVal nSODRARate As Double, ByVal nYear As Integer, _
                ByVal nQuarter As Integer, ByVal nMonth As Integer, ByVal nSODRAAccount As Long, _
                ByVal nSODRAAccount2 As Long, ByVal nMunicipalityCode As String, _
                ByVal nDeclarationItemCode As String) As Declaration

            Dim AllErrors As String = ""

            If (nDeclarationType = ApskaitaObjects.DeclarationType.SD13_1 OrElse _
                nDeclarationType = ApskaitaObjects.DeclarationType.SD13_2 OrElse _
                nDeclarationType = ApskaitaObjects.DeclarationType.SD13_5 OrElse _
                nDeclarationType = ApskaitaObjects.DeclarationType.SAM_1 OrElse _
                nDeclarationType = ApskaitaObjects.DeclarationType.SAM_Aut_1 OrElse _
                nDeclarationType = ApskaitaObjects.DeclarationType.SAM_2 OrElse _
                nDeclarationType = ApskaitaObjects.DeclarationType.SAM_3 OrElse _
                nDeclarationType = ApskaitaObjects.DeclarationType.SAM_4) AndAlso _
                String.IsNullOrEmpty(nSODRADepartment.Trim) Then _
                AllErrors = AllErrors & vbCrLf & "Nepasirinktas SODROS skyrius."
            If (nDeclarationType = ApskaitaObjects.DeclarationType.SAM_1 OrElse _
                nDeclarationType = ApskaitaObjects.DeclarationType.SAM_Aut_1) AndAlso Not nSODRAAccount > 0 Then _
                AllErrors = AllErrors & vbCrLf & "Nepasirinkta mokėtinų SODROS įmokų apskaitos sąskaita."
            If (nDeclarationType = ApskaitaObjects.DeclarationType.SAM_1 OrElse _
                nDeclarationType = ApskaitaObjects.DeclarationType.SAM_Aut_1 OrElse _
                nDeclarationType = ApskaitaObjects.DeclarationType.FR0572 OrElse _
                nDeclarationType = ApskaitaObjects.DeclarationType.FR0572_3 OrElse _
                nDeclarationType = ApskaitaObjects.DeclarationType.FR0573 OrElse _
                nDeclarationType = ApskaitaObjects.DeclarationType.FR0573_3 OrElse _
                nDeclarationType = ApskaitaObjects.DeclarationType.FR0573_4 OrElse _
                nDeclarationType = ApskaitaObjects.DeclarationType.SAM_2 OrElse _
                nDeclarationType = ApskaitaObjects.DeclarationType.SAM_3 OrElse _
                nDeclarationType = ApskaitaObjects.DeclarationType.SAM_4) AndAlso Not nYear > 0 Then _
                AllErrors = AllErrors & vbCrLf & "Nepasirinkti deklaracijos laikotarpio metai."
            If (nDeclarationType = ApskaitaObjects.DeclarationType.SAM_1 OrElse _
                nDeclarationType = ApskaitaObjects.DeclarationType.SAM_Aut_1) AndAlso Not nQuarter > 0 Then _
                AllErrors = AllErrors & vbCrLf & "Nepasirinktas deklaracijos laikotarpio ketvirtis."
            If (nDeclarationType = ApskaitaObjects.DeclarationType.FR0572 OrElse _
                nDeclarationType = ApskaitaObjects.DeclarationType.FR0572_3 OrElse _
                nDeclarationType = ApskaitaObjects.DeclarationType.SAM_2 OrElse _
                nDeclarationType = ApskaitaObjects.DeclarationType.SAM_3 OrElse _
                nDeclarationType = ApskaitaObjects.DeclarationType.SAM_4) AndAlso Not nMonth > 0 Then _
                AllErrors = AllErrors & vbCrLf & "Nepasirinktas deklaracijos laikotarpio mėnuo."
            If (nDeclarationType = ApskaitaObjects.DeclarationType.FR0572 OrElse _
                nDeclarationType = ApskaitaObjects.DeclarationType.FR0572_3 OrElse _
                nDeclarationType = ApskaitaObjects.DeclarationType.FR0573 OrElse _
                nDeclarationType = ApskaitaObjects.DeclarationType.FR0573_3 OrElse _
                nDeclarationType = ApskaitaObjects.DeclarationType.FR0573_4) AndAlso _
                String.IsNullOrEmpty(nMunicipalityCode.Trim) Then _
                AllErrors = AllErrors & vbCrLf & "Nepasirinktas savivaldybės kodas."
            If (nDeclarationType = ApskaitaObjects.DeclarationType.SAM_3 OrElse _
                nDeclarationType = ApskaitaObjects.DeclarationType.SAM_4) AndAlso _
                Not CRound(nSODRARate) > 0 Then _
                AllErrors = AllErrors & vbCrLf & "Nenurodytas SODRA tarifas."

            If Not String.IsNullOrEmpty(AllErrors.Trim) Then _
                Throw New Exception("Klaida. Nepasirinkti visi deklaracijai gauti " & _
                    "reikalingi parametrai: " & vbCrLf & AllErrors.Trim)



            Return DataPortal.Fetch(Of Declaration)(New Criteria(nDeclarationType, _
                nDate, nDateFrom, nDateTo, nSODRADepartment, nSODRARate, nYear, nQuarter, nMonth, _
                nSODRAAccount, nSODRAAccount2, nMunicipalityCode, nDeclarationItemCode))
        End Function

        Private Sub New()
            ' require use of factory methods
        End Sub

#End Region

#Region " Data Access "

        <Serializable()> _
        Private Class Criteria
            Private _DeclarationType As DeclarationType
            Private _Date As Date
            Private _DateFrom As Date
            Private _DateTo As Date
            Private _SODRADepartment As String
            Private _SODRARate As Double
            Private _Year As Integer
            Private _Quarter As Integer
            Private _Month As Integer
            Private _SODRAAccount As Long
            Private _SODRAAccount2 As Long
            Private _MunicipalityCode As String
            Private _DeclarationItemCode As String

            Public ReadOnly Property DeclarationType() As DeclarationType
                Get
                    Return _DeclarationType
                End Get
            End Property
            Public ReadOnly Property [Date]() As Date
                Get
                    Return _Date
                End Get
            End Property
            Public ReadOnly Property DateFrom() As Date
                Get
                    Return _DateFrom
                End Get
            End Property
            Public ReadOnly Property DateTo() As Date
                Get
                    Return _DateTo
                End Get
            End Property
            Public ReadOnly Property SODRADepartment() As String
                Get
                    Return _SODRADepartment
                End Get
            End Property
            Public ReadOnly Property SODRARate() As Double
                Get
                    Return CRound(_SODRARate)
                End Get
            End Property
            Public ReadOnly Property Year() As Integer
                Get
                    Return _Year
                End Get
            End Property
            Public ReadOnly Property Quarter() As Integer
                Get
                    Return _Quarter
                End Get
            End Property
            Public ReadOnly Property Month() As Integer
                Get
                    Return _Month
                End Get
            End Property
            Public ReadOnly Property SODRAAccount() As Long
                Get
                    Return _SODRAAccount
                End Get
            End Property
            Public ReadOnly Property SODRAAccount2() As Long
                Get
                    Return _SODRAAccount2
                End Get
            End Property
            Public ReadOnly Property MunicipalityCode() As String
                Get
                    Return _MunicipalityCode
                End Get
            End Property
            Public ReadOnly Property DeclarationItemCode() As String
                Get
                    Return _DeclarationItemCode
                End Get
            End Property

            Public Sub New(ByVal nDeclarationType As DeclarationType, _
                ByVal nDate As Date, ByVal nDateFrom As Date, ByVal nDateTo As Date, _
                ByVal nSODRADepartment As String, ByVal nSODRARate As Double, ByVal nYear As Integer, _
                ByVal nQuarter As Integer, ByVal nMonth As Integer, ByVal nSODRAAccount As Long, _
                ByVal nSODRAAccount2 As Long, ByVal nMunicipalityCode As String, _
                ByVal nDeclarationItemCode As String)
                _DeclarationType = nDeclarationType
                _Date = nDate.Date
                _DateFrom = nDateFrom.Date
                _DateTo = nDateTo.Date
                _SODRADepartment = nSODRADepartment
                _SODRARate = nSODRARate
                _Year = nYear
                _Quarter = nQuarter
                _Month = nMonth
                _SODRAAccount = nSODRAAccount
                _SODRAAccount2 = nSODRAAccount2
                _MunicipalityCode = nMunicipalityCode
                _DeclarationItemCode = nDeclarationItemCode
            End Sub
        End Class


        Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

            If Not CanGetObject() Then Throw New System.Security.SecurityException( _
                "Klaida. Jūsų teisių nepakanka šiai informacijai gauti.")

            Dim result As DataSet
            If criteria.DeclarationType = ApskaitaObjects.DeclarationType.SD13_1 OrElse _
                criteria.DeclarationType = ApskaitaObjects.DeclarationType.SD13_2 OrElse _
                criteria.DeclarationType = ApskaitaObjects.DeclarationType.SD13_5 Then
                result = FetchSD13(criteria)
            ElseIf criteria.DeclarationType = ApskaitaObjects.DeclarationType.SAM_1 Then
                result = FetchSAM_1(criteria)
            ElseIf criteria.DeclarationType = ApskaitaObjects.DeclarationType.SAM_Aut_1 Then
                result = FetchSAM_Aut_1(criteria)
            ElseIf criteria.DeclarationType = ApskaitaObjects.DeclarationType.FR0572 Then
                result = FetchFR0572_2(criteria)
            ElseIf criteria.DeclarationType = ApskaitaObjects.DeclarationType.FR0572_3 _
                OrElse criteria.DeclarationType = ApskaitaObjects.DeclarationType.FR0572_4 Then
                result = FetchFR0572_3(criteria)
            ElseIf criteria.DeclarationType = ApskaitaObjects.DeclarationType.FR0573 OrElse _
                criteria.DeclarationType = ApskaitaObjects.DeclarationType.FR0573_3 OrElse _
                criteria.DeclarationType = ApskaitaObjects.DeclarationType.FR0573_4 Then
                result = FetchFR0573_3(criteria)
            ElseIf criteria.DeclarationType = ApskaitaObjects.DeclarationType.SAM_2 _
                OrElse criteria.DeclarationType = ApskaitaObjects.DeclarationType.SAM_3 _
                OrElse criteria.DeclarationType = ApskaitaObjects.DeclarationType.SAM_4 Then
                result = FetchSAM_2(criteria)


            Else
                Throw New NotSupportedException("Klaida. Deklaracijos tipas '" & _
                    criteria.DeclarationType.ToString & "' nepalaikomas.")
            End If

            WriteDataSetToString(result)
            result.Dispose()

            _DeclarationType = criteria.DeclarationType

            _DeclarationDateCulture = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat
            _NumberDecimalSeparator = _
                System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator
            _NumberGroupSeparator = _
                System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator

        End Sub

        Private Sub WriteDataSetToString(ByVal DS As DataSet)

            ' this method does not adds datatables with 0 rows
            ' thus we need to create an empty row and mark it for deletion when deserializing

            For Each DT As DataTable In DS.Tables
                If DT.Rows.Count < 1 AndAlso DT.Columns.Count > 0 Then
                    DT.Rows.Add()
                    DT.Rows(0).Item(0) = "%#DELETE#"
                End If
            Next

            Dim objStream As New IO.MemoryStream()
            DS.WriteXml(objStream)

            Dim objXmlWriter As New Xml.XmlTextWriter(objStream, Text.Encoding.UTF8)
            objStream = DirectCast(objXmlWriter.BaseStream, IO.MemoryStream)

            Dim objEncoding As New Text.UTF8Encoding

            _DeclarationString = objEncoding.GetString(objStream.ToArray())

            objStream.Dispose()

        End Sub

        Private Function FetchGeneralDataTable() As DataTable
            Dim result As New DataTable("General")

            result.Columns.Add("CompanyName")
            result.Columns.Add("Code")
            result.Columns.Add("Address")
            result.Columns.Add("CodeSODRA")
            result.Columns.Add("CodeVAT")
            result.Columns.Add("Bank")
            result.Columns.Add("BankAccount")
            result.Columns.Add("Email")
            result.Columns.Add("Tel")
            result.Columns.Add("HeadPerson")

            result.Rows.Add()
            result.Rows(0).Item(0) = GetLimitedLengthString(GetCurrentCompany.Name, 68)
            result.Rows(0).Item(1) = GetCurrentCompany.Code
            result.Rows(0).Item(2) = GetLimitedLengthString(GetCurrentCompany.Address, 68)
            result.Rows(0).Item(3) = GetCurrentCompany.CodeSODRA
            result.Rows(0).Item(4) = GetCurrentCompany.CodeVat
            result.Rows(0).Item(5) = GetCurrentCompany.Bank
            result.Rows(0).Item(6) = GetCurrentCompany.BankAccount
            result.Rows(0).Item(7) = GetLimitedLengthString(GetCurrentCompany.Email, 45)
            result.Rows(0).Item(8) = "" ' GetLimitedLengthString(GetCurrentCompany.tel, 15)
            result.Rows(0).Item(9) = GetCurrentCompany.HeadPerson

            Return result
        End Function

        Private Shared Function GetEmployeesCount(ByVal AtDate As Date) As Integer

            Dim result As Integer = 0

            Dim myComm As New SQLCommand("FetchEmployeesCount")
            myComm.AddParam("?DT", AtDate.Date)

            Using myData As DataTable = myComm.Fetch
                ClearDatatable(myData, 0)
                If myData.Rows.Count > 0 AndAlso myData.Columns.Count > 1 Then _
                    result = CInt(myData.Rows(0).Item(0)) - CInt(myData.Rows(0).Item(1))
                If result < 0 Then result = 0
            End Using

            Return result
        End Function

        Private Function FetchSD13(ByVal criteria As Criteria) As DataSet

            Dim result As New DataSet
            result.Tables.Add(FetchGeneralDataTable)

            Dim SD As New DataTable("Specific")
            SD.Columns.Add("Date")
            SD.Columns.Add("SODRADepartment")
            SD.Columns.Add("InsuredCount")
            SD.Columns.Add("TotalIncome")
            SD.Columns.Add("TotalPayments")
            SD.Columns.Add("TarifForInsurant")
            SD.Columns.Add("TarifForAssured")
            SD.Columns.Add("TarifTotal")
            SD.Columns.Add("DateFrom")
            SD.Columns.Add("DateTo")
            SD.Rows.Add()

            Dim DD As New DataTable("Details")
            DD.Columns.Add("Count")
            DD.Columns.Add("PersonCode")
            DD.Columns.Add("SODRASerial")
            DD.Columns.Add("SODRACode")
            DD.Columns.Add("Date")
            DD.Columns.Add("Income")
            DD.Columns.Add("Payment")
            DD.Columns.Add("PersonName")
            DD.Columns.Add("ReasonCode")
            DD.Columns.Add("ReasonText")

            Try

                Dim myComm As New SQLCommand("FetchDeclarationSD13")
                myComm.AddParam("?DF", criteria.DateFrom.Date)
                myComm.AddParam("?DT", criteria.DateTo.Date)

                Using myData As DataTable = myComm.Fetch

                    Dim SmB As Double = 0
                    Dim SmI As Double = 0
                    Dim TrIs As Double = 0
                    Dim TrPri As Double = 0
                    Dim SC As HelperLists.NameValueItemList = _
                        HelperLists.NameValueItemList.GetNameValueItemList(HelperLists.SettingListType.SodraCodeList)
                    Dim i As Integer = 0

                    For Each dr As DataRow In myData.Rows
                        DD.Rows.Add()
                        DD.Rows(i).Item(0) = i + 1
                        DD.Rows(i).Item(1) = dr.Item(0).ToString
                        If Not IsDBNull(dr.Item(1)) AndAlso Not String.IsNullOrEmpty(dr.Item(1).ToString.Trim) Then
                            DD.Rows(i).Item(2) = dr.Item(1).ToString.Trim.Substring(0, 2)
                            DD.Rows(i).Item(3) = GetNumericPart(dr.Item(1).ToString.Trim)
                        Else
                            DD.Rows(i).Item(2) = ""
                            DD.Rows(i).Item(3) = ""
                        End If
                        DD.Rows(i).Item(4) = CDate(dr.Item(2)).ToShortDateString
                        DD.Rows(i).Item(5) = DblParser(CDbl(dr.Item(3)))
                        DD.Rows(i).Item(6) = DblParser(CDbl(dr.Item(4)))
                        DD.Rows(i).Item(7) = GetLimitedLengthString(dr.Item(5).ToString.Trim, 68)

                        If Not IsDBNull(dr.Item(6)) AndAlso Not String.IsNullOrEmpty(dr.Item(6).ToString.Trim) Then
                            Dim SCI As NameValueItem = SC.GetItemByValue(dr.Item(6).ToString.Trim)
                            If SCI Is Nothing Then Throw New Exception("Klaida. Nežinomas priežasties kodas '" & _
                                dr.Item(6).ToString.Trim & "'.")
                            DD.Rows(i).Item(8) = SCI.Value
                            DD.Rows(i).Item(9) = SCI.Name
                        Else
                            DD.Rows(i).Item(8) = ""
                            DD.Rows(i).Item(9) = ""
                        End If

                        SmB = SmB + CDbl(dr.Item(3))
                        SmI = SmI + CDbl(dr.Item(4))

                        If CDblSafe(dr.Item(7), 2, 0) > 0 Then TrIs = CDblSafe(dr.Item(7), 2, 0)
                        If CDblSafe(dr.Item(8), 2, 0) > 0 Then TrPri = CDblSafe(dr.Item(8), 2, 0)

                        i += 1

                    Next

                    SD.Rows(0).Item("SODRADepartment") = criteria.SODRADepartment
                    SD.Rows(0).Item("InsuredCount") = myData.Rows.Count
                    SD.Rows(0).Item("TotalIncome") = DblParser(SmB)
                    SD.Rows(0).Item("TotalPayments") = DblParser(SmI)
                    SD.Rows(0).Item("TarifForInsurant") = CRound(TrIs)
                    SD.Rows(0).Item("TarifForAssured") = CRound(TrPri)
                    SD.Rows(0).Item("TarifTotal") = CRound(TrIs + TrPri)
                    SD.Rows(0).Item("DateFrom") = criteria.DateFrom.ToShortDateString
                    SD.Rows(0).Item("DateTo") = criteria.DateTo.ToShortDateString
                    SD.Rows(0).Item("Date") = criteria.Date.ToShortDateString

                End Using

                result.Tables.Add(SD)
                result.Tables.Add(DD)

            Catch ex As Exception
                SD.Dispose()
                DD.Dispose()
                result.Dispose()
                Throw ex
            End Try

            Return result
        End Function

        Private Function FetchSAM_1(ByVal criteria As Criteria) As DataSet

            Dim result As New DataSet
            result.Tables.Add(FetchGeneralDataTable)

            Dim SD As New DataTable("Specific")
            SD.Columns.Add("Date")
            SD.Columns.Add("SODRADepartment")
            SD.Columns.Add("Year")
            SD.Columns.Add("Quarter")
            SD.Columns.Add("Tarif")
            SD.Columns.Add("3SDPageCount")
            SD.Columns.Add("TotalPageCount")
            SD.Columns.Add("WorkerCount")
            SD.Columns.Add("TotalIncome")
            SD.Columns.Add("TotalPayments")
            SD.Columns.Add("F1")
            SD.Columns.Add("F2")
            SD.Columns.Add("F3")
            SD.Columns.Add("F4")
            SD.Columns.Add("F6")
            SD.Columns.Add("M1")
            SD.Columns.Add("M2")
            SD.Columns.Add("M3")
            SD.Columns.Add("M3.1")
            SD.Columns.Add("M3.2")
            SD.Columns.Add("M3.3")
            SD.Columns.Add("M4")
            SD.Columns.Add("M5")
            SD.Columns.Add("M6")
            SD.Columns.Add("M7")
            SD.Columns.Add("M7.1")
            SD.Columns.Add("M7.2")
            SD.Columns.Add("M7.3")
            SD.Columns.Add("M8")
            SD.Columns.Add("M11")

            SD.Rows.Add()

            Dim DD As New DataTable("Details")
            DD.Columns.Add("Count")
            DD.Columns.Add("PersonName")
            DD.Columns.Add("PersonCode")
            DD.Columns.Add("SODRACode")
            DD.Columns.Add("Income")
            DD.Columns.Add("Payment")

            Try
                Dim LastDayInQuarter As Integer = 31
                If criteria.Quarter = 2 OrElse criteria.Quarter = 3 Then LastDayInQuarter = 30

                Dim myComm As New SQLCommand("FetchDeclarationSAM(1)_1")
                myComm.AddParam("?YR", criteria.Year)
                myComm.AddParam("?MF", ((criteria.Quarter - 1) * 3) + 1)
                myComm.AddParam("?MT", ((criteria.Quarter - 1) * 3) + 3)
                myComm.AddParam("?DT", New Date(criteria.Year, _
                    ((criteria.Quarter - 1) * 3) + 3, LastDayInQuarter))

                Dim TotalIncome As Double = 0
                Dim TotalPayments As Double = 0
                Dim TotalTarif As Double
                Dim i As Integer

                Using myData As DataTable = myComm.Fetch
                    ClearDatatable(myData, 0)
                    i = 0
                    For Each dr As DataRow In myData.Rows
                        DD.Rows.Add()
                        DD.Rows(i).Item(0) = i + 1
                        DD.Rows(i).Item(1) = GetLimitedLengthString(dr.Item(0).ToString.Trim, 68).ToUpper
                        DD.Rows(i).Item(2) = dr.Item(1).ToString.Trim
                        DD.Rows(i).Item(3) = dr.Item(2).ToString.Trim
                        DD.Rows(i).Item(4) = DblParser(CDbl(dr.Item(3)))
                        DD.Rows(i).Item(5) = DblParser(CDbl(dr.Item(4)))
                        TotalIncome = TotalIncome + CDbl(dr.Item(3))
                        TotalPayments = TotalPayments + CDbl(dr.Item(4))
                        i += 1
                    Next
                End Using

                myComm = New SQLCommand("FetchDeclarationSAM(1)_2")
                myComm.AddParam("?YR", criteria.Year)
                myComm.AddParam("?MF", ((criteria.Quarter - 1) * 3) + 1)
                myComm.AddParam("?MT", ((criteria.Quarter - 1) * 3) + 3)

                Using myData As DataTable = myComm.Fetch
                    ClearDatatable(myData, 0)

                    If myData.Rows.Count > 1 Then
                        _Warning = "DĖMESIO. Pasirinktą ketvirtį buvo taikomi skirtingi " _
                        & "SODROS ir PSD tarifai: "
                        For Each dr As DataRow In myData.Rows
                            _Warning = _Warning & vbCrLf & "SODRA išskaičiuota - " & dr.Item(0).ToString _
                            & "; PSD isšskaičiuota - " & dr.Item(2).ToString & "; SODRA Priskaičiuota - " _
                            & dr.Item(1).ToString & "PSD priskaičiuota - " & dr.Item(3).ToString & ";"
                        Next
                        _Warning = _Warning & vbCrLf & "Deklaracijoje naudojama tik pirmoji tarifų kombinacija."

                    ElseIf myData.Rows.Count < 1 Then

                        myData.Rows.Add()
                        myData.Rows(0).Item(0) = GetCurrentCompany.Rates.GetRate(RateType.SodraEmployee)
                        myData.Rows(0).Item(1) = GetCurrentCompany.Rates.GetRate(RateType.SodraEmployer)
                        myData.Rows(0).Item(2) = GetCurrentCompany.Rates.GetRate(RateType.PsdEmployee)
                        myData.Rows(0).Item(3) = GetCurrentCompany.Rates.GetRate(RateType.PsdEmployer)

                        _Warning = "DĖMESIO. Pasirinktą ketvirtį nebuvo apskaitoje registruotų " _
                        & "darbo užmokesčio žiniaraščių, iš kurių būtų matomi pasirinktą ketvirtį " _
                        & "faktiškai taikyti SODROS įmokų tarifai. Deklaracijoje naudojami einamieji tarifai."

                    End If

                    If criteria.Year < 2009 Then
                        TotalTarif = CDbl(myData.Rows(0).Item(0)) + CDbl(myData.Rows(0).Item(1))
                    Else
                        TotalTarif = CDbl(myData.Rows(0).Item(0)) + CDbl(myData.Rows(0).Item(1)) _
                            + CDbl(myData.Rows(0).Item(2)) + CDbl(myData.Rows(0).Item(3))
                    End If

                End Using

                SD.Rows(0).Item(0) = criteria.Date.ToShortDateString
                SD.Rows(0).Item(1) = criteria.SODRADepartment
                SD.Rows(0).Item(2) = criteria.Year
                SD.Rows(0).Item(3) = criteria.Quarter
                SD.Rows(0).Item(4) = DblParser(TotalTarif)
                SD.Rows(0).Item(5) = Math.Ceiling(DD.Rows.Count / 9)
                SD.Rows(0).Item(6) = Math.Ceiling(DD.Rows.Count / 9) + 1
                SD.Rows(0).Item(7) = DD.Rows.Count
                SD.Rows(0).Item(8) = DblParser(TotalIncome)
                SD.Rows(0).Item(9) = DblParser(TotalPayments)

                myComm = New SQLCommand("FetchDeclarationSAM(1)_3")
                myComm.AddParam("?DF", New Date(criteria.Year, _
                    ((criteria.Quarter - 1) * 3) + 1, 1))
                myComm.AddParam("?DT", New Date(criteria.Year, _
                    ((criteria.Quarter - 1) * 3) + 3, LastDayInQuarter))

                Using myData As DataTable = myComm.Fetch
                    SD.Rows(0).Item(10) = CInt(myData.Rows(0).Item(1)) - CInt(myData.Rows(1).Item(1))
                    SD.Rows(0).Item(11) = CInt(myData.Rows(2).Item(1))
                    SD.Rows(0).Item(12) = CInt(myData.Rows(3).Item(1))
                    SD.Rows(0).Item(13) = CInt(myData.Rows(0).Item(1)) - _
                        CInt(myData.Rows(1).Item(1)) + CInt(myData.Rows(2).Item(1)) _
                        - CInt(myData.Rows(3).Item(1))
                End Using

                Dim BalanceYearStart As Double = 0
                Dim PaymentsDueQuarterStart As Double = 0
                Dim PaymentsDueQuarter As Double = 0
                Dim PaymentsDue1 As Double = 0
                Dim PaymentsDue2 As Double = 0
                Dim PaymentsDue3 As Double = 0
                Dim PaymentsDueQuarterEnd As Double = 0
                Dim PaymentsBalanceQuarterEnd As Double = 0
                Dim PaymentsMadeQuarterStart As Double = 0
                Dim PaymentsMadeQuarter As Double = 0
                Dim PaymentsMade1 As Double = 0
                Dim PaymentsMade2 As Double = 0
                Dim PaymentsMade3 As Double = 0
                Dim PaymentsMadeQuarterEnd As Double = 0
                Dim BalanceQuarterEnd As Double = 0

                Dim TotalIncomePerQuarter As Double = 0

                ' there is a problem with this query:
                ' a journal entry that transfers sums between pripary and secondary
                ' accounts is treated as affecting balance at the begining of the year
                myComm = New SQLCommand("FetchDeclarationSAM(1)_4")
                myComm.AddParam("?YR", criteria.Year)
                myComm.AddParam("?SP", criteria.SODRAAccount)
                myComm.AddParam("?SS", criteria.SODRAAccount2)

                Using myData As DataTable = myComm.Fetch

                    ClearDatatable(myData, 0)

                    ' simplification (joining) by month
                    Dim j As Integer
                    For i = myData.Rows.Count To 1 Step -1
                        For j = 1 To i - 1
                            If CInt(myData.Rows(i - 1).Item(0)) = CInt(myData.Rows(j - 1).Item(0)) Then
                                myData.Rows(j - 1).Item(1) = CRound(CDbl(myData.Rows(j - 1).Item(1))) _
                                    + CRound(CDbl(myData.Rows(i - 1).Item(1)))
                                myData.Rows(j - 1).Item(2) = CRound(CDbl(myData.Rows(j - 1).Item(2))) _
                                    + CRound(CDbl(myData.Rows(i - 1).Item(2)))
                                myData.Rows(j - 1).Item(3) = CRound(CDbl(myData.Rows(j - 1).Item(3))) _
                                    + CRound(CDbl(myData.Rows(i - 1).Item(3)))
                                myData.Rows.RemoveAt(i - 1)
                                Exit For
                            End If
                        Next
                    Next

                    ' QuarterStartMonth and QuarterEndMonth
                    Dim QSM As Integer = ((criteria.Quarter - 1) * 3) + 1
                    Dim QEM As Integer = ((criteria.Quarter - 1) * 3) + 3

                    For Each dr As DataRow In myData.Rows
                        If CInt(dr.Item(0)) < QSM Then
                            PaymentsDueQuarterStart = PaymentsDueQuarterStart + CDbl(dr.Item(2))
                            PaymentsMadeQuarterStart = PaymentsMadeQuarterStart + CDbl(dr.Item(3))

                        ElseIf CInt(dr.Item(0)) >= QSM AndAlso CInt(dr.Item(0)) <= QEM Then

                            If CInt(dr.Item(0)) = QSM Then
                                PaymentsDue1 = PaymentsDue1 + CDbl(dr.Item(2))
                                PaymentsMade1 = PaymentsMade1 + CDbl(dr.Item(3))
                            ElseIf CInt(dr.Item(0)) = QEM Then
                                PaymentsDue3 = PaymentsDue3 + CDbl(dr.Item(2))
                                PaymentsMade3 = PaymentsMade3 + CDbl(dr.Item(3))
                            Else
                                PaymentsDue2 = PaymentsDue2 + CDbl(dr.Item(2))
                                PaymentsMade2 = PaymentsMade2 + CDbl(dr.Item(3))
                            End If

                            TotalIncomePerQuarter = TotalIncomePerQuarter + CDbl(dr.Item(1))

                        ElseIf CInt(dr.Item(0)) = 13 Then
                            BalanceYearStart = BalanceYearStart + CDbl(dr.Item(3))

                        End If
                    Next

                End Using

                PaymentsDueQuarter = CRound(PaymentsDue1 + PaymentsDue2 + PaymentsDue3, 2)
                PaymentsDueQuarterEnd = CRound(PaymentsDueQuarterStart + PaymentsDueQuarter, 2)
                PaymentsBalanceQuarterEnd = CRound(BalanceYearStart + PaymentsDueQuarterEnd, 2)

                PaymentsMadeQuarter = CRound(PaymentsMade1 + PaymentsMade2 + PaymentsMade3, 2)
                PaymentsMadeQuarterEnd = CRound(PaymentsMadeQuarterStart + PaymentsMadeQuarter, 2)
                BalanceQuarterEnd = CRound(PaymentsBalanceQuarterEnd - PaymentsMadeQuarterEnd, 2)

                SD.Rows(0).Item(14) = DblParser(TotalIncomePerQuarter)
                SD.Rows(0).Item(15) = DblParser(BalanceYearStart)
                SD.Rows(0).Item(16) = DblParser(PaymentsDueQuarterStart)
                SD.Rows(0).Item(17) = DblParser(PaymentsDueQuarter)
                SD.Rows(0).Item(18) = DblParser(PaymentsDue1)
                SD.Rows(0).Item(19) = DblParser(PaymentsDue2)
                SD.Rows(0).Item(20) = DblParser(PaymentsDue3)
                SD.Rows(0).Item(21) = DblParser(PaymentsDueQuarterEnd)
                SD.Rows(0).Item(22) = DblParser(PaymentsBalanceQuarterEnd)
                SD.Rows(0).Item(23) = DblParser(PaymentsMadeQuarterStart)
                SD.Rows(0).Item(24) = DblParser(PaymentsMadeQuarter)
                SD.Rows(0).Item(25) = DblParser(PaymentsMade1)
                SD.Rows(0).Item(26) = DblParser(PaymentsMade2)
                SD.Rows(0).Item(27) = DblParser(PaymentsMade3)
                SD.Rows(0).Item(28) = DblParser(PaymentsMadeQuarterEnd)
                SD.Rows(0).Item(29) = DblParser(BalanceQuarterEnd)

                If criteria.Quarter = 1 Then
                    SD.Rows(0).Item(16) = ""
                    SD.Rows(0).Item(23) = ""
                End If

                result.Tables.Add(SD)
                result.Tables.Add(DD)

            Catch ex As Exception
                SD.Dispose()
                DD.Dispose()
                result.Dispose()
                Throw ex
            End Try

            Return result
        End Function

        Private Function FetchSAM_Aut_1(ByVal criteria As Criteria) As DataSet

            Dim result As New DataSet
            result.Tables.Add(FetchGeneralDataTable)

            Dim SD As New DataTable("Specific")
            SD.Columns.Add("Date")
            SD.Columns.Add("SODRADepartment")
            SD.Columns.Add("Year")
            SD.Columns.Add("Quarter")
            SD.Columns.Add("Tarif")
            SD.Columns.Add("3SDPageCount")
            SD.Columns.Add("TotalPageCount")
            SD.Columns.Add("WorkerCount")
            SD.Columns.Add("TotalIncome")
            SD.Columns.Add("TotalPayments")
            SD.Columns.Add("F1")
            SD.Columns.Add("F2")
            SD.Columns.Add("F3")
            SD.Columns.Add("F4")
            SD.Columns.Add("F6")
            SD.Columns.Add("M1")
            SD.Columns.Add("M2")
            SD.Columns.Add("M3")
            SD.Columns.Add("M3.1")
            SD.Columns.Add("M3.2")
            SD.Columns.Add("M3.3")
            SD.Columns.Add("M4")
            SD.Columns.Add("M5")
            SD.Columns.Add("M6")
            SD.Columns.Add("M7")
            SD.Columns.Add("M7.1")
            SD.Columns.Add("M7.2")
            SD.Columns.Add("M7.3")
            SD.Columns.Add("M8")
            SD.Columns.Add("M11")

            SD.Rows.Add()

            Dim DD As New DataTable("Details")
            DD.Columns.Add("Count")
            DD.Columns.Add("PersonName")
            DD.Columns.Add("PersonCode")
            DD.Columns.Add("SODRACode")
            DD.Columns.Add("Income")
            DD.Columns.Add("Payment")

            SD.Rows(0).Item(0) = criteria.Date.ToShortDateString
            SD.Rows(0).Item(1) = criteria.SODRADepartment
            SD.Rows(0).Item(2) = criteria.Year
            SD.Rows(0).Item(3) = criteria.Quarter
            SD.Rows(0).Item(5) = ""
            SD.Rows(0).Item(6) = 1
            SD.Rows(0).Item(7) = ""
            SD.Rows(0).Item(8) = ""
            SD.Rows(0).Item(9) = ""
            SD.Rows(0).Item(10) = ""
            SD.Rows(0).Item(11) = ""
            SD.Rows(0).Item(12) = ""
            SD.Rows(0).Item(13) = ""

            Try

                Dim myComm As New SQLCommand("FetchDeclarationSAM(1)_Aut_1")
                myComm.AddParam("?YR", criteria.Year)
                myComm.AddParam("?MF", ((criteria.Quarter - 1) * 3) + 1)
                myComm.AddParam("?MT", ((criteria.Quarter - 1) * 3) + 3)

                Using myData As DataTable = myComm.Fetch

                    ClearDatatable(myData, 0)

                    If myData.Rows.Count > 1 Then
                        _Warning = "DĖMESIO. Pasirinktą ketvirtį buvo taikomi skirtingi " _
                        & "SODROS ir PSD tarifai: "
                        For Each dr As DataRow In myData.Rows
                            _Warning = _Warning & vbCrLf & "SODRA išskaičiuota - " & dr.Item(0).ToString _
                            & "; PSD isšskaičiuota - " & dr.Item(2).ToString & "; SODRA Priskaičiuota - " _
                            & dr.Item(1).ToString & "PSD priskaičiuota - " & dr.Item(3).ToString & ";"
                        Next
                        _Warning = _Warning & vbCrLf & "Deklaracijoje naudojama tik pirmoji tarifų kombinacija."

                    ElseIf myData.Rows.Count < 1 Then

                        myData.Rows.Add()
                        myData.Rows(0).Item(0) = 1
                        myData.Rows(0).Item(1) = 7
                        myData.Rows(0).Item(2) = 6
                        myData.Rows(0).Item(3) = 3

                        _Warning = "DĖMESIO. Pasirinktą ketvirtį nebuvo apskaitoje registruotų " _
                        & "išmokų autoriams ar sportininkams, iš kurių būtų matomi pasirinktą ketvirtį " _
                        & "faktiškai taikyti SODROS įmokų tarifai. Deklaracijoje naudojami einamieji tarifai."

                    End If

                    SD.Rows(0).Item(4) = CRound(CDbl(myData.Rows(0).Item(0)) + CDbl(myData.Rows(0).Item(1)) _
                            + CDbl(myData.Rows(0).Item(2)) + CDbl(myData.Rows(0).Item(3)))

                End Using

                Dim BalanceYearStart As Double = 0
                Dim PaymentsDueQuarterStart As Double = 0
                Dim PaymentsDueQuarter As Double = 0
                Dim PaymentsDue1 As Double = 0
                Dim PaymentsDue2 As Double = 0
                Dim PaymentsDue3 As Double = 0
                Dim PaymentsDueQuarterEnd As Double = 0
                Dim PaymentsBalanceQuarterEnd As Double = 0
                Dim PaymentsMadeQuarterStart As Double = 0
                Dim PaymentsMadeQuarter As Double = 0
                Dim PaymentsMade1 As Double = 0
                Dim PaymentsMade2 As Double = 0
                Dim PaymentsMade3 As Double = 0
                Dim PaymentsMadeQuarterEnd As Double = 0
                Dim BalanceQuarterEnd As Double = 0

                Dim TotalIncomePerQuarter As Double = 0

                myComm = New SQLCommand("FetchDeclarationSAM(1)_Aut_2")
                myComm.AddParam("?YR", criteria.Year)
                myComm.AddParam("?SP", criteria.SODRAAccount)
                myComm.AddParam("?SS", criteria.SODRAAccount2)

                Using myData As DataTable = myComm.Fetch

                    ClearDatatable(myData, 0)

                    ' simplification (joining) by month
                    Dim j As Integer
                    For i As Integer = myData.Rows.Count To 1 Step -1
                        For j = 1 To i - 1
                            If CInt(myData.Rows(i - 1).Item(0)) = CInt(myData.Rows(j - 1).Item(0)) Then
                                myData.Rows(j - 1).Item(1) = CRound(CDbl(myData.Rows(j - 1).Item(1))) _
                                    + CRound(CDbl(myData.Rows(i - 1).Item(1)))
                                myData.Rows(j - 1).Item(2) = CRound(CDbl(myData.Rows(j - 1).Item(2))) _
                                    + CRound(CDbl(myData.Rows(i - 1).Item(2)))
                                myData.Rows(j - 1).Item(3) = CRound(CDbl(myData.Rows(j - 1).Item(3))) _
                                    + CRound(CDbl(myData.Rows(i - 1).Item(3)))
                                myData.Rows.RemoveAt(i - 1)
                                Exit For
                            End If
                        Next
                    Next

                    ' QuarterStartMonth and QuarterEndMonth
                    Dim QSM As Integer = ((criteria.Quarter - 1) * 3) + 1
                    Dim QEM As Integer = ((criteria.Quarter - 1) * 3) + 3

                    For Each dr As DataRow In myData.Rows
                        If CInt(dr.Item(0)) < QSM Then
                            PaymentsDueQuarterStart = PaymentsDueQuarterStart + CDbl(dr.Item(2))
                            PaymentsMadeQuarterStart = PaymentsMadeQuarterStart + CDbl(dr.Item(3))

                        ElseIf CInt(dr.Item(0)) >= QSM AndAlso CInt(dr.Item(0)) <= QEM Then

                            If CInt(dr.Item(0)) = QSM Then
                                PaymentsDue1 = PaymentsDue1 + CDbl(dr.Item(2))
                                PaymentsMade1 = PaymentsMade1 + CDbl(dr.Item(3))
                            ElseIf CInt(dr.Item(0)) = QEM Then
                                PaymentsDue3 = PaymentsDue3 + CDbl(dr.Item(2))
                                PaymentsMade3 = PaymentsMade3 + CDbl(dr.Item(3))
                            Else
                                PaymentsDue2 = PaymentsDue2 + CDbl(dr.Item(2))
                                PaymentsMade2 = PaymentsMade2 + CDbl(dr.Item(3))
                            End If

                            TotalIncomePerQuarter = TotalIncomePerQuarter + CDbl(dr.Item(1))

                        ElseIf CInt(dr.Item(0)) = 13 Then
                            BalanceYearStart = BalanceYearStart + CDbl(dr.Item(3))

                        End If
                    Next
                End Using

                PaymentsDueQuarter = PaymentsDue1 + PaymentsDue2 + PaymentsDue3
                PaymentsDueQuarterEnd = PaymentsDueQuarterStart + PaymentsDueQuarter
                PaymentsBalanceQuarterEnd = BalanceYearStart + PaymentsDueQuarterEnd

                PaymentsMadeQuarter = PaymentsMade1 + PaymentsMade2 + PaymentsMade3
                PaymentsMadeQuarterEnd = PaymentsMadeQuarterStart + PaymentsMadeQuarter
                BalanceQuarterEnd = PaymentsBalanceQuarterEnd - PaymentsMadeQuarterEnd

                SD.Rows(0).Item(14) = DblParser(TotalIncomePerQuarter)
                SD.Rows(0).Item(15) = DblParser(BalanceYearStart)
                SD.Rows(0).Item(16) = DblParser(PaymentsDueQuarterStart)
                SD.Rows(0).Item(17) = DblParser(PaymentsDueQuarter)
                SD.Rows(0).Item(18) = DblParser(PaymentsDue1)
                SD.Rows(0).Item(19) = DblParser(PaymentsDue2)
                SD.Rows(0).Item(20) = DblParser(PaymentsDue3)
                SD.Rows(0).Item(21) = DblParser(PaymentsDueQuarterEnd)
                SD.Rows(0).Item(22) = DblParser(PaymentsBalanceQuarterEnd)
                SD.Rows(0).Item(23) = DblParser(PaymentsMadeQuarterStart)
                SD.Rows(0).Item(24) = DblParser(PaymentsMadeQuarter)
                SD.Rows(0).Item(25) = DblParser(PaymentsMade1)
                SD.Rows(0).Item(26) = DblParser(PaymentsMade2)
                SD.Rows(0).Item(27) = DblParser(PaymentsMade3)
                SD.Rows(0).Item(28) = DblParser(PaymentsMadeQuarterEnd)
                SD.Rows(0).Item(29) = DblParser(BalanceQuarterEnd)

                If criteria.Quarter = 1 Then
                    SD.Rows(0).Item(16) = ""
                    SD.Rows(0).Item(23) = ""
                End If

                result.Tables.Add(SD)
                result.Tables.Add(DD)

            Catch ex As Exception
                SD.Dispose()
                DD.Dispose()
                result.Dispose()
                Throw ex
            End Try

            Return result

        End Function

        Private Function FetchFR0572_2(ByVal criteria As Criteria) As DataSet

            Dim result As New DataSet
            result.Tables.Add(FetchGeneralDataTable)

            Dim SD As New DataTable("Specific")
            SD.Columns.Add("MunicipalityCode")
            SD.Columns.Add("Date")
            SD.Columns.Add("Year")
            SD.Columns.Add("Month")
            SD.Columns.Add("WorkerCount")
            SD.Columns.Add("LabourIncome")
            SD.Columns.Add("LabourIncomeTaxBefore15")
            SD.Columns.Add("LabourIncomeTaxAfter15")
            SD.Columns.Add("OtherIncome")
            SD.Columns.Add("OtherIncomeTaxBefore15")
            SD.Columns.Add("OtherIncomeTaxAfter15")

            SD.Rows.Add()

            SD.Rows(0).Item(0) = criteria.MunicipalityCode
            SD.Rows(0).Item(1) = criteria.Date.ToShortDateString
            SD.Rows(0).Item(2) = criteria.Year
            SD.Rows(0).Item(3) = criteria.Month

            Try
                SD.Rows(0).Item(4) = GetEmployeesCount(New Date(criteria.Year, _
                    criteria.Month, Date.DaysInMonth(criteria.Year, criteria.Month)))

                Dim myComm As New SQLCommand("FetchDeclarationFR0572(2)")
                myComm.AddParam("?YR", criteria.Year)
                myComm.AddParam("?MN", criteria.Month)

                Using myData As DataTable = myComm.Fetch

                    If myData.Rows.Count < 1 Then myData.Rows.Add()
                    ClearDatatable(myData, 0)

                    SD.Rows(0).Item(5) = DblParser(CDbl(myData.Rows(0).Item(0)))
                    SD.Rows(0).Item(6) = DblParser(CDbl(myData.Rows(0).Item(1)))
                    SD.Rows(0).Item(7) = DblParser(CDbl(myData.Rows(0).Item(2)))
                    SD.Rows(0).Item(8) = DblParser(CDbl(myData.Rows(0).Item(3)))
                    SD.Rows(0).Item(9) = DblParser(CDbl(myData.Rows(0).Item(4)))
                    SD.Rows(0).Item(10) = DblParser(CDbl(myData.Rows(0).Item(5)))

                End Using

                result.Tables.Add(SD)

            Catch ex As Exception
                SD.Dispose()
                result.Dispose()
                Throw ex
            End Try

            Return result
        End Function

        Private Function FetchFR0572_3(ByVal criteria As Criteria) As DataSet

            Dim result As New DataSet
            result.Tables.Add(FetchGeneralDataTable)

            Dim SD As New DataTable("Specific")
            SD.Columns.Add("MunicipalityCode")
            SD.Columns.Add("Date")
            SD.Columns.Add("Year")
            SD.Columns.Add("Month")
            SD.Columns.Add("WorkerCount")
            SD.Columns.Add("AppendixPageCount")
            SD.Columns.Add("AppendixLineCount")
            SD.Columns.Add("LabourIncome")
            SD.Columns.Add("LabourIncomeTaxBefore15")
            SD.Columns.Add("LabourIncomeTaxAfter15")
            SD.Columns.Add("OtherIncome")
            SD.Columns.Add("OtherIncomeTaxBefore15")
            SD.Columns.Add("OtherIncomeTaxAfter15")
            SD.Columns.Add("SalesIncome")
            SD.Columns.Add("SalesIncomeTaxBefore15")
            SD.Columns.Add("SalesIncomeTaxAfter15")
            SD.Columns.Add("PSDBefore15")
            SD.Columns.Add("PSDAfter15")
            SD.Columns.Add("TotalIncome")
            SD.Columns.Add("TotalIncomeTax")
            SD.Columns.Add("TotalPSD")

            SD.Rows.Add()

            SD.Rows(0).Item(0) = criteria.MunicipalityCode
            SD.Rows(0).Item(1) = criteria.Date.ToShortDateString
            SD.Rows(0).Item(2) = criteria.Year
            SD.Rows(0).Item(3) = criteria.Month

            Dim DD As New DataTable("Details")
            DD.Columns.Add("Count")
            DD.Columns.Add("PersonName")
            DD.Columns.Add("PersonCode")
            DD.Columns.Add("MunicipalityCode")
            DD.Columns.Add("Income")
            DD.Columns.Add("IncomeTax")
            DD.Columns.Add("TaxDueDate")
            DD.Columns.Add("IncomeCode")
            DD.Columns.Add("PSD")

            Try

                SD.Rows(0).Item(4) = GetEmployeesCount(New Date( _
                    criteria.Year, criteria.Month, Date.DaysInMonth(criteria.Year, criteria.Month)))

                Dim myComm As New SQLCommand("FetchDeclarationFR0572(3)_1")
                myComm.AddParam("?MN", criteria.Month)
                myComm.AddParam("?YR", criteria.Year)
                Using myData As DataTable = myComm.Fetch

                    If myData.Rows.Count < 1 Then myData.Rows.Add()
                    ClearDatatable(myData, 0)

                    SD.Rows(0).Item(7) = DblParser(CDbl(myData.Rows(0).Item(0)) + _
                        CDbl(myData.Rows(0).Item(1)) + CDbl(myData.Rows(0).Item(4)))
                    SD.Rows(0).Item(8) = DblParser(CDbl(myData.Rows(0).Item(2)))
                    SD.Rows(0).Item(9) = DblParser(CDbl(myData.Rows(0).Item(3)))

                End Using

                Dim OtherIncomeTotal, PSDBefore15, PSDAfter15 As Double

                myComm = New SQLCommand("FetchDeclarationFR0572(3)_2")
                myComm.AddParam("?MN", criteria.Month)
                myComm.AddParam("?YR", criteria.Year)
                Using myData As DataTable = myComm.Fetch

                    If myData.Rows.Count < 1 Then myData.Rows.Add()
                    ClearDatatable(myData, 0)

                    OtherIncomeTotal = CRound(CDbl(myData.Rows(0).Item(0)))
                    SD.Rows(0).Item(11) = DblParser(CDbl(myData.Rows(0).Item(1)))
                    SD.Rows(0).Item(12) = DblParser(CDbl(myData.Rows(0).Item(2)))
                    PSDBefore15 = CRound(CDbl(myData.Rows(0).Item(3)))
                    PSDAfter15 = CRound(CDbl(myData.Rows(0).Item(4)))

                End Using

                Dim SalesIncomeTotal As Double = 0
                Dim SalesIncomeTaxBefore15 As Double = 0
                Dim SalesIncomeTaxAfter15 As Double = 0
                Dim AppendixIncomeTotal As Double = 0
                Dim AppendixIncomeTaxTotal As Double = 0
                Dim AppendixPSDTotal As Double = 0
                Dim i As Integer = 0

                myComm = New SQLCommand("FetchDeclarationFR0572(3)_3")
                myComm.AddParam("?MN", criteria.Month)
                myComm.AddParam("?YR", criteria.Year)
                Using myData As DataTable = myComm.Fetch

                    ClearDatatable(myData, 0)

                    For Each dr As DataRow In myData.Rows
                        DD.Rows.Add()
                        DD.Rows(i).Item(0) = i + 1
                        DD.Rows(i).Item(1) = dr.Item(0).ToString
                        DD.Rows(i).Item(2) = dr.Item(1).ToString
                        DD.Rows(i).Item(3) = criteria.MunicipalityCode
                        DD.Rows(i).Item(4) = DblParser(CDbl(dr.Item(2)))
                        DD.Rows(i).Item(5) = DblParser(CDbl(dr.Item(3)))
                        DD.Rows(i).Item(6) = CInt(dr.Item(4))
                        DD.Rows(i).Item(7) = GetMinLengthString(dr.Item(5).ToString, 2, "0"c)
                        DD.Rows(i).Item(8) = DblParser(CDbl(dr.Item(6)))

                        If CInt(dr.Item(5)) > 13 AndAlso CInt(dr.Item(5)) < 19 Then
                            ' it's a sales income tax codes
                            SalesIncomeTotal = SalesIncomeTotal + CDbl(dr.Item(2))
                            If CInt(dr.Item(4)) = 1 Then
                                SalesIncomeTaxBefore15 = SalesIncomeTaxBefore15 + CDbl(dr.Item(3))
                            Else
                                SalesIncomeTaxAfter15 = SalesIncomeTaxAfter15 + CDbl(dr.Item(3))
                            End If

                        ElseIf CInt(dr.Item(5)) = 3 Then
                            ' it's a sick leave tax code
                            OtherIncomeTotal = OtherIncomeTotal + CDbl(dr.Item(2))
                            If CInt(dr.Item(4)) = 1 Then
                                PSDBefore15 = PSDBefore15 + CDbl(dr.Item(6))
                            Else
                                PSDAfter15 = PSDAfter15 + CDbl(dr.Item(6))
                            End If

                        End If

                        AppendixIncomeTotal = AppendixIncomeTotal + CDbl(dr.Item(2))
                        AppendixIncomeTaxTotal = AppendixIncomeTaxTotal + CDbl(dr.Item(3))
                        AppendixPSDTotal = AppendixPSDTotal + CDbl(dr.Item(6))

                        i += 1
                    Next

                End Using

                SD.Rows(0).Item(5) = Math.Ceiling(i / 4)
                SD.Rows(0).Item(6) = Math.Ceiling(i)
                SD.Rows(0).Item(10) = DblParser(OtherIncomeTotal)
                SD.Rows(0).Item(13) = DblParser(SalesIncomeTotal)
                SD.Rows(0).Item(14) = DblParser(SalesIncomeTaxBefore15)
                SD.Rows(0).Item(15) = DblParser(SalesIncomeTaxAfter15)
                SD.Rows(0).Item(16) = DblParser(PSDBefore15)
                SD.Rows(0).Item(17) = DblParser(PSDAfter15)
                SD.Rows(0).Item(18) = DblParser(AppendixIncomeTotal)
                SD.Rows(0).Item(19) = DblParser(AppendixIncomeTaxTotal)
                SD.Rows(0).Item(20) = DblParser(AppendixPSDTotal)

                result.Tables.Add(SD)
                result.Tables.Add(DD)

            Catch ex As Exception
                SD.Dispose()
                DD.Dispose()
                result.Dispose()
                Throw ex
            End Try

            Return result
        End Function

        Private Function FetchFR0573_3(ByVal criteria As Criteria) As DataSet

            Dim result As New DataSet
            result.Tables.Add(FetchGeneralDataTable)

            Dim SD As New DataTable("Specific")
            SD.Columns.Add("MunicipalityCode")
            SD.Columns.Add("Date")
            SD.Columns.Add("Year")
            SD.Columns.Add("MNPDTotal")
            SD.Columns.Add("MPNPDTotal")
            SD.Columns.Add("IncomeLabourTotal")
            SD.Columns.Add("IncomeOthersTotal")
            SD.Columns.Add("IncomeTotal")
            SD.Columns.Add("IncomeTaxLabourTotal")
            SD.Columns.Add("IncomeTaxOthersTotal")
            SD.Columns.Add("IncomeTaxTotal")

            SD.Rows.Add()

            SD.Rows(0).Item(0) = criteria.MunicipalityCode
            SD.Rows(0).Item(1) = criteria.Date.ToShortDateString
            SD.Rows(0).Item(2) = criteria.Year

            Dim DD As New DataTable("Details")
            DD.Columns.Add("Count")
            DD.Columns.Add("PersonCode")
            DD.Columns.Add("PersonName")
            DD.Columns.Add("MunicipalityCode")
            DD.Columns.Add("IncomeCode")
            DD.Columns.Add("MNPD")
            DD.Columns.Add("MPNPD")
            DD.Columns.Add("Income")
            DD.Columns.Add("IncomeTaxRate")
            DD.Columns.Add("IncomeTax")
            DD.Columns.Add("IsLabourRelated")

            Try

                Dim MNPDTotal As Double = 0
                Dim MPNPDTotal As Double = 0
                Dim IncomeLabourTotal As Double = 0
                Dim IncomeOthersTotal As Double = 0
                Dim IncomeTaxLabourTotal As Integer = 0
                Dim IncomeTaxOthersTotal As Integer = 0

                Dim myComm As New SQLCommand("FetchDeclarationFR0573(3)")
                myComm.AddParam("?DF", New Date(criteria.Year, 1, 1))
                myComm.AddParam("?DT", New Date(criteria.Year, 12, 31))
                Using myData As DataTable = myComm.Fetch

                    ClearDatatable(myData, 0)

                    Dim i As Integer = 0
                    For Each dr As DataRow In myData.Rows
                        DD.Rows.Add()
                        DD.Rows(i).Item(0) = i + 1
                        DD.Rows(i).Item(1) = dr.Item(0).ToString
                        DD.Rows(i).Item(2) = dr.Item(1).ToString.ToUpper
                        DD.Rows(i).Item(3) = criteria.MunicipalityCode
                        If dr.Item(8).ToString.Trim.ToLower = "t" Then
                            DD.Rows(i).Item(4) = GetMinLengthString(criteria.DeclarationItemCode, 2, "0"c)
                            DD.Rows(i).Item(10) = "X"
                            IncomeLabourTotal = IncomeLabourTotal + CDbl(dr.Item(6))
                            IncomeTaxLabourTotal = IncomeTaxLabourTotal + CInt(CRound(CDbl(dr.Item(7)), 0))
                        Else
                            DD.Rows(i).Item(4) = GetMinLengthString(dr.Item(3).ToString.Trim, 2, "0"c)
                            DD.Rows(i).Item(10) = ""
                            IncomeOthersTotal = IncomeOthersTotal + CDbl(dr.Item(6))
                            IncomeTaxOthersTotal = IncomeTaxOthersTotal + CInt(CRound(CDbl(dr.Item(7)), 0))
                        End If
                        DD.Rows(i).Item(5) = DblParser(CDbl(dr.Item(4)))
                        DD.Rows(i).Item(6) = DblParser(CDbl(dr.Item(5)))
                        DD.Rows(i).Item(7) = DblParser(CDbl(dr.Item(6)))
                        DD.Rows(i).Item(8) = CInt(dr.Item(2))
                        DD.Rows(i).Item(9) = CInt(CRound(CDbl(dr.Item(7)), 0))

                        MNPDTotal = MNPDTotal + CDbl(dr.Item(4))
                        MPNPDTotal = MPNPDTotal + CDbl(dr.Item(5))

                        i += 1
                    Next

                End Using

                SD.Rows(0).Item(3) = DblParser(MNPDTotal)
                SD.Rows(0).Item(4) = DblParser(MPNPDTotal)
                SD.Rows(0).Item(5) = DblParser(IncomeLabourTotal)
                SD.Rows(0).Item(6) = DblParser(IncomeOthersTotal)
                SD.Rows(0).Item(7) = DblParser(IncomeLabourTotal + IncomeOthersTotal)
                SD.Rows(0).Item(8) = IncomeTaxLabourTotal
                SD.Rows(0).Item(9) = IncomeTaxOthersTotal
                SD.Rows(0).Item(10) = IncomeTaxLabourTotal + IncomeTaxOthersTotal

                result.Tables.Add(SD)
                result.Tables.Add(DD)

            Catch ex As Exception
                SD.Dispose()
                DD.Dispose()
                result.Dispose()
                Throw ex
            End Try

            Return result

        End Function

        Private Function FetchSAM_2(ByVal criteria As Criteria) As DataSet

            Dim result As New DataSet
            result.Tables.Add(FetchGeneralDataTable)

            Dim SD As New DataTable("Specific")
            SD.Columns.Add("Date")
            SD.Columns.Add("SODRADepartment")
            SD.Columns.Add("Year")
            SD.Columns.Add("Month")
            SD.Columns.Add("Tarif")
            SD.Columns.Add("3SDPageCount")
            SD.Columns.Add("TotalWorkerCount")
            SD.Columns.Add("TotalIncome")
            SD.Columns.Add("TotalPayments")

            SD.Rows.Add()

            SD.Rows(0).Item(0) = criteria.Date.ToShortDateString
            SD.Rows(0).Item(1) = criteria.SODRADepartment
            SD.Rows(0).Item(2) = criteria.Year
            SD.Rows(0).Item(3) = criteria.Month

            Dim DD As New DataTable("Details")
            DD.Columns.Add("Count")
            DD.Columns.Add("PersonCode")
            DD.Columns.Add("SODRACodeSerial")
            DD.Columns.Add("SODRACode")
            DD.Columns.Add("PersonName")
            DD.Columns.Add("Income")
            DD.Columns.Add("Payment")

            Try

                Dim myComm As SQLCommand

                Dim CurrentRate As Double = criteria.SODRARate

                If criteria.DeclarationType = ApskaitaObjects.DeclarationType.SAM_2 Then

                    myComm = New SQLCommand("FetchDeclarationSAM(1)_2")
                    myComm.AddParam("?YR", criteria.Year)
                    myComm.AddParam("?MF", criteria.Month)
                    myComm.AddParam("?MT", criteria.Month)

                    Using myData As DataTable = myComm.Fetch
                        ClearDatatable(myData, 0)

                        If myData.Rows.Count > 1 Then
                            _Warning = "DĖMESIO. Pasirinktą mėnesį buvo taikomi skirtingi " _
                            & "SODROS ir PSD tarifai: "
                            For Each dr As DataRow In myData.Rows
                                _Warning = _Warning & vbCrLf & "SODRA išskaičiuota - " & dr.Item(0).ToString _
                                & "; PSD isšskaičiuota - " & dr.Item(2).ToString & "; SODRA Priskaičiuota - " _
                                & dr.Item(1).ToString & "PSD priskaičiuota - " & dr.Item(3).ToString & ";"
                            Next
                            _Warning = _Warning & vbCrLf & "Deklaracijoje naudojama tik pirmoji tarifų kombinacija."

                        ElseIf myData.Rows.Count < 1 Then

                            myData.Rows.Add()
                            myData.Rows(0).Item(0) = GetCurrentCompany.Rates.GetRate(RateType.SodraEmployee)
                            myData.Rows(0).Item(1) = GetCurrentCompany.Rates.GetRate(RateType.SodraEmployer)
                            myData.Rows(0).Item(2) = GetCurrentCompany.Rates.GetRate(RateType.PsdEmployee)
                            myData.Rows(0).Item(3) = GetCurrentCompany.Rates.GetRate(RateType.PsdEmployer)

                            _Warning = "DĖMESIO. Pasirinktą mėnesį nebuvo apskaitoje registruotų " _
                            & "darbo užmokesčio žiniaraščių, iš kurių būtų matomi pasirinktą mėnesį " _
                            & "faktiškai taikyti SODROS įmokų tarifai. Deklaracijoje naudojami einamieji tarifai."

                        End If

                        If criteria.Year < 2009 Then
                            CurrentRate = CRound(CDblSafe(myData.Rows(0).Item(0), 2, 0) _
                                + CDblSafe(myData.Rows(0).Item(1), 2, 0), 2)
                            SD.Rows(0).Item(4) = DblParser(CDbl(myData.Rows(0).Item(0)) + CDbl(myData.Rows(0).Item(1)))
                        Else
                            CurrentRate = CRound(CDblSafe(myData.Rows(0).Item(0), 2, 0) _
                                + CDblSafe(myData.Rows(0).Item(1), 2, 0) _
                                + CDblSafe(myData.Rows(0).Item(2), 2, 0) _
                                + CDblSafe(myData.Rows(0).Item(3), 2, 0), 2)
                            SD.Rows(0).Item(4) = DblParser(CDbl(myData.Rows(0).Item(0)) + CDbl(myData.Rows(0).Item(1)) _
                                + CDbl(myData.Rows(0).Item(2)) + CDbl(myData.Rows(0).Item(3)))
                        End If

                    End Using

                End If

                SD.Rows(0).Item(4) = DblParser(CurrentRate)

                If criteria.DeclarationType = ApskaitaObjects.DeclarationType.SAM_2 Then
                    myComm = New SQLCommand("FetchDeclarationSAM(1)_1")
                    myComm.AddParam("?YR", criteria.Year)
                    myComm.AddParam("?MF", criteria.Month)
                    myComm.AddParam("?MT", criteria.Month)
                    myComm.AddParam("?DT", New Date(criteria.Year, criteria.Month, _
                        Date.DaysInMonth(criteria.Year, criteria.Month)))
                Else
                    myComm = New SQLCommand("FetchDeclarationSAM(3)_1")
                    myComm.AddParam("?YR", criteria.Year)
                    myComm.AddParam("?MN", criteria.Month)
                    myComm.AddParam("?RT", criteria.SODRARate)
                    myComm.AddParam("?DT", New Date(criteria.Year, criteria.Month, _
                        Date.DaysInMonth(criteria.Year, criteria.Month)))
                End If

                Dim TotalIncome As Double = 0
                Dim TotalPayments As Double = 0
                Dim i As Integer = 0
                Using myData As DataTable = myComm.Fetch
                    For Each dr As DataRow In myData.Rows
                        DD.Rows.Add()
                        DD.Rows(i).Item(0) = i + 1
                        DD.Rows(i).Item(1) = dr.Item(1).ToString.Trim
                        DD.Rows(i).Item(2) = dr.Item(2).ToString.Trim.Substring(0, 2)
                        DD.Rows(i).Item(3) = GetNumericPart(dr.Item(2).ToString.Trim)
                        DD.Rows(i).Item(4) = dr.Item(0).ToString.Trim.ToUpper
                        DD.Rows(i).Item(5) = DblParser(CDblSafe(dr.Item(3), 2, 0))
                        DD.Rows(i).Item(6) = DblParser(CDblSafe(dr.Item(4), 2, 0))

                        TotalIncome = CRound(TotalIncome + CDblSafe(dr.Item(3), 2, 0), 2)
                        TotalPayments = CRound(TotalPayments + CDblSafe(dr.Item(4), 2, 0), 2)
                        i += 1
                    Next
                End Using

                SD.Rows(0).Item(5) = Math.Ceiling(DD.Rows.Count / 9)
                SD.Rows(0).Item(6) = DD.Rows.Count
                SD.Rows(0).Item(7) = DblParser(TotalIncome)
                SD.Rows(0).Item(8) = DblParser(TotalPayments)

                result.Tables.Add(SD)
                result.Tables.Add(DD)

            Catch ex As Exception
                SD.Dispose()
                DD.Dispose()
                result.Dispose()
                Throw ex
            End Try

            Return result

        End Function


        ''' <summary>
        ''' Changes Null (DBNull) values in the datatable to 
        ''' the passed DBNull object (string, double or integer).
        ''' </summary>
        Private Shared Sub ClearDatatable(ByRef myData As DataTable, ByVal DBNull As Object)
            For i As Integer = 1 To myData.Rows.Count
                For j As Integer = 1 To myData.Columns.Count
                    If IsDBNull(myData.Rows(i - 1).Item(j - 1)) _
                        OrElse myData.Rows(i - 1).Item(j - 1) Is Nothing Then
                        If Not myData.Columns(j - 1).DataType Is GetType(Date) AndAlso _
                            Not myData.Columns(j - 1).DataType Is GetType(DateTime) Then
                            myData.Rows(i - 1).Item(j - 1) = DBNull
                        End If
                    End If
                Next
            Next
        End Sub

#End Region

    End Class

End Namespace