﻿using WebApp.AccountManager;

namespace WebApp.ViewModels
{
    public class UserViewModel
    {
        public User User { get; set; }

        public UserViewModel(User user)
        {
            User = user;
        }

        public List<User> Friends { get; set; }
    }
}
