using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using src.Model;
using src.Repos;
using src.View.Components;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.UI.Xaml;
using src.Data;
using Microsoft.Data.SqlClient;
using System.Data;
using System;
using src.Services;

namespace src.View.Pages
{
    public sealed partial class TipHistoryWindow : Window
    {
        private User selectedUser;
        MessagesRepository _messagesRepository;
        TipsRepository _tipsRepository;

        public TipHistoryWindow(User selectedUser, MessagesRepository messagesRepository, TipsRepository tipsRepository)
        {
            this.InitializeComponent();
            this.selectedUser = selectedUser;
            DatabaseConnection dbConn = new DatabaseConnection();
            _messagesRepository = messagesRepository;
            _tipsRepository = tipsRepository;

            List<Message> messages = _messagesRepository.GetMessagesForGivenUser(selectedUser.Cnp);
            List<Tip> tips = _tipsRepository.GetTipsForGivenUser(selectedUser.Cnp);

            LoadHistory(tips);
            LoadHistory(messages);
        }
        private void LoadHistory(List<Message> messages)
        {
            foreach (Message message in messages)
            {
                MessageHistoryComponent messageComponent = new MessageHistoryComponent();
                messageComponent.setMessageData(message);
                MessageHistoryContainer.Items.Add(messageComponent);
            }
        }
        private void LoadHistory(List<Tip> tips)
        {
            foreach (Tip tip in tips)
            {
                TipHistoryComponent tipComponent = new TipHistoryComponent();
                tipComponent.setTipData(tip);
                TipHistoryContainer.Items.Add(tipComponent);
            }
        }
    }
}
