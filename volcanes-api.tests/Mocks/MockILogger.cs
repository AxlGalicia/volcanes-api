using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace volcanes_api.tests.Mocks
{
    public class MockILogger<VolcanesController> : ILogger<VolcanesController>
    {

        private readonly List<string> logMessages = new List<string>();
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel,
                                Microsoft.Extensions.Logging.EventId eventId,
                                TState state,
                                Exception? exception,
                                Func<TState, Exception?,
                                string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                string mensaje = formatter(state, exception);
                logMessages.Add(mensaje);
                Console.WriteLine(mensaje);
            }
        }

        public List<string> GetLogMessages()
        {
            return logMessages;
        }
    }
}
