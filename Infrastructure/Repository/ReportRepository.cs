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
        public async Task<IEnumerable<TotalSalesDto?>> GetTotalSalesForPreviousMonthAsync()
        {
            using var db = _context.CreateConnection();
            var sql = @"SELECT
    ISNULL(SUM(oi.Quantity * oi.Unit_Price), 0) AS totalSales,
    COUNT(DISTINCT co.Order_ID)         AS totalOrders,
    ISNULL(SUM(oi.Quantity), 0)        AS totalItemsSold
FROM Customer_Order co
INNER JOIN Order_Items oi
    ON co.Order_ID = oi.Order_ID
WHERE
    co.Order_Date >= DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) - 1, 0)
    AND co.Order_Date <  DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0);
";
            return await db.QueryAsync<TotalSalesDto>(sql);

        }
        public async Task<IEnumerable<TotalSalesForDateDto?>> GetTotalSalesForDateAsync(DateTime date)
        {

                      using var db = _context.CreateConnection();
            var sql = @"
            SELECT
    ISNULL(SUM(OI.Quantity * OI.Unit_Price), 0) AS TotalSales,
    COUNT(DISTINCT CO.Order_ID)                 AS TotalOrders,
    ISNULL(SUM(OI.Quantity), 0)                 AS TotalItemsSold
FROM Customer_Order CO
JOIN Order_Items OI
    ON CO.Order_ID = OI.Order_ID
WHERE
    CO.Order_Date >= @date
    AND CO.Order_Date < DATEADD(DAY, 1, @date)";
            return await db.QueryAsync<TotalSalesForDateDto>(sql, new { date = date.Date });
        }
        public async Task<IEnumerable<TopTenBooks?>> GetTopTenBooksAsync()
        {
            using var db = _context.CreateConnection();
            var sql = @"
           SELECT TOP 10
    B.ISBN,
    B.Title,
    SUM(OI.Quantity) AS TotalSold,
    SUM(OI.Quantity * OI.Unit_Price) AS TotalSales
FROM Order_Items OI
JOIN Customer_Order CO ON OI.Order_ID = CO.Order_ID
JOIN Book B ON OI.ISBN = B.ISBN
WHERE CO.Order_Date >=DATEADD(MONTH, -3, GETDATE())
GROUP BY B.ISBN, B.Title
ORDER BY TotalSold DESC, TotalSales DESC";
            return await db.QueryAsync<TopTenBooks>(sql);

        }
    }
}
