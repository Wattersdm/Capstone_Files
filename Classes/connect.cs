using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace TossAway2.Classes
{
    // **********************************************************
    // Class: connect
    // Purpose: Handles all calls to DB
    // **********************************************************
    public class connect
    {
        #region Properties
        // Connection string parameters.  CHANGE THESE FOR YOUR LOCAL OR ITD2 SERVER AS NEEDED
        private static string _dataSource = @"DESKTOP-A4GV8M1\DBSQL1";
        private static string _targetDatabase = "PlanIt";
        private static string _username = "sa";
        private static string _password = "Ww1085895!";

        private readonly string _connectionString = string.Format("Data Source={0};Initial Catalog={1};User Id={2};Password={3}",
            _dataSource,
            _targetDatabase,
            _username,
            _password);
        #endregion

        #region Constructors

        public connect() { } // default

        #endregion

        #region Methods

        // ------------------------------------------------------------------------
        // Method: Login()
        // Purpose: Handles the DB portion of the login process
        // ------------------------------------------------------------------------
        public User Login(string loginUsername, string loginPassword)
        {
            User newUser = null;

            // establish connection
            string sql = "uspFindUser";
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sql, conn);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.AddWithValue("Username", loginUsername);
            command.Parameters.AddWithValue("Password", loginPassword);

            try
            {
                conn.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Dictionary<string, string> userData = new Dictionary<string, string>()
                    {
                        {"userId", reader["UserID"].ToString() },
                        {"firstName", reader["FirstName"].ToString() },
                        {"lastName", reader["LastName"].ToString() },
                        {"username", reader["Username"].ToString() },
                        {"password", reader["Password"].ToString() },
                        {"email", reader["Email"].ToString() },
                        {"phone", reader["PhoneNumber"].ToString() },
                        {"userTypeId", reader["UserTypeID"].ToString() }
                    };
                        newUser = new User(userData);
                    }
                }

            }
            catch (Exception ex)
            {
                // TODO: error handling
            }
            finally
            {
                conn.Close();
            }

            return newUser;
        }

        // ------------------------------------------------------------------------
        // Method: UserExists()
        // Purpose: Verifies the user exists in the DB
        // ------------------------------------------------------------------------
        public bool UserExists(string loginUsername, string loginPassword)
        {
            bool userExists = false;

            SqlConnection conn = new SqlConnection(_connectionString);
            string sql = "uspFindUser";
            SqlCommand command = new SqlCommand(sql, conn);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.AddWithValue("Username", loginUsername);
            command.Parameters.AddWithValue("Password", loginPassword);
            try
            {
                conn.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows) userExists = true;
            }
            catch (Exception ex)
            {
                // error logging
            }
            finally
            {
                conn.Close();
            }

            return userExists;
        }
        // ---------------------------------------------------------------------------
        // Method: GetUser()
        // Purpose: Calls DB and pulls user data based on userID, returns user object
        //          for the found user.
        // ---------------------------------------------------------------------------
        public User GetUser(int userID)
        {
            // returns user object based on userID
            User user = null;
            SqlConnection conn = new SqlConnection(_connectionString);
            string sql = "uspFindUserWithID";
            SqlCommand command = new SqlCommand(sql, conn);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("UserID", userID);

            try
            {
                conn.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Dictionary<string, string> userData = new Dictionary<string, string>()
                        {
                            {"userId", reader["UserID"].ToString() },
                            {"firstName", reader["FirstName"].ToString() },
                            {"lastName", reader["LastName"].ToString() },
                            {"username", reader["Username"].ToString() },
                            {"password", reader["Password"].ToString() },
                            {"email", reader["Email"].ToString() },
                            {"phone", reader["PhoneNumber"].ToString() },
                            {"userTypeId", reader["UserTypeID"].ToString() }
                        };

                        user = new User(userData);
                    }
                }
            }
            catch( Exception ex )
            {
                // todo
            }
            finally
            {
                conn.Close();
            }

            return user;
        }
        // -----------------------------------------------------------------------------------
        // Method: GetRelatedAcounts()
        // Purpose: Calls DB for all related user accounts based on their relationship type
        //          and returns a list of user objects for each record found
        // -----------------------------------------------------------------------------------
        public List<User> GetRelatedAccounts(int userId, relationshipTypeId RelationshipTypeId)
        {
            List<User> childUsers = new List<User>();
            List<int> userIDs = new List<int>();
            string query = GetQueryStringForRelationshipType(userId);

                string sql = string.Format(query, userId, Convert.ToInt32(RelationshipTypeId));

                SqlConnection conn = new SqlConnection(_connectionString);
                SqlCommand command = new SqlCommand(sql, conn);
                try
                {
                    // Grab the associated IDs
                    conn.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        userIDs.Add(Convert.ToInt32(reader["UserID"]));
                    }
                    conn.Close();

                    // Turn IDs into list of Users
                    foreach (int targetUserID in userIDs)
                    {
                        childUsers.Add(GetUser(targetUserID));
                    }
                }
                catch (Exception ex)
                {
                    // todo
                }
                finally
                {
                    conn.Close();
                }
            return childUsers;
        }
        // ----------------------------------------------------------------------------
        // Method: GetQueryStringForRelationshipType()
        // Purpose: If the user is a child account, the query string to find related
        //          accounts has to be constructed differently.  This method handles
        //          that part.
        // ----------------------------------------------------------------------------
        private string GetQueryStringForRelationshipType (int UserID)
        {
            string query = string.Empty;

            if (IsChildAccount(UserID))
            {
                query = @"SELECT U.UserID FROM Users U INNER JOIN Relationships R ON R.OtherUserID = U.userID WHERE R.childUserID = {0} AND RelationshipTypeID = {1}";
            }
            else
            {
                query = @"SELECT U.UserID FROM Users U INNER JOIN Relationships R  ON R.ChildUserID = U.UserID ";
                query += @"INNER JOIN UserTypes UT ON U.UserTypeID = UT.UserTypeID WHERE R.OtherUserID = {0} AND R.RelationshipTypeID = {1}";
            }

            return query;
        }
        // ------------------------------------------------------------------------
        // Method: IsChildAccount()
        // Purpose: Determines if the user ID passed is for a child or non-child
        //          account
        // ------------------------------------------------------------------------
        public bool IsChildAccount(int UserID)
        {
            // determines if passed user ID is for a child account
            bool isChildAccount = false;

            string sql = "uspIsChildAccount";
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sql, conn);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("UserID", UserID);
            try
            {
                conn.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while(reader.Read())
                    {
                        if (Convert.ToInt32(reader[0]) > 0)
                        {
                            isChildAccount = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO
            }
            finally
            {
                conn.Close();
            }
            return isChildAccount;
        }

        // ------------------------------------------------------------------------
        // Method: AddNewUser()
        // Purpose: Inserts new user record into DB
        // ------------------------------------------------------------------------
        public User AddNewUser(Dictionary<string, string> userData)
        {
            User newUser = null;
            int newUserId = 0;

            SqlConnection conn = new SqlConnection(_connectionString);
            string sql = "uspAddNewUser";
            SqlCommand command = new SqlCommand(sql, conn);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.AddWithValue("UserTypeID", Convert.ToInt32(userData["userTypeId"]));
            command.Parameters.AddWithValue("Username", userData["username"]);
            command.Parameters.AddWithValue("FirstName", userData["firstName"]);
            command.Parameters.AddWithValue("LastName", userData["lastName"]);
            command.Parameters.AddWithValue("Email", userData["email"]);
            command.Parameters.AddWithValue("PhoneNumber", userData["phone"]);
            command.Parameters.AddWithValue("Password", userData["password"]);

            try
            {
                conn.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    newUserId = (int)reader[0];
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                conn.Close();
            }

            if (newUserId > 0)
            {
                userData.Add("userId", newUserId.ToString());
                newUser = new User(userData);
            }
            return newUser;
        }

        // ------------------------------------------------------------------------
        // Method: RegisterNewUser()
        // Purpose: Creates new user record in DB and loads SESSION variable
        // ------------------------------------------------------------------------
        public bool RegisterNewUser(Dictionary<string, string> userData)
        {
            bool isRegistered = false;

            User newUser = AddNewUser(userData);
            if (newUser != null)
            {
                HttpContext.Current.Session["user"] = newUser;
                isRegistered = true;
            }

            return isRegistered;
        }
        // ------------------------------------------------------------------------
        // Method: AddNewRelationship()
        // Purpose: Adds new Relationship record to DB
        // ------------------------------------------------------------------------
        public bool AddNewRelationship (int childUserId, int otherUserId, relationshipTypeId RelationshipTypeId)
        {
            bool IsAdded = false;

            SqlConnection conn = new SqlConnection(_connectionString);
            string sql = "uspAddNewRelationship";
            SqlCommand command = new SqlCommand(sql, conn);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            // add parameters
            command.Parameters.AddWithValue("ChildUserID", childUserId);
            command.Parameters.AddWithValue("OtherUserID", otherUserId);
            command.Parameters.AddWithValue("RelationshipTypeID", Convert.ToInt32(RelationshipTypeId));

            try
            {
                conn.Open();
                SqlDataReader reader = command.ExecuteReader();
                while(reader.Read())
                {
                    if (Convert.ToInt32(reader[0]) > 0) IsAdded = true;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                conn.Close();
            }

            return IsAdded;
        }

        // ------------------------------------------------------------------------
        // Method: ChangeRelationship()
        // Purpose: Change existing relationship record in DB
        // ------------------------------------------------------------------------
        public bool ChangeRelationship(int childUserId, int otherUserId, relationshipTypeId RelationType)
        {
            bool isChanged = false;

            SqlConnection conn = new SqlConnection(_connectionString);
            string sql = "uspChangeRelationship";
            SqlCommand command = new SqlCommand(sql, conn);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            // add parameters
            command.Parameters.AddWithValue("ChildUserID", childUserId);
            command.Parameters.AddWithValue("OtherUserID", otherUserId);
            command.Parameters.AddWithValue("RelationshipTypeID", Convert.ToInt32(RelationType));

            try
            {
                conn.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows) isChanged = true;
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                conn.Close();
            }

            return isChanged;
        }

        // ------------------------------------------------------------------------
        // Method: RemoveRelationship()
        // Purpose: Remove existing relationship
        // ------------------------------------------------------------------------
        public bool RemoveRelationship(int childUserId, int otherUserId, relationshipTypeId RelationType)
        {
            bool isChanged = false;

            SqlConnection conn = new SqlConnection(_connectionString);
            string sql = "uspRemoveRelationship";
            SqlCommand command = new SqlCommand(sql, conn);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            // add parameters
            command.Parameters.AddWithValue("ChildUserID", childUserId);
            command.Parameters.AddWithValue("OtherUserID", otherUserId);
            command.Parameters.AddWithValue("RelationshipTypeID", Convert.ToInt32(RelationType));

            try
            {
                conn.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows) isChanged = true;
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                conn.Close();
            }

            return isChanged;
        }

        // ------------------------------------------------------------------------
        // Method: DeactivateUser
        // Purpose: Sets user record inactive in DB
        // ------------------------------------------------------------------------
        public bool DeactivateUser(int userId)
        {
            bool isDeactivated = false;

            string sql = "uspDeactivateUser";
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sql, conn);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.AddWithValue("UserID", userId);

            try
            {
                conn.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows) isDeactivated = true;
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                conn.Close();
            }

            return isDeactivated;
        }

        // ------------------------------------------------------------------------
        // Method: Activate User
        // Purpose: Sets Active to True in DB record for user
        // ------------------------------------------------------------------------
        public bool ActivateUser (int userId)
        {
            bool isActivated = false;

            SqlConnection conn = new SqlConnection(_connectionString);
            string sql = "uspActivateUser";
            SqlCommand command = new SqlCommand(sql, conn);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("UserID", userId);
            try
            {
                conn.Open();
                SqlDataReader reader = command.ExecuteReader();
                while(reader.Read())
                {
                    if (Convert.ToInt32(reader["UserID"]) > 0) isActivated = true;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                conn.Close();
            }

            return isActivated;
        }

        // ------------------------------------------------------------------------
        // Method: Update User
        // Purpose: Updates record in DB for user
        // ------------------------------------------------------------------------
        public bool UpdateUser(Dictionary<string, string> userData)
        {
            bool isUpdated = false;

            SqlConnection conn = new SqlConnection(_connectionString);
            string sql = "uspUpdateUser";
            SqlCommand command = new SqlCommand(sql, conn);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            // Add parameters
            command.Parameters.AddWithValue("UserID", Convert.ToInt32(userData["userId"]));
            command.Parameters.AddWithValue("UserTypeID", Convert.ToInt32(userData["userTypeId"]));
            command.Parameters.AddWithValue("Firstname", userData["firstName"]);
            command.Parameters.AddWithValue("Lastname", userData["lastName"]);
            command.Parameters.AddWithValue("Email", userData["email"]);
            command.Parameters.AddWithValue("Phonenumber", userData["phone"]);
            command.Parameters.AddWithValue("Password", userData["password"]);
            command.Parameters.AddWithValue("Username", userData["username"]);

            try
            {
                conn.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows) isUpdated = true;
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                conn.Close();
            }

            return isUpdated;
        }

        // ------------------------------------------------------------------------
        // Method: GetCalendarEvent
        // Purpose: Returns a single calendar event for child
        // ------------------------------------------------------------------------
        public Event GetCalendarEvent(int calendarEventId, int childUserId)
        {
            Event newEvent = null;
            string sql = "uspGetCalendarEvent";
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sql, conn);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            // Parameters
            command.Parameters.AddWithValue("CalendarEventID", calendarEventId);
            command.Parameters.AddWithValue("ChildUserID", childUserId);

            try
            {
                conn.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while(reader.Read())
                    {
                        Dictionary<string, string> eventData = new Dictionary<string, string>()
                        {
                            {"calendarEventId", reader["CalendarEventID"].ToString() },
                            {"description", reader["Dscr"].ToString() },
                            {"eventStart", reader["EventStart"].ToString() },
                            {"eventEnd", reader["EventEnd"].ToString() },
                            {"location", reader["Location"].ToString() },
                            {"responsibleUserId", reader["ResponsibleUserID"].ToString() },
                            {"childUserId", childUserId.ToString() }
                        };

                        newEvent = new Event(eventData);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                conn.Close();
            }

            return newEvent;
        }

        // ------------------------------------------------------------------------
        // Method: GetAllCalendarEventsForChild
        // Purpose: Returns all calendar events for a single child
        // ------------------------------------------------------------------------
        public List<Event> GetAllCalendarEventsForChild (int childUserId)
        {
            List<Event> allEvents = new List<Event>();

            string sql = "uspGetCalendarEventsForChild";
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sql, conn);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.AddWithValue("ChildUserID", childUserId);

            try
            {
                conn.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while(reader.Read())
                    {
                        Dictionary<string, string> eventData = new Dictionary<string, string>()
                        {
                            {"calendarEventId", reader["CalendarEventID"].ToString() },
                            {"description", reader["Dscr"].ToString() },
                            {"eventStart", reader["EventStart"].ToString() },
                            {"eventEnd", reader["EventEnd"].ToString() },
                            {"location", reader["Location"].ToString() },
                            {"responsibleUserId", reader["ResponsibleUserID"].ToString() },
                            {"childUserId", childUserId.ToString() }
                        };

                        allEvents.Add(new Event(eventData));
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                conn.Close();
            }
            return allEvents;
        }

        // ------------------------------------------------------------------------
        // Method: UpdateEventData
        // Purpose: Updates DB record for event
        // ------------------------------------------------------------------------
        public bool UpdateEventData(Dictionary<string, string> eventData)
        {
            bool isUpdated = false;

            SqlConnection conn = new SqlConnection(_connectionString);
            string sql = "uspUpdateEvent";
            SqlCommand command = new SqlCommand(sql, conn);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            // parameters
            command.Parameters.AddWithValue("CalendarEventID", Convert.ToInt32(eventData["calendarEventId"]));
            command.Parameters.AddWithValue("Dscr", eventData["description"]);
            command.Parameters.AddWithValue("EventStart", Convert.ToDateTime(eventData["eventStart"]));
            command.Parameters.AddWithValue("EventEnd", Convert.ToDateTime(eventData["eventEnd"]));
            command.Parameters.AddWithValue("Location", eventData["location"]);
            command.Parameters.AddWithValue("ResponsibleUserID", Convert.ToInt32(eventData["responsibleUserId"]));

            try
            {
                conn.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows) isUpdated = true;
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                conn.Close();
            }

            return isUpdated;
        }

        // ------------------------------------------------------------------------
        // Method: RemoveChildFromEvent()
        // Purpose: Removes child user from event in DB
        // NOTE: If child was the only attendee, the event will also be made 
        //       inactive in the DB
        // ------------------------------------------------------------------------
        public bool RemoveChildFromEvent(int calendarEventId, int childUserId)
        {
            bool isRemoved = false;

            SqlConnection conn = new SqlConnection(_connectionString);
            string sql = "uspRemoveChildFromEvent";
            SqlCommand command = new SqlCommand(sql, conn);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            // parameters
            command.Parameters.AddWithValue("ChildUserID", childUserId);
            command.Parameters.AddWithValue("CalendarEventID", calendarEventId);

            try
            {
                conn.Open();
                SqlDataReader reader = command.ExecuteReader();
                while(reader.Read())
                {
                    if (Convert.ToInt32(reader["UserID"]) > 0) isRemoved = true;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                conn.Close();
                command.Dispose();
            }

            return isRemoved;
        }

        // ------------------------------------------------------------------------
        // Method: CancelEntireEvent()
        // Purpose: Cancels entire event and removes all associated records in DB
        //          from the UserCalendarEvents table.  This will affect all 
        //          associated child users.
        // ------------------------------------------------------------------------
        public bool CancelEntireEvent(int calendarEventId)
        {
            bool isCanceled = false;

            SqlConnection conn = new SqlConnection(_connectionString);
            string sql = "uspRemoveCalendarEvent";
            SqlCommand command = new SqlCommand(sql, conn);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.AddWithValue("CalendarEventID", calendarEventId);

            try
            {
                conn.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows) isCanceled = true;
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                conn.Close();
                command.Dispose();
            }

            return isCanceled;
        }
        #endregion
    }
}