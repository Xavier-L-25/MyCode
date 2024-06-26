USE [Tabi]
GO
/****** Object:  StoredProcedure [dbo].[User_EmailCheck]    Script Date: 6/14/2024 4:22:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[User_EmailCheck]
			@Email nvarchar(255)
			,@Token varchar(200)

AS

/*

*/

BEGIN

	DECLARE @UserId int

	IF EXISTS (
			SELECT 1
			FROM dbo.Users
			WHERE Email = @Email
		)
	BEGIN
		SET @UserId = (
				SELECT Id
				FROM dbo.Users
				WHERE Email = @Email
			)

		INSERT INTO dbo.UserTokens
					([Token]
					,[UserId]
					,[TokenType])

		VALUES (@Token
				,@UserId
				,2)
	END

END
GO
