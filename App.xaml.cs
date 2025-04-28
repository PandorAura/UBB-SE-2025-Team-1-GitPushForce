using System;
using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using src.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace src
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider Services { get; }
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IActivityService, ActivityService>();
            services.AddSingleton<IBillSplitReportService, BillSplitReportService>();
            services.AddSingleton<IChatReportService, ChatReportService>();
            services.AddSingleton<IHistoryService, HistoryService>();
            services.AddSingleton<IInvestmentsService, InvestmentsService>();
            services.AddSingleton<ILoanCheckerService, LoanCheckerService>();
            services.AddSingleton<ILoanRequestService, LoanRequestService>();
            services.AddSingleton<ILoanService, LoanService>();
            services.AddSingleton<IMessagesService, MessagesService>();
            services.AddSingleton<ITipsService, TipsService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IZodiacService, ZodiacService>();
            return services.BuildServiceProvider();
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
