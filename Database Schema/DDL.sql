Create Database Bookstore
use Bookstore;
go
-- 1. PUBLISHERS 
CREATE TABLE Publisher (
    Publisher_ID INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Address VARCHAR(255),
    Phone VARCHAR(20)
);

-- 2. AUTHORS 
CREATE TABLE Author (
    Author_ID INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL
);

-- 3. BOOKS 
CREATE TABLE Book (
    ISBN VARCHAR(20) PRIMARY KEY,
    Title VARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    Pub_Year INT,
    Price DECIMAL(10, 2) NOT NULL,
    Category VARCHAR(50) CHECK (Category IN ('Science', 'Art', 'Religion', 'History', 'Geography')), 
    Stock_Qty INT NOT NULL DEFAULT 0,
    Threshold INT NOT NULL DEFAULT 10,
    Publisher_ID INT NOT NULL,
    BookPhoto VARBINARY(MAX),
    FOREIGN KEY (Publisher_ID) REFERENCES Publisher(Publisher_ID)
);

-- 4. BOOK_AUTHORS (Many-to-Many) 
CREATE TABLE Book_Author (
    ISBN VARCHAR(20),
    Author_ID INT,
    PRIMARY KEY (ISBN, Author_ID),
    FOREIGN KEY (ISBN) REFERENCES Book(ISBN) ON DELETE CASCADE,
    FOREIGN KEY (Author_ID) REFERENCES Author(Author_ID)
);

-- 5. USERS 
CREATE TABLE Users (
    User_ID INT IDENTITY(1,1) PRIMARY KEY,
    Username VARCHAR(50) UNIQUE NOT NULL,
    Password VARCHAR(50) NOT NULL,
    First_Name VARCHAR(50) NOT NULL,
    Last_Name VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Phone VARCHAR(20),
    Address VARCHAR(255),
    Role VARCHAR(10) DEFAULT 'Customer' CHECK (Role IN ('Admin', 'Customer'))
);

-- 6. SHOPPING_CART 
CREATE TABLE Shopping_Cart (
    Cart_ID INT IDENTITY(1,1) PRIMARY KEY,
    Customer_ID INT NOT NULL,
    ISBN VARCHAR(20) NOT NULL,
    Quantity INT NOT NULL,
    FOREIGN KEY (Customer_ID) REFERENCES Users(User_ID),
    FOREIGN KEY (ISBN) REFERENCES Book(ISBN) ON DELETE CASCADE
);

-- 7. CUSTOMER_ORDER 
CREATE TABLE Customer_Order (
    Order_ID INT IDENTITY(1,1) PRIMARY KEY,
    Customer_ID INT NOT NULL,
    Order_Date DATETIME DEFAULT GETDATE(),
    Total_Amount DECIMAL(10, 2) NOT NULL,
    CC_Number VARCHAR(20) NOT NULL,
    CC_Expiry DATE NOT NULL,
    FOREIGN KEY (Customer_ID) REFERENCES Users(User_ID)
);

-- 8. ORDER_ITEMS 
CREATE TABLE Order_Items (
    Item_ID INT IDENTITY(1,1) PRIMARY KEY,
    Order_ID INT NOT NULL,
    ISBN VARCHAR(20) NOT NULL,
    Quantity INT NOT NULL,
    Unit_Price DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (Order_ID) REFERENCES Customer_Order(Order_ID),
    FOREIGN KEY (ISBN) REFERENCES Book(ISBN) ON DELETE CASCADE
);

-- 9. PUBLISHER_ORDER 
CREATE TABLE Publisher_Order (
    Pub_Order_ID INT IDENTITY(1,1) PRIMARY KEY,
    ISBN VARCHAR(20) NOT NULL,
    Quantity INT NOT NULL,
    Order_Date DATETIME DEFAULT GETDATE(),
    Status VARCHAR(20) DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Confirmed')),
    FOREIGN KEY (ISBN) REFERENCES Book(ISBN) ON DELETE CASCADE
);
-- Triggers
-- 1. Prevent Negative Stock (BEFORE UPDATE logic)
go

CREATE TRIGGER TR_Book_PreventNegativeStock
ON Book
AFTER UPDATE
AS
BEGIN
    -- Check if the update is trying to set Stock_Qty to a negative value
    IF EXISTS (SELECT 1 FROM INSERTED WHERE Stock_Qty < 0)
    BEGIN
        -- Raise an error and rollback the transaction
        RAISERROR('Error: Stock quantity cannot be negative.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO

-- 2. Auto-Place Publisher Order (AFTER UPDATE on Book)

CREATE TRIGGER TR_Book_AutoRestock
ON Book
AFTER UPDATE
AS
BEGIN
    -- Only fire if Stock_Qty was updated
    IF UPDATE(Stock_Qty)
    BEGIN
        INSERT INTO Publisher_Order (ISBN, Quantity, Status, Order_Date)
        SELECT
            I.ISBN,
            50, -- Fixed constant quantity for restocking (e.g., 50)
            'Pending',
            GETDATE()
        FROM INSERTED I
        INNER JOIN DELETED D ON I.ISBN = D.ISBN
        -- Condition: Stock dropped below the threshold and was previously above or equal
        WHERE D.Stock_Qty >= I.Threshold
          AND I.Stock_Qty < I.Threshold;
    END
END;
GO


-- 3. Confirm Publisher Order & Update Stock (AFTER UPDATE on Publisher_Order)

CREATE TRIGGER TR_PubOrder_Confirm
ON Publisher_Order
AFTER UPDATE
AS
BEGIN
    -- Check if the status changed to 'Confirmed'
    IF UPDATE(Status)
    BEGIN
        UPDATE B
        SET Stock_Qty = B.Stock_Qty + I.Quantity
        FROM Book B
        INNER JOIN INSERTED I ON B.ISBN = I.ISBN
        INNER JOIN DELETED D ON I.Pub_Order_ID = D.Pub_Order_ID
        -- Only update stock if the status changed to Confirmed from any other state
        WHERE I.Status = 'Confirmed'
          AND D.Status <> 'Confirmed';
    END
END;
GO


-- 4. Deduct Stock on Customer Sale (AFTER INSERT on Order_Items)

CREATE TRIGGER TR_OrderItems_DeductStock
ON Order_Items
AFTER INSERT
AS
BEGIN
    UPDATE B
    SET Stock_Qty = B.Stock_Qty - I.Quantity
    FROM Book B
    INNER JOIN INSERTED I ON B.ISBN = I.ISBN;
    
    -- NOTE: The 'Prevent Negative Stock' trigger (TR_Book_PreventNegativeStock)
    -- will fire automatically when the Book table is updated by this trigger. 
    -- If the deduction causes a negative value, the entire transaction will be rolled back.
END;
GO