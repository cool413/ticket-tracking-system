IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[system].[Role]') AND type in (N'U')) 
TRUNCATE TABLE [system].[Role]
GO

SET IDENTITY_INSERT [system].[Role] ON
GO

insert into [system].[Role](ID,Name,CreatedAt,CreatedBy,LastModifiedAt,LastModifiedBy) values
(1,'RD',GETDATE(),'system',GETDATE(),'system')
,(2,'QA',GETDATE(),'system',GETDATE(),'system')

SET IDENTITY_INSERT [system].Role OFF
GO