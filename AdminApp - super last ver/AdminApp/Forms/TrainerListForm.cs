using System;
using System.Windows.Forms;
using AdminApp.Services;
using AdminApp.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace AdminApp.Forms
{
    public class TrainerListForm : Form
    {
        private DataGridView? dgvTrainers;
        private Button? btnRefresh;
        private Button? btnAddTrainer;
        private Label? lblLoading;
        private readonly TrainerService _trainerService;

        public TrainerListForm(TrainerService trainerService)
        {
            _trainerService = trainerService;
            InitializeComponents();
            LoadTrainers();
        }

        private void InitializeComponents()
        {
            this.Text = "Список тренеров";
            this.Size = new System.Drawing.Size(800, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            dgvTrainers = new DataGridView()
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
            btnRefresh.Click += (s, e) => LoadTrainers();

            btnAddTrainer = new Button()
            {
                Text = "Добавить тренера",
                Dock = DockStyle.Top,
                Height = 40
            };
            btnAddTrainer.Click += (s, e) => OpenAddTrainerForm();

            lblLoading = new Label()
            {
                Text = "Загрузка...",
                Dock = DockStyle.Bottom,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Visible = false
            };

            // Контекстное меню для редактирования и удаления тренера
            var contextMenu = new ContextMenuStrip();
            var editItem = new ToolStripMenuItem("Редактировать");
            editItem.Click += EditTrainer;
            var deleteItem = new ToolStripMenuItem("Удалить");
            deleteItem.Click += DeleteTrainer;

            contextMenu.Items.AddRange(new ToolStripItem[] { editItem, deleteItem });
            dgvTrainers.ContextMenuStrip = contextMenu;
            dgvTrainers.MouseDown += DgvTrainers_MouseDown;

            this.Controls.Add(dgvTrainers);
            this.Controls.Add(btnAddTrainer);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(lblLoading);
        }

        // Метод для загрузки тренеров
        public async void LoadTrainers()
        {
            if (dgvTrainers == null || btnRefresh == null || lblLoading == null)
                return;

            try
            {
                lblLoading.Visible = true;
                btnRefresh.Enabled = false;
                btnAddTrainer.Enabled = false;
                Cursor.Current = Cursors.WaitCursor;

                var trainers = await _trainerService.GetAllTrainersAsync();
                dgvTrainers.DataSource = trainers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке тренеров: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                lblLoading.Visible = false;
                btnRefresh.Enabled = true;
                btnAddTrainer.Enabled = true;
                Cursor.Current = Cursors.Default;
            }
        }

        private void OpenAddTrainerForm()
        {
            var addTrainerForm = new AddTrainerForm(_trainerService);
            addTrainerForm.FormClosed += (s, e) => LoadTrainers();
            addTrainerForm.ShowDialog();
        }

        private void EditTrainer(object? sender, EventArgs e)
        {
            if (dgvTrainers?.SelectedRows.Count > 0)
            {
                var selectedRow = dgvTrainers.SelectedRows[0];
                var trainer = selectedRow.DataBoundItem as Trainer;

                if (trainer != null)
                {
                    var editTrainerForm = new EditTrainerForm(trainer, _trainerService);
                    editTrainerForm.FormClosed += (s, e) => LoadTrainers();
                    editTrainerForm.ShowDialog();
                }
            }
        }

        private async void DeleteTrainer(object? sender, EventArgs e)
        {
            if (dgvTrainers?.SelectedRows.Count > 0)
            {
                var selectedRow = dgvTrainers.SelectedRows[0];
                var trainer = selectedRow.DataBoundItem as Trainer;

                if (trainer != null)
                {
                    var confirmResult = MessageBox.Show($"Вы уверены, что хотите удалить тренера '{trainer.Name}'?",
                                                         "Подтверждение",
                                                         MessageBoxButtons.YesNo,
                                                         MessageBoxIcon.Question);
                    if (confirmResult == DialogResult.Yes)
                    {
                        var success = await _trainerService.DeleteTrainerAsync(trainer.Id!);
                        if (success)
                        {
                            MessageBox.Show("Тренер успешно удалён!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadTrainers();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось удалить тренера.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        // Позволяет выделять строку по клику правой кнопкой мыши
        private void DgvTrainers_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitTestInfo = dgvTrainers?.HitTest(e.X, e.Y);
                if (hitTestInfo?.RowIndex >= 0)
                {
                    dgvTrainers.ClearSelection();
                    dgvTrainers.Rows[hitTestInfo.RowIndex].Selected = true;
                }
            }
        }
    }
}
