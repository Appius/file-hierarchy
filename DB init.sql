------------------------------------------------------------------------------------------------------------------------------
-- DROPPING TABLES
------------------------------------------------------------------------------------------------------------------------------

IF (EXISTS (SELECT * 
            FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_SCHEMA = 'dbo'
            AND   TABLE_NAME = 'Tree'))
BEGIN
    DROP TABLE [dbo].[Tree]
END


IF (EXISTS (SELECT * 
            FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_SCHEMA = 'dbo'
            AND   TABLE_NAME = 'Entities'))
BEGIN
    DROP TABLE [dbo].[Entities]
END

------------------------------------------------------------------------------------------------------------------------------
-- CREATING ENTITIES TABLE
------------------------------------------------------------------------------------------------------------------------------

USE [Test]
GO

/****** Object:  Table [dbo].[Entities]    Script Date: 26-Apr-17 12:50:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Entities](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ParentId] [int] NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Type] [tinyint] NOT NULL,
	[SeqNum] [int] NOT NULL
 CONSTRAINT [PK_Entities] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Entities]  WITH CHECK ADD  CONSTRAINT [FK_ParentId_Entities] FOREIGN KEY([ParentId])
REFERENCES [dbo].[Entities] ([Id])
GO

ALTER TABLE [dbo].[Entities] CHECK CONSTRAINT [FK_ParentId_Entities]
GO

------------------------------------------------------------------------------------------------------------------------------
-- CREATING TREE TABLE
------------------------------------------------------------------------------------------------------------------------------
USE [Test]
GO

/****** Object:  Table [dbo].[Tree]    Script Date: 27-Apr-17 06:17:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Tree](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ParentId] [int] NOT NULL,
	[ChildId] [int] NOT NULL,
	[Level] [int] NOT NULL,
 CONSTRAINT [PK_Tree] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Tree]  WITH CHECK ADD  CONSTRAINT [FK_Tree_Entities_ParentId] FOREIGN KEY([ParentId])
REFERENCES [dbo].[Entities] ([Id])
GO

ALTER TABLE [dbo].[Tree] CHECK CONSTRAINT [FK_Tree_Entities_ParentId]
GO

ALTER TABLE [dbo].[Tree]  WITH CHECK ADD  CONSTRAINT [FK_Tree_Entities_ChildId] FOREIGN KEY([ChildId])
REFERENCES [dbo].[Entities] ([Id])
GO

ALTER TABLE [dbo].[Tree] CHECK CONSTRAINT [FK_Tree_Entities_ChildId]
GO

------------------------------------------------------------------------------------------------------------------------------
-- CREATING TRIGGERS
------------------------------------------------------------------------------------------------------------------------------
CREATE TRIGGER [FillHierarchy] ON [dbo].[Entities]
AFTER INSERT
AS
WITH Hierarchy([ChildId], [ParentId], [Level]) AS
(
     SELECT
          [e].[Id]
         ,[e].[ParentID]
         ,1 AS [Level]
     FROM [Entities] [e]
     JOIN [inserted] [i] ON [i].[Id] = [e].[Id]
     WHERE [i].[ParentID] IS NOT NULL

     UNION ALL

     SELECT
          [eh].[ChildId]
         ,[e].[ParentId]
         ,[eh].[Level] + 1
     FROM [Entities] AS [e]
     JOIN [Hierarchy] AS [eh] ON [e].[Id] = [eh].[ParentId]
)
INSERT INTO [Tree]
(
     [ParentId]
    ,[ChildId]
    ,[Level]
)
SELECT
     [ParentId]
    ,[ChildId]
    ,[Level]
FROM [Hierarchy] [h]
WHERE [h].[ParentId] IS NOT NULL;
GO

CREATE TRIGGER [DeleteHierarchy] ON [dbo].[Entities]
AFTER DELETE
AS
    DELETE [t]
    FROM [Tree] [t]
    JOIN [deleted] [d] on [d].Id = [t].[ChildId]
GO

------------------------------------------------------------------------------------------------------------------------------
-- INSERTING INITIAL DATA
------------------------------------------------------------------------------------------------------------------------------
INSERT INTO [dbo].[Entities] ([ParentId], [Name], [Type], [SeqNum])
VALUES
(null, 'Financials', 0, 1),
(null, 'Org Structure', 0, 2),
(null, 'Doc.docx', 1, 3);

GO
DECLARE @parentId INT = (SELECT Id FROM [dbo].[Entities] WHERE [Name] = 'Financials')
INSERT INTO [dbo].[Entities] ([ParentId], [Name], [Type], [SeqNum])
VALUES
(@parentId, 'Taxes', 0, 1),
(@parentId, 'Debits', 0, 2);

GO
DECLARE @parentId INT = (SELECT Id FROM [dbo].[Entities] WHERE [Name] = 'Taxes')
INSERT INTO [dbo].[Entities] ([ParentId], [Name], [Type], [SeqNum])
VALUES
(@parentId, 'Q1', 0, 1);

GO
DECLARE @parentId INT = (SELECT Id FROM [dbo].[Entities] WHERE [Name] = 'Q1')
INSERT INTO [dbo].[Entities] ([ParentId], [Name], [Type], [SeqNum])
VALUES
(@parentId, 'Doc.pdf', 1, 1);

GO
DECLARE @parentId INT = (SELECT Id FROM [dbo].[Entities] WHERE [Name] = 'Debits')
INSERT INTO [dbo].[Entities] ([ParentId], [Name], [Type], [SeqNum])
VALUES
(@parentId, 'Q2', 0, 1);

GO
DECLARE @parentId INT = (SELECT Id FROM [dbo].[Entities] WHERE [Name] = 'Q2')
INSERT INTO [dbo].[Entities] ([ParentId], [Name], [Type], [SeqNum])
VALUES
(@parentId, 'Doc.pdf', 1, 1);

GO
DECLARE @parentId INT = (SELECT Id FROM [dbo].[Entities] WHERE [Name] = 'Org Structure');
INSERT INTO [dbo].[Entities] ([ParentId], [Name], [Type], [SeqNum])
VALUES
(@parentId, 'Doc.pdf', 1, 1);
