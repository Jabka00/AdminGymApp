using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using AdminApp.Models;
using AdminApp.Services;

namespace AdminApp.Forms
{
    public class SelectUserForSubscriptionForm : Form
    {
        private ComboBox cmbUsers;
        private Button btnManage;
        private readonly UserService _userService;
        private List<User> _users = new List<User>();

        public SelectUserForSubscriptionForm(UserService userService)
        {
            _userService = userService;
            InitializeComponents();
            LoadUsersAsync();
        }

        private void InitializeComponents()
        {
            this.Text = "Chose User";
            this.Size = new System.Drawing.Size(400, 150);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            var lbl = new Label() { Text = "User:", AutoSize = true, Left = 10, Top = 20 };
            cmbUsers = new ComboBox() { Left = 120, Top = 20, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            btnManage = new Button() { Text = "manage subscription", Left = 120, Top = 60, Width = 150 };
            btnManage.Click += BtnManage_Click;

            this.Controls.Add(lbl);
            this.Controls.Add(cmbUsers);
            this.Controls.Add(btnManage);
        }

        private async void LoadUsersAsync()
        {
            _users = await _userService.GetAllUsersAsync();
            cmbUsers.DataSource = _users;
            cmbUsers.DisplayMember = "Username";
            cmbUsers.ValueMember = "Id";
        }

        private void BtnManage_Click(object sender, EventArgs e)
        {
            if (cmbUsers.SelectedValue == null)
            {
                MessageBox.Show("Chose User.", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string userId = cmbUsers.SelectedValue.ToString();
            var subscriptionService = new SubscriptionService();
            var manageForm = new ManageSubscriptionForm(userId, subscriptionService);
            manageForm.ShowDialog();
        }
    }
}
