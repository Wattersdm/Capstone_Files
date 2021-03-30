USE PlanIt
GO

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