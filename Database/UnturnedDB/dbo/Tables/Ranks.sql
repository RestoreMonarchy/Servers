CREATE TABLE dbo.Ranks (
	RankId SMALLINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Ranks PRIMARY KEY,
	Name NVARCHAR(255) NOT NULL,
	PermissionTags VARCHAR(1024) NULL, -- comma delimited tags, eg kit.vip,kit.epic
	ValidDays SMALLINT NOT NULL CONSTRAINT DF_Ranks_ValidDays DEFAULT (0), -- 0 means permanent
	CreateDate DATETIME2(0) NOT NULL CONSTRAINT DF_Ranks_CreateDate DEFAULT SYSDATETIME(),
	ActiveFlag BIT NOT NULL CONSTRAINT DF_Ranks_ActiveFlag DEFAULT (1)
);