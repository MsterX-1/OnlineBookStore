-- =====================================================

USE Bookstore;
GO

-- =====================================================
-- 1. INSERT PUBLISHERS
-- =====================================================
INSERT INTO Publisher (Name, Address, Phone) VALUES
('Penguin Random House', '1745 Broadway, New York, NY 10019', '212-782-9000'),
('HarperCollins', '195 Broadway, New York, NY 10007', '212-207-7000'),
('Simon & Schuster', '1230 Avenue of Americas, New York, NY 10020', '212-698-7000'),
('Hachette Book Group', '1290 Avenue of Americas, New York, NY 10104', '212-364-1100'),
('Macmillan Publishers', '120 Broadway, New York, NY 10271', '646-307-5151'),
('Scholastic Corporation', '557 Broadway, New York, NY 10012', '212-343-6100'),
('Vintage Books', '1745 Broadway, New York, NY 10019', '212-572-2420'),
('Del Rey Books', '1745 Broadway, New York, NY 10019', '212-572-2677'),
('VIZ Media', 'PO Box 77010, San Francisco, CA 94107', '415-546-7073'),
('Kodansha Comics', '451 Park Ave S, New York, NY 10016', '212-935-6500');

-- =====================================================
-- 2. INSERT AUTHORS
-- =====================================================

INSERT INTO Author (Name) VALUES
('Kentaro Miura'),
('Jenna Wood'),
('George Orwell'),
('Makoto Shinkai'),
('Wee'),
('Christopher Nolan'),
('Bret Easton Ellis'),
('Roderick Thorp'),
('Makoto Yukimura'),
('Benedict Cumberbatch'),
('Martin Freeman'),
('J.R.R. Tolkien'),
('Bob Kane'),
('Bill Finger'),
('Stella Gibbons'),
('Jessie Ann Foley'),
('Reginald Rose'),
('Philip K. Dick'),
('Matthew Weiner'),
('Mario Puzo'),
('Brad Bird'),
('Hayao Miyazaki'),
('Mimidaisy'),
('Natalie Portman'),
('F. Scott Fitzgerald'),
('Herman Melville'),
('Damien Chazelle'),
('Sergei Prokofiev'),
('Rachel Gillig'),
('Aiden Thomas'),
('Christopher Nolan'),
('Brothers Grimm');

-- =====================================================
-- 3. INSERT BOOKS 
-- =====================================================

INSERT INTO Book (ISBN, Title, Description, Pub_Year, Price, Category, Stock_Qty, Threshold, Publisher_ID, BookPhoto) VALUES
('978-1-59307-020-5', 'Berserk Vol. 1', 'A dark medieval fantasy following the journey of Guts, a lone mercenary warrior, in a world of violence and supernatural horror.', 2003, 14.99, 'Art', 45, 10, 9, NULL),
('978-0-06-234567-8', 'Black Hearts', 'A dark fantasy tale of wolves, magic, and survival in a mysterious woodland realm.', 2020, 16.99, 'Art', 32, 10, 2, NULL),
('978-0-452-28423-4', '1984', 'A dystopian social science fiction novel depicting a totalitarian society under constant surveillance.', 1949, 13.99, 'History', 67, 10, 1, NULL),
('978-1-97467-890-1', 'Suzume', 'A beautiful story of a young girl who encounters mysterious doors and embarks on a journey across Japan.', 2022, 18.99, 'Art', 28, 10, 10, NULL),
('978-0-99876-543-2', 'Late Night Thoughts', 'A collection of reflective poetry and musings for the quiet hours of contemplation.', 2019, 12.99, 'Art', 41, 10, 3, NULL),
('978-1-58836-234-7', 'Interstellar: The Complete Screenplay', 'The complete screenplay of the epic space exploration film about love, time, and survival.', 2014, 19.99, 'Science', 25, 10, 4, NULL),
('978-0-375-72177-6', 'American Psycho', 'A controversial novel exploring the dark mind of a wealthy New York investment banker.', 1991, 15.99, 'Art', 38, 10, 7, NULL),
('978-0-06-114355-8', 'Die Hard', 'An action-packed thriller about a cop fighting terrorists in a Los Angeles skyscraper.', 1979, 14.49, 'Art', 29, 10, 2, NULL),
('978-1-61262-456-3', 'Vinland Saga Vol. 1', 'An epic historical manga following Vikings in their quest for the legendary land of Vinland.', 2013, 19.99, 'History', 34, 10, 10, NULL),
('978-0-14-103614-4', '1984 Illustrated Edition', 'George Orwell''s classic dystopian novel with stunning illustrations exploring themes of totalitarianism.', 2020, 24.99, 'History', 22, 10, 1, NULL),
('978-1-78503-456-7', 'Sherlock: The Complete Scripts', 'The complete collection of scripts from the acclaimed BBC series starring Benedict Cumberbatch.', 2015, 29.99, 'Art', 18, 10, 5, NULL),
('978-0-544-27344-3', 'The Lord of the Rings: The Two Towers', 'The second volume of the epic fantasy trilogy following the Fellowship''s continued journey.', 1954, 18.99, 'Art', 55, 10, 8, NULL),
('978-1-4012-4567-8', 'Batman: The Dark Knight Returns', 'A groundbreaking graphic novel depicting an aged Batman returning to fight crime in Gotham.', 1986, 19.99, 'Art', 42, 10, 6, NULL),
('978-0-14-118964-5', 'Cold Comfort Farm (Vintage Classics)', 'A witty satire of rural life and romantic novels set in the English countryside.', 1932, 14.99, 'Art', 27, 10, 7, NULL),
('978-0-06-249876-5', 'Sorry For Your Loss', 'A powerful young adult novel about grief, family, and finding hope after tragedy.', 2019, 17.99, 'Art', 31, 10, 2, NULL),
('978-0-14-310478-3', '12 Angry Men', 'A tense courtroom drama exploring justice, prejudice, and the jury deliberation process.', 1954, 12.99, 'History', 36, 10, 1, NULL),
('978-0-345-40479-1', 'Do Androids Dream of Electric Sheep?', 'A science fiction novel exploring what it means to be human in a post-apocalyptic world.', 1968, 14.99, 'Science', 44, 10, 8, NULL),
('978-0-8021-4567-3', 'Mad Men: The Illustrated World', 'An illustrated companion to the acclaimed series about advertising executives in 1960s New York.', 2013, 35.99, 'Art', 15, 10, 4, NULL),
('978-0-451-20547-9', 'The Godfather', 'The epic tale of the Corleone crime family and the transformation of Michael Corleone.', 1969, 16.99, 'Art', 58, 10, 1, NULL),
('978-0-06-245678-9', 'The Iron Giant: The Complete Storybook', 'The heartwarming story of a boy who befriends a giant robot from outer space.', 1999, 14.99, 'Art', 26, 10, 2, NULL),
('978-1-4215-7890-1', 'Spirited Away: The Art of the Film', 'An exploration of the artistry behind Hayao Miyazaki''s masterpiece about a girl in a spirit world.', 2001, 29.99, 'Art', 33, 10, 9, NULL),
('978-0-98765-432-1', 'Lovers by the Sea', 'A romantic story of two people finding love and healing by the ocean.', 2021, 15.99, 'Art', 24, 10, 3, NULL),
('978-0-571-20547-8', 'Black Swan: Behind the Scenes', 'An intimate look at the making of the psychological thriller about a ballet dancer.', 2010, 24.99, 'Art', 19, 10, 4, NULL),
('978-0-7432-7356-5', 'The Great Gatsby', 'A classic American novel of love, wealth, and tragedy in the Jazz Age.', 1925, 13.99, 'History', 61, 10, 3, NULL),
('978-0-14-243724-7', 'Moby-Dick', 'Herman Melville''s epic tale of obsession and revenge on the high seas.', 1851, 15.99, 'History', 39, 10, 1, NULL),
('978-1-4555-6789-0', 'Whiplash: The Complete Screenplay', 'The intense screenplay about a young drummer and his demanding instructor.', 2014, 16.99, 'Art', 21, 10, 4, NULL),
('978-0-14-024567-8', 'Peter and the Wolf', 'The classic musical fairy tale about a brave boy and his animal friends.', 1936, 11.99, 'Art', 35, 10, 1, NULL),
('978-0-316-45678-9', 'One Dark Window', 'A fantasy tale of a maiden with dark powers in a kingdom shrouded in mystery.', 2022, 18.99, 'Art', 28, 10, 5, NULL),
('978-1-250-62345-6', 'Lost in the Never Woods', 'A reimagining of Peter Pan where Wendy returns to find missing children in the woods.', 2020, 17.99, 'Art', 30, 10, 5, NULL),
('978-0-593-23456-7', 'Inception: The Shooting Script', 'Christopher Nolan''s mind-bending screenplay about dreams within dreams.', 2010, 19.99, 'Science', 27, 10, 4, NULL),
('978-0-7636-5678-9', 'Snow White and the Seven Dwarfs: The Classic Tale', 'The beloved fairy tale of a princess and seven dwarfs in an enchanted forest.', 2015, 12.99, 'Art', 43, 10, 6, NULL),
('978-0-544-27343-6', 'The Hobbit', 'The prequel to The Lord of the Rings, following Bilbo Baggins on an unexpected journey.', 1937, 16.99, 'Art', 52, 10, 8, NULL);

-- =====================================================
-- 4. INSERT BOOK_AUTHORS (Many-to-Many Relationships)
-- =====================================================

INSERT INTO Book_Author (ISBN, Author_ID) VALUES
('978-1-59307-020-5', 1),   -- Berserk - Kentaro Miura
('978-0-06-234567-8', 2),   -- Black Hearts - Jenna Wood
('978-0-452-28423-4', 3),   -- 1984 - George Orwell
('978-1-97467-890-1', 4),   -- Suzume - Makoto Shinkai
('978-0-99876-543-2', 5),   -- Late Night Thoughts - Wee
('978-1-58836-234-7', 6),   -- Interstellar - Christopher Nolan
('978-1-58836-234-7', 31),  -- Interstellar - Also Christopher Nolan (same person, ID 31)
('978-0-375-72177-6', 7),   -- American Psycho - Bret Easton Ellis
('978-0-06-114355-8', 8),   -- Die Hard - Roderick Thorp
('978-1-61262-456-3', 9),   -- Vinland Saga - Makoto Yukimura
('978-0-14-103614-4', 3),   -- 1984 Illustrated - George Orwell
('978-1-78503-456-7', 10),  -- Sherlock - Benedict Cumberbatch
('978-1-78503-456-7', 11),  -- Sherlock - Martin Freeman (MULTIPLE AUTHORS)
('978-0-544-27344-3', 12),  -- LOTR Two Towers - J.R.R. Tolkien
('978-1-4012-4567-8', 13),  -- Batman - Bob Kane
('978-1-4012-4567-8', 14),  -- Batman - Bill Finger (MULTIPLE AUTHORS)
('978-0-14-118964-5', 15),  -- Starlight - Stella Gibbons
('978-0-06-249876-5', 16),  -- Sorry For Your Loss - Jessie Ann Foley
('978-0-14-310478-3', 17),  -- 12 Angry Men - Reginald Rose
('978-0-345-40479-1', 18),  -- Do Androids Dream - Philip K. Dick
('978-0-8021-4567-3', 19),  -- Mad Men - Matthew Weiner
('978-0-451-20547-9', 20),  -- The Godfather - Mario Puzo
('978-0-06-245678-9', 21),  -- The Iron Giant - Brad Bird
('978-1-4215-7890-1', 22),  -- Spirited Away - Hayao Miyazaki
('978-0-98765-432-1', 23),  -- Lovers by the Sea - Mimidaisy
('978-0-571-20547-8', 24),  -- Black Swan - Natalie Portman
('978-0-7432-7356-5', 25),  -- The Great Gatsby - F. Scott Fitzgerald
('978-0-14-243724-7', 26),  -- Moby-Dick - Herman Melville
('978-1-4555-6789-0', 27),  -- Whiplash - Damien Chazelle
('978-0-14-024567-8', 28),  -- Peter and the Wolf - Sergei Prokofiev
('978-0-316-45678-9', 29),  -- One Dark Window - Rachel Gillig
('978-1-250-62345-6', 30),  -- Lost in the Never Woods - Aiden Thomas
('978-0-593-23456-7', 31),  -- Inception - Christopher Nolan
('978-0-7636-5678-9', 32),  -- Snow White - Brothers Grimm
('978-0-544-27343-6', 12);  -- The Hobbit - J.R.R. Tolkien

-- =====================================================
-- 5. INSERT USERS (Admins and Customers)
-- =====================================================

INSERT INTO Users (Username, Password, First_Name, Last_Name, Email, Phone, Address, Role) VALUES
-- Admins
('admin', 'admin123', 'John', 'Smith', 'admin@bookstore.com', '555-0100', '123 Admin St, New York, NY 10001', 'Admin'),
('manager1', 'pass123', 'Sarah', 'Johnson', 'sarah.j@bookstore.com', '555-0101', '456 Manager Ave, New York, NY 10002', 'Admin'),

-- Customers
('jdoe', 'password1', 'John', 'Doe', 'john.doe@email.com', '555-0201', '789 Oak Street, Brooklyn, NY 11201', 'Customer'),
('mjane', 'password2', 'Mary', 'Jane', 'mary.jane@email.com', '555-0202', '321 Elm Street, Queens, NY 11354', 'Customer'),
('bwayne', 'password3', 'Bruce', 'Wayne', 'bruce.wayne@email.com', '555-0203', '1007 Mountain Drive, Manhattan, NY 10001', 'Customer'),
('tstark', 'password4', 'Tony', 'Stark', 'tony.stark@email.com', '555-0204', '10880 Malibu Point, Staten Island, NY 10301', 'Customer'),
('pparker', 'password5', 'Peter', 'Parker', 'peter.parker@email.com', '555-0205', '20 Ingram Street, Queens, NY 11375', 'Customer'),
('dprince', 'password6', 'Diana', 'Prince', 'diana.prince@email.com', '555-0206', '555 Embassy Row, Manhattan, NY 10022', 'Customer'),
('ckent', 'password7', 'Clark', 'Kent', 'clark.kent@email.com', '555-0207', '344 Clinton Street, Bronx, NY 10451', 'Customer'),
('nromanoff', 'password8', 'Natasha', 'Romanoff', 'natasha.r@email.com', '555-0208', '890 Shield Blvd, Brooklyn, NY 11217', 'Customer'),
('srogers', 'password9', 'Steve', 'Rogers', 'steve.rogers@email.com', '555-0209', '569 Leaman Place, Brooklyn, NY 11201', 'Customer'),
('bbanner', 'password10', 'Bruce', 'Banner', 'bruce.banner@email.com', '555-0210', '123 Science Way, Manhattan, NY 10016', 'Customer');

-- =====================================================
-- 6. INSERT PAST ORDERS
-- =====================================================

INSERT INTO Customer_Order (Customer_ID, Order_Date, Total_Amount, CC_Number, CC_Expiry) VALUES
(3, '2024-11-15 10:30:00', 39.98, '4111111111111111', '2026-12-31'),
(4, '2024-11-18 14:22:00', 29.99, '4111111111111112', '2027-06-30'),
(5, '2024-11-20 09:15:00', 49.97, '4111111111111113', '2026-09-30'),
(6, '2024-11-22 16:45:00', 19.99, '4111111111111114', '2027-03-31'),
(7, '2024-11-25 11:20:00', 48.97, '4111111111111115', '2026-11-30'),
(8, '2024-12-01 13:10:00', 26.98, '4111111111111116', '2027-08-31'),
(9, '2024-12-05 15:30:00', 14.99, '4111111111111117', '2026-10-31'),
(10, '2024-12-08 10:00:00', 76.97, '4111111111111118', '2027-05-31'),
(11, '2024-12-12 12:45:00', 35.98, '4111111111111119', '2026-07-31'),
(12, '2024-12-15 14:20:00', 52.97, '4111111111111120', '2027-02-28');

-- =====================================================
-- 7. INSERT ORDER_ITEMS (Testing stock deduction trigger)
-- =====================================================

INSERT INTO Order_Items (Order_ID, ISBN, Quantity, Unit_Price) VALUES
-- Order 1 (John Doe)
(1, '978-0-14-103614-4', 1, 24.99),
(1, '978-0-06-245678-9', 1, 14.99),

-- Order 2 (Mary Jane)
(2, '978-1-4215-7890-1', 1, 29.99),

-- Order 3 (Bruce Wayne)
(3, '978-1-4012-4567-8', 1, 19.99),
(3, '978-0-451-20547-9', 1, 16.99),
(3, '978-0-99876-543-2', 1, 12.99),

-- Order 4 (Tony Stark)
(4, '978-1-58836-234-7', 1, 19.99),

-- Order 5 (Peter Parker)
(5, '978-0-06-234567-8', 2, 16.99),
(5, '978-1-59307-020-5', 1, 14.99),

-- Order 6 (Diana Prince)
(6, '978-0-7432-7356-5', 1, 13.99),
(6, '978-0-14-310478-3', 1, 12.99),

-- Order 7 (Clark Kent)
(7, '978-0-345-40479-1', 1, 14.99),

-- Order 8 (Natasha)
(8, '978-0-375-72177-6', 1, 15.99),
(8, '978-0-571-20547-8', 1, 24.99),
(8, '978-0-8021-4567-3', 1, 35.99),

-- Order 9 (Steve Rogers)
(9, '978-0-544-27344-3', 1, 18.99),
(9, '978-0-544-27343-6', 1, 16.99),

-- Order 10 (Bruce Banner)
(10, '978-1-61262-456-3', 1, 19.99),
(10, '978-0-593-23456-7', 1, 19.99),
(10, '978-0-99876-543-2', 1, 12.99);

-- =====================================================
-- 8. INSERT SHOPPING CARTS (Current Active Carts)
-- Testing cart functionality
-- =====================================================

INSERT INTO Shopping_Cart (Customer_ID, ISBN, Quantity) VALUES
(3, '978-0-452-28423-4', 1),      -- John Doe: 1984
(3, '978-0-544-27344-3', 1),      -- John Doe: LOTR Two Towers
(4, '978-1-4215-7890-1', 1),      -- Mary Jane: Spirited Away
(4, '978-1-97467-890-1', 2),      -- Mary Jane: Suzume (x2)
(5, '978-1-4012-4567-8', 1),      -- Bruce Wayne: Batman
(6, '978-1-58836-234-7', 1),      -- Tony Stark: Interstellar
(7, '978-0-06-234567-8', 1),      -- Peter Parker: Black Hearts
(8, '978-0-7432-7356-5', 1),      -- Diana Prince: The Great Gatsby
(9, '978-0-345-40479-1', 1),      -- Clark Kent: Do Androids Dream
(10, '978-0-375-72177-6', 1);    -- Natasha: American Psycho
-- =====================================================
-- 9. INSERT PUBLISHER_ORDERS (Testing restock trigger)
-- Some pending, some confirmed
-- =====================================================
INSERT INTO Publisher_Order (ISBN, Quantity, Order_Date, Status) VALUES
('978-1-4215-7890-1', 50, '2024-12-01 08:00:00', 'Confirmed'),
('978-0-571-20547-8', 50, '2024-12-05 09:30:00', 'Confirmed'),
('978-0-8021-4567-3', 50, '2024-12-10 10:15:00', 'Pending'),
('978-1-58836-234-7', 50, '2024-12-12 11:00:00', 'Pending'),
('978-0-593-23456-7', 50, '2024-12-15 14:30:00', 'Pending');
