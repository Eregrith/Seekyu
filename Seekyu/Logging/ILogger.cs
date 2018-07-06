using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seekyu.Logging
{
    public interface ILogger
    {
        void LogQuery(string message);
        void LogResponse(string message);
        void LogException(Exception exception);
    }
}
