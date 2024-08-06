use [recipe_manager]
go

CREATE TABLE [recipes] (
    [id] int identity(1, 1) NOT NULL,
    -- [category] nvarchar(15) NULL,
	[category] int NULL,
    [name] nvarchar(128) NULL,
	[image] nvarchar(1024) NULL,
	[description] nvarchar(max) NULL,
	[ingredients] nvarchar(max) NULL,
	[instructions] nvarchar(max) NULL,
	[add_date] datetime NULL,
	[change_date] datetime NULL,
    CONSTRAINT [PK_recipes] PRIMARY KEY ([id]),
);
go

CREATE TABLE [categories] (
    [id] int identity(1, 1) NOT NULL,    
    [name] nvarchar(128) NULL,	
	[image] nvarchar(1024) NULL,
    CONSTRAINT [PK_categories] PRIMARY KEY ([id]),
);
go

-- =============================================
-- Author: Joanna	
-- Create date: 5/1/2024
-- Description:	Returns recipes containing the searched text in name, description, or ingredients.
-- =============================================
CREATE PROCEDURE [dbo].[SearchRecipesByText] 
	-- Add the parameters for the stored procedure here
	@searchText nvarchar(256) = NULL,	
	@categoryId bigint = NULL,
	@pageNumber int = 1,
	@recordsPerPage int = 16
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements
	SET NOCOUNT ON;	

	DECLARE @totalPages int = 1

	IF @pageNumber < 1 SET @pageNumber = 1;
	IF @recordsPerPage < 1 SET @recordsPerPage = 1;

	-- calculate the total number of pager pages in the result set for paging configuration
	SELECT @totalPages = ceiling(cast(count(*) as decimal) / cast(@recordsPerPage as decimal))
	FROM [recipes]
    WHERE 
		([name] like concat('%', @searchText, '%')
		or [description] like concat('%', @searchText, '%')
		or [ingredients] like concat('%', @searchText, '%'))
		and (@categoryId is NULL or [category] = @categoryId)

	SELECT *, @totalPages [total_pages] 
	FROM [recipes]
    WHERE 
		([name] like concat('%', @searchText, '%')
		or [description] like concat('%', @searchText, '%')
		or [ingredients] like concat('%', @searchText, '%'))
		and (@categoryId is NULL or [category] = @categoryId)
	ORDER BY add_date
	OFFSET (@pageNumber - 1) * @recordsPerPage ROWS
    FETCH NEXT @recordsPerPage ROWS only
END
GO