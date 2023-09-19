using NerdStore.Core.DomainObjects.Dto;
using NerdStore.Core.Messagens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdStore.Core.Messagens.CommonMessages.IntegrationEvents
{
    public class PedidoIniciadoEvent : IntegrationEvent
    {
        public Guid PedidoId { get; private set; }
        public Guid ClienteId { get; private set; }       
        public ListaProdutosPedido ProdutosPedido { get; private set; }
        public decimal Total { get; private set; }
        public string NomeCartao { get; private set; }
        public string NumeroCartao { get; private set; }
        public string ExpiracaoCartao { get; private set; }
        public string CvvCartao { get; private set; }

        public PedidoIniciadoEvent(Guid pedidoId, Guid clienteId, decimal total, ListaProdutosPedido produtosPedido, string nomeCartao, string numeroCarao, string expiracaoCartao, string cvvCartao)
        {
            PedidoId = pedidoId;
            ClienteId = clienteId;
            Total = total;
            ProdutosPedido = produtosPedido;
            NomeCartao = nomeCartao;
            NumeroCartao = numeroCarao;
            ExpiracaoCartao = expiracaoCartao;
            CvvCartao = cvvCartao;
        }
    }
}
