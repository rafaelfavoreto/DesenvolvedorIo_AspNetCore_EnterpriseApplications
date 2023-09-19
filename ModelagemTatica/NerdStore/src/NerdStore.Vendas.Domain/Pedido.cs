using FluentValidation.Results;
using NerdStore.Core.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdStore.Vendas.Domain
{
    public class Pedido : Entity, IAggregateRoot
    {
        public int Codigo { get; private set; }
        public Guid ClienteId { get; private set; }
        public Guid? VoucherId { get; private set; }
        public bool VoucherUtilizado { get; private set; }
        public decimal Desconto { get; private set; }
        public decimal ValorTotal { get; private set; }
        public DateTime DataCadastro { get; private set; }
        public PedidoStatus PedidoStatus { get; private set; }

        private readonly List<PedidoItem> _pedidosItems;
        public IReadOnlyCollection<PedidoItem> PedidoItems => _pedidosItems;

        //EF Rel.
        public Voucher Voucher { get; private set; }

        public Pedido(Guid clienteId,bool voucherUtilizado,decimal desconto,decimal valorTotal)
        {
            ClienteId = clienteId;
            VoucherUtilizado = voucherUtilizado;
            Desconto = desconto;
            ValorTotal = valorTotal;
            _pedidosItems = new List<PedidoItem>();
        }

        protected Pedido()
        {
            _pedidosItems = new List<PedidoItem>();
        }

        public ValidationResult AplicarVouchger(Voucher voucher)
        {
            var validationResult = voucher.ValidarSeAplicavel();
            if (!validationResult.IsValid) return validationResult;

            Voucher = voucher;
            VoucherUtilizado = true;
            CalcularValorPedido();

            return validationResult;

        }

        public void CalcularValorPedido()
        {
            ValorTotal = PedidoItems.Sum(p => p.CalcularValor());
            CalcularValorTotalDesconto();
        }

        private void CalcularValorTotalDesconto()
        {
            if (!VoucherUtilizado) return;

            decimal desconto = 0;
            var valor = ValorTotal;

            if(Voucher.TipoDescontoVoucher == TipoDescontoVoucher.Porcentagem)
            {
                if (Voucher.Percentual.HasValue)
                {
                    desconto = (valor * Voucher.Percentual.Value) / 100;
                    valor -= desconto;
                }
            }
            else
            {
                if (Voucher.ValorDesconto.HasValue)
                {
                    desconto = Voucher.ValorDesconto.Value;
                    valor -= desconto;
                }
            }

            ValorTotal = valor < 0 ? 0 : valor;
            Desconto = desconto;

        }

        public bool PedidoItemExistente(PedidoItem item)
        {
            return _pedidosItems.Any(p => p.ProdutoId == item.ProdutoId);
        }

        public void AdicionarItem(PedidoItem item)
        {
            if (!item.EhValido()) return;

            item.AssociarPedido(Id);

            if (PedidoItemExistente(item))
            {
                var itemExistente = _pedidosItems.FirstOrDefault(p => p.ProdutoId == item.ProdutoId);
                itemExistente.AdicionarUnidades(item.Quantidade);
                item = itemExistente;

                _pedidosItems.Remove(itemExistente);
            }

            item.CalcularValor();
            _pedidosItems.Add(item);

            CalcularValorPedido();
        }

        public void RemoverItem(PedidoItem item)
        {
            if (!item.EhValido()) return;

            var itemExistente = PedidoItems.FirstOrDefault(p => p.ProdutoId == item.ProdutoId);

            if (itemExistente == null) throw new DomainException("O item não pertence ao pedido");
            _pedidosItems.Remove(itemExistente);

            CalcularValorPedido();
        }

        public void AtualizarItem(PedidoItem item)
        {
            if (!item.EhValido()) return;
            item.AssociarPedido(Id);

            var itemExistente = PedidoItems.FirstOrDefault(p => p.ProdutoId == item.ProdutoId);

            if (itemExistente == null) throw new DomainException("O item não pertence ao pedido");

            _pedidosItems.Remove(itemExistente);
            _pedidosItems.Add(item);

            CalcularValorPedido();
        }

        public void AtualizarUnidades(PedidoItem item, int unidades)
        {
            item.AtualizarUnidades(unidades);
            AtualizarItem(item);
        }

        public void TornarRascunho()
        {
            PedidoStatus = PedidoStatus.Rascunho;
        }

        public void IniciarPedido()
        {
            PedidoStatus = PedidoStatus.Iniciado;
        }

        public void FinalizarPedido()
        {
            PedidoStatus = PedidoStatus.Pago;
        }

        public void CancelarPedido()
        {
            PedidoStatus = PedidoStatus.Cancelado;
        }

        public static class PedidoFactory
        {
            public static Pedido NovoPedidoRascunho(Guid clienteId)
            {
                var pedido = new Pedido
                {
                    ClienteId = clienteId,
                };

                pedido.TornarRascunho();
                return pedido;
            }
        }
    }
}
