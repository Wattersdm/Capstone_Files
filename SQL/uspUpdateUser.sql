USE PlanIt
GO

DROP PROCEDURE IF EXISTS uspUpdateUser
GO

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