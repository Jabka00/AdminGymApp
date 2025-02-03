using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using AdminApp.DB;
using AdminApp.Forms;
using AdminApp.Services;
using System.Threading.Tasks;

namespace AdminApp
{
    public static class Program
    {
        public static ServiceProvider? ServiceProvider { get; private set; }

        [STAThread]
        public static async Task Main()
        {
            // Настройка сервисов DI
            var services = new ServiceCollection();
            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();

            // Попытка подключения к базе данных
            var dbManager = ServiceProvider.GetRequiredService<DatabaseManager>();
            try
            {
                await dbManager.ConnectAsync();
            }
            catch
            {
                MessageBox.Show("Не удалось подключиться к базе данных. Приложение будет закрыто.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(ServiceProvider.GetRequiredService<MainForm>());
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            // Регистрация зависимостей
            services.AddSingleton<DatabaseManager>(provider =>
                new DatabaseManager("mongodb+srv://Admin:strongpassword@cluster0.lrajj.mongodb.net", "GymDatabase"));
            services.AddTransient<UserService>();
            services.AddTransient<TrainingService>();
            services.AddTransient<TrainerService>(); // Добавлено
            services.AddTransient<MainForm>();
            services.AddTransient<UserListForm>();
            services.AddTransient<AddUserForm>();
            services.AddTransient<TrainingListForm>();
            services.AddTransient<AddTrainingForm>();
            services.AddTransient<EditTrainingForm>();
            services.AddTransient<EnrollUserForm>();
            services.AddTransient<TrainerListForm>(); // Добавлено
            services.AddTransient<AddTrainerForm>();
            services.AddTransient<EditTrainerForm>();
            services.AddTransient<EnrolledUsersForm>(); // Добавлено
        }

    }
}
