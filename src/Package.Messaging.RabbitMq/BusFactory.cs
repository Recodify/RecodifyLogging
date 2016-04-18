using System;
using EasyNetQ;
using EasyNetQ.Loggers;

namespace Recodify.Messaging.RabbitMq
{
    public interface IBusFactory
    {
        IBus CreateBus(Settings settings);
    }

    public class BusFactory : IBusFactory
    {
        private static IBus bus;
        private static readonly object Synchronise;

        static BusFactory()
        {
            Synchronise = new object();
        }

        #region IBusFactory Members

        public virtual IBus CreateBus(Settings settings)
        {
            if (bus == null || !bus.IsConnected)
            {
                lock (Synchronise)
                    bus = RabbitHutch.CreateBus(settings.ConnectionString, (x) => x.Register<IEasyNetQLogger>(y => new NullLogger()));

                AppDomain.CurrentDomain.DomainUnload +=
                    (sender, args) =>
                        {
                            if (bus != null)
                            {
                                bus.Dispose();
                                bus = null;
                            }
                        };
            }

            return bus;
        }

        #endregion
    }
}