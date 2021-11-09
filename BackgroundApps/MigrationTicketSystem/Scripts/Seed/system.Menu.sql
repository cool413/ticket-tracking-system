IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[system].[Menu]') AND type in (N'U')) 
TRUNCATE TABLE [system].[Menu]
GO

SET IDENTITY_INSERT [system].[Menu] ON
GO

insert into [system].[Menu](ID,Name,CreatedAt,CreatedBy,LastModifiedAt,LastModifiedBy) values
(1,'Bug單管理頁',GETDATE(),'system',GETDATE(),'system')

SET IDENTITY_INSERT [system].[Menu] OFF
GO