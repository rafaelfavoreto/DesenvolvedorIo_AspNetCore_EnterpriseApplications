using NerdStore.Core.Messagens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdStore.Vendas.Application.Events
{
    public class PedidoProdutoRemovidoEvent : Event
    {
        public Guid ClienteId { get;private set; }
        public Guid PedidoId { get;private set; }
        public Guid ProdutoId { get;private set; }

        public PedidoProdutoRemovidoEvent(Guid clienteId, Guid pedidoId, Guid produtoId)
        {
            AggregateId = pedidoId;
            ClienteId = clienteId;
            PedidoId = pedidoId;
            ProdutoId = produtoId;
        }
    }
}
