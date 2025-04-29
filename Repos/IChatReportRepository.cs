using src.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Repos
{
    public interface IChatReportRepository
    {
        public List<ChatReport> GetChatReports();

        public void DeleteChatReport(Int32 id);

        public void UpdateHistoryForUser(string UserCNP, int NewScore);

    }
}
