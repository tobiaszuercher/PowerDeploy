-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Movies'
-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Movies'
CREATE TABLE [dbo].[Movies] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ImdbId] nvarchar(max)  NOT NULL,
    [TmdbId] int  NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Teaser] nvarchar(max)  NOT NULL,
    [Year] int  NOT NULL,
    [Backdrop] nvarchar(max)  NOT NULL,
    [Cover] nvarchar(max)  NOT NULL
);





