using Application.Dtos.ReportDto;
using Application.Interfaces;
using Dapper;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Infrastructure.Repository
{
    public class ReportRepository: IReportRepository
    {
        private readonly DatabaseContext _context;

        public ReportRepository(DatabaseContext context)
        {
            _context = context;
        }
        // Implement The Methods Defined in IReportRepository Interface
        public async Task<TotalSalesDto> GetTotalSalesForPreviousMonthAsync()
        {
            using var db = _context.CreateConnection();

            var sql = @"
                SELECT 
                ISNULL(SUM(oi.Quantity * oi.Unit_Price), 0) AS TotalSales,
                COUNT(DISTINCT co.Order_ID) AS TotalOrders,
                ISNULL(SUM(oi.Quantity), 0) AS TotalItemsSold
                    FROM Customer_Order co
                    JOIN Order_Items oi ON co.Order_ID = oi.Order_ID
                    WHERE co.Order_Date >= DATEADD(DAY, -30, GETDATE());";
            // we get data 30 days before now

            return await db.QueryFirstOrDefaultAsync<TotalSalesDto>(sql)
                   ?? new TotalSalesDto { TotalSales = 0, TotalOrders = 0, TotalItemsSold = 0 };
        }

        public async Task<TotalSalesDto> GetTotalSalesForSpecificDateAsync(DateTime date)
        {
            using var db = _context.CreateConnection();

            var sql = @"
                SELECT 
                ISNULL(SUM(oi.Quantity * oi.Unit_Price), 0) AS TotalSales,
                COUNT(DISTINCT co.Order_ID) AS TotalOrders,
                ISNULL(SUM(oi.Quantity), 0) AS TotalItemsSold
                FROM Customer_Order co
                JOIN Order_Items oi ON co.Order_ID = oi.Order_ID
                WHERE co.Order_Date >= @Date
                  AND co.Order_Date < DATEADD(DAY, 1, @Date)";
            // we get data for the specific date by filtering orders between the start of that date and the start of the next day

            return await db.QueryFirstOrDefaultAsync<TotalSalesDto>(sql, new { Date = date.Date })
                   ?? new TotalSalesDto { TotalSales = 0, TotalOrders = 0, TotalItemsSold = 0 };
        }

        public async Task<IEnumerable<TopCustomerDto>> GetTop5CustomersLast3MonthsAsync()
        {
            using var db = _context.CreateConnection();

            var sql = @"
                SELECT TOP 5
                u.User_ID AS CustomerId,
                CONCAT(u.First_Name, ' ', u.Last_Name) AS CustomerName,
                u.Email,
                ISNULL(SUM(co.Total_Amount), 0) AS TotalPurchaseAmount,
                COUNT(co.Order_ID) AS TotalOrders
                    FROM Users u
                    LEFT JOIN Customer_Order co 
                        ON u.User_ID = co.Customer_ID
                        AND co.Order_Date >= DATEADD(DAY, -90, GETDATE())
                    WHERE u.Role = 'Customer'
                    GROUP BY u.User_ID, u.First_Name, u.Last_Name, u.Email
                    HAVING SUM(co.Total_Amount) > 0
                    ORDER BY TotalPurchaseAmount DESC;";

            return await db.QueryAsync<TopCustomerDto>(sql);
        }

        public async Task<IEnumerable<TopSellingBookDto>> GetTop10SellingBooksLast3MonthsAsync()
        {
            using var db = _context.CreateConnection();

            var sql = @"
                SELECT TOP 10
                b.ISBN,
                b.Title,
                b.Category,
                ISNULL(SUM(oi.Quantity), 0) AS TotalCopiesSold,
                ISNULL(SUM(oi.Quantity * oi.Unit_Price), 0) AS TotalRevenue
                    FROM Book b
                    LEFT JOIN Order_Items oi ON b.ISBN = oi.ISBN
                    LEFT JOIN Customer_Order co ON oi.Order_ID = co.Order_ID
                    WHERE co.Order_Date >= DATEADD(DAY, -90, GETDATE())
                    GROUP BY b.ISBN, b.Title, b.Category
                    HAVING SUM(oi.Quantity) > 0
                    ORDER BY TotalCopiesSold DESC;";

            return await db.QueryAsync<TopSellingBookDto>(sql);
        }

        public async Task<BookOrderCountDto?> GetBookOrderCountAsync(string isbn)
        {
            using var db = _context.CreateConnection();

            var sql = @"
                SELECT 
                    b.ISBN,
                    b.Title,
                    COUNT(po.Pub_Order_ID) AS TotalTimesOrdered,
                    ISNULL(SUM(po.Quantity), 0) AS TotalQuantityOrdered,
                    SUM(CASE WHEN po.Status = 'Pending' THEN 1 ELSE 0 END) AS PendingOrders,
                    SUM(CASE WHEN po.Status = 'Confirmed' THEN 1 ELSE 0 END) AS ConfirmedOrders
                FROM Book b
                LEFT JOIN Publisher_Order po ON b.ISBN = po.ISBN
                WHERE b.ISBN = @ISBN
                GROUP BY b.ISBN, b.Title";

            return await db.QueryFirstOrDefaultAsync<BookOrderCountDto>(sql, new { ISBN = isbn });
        }
    }
}
