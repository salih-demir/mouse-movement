Imports System.Windows.Media.Animation
Imports System.Runtime.InteropServices

Class MainWindow
    Dim MoveCursorAction As New CursorAction()
    Dim AnimationStoryboard As New Storyboard
    Dim TotalTimePassed As Integer = 0
    Public Sub New()
        InitializeComponent()
        Dim AnimationList As New List(Of CustomPointAnimation)
        AnimationList.Add(CreatePointAnimation(New Point(33, 65), New Point(453, 543), 1500))
        AnimationList.Add(CreatePointAnimation(New Point(453, 543), New Point(1000, 543), 1500, True))
        For index As Integer = 0 To AnimationList.Count - 1
            AnimationStoryboard.Children.Add(AnimationList.Item(index))
            Storyboard.SetTarget(AnimationList.Item(index), MoveCursorAction)
            Storyboard.SetTargetProperty(AnimationList.Item(index), New PropertyPath(CursorAction.PositionProperty))
        Next
        AnimationStoryboard.Begin()
    End Sub
    Public Function CreatePointAnimation(ByVal Start As Point, ByVal Destination As Point, ByVal InMilliseconds As Integer, Optional ByVal ClickAfter As Boolean = False)
        Dim NewPointAnimation As New CustomPointAnimation() With {.From = Start, .To = Destination, .Duration = New Duration(TimeSpan.FromMilliseconds(InMilliseconds))}
        NewPointAnimation.EasingFunction = New CubicEase
        NewPointAnimation.BeginTime = TimeSpan.FromMilliseconds(TotalTimePassed)
        NewPointAnimation.ClickAfter = ClickAfter
        TotalTimePassed += InMilliseconds
        Return NewPointAnimation
    End Function
End Class
Public Class CustomPointAnimation
    Inherits PointAnimation
    Public Property ClickAfter = False
    Private Sub CustomPointAnimation_Completed(sender As Object, e As EventArgs) Handles Me.Completed
        If ClickAfter Then
            CursorAction.MouseEvent(CursorAction.MouseEventLeftDown, 0, 0, 0, 0)
            CursorAction.MouseEvent(CursorAction.MouseEventLeftUp, 0, 0, 0, 0)
        End If
    End Sub
End Class
Public Class CursorAction
    Inherits FrameworkElement
    Private PositionX As Integer
    Private PositionY As Integer
    Public Const MouseEventRightDown = &H8
    Public Const MouseEventRightUp = &H1
    Public Const MouseEventAbsoluteMove = &H8000
    Public Const MouseEventLeftDown = &H2
    Public Const MouseEventLeftUp = &H4
    Public Const MouseEventMove = &H1
    Public Const MouseEventMiddleDown = &H20
    Public Const MouseEventMiddleUp = &H40
    Public Shared PositionProperty As DependencyProperty
    Private Shared DefaultPosition As Point = New Point(0, 0)
    Declare Sub SetCursorPos Lib "user32" Alias "SetCursorPos" (X As Integer, Y As Integer)
    Declare Sub MouseEvent Lib "user32" Alias "mouse_event" (ByVal dwFlags As Integer, ByVal dx As Integer, ByVal dy As Integer, ByVal cButtons As Integer, ByVal dwExtraInfo As Integer)
    Sub New()
        CursorAction.PositionProperty = DependencyProperty.Register("Position", GetType(Point), GetType(CursorAction), New PropertyMetadata(DefaultPosition, New PropertyChangedCallback(AddressOf OnPositionChanged)), New ValidateValueCallback(AddressOf CheckIfLegal))
    End Sub
    Private Sub OnPositionChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
        PositionX = CType(e.NewValue, Point).X
        PositionY = CType(e.NewValue, Point).Y
        SetCursorPos(PositionX, PositionY)
    End Sub
    Private Shared Function CheckIfLegal(ByVal value As Object) As Boolean
        If value.GetType <> GetType(Point) Then
            Return False
        Else
            Return True
        End If
    End Function
End Class

<StructLayout(LayoutKind.Sequential)> _
Friend Structure Win32Point
    Public X As Int32
    Public Y As Int32
End Structure