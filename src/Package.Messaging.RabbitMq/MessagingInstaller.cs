using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Recodify.Messaging.RabbitMq
{
    public class MessagingInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<Messaging>().LifeStyle.Singleton);
            container.Register(Component.For<Settings>().ImplementedBy<AppSettings>().LifeStyle.Singleton);
            container.Register(Component.For<Settings>().ImplementedBy<ImportDocumentAppSettings>().LifeStyle.Singleton.Named("ImportDocumentAppSettings"));
            container.Register(Component.For<IBusFactory>().ImplementedBy<BusFactory>().LifeStyle.Singleton);

            container.Register(Component.For<IPublisher>().ImplementedBy<Publisher>().LifeStyle.Transient);
            container.Register(Component.For<IPublisher>().ImplementedBy<Publisher>()
                .DependsOn(Dependency.OnComponent(typeof(Settings), "ImportDocumentAppSettings"))
                .LifeStyle.Transient.Named("ImportDocumentPublisher"));

            container.Register(Component.For<ISubscriber>().ImplementedBy<Subscriber>().LifeStyle.Transient);
            container.Register(Component.For<ISubscriber>().ImplementedBy<Subscriber>()
                .DependsOn(Dependency.OnComponent(typeof(Settings), "ImportDocumentAppSettings"))
                .LifeStyle.Transient.Named("ImportDocumentSubscriber"));

            container.Register(Component.For<IMessagingFacility>().ImplementedBy<MessagingFacility>().LifeStyle.Transient);
        }
    }
}