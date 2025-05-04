namespace Src.View.Pages
{
    using System.Collections.Generic;
    using Microsoft.UI.Xaml;
    using Src.Model;
    using Src.Repos;
    using Src.View.Components;

    public sealed partial class TipHistoryWindow : Window
    {
        private User selectedUser;
        private readonly MessagesRepository messagesRepository;
        private readonly TipsRepository tipsRepository;

        public TipHistoryWindow(User selectedUser, MessagesRepository messagesRepository, TipsRepository tipsRepository)
        {
            this.InitializeComponent();
            this.selectedUser = selectedUser;
            this.messagesRepository = messagesRepository;
            this.tipsRepository = tipsRepository;

            List<Message> messages = this.messagesRepository.GetMessagesForGivenUser(selectedUser.Cnp);
            List<Tip> tips = this.tipsRepository.GetTipsForGivenUser(selectedUser.Cnp);

            this.LoadHistory(tips);
            this.LoadHistory(messages);
        }

        private void LoadHistory(List<Message> messages)
        {
            foreach (Message message in messages)
            {
                MessageHistoryComponent messageComponent = new MessageHistoryComponent();
                messageComponent.SetMessageData(message);
                this.MessageHistoryContainer.Items.Add(messageComponent);
            }
        }

        private void LoadHistory(List<Tip> tips)
        {
            foreach (Tip tip in tips)
            {
                TipHistoryComponent tipComponent = new TipHistoryComponent();
                tipComponent.SetTipData(tip);
                this.TipHistoryContainer.Items.Add(tipComponent);
            }
        }
    }
}
