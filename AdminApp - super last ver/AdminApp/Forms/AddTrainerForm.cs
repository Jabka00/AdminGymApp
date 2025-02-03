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
        private NumericUpDown? nudTrainingPrice; // поле для цены
        private CheckedListBox? clbWorkingDays;
        private Button? btnSave, btnCancel;

        public AddTrainerForm(TrainerService trainerService)
        {
            _trainerService = trainerService;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Добавить тренера";
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

            // Имя тренера
            txtName = new TextBox() { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            AddRow("Имя тренера:", txtName);

            // Цена за тренировку
            nudTrainingPrice = new NumericUpDown()
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Minimum = 0,
                Maximum = 999999,
                DecimalPlaces = 2,
                Value = 100 // можно задать дефолтное
            };
            AddRow("Цена за тренировку:", nudTrainingPrice);

            // Рабочие дни
            clbWorkingDays = new CheckedListBox()
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                CheckOnClick = true
            };
            clbWorkingDays.Items.AddRange(Enum.GetNames(typeof(DayOfWeek)));
            AddRow("Рабочие дни:", clbWorkingDays);

            // Кнопки
            btnSave = new Button() { Text = "Сохранить", Anchor = AnchorStyles.None, Width = 100 };
            btnSave.Click += async (s, e) => await SaveTrainer();

            btnCancel = new Button() { Text = "Отмена", Anchor = AnchorStyles.None, Width = 100 };
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
                MessageBox.Show("Пожалуйста, заполните имя тренера.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (clbWorkingDays.CheckedItems.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите хотя бы один рабочий день.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Создаём тренера
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
                MessageBox.Show("Тренер успешно добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Не удалось добавить тренера.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
