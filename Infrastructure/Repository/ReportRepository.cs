using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Infrastructure.Data;

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
    }
}
