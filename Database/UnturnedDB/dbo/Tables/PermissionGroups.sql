CREATE TABLE [dbo].[PermissionGroups] (
    [GroupID]       VARCHAR (30)  NOT NULL,
    [GroupName]     NVARCHAR (90) NOT NULL,
    [GroupColor]    VARCHAR (7)   NULL,
    [GroupPriority] TINYINT       DEFAULT ((0)) NOT NULL,
    [DateCreated]   DATETIME2 (0) DEFAULT (sysdatetime()) NOT NULL,
    PRIMARY KEY CLUSTERED ([GroupID] ASC)
);