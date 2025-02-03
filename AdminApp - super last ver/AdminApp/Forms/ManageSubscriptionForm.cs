
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using AdminApp.Models;
using AdminApp.Services;

namespace AdminApp.Forms
{
    public class ManageSubscriptionForm : Form
    {
        private readonly string _userId;
        private readonly SubscriptionService _subscriptionService;
        private Subscription? _subscription;

        private GroupBox groupExistingSubscription;
        private Label lblExistingStart, lblExistingEnd, lblExistingDuration, lblExistingPrice, lblExistingTotal;
        private Button btnCancelSubscription;

        private GroupBox groupNewSubscription;
        private NumericUpDown nudNewDuration;
        private NumericUpDown nudNewPrice;
        private DateTimePicker dtpNewStart;
        private Label lblNewTotal, lblNewEnd;
        private Button btnBuySubscription;

        private Button btnRefresh;

        public ManageSubscriptionForm(string userId, SubscriptionService subscriptionService)
        {
            _userId = userId;
            _subscriptionService = subscriptionService;
            InitializeComponents();
            LoadSubscriptionAsync();
        }

        private void InitializeComponents()
        {
            this.Text = "Manage Subscription";
            this.Size = new Size(500, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            btnRefresh = new Button() { Text = "Refresh", Dock = DockStyle.Top, Height = 30 };
            btnRefresh.Click += async (s, e) => await LoadSubscriptionAsync();
            this.Controls.Add(btnRefresh);

            // Existing subscription group
            groupExistingSubscription = new GroupBox() { Text = "Existing Subscription", Dock = DockStyle.Top, Height = 150 };
            lblExistingStart = new Label() { Text = "Start:", AutoSize = true };
            lblExistingEnd = new Label() { Text = "End:", AutoSize = true };
            lblExistingDuration = new Label() { Text = "Duration:", AutoSize = true };
            lblExistingPrice = new Label() { Text = "Price per month:", AutoSize = true };
            lblExistingTotal = new Label() { Text = "Total Price:", AutoSize = true };
            btnCancelSubscription = new Button() { Text = "Cancel Subscription", AutoSize = true };
            btnCancelSubscription.Click += async (s, e) => await CancelSubscriptionAsync();

            var existingLayout = new TableLayoutPanel() { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 6 };
            existingLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            existingLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            existingLayout.Controls.Add(new Label() { Text = "Start:", Anchor = AnchorStyles.Right, AutoSize = true }, 0, 0);
            existingLayout.Controls.Add(lblExistingStart, 1, 0);
            existingLayout.Controls.Add(new Label() { Text = "End:", Anchor = AnchorStyles.Right, AutoSize = true }, 0, 1);
            existingLayout.Controls.Add(lblExistingEnd, 1, 1);
            existingLayout.Controls.Add(new Label() { Text = "Duration (months):", Anchor = AnchorStyles.Right, AutoSize = true }, 0, 2);
            existingLayout.Controls.Add(lblExistingDuration, 1, 2);
            existingLayout.Controls.Add(new Label() { Text = "Price per month:", Anchor = AnchorStyles.Right, AutoSize = true }, 0, 3);
            existingLayout.Controls.Add(lblExistingPrice, 1, 3);
            existingLayout.Controls.Add(new Label() { Text = "Total Price:", Anchor = AnchorStyles.Right, AutoSize = true }, 0, 4);
            existingLayout.Controls.Add(lblExistingTotal, 1, 4);
            existingLayout.Controls.Add(btnCancelSubscription, 1, 5);
            groupExistingSubscription.Controls.Add(existingLayout);
            groupExistingSubscription.Visible = false;
            this.Controls.Add(groupExistingSubscription);

            // New subscription group
            groupNewSubscription = new GroupBox() { Text = "Buy New Subscription", Dock = DockStyle.Fill };
            nudNewDuration = new NumericUpDown() { Minimum = 1, Maximum = 24, Value = 1 };
            nudNewPrice = new NumericUpDown() { Minimum = 0, Maximum = 10000, DecimalPlaces = 2, Value = 900 };
            dtpNewStart = new DateTimePicker() { Format = DateTimePickerFormat.Short, Value = DateTime.Today };
            lblNewTotal = new Label() { Text = "Total Price:", AutoSize = true };
            lblNewEnd = new Label() { Text = "End Date:", AutoSize = true };
            btnBuySubscription = new Button() { Text = "Buy Subscription", AutoSize = true };
            btnBuySubscription.Click += async (s, e) => await BuySubscriptionAsync();

            nudNewDuration.ValueChanged += (s, e) => UpdateNewComputedFields();
            nudNewPrice.ValueChanged += (s, e) => UpdateNewComputedFields();
            dtpNewStart.ValueChanged += (s, e) => UpdateNewComputedFields();

            var newLayout = new TableLayoutPanel() { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 5 };
            newLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            newLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            newLayout.Controls.Add(new Label() { Text = "Duration (months):", Anchor = AnchorStyles.Right, AutoSize = true }, 0, 0);
            newLayout.Controls.Add(nudNewDuration, 1, 0);
            newLayout.Controls.Add(new Label() { Text = "Price per month:", Anchor = AnchorStyles.Right, AutoSize = true }, 0, 1);
            newLayout.Controls.Add(nudNewPrice, 1, 1);
            newLayout.Controls.Add(new Label() { Text = "Start Date:", Anchor = AnchorStyles.Right, AutoSize = true }, 0, 2);
            newLayout.Controls.Add(dtpNewStart, 1, 2);
            newLayout.Controls.Add(new Label() { Text = "Total Price:", Anchor = AnchorStyles.Right, AutoSize = true }, 0, 3);
            newLayout.Controls.Add(lblNewTotal, 1, 3);
            newLayout.Controls.Add(new Label() { Text = "End Date:", Anchor = AnchorStyles.Right, AutoSize = true }, 0, 4);
            newLayout.Controls.Add(lblNewEnd, 1, 4);

            var panelBuy = new FlowLayoutPanel() { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Bottom, Height = 40 };
            panelBuy.Controls.Add(btnBuySubscription);
            groupNewSubscription.Controls.Add(newLayout);
            groupNewSubscription.Controls.Add(panelBuy);
            groupNewSubscription.Visible = false;
            this.Controls.Add(groupNewSubscription);
        }

        private async Task LoadSubscriptionAsync()
        {
            _subscription = await _subscriptionService.GetSubscriptionByUserIdAsync(_userId);
            if (_subscription != null)
            {
                groupExistingSubscription.Visible = true;
                groupNewSubscription.Visible = false;
                lblExistingStart.Text = _subscription.StartDate.ToShortDateString();
                lblExistingEnd.Text = _subscription.EndDate.ToShortDateString();
                lblExistingDuration.Text = _subscription.DurationMonths.ToString();
                lblExistingPrice.Text = _subscription.PricePerMonth.ToString("C2");
                lblExistingTotal.Text = _subscription.TotalPrice.ToString("C2");
            }
            else
            {
                groupExistingSubscription.Visible = false;
                groupNewSubscription.Visible = true;
                UpdateNewComputedFields();
            }
        }

        private void UpdateNewComputedFields()
        {
            int duration = (int)nudNewDuration.Value;
            double price = (double)nudNewPrice.Value;
            double total = duration * price;
            lblNewTotal.Text = total.ToString("C2");
            DateTime start = dtpNewStart.Value;
            DateTime end = start.AddMonths(duration);
            lblNewEnd.Text = end.ToShortDateString();
        }

        private async Task BuySubscriptionAsync()
        {
            int duration = (int)nudNewDuration.Value;
            double price = (double)nudNewPrice.Value;
            DateTime start = dtpNewStart.Value;
            double total = duration * price;
            DateTime end = start.AddMonths(duration);

            Subscription newSub = new Subscription
            {
                UserId = _userId,
                DurationMonths = duration,
                PricePerMonth = price,
                TotalPrice = total,
                StartDate = start,
                EndDate = end,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            bool success = await _subscriptionService.InsertSubscriptionAsync(newSub);
            if (success)
            {
                MessageBox.Show("Subscription has been purchased successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadSubscriptionAsync();
            }
            else
            {
                MessageBox.Show("Failed to purchase subscription.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CancelSubscriptionAsync()
        {
            if (_subscription == null)
                return;

            var result = MessageBox.Show("Are you sure you want to cancel the subscription?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                bool success = await _subscriptionService.DeleteSubscriptionAsync(_subscription.Id);
                if (success)
                {
                    MessageBox.Show("Subscription has been canceled.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadSubscriptionAsync();
                }
                else
                {
                    MessageBox.Show("Failed to cancel the subscription.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}