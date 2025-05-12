using System;
using System.Windows.Forms;
using AdminApp.Models;
using AdminApp.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdminApp.Forms
{
    public class AddTrainerForm : Form
    {
        private readonly TrainerService _trainerService;

        private TextBox? txtName;
        private NumericUpDown? nudTrainingPrice; 
        private CheckedListBox? clbWorkingDays;
        private Button? btnSave, btnCancel;

        public AddTrainerForm(TrainerService trainerService)
        {
            _trainerService = trainerService;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Add Trainer";
            this.Size = new System.Drawing.Size(400, 450);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            var tableLayout = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 5,
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

            txtName = new TextBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            AddRow("Trainer Name:", txtName);

            nudTrainingPrice = new NumericUpDown()
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Minimum = 0,
                Maximum = 999999,
                DecimalPlaces = 2,
                Value = 100 
            };
            AddRow("Training Price:", nudTrainingPrice);

            clbWorkingDays = new CheckedListBox()
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                CheckOnClick = true
            };
            clbWorkingDays.Items.AddRange(Enum.GetNames(typeof(DayOfWeek)));
            AddRow("Working Days:", clbWorkingDays);

            btnSave = new Button() { Text = "Save", Anchor = AnchorStyles.None, Width = 100 };
            btnSave.Click += async (s, e) => await SaveTrainer();

            btnCancel = new Button() { Text = "Cancel", Anchor = AnchorStyles.None, Width = 100 };
            btnCancel.Click += (s, e) => this.Close();

            var flowPanel = new FlowLayoutPanel() { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill };
            flowPanel.Controls.Add(btnCancel);
            flowPanel.Controls.Add(btnSave);

            tableLayout.Controls.Add(flowPanel, 1, tableLayout.RowCount - 1);
            this.Controls.Add(tableLayout);
        }

        private async Task SaveTrainer()
        {
            if (txtName == null || clbWorkingDays == null || nudTrainingPrice == null)
                return;

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter the trainer's name.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (clbWorkingDays.CheckedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one working day.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var newTrainer = new Trainer
            {
                Name = txtName.Text.Trim(),
                WorkingDays = new List<DayOfWeek>(),
                TrainingPrice = (double)nudTrainingPrice.Value,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            foreach (var item in clbWorkingDays.CheckedItems)
            {
                if (Enum.TryParse(item.ToString(), out DayOfWeek day))
                {
                    newTrainer.WorkingDays.Add(day);
                }
            }

            var success = await _trainerService.InsertTrainerAsync(newTrainer);
            if (success)
            {
                MessageBox.Show("Trainer has been successfully added!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to add the trainer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}