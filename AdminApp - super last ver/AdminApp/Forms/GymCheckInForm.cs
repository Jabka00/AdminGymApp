using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using AdminApp.Services;

namespace AdminApp.Forms
{
    public class GymCheckInForm : Form
    {
        private readonly GymVisitService _gymVisitService;
        private TextBox txtUserId;
        private Button btnCheckIn;
        private Label lblResult;

        public GymCheckInForm(GymVisitService gymVisitService)
        {
            _gymVisitService = gymVisitService;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Вход в зал";
            this.Size = new System.Drawing.Size(400, 200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            Label lblPrompt = new Label() { Text = "Введите ID пользователя (или отсканируйте штрихкод):", AutoSize = true, Top = 20, Left = 20 };
            txtUserId = new TextBox() { Top = 50, Left = 20, Width = 340 };
            btnCheckIn = new Button() { Text = "Зачекинить", Top = 90, Left = 20, Width = 100 };
            lblResult = new Label() { Top = 130, Left = 20, Width = 340, AutoSize = true };

            btnCheckIn.Click += async (s, e) => await CheckInUser();

            this.Controls.Add(lblPrompt);
            this.Controls.Add(txtUserId);
            this.Controls.Add(btnCheckIn);
            this.Controls.Add(lblResult);
        }

        private async Task CheckInUser()
        {
            string userId = txtUserId.Text.Trim();
            if (string.IsNullOrEmpty(userId))
            {
                MessageBox.Show("Введите ID пользователя.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var result = await _gymVisitService.CheckInUserAsync(userId);
            lblResult.Text = result.message;
            if (result.success)
            {
                MessageBox.Show(result.message, "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show(result.message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
