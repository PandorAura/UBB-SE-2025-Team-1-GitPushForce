using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using src.Services;
using src.Model;
using src.Data;
using src.Repos;
using System;

namespace src.View.Components
{
    public sealed partial class LoanRequestComponent : Page
    {
        private readonly LoanRequestService _loanRequestService;
        private readonly LoanServices _loanServices;
        public event EventHandler LoanRequestSolved;

        // Properties
        public int RequestID { get; set; }
        public string RequestingUserCNP { get; set; }
        public float RequestedAmount { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime RepaymentDate { get; set; }
        public string State { get; set; }
        public string Suggestion { get; set; }

        public LoanRequestComponent()
        {
            this.InitializeComponent();
            _loanRequestService = new LoanRequestService(new LoanRequestRepository(new DatabaseConnection()));
            _loanServices = new LoanServices(new LoanRepository(new DatabaseConnection()));
        }

        private async void OnDenyClick(object sender, RoutedEventArgs e)
        {
            LoanRequest loanRequest = new LoanRequest(RequestID, RequestingUserCNP, RequestedAmount, ApplicationDate, RepaymentDate, State);
            _loanRequestService.DenyLoanRequest(loanRequest);
            LoanRequestSolved?.Invoke(this, EventArgs.Empty);
        }

        private async void OnApproveClick(object sender, RoutedEventArgs e)
        {
            LoanRequest loanRequest = new LoanRequest(RequestID, RequestingUserCNP, RequestedAmount, ApplicationDate, RepaymentDate, State);
            _loanServices.AddLoan(loanRequest);
            _loanRequestService.SolveLoanRequest(loanRequest);
            LoanRequestSolved?.Invoke(this, EventArgs.Empty);
        }

        public void SetRequestData(int id, string requestingUserCnp, float requestedAmount, DateTime applicationDate, DateTime repaymentDate, string state, string suggestion)
        {
            RequestID = id;
            RequestingUserCNP = requestingUserCnp;
            RequestedAmount = requestedAmount;
            ApplicationDate = applicationDate;
            RepaymentDate = repaymentDate;
            State = state;
            Suggestion = suggestion;


            // Update UI elements
            IdTextBlock.Text = $"ID: {id}";
            RequestingUserCNPTextBlock.Text = $"User CNP: {requestingUserCnp}";
            RequestedAmountTextBlock.Text = $"Amount: {requestedAmount}";
            ApplicationDateTextBlock.Text = $"Application Date: {applicationDate:yyyy-MM-dd}";
            RepaymentDateTextBlock.Text = $"Repayment Date: {repaymentDate:yyyy-MM-dd}";
            SuggestionTextBlock.Text = $"{suggestion}";
        }
    }
}
