using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KP.Api.AspNetCore
{
    /// <summary>
    /// Настройки для <see cref="ApiJsonController"/>.
    /// </summary>
    public class ApiJsonControllerOptions
    {
        public Func<Exception, bool> ExceptionPredicate { get; set; }
    }
}
