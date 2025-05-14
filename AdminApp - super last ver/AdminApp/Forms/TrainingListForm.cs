using System;
using System.Windows.Forms;
using AdminApp.Models;
using AdminApp.Services;
using System.Threading.Tasks;
using System.Linq;

namespace AdminApp.Forms
{
    public class TrainingListForm : Form
    {
        private DataGridView? dgvTrainings;
        private Button? btnRefresh;
        private Button? btnAddTraining;
        private Label? lblLoading;
        private readonly TrainingService _trainingService;
        private readonly UserService _userService;
        private readonly TrainerService _trainerService;

        public TrainingListForm(TrainingService trainingService, UserService userService, TrainerService trainerService)
        {
            _trainingService = trainingService;
            _userService = userService;
            _trainerService = trainerService;
            InitializeComponents();
            LoadTrainingsAsync();
        }

        private void InitializeComponents()
        {
            this.Text = "Training list";
            this.Size = new System.Drawing.Size(1000, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            dgvTrainings = new DataGridView()
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false, 
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgvTrainings.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Price",
                Name = "GroupPrice",
                DataPropertyName = "GroupPrice",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "" }
            });
            dgvTrainings.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Title",
                Name = "Title", 
                DataPropertyName = "Title",
                Width = 150
            });

            dgvTrainings.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Description",
                Name = "Description",
                DataPropertyName = "Description",
                Width = 200
            });

            dgvTrainings.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Type",
                Name = "Type",
                DataPropertyName = "Type",
                Width = 80
            });

            dgvTrainings.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Schedule",
                Name = "Schedule",
                DataPropertyName = "Schedule",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd.MM.yyyy HH:mm" }
            });

            dgvTrainings.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Duration",
                Name = "Duration",
                DataPropertyName = "Duration",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = @"hh\:mm" }
            });

            dgvTrainings.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "TrainerName",
                Name = "TrainerName",
                DataPropertyName = "TrainerName",
                Width = 150
            });

            dgvTrainings.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Capacity",
                Name = "Capacity",
                DataPropertyName = "Capacity",
                Width = 80
            });

            dgvTrainings.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "EnrolledCount",
                Name = "EnrolledCount",
                DataPropertyName = "EnrolledCount",
                Width = 80
            });

            btnRefresh = new Button()
            {
                Text = "Update",
                Dock = DockStyle.Top,
                Height = 40
            };
            btnRefresh.Click += async (s, e) => await LoadTrainingsAsync();

            btnAddTraining = new Button()
            {
                Text = "Add training",
                Dock = DockStyle.Top,
                Height = 40
            };
            btnAddTraining.Click += (s, e) => OpenAddTrainingForm();

            lblLoading = new Label()
            {
                Text = "Loading...",
                Dock = DockStyle.Bottom,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Visible = false
            };

            var contextMenu = new ContextMenuStrip();
            var editItem = new ToolStripMenuItem("Edit");
            editItem.Click += EditTraining;
            var deleteItem = new ToolStripMenuItem("Delate");
            deleteItem.Click += DeleteTraining;
            var enrollItem = new ToolStripMenuItem("Enrole user");
            enrollItem.Click += EnrollUser;
            var viewEnrolledItem = new ToolStripMenuItem("View users");
            viewEnrolledItem.Click += ViewEnrolledUsers;

            contextMenu.Items.AddRange(new ToolStripItem[] { editItem, deleteItem, enrollItem, viewEnrolledItem });
            dgvTrainings.ContextMenuStrip = contextMenu;
            dgvTrainings.MouseDown += DgvTrainings_MouseDown;

            this.Controls.Add(dgvTrainings);
            this.Controls.Add(btnAddTraining);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(lblLoading);
        }

        public async Task LoadTrainingsAsync()
        {
            if (dgvTrainings == null || btnRefresh == null || lblLoading == null)
                return;

            try
            {
                lblLoading.Visible = true;
                btnRefresh.Enabled = false;
                btnAddTraining.Enabled = false;
                Cursor.Current = Cursors.WaitCursor;

                var trainings = await _trainingService.GetAllTrainingsAsync();
                var trainers = await _trainerService.GetAllTrainersAsync();
                var users = await _userService.GetAllUsersAsync();

                var displayList = trainings.Select(t => new
                {
                    t.Title,
                    t.Description,
                    t.Type,
                    Schedule = t.Schedule,
                    t.Duration,
                    TrainerName = trainers.FirstOrDefault(tr => tr.Id == t.TrainerId)?.Name ?? "Not found",
                    t.Capacity,
                    EnrolledCount = t.EnrolledUsers?.Count ?? 0,
                    t.GroupPrice
                }).ToList();


                dgvTrainings.DataSource = displayList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while loading: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                lblLoading.Visible = false;
                btnRefresh.Enabled = true;
                btnAddTraining.Enabled = true;
                Cursor.Current = Cursors.Default;
            }
        }

        private void OpenAddTrainingForm()
        {
            var addTrainingForm = new AddTrainingForm(_trainingService, _trainerService);
            addTrainingForm.FormClosed += async (s, e) => await LoadTrainingsAsync();
            addTrainingForm.ShowDialog();
        }

        private async void EditTraining(object? sender, EventArgs e)
        {
            if (dgvTrainings?.SelectedRows.Count > 0)
            {
                var selectedRow = dgvTrainings.SelectedRows[0];
                var trainingTitle = selectedRow.Cells["Title"].Value?.ToString();
                if (string.IsNullOrEmpty(trainingTitle))
                {
                    MessageBox.Show("Error with name .", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var training = await _trainingService.GetTrainingByTitleAsync(trainingTitle);
                if (training != null)
                {
                    var editTrainingForm = new EditTrainingForm(training, _trainingService, _trainerService);
                    editTrainingForm.FormClosed += async (s, e) => await LoadTrainingsAsync();
                    editTrainingForm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Error.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void DeleteTraining(object? sender, EventArgs e)
        {
            if (dgvTrainings?.SelectedRows.Count > 0)
            {
                var selectedRow = dgvTrainings.SelectedRows[0];
                var trainingTitle = selectedRow.Cells["Title"].Value?.ToString();

                if (string.IsNullOrEmpty(trainingTitle))
                {
                    MessageBox.Show("Error.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var training = await _trainingService.GetTrainingByTitleAsync(trainingTitle);
                if (training != null)
                {
                    var confirmResult = MessageBox.Show($"Are you sure you want to delate training '{training.Title}'?",
                                                         "Yes",
                                                         MessageBoxButtons.YesNo,
                                                         MessageBoxIcon.Question);
                    if (confirmResult == DialogResult.Yes)
                    {
                        var success = await _trainingService.DeleteTrainingAsync(training.Id!);
                        if (success)
                        {
                            MessageBox.Show("Success!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            await LoadTrainingsAsync();
                        }
                        else
                        {
                            MessageBox.Show("Error.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private async void EnrollUser(object? sender, EventArgs e)
        {
            if (dgvTrainings?.SelectedRows.Count > 0)
            {
                var selectedRow = dgvTrainings.SelectedRows[0];
                var trainingTitle = selectedRow.Cells["Title"].Value?.ToString();

                if (string.IsNullOrEmpty(trainingTitle))
                {
                    MessageBox.Show("Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var training = await _trainingService.GetTrainingByTitleAsync(trainingTitle);
                if (training == null)
                {
                    MessageBox.Show("Not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var enrollForm = new EnrollUserForm(training, _trainingService, _trainerService, _userService);
                enrollForm.FormClosed += async (s, e) => await LoadTrainingsAsync();
                enrollForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Chose training.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void ViewEnrolledUsers(object? sender, EventArgs e)
        {
            if (dgvTrainings?.SelectedRows.Count > 0)
            {
                var selectedRow = dgvTrainings.SelectedRows[0];
                var trainingTitle = selectedRow.Cells["Title"].Value?.ToString();

                if (string.IsNullOrEmpty(trainingTitle))
                {
                    MessageBox.Show("Error with name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var training = await _trainingService.GetTrainingByTitleAsync(trainingTitle);
                if (training != null)
                {
                    var enrolledUsersForm = new EnrolledUsersForm(training, _userService);
                    enrolledUsersForm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Chose training.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DgvTrainings_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitTestInfo = dgvTrainings?.HitTest(e.X, e.Y);
                if (hitTestInfo?.RowIndex >= 0)
                {
                    dgvTrainings.ClearSelection();
                    dgvTrainings.Rows[hitTestInfo.RowIndex].Selected = true;
                }
            }
        }
    }
}
