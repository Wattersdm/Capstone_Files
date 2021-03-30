USE PlanIt
GO

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

