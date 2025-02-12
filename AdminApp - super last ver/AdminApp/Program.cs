using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using AdminApp.Forms;
using AdminApp.Services;
using AdminApp.DB;

namespace AdminApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Set up DI container.
            var services = new ServiceCollection();

            // Register DatabaseManager with connection details.
            services.AddSingleton<DatabaseManager>(new DatabaseManager(
                "mongodb+srv://Admin:strongpassword@cluster0.lrajj.mongodb.net", 
                "GymDatabase"));

            // Register all services.
            services.AddSingleton<UserService>();
            services.AddSingleton<PurchaseService>();       // <-- Required for revenue from purchases.
            services.AddSingleton<GymVisitService>();
            services.AddSingleton<TrainingService>();
            services.AddSingleton<TrainerService>();
            services.AddSingleton<SubscriptionService>();
            services.AddSingleton<ProductService>();

            // Register forms.
            services.AddTransient<MainForm>();
            services.AddTransient<UserListForm>();
            services.AddTransient<AddUserForm>();
            services.AddTransient<TrainingListForm>();
            services.AddTransient<TrainerListForm>();
            services.AddTransient<SubscriptionListForm>();
            services.AddTransient<GymAttendanceMenuForm>();
            services.AddTransient<ProductListForm>();
            services.AddTransient<PurchaseHistoryForm>();
            // Note: StatisticsForm will be resolved with its dependencies.
            services.AddTransient<StatisticsForm>();

            var serviceProvider = services.BuildServiceProvider();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(serviceProvider.GetRequiredService<MainForm>());
        }
    }
}