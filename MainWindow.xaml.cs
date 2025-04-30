using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using src.Views; // Import Views namespace
using src.Services;
using src.Repos; // Import Services namespace
using src.Data;
using System.Collections.Generic;
using src.Model;
using src.View;
using src.View.Components;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace src
{
    public sealed partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;

        public MainWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;

            // Navigate to an initial page if needed:
            //MainFrame.Navigate(typeof(UsersView));
            var usersView = _serviceProvider.GetRequiredService<UsersView>();
            MainFrame.Content = usersView;
        }


        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer != null)
            {
               string invokedItemTag = args.SelectedItemContainer.Tag.ToString();
                switch (invokedItemTag)
                {
                    case "ChatReports":
                        var chatReportsPage = _serviceProvider.GetRequiredService<ChatReportComponent>();
                        MainFrame.Content = chatReportsPage;
                        break;
                    case "LoanRequest":
                        var loanRequestPage = _serviceProvider.GetRequiredService<LoanRequestView>();
                        MainFrame.Content = loanRequestPage;
                        break;
                    case "Loans":
                        var loansPage = _serviceProvider.GetRequiredService<LoansView>();
                        MainFrame.Content = loansPage;
                        break;
                    case "UsersList":
                        var usersView = _serviceProvider.GetRequiredService<UsersView>();
                        MainFrame.Content = usersView;
                        break;
                    case "BillSplitReports":
                        var billSplitPage = _serviceProvider.GetRequiredService<BillSplitReportPage>();
                        MainFrame.Content = billSplitPage;
                        break;
                    case "Zodiac":
                        ZodiacFeature(sender, null);
                        break;
                    case "Investments":
                        MainFrame.Navigate(typeof(InvestmentsView));
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

            MainFrame.Navigate(typeof(ZodiacFeatureView));
        }
    }
}
