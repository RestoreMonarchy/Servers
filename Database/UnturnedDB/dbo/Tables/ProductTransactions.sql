CREATE TABLE dbo.ProductTransactions
(
	PurchaseId INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_ProductPurchases PRIMARY KEY,
	ProductId INT NOT NULL CONSTRAINT FK_ProductPurchases_ProductId REFERENCES dbo.Products(ProductId),
	PlayerId VARCHAR(255) NOT NULL CONSTRAINT FK_ProductPurchases_PlayerId REFERENCES dbo.Players(PlayerId),
	PaymentMethod VARCHAR(255) NOT NULL,
	PaymentStatus VARCHAR(255) NOT NULL,
	Price DECIMAL(9, 2) NOT NULL,
	Duration INT NULL,
	ExpiryDate DATETIME2 NULL,
	CreateDate DATETIME2 NOT NULL CONSTRAINT DF_ProductPurchases_CreateDate DEFAULT(SYSDATETIME())
);