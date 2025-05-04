namespace Src
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Src.Data;
    using Src.Repos;
    using Src.Services;
    using Src.View;
    using Src.View.Components;
    using Src.Views;

    public sealed partial class MainWindow : Window
    {
        private readonly IServiceProvider serviceProvider;

        public MainWindow(IServiceProvider serviceProvider)
        {
            this.InitializeComponent();
            this.serviceProvider = serviceProvider;

            var usersView = this.serviceProvider.GetRequiredService<UsersView>();
            this.MainFrame.Content = usersView;
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer != null)
            {
               string invokedItemTag = args.SelectedItemContainer.Tag.ToString();
                switch (invokedItemTag)
                {
                    case "ChatReports":
                        var chatReportsPage = this.serviceProvider.GetRequiredService<ChatReportView>();
                        this.MainFrame.Content = chatReportsPage;
                        break;
                    case "LoanRequest":
                        var loanRequestPage = this.serviceProvider.GetRequiredService<LoanRequestView>();
                        this.MainFrame.Content = loanRequestPage;
                        break;
                    case "Loans":
                        var loansPage = this.serviceProvider.GetRequiredService<LoansView>();
                        this.MainFrame.Content = loansPage;
                        break;
                    case "UsersList":
                        var usersView = this.serviceProvider.GetRequiredService<UsersView>();
                        this.MainFrame.Content = usersView;
                        break;
                    case "BillSplitReports":
                        var billSplitPage = this.serviceProvider.GetRequiredService<BillSplitReportPage>();
                        this.MainFrame.Content = billSplitPage;
                        break;
                    case "Zodiac":
                        this.ZodiacFeature(sender, null);
                        break;
                    case "Investments":
                        this.MainFrame.Navigate(typeof(InvestmentsView));
                        break;
                }
            }
        }

        private void ZodiacFeature(object sender, RoutedEventArgs e)
        {
            DatabaseConnection dbConn = new DatabaseConnection();
            UserRepository userRepository = new UserRepository(dbConn);
            ZodiacService zodiacService = new ZodiacService(userRepository);

            zodiacService.CreditScoreModificationBasedOnJokeAndCoinFlipAsync();
            zodiacService.CreditScoreModificationBasedOnAttributeAndGravity();

            this.MainFrame.Navigate(typeof(ZodiacFeatureView));
        }
    }
}
