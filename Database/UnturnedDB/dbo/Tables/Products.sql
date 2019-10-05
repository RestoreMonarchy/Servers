CREATE TABLE dbo.Products
(
	ProductId INT IDENTITY(1, 1) NOT NULL,
	Name VARCHAR(30) NOT NULL,
	Description VARCHAR(4000) NOT NULL,
	Price DECIMAL(9, 2) NOT NULL,
	Category VARCHAR(30) NOT NULL,
	Duration INT DEFAULT(NULL) NULL,
	UpdateDate DATETIME2 NOT NULL DEFAULT(SYSDATETIME()),
	CreateDate DATETIME2 NOT NULL DEFAULT(SYSDATETIME())
);

--public class Product
--    {
--        public int ProductId { get; set; }
--        public string Name { get; set; }
--        public string Description { get; set; }
--        public decimal Price { get; set; }
--        public string Category { get; set; }
--        public int? Duration { get; set; }
--        public DateTime UpdateDate { get; set; }
--        public DateTime CreateDate { get; set; }
--    }