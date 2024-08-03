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

CREATE TABLE [categories] (
    [id] int identity(1, 1) NOT NULL,    
    [name] nvarchar(128) NULL,	
	[image] nvarchar(1024) NULL,
    CONSTRAINT [PK_categories] PRIMARY KEY ([id]),
);