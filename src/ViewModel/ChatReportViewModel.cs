﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Src.Model;
using Src.Services;

namespace Src.ViewModel
{
    public class ChatReportsViewModel
    {
        private readonly IChatReportService chatReportService;

        public ObservableCollection<ChatReport> ChatReports { get; set; }

        public ChatReportsViewModel()
        {
            ChatReports = new ObservableCollection<ChatReport>();
        }

        public async Task LoadChatReports()
        {
            try
            {
                var reports = chatReportService.GetChatReports();
                foreach (var report in reports)
                {
                    ChatReports.Add(report);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error: {exception.Message}");
            }
        }
    }
}
