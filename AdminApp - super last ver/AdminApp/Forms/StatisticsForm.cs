using AdminApp.Models;
using AdminApp.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdminApp.Forms
{
    public class StatisticsForm : Form
    {
        // Required services.
        private readonly UserService _userService;
        private readonly PurchaseService _purchaseService;
        private readonly GymVisitService _gymVisitService;
        private readonly TrainingService _trainingService;
        private readonly SubscriptionService _subscriptionService;
        private readonly TrainerService _trainerService;
        private readonly ProductService _productService;

        // UI Controls.
        private ComboBox cmbPeriod;
        private Button btnLoad;
        
        // Labels for the basic six statistics.
        private Label lblTotalMoneySpent;
        private Label lblTotalUsers;
        private Label lblNewUsers;
        private Label lblProductsSold;
        private Label lblTrainingsHeld;
        private Label lblGymVisits;
        
        // Additional statistics.
        private Label lblPurchaseRevenue;
        private Label lblSubscriptionRevenue;
        private Label lblTrainingRevenue;
        private Label lblAvgPurchaseAmount;
        private Label lblTotalSubscriptionsSold;
        private Label lblAvgTrainingAttendance;
        private Label lblTotalTrainers;
        private Label lblMostPopularTraining;
        private Label lblMostActiveUser;
        private Label lblAvgGymVisitDuration;
        private Label lblProductInventoryValue;
        private Label lblTotalUserBalance;
        private Label lblAvgSubscriptionDuration;
        private Label lblActiveSubscriptionsCount;
        private Label lblPercentageActiveSubscriptions;

        public StatisticsForm(
            UserService userService,
            PurchaseService purchaseService,
            GymVisitService gymVisitService,
            TrainingService trainingService,
            SubscriptionService subscriptionService,
            TrainerService trainerService,
            ProductService productService)
        {
            _userService = userService;
            _purchaseService = purchaseService;
            _gymVisitService = gymVisitService;
            _trainingService = trainingService;
            _subscriptionService = subscriptionService;
            _trainerService = trainerService;
            _productService = productService;

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Statistics";
            this.Size = new Size(550, 750);
            this.StartPosition = FormStartPosition.CenterParent;
            this.AutoScroll = true;

            // Period selection controls.
            Label lblSelectPeriod = new Label
            {
                Text = "Select Period:",
                AutoSize = true,
                Left = 20,
                Top = 20
            };

            cmbPeriod = new ComboBox
            {
                Left = 140,
                Top = 15,
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbPeriod.Items.AddRange(new string[] { "Month", "6 Months", "Year", "All Time" });
            cmbPeriod.SelectedIndex = 0;

            btnLoad = new Button
            {
                Text = "Load Statistics",
                Left = 310,
                Top = 15,
                Width = 150
            };
            btnLoad.Click += async (s, e) => await LoadStatisticsAsync();

            // Create statistic labels.
            // We'll use a vertical spacing of 30 pixels starting at y = 60.
            int baseTop = 60;
            int spacing = 30;
            lblTotalMoneySpent = CreateStatLabel("Total Money Spent (Overall Revenue): $0.00", baseTop + spacing * 0);
            lblTotalUsers = CreateStatLabel("Total Users: 0", baseTop + spacing * 1);
            lblNewUsers = CreateStatLabel("New Users: 0", baseTop + spacing * 2);
            lblProductsSold = CreateStatLabel("Products Sold: 0", baseTop + spacing * 3);
            lblTrainingsHeld = CreateStatLabel("Trainings Held: 0", baseTop + spacing * 4);
            lblGymVisits = CreateStatLabel("Gym Visits: 0", baseTop + spacing * 5);
            lblPurchaseRevenue = CreateStatLabel("Purchase Revenue: $0.00", baseTop + spacing * 6);
            lblSubscriptionRevenue = CreateStatLabel("Subscription Revenue: $0.00", baseTop + spacing * 7);
            lblTrainingRevenue = CreateStatLabel("Training Revenue: $0.00", baseTop + spacing * 8);
            lblAvgPurchaseAmount = CreateStatLabel("Avg Purchase Amount: $0.00", baseTop + spacing * 9);
            lblTotalSubscriptionsSold = CreateStatLabel("Total Subscriptions Sold: 0", baseTop + spacing * 10);
            lblAvgTrainingAttendance = CreateStatLabel("Avg Training Attendance: 0", baseTop + spacing * 11);
            lblTotalTrainers = CreateStatLabel("Total Trainers: 0", baseTop + spacing * 12);
            lblMostPopularTraining = CreateStatLabel("Most Popular Training: N/A", baseTop + spacing * 13);
            lblMostActiveUser = CreateStatLabel("Most Active User: N/A", baseTop + spacing * 14);
            lblAvgGymVisitDuration = CreateStatLabel("Avg Gym Visit Duration: 0 min", baseTop + spacing * 15);
            lblProductInventoryValue = CreateStatLabel("Product Inventory Value: $0.00", baseTop + spacing * 16);
            lblTotalUserBalance = CreateStatLabel("Total User Balance: $0.00", baseTop + spacing * 17);
            lblAvgSubscriptionDuration = CreateStatLabel("Avg Subscription Duration: 0 months", baseTop + spacing * 18);
            lblActiveSubscriptionsCount = CreateStatLabel("Active Subscriptions: 0", baseTop + spacing * 19);
            lblPercentageActiveSubscriptions = CreateStatLabel("Percentage with Active Subscriptions: 0%", baseTop + spacing * 20);

            // Add controls.
            this.Controls.Add(lblSelectPeriod);
            this.Controls.Add(cmbPeriod);
            this.Controls.Add(btnLoad);
            this.Controls.Add(lblTotalMoneySpent);
            this.Controls.Add(lblTotalUsers);
            this.Controls.Add(lblNewUsers);
            this.Controls.Add(lblProductsSold);
            this.Controls.Add(lblTrainingsHeld);
            this.Controls.Add(lblGymVisits);
            this.Controls.Add(lblPurchaseRevenue);
            this.Controls.Add(lblSubscriptionRevenue);
            this.Controls.Add(lblTrainingRevenue);
            this.Controls.Add(lblAvgPurchaseAmount);
            this.Controls.Add(lblTotalSubscriptionsSold);
            this.Controls.Add(lblAvgTrainingAttendance);
            this.Controls.Add(lblTotalTrainers);
            this.Controls.Add(lblMostPopularTraining);
            this.Controls.Add(lblMostActiveUser);
            this.Controls.Add(lblAvgGymVisitDuration);
            this.Controls.Add(lblProductInventoryValue);
            this.Controls.Add(lblTotalUserBalance);
            this.Controls.Add(lblAvgSubscriptionDuration);
            this.Controls.Add(lblActiveSubscriptionsCount);
            this.Controls.Add(lblPercentageActiveSubscriptions);
        }

        private Label CreateStatLabel(string text, int top)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Left = 20,
                Top = top,
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };
        }

        private async Task LoadStatisticsAsync()
        {
            // Determine the start date based on the selected period.
            DateTime? periodStart = null;
            string period = cmbPeriod.SelectedItem.ToString();
            switch (period)
            {
                case "Month":
                    periodStart = DateTime.UtcNow.AddMonths(-1);
                    break;
                case "6 Months":
                    periodStart = DateTime.UtcNow.AddMonths(-6);
                    break;
                case "Year":
                    periodStart = DateTime.UtcNow.AddYears(-1);
                    break;
                case "All Time":
                    periodStart = null;
                    break;
            }

            // --- PURCHASES ---
            List<Purchase> allPurchases = await _purchaseService.GetAllPurchasesAsync();
            List<Purchase> purchasesInPeriod = periodStart.HasValue
                ? allPurchases.Where(p => p.PurchaseDate >= periodStart.Value).ToList()
                : allPurchases;
            double purchaseRevenue = purchasesInPeriod.Sum(p => p.TotalAmount);
            int numPurchases = purchasesInPeriod.Count;
            double avgPurchaseAmount = numPurchases > 0 ? purchaseRevenue / numPurchases : 0;

            // --- SUBSCRIPTIONS ---
            List<Subscription> allSubscriptions = await _subscriptionService.GetAllSubscriptionsAsync();
            List<Subscription> subscriptionsInPeriod = periodStart.HasValue
                ? allSubscriptions.Where(s => s.StartDate >= periodStart.Value).ToList()
                : allSubscriptions;
            double subscriptionRevenue = subscriptionsInPeriod.Sum(s => s.TotalPrice);
            int totalSubscriptionsSold = subscriptionsInPeriod.Count;
            double avgSubscriptionDuration = totalSubscriptionsSold > 0
                ? subscriptionsInPeriod.Average(s => s.DurationMonths)
                : 0;
            int activeSubscriptionsCount = allSubscriptions.Count(s => s.EndDate >= DateTime.UtcNow);

            // --- TRAININGS ---
            List<Training> allTrainings = await _trainingService.GetAllTrainingsAsync();
            List<Training> trainingsInPeriod = periodStart.HasValue
                ? allTrainings.Where(t => t.Schedule >= periodStart.Value).ToList()
                : allTrainings;
            double trainingRevenue = 0;
            int totalEnrollment = 0;
            Training mostPopularTraining = null;
            int maxEnrollment = 0;
            foreach (var training in trainingsInPeriod)
            {
                int enrollment = training.EnrolledUsers?.Count ?? 0;
                totalEnrollment += enrollment;
                if (!string.IsNullOrEmpty(training.Type) && training.Type.ToLower() == "group")
                {
                    if (training.GroupPrice.HasValue)
                        trainingRevenue += training.GroupPrice.Value * enrollment;
                }
                else if (!string.IsNullOrEmpty(training.Type) && training.Type.ToLower() == "personal")
                {
                    var trainer = await _trainerService.GetTrainerByIdAsync(training.TrainerId);
                    if (trainer != null)
                        trainingRevenue += trainer.TrainingPrice * enrollment;
                }
                if (enrollment > maxEnrollment)
                {
                    maxEnrollment = enrollment;
                    mostPopularTraining = training;
                }
            }
            double avgTrainingAttendance = trainingsInPeriod.Count > 0
                ? (double)totalEnrollment / trainingsInPeriod.Count
                : 0;
            int trainingsHeld = trainingsInPeriod.Count;

            // --- OVERALL REVENUE ---
            double overallRevenue = purchaseRevenue + subscriptionRevenue + trainingRevenue;

            // --- USERS ---
            List<Models.User> allUsers = await _userService.GetAllUsersAsync();
            int totalUsers = allUsers.Count;
            int newUsers = periodStart.HasValue
                ? allUsers.Count(u => u.CreatedAt >= periodStart.Value)
                : 0;
            double totalUserBalance = allUsers.Sum(u => u.Balance);

            // --- GYM VISITS ---
            List<GymVisit> allGymVisits = await _gymVisitService.GetAllGymVisitsAsync();
            List<GymVisit> gymVisitsInPeriod = periodStart.HasValue
                ? allGymVisits.Where(v => v.CheckInTime >= periodStart.Value).ToList()
                : allGymVisits;
            int gymVisitsCount = gymVisitsInPeriod.Count;
            var durations = gymVisitsInPeriod
                .Where(v => v.CheckOutTime.HasValue)
                .Select(v => (v.CheckOutTime.Value - v.CheckInTime).TotalMinutes)
                .ToList();
            double avgGymVisitDuration = durations.Count > 0 ? durations.Average() : 0;

            // Most Active User (by gym visits)
            string mostActiveUserText = "N/A";
            if (gymVisitsInPeriod.Any())
            {
                var userVisitGroups = gymVisitsInPeriod.GroupBy(v => v.UserId)
                                                       .Select(g => new { UserId = g.Key, Count = g.Count() })
                                                       .OrderByDescending(g => g.Count)
                                                       .ToList();
                var mostActiveGroup = userVisitGroups.FirstOrDefault();
                if (mostActiveGroup != null)
                {
                    var mostActiveUser = allUsers.FirstOrDefault(u => u.Id == mostActiveGroup.UserId);
                    if (mostActiveUser != null)
                        mostActiveUserText = $"{mostActiveUser.Username} ({mostActiveGroup.Count} visits)";
                }
            }

            // --- PRODUCTS ---
            List<Product> allProducts = await _productService.GetAllProductsAsync();
            double productInventoryValue = allProducts.Sum(p => p.Price * p.Quantity);

            // --- TRAINER COUNT ---
            List<Trainer> allTrainers = await _trainerService.GetAllTrainersAsync();
            int totalTrainers = allTrainers.Count;

            // --- ACTIVE SUBSCRIPTIONS ---
            double percentageActiveSubscriptions = totalUsers > 0
                ? (activeSubscriptionsCount / (double)totalUsers) * 100
                : 0;

            // --- UPDATE UI LABELS ---
            lblTotalMoneySpent.Text = $"Total Money Spent (Overall Revenue): ${overallRevenue:N2}";
            lblTotalUsers.Text = $"Total Users: {totalUsers}";
            lblNewUsers.Text = $"New Users: {newUsers}";
            lblProductsSold.Text = $"Products Sold: {purchasesInPeriod.Sum(p => p.Items.Sum(i => i.Quantity))}";
            lblTrainingsHeld.Text = $"Trainings Held: {trainingsHeld}";
            lblGymVisits.Text = $"Gym Visits: {gymVisitsCount}";
            lblPurchaseRevenue.Text = $"Purchase Revenue: ${purchaseRevenue:N2}";
            lblSubscriptionRevenue.Text = $"Subscription Revenue: ${subscriptionRevenue:N2}";
            lblTrainingRevenue.Text = $"Training Revenue: ${trainingRevenue:N2}";
            lblAvgPurchaseAmount.Text = $"Avg Purchase Amount: ${avgPurchaseAmount:N2}";
            lblTotalSubscriptionsSold.Text = $"Total Subscriptions Sold: {totalSubscriptionsSold}";
            lblAvgTrainingAttendance.Text = $"Avg Training Attendance: {avgTrainingAttendance:F1}";
            lblTotalTrainers.Text = $"Total Trainers: {totalTrainers}";
            lblMostPopularTraining.Text = mostPopularTraining != null
                ? $"Most Popular Training: {mostPopularTraining.Title} ({maxEnrollment} enrollments)"
                : "Most Popular Training: N/A";
            lblMostActiveUser.Text = $"Most Active User: {mostActiveUserText}";
            lblAvgGymVisitDuration.Text = $"Avg Gym Visit Duration: {avgGymVisitDuration:F1} min";
            lblProductInventoryValue.Text = $"Product Inventory Value: ${productInventoryValue:N2}";
            lblTotalUserBalance.Text = $"Total User Balance: ${totalUserBalance:N2}";
            lblAvgSubscriptionDuration.Text = $"Avg Subscription Duration: {avgSubscriptionDuration:F1} months";
            lblActiveSubscriptionsCount.Text = $"Active Subscriptions: {activeSubscriptionsCount}";
            lblPercentageActiveSubscriptions.Text = $"Percentage with Active Subscriptions: {percentageActiveSubscriptions:F1}%";
        }
    }
}
