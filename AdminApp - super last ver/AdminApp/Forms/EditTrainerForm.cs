
using System;
using System.Windows.Forms;
using AdminApp.Models;
using AdminApp.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdminApp.Forms
{
    public class EditTrainerForm : Form
    {
        private readonly TrainerService _trainerService;
        private Trainer _trainer;

        private TextBox? txtName;
        private CheckedListBox? clbWorkingDays;
        private Button? btnSave, btnCancel;

        public EditTrainerForm(Trainer trainer, TrainerService trainerService)
        {
            _trainer = trainer;
            _trainerService = trainerService;
            InitializeComponents();
            LoadTrainerData();
        }

        private void InitializeComponents()
        {
            this.Text = "Edit Trainer";
            this.Size = new System.Drawing.Size(400, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            var tableLayout = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
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

            // Name
            txtName = new TextBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            AddRow("Trainer Name:", txtName);

            // Working Days
            clbWorkingDays = new CheckedListBox()
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                CheckOnClick = true
            };
            clbWorkingDays.Items.AddRange(Enum.GetNames(typeof(DayOfWeek)));
            AddRow("Working Days:", clbWorkingDays);

            // Save/Cancel
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

        private void LoadTrainerData()
        {
            if (txtName == null || clbWorkingDays == null)
                return;

            txtName.Text = _trainer.Name;

            for (int i = 0; i < clbWorkingDays.Items.Count; i++)
            {
                var day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), clbWorkingDays.Items[i].ToString()!);
                if (_trainer.WorkingDays.Contains(day))
                {
                    clbWorkingDays.SetItemChecked(i, true);
                }
                else
                {
                    clbWorkingDays.SetItemChecked(i, false);
                }
            }
        }

        private async Task SaveTrainer()
        {
            if (txtName == null || clbWorkingDays == null || btnSave == null)
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

            // Update the trainer data
            _trainer.Name = txtName.Text.Trim();
            _trainer.WorkingDays = new List<DayOfWeek>();

            foreach (var item in clbWorkingDays.CheckedItems)
            {
                if (Enum.TryParse(item.ToString(), out DayOfWeek day))
                {
                    _trainer.WorkingDays.Add(day);
                }
            }

            _trainer.UpdatedAt = DateTime.UtcNow;

            var success = await _trainerService.UpdateTrainerAsync(_trainer);
            if (success)
            {
                MessageBox.Show("Trainer data has been successfully updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to update trainer data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
