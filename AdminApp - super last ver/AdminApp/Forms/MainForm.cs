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
        private Button btnManageProducts;
        private Button btnViewPurchaseHistory;
        private readonly IServiceProvider _serviceProvider;
    
        public MainForm(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeComponents();
        }
    
        private void InitializeComponents()
        {
            this.Text = "Administrator Panel";
            this.Size = new System.Drawing.Size(400, 700);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
    
            int buttonWidth = 300;
            int buttonHeight = 50;
            int left = 50;
            int top = 20;
            int gap = 20;
    
            btnViewUsers = new Button()
            {
                Text = "View Users",
                Top = top,
                Width = buttonWidth,
                Height = buttonHeight,
                Left = left
            };
            btnViewUsers.Click += (s, e) => OpenUserList();
    
            btnAddUser = new Button()
            {
                Text = "Add User",
                Top = btnViewUsers.Bottom + gap,
                Width = buttonWidth,
                Height = buttonHeight,
                Left = left
            };
            btnAddUser.Click += (s, e) => OpenAddUserForm();
    
            btnManageTrainings = new Button()
            {
                Text = "Manage Trainings",
                Top = btnAddUser.Bottom + gap,
                Width = buttonWidth,
                Height = buttonHeight,
                Left = left
            };
            btnManageTrainings.Click += (s, e) => OpenTrainingListForm();
    
            btnManageTrainers = new Button()
            {
                Text = "Manage Trainers",
                Top = btnManageTrainings.Bottom + gap,
                Width = buttonWidth,
                Height = buttonHeight,
                Left = left
            };
            btnManageTrainers.Click += (s, e) => OpenTrainerListForm();
    
            btnManageSubscriptions = new Button()
            {
                Text = "Manage Subscriptions",
                Top = btnManageTrainers.Bottom + gap,
                Width = buttonWidth,
                Height = buttonHeight,
                Left = left
            };
            btnManageSubscriptions.Click += (s, e) => OpenSubscriptionListForm();
    
            btnManageGymAttendance = new Button()
            {
                Text = "Gym Attendance Management",
                Top = btnManageSubscriptions.Bottom + gap,
                Width = buttonWidth,
                Height = buttonHeight,
                Left = left
            };
            btnManageGymAttendance.Click += (s, e) => OpenGymAttendanceMenu();

            btnManageProducts = new Button()
            {
                Text = "Manage Shop Products",
                Top = btnManageGymAttendance.Bottom + gap,
                Width = buttonWidth,
                Height = buttonHeight,
                Left = left
            };
            btnManageProducts.Click += (s, e) => OpenProductListForm();

            btnViewPurchaseHistory = new Button()
            {
                Text = "View Purchase History",
                Top = btnManageProducts.Bottom + gap,
                Width = buttonWidth,
                Height = buttonHeight,
                Left = left
            };
            btnViewPurchaseHistory.Click += (s, e) => OpenPurchaseHistoryForm();
    
            this.Controls.Add(btnViewUsers);
            this.Controls.Add(btnAddUser);
            this.Controls.Add(btnManageTrainings);
            this.Controls.Add(btnManageTrainers);
            this.Controls.Add(btnManageSubscriptions);
            this.Controls.Add(btnManageGymAttendance);
            this.Controls.Add(btnManageProducts);
            this.Controls.Add(btnViewPurchaseHistory);
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
    
        private void OpenGymAttendanceMenu()
        {
            var gymVisitService = _serviceProvider.GetService<GymVisitService>()
                                  ?? new GymVisitService(new DatabaseManager("mongodb+srv://Admin:strongpassword@cluster0.lrajj.mongodb.net", "GymDatabase"));
            var form = new GymAttendanceMenuForm(gymVisitService);
            form.ShowDialog();
        }

        private void OpenProductListForm()
        {
            var productService = _serviceProvider.GetService<ProductService>()
                                 ?? new ProductService(new DatabaseManager("mongodb+srv://Admin:strongpassword@cluster0.lrajj.mongodb.net", "GymDatabase"));
            var productListForm = new ProductListForm(productService);
            productListForm.ShowDialog();
        }

        private void OpenPurchaseHistoryForm()
        {
            var purchaseService = _serviceProvider.GetService<PurchaseService>()
                                  ?? new PurchaseService(new DatabaseManager("mongodb+srv://Admin:strongpassword@cluster0.lrajj.mongodb.net", "GymDatabase"));
            var purchaseHistoryForm = new PurchaseHistoryForm(purchaseService);
            purchaseHistoryForm.ShowDialog();
        }
    }
}
