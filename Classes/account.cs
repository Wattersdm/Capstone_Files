using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace TossAway2.Classes
{
    // **********************************************************
    // Class: account
    // Purpose: Manages operations related to user login / logout
    //          status
    // **********************************************************
    public class account
    {
        #region Properties

        #endregion

        #region Constructors

        public account() { } // default

        #endregion

        #region Methods
        // ------------------------------------------------------------------------
        // Method: Loginuser()
        // Purpose: Calls connect class to log in the user and sets the SESSION
        //          variable for the user 
        // ------------------------------------------------------------------------
        public bool LoginUser(string loginUsername, string loginPassword)
        {
            bool isLoggedIn = false;

            connect DBConnection = new connect();
            if (DBConnection.UserExists(loginUsername, loginPassword))
            {
                User currentUser = DBConnection.Login(loginUsername, loginPassword);
                if (currentUser != null)
                {
                    HttpContext.Current.Session["user"] = currentUser;
                    if (HttpContext.Current.Session["user"] != null) isLoggedIn = true;
                }
            }
            return isLoggedIn;
        }

        // ------------------------------------------------------------------------
        // Method: UserIsLoggedIn()
        // Purpose: Verifies if the user SESSION variable is set, indicating they are
        //          currently logged in.  Should be called at every page load and
        //          if false, handle appropriately
        // ------------------------------------------------------------------------
        public bool UserIsLoggedIn()
        {
            bool isLoggedIn = false;

            if (HttpContext.Current.Session["user"] != null) isLoggedIn = true;

            return isLoggedIn;
        }
        // ------------------------------------------------------------------------
        // Method: Logout()
        // Purpose: Removes user SESSION variable
        // ------------------------------------------------------------------------
        public bool Logout()
        {
            bool isLoggedOut = false;

            if (HttpContext.Current.Session["user"] != null) HttpContext.Current.Session.Remove("user");
            if (HttpContext.Current.Session["user"] == null) isLoggedOut = true;

            return isLoggedOut;
        }

        #endregion
    }
}