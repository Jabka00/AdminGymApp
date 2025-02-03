
using System;
using System.Windows.Forms;
using AdminApp.Services;

namespace AdminApp.Forms
{
    public class GymAttendanceMenuForm : Form
    {
        private readonly GymVisitService _gymVisitService;
        private Button btnCheckIn;
        private Button btnCheckOut;
        private Button btnHistory;

        public GymAttendanceMenuForm(GymVisitService gymVisitService)
        {
            _gymVisitService = gymVisitService;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Gym Attendance Management";
            this.Size = new System.Drawing.Size(400, 300);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            btnCheckIn = new Button() { Text = "Check In", Top = 30, Left = 50, Width = 300, Height = 40 };
            btnCheckOut = new Button() { Text = "Check Out", Top = 90, Left = 50, Width = 300, Height = 40 };
            btnHistory = new Button() { Text = "Visit History", Top = 150, Left = 50, Width = 300, Height = 40 };

            btnCheckIn.Click += (s, e) =>
            {
                var form = new GymCheckInForm(_gymVisitService);
                form.ShowDialog();
            };

            btnCheckOut.Click += (s, e) =>
            {
                var form = new GymCheckOutForm(_gymVisitService);
                form.ShowDialog();
            };

            btnHistory.Click += (s, e) =>
            {
                var form = new GymVisitHistoryForm(_gymVisitService);
                form.ShowDialog();
            };

            this.Controls.Add(btnCheckIn);
            this.Controls.Add(btnCheckOut);
            this.Controls.Add(btnHistory);
        }
    }
}
