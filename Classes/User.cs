using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace TossAway2.Classes
{
    // **********************************************************
    // Class: user
    // Purpose: Encaspulates all data and methods related to 
    //          a single user
    // **********************************************************
    public class User
    {
        #region Properties

        private int userId { get; set; }
        public int UserId
        {
            get => userId;
            set
            {
                userId = value;
            }
        }
        public int UserID
        {
            get => userId;
            set
            {
                userId = value;
            }
        }
        private userTypeId userType { get; set; }
        public userTypeId UserType
        {
            get => userType;
            set
            {
                userType = value;
            }
        }
        private string username { get; set; }
        public string Username
        {
            get => username;
            set
            {
                username = value;
            }
        }
        private string password { get; set; }
        public string Password
        {
            get => password;
            set
            {
                password = value;
            }
        }
        
        private string firstName { get; set; }
        public string FirstName
        {
            get => firstName;
            set { firstName = value;  }
        }

        private string lastName { get; set; }
        public string LastName
        {
            get => lastName;
            set { lastName = value;  }
        }
        public string fullName
        {
            get { return firstName + " " + lastName; }
        }
        private string email { get; set; }
        public string Email
        {
            get => email;
            set
            {
                email = value;
            }
        }
        private string phone { get; set; }
        public string Phone
        {
            get => phone;
            set
            {
                phone = value;
            }
        }

        #endregion

        #region Constructors

        public User() { } // default

        public User(Dictionary<string, string> userData)
        {
            userId = Convert.ToInt32(userData["userId"]);
            username = userData["username"];
            password = userData["password"];
            firstName = userData["firstName"];
            lastName = userData["lastName"];
            email = userData["email"];
            phone = userData["phone"];
            userType = (userTypeId)Enum.Parse(typeof(userTypeId), userData["userTypeId"]);
        }

        public bool UpdateProfile(Dictionary<string, string> updatedUserData)
        {
            bool isUpdated = false;

            connect Connect = new connect();
            if (Connect.UpdateUser(updatedUserData))
            {
                firstName = updatedUserData["firstName"];
                lastName = updatedUserData["lastName"];
                username = updatedUserData["username"];
                password = updatedUserData["password"];
                email = updatedUserData["email"];
                phone = updatedUserData["phone"];
                userType = (userTypeId)Enum.Parse(typeof(userTypeId), updatedUserData["userTypeId"]);

                isUpdated = true;
            }

            return isUpdated;
        }
        #endregion

    }
}