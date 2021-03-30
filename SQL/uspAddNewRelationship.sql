USE PlanIt
GO

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