using src.Model;
using System.Collections.Generic;

namespace src.Services
{
    public interface IHistoryService
    {
        public List<HistoryCreditScore> GetHistoryByUserCNP(string userCNP);
        public List<HistoryCreditScore> GetHistoryWeekly(string userCNP);
        public List<HistoryCreditScore> GetHistoryMonthly(string userCNP);
        public List<HistoryCreditScore> GetHistoryYearly(string userCNP);
    }
}
