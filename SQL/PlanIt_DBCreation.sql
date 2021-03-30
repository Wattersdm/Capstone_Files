USE PlanIt

SET NOCOUNT ON

-- Drop tables

IF OBJECT_ID('MessagePostUsers')				IS NOT NULL DROP TABLE MessagePostUsers
IF OBJECT_ID('UserCalendarEvents')				IS NOT NULL DROP TABLE UserCalendarEvents
IF OBJECT_ID('MessagePosts')					IS NOT NULL DROP TABLE MessagePosts
IF OBJECT_ID('CalendarEvents')					IS NOT NULL DROP TABLE CalendarEvents
IF OBJECT_ID('Relationships')					IS NOT NULL DROP TABLE Relationships
IF OBJECT_ID('RelationshipTypes')				IS NOT NULL DROP TABLE RelationshipTypes
IF OBJECT_ID('Users')							IS NOT NULL DROP TABLE Users
IF OBJECT_ID('UserTypes')						IS NOT NULL DROP TABLE UserTypes

-- Drop Procedures
DROP PROCEDURE IF EXISTS uspActivateUser
DROP PROCEDURE IF EXISTS uspAddNewRelationship
DROP PROCEDURE IF EXISTS uspAddNewUser
DROP PROCEDURE IF EXISTS uspChangeRelationship
DROP PROCEDURE IF EXISTS uspDeactivateUser
DROP PROCEDURE IF EXISTS uspFindUser
DROP PROCEDURE IF EXISTS uspFindUserWithID
DROP PROCEDURE IF EXISTS uspGetCalendarEvent
DROP PROCEDURE IF EXISTS uspGetCalendarEventsForChild
DROP PROCEDURE IF EXISTS uspIsChildAccount
DROP PROCEDURE IF EXISTS uspRemoveChildFromEvent
DROP PROCEDURE IF EXISTS uspRemoveRelationship
DROP PROCEDURE IF EXISTS uspUpdateEvent
DROP PROCEDURE IF EXISTS uspUpdateUser
DROP PROCEDURE IF EXISTS uspRemoveCalendarEvent

-- Create Tables

CREATE TABLE Users
(
	 UserID					INTEGER			NOT NULL
	,UserTypeID				INTEGER			NOT NULL
	,Username				VARCHAR(255)	NOT NULL
	,FirstName				VARCHAR(50)		NOT NULL
	,LastName				VARCHAR(50)		NOT NULL
	,Email					VARCHAR(50)		NOT NULL
	,PhoneNumber			VARCHAR(50)		NOT NULL
	,Password				VARCHAR(255)	NOT NULL
	,Active					BIT				NOT NULL
	,CONSTRAINT Users_PK  PRIMARY KEY ( UserID )
)

CREATE TABLE UserTypes
(
	 UserTypeID				INTEGER			NOT NULL
	,Dscr					VARCHAR(50)		NOT NULL
	,CONSTRAINT UserTypes_PK  PRIMARY KEY ( UserTypeID )
)

CREATE TABLE Relationships
(
	  ChildUserID			INTEGER			NOT NULL
	 ,OtherUserID			INTEGER			NOT NULL
	 ,RelationshipTypeID	INTEGER			NOT NULL
	 ,CONSTRAINT Relationships_PK  PRIMARY KEY (ChildUserID, OtherUserID)
)

CREATE TABLE RelationshipTypes
(
	 RelationshipTypeID		INTEGER			NOT NULL
	,Dscr					VARCHAR(50)		NOT NULL
	,CONSTRAINT RelationshipTypes_PK  PRIMARY KEY ( RelationshipTypeID )
)

CREATE TABLE UserCalendarEvents
(
	 CalendarEventID		INTEGER			NOT NULL
	,UserID					INTEGER			NOT NULL
	,CONSTRAINT UserEvents_PK  PRIMARY KEY (CalendarEventID, UserID)
)

CREATE TABLE CalendarEvents
(
	 CalendarEventID				INTEGER			NOT NULL
	,Dscr							VARCHAR(255)	NOT NULL
	,EventStart						DATETIME		NOT NULL
	,EventEnd						DATETIME		NOT NULL
	,Location						VARCHAR(255)	NOT NULL
	,ResponsibleUserID				INTEGER			NOT NULL
	,Active							BIT				NOT NULL
	,CONSTRAINT Events_PK  PRIMARY KEY (CalendarEventID)
)

CREATE TABLE MessagePosts
(
	 MessagePostID					INTEGER			NOT NULL
	,Body							TEXT			NOT NULL
	,Sent							DATETIME		NOT NULL
	,Active							BIT				NOT NULL
	,CONSTRAINT Messages_PK  PRIMARY KEY (MessagePostID)
)

CREATE TABLE MessagePostUsers
(
	 MessagePostID					INTEGER			NOT NULL
	,UserID							INTEGER			NOT NULL
	,CONSTRAINT  MessagePostUsers_PK  PRIMARY KEY (MessagePostID, UserID)
)

-- FOREIGN KEY CONSTRAINTS

ALTER TABLE
	Users	
ADD CONSTRAINT
	Users_UserTypes_FK
	FOREIGN KEY ( UserTypeID )
	REFERENCES UserTypes ( UserTypeID)
	
ALTER TABLE
	UserCalendarEvents
ADD CONSTRAINT
	UserCalendarEvents_Users_FK
	FOREIGN KEY ( UserID )
	REFERENCES Users ( UserID )
	
ALTER TABLE
	UserCalendarEvents
ADD CONSTRAINT
	UserCalendarEvents_Events_FK
	FOREIGN KEY ( CalendarEventID )
	REFERENCES CalendarEvents ( CalendarEventID )
	
ALTER TABLE
	MessagePostUsers
ADD CONSTRAINT
	MessagePostUsers_Users_FK
	FOREIGN KEY ( UserID )
	REFERENCES Users ( UserID )
	
ALTER TABLE
	MessagePostUsers
ADD CONSTRAINT
	MessagePostUsers_Messages_FK
	FOREIGN KEY ( MessagePostID )
	REFERENCES MessagePosts ( MessagePostID )

ALTER TABLE
	Relationships
ADD CONSTRAINT
	Relationships_RelationshipTypes_FK
	FOREIGN KEY ( RelationshipTypeID )
	REFERENCES RelationshipTypes ( RelationshipTypeID )

ALTER TABLE
	Relationships
ADD CONSTRAINT
	Relationships_Users_ChildUsers_FK
	FOREIGN KEY ( ChildUserID )
	REFERENCES Users ( UserID )

ALTER TABLE
	Relationships
ADD CONSTRAINT
	Relationships_Users_OtherUsers_FK
	FOREIGN KEY ( OtherUserID )
	REFERENCES Users ( UserID )

-- DUMMY DATA 

INSERT INTO UserTypes (UserTypeID, UserTypes.Dscr)
	VALUES  (1, 'Guardian')
		   ,(2, 'Caretaker')
		   ,(3, 'Adolescent')
		   ,(4, 'Child')
		   ,(5, 'Admin')

INSERT INTO Users (UserID, UserTypeID, FirstName, LastName, Username, Email, PhoneNumber, Password, Active)
	VALUES  (1, 5, 'Joshua', 'Gravatt', 'jgravatt', 'daemondog@gmail.com', '513-680-0909', 'Admin01', 1)
		   ,(2, 5, 'Devin', 'Watters', 'dwatters', 'devinwatters57@gmail.com', '910-527-1157', 'Admin02', 1)
		   ,(3, 5, 'Rodney', 'Clark', 'rclark', 'hotrod11194@gmail.com', '513-869-1900', 'Admin03', 1)
		   ,(4, 1, 'Jack', 'Smith', 'jsmith1', 'jsmith1@gmail.com', '123-456-0001', 'Password01', 1)
		   ,(5, 1, 'Eloise', 'Smith', 'esmith', 'esmith@gmail.com', '123-456-0002', 'Password02', 1)
		   ,(6, 2, 'Jebediah', 'Rogers', 'jrogers', 'jrogers@hotmail.com', '123-456-0003', 'Password03', 1)
		   ,(7, 3, 'William', 'Smith', 'wsmith1', 'wsmith1@yahoo.com', '123-456-0004', 'Password04', 1)
		   ,(8, 4, 'Jessica', 'Smith', 'jsmith2', 'jsmith2@msn.com', '123-456-0005', 'Password05', 1)
		   ,(9, 4, 'Walter', 'Smith', 'wsmith2', 'wsmith2@juno.com', '123-456-0006', 'Password06', 1)
		   ,(10, 4, 'Joshua', 'Smith', 'jsmith3', 'jsmith3@gmail.com', '123-456-0007', 'Password07', 1)
		   ,(11, 5, 'Perry', 'Jackson', 'pjackson', 'pjackson@msn.com', '456-157-4503', 'Password08', 0)


INSERT INTO RelationshipTypes (RelationshipTypeID, Dscr)
	VALUES  (1, 'Child To Guardian')
		   ,(2, 'Child To Caretaker')
		   ,(3, 'Child To Adolescent')
		   ,(4, 'Child To Child')

INSERT INTO Relationships (ChildUserID, OtherUserID, RelationshipTypeID)
	VALUES   (8, 4, 1)
			,(8, 5, 1)
			,(8, 6, 2)
			,(8, 7, 3)
			,(8, 9, 4)
			,(8, 10, 4)
			,(9, 4, 1)
			,(9, 5, 1)
			,(9, 6, 2)
			,(9, 7, 3)
			,(9, 8, 4)
			,(9, 10, 4)
			,(10, 4, 1)
			,(10, 5, 1)
			,(10, 6, 2)
			,(10, 7, 3)
			,(10, 8, 4)
			,(10, 9, 4)

INSERT INTO CalendarEvents (CalendarEventID, Dscr, EventStart, EventEnd, Location, ResponsibleUserID, Active)
	VALUES  (1, 'Birthday Party', '2020-07-18 10:00:00', '2020-07-18 16:00:00', 'Chuck E Cheeze', -1, 1)
		   ,(2, 'Tuba Recital', '2020-07-23 18:30:00', '2020-07-23 20:00:00', 'Retro Music Hall', 4, 1)
		   ,(3, 'Doctor Appointment', '2020-08-05 13:30:00', '2020-08-05 14:00:00', 'Mt. Carmel Pediatric', 7, 1)
		   ,(4, 'Summer Camp', '2020-08-17 15:00:00', '2020-08-21 12:00:00', 'Shasta Lake', -1, 1)
		   ,(5, 'Barmitzvah', '2020-09-20 13:30:00', '2020-09-20 17:00:00', 'Downton Synagogue', 5, 0)

INSERT INTO UserCalendarEvents (CalendarEventID, UserID)
	VALUES   (1, 8)
			,(2, 8)
			,(2, 9)
			,(3, 9)
			,(4, 10)

--------- STORED PROCEDURES ----------------------------------------
GO

/*********** uspActivateUser *****************************/
CREATE PROCEDURE uspActivateUser
	@UserID AS INT
AS

SET NOCOUNT ON
SET XACT_ABORT ON

BEGIN TRANSACTION

	UPDATE Users SET Active = 1
	OUTPUT INSERTED.*
	WHERE UserID = @UserID
	AND Active <> 1

COMMIT TRANSACTION
GO

/*********** uspAddNewRelationship ***********************/
CREATE PROCEDURE uspAddNewRelationship
	@ChildUserID			AS INT,
	@OtherUserID			AS INT,
	@RelationshipTypeID		AS INT
AS

SET NOCOUNT ON
SET XACT_ABORT ON

BEGIN TRANSACTION

	INSERT INTO Relationships (ChildUserID, OtherUserID, RelationshipTypeID)
	OUTPUT INSERTED.*
	VALUES (@ChildUserID, @OtherUserID, @RelationshipTypeId)

COMMIT TRANSACTION
GO

/*********** uspAddNewUser *******************************/
CREATE PROCEDURE uspAddNewUser
	@UserTypeID as INT,
	@Username as VARCHAR(255),
	@FirstName as VARCHAR(255),
	@LastName as VARCHAR(255),
	@Email as VARCHAR(255),
	@PhoneNumber as VARCHAR(255),
	@Password as VARCHAR(255)
AS
SET XACT_ABORT ON
SET NOCOUNT ON

BEGIN TRANSACTION
	DECLARE @UserID as INT

	SELECT @UserID = MAX(UserID) + 1 
	FROM Users

	INSERT INTO Users (UserID, UserTypeID, Username, FirstName, LastName, Email, PhoneNumber, Password)
	OUTPUT INSERTED.UserID
	VALUES (@UserID, @UserTypeID, @Username, @FirstName, @LastName, @Email, @PhoneNumber, @Password)
COMMIT TRANSACTION

GO

/*********** uspChangeRelationship ***********************/
CREATE PROCEDURE uspChangeRelationship
	@ChildUserID			AS INT,
	@OtherUserID			AS INT,
	@RelationshipTypeID		AS INT
AS

SET NOCOUNT ON
SET XACT_ABORT ON

BEGIN TRANSACTION

	UPDATE Relationships SET 
		RelationshipTypeID = @RelationshipTypeID
	OUTPUT INSERTED.*
	WHERE 
		ChildUserID = @ChildUserID
		AND OtherUserID = @OtherUserID

COMMIT TRANSACTION
GO

/*********** uspDeactivateUser ***************************/
CREATE PROCEDURE uspDeactivateUser
	@UserID AS INT
AS
SET NOCOUNT ON
SET XACT_ABORT ON

BEGIN TRANSACTION

	UPDATE Users SET Active = 0 
	OUTPUT INSERTED.*
	WHERE UserID = @UserID
	AND Active <> 0

COMMIT TRANSACTION
GO
/*********** uspFindUser *********************************/
CREATE PROCEDURE uspFindUser
	@Username AS VARCHAR(255),
	@Password AS VARCHAR(255)
AS

SET NOCOUNT ON
SET XACT_ABORT ON

BEGIN TRANSACTION

	SELECT * FROM Users
	WHERE Username = @Username
	AND Password = @Password
	AND Active = 1

COMMIT TRANSACTION
GO

/*********** uspFindUserWithID ***************************/
CREATE PROCEDURE uspFindUserWithID
	@UserID		AS INT
AS

SET NOCOUNT ON
SET XACT_ABORT ON

BEGIN TRANSACTION

	SELECT * FROM Users
	WHERE UserID = @UserID
	AND Active = 1

COMMIT TRANSACTION
GO

/*********** uspGetCalendarEvent *************************/
CREATE PROCEDURE uspGetCalendarEvent
	@CalendarEventID		AS INT,
	@ChildUserID			AS INT
AS
SET NOCOUNT ON
SET XACT_ABORT ON

BEGIN TRANSACTION

	SELECT
		CE.*
	FROM CalendarEvents CE
	
	INNER JOIN UserCalendarEvents UCE
	ON CE.CalendarEventID = UCE.CalendarEventID

	WHERE CE.CalendarEventID = @CalendarEventID
	AND UCE.UserID = @ChildUserID

COMMIT TRANSACTION
GO

/*********** uspGetCalendarEventsForChild ****************/
CREATE PROCEDURE uspGetCalendarEventsForChild
	@ChildUserID AS INT
AS
SET NOCOUNT ON
SET XACT_ABORT ON

BEGIN TRANSACTION

	SELECT
		CE.*
	FROM
		CalendarEvents CE
		INNER JOIN UserCalendarEvents UCE
		ON CE.CalendarEventID = UCE.CalendarEventID

	WHERE UCE.UserID = @ChildUserID

COMMIT TRANSACTION
GO

/*********** uspIsChildAccount ***************************/
CREATE PROCEDURE uspIsChildAccount
	@UserID AS INT
AS
SET NOCOUNT ON
SET XACT_ABORT ON

BEGIN TRANSACTION

	SELECT COUNT(*) FROM Users U INNER JOIN UserTypes UT ON U.UsertypeID = UT.UserTypeID 
	WHERE UserID = @UserID 
	AND UT.UserTypeID = 4
	AND U.Active = 1

COMMIT TRANSACTION
GO

/*********** uspRemoveCalendarEvent **********************/
CREATE PROCEDURE uspRemoveCalendarEvent
	@CalendarEventID		AS INT
AS
SET NOCOUNT ON
SET XACT_ABORT ON

BEGIN TRANSACTION

	UPDATE CalendarEvents
	SET Active = 0
	OUTPUT Inserted.*
	WHERE CalendarEventID = @CalendarEventID

	-- Find and remove any related records for event attendees in DB
	DECLARE @result AS INT

	SELECT @result = COUNT(*)
	FROM UserCalendarEvents
	WHERE CalendarEventID = @CalendarEventID

	IF @result > 0
	BEGIN
		DELETE FROM UserCalendarEvents
		OUTPUT Deleted.*
		WHERE CalendarEventID = @CalendarEventID
	END

COMMIT TRANSACTION
GO


/*********** uspRemoveChildFromEvent *********************/
CREATE PROCEDURE uspRemoveChildFromEvent
	@ChildUserID			AS INT,
	@CalendarEventID		AS INT
AS

SET NOCOUNT ON
SET XACT_ABORT ON

BEGIN TRANSACTION
	DECLARE @Return AS INT

	DELETE FROM UserCalendarEvents
	OUTPUT Deleted.*
	WHERE UserID = @ChildUserID
	AND CalendarEventID = @CalendarEventID

	-- now see if there are any other child users
	-- associated to this event.  If not, delete
	-- the event from the CalendarEvents table also.

	DECLARE @Result AS INT

	SELECT @Result = COUNT(*) FROM UserCalendarEvents
	WHERE CalendarEventID = @CalendarEventID

	IF @Result = 0
	BEGIN
		UPDATE CalendarEvents
		SET Active = 0 
		OUTPUT INSERTED.*
		WHERE CalendarEventID = @CalendarEventID
	END

COMMIT TRANSACTION
GO

/*********** uspRemoveRelationship ***********************/
CREATE PROCEDURE uspRemoveRelationship
	@ChildUserID			AS INT,
	@OtherUserID			AS INT,
	@RelationshipTypeID		AS INT
AS

SET NOCOUNT ON
SET XACT_ABORT ON

BEGIN TRANSACTION

	DELETE FROM Relationships
	OUTPUT Deleted.*
	WHERE ChildUserID = @ChildUserID
	AND OtherUserID = @OtherUserID
	AND RelationshipTypeID = @RelationshipTypeID

COMMIT TRANSACTION
GO

/*********** uspUpdateEvent ******************************/
CREATE PROCEDURE uspUpdateEvent
	@CalendarEventID	AS INT,
	@Dscr				AS VARCHAR(255),
	@EventStart			AS DATETIME,
	@EventEnd			AS DATETIME,
	@Location			AS VARCHAR(255),
	@ResponsibleUserID	AS INT
AS

SET NOCOUNT ON
SET XACT_ABORT ON

BEGIN TRANSACTION

	UPDATE CalendarEvents
	SET Dscr = @Dscr,
		EventStart = @EventStart,
		EventEnd = @EventEnd,
		Location = @Location,
		ResponsibleUserID = @ResponsibleUserID
	OUTPUT Inserted.*
	WHERE
		CalendarEventID = @CalendarEventID
		AND Active = 1

COMMIT TRANSACTION
GO

/*********** uspUpdateUser *******************************/
CREATE PROCEDURE uspUpdateUser
	@UserID			AS INT,
	@UserTypeID		AS INT,
	@Firstname		AS VARCHAR(255),
	@Lastname		AS VARCHAR(255),
	@Username		AS VARCHAR(255),
	@Email			AS VARCHAR(255),
	@Phonenumber	AS VARCHAR(255),
	@Password		AS VARCHAR(255)
AS
SET NOCOUNT ON
SET XACT_ABORT ON

BEGIN TRANSACTION

	UPDATE Users SET 
		UserTypeID = @UserTypeID,
		FirstName = @Firstname,
		LastName = @Lastname,
		Username = @Username,
		Email = @Email,
		PhoneNumber = @Phonenumber,
		Password = @Password
	OUTPUT INSERTED.*
	WHERE UserID = @UserID
		AND Active = 1

COMMIT TRANSACTION
GO