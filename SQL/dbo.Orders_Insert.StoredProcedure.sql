USE [Tabi]
GO
/****** Object:  StoredProcedure [dbo].[Orders_Insert]    Script Date: 6/14/2024 4:22:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Orders_Insert]
			@Name nvarchar(50)
			,@Price float
			,@VenueId int
			,@CreatedBy int
			,@Id int OUTPUT

AS

/*
	DECLARE @Id int = 1
			,@Name nvarchar(50) = 'Salad'
			,@Price float = 12.99
			,@VenueId int = 2
			,@CreatedBy int = 1

	EXECUTE [dbo].[Orders_Insert]
			@Id OUTPUT
			,@Name 
			,@Price 
			,@VenueId 
			,@CreatedBy
*/

BEGIN

	INSERT INTO dbo.Orders
				(Name
				,Price
				,VenueId
				,CreatedBy)

	VALUES (@Name
			,@Price
			,@VenueId
			,@CreatedBy)

	SET @Id = SCOPE_IDENTITY()

END
GO
