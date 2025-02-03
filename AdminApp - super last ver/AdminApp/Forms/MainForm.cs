
using System;
using System.Windows.Forms;
using AdminApp.DB;
using Microsoft.Extensions.DependencyInjection;
using AdminApp.Services;

namespace AdminApp.Forms
{
    public class MainForm : Form
    {
        private Button btnViewUsers;
        private Button btnAddUser;
        private Button btnManageTrainings;
        private Button btnManageTrainers;
        private Button btnManageSubscriptions;
        private Button btnManageGymAttendance;
        private readonly IServiceProvider _serviceProvider;
    
        public MainForm(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeComponents();
        }
    
        private void InitializeComponents()
        {
            this.Text = "Administrator Panel";
            this.Size = new System.Drawing.Size(400, 500);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
    
            btnViewUsers = new Button()
            {
                Text = "View Users",
                Top = 50,
                Width = 300,
                Height = 50,
                Left = 50
            };
            btnViewUsers.Click += (s, e) => OpenUserList();
    
            btnAddUser = new Button()
            {
                Text = "Add User",
                Top = btnViewUsers.Top + 70,
                Width = 300,
                Height = 50,
                Left = 50
            };
            btnAddUser.Click += (s, e) => OpenAddUserForm();
    
            btnManageTrainings = new Button()
            {
                Text = "Manage Trainings",
                Top = btnAddUser.Top + 70,
                Width = 300,
                Height = 50,
                Left = 50
            };
            btnManageTrainings.Click += (s, e) => OpenTrainingListForm();
    
            btnManageTrainers = new Button()
            {
                Text = "Manage Trainers",
                Top = btnManageTrainings.Top + 70,
                Width = 300,
                Height = 50,
                Left = 50
            };
            btnManageTrainers.Click += (s, e) => OpenTrainerListForm();
    
            btnManageSubscriptions = new Button()
            {
                Text = "Manage Subscriptions",
                Top = btnManageTrainers.Top + 70,
                Width = 300,
                Height = 50,
                Left = 50
            };
            btnManageSubscriptions.Click += (s, e) => OpenSubscriptionListForm();
    
            btnManageGymAttendance = new Button()
            {
                Text = "Gym Attendance Management",
                Top = btnManageSubscriptions.Top + 70,
                Width = 300,
                Height = 50,
                Left = 50
            };
            btnManageGymAttendance.Click += (s, e) => OpenGymAttendanceMenu();
    
            this.Controls.Add(btnViewUsers);
            this.Controls.Add(btnAddUser);
            this.Controls.Add(btnManageTrainings);
            this.Controls.Add(btnManageTrainers);
            this.Controls.Add(btnManageSubscriptions);
            this.Controls.Add(btnManageGymAttendance);
        }
    
        private void OpenGymAttendanceMenu()
        {
            var gymVisitService = _serviceProvider.GetService<GymVisitService>()
                                  ?? new GymVisitService(new DatabaseManager("mongodb+srv://Admin:strongpassword@cluster0.lrajj.mongodb.net", "GymDatabase"));
            var form = new GymAttendanceMenuForm(gymVisitService);
            form.ShowDialog();
        }
        
        private void OpenUserList()
        {
            var userListForm = _serviceProvider.GetRequiredService<UserListForm>();
            userListForm.ShowDialog();
        }
    
        private void OpenAddUserForm()
        {
            var addUserForm = _serviceProvider.GetRequiredService<AddUserForm>();
            addUserForm.ShowDialog();
        }
    
        private void OpenTrainingListForm()
        {
            var trainingListForm = _serviceProvider.GetRequiredService<TrainingListForm>();
            trainingListForm.ShowDialog();
        }
    
        private void OpenTrainerListForm()
        {
            var trainerListForm = _serviceProvider.GetRequiredService<TrainerListForm>();
            trainerListForm.ShowDialog();
        }
    
        private void OpenSubscriptionListForm()
        {
            var subscriptionService = _serviceProvider.GetService<SubscriptionService>() ?? new SubscriptionService();
            var subscriptionListForm = new SubscriptionListForm(subscriptionService);
            subscriptionListForm.ShowDialog();
        }
    }
}
