using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reporting.Pages.Exceptions
{
    public class ReportNameIsNull : Exception
    {
        public ReportNameIsNull() : base("Report name is null or empty.")
        {
        }
    }
}
