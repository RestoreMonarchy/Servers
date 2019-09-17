namespace RestoreMonarchy.Moderation.Database
{
    public class DatabaseQuery
    {
        public const string PlayersTable = @"
CREATE TABLE IF NOT EXISTS Players (
    PlayerId BIGINT NOT NULL,
    PlayerName NVARCHAR(255) NOT NULL,
    PlayerCountry CHAR(2) NULL,
    PlayerCreated DATETIME NOT NULL,
    CONSTRAINT PK_Players PRIMARY KEY (PlayerId)    
);
        ";

        public const string BansTable = @"
CREATE TABLE IF NOT EXISTS Bans (
    BanId INT NOT NULL AUTO_INCREMENT,
    PlayerId BIGINT NOT NULL,
    PunisherId BIGINT NOT NULL,
    BanReason VARCHAR(255) NULL,
    BanDuration INT(11) NULL,
    BanCreated DATETIME NOT NULL,
    SendFlag TINYINT NOT NULL DEFAULT 0,
    CONSTRAINT PK_BanId PRIMARY KEY (BanId),
    CONSTRAINT FK_Players_PlayerId FOREIGN KEY (PlayerId) REFERENCES Players(PlayerId),
    CONSTRAINT FK_Players_PunisherId FOREIGN KEY (PlayerId) REFERENCES Players(PlayerId)
);
        ";
    }
}
