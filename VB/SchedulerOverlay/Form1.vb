Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Reflection
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports DevExpress.Skins
Imports DevExpress.Utils.Drawing
Imports DevExpress.XtraScheduler
Imports DevExpress.XtraScheduler.Drawing
Imports DevExpress.XtraScheduler.Native
Imports DevExpress.XtraTab

Namespace SchedulerOverlay
    Partial Public Class Form1
        Inherits Form

        Private schedulerControl As SchedulerControl
        Public Sub New()
            InitializeComponent()

            schedulerControl = New SchedulerControl(schedulerStorage)
            AddHandler schedulerControl.CustomDrawAppointmentBackground, AddressOf schedulerControl_CustomDrawAppointmentBackground
            schedulerControl.Dock = DockStyle.Fill

            InitResources(schedulerStorage)
            InitAppointments(schedulerStorage)

            AddHandler xtraTabControl1.SelectedPageChanged, AddressOf xtraTabControl1_SelectedPageChanged
            xtraTabControl1.SelectedTabPage.Controls.Add(schedulerControl)

        End Sub
        Private Sub InitResources(ByVal storage As SchedulerStorage)
            For i As Integer = 0 To 4
                Dim resourceName As String = "Resource" & i
                storage.Resources.Add(storage.CreateResource(i, resourceName))
                InitTabs(resourceName, i)
            Next i
        End Sub
        Private Sub InitTabs(ByVal resourceName As String, ByVal id As Integer)
            If xtraTabControl1.TabPages.Count <= id Then
                xtraTabControl1.TabPages.Add()
            End If
            xtraTabControl1.TabPages(id).Text = resourceName
        End Sub

        Private Sub xtraTabControl1_SelectedPageChanged(ByVal sender As Object, ByVal e As TabPageChangedEventArgs)
            e.Page.Controls.Add(schedulerControl)
        End Sub

        Private Sub schedulerControl_CustomDrawAppointmentBackground(ByVal sender As Object, ByVal e As CustomDrawObjectEventArgs)
            Dim schedulerControl As SchedulerControl = TryCast(sender, SchedulerControl)
            Dim info As AppointmentViewInfo = TryCast(e.ObjectInfo, AppointmentViewInfo)
            Dim tabResourceName As String = (TryCast(schedulerControl.Parent, XtraTabPage)).Text
            Dim appointmentResourceName As String = String.Empty
            If Not (TypeOf info.Appointment.ResourceId Is EmptyResourceId) Then
                appointmentResourceName = schedulerStorage.Resources(CInt((info.Appointment.ResourceId))).Caption
            End If
            e.DrawDefault()
            If appointmentResourceName <> tabResourceName Then
                Dim color As Color = schedulerStorage.Appointments.Labels.GetById(info.Appointment.LabelKey).Color
                Dim lightColor As Color = ControlPaint.LightLight(color)
                Dim rect As New Rectangle(e.Bounds.X + 2, e.Bounds.Y + 2, e.Bounds.Width - 4, e.Bounds.Height - 4)
                e.Graphics.FillRectangle(New SolidBrush(lightColor), rect)
            End If

            e.Handled = True
        End Sub
        Private Sub InitAppointments(ByVal storage As SchedulerStorage)
            For i As Integer = 0 To storage.Resources.Count - 1
                Dim resource As Resource = storage.Resources(i)
                Dim subj As String = resource.Caption & "'s "
                Dim appointment1 As Appointment = storage.CreateAppointment(AppointmentType.Normal, Date.Today.AddHours(10), Date.Today.AddHours(11), subj & "birthday")
                appointment1.LabelKey = 8
                appointment1.StatusKey = 3
                appointment1.ResourceId = i
                storage.Appointments.Add(appointment1)

                Dim appointment2 As Appointment = storage.CreateAppointment(AppointmentType.Normal, Date.Today.AddHours(13), Date.Today.AddHours(14), subj & "meeting")
                appointment2.LabelKey = 2
                appointment2.StatusKey = 2
                appointment2.ResourceId = i
                storage.Appointments.Add(appointment2)

                Dim appointment3 As Appointment = storage.CreateAppointment(AppointmentType.Normal, Date.Today.AddHours(15), Date.Today.AddHours(16), subj & "phone call")
                appointment3.LabelKey = 10
                appointment3.StatusKey = 0
                appointment3.ResourceId = i
                storage.Appointments.Add(appointment3)
            Next i
        End Sub

    End Class
End Namespace
