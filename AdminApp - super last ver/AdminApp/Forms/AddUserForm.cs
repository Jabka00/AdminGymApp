
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using AdminApp.Models;
using AdminApp.Services;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AdminApp.Forms
{
    public class AddUserForm : Form
    {
        private readonly UserService _userService;

        private TextBox? txtUsername, txtEmail, txtPassword;
        private TextBox? txtFirstName, txtMiddleName, txtLastName;
        private DateTimePicker? dtpDateOfBirth;
        private ComboBox? cmbGender;
        private TextBox? txtPhone, txtAddress;
        private ComboBox? cmbRole;
        private Button? btnSave, btnCancel;

        public AddUserForm(UserService userService)
        {
            _userService = userService;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Add User";
            this.Size = new System.Drawing.Size(500, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            var tableLayout = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 14,
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

            // Username
            txtUsername = new TextBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right };
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

        private async Task SaveUser()
        {
            if (txtUsername == null || txtEmail == null || txtPassword == null ||
                txtFirstName == null || txtLastName == null || dtpDateOfBirth == null ||
                cmbGender == null || txtPhone == null || cmbRole == null)
                return;

            // Validation of required fields
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Please fill in all required fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate Email
            if (!IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Invalid email format.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var newUser = new User
            {
                Username = txtUsername.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Password = txtPassword.Text.Trim(),
                FirstName = txtFirstName.Text.Trim(),
                MiddleName = string.IsNullOrWhiteSpace(txtMiddleName.Text) ? null : txtMiddleName.Text.Trim(),
                LastName = txtLastName.Text.Trim(),
                DateOfBirth = dtpDateOfBirth.Value.Date,
                Gender = cmbGender.SelectedItem?.ToString() ?? "Other",
                Phone = txtPhone.Text.Trim(),
                Address = string.IsNullOrWhiteSpace(txtAddress.Text) ? null : txtAddress.Text.Trim(),
                Role = cmbRole.SelectedItem?.ToString() ?? "client",
                SubscriptionPlan = null,
                SubscriptionEndDate = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var success = await _userService.InsertUserAsync(newUser);
            if (success)
            {
                MessageBox.Show("User has been successfully added!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to add user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}