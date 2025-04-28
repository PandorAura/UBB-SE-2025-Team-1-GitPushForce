﻿using src.Model;
using src.Repos;
using System;
using System.Collections.Generic;

namespace src.Services
{
    public class UserService : IUserService
    {
        private readonly UserRepository _userRepository;
        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public User GetUserByCNP(string cnp)
        {
            if (string.IsNullOrWhiteSpace(cnp))
            {
                throw new ArgumentException("CNP cannot be empty");
            }
            return _userRepository.GetUserByCNP(cnp);
        }


        public List<User> GetUsers()
        {
            return _userRepository.GetUsers();
        }
    }
}