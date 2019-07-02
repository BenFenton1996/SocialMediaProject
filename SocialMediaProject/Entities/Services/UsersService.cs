﻿using SocialMediaProject.Entities.Models;
using System.Collections.Generic;
using System.Linq;

namespace SocialMediaProject.Entities.Services
{
    public interface IUsersService
    {
        /// <summary>
        /// Gets all rows in the Users table and returns them as a List
        /// </summary>
        /// <returns>A List containing all rows in the Users table</returns>
        List<User> GetAllUsers();

        /// <summary>
        /// Checks the Users table and returns the row with a matching Email and Password
        /// </summary>
        /// <param name="Email">The Email to check against the Email column in the Users table</param>
        /// <param name="Password">The Password to check against the Password column in the Users table</param>
        /// <returns>The row in Users with matching details</returns>
        User CheckUserDetails(string Email, string Password);

        bool CreateNewAccount(string Username, string Email, string Password);
    }

    public class UsersService : IUsersService
    {
        private readonly SMPDbContext context;
        public UsersService(SMPDbContext context)
        {
            this.context = context;
        }

        public List<User> GetAllUsers()
        {
            return context.Users.ToList();
        }

        public User CheckUserDetails(string Email, string Password)
        {
            return context.Users
                .FirstOrDefault(u => u.Email == Email && u.Password.SequenceEqual(Utilities.SMPHash.HashText(Password, u.Salt)));
        }

        public bool CreateNewAccount(string Username, string Email, string Password)
        {
            if (!context.Users.Where(u => u.Username == Username || u.Email == Email).Any())
            {
                byte[] salt = Utilities.SMPHash.GenerateSalt();
                context.Users.Add(new User
                {
                    Username = Username,
                    Email = Email,
                    Password = Utilities.SMPHash.HashText(Password, salt),
                    Salt = salt,
                    Role = "User"
                });
                context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
