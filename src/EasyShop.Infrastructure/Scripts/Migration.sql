
-----------------product
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Product' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
CREATE TABLE dbo.Product
(
    Id UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    Sku NVARCHAR(100) NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Product_IsActive DEFAULT (1),

    CONSTRAINT PK_Product PRIMARY KEY CLUSTERED (Id),
);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Product_Name')
BEGIN
CREATE INDEX IX_Product_Name ON dbo.Product(Name);
END
GO
--------------------- inventoryItem
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'InventoryItem' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
CREATE TABLE dbo.InventoryItem
(
    Id UNIQUEIDENTIFIER NOT NULL,
    ProductId UNIQUEIDENTIFIER NOT NULL,
    WarehouseId UNIQUEIDENTIFIER NOT NULL,
    OnHandQty INT NOT NULL,
    ReservedQty INT NOT NULL CONSTRAINT DF_Product_IsActive DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    CONSTRAINT PK_InventoryItem
        PRIMARY KEY CLUSTERED (Id)
);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_InventoryItem_Product')
BEGIN
ALTER TABLE dbo.InventoryItem
    ADD CONSTRAINT FK_InventoryItem_Product
        FOREIGN KEY (ProductId)
            REFERENCES dbo.Product(Id);
END
GO

------------------ order 
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Order' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
CREATE TABLE dbo.[Order]
(
    Id UNIQUEIDENTIFIER NOT NULL,
    CustomerId UNIQUEIDENTIFIER NOT NULL,
    Status int NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME2 NOT NULL
    CONSTRAINT DF_Order_CreatedAt DEFAULT (SYSUTCDATETIME()),
    RowVersion ROWVERSION NOT NULL,

    CONSTRAINT PK_Order
    PRIMARY KEY CLUSTERED (Id)
    );
END
GO

------------------ order item
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'OrderItem' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
CREATE TABLE dbo.OrderItem
(
    Id UNIQUEIDENTIFIER NOT NULL,
    OrderId UNIQUEIDENTIFIER NOT NULL,
    ProductId UNIQUEIDENTIFIER NOT NULL,
    Qty INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,

    CONSTRAINT PK_OrderItem
        PRIMARY KEY CLUSTERED (Id)
);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_OrderItem_Order')
BEGIN
ALTER TABLE dbo.OrderItem
    ADD CONSTRAINT FK_OrderItem_Order
        FOREIGN KEY (OrderId)
            REFERENCES dbo.[Order](Id)
        ON DELETE CASCADE;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_OrderItem_Product')
BEGIN
ALTER TABLE dbo.OrderItem
    ADD CONSTRAINT FK_OrderItem_Product
        FOREIGN KEY (ProductId)
            REFERENCES dbo.Product(Id);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_OrderItem_OrderId')
BEGIN
CREATE INDEX IX_OrderItem_OrderId
    ON dbo.OrderItem(Id);
END
GO

------------ outbox
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Outbox' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
CREATE TABLE dbo.Outbox
(
    Id UNIQUEIDENTIFIER NOT NULL,
    Type NVARCHAR(200) NOT NULL,
    Body NVARCHAR(100) NOT NULL,
    IsDone BIT NOT NULL
        CONSTRAINT DF_Outbox_IsDone DEFAULT (0),
    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Outbox_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT PK_Outbox
        PRIMARY KEY CLUSTERED (Id)
);
END
GO