CREATE TABLE [dbo].[PermissionMembers] (
    [GroupID] VARCHAR (30) NOT NULL,
    [SteamID] BIGINT       NOT NULL,
    FOREIGN KEY ([GroupID]) REFERENCES [dbo].[PermissionGroups] ([GroupID])
);

