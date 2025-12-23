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
        // Define method signatures for report repository operations
        Task<IEnumerable<TotalSalesDto?>> GetTotalSalesForPreviousMonthAsync();
    }
}
