using System;
using NerdStore.Core.DomainObjects;
using NerdStore.Core.Messagens.CommonMessages.DomainEvents;


namespace NerdStore.Catalago.Domain.Events
{
    public class ProdutoAbaixoEstoqueEvent : DomainEvent
    {
        public int QuantidadeRestante { get; private set; }
        public ProdutoAbaixoEstoqueEvent(Guid aggregateId, int quantidadeRestante) : base(aggregateId)
        {            
            QuantidadeRestante = quantidadeRestante;
        }
    }
}
