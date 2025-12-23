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
        public async Task<IEnumerable<TotalSalesDto?>> GetTotalSalesForPreviousMonthAsync()
        {
                        var report =  await _reportRepo.GetTotalSalesForPreviousMonthAsync();
            if (report == null || !report.Any())
                throw new Exception("No report data found.");
            return report;
        }
    }
}
