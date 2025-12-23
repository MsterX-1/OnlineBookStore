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
    
        [HttpGet("GetTotalSalesForPreviousMonth")]
        public async Task<IActionResult> GetTotalSalesForPreviousMonth()
        {
            try
            {
                var report = await _reportService.GetTotalSalesForPreviousMonthAsync();
                return Ok(report);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("GetTotalSalesForDate/{date}")]
        public async Task<IActionResult> GetTotalSalesForDate(DateTime date)
        {
            try
            {
                var report = await _reportService.GetTotalSalesForDateAsync(date);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("GetTopTenBooks")]
        public async Task<IActionResult> GetTopTenBooks()
        {
            try
            {
                var report = await _reportService.GetTopTenBooksAsync();
                return Ok(report);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
