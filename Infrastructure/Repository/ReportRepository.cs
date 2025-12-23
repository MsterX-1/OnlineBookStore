using Application.Dtos.ReportDto;
using Application.Interfaces;
using Dapper;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class ReportRepository: IReportRepository
    {
        private readonly DatabaseContext _context;

        public ReportRepository(DatabaseContext context)
        {
            _context = context;
        }
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
        // Implement The Methods Defined in IReportRepository Interface
    }
}
