using System;
using System.Windows.Forms;
using AdminApp.Models;
using AdminApp.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdminApp.Forms
{
    public class AddTrainingForm : Form
    {
        private readonly TrainingService _trainingService;
        private readonly TrainerService _trainerService;

        private TextBox? txtTitle, txtDescription;
        private ComboBox? cmbType, cmbTrainer;
        private DateTimePicker? dtpSchedule;
        private NumericUpDown? nudDurationHours, nudDurationMinutes, nudCapacity;
        
        // Поле для цены групповой тренировки
        private NumericUpDown? nudGroupPrice;
        
        private Button? btnSave, btnCancel;

        public AddTrainingForm(TrainingService trainingService, TrainerService trainerService)
        {
            _trainingService = trainingService;
            _trainerService = trainerService;
            InitializeComponents();
            LoadTrainers();
        }

        private void InitializeComponents()
        {
            this.Text = "Добавить тренировку";
            this.Size = new System.Drawing.Size(500, 550);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            var tableLayout = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 10,
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

            // Title
            txtTitle = new TextBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            AddRow("Название:", txtTitle);

            // Description
            txtDescription = new TextBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right, Multiline = true, Height = 60 };
            AddRow("Описание:", txtDescription);

            // Type
            cmbType = new ComboBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbType.Items.AddRange(new string[] { "group", "personal" });
            cmbType.SelectedIndex = 0;
            cmbType.SelectedIndexChanged += CmbType_SelectedIndexChanged; // событие при смене
            AddRow("Тип:", cmbType);

            // Trainer
            cmbTrainer = new ComboBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right, DropDownStyle = ComboBoxStyle.DropDownList };
            AddRow("Тренер:", cmbTrainer);

            // Schedule
            dtpSchedule = new DateTimePicker() { Anchor = AnchorStyles.Left | AnchorStyles.Right, Format = DateTimePickerFormat.Custom, CustomFormat = "dd.MM.yyyy HH:mm" };
            AddRow("Дата и время:", dtpSchedule);

            // Duration
            var durationPanel = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Fill };
            nudDurationHours = new NumericUpDown() { Minimum = 0, Maximum = 24, Width = 60 };
            nudDurationMinutes = new NumericUpDown() { Minimum = 0, Maximum = 59, Width = 60 };
            durationPanel.Controls.Add(new Label() { Text = "Часы:", AutoSize = true, TextAlign = System.Drawing.ContentAlignment.MiddleLeft });
            durationPanel.Controls.Add(nudDurationHours);
            durationPanel.Controls.Add(new Label() { Text = "Минуты:", AutoSize = true, TextAlign = System.Drawing.ContentAlignment.MiddleLeft });
            durationPanel.Controls.Add(nudDurationMinutes);
            AddRow("Продолжительность:", durationPanel);

            // Capacity
            nudCapacity = new NumericUpDown() { Minimum = 1, Maximum = 100, Width = 100 };
            AddRow("Вместимость:", nudCapacity);

            // **Group Price** (новое поле)
            nudGroupPrice = new NumericUpDown()
            {
                Minimum = 0,
                Maximum = 999999,
                DecimalPlaces = 2,
                Value = 0,
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            AddRow("Цена для группы:", nudGroupPrice);

            // Save and Cancel Buttons
            btnSave = new Button() { Text = "Сохранить", Anchor = AnchorStyles.None, Width = 100 };
            btnSave.Click += async (s, e) => await SaveTrainingAsync();

            btnCancel = new Button() { Text = "Отмена", Anchor = AnchorStyles.None, Width = 100 };
            btnCancel.Click += (s, e) => this.Close();

            var flowPanel = new FlowLayoutPanel() { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill };
            flowPanel.Controls.Add(btnCancel);
            flowPanel.Controls.Add(btnSave);

            tableLayout.Controls.Add(flowPanel, 1, tableLayout.RowCount - 1);
            this.Controls.Add(tableLayout);

            // Изначально, если тип по умолчанию "group", то поле доступно
            // Если "personal", то поле можно заблокировать:
            UpdateGroupPriceVisibility();
        }

        private void CmbType_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateGroupPriceVisibility();
        }

        private void UpdateGroupPriceVisibility()
        {
            if (cmbType == null || nudGroupPrice == null)
                return;

            string? selectedType = cmbType.SelectedItem?.ToString();
            // Если выбрано group - поле для цены группы активно
            // Если personal - можно скрыть или заблокировать
            if (selectedType == "group")
            {
                nudGroupPrice.Enabled = true;
            }
            else
            {
                // Если персональная тренировка
                nudGroupPrice.Value = 0;
                nudGroupPrice.Enabled = false;
            }
        }

        private async void LoadTrainers()
        {
            if (cmbTrainer == null)
                return;

            try
            {
                var trainers = await _trainerService.GetAllTrainersAsync();
                cmbTrainer.DataSource = trainers;
                cmbTrainer.DisplayMember = "Name";
                cmbTrainer.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке тренеров: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task SaveTrainingAsync()
        {
            if (txtTitle == null || txtDescription == null || cmbType == null || cmbTrainer == null ||
                dtpSchedule == null || nudDurationHours == null || nudDurationMinutes == null || 
                nudCapacity == null || nudGroupPrice == null)
                return;

            // Валидация
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Пожалуйста, заполните поле названия.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (dtpSchedule.Value < DateTime.Now)
            {
                MessageBox.Show("Дата и время тренировки должны быть в будущем.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedTrainerId = cmbTrainer.SelectedValue?.ToString();
            if (string.IsNullOrEmpty(selectedTrainerId))
            {
                MessageBox.Show("Пожалуйста, выберите тренера.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var trainingType = cmbType.SelectedItem?.ToString() ?? "group";
            
            // Создаём новую тренировку
            var newTraining = new Training
            {
                Title = txtTitle.Text.Trim(),
                Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text.Trim(),
                Type = trainingType,
                TrainerId = selectedTrainerId,
                Schedule = dtpSchedule.Value,
                Duration = new TimeSpan((int)nudDurationHours.Value, (int)nudDurationMinutes.Value, 0),
                Capacity = (int)nudCapacity.Value,
                EnrolledUsers = new List<string>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Если выбран group, берём значение
            if (trainingType == "group")
            {
                newTraining.GroupPrice = (double)nudGroupPrice.Value;
            }
            else
            {
                newTraining.GroupPrice = null; // или 0
            }

            try
            {
                var success = await _trainingService.InsertTrainingAsync(newTraining);
                if (success)
                {
                    MessageBox.Show("Тренировка успешно добавлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Не удалось добавить тренировку.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении тренировки: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
