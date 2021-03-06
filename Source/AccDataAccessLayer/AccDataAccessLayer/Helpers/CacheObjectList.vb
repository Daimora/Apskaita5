Imports System.Reflection
<Serializable()> _
Public Class CacheObjectList
    Inherits ReadOnlyListBase(Of CacheObjectList, Object)

#Region " Business Methods "

#End Region

#Region " Factory Methods "

    Public Shared Sub GetList(ByVal nBaseType As Type())

        If nBaseType Is Nothing OrElse nBaseType.Length < 1 Then Exit Sub

        Dim InvalidatedTypeList As New List(Of Type)

        For Each t As Type In nBaseType
            If Not InvalidatedTypeList.Contains(t) AndAlso CacheManager.CacheIsInvalidated(t) Then _
                InvalidatedTypeList.Add(t)
        Next

        If InvalidatedTypeList.Count < 1 Then Exit Sub

        Dim result As CacheObjectList = DataPortal.Fetch(Of CacheObjectList) _
            (New Criteria(InvalidatedTypeList.ToArray))

        For Each item As Object In result
            If item Is Nothing Then Throw New Exception("Klaida. Nepavyko gauti kešuojamų sąrašų.")
            CacheManager.AddCacheItem(item.GetType, item, Nothing)
        Next

    End Sub

    Private Sub New()
        ' require use of factory methods
    End Sub

#End Region

#Region " Data Access "

    <Serializable()> _
    Private Class Criteria
        Private _BaseType As Type()
        Public ReadOnly Property BaseType() As Type()
            Get
                Return _BaseType
            End Get
        End Property
        Public Sub New(ByVal nBaseType As Type())
            _BaseType = nBaseType
        End Sub
    End Class

    Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

        RaiseListChangedEvents = False
        IsReadOnly = False

        For Each t As Type In criteria.BaseType
            Try
                Dim MI As MethodInfo = t.GetMethod("GetListOnServer", _
                    BindingFlags.NonPublic Or BindingFlags.Static)
                Dim item As Object = MI.Invoke(Nothing, Nothing)
                If item Is Nothing Then Throw New Exception("Tipas '" & t.FullName _
                    & "' neimplementuoja private metodo GetListOnServer arba metodas grąžina null.")
                Add(item)
            Catch ex As Exception
                Throw New Exception("Klaida. Nepavyko gauti kešuojamo objekto, kurio tipas '" _
                    & t.FullName & "'. " & ex.Message, ex)
            End Try
        Next

        IsReadOnly = True
        RaiseListChangedEvents = True

    End Sub

#End Region

End Class