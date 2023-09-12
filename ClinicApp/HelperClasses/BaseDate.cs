using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.HelperClasses
{
    class BaseDate
    {
        public static readonly DateTime BaseDateConst = new DateTime(1753, 1, 1); // Minimal date for MS SQL Server
    }
}
