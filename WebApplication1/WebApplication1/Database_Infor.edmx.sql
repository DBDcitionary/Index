
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 04/22/2016 10:50:17
-- Generated from EDMX file: C:\Users\kgothatso.manganye\Documents\GitHub\DB_Dictionary-Project\WebApplication1\WebApplication1\Database_Infor.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [DB_Dictionary];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[fk_DB_TBL]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Table_Tbl] DROP CONSTRAINT [fk_DB_TBL];
GO
IF OBJECT_ID(N'[dbo].[fk_TBL_Field]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Field_Tbl] DROP CONSTRAINT [fk_TBL_Field];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Database_Tbl]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Database_Tbl];
GO
IF OBJECT_ID(N'[dbo].[Field_Tbl]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Field_Tbl];
GO
IF OBJECT_ID(N'[dbo].[Table_Tbl]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Table_Tbl];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Database_Tbl'
CREATE TABLE [dbo].[Database_Tbl] (
    [DB_ID] int IDENTITY(1,1) NOT NULL,
    [DB_Name] varchar(max)  NULL,
    [DB_Description] varchar(max)  NOT NULL,
    [ServerName] varchar(max)  NULL,
    [CreatedDate] datetime  NULL,
    [UpdatedDate] datetime  NULL,
    [FK_Databaseid] varchar(max)  NULL
);
GO

-- Creating table 'Field_Tbl'
CREATE TABLE [dbo].[Field_Tbl] (
    [Field_ID] int IDENTITY(1,1) NOT NULL,
    [Field_Name] varchar(max)  NULL,
    [Field_Description] varchar(max)  NOT NULL,
    [TBL_ID] int  NOT NULL,
    [ServerName] varchar(max)  NULL,
    [DBName] varchar(max)  NULL,
    [TableName] varchar(max)  NULL,
    [SchemaName] varchar(max)  NULL,
    [DataType] varchar(max)  NULL,
    [Prec] int  NULL,
    [is_null] varchar(max)  NULL,
    [CreatedDate] datetime  NULL,
    [UpdatedDate] datetime  NULL,
    [ConstraintType] varchar(max)  NULL,
    [ColLength] int  NULL,
    [FK_Fieldid] varchar(max)  NULL
);
GO

-- Creating table 'Table_Tbl'
CREATE TABLE [dbo].[Table_Tbl] (
    [TBL_ID] int IDENTITY(1,1) NOT NULL,
    [TBL_Name] varchar(max)  NULL,
    [TBL_Description] varchar(max)  NOT NULL,
    [DB_ID] int  NOT NULL,
    [CreatedDate] datetime  NULL,
    [UpdatedDate] datetime  NULL,
    [FK_Tableid] varchar(max)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [DB_ID] in table 'Database_Tbl'
ALTER TABLE [dbo].[Database_Tbl]
ADD CONSTRAINT [PK_Database_Tbl]
    PRIMARY KEY CLUSTERED ([DB_ID] ASC);
GO

-- Creating primary key on [Field_ID] in table 'Field_Tbl'
ALTER TABLE [dbo].[Field_Tbl]
ADD CONSTRAINT [PK_Field_Tbl]
    PRIMARY KEY CLUSTERED ([Field_ID] ASC);
GO

-- Creating primary key on [TBL_ID] in table 'Table_Tbl'
ALTER TABLE [dbo].[Table_Tbl]
ADD CONSTRAINT [PK_Table_Tbl]
    PRIMARY KEY CLUSTERED ([TBL_ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [DB_ID] in table 'Table_Tbl'
ALTER TABLE [dbo].[Table_Tbl]
ADD CONSTRAINT [fk_DB_TBL]
    FOREIGN KEY ([DB_ID])
    REFERENCES [dbo].[Database_Tbl]
        ([DB_ID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'fk_DB_TBL'
CREATE INDEX [IX_fk_DB_TBL]
ON [dbo].[Table_Tbl]
    ([DB_ID]);
GO

-- Creating foreign key on [TBL_ID] in table 'Field_Tbl'
ALTER TABLE [dbo].[Field_Tbl]
ADD CONSTRAINT [fk_TBL_Field]
    FOREIGN KEY ([TBL_ID])
    REFERENCES [dbo].[Table_Tbl]
        ([TBL_ID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'fk_TBL_Field'
CREATE INDEX [IX_fk_TBL_Field]
ON [dbo].[Field_Tbl]
    ([TBL_ID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------