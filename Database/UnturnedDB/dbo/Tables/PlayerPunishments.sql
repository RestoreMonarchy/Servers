CREATE TABLE PlayerPunishments 
(
	PunishmentId INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_PlayerPunishments PRIMARY KEY,
	PlayerId VARCHAR(255) NOT NULL CONSTRAINT FK_PlayerPunishments_PlayerId REFERENCES dbo.Players (PlayerId),
	PunisherId VARCHAR(255) NOT NULL CONSTRAINT FK_PlayerPunishments_PunisherId REFERENCES dbo.Players (PlayerId),
	Category VARCHAR(255) NOT NULL,
	Reason NVARCHAR(1000) NULL,
	ExpiryDate DATETIME2(0) NULL,
	CreateDate DATETIME2(0) NOT NULL CONSTRAINT DF_PlayerPunishments_CreateDate DEFAULT (SYSDATETIME())
);