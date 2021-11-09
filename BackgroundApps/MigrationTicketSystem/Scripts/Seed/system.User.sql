IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[system].[User]') AND type in (N'U')) 
TRUNCATE TABLE [system].[User]
GO

SET IDENTITY_INSERT [system].[User] ON
GO

insert into [system].[User](ID,Account,RoleID,CreatedAt,CreatedBy,LastModifiedAt,LastModifiedBy) values
(1,'Default_RD',1,GETDATE(),'system',GETDATE(),'system')
,(2,'Default_QA',2,GETDATE(),'system',GETDATE(),'system')

SET IDENTITY_INSERT [system].[User] OFF
GO