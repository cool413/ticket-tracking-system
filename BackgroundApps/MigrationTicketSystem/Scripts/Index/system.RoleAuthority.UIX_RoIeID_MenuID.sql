drop index if exists UIX_RoIeID_MenuID on system.RoleAuthority
GO
create unique index UIX_RoIeID_MenuID on system.RoleAuthority(
    RoleID,
    MenuID
	)
	WITH (MAXDOP = 4)
GO
