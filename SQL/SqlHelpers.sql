use PlanIt

/* find children associated to adult users */
SELECT
	U.UserID
FROM
	Users U
	INNER JOIN Relationships R
	ON R.ChildUserID = U.UserID

	INNER JOIN UserTypes UT
	ON U.UserTypeID = UT.UserTypeID

WHERE
	R.OtherUserID = 5
	AND R.RelationshipTypeID = 1

/* find children associated to adult users: compact */
SELECT	U.UserID FROM Users U INNER JOIN Relationships R  ON R.ChildUserID = U.UserID INNER JOIN UserTypes UT ON U.UserTypeID = UT.UserTypeID WHERE R.OtherUserID = 5 AND R.RelationshipTypeID = 1


/* Find adults associated to children users */
SELECT 
	U.UserID
FROM
	Users U
	INNER JOIN Relationships R
	ON R.OtherUserID = U.userID
WHERE
	R.childUserID = 10
	AND RelationshipTypeID = 1

/* Find adults associated to children users: compact */

SELECT U.UserID FROM Users U INNER JOIN Relationships R ON R.OtherUserID = U.userID WHERE R.childUserID = 10 AND RelationshipTypeID = 1