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

        // Панель и элементы управления для постраничной навигации
        private Panel paginationPanel;
        private Button btnPrev;
        private Button btnNext;
        private Label lblPageInfo;
        private TextBox txtPage;
        private Button btnGo;

        // Переменные для пагинации
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalPages = 1;

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
            // Разрешаем изменять размер окна и максимизацию
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;

            // Инициализация BindingSource
            bindingSource = new BindingSource();

            // Верхняя панель для элементов управления (поиск, обновление)
            topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(10),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Кнопка "Обновить"
            btnRefresh = new Button
            {
                Text = "Обновить",
                Width = 100,
                Height = 30,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                BackColor = Color.LightGray,
                Left = 10,
                Top = 15
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += async (s, e) => { _currentPage = 1; await LoadUsersAsync(); };

            // Метка для поиска по username
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

            // Кнопка "Поиск"
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
            btnSearch.Click += (s, e) => { _currentPage = 1; ApplySearchFilter(); };

            // Добавляем элементы на верхнюю панель
            topPanel.Controls.Add(btnRefresh);
            topPanel.Controls.Add(lblSearch);
            topPanel.Controls.Add(txtSearch);
            topPanel.Controls.Add(btnSearch);

            // Инициализация DataGridView
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

            // Панель для постраничной навигации
            paginationPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                Padding = new Padding(10),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            btnPrev = new Button
            {
                Text = "Previous",
                Width = 80,
                Height = 30,
                Left = 10,
                Top = 5
            };
            btnPrev.Click += async (s, e) =>
            {
                if (_currentPage > 1)
                {
                    _currentPage--;
                    await LoadUsersAsync();
                }
            };

            btnNext = new Button
            {
                Text = "Next",
                Width = 80,
                Height = 30,
                Left = btnPrev.Right + 10,
                Top = 5
            };
            btnNext.Click += async (s, e) =>
            {
                if (_currentPage < _totalPages)
                {
                    _currentPage++;
                    await LoadUsersAsync();
                }
            };

            lblPageInfo = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Left = btnNext.Right + 10,
                Top = 10,
                Text = "Page 0 of 0 (Total: 0)"
            };

            txtPage = new TextBox
            {
                Width = 40,
                Height = 25,
                Left = lblPageInfo.Right + 20,
                Top = 8
            };

            btnGo = new Button
            {
                Text = "Go",
                Width = 40,
                Height = 30,
                Left = txtPage.Right + 5,
                Top = 5
            };
            btnGo.Click += async (s, e) =>
            {
                if (int.TryParse(txtPage.Text.Trim(), out int pageNum))
                {
                    if (pageNum >= 1 && pageNum <= _totalPages)
                    {
                        _currentPage = pageNum;
                        await LoadUsersAsync();
                    }
                }
            };

            paginationPanel.Controls.Add(btnPrev);
            paginationPanel.Controls.Add(btnNext);
            paginationPanel.Controls.Add(lblPageInfo);
            paginationPanel.Controls.Add(txtPage);
            paginationPanel.Controls.Add(btnGo);

            // Добавляем элементы на форму (порядок: DataGridView, панель навигации, верхняя панель, индикатор загрузки)
            Controls.Add(dgvUsers);
            Controls.Add(paginationPanel);
            Controls.Add(topPanel);
            Controls.Add(lblLoading);
        }

        // Асинхронно загружает пользователей, обновляет _allUsers и вызывает фильтрацию с пагинацией
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

        // Применяет фильтр поиска, вычисляет общее число страниц и отображает данные текущей страницы
        private void ApplySearchFilter()
        {
            string filter = txtSearch?.Text.Trim() ?? string.Empty;
            var filtered = string.IsNullOrEmpty(filter)
                ? _allUsers
                : _allUsers.Where(u => !string.IsNullOrEmpty(u.Username) &&
                                       u.Username.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                           .ToList();

            int totalUsers = filtered.Count;
            _totalPages = (int)Math.Ceiling(totalUsers / (double)_pageSize);
            if (_totalPages == 0)
                _totalPages = 1;
            if (_currentPage > _totalPages)
                _currentPage = _totalPages;

            // Выбираем данные для текущей страницы
            var pageData = filtered.Skip((_currentPage - 1) * _pageSize).Take(_pageSize).ToList();
            bindingSource.DataSource = pageData;

            lblPageInfo.Text = $"Page {_currentPage} of {_totalPages} (Total: {totalUsers})";
            btnPrev.Enabled = _currentPage > 1;
            btnNext.Enabled = _currentPage < _totalPages;
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

        // Обработчик сортировки при клике по заголовку столбца (с использованием Reflection)
        private void DgvUsers_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (dgvUsers.Columns.Count <= e.ColumnIndex)
                return;

            var column = dgvUsers.Columns[e.ColumnIndex];
            string propertyName = column.DataPropertyName;
            if (string.IsNullOrEmpty(propertyName))
                return;

            if (_lastSortedColumn == propertyName)
                _sortAscending = !_sortAscending;
            else
            {
                _lastSortedColumn = propertyName;
                _sortAscending = true;
            }

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
