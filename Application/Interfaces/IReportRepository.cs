using Application.Dtos.ReportDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IReportRepository
    {
        Task<TotalSalesDto> GetTotalSalesForPreviousMonthAsync();
        Task<TotalSalesDto> GetTotalSalesForSpecificDateAsync(DateTime date);
        Task<IEnumerable<TopCustomerDto>> GetTop5CustomersLast3MonthsAsync();
        Task<IEnumerable<TopSellingBookDto>> GetTop10SellingBooksLast3MonthsAsync();
        Task<BookOrderCountDto?> GetBookOrderCountAsync(string isbn);
    }
}
