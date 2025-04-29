using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using src.Data;
using src.ViewModel;
using src.Repos;
using src.Services;
using src.View.Components;
using src.Model;



namespace src.Views
{
    public sealed partial class UsersView : Page
    {

        public UsersView()
        {
            this.InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            UsersContainer.Items.Clear();

            DatabaseConnection dbConnection = new DatabaseConnection();
            UserRepository userRepository = new UserRepository(dbConnection);
            UserService userService = new UserService(userRepository);
            UserViewModel userViewModel = new UserViewModel(userService);

            try
            {
                List<User> users = userService.GetUsers();
                foreach (var user in users)
                {
                    UserInfoComponent userComponent = new UserInfoComponent();
                    userComponent.SetUserData(user);
                    UsersContainer.Items.Add(userComponent);
                }
            }
            catch (Exception)
            {
                UsersContainer.Items.Add("There are no users to display.");
            }
        }
    }
}
