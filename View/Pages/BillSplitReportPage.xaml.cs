using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using src.Data;
using src.Model;
using src.Repos;
using src.Services;
using src.View.Components;

namespace src.View
{
    public sealed partial class BillSplitReportPage : Page
    {
        public BillSplitReportPage()
        {
            this.InitializeComponent();
            LoadReports();
        }

        private void LoadReports()
        {
            BillSplitReportsContainer.Items.Clear(); // Clear previous items before reloading

            DatabaseConnection dbConnection = new DatabaseConnection();
            BillSplitReportRepository billSplitReportRepository = new BillSplitReportRepository(dbConnection);
            BillSplitReportService billSplitReportService = new BillSplitReportService(billSplitReportRepository);

            try
            {
                List<BillSplitReport> reports = billSplitReportService.GetBillSplitReports();

                foreach (var report in reports)
                {
                    BillSplitReportComponent reportComponent = new BillSplitReportComponent();
                    reportComponent.SetReportData(report);

                    // Subscribe to the event to refresh when a report is solved
                    reportComponent.ReportSolved += OnReportSolved;

                    BillSplitReportsContainer.Items.Add(reportComponent);
                }
            }
            catch (Exception)
            {
                BillSplitReportsContainer.Items.Add("There are no chat reports that need solving.");
            }
        }

        private void OnReportSolved(object sender, EventArgs e)
        {
            LoadReports(); // Refresh the list instantly when a report is solved
        }
    }
}
