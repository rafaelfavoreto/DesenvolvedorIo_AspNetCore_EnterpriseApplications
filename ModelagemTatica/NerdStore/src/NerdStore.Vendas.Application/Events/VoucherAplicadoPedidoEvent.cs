using NerdStore.Core.Messagens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdStore.Vendas.Application.Events
{
    public class VoucherAplicadoPedidoEvent : Event
    {
        public Guid ClienteId { get; private set; }
        public Guid PedidoId { get; private set; }
        public Guid VoucherId { get; private set; }

        public VoucherAplicadoPedidoEvent(Guid clienteId, Guid pedidoId, Guid voucherId)
        {
            ClienteId = clienteId;
            PedidoId = pedidoId;
            VoucherId = voucherId;
        }
    }
}
