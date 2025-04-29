using src.Repos;
using src.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using src.Helpers;
using src.Data;
using System.Data;
using Microsoft.Data.SqlClient;
using src.Services;

namespace src.Services
{
    public class ChatReportService
    {
        ChatReportRepository _chatReportRepository;

        public ChatReportService(ChatReportRepository chatReportRepository)
        {
            _chatReportRepository = chatReportRepository;
        }

        public void DoNotPunishUser(ChatReport chatReportToBeSolved)
        {
            _chatReportRepository.DeleteChatReport(chatReportToBeSolved.Id);
        }

        public async Task<bool> PunishUser(ChatReport chatReportToBeSolved)
        {
            DatabaseConnection dbConn = new DatabaseConnection();
            UserRepository userRepo = new UserRepository(dbConn);

            User reportedUser = userRepo.GetUserByCNP(chatReportToBeSolved.ReportedUserCnp);

            Int32 noOffenses = reportedUser.NumberOfOffenses;
            const Int32 MINIMUM_NUMBER_OF_OFFENSES_BEFORE_PUNISHMENT_GROWS_DISTOPIANLY_ABSURD = 3;
            const Int32 CREDIT_SCORE_DECREASE_AMOUNT_FLAT_RATE = 15;

            int amount;

            if (noOffenses >= MINIMUM_NUMBER_OF_OFFENSES_BEFORE_PUNISHMENT_GROWS_DISTOPIANLY_ABSURD)
            {
                userRepo.PenalizeUser(chatReportToBeSolved.ReportedUserCnp, noOffenses * CREDIT_SCORE_DECREASE_AMOUNT_FLAT_RATE);
                Int32 decrease = reportedUser.CreditScore - CREDIT_SCORE_DECREASE_AMOUNT_FLAT_RATE * noOffenses;
                UpdateHistoryForUser(chatReportToBeSolved.ReportedUserCnp, decrease);
                amount = CREDIT_SCORE_DECREASE_AMOUNT_FLAT_RATE * noOffenses;
            }
            else
            {
                userRepo.PenalizeUser(chatReportToBeSolved.ReportedUserCnp, CREDIT_SCORE_DECREASE_AMOUNT_FLAT_RATE);
                Int32 decrease = userRepo.GetUserByCNP(chatReportToBeSolved.ReportedUserCnp).CreditScore - CREDIT_SCORE_DECREASE_AMOUNT_FLAT_RATE;
                UpdateHistoryForUser(chatReportToBeSolved.ReportedUserCnp, decrease);
                amount = CREDIT_SCORE_DECREASE_AMOUNT_FLAT_RATE;
            }
            userRepo.IncrementOffenesesCountByOne(chatReportToBeSolved.ReportedUserCnp);
            _chatReportRepository.DeleteChatReport(chatReportToBeSolved.Id);
            TipsService service = new TipsService(new TipsRepository(dbConn));
            service.GiveTipToUser(chatReportToBeSolved.ReportedUserCnp);
            SqlParameter[] tipsParameters = new SqlParameter[]
            {
                 new SqlParameter("@UserCNP", chatReportToBeSolved.ReportedUserCnp)
            };
            
            int countTips = dbConn.ExecuteScalar<int>("GetNumberOfGivenTipsForUser", tipsParameters, CommandType.StoredProcedure);
            if (countTips % 3 == 0)
            {
                MessagesService services = new MessagesService(new MessagesRepository(dbConn));
                services.GiveMessageToUser(chatReportToBeSolved.ReportedUserCnp);
            }

            SqlParameter[] activityParameters = new SqlParameter[]
            {
                new SqlParameter("@UserCNP", chatReportToBeSolved.ReportedUserCnp),
                new SqlParameter("@ActivityName", "Chat"),
                new SqlParameter("@Amount", amount),
                new SqlParameter("@Details", "Chat abuse")
            };


            dbConn.ExecuteNonQuery("UpdateActivityLog", activityParameters, CommandType.StoredProcedure);

            return true;
        }
        public async Task<bool> IsMessageOffensive(string messageToBeChecked)
        {
            bool isOffensive = await ProfanityChecker.IsMessageOffensive(messageToBeChecked);
            return isOffensive;
        }

        public void UpdateHistoryForUser(string UserCNP, int NewScore)
        {
            this._chatReportRepository.UpdateHistoryForUser(UserCNP, NewScore);
        }

        public List<ChatReport> GetChatReports()
        {
            return _chatReportRepository.GetChatReports();
        }
    }
}
