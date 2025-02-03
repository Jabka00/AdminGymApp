
using System;
using System.Windows.Forms;
using AdminApp.Models;
using AdminApp.Services;
using System.Linq;
using System.Collections.Generic;

namespace AdminApp.Forms
{
    public class EnrolledUsersForm : Form
    {
        private DataGridView? dgvUsers;
        private Label? lblTrainingTitle;
        private readonly Training _training;
        private readonly UserService _userService;

        public EnrolledUsersForm(Training training, UserService userService)
        {
            _training = training;
            _userService = userService;
            InitializeComponents();
            LoadEnrolledUsers();
        }

        private void InitializeComponents()
        {
            this.Text = "Enrolled Users";
            this.Size = new System.Drawing.Size(600, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            lblTrainingTitle = new Label()
            {
                Text = $"Training: {_training.Title}",
                Dock = DockStyle.Top,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold),
                Height = 40
            };

            dgvUsers = new DataGridView()
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = true,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            this.Controls.Add(dgvUsers);
            this.Controls.Add(lblTrainingTitle);
        }

        private async void LoadEnrolledUsers()
        {
            if (dgvUsers == null)
                return;

            try
            {
                var users = await _userService.GetAllUsersAsync();
                var enrolledUsers = users.Where(u => _training.EnrolledUsers.Contains(u.Id)).ToList();

                dgvUsers.DataSource = enrolledUsers.Select(u => new
                {
                    u.Username,
                    u.Email,
                    u.FirstName,
                    u.MiddleName,
                    u.LastName,
                    u.Phone,
                    u.Address
                }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}