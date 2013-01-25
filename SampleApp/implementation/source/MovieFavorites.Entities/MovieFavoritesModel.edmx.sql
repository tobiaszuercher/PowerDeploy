
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 10/26/2012 15:00:45
-- Generated from EDMX file: C:\Users\tzu\Documents\Visual Studio 2010\Projects\MovieFavorites\MovieFavorites.Entities\MovieFavoritesModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [MovieFavorites];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Movies]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Movies];
GO

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
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Movies'
ALTER TABLE [dbo].[Movies]
ADD CONSTRAINT [PK_Movies]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------