
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Forms;
using AdminApp.Models;
using AdminApp.Services;

namespace AdminApp.Forms
{
    public class EditUserForm : Form
    {
        private readonly UserService _userService;
        private User _user;

        // Controls for editing user data
        private TextBox? txtUsername, txtEmail, txtPassword, txtFirstName, txtMiddleName, txtLastName, txtPhone, txtAddress;
        private ComboBox? cmbGender, cmbRole;
        private DateTimePicker? dtpDateOfBirth;
        // Button to manage subscription
        private Button? btnManageSubscription;
        // Save/Cancel
        private Button? btnSave, btnCancel;

        public EditUserForm(User user, UserService userService)
        {
            _user = user;
            _userService = userService;
            InitializeComponents();
            LoadUserData();
        }

        private void InitializeComponents()
        {
            this.Text = "Edit User";
            this.Size = new System.Drawing.Size(500, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            var tableLayout = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 11,
                Padding = new Padding(10),
                AutoScroll = true
            };
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            void AddRow(string labelText, Control control)
            {
                var lbl = new Label() { Text = labelText, Anchor = AnchorStyles.Right, AutoSize = true };
                tableLayout.Controls.Add(lbl);
                tableLayout.Controls.Add(control);
            }

            // Username (read-only)
            txtUsername = new TextBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right, ReadOnly = true };
            AddRow("Username:", txtUsername);

            // Email
            txtEmail = new TextBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            AddRow("Email:", txtEmail);

            // Password
            txtPassword = new TextBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right, PasswordChar = '*' };
            AddRow("Password:", txtPassword);

            // First Name
            txtFirstName = new TextBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            AddRow("First Name:", txtFirstName);

            // Middle Name
            txtMiddleName = new TextBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            AddRow("Middle Name:", txtMiddleName);

            // Last Name
            txtLastName = new TextBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            AddRow("Last Name:", txtLastName);

            // Date of Birth
            dtpDateOfBirth = new DateTimePicker() { Anchor = AnchorStyles.Left | AnchorStyles.Right, Format = DateTimePickerFormat.Short };
            AddRow("Date of Birth:", dtpDateOfBirth);

            // Gender
            cmbGender = new ComboBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbGender.Items.AddRange(new string[] { "Male", "Female", "Other" });
            cmbGender.SelectedIndex = 0;
            AddRow("Gender:", cmbGender);

            // Phone
            txtPhone = new TextBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            AddRow("Phone:", txtPhone);

            // Address
            txtAddress = new TextBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            AddRow("Address:", txtAddress);

            // Role
            cmbRole = new ComboBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRole.Items.AddRange(new string[] { "client", "admin" });
            cmbRole.SelectedIndex = 0;
            AddRow("Role:", cmbRole);

            // Manage Subscription
            btnManageSubscription = new Button() { Text = "Manage Subscription", Width = 200 };
            btnManageSubscription.Click += (s, e) =>
            {
                if (string.IsNullOrEmpty(_user.Id))
                {
                    MessageBox.Show("Failed to detect user ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // Example usage of SubscriptionService
                var subscriptionService = new SubscriptionService();
                var manageSubForm = new ManageSubscriptionForm(_user.Id, subscriptionService);
                manageSubForm.ShowDialog();
            };
            AddRow("Subscription:", btnManageSubscription);

            // Save/Cancel
            btnSave = new Button() { Text = "Save", Anchor = AnchorStyles.None, Width = 100 };
            btnSave.Click += async (s, e) => await SaveUser();

            btnCancel = new Button() { Text = "Cancel", Anchor = AnchorStyles.None, Width = 100 };
            btnCancel.Click += (s, e) => this.Close();

            var flowPanel = new FlowLayoutPanel() { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill };
            flowPanel.Controls.Add(btnCancel);
            flowPanel.Controls.Add(btnSave);
            tableLayout.Controls.Add(flowPanel, 1, tableLayout.RowCount - 1);

            this.Controls.Add(tableLayout);
        }

        private void LoadUserData()
        {
            if (txtUsername == null || txtEmail == null || txtPassword == null || txtFirstName == null ||
                txtMiddleName == null || txtLastName == null || dtpDateOfBirth == null || cmbGender == null ||
                txtPhone == null || txtAddress == null || cmbRole == null)
                return;

            txtUsername.Text = _user.Username;
            txtEmail.Text = _user.Email;
            txtPassword.Text = _user.Password; 
            txtFirstName.Text = _user.FirstName;
            txtMiddleName.Text = _user.MiddleName;
            txtLastName.Text = _user.LastName;
            dtpDateOfBirth.Value = _user.DateOfBirth != DateTime.MinValue ? _user.DateOfBirth : DateTime.Today;
            cmbGender.SelectedItem = _user.Gender;
            txtPhone.Text = _user.Phone;
            txtAddress.Text = _user.Address;
            cmbRole.SelectedItem = _user.Role;
        }

        private async System.Threading.Tasks.Task SaveUser()
        {
            if (txtEmail == null || txtPassword == null || txtFirstName == null ||
                txtLastName == null || txtPhone == null || txtAddress == null || cmbGender == null ||
                dtpDateOfBirth == null || cmbRole == null)
                return;

            if (string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Please fill in all required fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _user.Email = txtEmail.Text.Trim();
                _user.Password = txtPassword.Text.Trim();
                _user.FirstName = txtFirstName.Text.Trim();
                _user.MiddleName = string.IsNullOrWhiteSpace(txtMiddleName.Text) ? null : txtMiddleName.Text.Trim();
                _user.LastName = txtLastName.Text.Trim();
                _user.DateOfBirth = dtpDateOfBirth.Value.Date;
                _user.Gender = cmbGender.SelectedItem?.ToString() ?? "Other";
                _user.Phone = txtPhone.Text.Trim();
                _user.Address = string.IsNullOrWhiteSpace(txtAddress.Text) ? null : txtAddress.Text.Trim();
                _user.Role = cmbRole.SelectedItem?.ToString() ?? "client";
                _user.UpdatedAt = DateTime.UtcNow;

                var validationContext = new ValidationContext(_user);
                var validationResults = new List<ValidationResult>();

                if (!Validator.TryValidateObject(_user, validationContext, validationResults, true))
                {
                    var errors = string.Join("\n", validationResults.Select(r => r.ErrorMessage));
                    MessageBox.Show($"Validation failed:\n{errors}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool success = await _userService.UpdateUserAsync(_user);
                if (success)
                {
                    MessageBox.Show("User data has been updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to update user data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}