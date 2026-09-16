USE [Amazon];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @Now DATETIMEOFFSET = SYSDATETIMEOFFSET();
DECLARE @User NVARCHAR(100) = N'System';

BEGIN TRANSACTION;

PRINT N'Seeding products...';

INSERT INTO dbo.Products (Name, Price, ProductType, Author, CreatedAt, CreatedBy)
SELECT v.Name, v.Price, N'Book', v.Author, @Now, @User
FROM (VALUES
    (N'The Girl on the Train',    14.50, N'Paula Hawkins'),
    (N'Clean Code',               32.99, N'Robert C. Martin'),
    (N'Dune',                     18.75, N'Frank Herbert'),
    (N'The Pragmatic Programmer', 39.90, N'Andrew Hunt, David Thomas'),
    (N'Sapiens',                  21.00, N'Yuval Noah Harari')
) AS v (Name, Price, Author)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Products AS p WHERE p.Name = v.Name);

INSERT INTO dbo.Products (Name, Price, ProductType, DurationMinutes, CreatedAt, CreatedBy)
SELECT v.Name, v.Price, N'Video', v.DurationMinutes, @Now, @User
FROM (VALUES
    (N'Comprehensive First Aid Training', 19.00,  95),
    (N'Intro to Watercolor Painting',     12.00,  60),
    (N'C# Fundamentals',                  24.99, 180),
    (N'Yoga for Beginners',                9.99,  45)
) AS v (Name, Price, DurationMinutes)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Products AS p WHERE p.Name = v.Name);

INSERT INTO dbo.Products (Name, Price, ProductType, [Type], CreatedAt, CreatedBy)
SELECT v.Name, v.Price, N'Membership', v.MembershipType, @Now, @User
FROM (VALUES
    (N'Book Club Membership',  15.00, N'BookClub'),
    (N'Video Club Membership', 15.00, N'VideoClub'),
    (N'Premium Membership',    25.00, N'Premium')
) AS v (Name, Price, MembershipType)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Products AS p WHERE p.Name = v.Name);

PRINT N'Seeding customers...';

INSERT INTO dbo.Customers (Name, Email, Membership, CreatedAt, CreatedBy)
SELECT v.Name, v.Email, v.Membership, @Now, @User
FROM (VALUES
    (N'Jane Doe',     N'jane.doe@example.com',     N'BookClub'),
    (N'John Smith',   N'john.smith@example.com',   N'Premium'),
    (N'Maria Garcia', N'maria.garcia@example.com', N'None'),
    (N'Alex Popescu', N'alex.popescu@example.com', N'VideoClub'),
    (N'Emma Brown',   N'emma.brown@example.com',   N'None')
) AS v (Name, Email, Membership)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Customers AS c WHERE c.Email = v.Email);

IF NOT EXISTS (SELECT 1 FROM dbo.PurchaseOrders)
BEGIN
    PRINT N'Seeding purchase orders and shipping slips...';

    DECLARE @Orders TABLE (OrderNo INT PRIMARY KEY, CustomerEmail NVARCHAR(320) NOT NULL);
    DECLARE @OrderItems TABLE (OrderNo INT NOT NULL, Seq INT NOT NULL, ProductName NVARCHAR(200) NOT NULL);

    INSERT INTO @Orders (OrderNo, CustomerEmail)
    VALUES
        (1, N'jane.doe@example.com'),
        (2, N'john.smith@example.com'),
        (3, N'maria.garcia@example.com'),
        (4, N'alex.popescu@example.com'),
        (5, N'maria.garcia@example.com');

    INSERT INTO @OrderItems (OrderNo, Seq, ProductName)
    VALUES
        (1, 1, N'Comprehensive First Aid Training'),
        (1, 2, N'The Girl on the Train'),
        (1, 3, N'Book Club Membership'),
        (2, 1, N'Premium Membership'),
        (3, 1, N'Clean Code'),
        (3, 2, N'Dune'),
        (4, 1, N'Video Club Membership'),
        (4, 2, N'Intro to Watercolor Painting'),
        (5, 1, N'Yoga for Beginners');

    DECLARE @OrderNo INT = 1;
    DECLARE @LastOrderNo INT = (SELECT MAX(OrderNo) FROM @Orders);
    DECLARE @PurchaseOrderId INT;
    DECLARE @ShippingSlipId INT;

    WHILE @OrderNo <= @LastOrderNo
    BEGIN
        INSERT INTO dbo.PurchaseOrders (CustomerId, Total, Status, ProcessedAt, CreatedAt, CreatedBy)
        SELECT c.Id, 0, N'Processed', @Now, @Now, @User
        FROM @Orders AS o
        JOIN dbo.Customers AS c ON c.Email = o.CustomerEmail
        WHERE o.OrderNo = @OrderNo;

        IF @@ROWCOUNT = 0
            THROW 50001, N'Customer for a seeded purchase order was not found.', 1;

        SET @PurchaseOrderId = SCOPE_IDENTITY();

        INSERT INTO dbo.PurchaseOrderLines (PurchaseOrderId, ProductId, ProductName, Price)
        SELECT @PurchaseOrderId, p.Id, p.Name, p.Price
        FROM @OrderItems AS i
        JOIN dbo.Products AS p ON p.Name = i.ProductName
        WHERE i.OrderNo = @OrderNo
        ORDER BY i.Seq;

        UPDATE dbo.PurchaseOrders
        SET Total = (SELECT SUM(l.Price) FROM dbo.PurchaseOrderLines AS l WHERE l.PurchaseOrderId = @PurchaseOrderId)
        WHERE Id = @PurchaseOrderId;

        IF EXISTS (
            SELECT 1
            FROM dbo.PurchaseOrderLines AS l
            JOIN dbo.Products AS p ON p.Id = l.ProductId
            WHERE l.PurchaseOrderId = @PurchaseOrderId AND p.ProductType = N'Book')
        BEGIN
            INSERT INTO dbo.ShippingSlips (PurchaseOrderId, CustomerId, CreatedAt, CreatedBy)
            SELECT po.Id, po.CustomerId, @Now, @User
            FROM dbo.PurchaseOrders AS po
            WHERE po.Id = @PurchaseOrderId;

            SET @ShippingSlipId = SCOPE_IDENTITY();

            INSERT INTO dbo.ShippingSlipLines (ShippingSlipId, ProductId, ProductName)
            SELECT @ShippingSlipId, l.ProductId, l.ProductName
            FROM dbo.PurchaseOrderLines AS l
            JOIN dbo.Products AS p ON p.Id = l.ProductId
            WHERE l.PurchaseOrderId = @PurchaseOrderId AND p.ProductType = N'Book'
            ORDER BY l.Id;
        END;

        SET @OrderNo += 1;
    END;
END
ELSE
BEGIN
    PRINT N'Purchase orders already exist, skipping order seeding.';
END;

COMMIT TRANSACTION;

PRINT N'Seeding completed.';
GO

SELECT p.Id, p.ProductType, p.Name, p.Price, p.Author, p.DurationMinutes, p.[Type] AS MembershipType, p.CreatedAt, p.CreatedBy
FROM dbo.Products AS p
ORDER BY p.Id;

SELECT c.Id, c.Name, c.Email, c.Membership, c.CreatedAt, c.CreatedBy
FROM dbo.Customers AS c
ORDER BY c.Id;

SELECT
    po.Id AS PurchaseOrderId,
    c.Name AS Customer,
    po.Total,
    po.Status,
    po.ProcessedAt,
    (SELECT COUNT(*) FROM dbo.PurchaseOrderLines AS l WHERE l.PurchaseOrderId = po.Id) AS LineCount,
    CASE WHEN EXISTS (SELECT 1 FROM dbo.ShippingSlips AS s WHERE s.PurchaseOrderId = po.Id) THEN N'Yes' ELSE N'No' END AS HasShippingSlip
FROM dbo.PurchaseOrders AS po
JOIN dbo.Customers AS c ON c.Id = po.CustomerId
ORDER BY po.Id;

SELECT l.PurchaseOrderId, l.Id AS LineId, p.ProductType, l.ProductName, l.Price
FROM dbo.PurchaseOrderLines AS l
JOIN dbo.Products AS p ON p.Id = l.ProductId
ORDER BY l.PurchaseOrderId, l.Id;

SELECT s.Id AS ShippingSlipId, s.PurchaseOrderId, c.Name AS Customer, sl.ProductName
FROM dbo.ShippingSlips AS s
JOIN dbo.Customers AS c ON c.Id = s.CustomerId
JOIN dbo.ShippingSlipLines AS sl ON sl.ShippingSlipId = s.Id
ORDER BY s.Id, sl.Id;
GO
