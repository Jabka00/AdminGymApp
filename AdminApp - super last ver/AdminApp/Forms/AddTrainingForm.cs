
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
            this.Text = "Add Training";
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

            txtTitle = new TextBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            AddRow("Title:", txtTitle);

            txtDescription = new TextBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right, Multiline = true, Height = 60 };
            AddRow("Description:", txtDescription);

            cmbType = new ComboBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbType.Items.AddRange(new string[] { "group", "personal" });
            cmbType.SelectedIndex = 0;
            cmbType.SelectedIndexChanged += CmbType_SelectedIndexChanged; 
            AddRow("Type:", cmbType);

            cmbTrainer = new ComboBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right, DropDownStyle = ComboBoxStyle.DropDownList };
            AddRow("Trainer:", cmbTrainer);

            dtpSchedule = new DateTimePicker() { Anchor = AnchorStyles.Left | AnchorStyles.Right, Format = DateTimePickerFormat.Custom, CustomFormat = "dd.MM.yyyy HH:mm" };
            AddRow("Schedule (date/time):", dtpSchedule);

            var durationPanel = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Fill };
            nudDurationHours = new NumericUpDown() { Minimum = 0, Maximum = 24, Width = 60 };
            nudDurationMinutes = new NumericUpDown() { Minimum = 0, Maximum = 59, Width = 60 };
            durationPanel.Controls.Add(new Label() { Text = "Hours:", AutoSize = true, TextAlign = System.Drawing.ContentAlignment.MiddleLeft });
            durationPanel.Controls.Add(nudDurationHours);
            durationPanel.Controls.Add(new Label() { Text = "Minutes:", AutoSize = true, TextAlign = System.Drawing.ContentAlignment.MiddleLeft });
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
            btnSave.Click += async (s, e) => await SaveTrainingAsync();

            btnCancel = new Button() { Text = "Cancel", Anchor = AnchorStyles.None, Width = 100 };
            btnCancel.Click += (s, e) => this.Close();

            var flowPanel = new FlowLayoutPanel() { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill };
            flowPanel.Controls.Add(btnCancel);
            flowPanel.Controls.Add(btnSave);

            tableLayout.Controls.Add(flowPanel, 1, tableLayout.RowCount - 1);
            this.Controls.Add(tableLayout);

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

            try
            {
                var trainers = await _trainerService.GetAllTrainersAsync();
                cmbTrainer.DataSource = trainers;
                cmbTrainer.DisplayMember = "Name";
                cmbTrainer.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading trainers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task SaveTrainingAsync()
        {
            if (txtTitle == null || txtDescription == null || cmbType == null || cmbTrainer == null ||
                dtpSchedule == null || nudDurationHours == null || nudDurationMinutes == null || 
                nudCapacity == null || nudGroupPrice == null)
                return;

            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Please enter a title.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (dtpSchedule.Value < DateTime.Now)
            {
                MessageBox.Show("The training date/time must be in the future.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedTrainerId = cmbTrainer.SelectedValue?.ToString();
            if (string.IsNullOrEmpty(selectedTrainerId))
            {
                MessageBox.Show("Please select a trainer.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var trainingType = cmbType.SelectedItem?.ToString() ?? "group";
            
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

            if (trainingType == "group")
            {
                newTraining.GroupPrice = (double)nudGroupPrice.Value;
            }
            else
            {
                newTraining.GroupPrice = null; 
            }

            try
            {
                var success = await _trainingService.InsertTrainingAsync(newTraining);
                if (success)
                {
                    MessageBox.Show("Training has been successfully added!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to add the training.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding training: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
