using System;
using System.Windows.Forms;
using AdminApp.Models;
using AdminApp.Services;
using System.Threading.Tasks;

namespace AdminApp.Forms
{
    public class UserListForm : Form
    {
        private DataGridView? dgvUsers;
        private Button? btnRefresh;
        private Label? lblLoading;
        private readonly UserService _userService;

        public UserListForm(UserService userService)
        {
            _userService = userService;
            InitializeComponents();
            LoadUsers();
        }

        private void InitializeComponents()
        {
            this.Text = "Список клиентов";
            this.Size = new System.Drawing.Size(900, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            dgvUsers = new DataGridView()
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = true,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            btnRefresh = new Button()
            {
                Text = "Обновить",
                Dock = DockStyle.Top,
                Height = 40
            };
            btnRefresh.Click += (s, e) => LoadUsers();

            lblLoading = new Label()
            {
                Text = "Загрузка...",
                Dock = DockStyle.Bottom,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Visible = false
            };

            // Контекстное меню для редактирования пользователя
            var contextMenu = new ContextMenuStrip();
            var editItem = new ToolStripMenuItem("Редактировать");
            editItem.Click += EditUser;
            contextMenu.Items.Add(editItem);
            dgvUsers.ContextMenuStrip = contextMenu;
            dgvUsers.MouseDown += DgvUsers_MouseDown;

            this.Controls.Add(dgvUsers);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(lblLoading);
        }

        // Метод для загрузки пользователей (публичный)
        public async void LoadUsers()
        {
            if (dgvUsers == null || btnRefresh == null || lblLoading == null)
                return;

            try
            {
                lblLoading.Visible = true;
                btnRefresh.Enabled = false;
                Cursor.Current = Cursors.WaitCursor;

                var users = await _userService.GetAllUsersAsync();
                dgvUsers.DataSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке пользователей: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                lblLoading.Visible = false;
                btnRefresh.Enabled = true;
                Cursor.Current = Cursors.Default;
            }
        }

        // Обработка редактирования пользователя
        private void EditUser(object? sender, EventArgs e)
        {
            if (dgvUsers?.SelectedRows.Count > 0)
            {
                var selectedRow = dgvUsers.SelectedRows[0];
                var user = selectedRow.DataBoundItem as User;

                if (user != null)
                {
                    var editUserForm = new EditUserForm(user, _userService);
                    editUserForm.FormClosed += (s, e) => LoadUsers(); // Обновление списка после закрытия формы редактирования
                    editUserForm.ShowDialog();
                }
            }
        }

        // Позволяет выделять строку по клику правой кнопкой мыши
        private void DgvUsers_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitTestInfo = dgvUsers?.HitTest(e.X, e.Y);
                if (hitTestInfo?.RowIndex >= 0)
                {
                    dgvUsers.ClearSelection();
                    dgvUsers.Rows[hitTestInfo.RowIndex].Selected = true;
                }
            }
        }
        
    }
}
