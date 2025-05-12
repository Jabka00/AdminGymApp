using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using AdminApp.Models;
using AdminApp.Services;

namespace AdminApp.Forms
{
    public class EditTrainingForm : Form
    {
        private readonly TrainingService _trainingService;
        private readonly TrainerService _trainerService;
        private Training _training;

        private TextBox? txtTitle, txtDescription;
        private ComboBox? cmbType, cmbTrainer;
        private DateTimePicker? dtpSchedule;
        private NumericUpDown? nudDurationHours, nudDurationMinutes, nudCapacity, nudGroupPrice;
        private Button? btnSave, btnCancel;

        public EditTrainingForm(Training training, TrainingService trainingService, TrainerService trainerService)
        {
            _training = training;
            _trainingService = trainingService;
            _trainerService = trainerService;
            InitializeComponents();
            LoadTrainingData();
            LoadTrainers();
        }

        private void InitializeComponents()
        {
            this.Text = "Edit Training";
            this.Size = new System.Drawing.Size(500, 600);
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

            txtTitle = new TextBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            AddRow("Title:", txtTitle);

            txtDescription = new TextBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right, Multiline = true, Height = 60 };
            AddRow("Description:", txtDescription);

            cmbType = new ComboBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbType.Items.AddRange(new string[] { "group", "personal" });
            cmbType.SelectedIndexChanged += (s, e) => UpdateGroupPriceVisibility();
            AddRow("Type:", cmbType);

            cmbTrainer = new ComboBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right, DropDownStyle = ComboBoxStyle.DropDownList };
            AddRow("Trainer:", cmbTrainer);

            dtpSchedule = new DateTimePicker() { Anchor = AnchorStyles.Left | AnchorStyles.Right, Format = DateTimePickerFormat.Custom, CustomFormat = "dd.MM.yyyy HH:mm" };
            AddRow("Schedule (date/time):", dtpSchedule);

            var durationPanel = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Fill };
            nudDurationHours = new NumericUpDown() { Minimum = 0, Maximum = 24, Width = 60 };
            nudDurationMinutes = new NumericUpDown() { Minimum = 0, Maximum = 59, Width = 60 };
            durationPanel.Controls.Add(new Label() { Text = "Hours:", AutoSize = true });
            durationPanel.Controls.Add(nudDurationHours);
            durationPanel.Controls.Add(new Label() { Text = "Minutes:", AutoSize = true });
            durationPanel.Controls.Add(nudDurationMinutes);
            AddRow("Duration:", durationPanel);

            nudCapacity = new NumericUpDown() { Minimum = 1, Maximum = 100, Width = 100 };
            AddRow("Capacity:", nudCapacity);

            nudGroupPrice = new NumericUpDown()
            {
                Minimum = 0,
                Maximum = 999999,
                DecimalPlaces = 2,
                Value = 0,
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            AddRow("Group Price:", nudGroupPrice);

            btnSave = new Button() { Text = "Save", Anchor = AnchorStyles.None, Width = 100 };
            btnSave.Click += async (s, e) => await SaveTraining();

            btnCancel = new Button() { Text = "Cancel", Anchor = AnchorStyles.None, Width = 100 };
            btnCancel.Click += (s, e) => this.Close();

            var flowPanel = new FlowLayoutPanel() { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill };
            flowPanel.Controls.Add(btnCancel);
            flowPanel.Controls.Add(btnSave);

            tableLayout.Controls.Add(flowPanel, 1, tableLayout.RowCount - 1);
            this.Controls.Add(tableLayout);
        }

        private void UpdateGroupPriceVisibility()
        {
            if (cmbType == null || nudGroupPrice == null)
                return;

            string? selectedType = cmbType.SelectedItem?.ToString();
            if (selectedType == "group")
            {
                nudGroupPrice.Enabled = true;
            }
            else
            {
                nudGroupPrice.Value = 0;
                nudGroupPrice.Enabled = false;
            }
        }

        private async void LoadTrainers()
        {
            if (cmbTrainer == null)
                return;

            var trainers = await _trainerService.GetAllTrainersAsync();
            cmbTrainer.DataSource = trainers;
            cmbTrainer.DisplayMember = "Name";
            cmbTrainer.ValueMember = "Id";
        }

        private void LoadTrainingData()
        {
            if (txtTitle == null || txtDescription == null || cmbType == null || cmbTrainer == null ||
                dtpSchedule == null || nudDurationHours == null || nudDurationMinutes == null || nudCapacity == null || nudGroupPrice == null)
                return;

            txtTitle.Text = _training.Title;
            txtDescription.Text = _training.Description;
            cmbType.SelectedItem = _training.Type;
            cmbTrainer.SelectedValue = _training.TrainerId;
            dtpSchedule.Value = _training.Schedule;
            nudDurationHours.Value = (decimal)_training.Duration.Hours;
            nudDurationMinutes.Value = (decimal)_training.Duration.Minutes;
            nudCapacity.Value = _training.Capacity;
            if (_training.Type == "group" && _training.GroupPrice.HasValue)
                nudGroupPrice.Value = (decimal)_training.GroupPrice.Value;
            else
                nudGroupPrice.Value = 0;

            UpdateGroupPriceVisibility();
        }

        private async Task SaveTraining()
        {
            if (txtTitle == null || txtDescription == null || cmbType == null || cmbTrainer == null ||
                dtpSchedule == null || nudDurationHours == null || nudDurationMinutes == null || nudCapacity == null || nudGroupPrice == null)
                return;

            if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Please fill in all required fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dtpSchedule.Value < DateTime.Now)
            {
                MessageBox.Show("The training date/time must be in the future.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbTrainer.SelectedValue == null)
            {
                MessageBox.Show("Please select a trainer.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedTrainerId = cmbTrainer.SelectedValue.ToString();
            var trainer = await _trainerService.GetTrainerByIdAsync(selectedTrainerId);
            if (trainer == null)
            {
                MessageBox.Show("Selected trainer was not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var trainingDay = dtpSchedule.Value.DayOfWeek;
            if (!trainer.WorkingDays.Contains(trainingDay))
            {
                MessageBox.Show($"Trainer {trainer.Name} does not work on {trainingDay}.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _training.Title = txtTitle.Text.Trim();
            _training.Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text.Trim();
            _training.Type = cmbType.SelectedItem?.ToString() ?? "group";
            _training.TrainerId = selectedTrainerId;
            _training.Schedule = dtpSchedule.Value;
            _training.Duration = new TimeSpan((int)nudDurationHours.Value, (int)nudDurationMinutes.Value, 0);
            _training.Capacity = (int)nudCapacity.Value;
            _training.GroupPrice = _training.Type == "group" ? (double)nudGroupPrice.Value : (double?)null;
            _training.UpdatedAt = DateTime.UtcNow;

            var success = await _trainingService.UpdateTrainingAsync(_training);
            if (success)
            {
                MessageBox.Show("Training has been successfully updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to update the training.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}