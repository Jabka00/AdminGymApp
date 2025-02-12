using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using AdminApp.Models;
using AdminApp.Services;

namespace AdminApp.Forms
{
    public class UserListForm : Form
    {
        private DataGridView dgvUsers;
        private Button btnRefresh;
        private Label lblLoading;
        private Panel topPanel;
        private Label lblSearch;
        private TextBox txtSearch;
        private Button btnSearch;
        private BindingSource bindingSource;

        // Хранит полный список пользователей для сортировки и фильтрации
        private List<User> _allUsers = new List<User>();
        private string? _lastSortedColumn;
        private bool _sortAscending = true;

        private readonly UserService _userService;

        public UserListForm(UserService userService)
        {
            _userService = userService;
            InitializeComponents();
            _ = LoadUsersAsync();
        }

        private void InitializeComponents()
        {
            // Основной вид формы
            Text = "User List";
            Size = new Size(900, 600);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White; // Фон формы – белый (без изменений в цветовой схеме)

            // Инициализация BindingSource
            bindingSource = new BindingSource();

            // Верхняя панель для элементов управления
            topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(10),
                BackColor = Color.White, // тот же цвет фона
                BorderStyle = BorderStyle.FixedSingle
            };

            // Кнопка "Обновить" с улучшенным внешним видом
            btnRefresh = new Button
            {
                Text = "Обновить",
                Width = 100,
                Height = 30,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                BackColor = Color.LightGray, // можно оставить как есть, или задать другой оттенок
                Left = 10,
                Top = 15
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += async (s, e) => await LoadUsersAsync();

            // Метка для поиска
            lblSearch = new Label
            {
                Text = "Username:",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Left = btnRefresh.Right + 20,
                Top = 20,
                ForeColor = Color.Black
            };

            // Текстовое поле поиска
            txtSearch = new TextBox
            {
                Width = 200,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Left = lblSearch.Right + 5,
                Top = 15
            };

            // Кнопка "Поиск" с улучшенным внешним видом
            btnSearch = new Button
            {
                Text = "Поиск",
                Width = 100,
                Height = 30,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                BackColor = Color.LightGray,
                Left = txtSearch.Right + 10,
                Top = 15
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += (s, e) => ApplySearchFilter();

            // Добавляем элементы на верхнюю панель
            topPanel.Controls.Add(btnRefresh);
            topPanel.Controls.Add(lblSearch);
            topPanel.Controls.Add(txtSearch);
            topPanel.Controls.Add(btnSearch);

            // Инициализация DataGridView с улучшенным оформлением
            dgvUsers = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = true,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                DataSource = bindingSource,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                GridColor = Color.LightGray,
                RowTemplate = { Height = 30 },
                Font = new Font("Segoe UI", 9, FontStyle.Regular)
            };

            // Стилизация заголовков столбцов
            dgvUsers.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(3)
            };

            dgvUsers.DefaultCellStyle = new DataGridViewCellStyle
            {
                Padding = new Padding(3),
                ForeColor = Color.Black,
                BackColor = Color.White,
                SelectionBackColor = Color.LightGray,
                SelectionForeColor = Color.Black
            };

            dgvUsers.EnableHeadersVisualStyles = false;
            dgvUsers.ColumnHeaderMouseClick += DgvUsers_ColumnHeaderMouseClick;

            // Контекстное меню для редактирования пользователя
            var contextMenu = new ContextMenuStrip { BackColor = Color.White };
            var editItem = new ToolStripMenuItem("Edit")
            {
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                BackColor = Color.White
            };
            editItem.Click += EditUser;
            contextMenu.Items.Add(editItem);
            dgvUsers.ContextMenuStrip = contextMenu;
            dgvUsers.MouseDown += DgvUsers_MouseDown;

            // Метка загрузки
            lblLoading = new Label
            {
                Text = "Loading...",
                Dock = DockStyle.Bottom,
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false,
                Height = 30,
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.Black
            };

            // Добавляем элементы на форму
            Controls.Add(dgvUsers);
            Controls.Add(topPanel);
            Controls.Add(lblLoading);
        }

        // Асинхронный метод загрузки пользователей
        public async Task LoadUsersAsync()
        {
            try
            {
                lblLoading.Visible = true;
                btnRefresh.Enabled = false;
                Cursor = Cursors.WaitCursor;

                var users = await _userService.GetAllUsersAsync();
                _allUsers = users;

                ApplySearchFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке пользователей: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                lblLoading.Visible = false;
                btnRefresh.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        // Применяет фильтр поиска по username и обновляет источник данных BindingSource
        private void ApplySearchFilter()
        {
            string filter = txtSearch?.Text.Trim() ?? string.Empty;
            var filtered = string.IsNullOrEmpty(filter)
                ? _allUsers
                : _allUsers.Where(u => !string.IsNullOrEmpty(u.Username) &&
                                       u.Username.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                           .ToList();

            bindingSource.DataSource = filtered;
        }

        // Обработка редактирования пользователя через контекстное меню
        private void EditUser(object? sender, EventArgs e)
        {
            if (dgvUsers?.SelectedRows.Count > 0)
            {
                var selectedRow = dgvUsers.SelectedRows[0];
                if (selectedRow.DataBoundItem is User user)
                {
                    var editUserForm = new EditUserForm(user, _userService);
                    editUserForm.FormClosed += async (s, e) => await LoadUsersAsync();
                    editUserForm.ShowDialog();
                }
            }
        }

        // Позволяет выделять строку по клику правой кнопкой мыши
        private void DgvUsers_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitTestInfo = dgvUsers.HitTest(e.X, e.Y);
                if (hitTestInfo.RowIndex >= 0)
                {
                    dgvUsers.ClearSelection();
                    dgvUsers.Rows[hitTestInfo.RowIndex].Selected = true;
                }
            }
        }

        // Обработчик сортировки при клике по заголовку столбца
        private void DgvUsers_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (dgvUsers.Columns.Count <= e.ColumnIndex)
                return;

            var column = dgvUsers.Columns[e.ColumnIndex];
            string propertyName = column.DataPropertyName;
            if (string.IsNullOrEmpty(propertyName))
                return;

            // Определяем направление сортировки
            if (_lastSortedColumn == propertyName)
                _sortAscending = !_sortAscending;
            else
            {
                _lastSortedColumn = propertyName;
                _sortAscending = true;
            }

            // Сортировка с использованием Reflection
            var propInfo = typeof(User).GetProperty(propertyName);
            if (propInfo == null)
                return;

            _allUsers = _sortAscending
                ? _allUsers.OrderBy(u => propInfo.GetValue(u, null)).ToList()
                : _allUsers.OrderByDescending(u => propInfo.GetValue(u, null)).ToList();

            ApplySearchFilter();
        }
    }
}
