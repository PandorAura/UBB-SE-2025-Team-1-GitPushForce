namespace Src.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Src.Model;
    using Src.Services;

    public class BillSplitReportViewModel
    {
        private readonly IBillSplitReportService billSplitReportService;

        public ObservableCollection<BillSplitReport> BillSplitReports { get; set; }

        public BillSplitReportViewModel()
        {
            this.BillSplitReports = new ObservableCollection<BillSplitReport>(this.billSplitReportService.GetBillSplitReports());
        }

        public async Task LoadBillSplitReports()
        {
            try
            {
                var reports = this.billSplitReportService.GetBillSplitReports();
                foreach (var report in reports)
                {
                    this.BillSplitReports.Add(report);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error: {exception.Message}");
            }
        }
    }
}
