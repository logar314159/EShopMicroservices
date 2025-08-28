using MediatR;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalCommon.Behaviors
{
    public class LogginBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
        where TResponse : notnull
    {
        private readonly ILogger<LogginBehavior<TRequest, TResponse>> logger;

        public LogginBehavior(ILogger<LogginBehavior<TRequest, TResponse>> logger)
        {
            this.logger = logger;
        }

        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            logger.LogInformation($"[START] Handle request:{typeof(TRequest).Name}, Response: {typeof(TResponse).Name}, Request data: {request}");

            var timer = new Stopwatch();
            timer.Start();

            var response = next();

            timer.Stop();
            if(timer.Elapsed.TotalMilliseconds > 3000)
            {
                logger.LogWarning($"[PERFORMANCE] the request{typeof(TRequest).Name} took {timer.ElapsedMilliseconds} ms");
            }

            logger.LogInformation($"[END] Handled request:{typeof(TRequest).Name}, with Response: {typeof(TResponse).Name}");

            return response;
        }
    }
}
