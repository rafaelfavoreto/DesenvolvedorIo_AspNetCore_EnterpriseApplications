using NerdStore.Core.Messagens;
using NerdStore.Core.Messagens.CommonMessages.DomainEvents;
using NerdStore.Core.Messagens.CommonMessages.Notifications;

namespace NerdStore.Core.Communication.Mediator
{
    public interface IMediatrHandler
    {
        Task PublicarEvento<T>(T evento) where T : Event;
        Task<bool> EnviarComando<T>(T comando) where T : Command;
        Task PublicarNotificacao<T>(T notificacao) where T : DomainNotification;
        Task PublicarDomainEvent<T>(T notificacao) where T : DomainEvent;
    }
}