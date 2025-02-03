
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AdminApp.Models;
using AdminApp.Services;
using System.Collections.Generic;

namespace AdminApp.Forms
{
    public class EnrollUserForm : Form
    {
        private Label lblSelectUser;
        private ComboBox cmbUsers;
        private Label lblSearch;
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnSave;
        private Button btnCancel;
        private Label lblStatus;

        private readonly Training _training;
        private readonly TrainingService _trainingService;
        private readonly TrainerService _trainerService;
        private readonly UserService _userService;

        private List<User> _allUsers = new List<User>();

        public EnrollUserForm(Training training, TrainingService trainingService, TrainerService trainerService, UserService userService)
        {
            _training = training;
            _trainingService = trainingService;
            _trainerService = trainerService;
            _userService = userService;

            InitializeComponents();
            LoadUsersAsync();
        }

        private void InitializeComponents()
        {
            this.Text = $"Enroll User to '{_training.Title}'";
            this.Size = new System.Drawing.Size(450, 300);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            lblSelectUser = new Label()
            {
                Text = "Select a user:",
                Location = new System.Drawing.Point(20, 20),
                AutoSize = true
            };

            cmbUsers = new ComboBox()
            {
                Location = new System.Drawing.Point(20, 50),
                Width = 340,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            lblSearch = new Label()
            {
                Text = "Search user:",
                Location = new System.Drawing.Point(20, 90),
                AutoSize = true
            };

            txtSearch = new TextBox()
            {
                Location = new System.Drawing.Point(20, 120),
                Width = 240
            };
            txtSearch.KeyDown += TxtSearch_KeyDown;

            btnSearch = new Button()
            {
                Text = "Search",
                Location = new System.Drawing.Point(270, 118),
                Width = 90
            };
            btnSearch.Click += async (s, e) => await PerformSearchAsync();

            btnSave = new Button()
            {
                Text = "Save",
                Location = new System.Drawing.Point(200, 180),
                Width = 80
            };
            btnSave.Click += async (s, e) => await SaveEnrollmentAsync();

            btnCancel = new Button()
            {
                Text = "Cancel",
                Location = new System.Drawing.Point(290, 180),
                Width = 80
            };
            btnCancel.Click += (s, e) => this.Close();

            lblStatus = new Label()
            {
                Text = "",
                Location = new System.Drawing.Point(20, 220),
                AutoSize = true,
                ForeColor = System.Drawing.Color.Red
            };

            this.Controls.Add(lblSelectUser);
            this.Controls.Add(cmbUsers);
            this.Controls.Add(lblSearch);
            this.Controls.Add(txtSearch);
            this.Controls.Add(btnSearch);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
            this.Controls.Add(lblStatus);
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                _allUsers = await _userService.GetAllUsersAsync();
                UpdateUserComboBox(_allUsers);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void UpdateUserComboBox(List<User> users)
        {
            cmbUsers.DataSource = null;
            cmbUsers.DataSource = users;
            cmbUsers.DisplayMember = "Username";
            cmbUsers.ValueMember = "Id";
        }

        private async Task PerformSearchAsync()
        {
            string query = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(query))
            {
                UpdateUserComboBox(_allUsers);
                lblStatus.Text = "";
                return;
            }

            var filteredUsers = _allUsers.Where(u => u.Username.ToLower().Contains(query)).ToList();

            if (filteredUsers.Any())
            {
                UpdateUserComboBox(filteredUsers);
                lblStatus.Text = "";
            }
            else
            {
                lblStatus.Text = "No users found.";
                cmbUsers.DataSource = null;
            }
        }

        private async void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                await PerformSearchAsync();
            }
        }

        private async Task SaveEnrollmentAsync()
        {
            if (cmbUsers.SelectedValue == null)
            {
                lblStatus.Text = "Please select a user.";
                return;
            }

            string userId = cmbUsers.SelectedValue.ToString();

            try
            {
                bool success = await _trainingService.EnrollUserAsync(_training.Id!, userId);
                if (success)
                {
                    MessageBox.Show("User has been successfully enrolled to the training!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    lblStatus.Text = "Failed to enroll user. Possibly the training is full or the user is already enrolled.";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
            }
        }
    }
}