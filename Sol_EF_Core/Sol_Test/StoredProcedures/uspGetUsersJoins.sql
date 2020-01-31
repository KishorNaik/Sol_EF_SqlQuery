CREATE PROC uspGetUsersJoins
	@UserId NUmeric(18,0)
AS
	BEGIN
		
		SELECT
			U.FirstName,
			U.LastName,
			UL.UserName,
			UL.Password
		FROM 
			tblUsers as U
		INNER JOIN 
			tblUserLogin AS UL
		ON 
			U.UserId=UL.UserId
		WHERE 
			U.UserId=@UserId

	END

