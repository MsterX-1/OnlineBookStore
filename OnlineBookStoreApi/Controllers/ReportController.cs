using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OnlineBookStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly ReportService _reportService;

        public ReportController(ReportService reportService)
        {
            _reportService = reportService;
        }
        // All Endpoints are created for Report Entity

        [HttpGet("TotalSalesPreviousMonth")]
        public async Task<IActionResult> GetTotalSalesPreviousMonth()
        {
            try
            {
                var sales = await _reportService.GetTotalSalesForPreviousMonthAsync();
                return Ok(sales);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // Specific Date to get Total Sales like yyyy-mm-dd
        [HttpGet("TotalSalesForDate")]
        public async Task<IActionResult> GetTotalSalesForDate([FromQuery] DateTime date)
        {
            try
            {
                var sales = await _reportService.GetTotalSalesForSpecificDateAsync(date);
                return Ok(sales);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("Top5Customers")]
        public async Task<IActionResult> GetTop5Customers()
        {
            try
            {
                var customers = await _reportService.GetTop5CustomersLast3MonthsAsync();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("Top10SellingBooks")]
        public async Task<IActionResult> GetTop10SellingBooks()
        {
            try
            {
                var books = await _reportService.GetTop10SellingBooksLast3MonthsAsync();
                return Ok(books);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("BookOrderCount/{isbn}")]
        public async Task<IActionResult> GetBookOrderCount(string isbn)
        {
            try
            {
                var orderCount = await _reportService.GetBookOrderCountAsync(isbn);
                return Ok(orderCount);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
