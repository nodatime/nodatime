using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace NodaTime.Web.Models
{
    public class TimerCache<T>
    {
        private readonly Timer timer;
        private readonly Func<T> provider;
        private readonly ILogger logger;
        private readonly object padlock = new object();
        private T value;
        // TODO: Use Interlocked or similar to ensure proper volatility.
        // It's possible that using volatile would be enough...
        public T Value
        {
            get
            {
                lock (padlock)
                {
                    return value;
                }
            }
            set
            {
                lock (padlock)
                {
                    this.value = value;
                }
            }
        }

        public TimerCache(IApplicationLifetime lifetime, Duration refreshPeriod, Func<T> provider, ILoggerFactory loggerFactory)
        {
            lifetime.ApplicationStopping.Register(() => timer?.Dispose());
            logger = loggerFactory.CreateLogger(typeof(TimerCache<T>));
            this.provider = provider;
            // Due time of zero means "immediately"
            timer = new Timer(Fetch, state: null, dueTime: TimeSpan.Zero, period: refreshPeriod.ToTimeSpan());
        }

        private void Fetch(object state)
        {
            try
            {
                logger.LogInformation($"Refreshing cache for {typeof(T)}");
                Value = provider();
                logger.LogInformation($"Cache refresh complete for {typeof(T)}");
            }
            catch (Exception e)
            {
                logger.LogError(0, e, $"Error fetching {typeof(T)}");
            }
        }
    }
}
