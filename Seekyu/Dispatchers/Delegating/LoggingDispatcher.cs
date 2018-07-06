using Seekyu.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seekyu.Dispatchers.Delegating
{
    public class LoggingDispatcher<TDispatchable> : DelegatingDispatcher<TDispatchable>
        where TDispatchable : IDispatchable
    {
        private readonly IObjectSerializer Serializer;
        private readonly ILogger Logger;


        public LoggingDispatcher(IObjectSerializer serializer, ILogger logger)
        {
            Serializer = serializer;
            Logger = logger;
        }

        public override TResult Dispatch<TResult>(TDispatchable dispatchable)
        {
            try
            {
                string formattedCommand = Serializer.Serialize(dispatchable);
                Logger.LogQuery(formattedCommand);

                TResult response = Next.Dispatch<TResult>(dispatchable);

                string formattedResponse = Serializer.Serialize(response);
                Logger.LogResponse(formattedResponse);
                return response;
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                throw;
            }
        }
    }
}
