USE PlanIt
GO

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