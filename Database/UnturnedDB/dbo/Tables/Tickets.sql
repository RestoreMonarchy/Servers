﻿CREATE TABLE dbo.Tickets 
(
	TicketId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Tickets PRIMARY KEY,
	Title NVARCHAR(60) NOT NULL,
	Content NVARCHAR(4000) NOT NULL,
	Category VARCHAR(60) NOT NULL,
	AuthorId VARCHAR(255) NOT NULL CONSTRAINT FK_Tickets_AuthorId REFERENCES dbo.Players (PlayerId),
	Status BIT NOT NULL CONSTRAINT DF_Tickets_Status DEFAULT (0),	
	LastUpdate DATETIME2(0) NOT NULL CONSTRAINT DF_Tickets_LastUpdate DEFAULT (SYSDATETIME()),
	CreateDate DATETIME2(0) NOT NULL CONSTRAINT DF_Tickets_CreateDate DEFAULT (SYSDATETIME())
);