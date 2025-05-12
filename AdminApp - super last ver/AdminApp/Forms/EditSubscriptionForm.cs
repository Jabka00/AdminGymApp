
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using AdminApp.Models;
using AdminApp.Services;

namespace AdminApp.Forms
{
    public class EditSubscriptionForm : Form
    {
        private readonly SubscriptionService _subscriptionService;
        private Subscription? _subscription;
        private readonly string _userId;

        private NumericUpDown? nudDurationMonths, nudPricePerMonth;
        private DateTimePicker? dtpStartDate;
        private Label? lblTotalPrice, lblEndDate;
        private Button? btnSave, btnCancel;

        public EditSubscriptionForm(string userId, SubscriptionService subscriptionService)
        {
            _userId = userId;
            _subscriptionService = subscriptionService;
            InitializeComponents();
            LoadSubscriptionData();
        }

        private void InitializeComponents()
        {
            this.Text = "Edit Subscription";
            this.Size = new System.Drawing.Size(400, 300);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            var tableLayout = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6,
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

            nudDurationMonths = new NumericUpDown() { Minimum = 1, Maximum = 24, Value = 1 };
            AddRow("Duration (months):", nudDurationMonths);

            nudPricePerMonth = new NumericUpDown() { Minimum = 0, Maximum = 10000, DecimalPlaces = 2, Value = 0 };
            AddRow("Price per month:", nudPricePerMonth);

            dtpStartDate = new DateTimePicker() { Format = DateTimePickerFormat.Short };
            AddRow("Start Date:", dtpStartDate);

            lblTotalPrice = new Label() { Text = "0", AutoSize = true };
            AddRow("Total Price:", lblTotalPrice);

            lblEndDate = new Label() { Text = "N/A", AutoSize = true };
            AddRow("End Date:", lblEndDate);

            nudDurationMonths.ValueChanged += (s, e) => UpdateComputedFields();
            nudPricePerMonth.ValueChanged += (s, e) => UpdateComputedFields();
            dtpStartDate.ValueChanged += (s, e) => UpdateComputedFields();

            btnSave = new Button() { Text = "Save", Width = 100 };
            btnSave.Click += async (s, e) => await SaveSubscription();

            btnCancel = new Button() { Text = "Cancel", Width = 100 };
            btnCancel.Click += (s, e) => this.Close();

            var flowPanel = new FlowLayoutPanel() { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Bottom };
            flowPanel.Controls.Add(btnCancel);
            flowPanel.Controls.Add(btnSave);

            tableLayout.Controls.Add(flowPanel, 1, tableLayout.RowCount - 1);

            this.Controls.Add(tableLayout);
        }

        private async void LoadSubscriptionData()
        {
            _subscription = await _subscriptionService.GetSubscriptionByUserIdAsync(_userId);
            if (_subscription != null)
            {
                nudDurationMonths.Value = _subscription.DurationMonths;
                nudPricePerMonth.Value = (decimal)_subscription.PricePerMonth;
                dtpStartDate.Value = _subscription.StartDate;
            }
            UpdateComputedFields();
        }

        private void UpdateComputedFields()
        {
            int duration = (int)nudDurationMonths.Value;
            double pricePerMonth = (double)nudPricePerMonth.Value;
            double totalPrice = duration * pricePerMonth;
            lblTotalPrice.Text = totalPrice.ToString("C2");

            DateTime startDate = dtpStartDate.Value;
            DateTime endDate = startDate.AddMonths(duration);
            lblEndDate.Text = endDate.ToShortDateString();
        }

        private async Task SaveSubscription()
        {
            int duration = (int)nudDurationMonths.Value;
            double pricePerMonth = (double)nudPricePerMonth.Value;
            DateTime startDate = dtpStartDate.Value;
            double totalPrice = duration * pricePerMonth;
            DateTime endDate = startDate.AddMonths(duration);

            if (_subscription == null)
            {
                var newSubscription = new Subscription
                {
                    UserId = _userId,
                    DurationMonths = duration,
                    PricePerMonth = pricePerMonth,
                    TotalPrice = totalPrice,
                    StartDate = startDate,
                    EndDate = endDate,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                bool success = await _subscriptionService.InsertSubscriptionAsync(newSubscription);
                if (success)
                {
                    MessageBox.Show("Subscription has been successfully created!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to create the subscription.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                _subscription.DurationMonths = duration;
                _subscription.PricePerMonth = pricePerMonth;
                _subscription.TotalPrice = totalPrice;
                _subscription.StartDate = startDate;
                _subscription.EndDate = endDate;
                _subscription.UpdatedAt = DateTime.UtcNow;
                bool success = await _subscriptionService.UpdateSubscriptionAsync(_subscription);
                if (success)
                {
                    MessageBox.Show("Subscription has been successfully updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to update the subscription.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}