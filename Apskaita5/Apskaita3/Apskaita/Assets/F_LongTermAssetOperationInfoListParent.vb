Imports ApskaitaObjects.Assets
Public Class F_LongTermAssetOperationInfoListParent
    Implements ISupportsPrinting, IObjectEditForm

    Private Obj As LongTermAssetOperationInfoListParent
    Private Loading As Boolean = True
    Private LongTermAssetID As Integer = -1
    Private nOperationListChanged As Boolean = False


    Public ReadOnly Property ObjectID() As Integer Implements IObjectEditForm.ObjectID
        Get
            If Not Obj Is Nothing Then Return Obj.ID
            Return 0
        End Get
    End Property

    Public ReadOnly Property ObjectType() As System.Type Implements IObjectEditForm.ObjectType
        Get
            Return GetType(LongTermAssetOperationInfoListParent)
        End Get
    End Property


    Public Sub New(ByVal nLongTermAssetID As Integer)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        LongTermAssetID = nLongTermAssetID

    End Sub


    Public Sub NotifyAboutOperationListChanges(ByVal nAssetID As Integer)
        If nAssetID = Obj.ID Then nOperationListChanged = True
    End Sub


    Private Sub F_LongTermAssetOperationInfoListParent_Activated(ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles Me.Activated
        If Me.WindowState = FormWindowState.Maximized AndAlso MyCustomSettings.AutoSizeForm Then _
            Me.WindowState = FormWindowState.Normal
        If nOperationListChanged Then
            nOperationListChanged = False
            If Not YesOrNo("Pasikeitė operacijų su šiuo turtu duomenys. Atnaujinti sąrašą?") Then Exit Sub
            RefreshButton.PerformClick()
        End If
    End Sub

    Private Sub F_LongTermAssetOperationInfoListParent_FormClosing(ByVal sender As Object, _
        ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        GetDataGridViewLayOut(OperationListDataGridView)
        GetFormLayout(Me)
    End Sub

    Private Sub F_LongTermAssetOperationInfoListParent_Load(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles MyBase.Load

        If Not LongTermAssetOperationInfoListParent.CanGetObject Then
            MsgBox("Klaida. Jūsų teisių nepakanka šiai informacijai gauti.", _
                MsgBoxStyle.Exclamation, "Klaida.")
            DisableAllControls(Me)
            Exit Sub
        End If

        Try
            Obj = LoadObject(Of LongTermAssetOperationInfoListParent)(Nothing, _
                "GetLongTermAssetOperationInfoListParent", True, LongTermAssetID)
        Catch ex As Exception
            ShowError(ex)
            Exit Sub
        End Try

        LongTermAssetOperationInfoListParentBindingSource.DataSource = Obj

        AddDGVColumnSelector(OperationListDataGridView)

        SetDataGridViewLayOut(OperationListDataGridView)
        SetFormLayout(Me)

        InitializeMenu(Of LongTermAssetOperationInfo)()

    End Sub


    Private Sub RefreshButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles RefreshButton.Click

        Using bm As New BindingsManager(LongTermAssetOperationInfoListParentBindingSource, _
            OperationListBindingSource, Nothing, False, True)

            Try
                Obj = LoadObject(Of LongTermAssetOperationInfoListParent)(Nothing, _
                    "GetLongTermAssetOperationInfoListParent", True, LongTermAssetID)
            Catch ex As Exception
                ShowError(ex)
                Exit Sub
            End Try

            bm.SetNewDataSource(Obj)

        End Using

    End Sub

    Private Sub NewOperationButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles NewOperationButton.Click
        NewItem(Nothing)
    End Sub


    Private Sub InitializeMenu(Of T As LongTermAssetOperationInfo)()

        Dim w As New ToolStripHelper(Of T)(OperationListDataGridView, _
            ContextMenuStrip1, "", True)

        w.AddMenuItemHandler(ChangeItem_MenuItem, New DelegateContainer(Of T)(AddressOf ChangeItem))
        w.AddMenuItemHandler(DeleteItem_MenuItem, New DelegateContainer(Of T)(AddressOf DeleteItem))
        w.AddMenuItemHandler(NewItem_MenuItem, New DelegateContainer(Of T)(AddressOf NewItem))

        w.AddButtonHandler("Keisti", "Keisti IT operacijos duomenis.", _
            New DelegateContainer(Of T)(AddressOf ChangeItem))
        w.AddButtonHandler("Ištrinti", "Pašalinti IT operacijos duomenis iš duomenų bazės.", _
            New DelegateContainer(Of T)(AddressOf DeleteItem))

    End Sub

    Private Sub ChangeItem(ByVal item As LongTermAssetOperationInfo)
        If item Is Nothing Then Exit Sub
        MDIParent1.LaunchForm(GetType(F_LongTermAssetOperation), False, False, item.ID, item.ID, False, False)
    End Sub

    Private Sub DeleteItem(ByVal item As LongTermAssetOperationInfo)

        If item Is Nothing Then Exit Sub

        For Each frm As Form In MDIParent1.MdiChildren
            If TypeOf frm Is F_LongTermAssetOperation AndAlso DirectCast(frm, F_LongTermAssetOperation).ObjectID = item.ID Then
                MsgBox("Negalima pašalinti duomenų, kol jie yra redaguojami. Uždarykite redagavimo formą.", _
                    MsgBoxStyle.Exclamation, "Klaida")
                frm.Activate()
                Exit Sub
            End If
        Next

        If Not YesOrNo("Ar tikrai norite pašalinti pasirinktos IT operacijos " _
            & "duomenis iš apskaitos?") Then Exit Sub

        Try
            Using busy As New StatusBusy
                LongTermAssetOperation.DeleteLongTermAssetOperation(item.ID)
            End Using
        Catch ex As Exception
            ShowError(ex)
            Exit Sub
        End Try

        MsgBox("Ilgalaikio turto operacijos duomenys sėkmingai pašalinti iš apskaitos.", _
            MsgBoxStyle.Information, "Info")

        RefreshButton.PerformClick()

    End Sub

    Private Sub NewItem(ByVal item As LongTermAssetOperationInfo)
        MDIParent1.LaunchForm(GetType(F_LongTermAssetOperation), False, False, 0, Obj.ID, True, False)
    End Sub


    Public Function GetMailDropDownItems() As System.Windows.Forms.ToolStripDropDown _
        Implements ISupportsPrinting.GetMailDropDownItems
        Return Nothing
    End Function

    Public Function GetPrintDropDownItems() As System.Windows.Forms.ToolStripDropDown _
        Implements ISupportsPrinting.GetPrintDropDownItems
        Return Nothing
    End Function

    Public Function GetPrintPreviewDropDownItems() As System.Windows.Forms.ToolStripDropDown _
        Implements ISupportsPrinting.GetPrintPreviewDropDownItems
        Return Nothing
    End Function

    Public Sub OnMailClick(ByVal sender As Object, ByVal e As System.EventArgs) _
        Implements ISupportsPrinting.OnMailClick
        If Obj Is Nothing Then Exit Sub

        Using frm As New F_SendObjToEmail(Obj, 0)
            frm.ShowDialog()
        End Using

    End Sub

    Public Sub OnPrintClick(ByVal sender As Object, ByVal e As System.EventArgs) _
        Implements ISupportsPrinting.OnPrintClick
        If Obj Is Nothing Then Exit Sub
        Try
            PrintObject(Obj, False, 0)
        Catch ex As Exception
            ShowError(ex)
        End Try
    End Sub

    Public Sub OnPrintPreviewClick(ByVal sender As Object, ByVal e As System.EventArgs) _
        Implements ISupportsPrinting.OnPrintPreviewClick
        If Obj Is Nothing Then Exit Sub
        Try
            PrintObject(Obj, True, 0)
        Catch ex As Exception
            ShowError(ex)
        End Try
    End Sub

    Public Function SupportsEmailing() As Boolean _
        Implements ISupportsPrinting.SupportsEmailing
        Return True
    End Function

End Class