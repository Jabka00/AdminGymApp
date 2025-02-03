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

        // Контролы для редактирования данных пользователя
        private TextBox? txtUsername, txtEmail, txtPassword, txtFirstName, txtMiddleName, txtLastName, txtPhone, txtAddress;
        private ComboBox? cmbGender, cmbRole;
        private DateTimePicker? dtpDateOfBirth;
        // Кнопка для управления подпиской
        private Button? btnManageSubscription;
        // Кнопки Сохранить/Отмена
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
            this.Text = "Редактировать клиента";
            this.Size = new System.Drawing.Size(500, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            // Создаём TableLayoutPanel для удобного расположения контролов
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

            // Вспомогательный метод для добавления строки (метки + контрол)
            void AddRow(string labelText, Control control)
            {
                var lbl = new Label() { Text = labelText, Anchor = AnchorStyles.Right, AutoSize = true };
                tableLayout.Controls.Add(lbl);
                tableLayout.Controls.Add(control);
            }

            // Username (только для чтения)
            txtUsername = new TextBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right, ReadOnly = true };
            AddRow("Имя пользователя:", txtUsername);

            // Email
            txtEmail = new TextBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            AddRow("Email:", txtEmail);

            // Пароль
            txtPassword = new TextBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right, PasswordChar = '*' };
            AddRow("Пароль:", txtPassword);

            // Имя
            txtFirstName = new TextBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            AddRow("Имя:", txtFirstName);

            // Отчество
            txtMiddleName = new TextBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            AddRow("Отчество:", txtMiddleName);

            // Фамилия
           	txtLastName = new TextBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            AddRow("Фамилия:", txtLastName);

            // Дата рождения
            dtpDateOfBirth = new DateTimePicker() { Anchor = AnchorStyles.Left | AnchorStyles.Right, Format = DateTimePickerFormat.Short };
            AddRow("Дата рождения:", dtpDateOfBirth);

            // Пол
            cmbGender = new ComboBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbGender.Items.AddRange(new string[] { "Male", "Female", "Other" });
            cmbGender.SelectedIndex = 0;
            AddRow("Пол:", cmbGender);

            // Телефон
            txtPhone = new TextBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            AddRow("Телефон:", txtPhone);

            // Адрес
            txtAddress = new TextBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            AddRow("Адрес:", txtAddress);

            // Роль
            cmbRole = new ComboBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRole.Items.AddRange(new string[] { "client", "admin" });
            cmbRole.SelectedIndex = 0;
            AddRow("Роль:", cmbRole);

            // Кнопка для управления подпиской
            btnManageSubscription = new Button() { Text = "Управление подпиской", Width = 200 };
            btnManageSubscription.Click += (s, e) =>
            {
                if (string.IsNullOrEmpty(_user.Id))
                {
                    MessageBox.Show("Не удалось определить Id пользователя.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // Получаем экземпляр сервиса подписок.
                // Здесь можно использовать DI, а можно создать новый экземпляр.
                var subscriptionService = new SubscriptionService();
                var manageSubForm = new ManageSubscriptionForm(_user.Id, subscriptionService);
                manageSubForm.ShowDialog();
            };
            AddRow("Подписка:", btnManageSubscription);

            // Кнопки Сохранить и Отмена
            btnSave = new Button() { Text = "Сохранить", Anchor = AnchorStyles.None, Width = 100 };
            btnSave.Click += async (s, e) => await SaveUser();

            btnCancel = new Button() { Text = "Отмена", Anchor = AnchorStyles.None, Width = 100 };
            btnCancel.Click += (s, e) => this.Close();

            // Добавляем панель с кнопками в конец таблицы
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
            txtPassword.Text = _user.Password; // При необходимости можно оставить пустым для безопасности
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
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    MessageBox.Show($"Валидация не пройдена:\n{errors}", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool success = await _userService.UpdateUserAsync(_user);
                if (success)
                {
                    MessageBox.Show("Данные обновлены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Не удалось обновить данные.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
