CREATE TABLE [dbo].[Tickets]
(
	TicketId INT IDENTITY(1, 1) NOT NULL,
	TicketTitle VARCHAR(255) NOT NULL,
	TicketContent NVARCHAR(3000) NOT NULL,
	TicketCategory VARCHAR(50) NOT NULL,
	TicketAuthor VARCHAR(255) NOT NULL,
	TargetTicketId INT NULL,
	TicketUpdate DATETIME2(0) NOT NULL CONSTRAINT DF_Tickets_TicketUpdate DEFAULT(SYSDATETIME()),
	TicketCreated DATETIME2(0) NOT NULL CONSTRAINT DF_Tickets_TicketCreated DEFAULT(SYSDATETIME()),
	CONSTRAINT PK_Tickets_TicketId PRIMARY KEY (TicketId),
	CONSTRAINT FK_Tickets_TargetTicketId FOREIGN KEY (TargetTicketId) REFERENCES dbo.Tickets(TicketId)	
)
