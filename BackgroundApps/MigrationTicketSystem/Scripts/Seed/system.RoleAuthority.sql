IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[system].[RoleAuthority]') AND type in (N'U')) 
TRUNCATE TABLE [system].[RoleAuthority]
GO

SET IDENTITY_INSERT [system].[RoleAuthority] ON
GO

insert into [system].[RoleAuthority](ID,RoleID,MenuID,CanInsert,CanDelete,CanUpdate,CanRead,CanResolve,CreatedAt,CreatedBy,LastModifiedAt,LastModifiedBy) values
(1,1,1,0,0,0,1,1,GETDATE(),'system',GETDATE(),'system')
,(2,2,1,1,1,1,1,1,GETDATE(),'system',GETDATE(),'system')

SET IDENTITY_INSERT [system].[RoleAuthority] OFF
GO
