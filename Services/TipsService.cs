using System;
using src.Data;
using src.Repos;


namespace src.Services
{
    class TipsService
    {
        private TipsRepository _tipsRepository;

        public TipsService(TipsRepository tipsRepository)
        {
            _tipsRepository = tipsRepository;
        }

        public void GiveTipToUser(string UserCNP)
        {
            DatabaseConnection dbConnection = new DatabaseConnection();
            UserRepository userRepository = new UserRepository(dbConnection);
            
            try{
               
                Int32 userCreditScore = userRepository.GetUserByCnp(UserCNP).CreditScore;
                if (userCreditScore < 300 ) 
                {
                    _tipsRepository.GiveUserTipForLowBracket(UserCNP);
                }
                else if (userCreditScore < 550)
                {
                    _tipsRepository.GiveUserTipForMediumBracket(UserCNP);
                }
                else if (userCreditScore > 549)
                {
                    _tipsRepository.GiveUserTipForHighBracket(UserCNP);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{exception.Message},User is not found");
            }
        }
    }
}
