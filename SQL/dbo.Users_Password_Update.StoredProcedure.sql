USE [Tabi]
GO
/****** Object:  StoredProcedure [dbo].[Users_Password_Update]    Script Date: 6/14/2024 4:22:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Users_Password_Update]
			@Token varchar(200)
			,@Password varchar(100)

AS

/*

*/

BEGIN 

	DECLARE @UserId int

	If EXISTS (
			SELECT 1
			FROM dbo.UserTokens
			Where Token = @Token
		)
	BEGIN
		SET @UserId = (
				SELECT UserId
				FROM dbo.UserTokens
				WHERE Token = @Token
			)

		UPDATE dbo.Users
		SET [Password] = @Password
		WHERE Id = @UserId

		DELETE FROM dbo.UserTokens
		WHERE Token = @Token
	END

END
GO
