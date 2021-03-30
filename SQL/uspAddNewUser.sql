Use Planit
GO

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
	VALUES (@UserID, @UserTypeID, @Username, @FirstName, @LastName, @Email, @PhoneNumber, @Password)

	SELECT @UserID
COMMIT TRANSACTION

GO