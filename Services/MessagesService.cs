using System;
using src.Data;
using src.Repos;


namespace src.Services
{
    class MessagesService
    {
        MessagesRepository _messagesRepository;

        public MessagesService(MessagesRepository messagesRepository)
        {
            _messagesRepository = messagesRepository;
        }

        public void GiveMessageToUser(string UserCNP)
        {
            DatabaseConnection dbConn = new DatabaseConnection();
            UserRepository userRepository = new UserRepository(dbConn);

            Int32 userCreditScore = userRepository.GetUserByCnp(UserCNP).CreditScore;
            try
            {
                if (userCreditScore >= 550)
                {
                    _messagesRepository.GiveUserRandomMessage(UserCNP);
                }
                else
                {
                    _messagesRepository.GiveUserRandomRoastMessage(UserCNP);
                }
            }

            catch (Exception e)
            {
                Console.WriteLine($"{e.Message},User is not found");
            }
        }
    }
}
