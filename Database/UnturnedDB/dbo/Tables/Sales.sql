﻿CREATE TABLE dbo.Sales
(
	SaleId INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_Sales PRIMARY KEY,
	ProductId SMALLINT NOT NULL CONSTRAINT FK_Sales_ProductId REFERENCES dbo.Products(ProductId),
	PlayerId VARCHAR(255) NOT NULL CONSTRAINT FK_Sales_PlayerId REFERENCES dbo.Players(PlayerId),
	TransactionId VARCHAR(255) NULL,
	TransactionType VARCHAR(255) NULL,
	PayerEmail VARCHAR(255) NULL,
	PaymentType VARCHAR(255) NULL,	
	PaymentStatus VARCHAR(255) NOT NULL CONSTRAINT DF_Sales_PaymentStatus DEFAULT ('PENDING'),
	Price DECIMAL(9, 2) NOT NULL,
	Currency CHAR(3) NOT NULL,
	CreateDate DATETIME2(0) NOT NULL CONSTRAINT DF_Sales_CreateDate DEFAULT(SYSDATETIME())
);