using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using src.Data;
using src.Repos;
using src.Services;
using src.Model;
using src.View.Components;

namespace src.Views
{
    public sealed partial class LoanRequestView : Page
    {
        private readonly ILoanRequestService _service;
        private readonly Func<LoanRequestComponent> _componentFactory;
        public LoanRequestView(ILoanRequestService loanRequestService, Func<LoanRequestComponent> componentFactory)
        {
            this.InitializeComponent();
            _service = loanRequestService;
            _componentFactory = componentFactory;
            LoadLoanRequests();
        }

        private void LoadLoanRequests()
        {
            LoanRequestContainer.Items.Clear();

            try
            {
                List<LoanRequest> loanRequests = _service.GetUnsolvedLoanRequests();

                if (loanRequests.Count == 0)
                {
                    LoanRequestContainer.Items.Add("There are no loan requests that need solving.");
                    return;
                }

                foreach (var request in loanRequests)
                {
                    LoanRequestComponent requestComponent = _componentFactory();
                    requestComponent.SetRequestData(
                        request.RequestID,
                        request.UserCNP,
                        request.Amount,
                        request.ApplicationDate,
                        request.RepaymentDate,
                        request.State,
                        _service.GiveSuggestion(request)
                    );

                    requestComponent.LoanRequestSolved += OnLoanRequestSolved;

                    LoanRequestContainer.Items.Add(requestComponent);
                }
            }
            catch (Exception ex)
            {
                LoanRequestContainer.Items.Add($"Error loading loan requests: {ex.Message}");
            }
        }

        private void OnLoanRequestSolved(object sender, EventArgs e)
        {
            LoadLoanRequests(); // Refresh the list instantly when a request is solved
        }
    }
}
