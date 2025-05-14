using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using AdminApp.Models;
using AdminApp.Services;

namespace AdminApp.Forms
{
    public class SubscriptionListForm : Form
    {
        private DataGridView? dgvSubscriptions;
        private Button? btnRefresh;
        private Label? lblLoading;
        private readonly SubscriptionService _subscriptionService;
    
        public SubscriptionListForm(SubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
            InitializeComponents();
            _ = LoadSubscriptionsAsync();
        }
    
        private void InitializeComponents()
        {
            this.Text = "Subscription List";
            this.Size = new System.Drawing.Size(900, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
    
            dgvSubscriptions = new DataGridView()
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = true,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
    
            var contextMenu = new ContextMenuStrip();
            var cancelItem = new ToolStripMenuItem("Cancel Subscription");
            cancelItem.Click += CancelSubscription;
            contextMenu.Items.Add(cancelItem);
            dgvSubscriptions.ContextMenuStrip = contextMenu;
            dgvSubscriptions.MouseDown += DgvSubscriptions_MouseDown;
    
            btnRefresh = new Button()
            {
                Text = "Refresh",
                Dock = DockStyle.Top,
                Height = 40
            };
            btnRefresh.Click += async (s, e) => await LoadSubscriptionsAsync();
    
            lblLoading = new Label()
            {
                Text = "Loading...",
                Dock = DockStyle.Bottom,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Visible = false
            };
    
            this.Controls.Add(dgvSubscriptions);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(lblLoading);
        }
    
        private async Task LoadSubscriptionsAsync()
        {
            if (dgvSubscriptions == null || btnRefresh == null || lblLoading == null)
                return;
    
            try
            {
                lblLoading.Visible = true;
                btnRefresh.Enabled = false;
                Cursor.Current = Cursors.WaitCursor;
    
                List<Subscription> subscriptions = await _subscriptionService.GetAllSubscriptionsAsync();
                Console.WriteLine($"Subscriptions found: {subscriptions.Count}");
                dgvSubscriptions.DataSource = subscriptions;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading subscriptions: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                lblLoading.Visible = false;
                btnRefresh.Enabled = true;
                Cursor.Current = Cursors.Default;
            }
        }
    
        private async void CancelSubscription(object? sender, EventArgs e)
        {
            if (dgvSubscriptions?.SelectedRows.Count > 0)
            {
                var selectedRow = dgvSubscriptions.SelectedRows[0];
                var subscription = selectedRow.DataBoundItem as Subscription;
                if (subscription != null)
                {
                    var confirmResult = MessageBox.Show(
                        "Are you sure you want to cancel this subscription?",
                        "Confirmation",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (confirmResult == DialogResult.Yes)
                    {
                        bool success = await _subscriptionService.DeleteSubscriptionAsync(subscription.Id!);
                        if (success)
                        {
                            MessageBox.Show("Subscription canceled!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            await LoadSubscriptionsAsync();
                        }
                        else
                        {
                            MessageBox.Show("Failed to cancel subscription.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }
    
        private void DgvSubscriptions_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitTestInfo = dgvSubscriptions?.HitTest(e.X, e.Y);
                if (hitTestInfo?.RowIndex >= 0)
                {
                    dgvSubscriptions.ClearSelection();
                    dgvSubscriptions.Rows[hitTestInfo.RowIndex].Selected = true;
                }
            }
        }
    }
}