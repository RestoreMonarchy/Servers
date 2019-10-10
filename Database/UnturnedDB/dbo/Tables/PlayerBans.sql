CREATE TABLE [dbo].[PlayerBans]
(
	BanId INT NOT NULL IDENTITY(1,1),
    PlayerId VARCHAR(255) NOT NULL,
    PunisherId VARCHAR(255) NOT NULL,
    BanReason VARCHAR(255) NULL,
    BanDuration INT NULL,
    BanCreated DATETIME2 NOT NULL DEFAULT(SYSDATETIME()),
    SendFlag BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_BanId PRIMARY KEY (BanId),
    CONSTRAINT FK_PlayerBans_PlayerId FOREIGN KEY (PlayerId) REFERENCES Players(PlayerId),
    CONSTRAINT FK_PlayerBans_PunisherId FOREIGN KEY (PunisherId) REFERENCES Players(PlayerId)
)
