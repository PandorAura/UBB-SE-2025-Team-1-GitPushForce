using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using src.Repos;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace src
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static IHost Host { get; private set; }
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.ConfigureHost();
        }
        private void ConfigureHost()
        {
            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
            {
                var config = new ConfigurationBuilder().AddUserSecrets<App>().AddEnvironmentVariables().Build();
                services.AddSingleton<IConfiguration>(config);

                services.AddSingleton<IActivityRepository, ActivityRepository>();
                services.AddSingleton<IBillSplitReportRepository, BillSplitReportRepository>();
                services.AddSingleton<IChatReportRepository, ChatReportRepository>();
                services.AddSingleton<IHistoryRepository, HistoryRepository>();
                services.AddSingleton<IInvestmentsRepository, InvestmentsRepository>();
                services.AddSingleton<ILoanRepository, LoanRepository>();
                services.AddSingleton<ILoanRequestRepository, LoanRequestRepository>();
                services.AddSingleton<IUserRepository, UserRepository>();
            }).Build();  
        }
        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
        }

        private Window? m_window;
    }
}
