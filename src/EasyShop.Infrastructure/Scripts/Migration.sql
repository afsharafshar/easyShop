
-----------------product
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Products' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
CREATE TABLE dbo.Products
(
    Id UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    Sku NVARCHAR(100) NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Product_IsActive DEFAULT (1),

    CONSTRAINT PK_Product PRIMARY KEY CLUSTERED (Id),
);
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Product_Name')
BEGIN
CREATE INDEX IX_Product_Name ON dbo.Products(Name);
END
 
--------------------- inventoryItem
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'InventoryItems' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
CREATE TABLE dbo.InventoryItems
(
    Id UNIQUEIDENTIFIER NOT NULL,
    ProductId UNIQUEIDENTIFIER NOT NULL,
    WarehouseId UNIQUEIDENTIFIER NOT NULL,
    OnHandQty INT NOT NULL,
    ReservedQty INT NOT NULL CONSTRAINT DF_Inventory_ReservedQty DEFAULT (0),
    RowVersion ROWVERSION NOT NULL,

    CONSTRAINT PK_InventoryItem
        PRIMARY KEY CLUSTERED (Id)
);
END
 

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_InventoryItem_Product')
BEGIN
ALTER TABLE dbo.InventoryItems
    ADD CONSTRAINT FK_InventoryItem_Product
        FOREIGN KEY (ProductId)
            REFERENCES dbo.Products(Id);
END
 

------------------ order 
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Orders' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
CREATE TABLE dbo.[Orders]
(
    Id UNIQUEIDENTIFIER NOT NULL,
    CustomerId UNIQUEIDENTIFIER NOT NULL,
    Status int NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    CreatedAt DATETIME2 NOT NULL
    CONSTRAINT DF_Order_CreatedAt DEFAULT (SYSUTCDATETIME()),
    RowVersion ROWVERSION NOT NULL,

    CONSTRAINT PK_Orders
    PRIMARY KEY CLUSTERED (Id)
    );
END
 

------------------ order item
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'OrderItems' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
CREATE TABLE dbo.OrderItems
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
 

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_OrderItem_Order')
BEGIN
ALTER TABLE dbo.OrderItems
    ADD CONSTRAINT FK_OrderItem_Order
        FOREIGN KEY (OrderId)
            REFERENCES dbo.[Orders](Id)
        ON DELETE CASCADE;
END
 

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_OrderItem_Product')
BEGIN
ALTER TABLE dbo.OrderItems
    ADD CONSTRAINT FK_OrderItem_Product
        FOREIGN KEY (ProductId)
            REFERENCES dbo.Products(Id);
END
 

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_OrderItem_OrderId')
BEGIN
CREATE INDEX IX_OrderItem_OrderId
    ON dbo.OrderItems(Id);
END
 

------------ outbox
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Outboxs' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
CREATE TABLE dbo.Outboxs
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
 