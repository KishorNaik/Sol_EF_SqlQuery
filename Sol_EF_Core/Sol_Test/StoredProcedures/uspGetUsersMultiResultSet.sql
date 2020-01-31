CREATE PROC uspGetUsersMultiResultSet
	@UserId numeric(18,0)
AS
	BEGIN
		
		SELECT * FROM tblUsers WHERE UserId=@UserId

		SELECT * FROM tblUserLogin WHERE UserId=@UserId

	END