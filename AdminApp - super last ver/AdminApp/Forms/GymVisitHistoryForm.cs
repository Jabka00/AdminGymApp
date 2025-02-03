using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using AdminApp.Models;
using AdminApp.Services;

namespace AdminApp.Forms
{
    public class GymVisitHistoryForm : Form
    {
        private readonly GymVisitService _gymVisitService;
        private DataGridView dgvHistory;
        private TextBox txtUserId;
        private Button btnLoadHistory;

        public GymVisitHistoryForm(GymVisitService gymVisitService)
        {
            _gymVisitService = gymVisitService;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "История посещений зала";
            this.Size = new System.Drawing.Size(800, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            Label lblPrompt = new Label() { Text = "Введите ID пользователя:", AutoSize = true, Top = 20, Left = 20 };
            txtUserId = new TextBox() { Top = 50, Left = 20, Width = 200 };
            btnLoadHistory = new Button() { Text = "Загрузить историю", Top = 50, Left = 240, Width = 150 };
            btnLoadHistory.Click += async (s, e) => await LoadHistory();

            dgvHistory = new DataGridView()
            {
                Top = 100,
                Left = 20,
                Width = 740,
                Height = 440,
                AutoGenerateColumns = true,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            this.Controls.Add(lblPrompt);
            this.Controls.Add(txtUserId);
            this.Controls.Add(btnLoadHistory);
            this.Controls.Add(dgvHistory);
        }

        private async Task LoadHistory()
        {
            string userId = txtUserId.Text.Trim();
            if (string.IsNullOrEmpty(userId))
            {
                MessageBox.Show("Введите ID пользователя.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                List<GymVisit> history = await _gymVisitService.GetVisitHistoryAsync(userId);
                dgvHistory.DataSource = history;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки истории: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
