using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using src.Data;
using src.Repos;
using src.Services;
using src.Model;
using src.View.Components;

namespace src.Views
{
    public sealed partial class LoanRequestView : Page
    {
        public LoanRequestView()
        {
            this.InitializeComponent();
            LoadLoanRequests();
        }

        private void LoadLoanRequests()
        {
            LoanRequestContainer.Items.Clear(); // Clear previous items before reloading

            DatabaseConnection dbConnection = new DatabaseConnection();
            LoanRequestRepository loanRequestRepository = new LoanRequestRepository(dbConnection);
            LoanRequestService loanRequestService = new LoanRequestService(loanRequestRepository);

            try
            {
                List<LoanRequest> loanRequests = loanRequestService.GetUnsolvedLoanRequests();

                foreach (var request in loanRequests)
                {
                    LoanRequestComponent requestComponent = new LoanRequestComponent();
                    requestComponent.SetRequestData(request.Id, request.UserCnp, request.Amount, request.ApplicationDate, request.RepaymentDate, request.Status, loanRequestService.GiveSuggestion(request));

                    // Subscribe to the event to refresh when a request is solved
                    requestComponent.LoanRequestSolved += OnLoanRequestSolved;

                    LoanRequestContainer.Items.Add(requestComponent);
                }
            }
            catch (Exception)
            {
                LoanRequestContainer.Items.Add("There are no loan requests that need solving.");
            }
        }

        private void OnLoanRequestSolved(object sender, EventArgs e)
        {
            LoadLoanRequests(); // Refresh the list instantly when a request is solved
        }
    }
}
