using Application.Dtos.ReportDto;
using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ReportService
    {
        private readonly IReportRepository _reportRepo;

        public ReportService(IReportRepository reportRepo)
        {
            _reportRepo = reportRepo;
        }
        // All the report related business logic will be implemented here
        public async Task<TotalSalesDto> GetTotalSalesForPreviousMonthAsync()
        {
            var result = await _reportRepo.GetTotalSalesForPreviousMonthAsync();
            if (result.TotalOrders == 0)
                throw new Exception("No sales found for the previous month.");
            return result;
        }

        public async Task<TotalSalesDto> GetTotalSalesForSpecificDateAsync(DateTime date)
        {
            var result = await _reportRepo.GetTotalSalesForSpecificDateAsync(date);
            if (result.TotalOrders == 0)
                throw new Exception($"No sales found for date {date.ToShortDateString()}.");
            return result;
        }

        public async Task<IEnumerable<TopCustomerDto>> GetTop5CustomersLast3MonthsAsync()
        {
            var customers = await _reportRepo.GetTop5CustomersLast3MonthsAsync();
            if (!customers.Any())
                throw new Exception("No customer purchase data found for the last 3 months.");
            return customers;
        }

        public async Task<IEnumerable<TopSellingBookDto>> GetTop10SellingBooksLast3MonthsAsync()
        {
            var books = await _reportRepo.GetTop10SellingBooksLast3MonthsAsync();
            if (!books.Any())
                throw new Exception("No book sales data found for the last 3 months.");
            return books;
        }

        public async Task<BookOrderCountDto> GetBookOrderCountAsync(string isbn)
        {
            var result = await _reportRepo.GetBookOrderCountAsync(isbn);
            if (result == null)
                throw new Exception($"Book with ISBN {isbn} not found.");
            return result;
        }
    }
}
