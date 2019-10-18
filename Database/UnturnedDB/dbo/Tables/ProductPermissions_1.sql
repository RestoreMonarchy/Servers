CREATE TABLE dbo.ProductPermissions 
(
	PermissionTag VARCHAR(30) NOT NULL,
	ProductId INT NOT NULL CONSTRAINT FK_ProductPermissions_ProductId REFERENCES dbo.Products(ProductId)
);