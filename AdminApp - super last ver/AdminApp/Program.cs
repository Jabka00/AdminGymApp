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
            var services = new ServiceCollection();

            services.AddSingleton<DatabaseManager>(new DatabaseManager(
                "mongodb+srv://Admin:strongpassword@cluster0.lrajj.mongodb.net", 
                "GymDatabase"));

            services.AddSingleton<UserService>();
            services.AddSingleton<PurchaseService>();      
            services.AddSingleton<GymVisitService>();
            services.AddSingleton<TrainingService>();
            services.AddSingleton<TrainerService>();
            services.AddSingleton<SubscriptionService>();
            services.AddSingleton<ProductService>();

            services.AddTransient<MainForm>();
            services.AddTransient<UserListForm>();
            services.AddTransient<AddUserForm>();
            services.AddTransient<TrainingListForm>();
            services.AddTransient<TrainerListForm>();
            services.AddTransient<SubscriptionListForm>();
            services.AddTransient<GymAttendanceMenuForm>();
            services.AddTransient<ProductListForm>();
            services.AddTransient<PurchaseHistoryForm>();
            services.AddTransient<StatisticsForm>();

            var serviceProvider = services.BuildServiceProvider();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(serviceProvider.GetRequiredService<MainForm>());
        }
    }
}