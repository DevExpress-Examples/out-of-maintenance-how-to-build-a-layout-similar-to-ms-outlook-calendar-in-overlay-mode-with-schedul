using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraTab;

namespace SchedulerOverlay
{
    public partial class Form1 : Form
    {
        SchedulerControl schedulerControl;
        public Form1()
        {
            InitializeComponent();

            schedulerControl = new SchedulerControl(schedulerStorage);
            schedulerControl.CustomDrawAppointmentBackground += schedulerControl_CustomDrawAppointmentBackground;
            schedulerControl.Dock = DockStyle.Fill;

            xtraTabControl1.SelectedPageChanged += xtraTabControl1_SelectedPageChanged;
            xtraTabControl1.SelectedTabPage.Controls.Add(schedulerControl);

            InitResources(schedulerStorage);
            InitAppointments(schedulerStorage);
            
        }
        private void InitResources(SchedulerStorage storage)
        {
            for (int i = 0; i < 5; i++)
            {
                string resourceName = "Resource" + i;
                storage.Resources.Add(new Resource(i, resourceName));
                InitTabs(resourceName, i);
            }
        }
        private void InitTabs(string resourceName, int id)
        {
            if (xtraTabControl1.TabPages.Count <= id)
            {
                xtraTabControl1.TabPages.Add();
            }
            xtraTabControl1.TabPages[id].Text = resourceName;
        }

        void xtraTabControl1_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
        {
            e.Page.Controls.Add(schedulerControl);
        }

        void schedulerControl_CustomDrawAppointmentBackground(object sender, CustomDrawObjectEventArgs e)
        {
            SchedulerControl schedulerControl = sender as SchedulerControl;
            AppointmentViewInfo info = e.ObjectInfo as AppointmentViewInfo;
            string tabResourceName = (schedulerControl.Parent as XtraTabPage).Text;
            string appointmentResourceName = String.Empty;
            if (!(info.Appointment.ResourceId is EmptyResource))
                appointmentResourceName = schedulerStorage.Resources[(int)info.Appointment.ResourceId].Caption;
            e.DrawDefault();
            if (appointmentResourceName != tabResourceName)
            {
                Color color = schedulerStorage.Appointments.Labels[info.Appointment.LabelId].Color;
                Color lightColor = ControlPaint.LightLight(color);
                Rectangle rect = new Rectangle(e.Bounds.X + 2, e.Bounds.Y + 2, e.Bounds.Width - 4, e.Bounds.Height - 4); 
                e.Graphics.FillRectangle(new SolidBrush(lightColor), rect);
            }
            
            e.Handled = true;
        }
        private void InitAppointments(SchedulerStorage storage)
        {
            for (int i = 0; i < storage.Resources.Count; i++)
            {
                Resource resource = storage.Resources[i];
                string subj = resource.Caption + "'s ";
                storage.Appointments.Add(new Appointment(AppointmentType.Normal, DateTime.Today.AddHours(10), DateTime.Today.AddHours(11), subj + "birthday") { LabelId = 8, StatusId = 3, ResourceId = i });
                storage.Appointments.Add(new Appointment(AppointmentType.Normal, DateTime.Today.AddHours(13), DateTime.Today.AddHours(14), subj + "meeting") { LabelId = 2, StatusId = 2, ResourceId = i });
                storage.Appointments.Add(new Appointment(AppointmentType.Normal, DateTime.Today.AddHours(15), DateTime.Today.AddHours(16), subj + "phone call") { LabelId = 10, StatusId = 0, ResourceId = i });
            }
        }

    }
}
