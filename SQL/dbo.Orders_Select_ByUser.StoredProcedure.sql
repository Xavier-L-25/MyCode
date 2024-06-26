USE [Tabi]
GO
/****** Object:  StoredProcedure [dbo].[Orders_Select_ByUser]    Script Date: 6/14/2024 4:22:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Orders_Select_ByUser]
			@UserId int
			,@PageIndex int
			,@PageSize int

AS

/*
	DECLARE @UserId int = 91
			,@PageIndex int = 0
			,@PageSize int = 10

	EXECUTE [dbo].[Orders_Select_ByUser]
			@UserId
			,@PageIndex
			,@PageSize
*/

BEGIN

	DECLARE @offSet int = @PageIndex * @PageSize

	SELECT o.Id
			,o.Name
			,o.Price
			,v.Id AS VenueId
			,v.Name AS VenueName
			,v.Description AS VenueDescription
			,v.LocationId 
			,v.VenueTypeId
			,v.Url
			,v.Createdby AS VenueCreatedBy
			,v.ModifiedBy AS VenueModifiedBy
			,v.DateCreated AS VenueDateCreated
			,v.DateModified AS VenueDateModified
			,u.Id AS UserId
			,u.FirstName
			,u.LastName
			,u.Mi
			,u.AvatarUrl
			,o.DateCreated AS OrderDateCreated
			,TotalCount = COUNT(1) OVER()
	FROM dbo.Orders AS o INNER JOIN dbo.Venues as v
			ON o.VenueId = v.Id
		INNER JOIN dbo.Users AS u
			ON o.CreatedBy = u.Id	
	WHERE o.CreatedBy = @UserId
	ORDER BY o.Id

	OFFSET @offSet rows
	FETCH NEXT @PageSize ROWS ONLY

END
GO
