using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
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
        private readonly IUserService _userService;
        private readonly Func<UserInfoComponent> _userComponentFactory;

        public UsersView(IUserService userService, Func<UserInfoComponent> userComponentFactory)
        {
            this.InitializeComponent();
            _userService = userService;
            _userComponentFactory = userComponentFactory;
            LoadUsers();
        }

        private void LoadUsers()
        {
            UsersContainer.Items.Clear();

            try
            {
                List<User> users = _userService.GetUsers();
                foreach (var user in users)
                {
                    var userComponent = _userComponentFactory();
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
