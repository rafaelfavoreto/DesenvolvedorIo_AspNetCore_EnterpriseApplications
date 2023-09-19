using MediatR;
using NerdStore.Core.Communication.Mediator;
using NerdStore.Core.DomainObjects.Dto;
using NerdStore.Core.Extensions;
using NerdStore.Core.Messagens;
using NerdStore.Core.Messagens.CommonMessages.IntegrationEvents;
using NerdStore.Core.Messagens.CommonMessages.Notifications;
using NerdStore.Vendas.Application.Events;
using NerdStore.Vendas.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdStore.Vendas.Application.Commands
{
    public class PedidoCommandHandler : 
        IRequestHandler<AdicionarItemPedidoCommand, bool>,
        IRequestHandler<AtualizarItemPedidoCommand, bool>,
        IRequestHandler<RemoverItemPedidoCommand, bool>,
        IRequestHandler<AplicarVoucherPedidoCommand, bool>,
        IRequestHandler<IniciarPedidoCommand, bool>,
        IRequestHandler<FinalizarPedidoCommand, bool>,
        IRequestHandler<CancelarProcessamentoPedidoEstornarEstoqueCommand, bool>

    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IMediatrHandler _mediatorHandler;

        public PedidoCommandHandler(IPedidoRepository pedidoRepository, IMediatrHandler mediator)
        {
            _pedidoRepository = pedidoRepository;
            _mediatorHandler = mediator;
        }

        public async Task<bool> Handle(AdicionarItemPedidoCommand message, CancellationToken cancellationToken)
        {
            if (!ValidarComando(message)) return false;

            var pedido = await _pedidoRepository.ObterPedidoRascunhoPorClienteId(message.ClienteId);
            var pedidoItem = new PedidoItem(message.ProdutoId, message.Nome, message.Quantidade, message.ValorUnitario);

            if (pedido == null)
            {
                pedido = Pedido.PedidoFactory.NovoPedidoRascunho(message.ClienteId);
                pedido.AdicionarItem(pedidoItem);

                _pedidoRepository.Adicionar(pedido);
                pedido.AdicionarEvento(new PedidoRascunhoIniciadoEvent(message.ClienteId, message.ProdutoId));
            }
            else
            {
                var pedidoItemExistente = pedido.PedidoItemExistente(pedidoItem);
                pedido.AdicionarItem(pedidoItem);

                if (pedidoItemExistente)
                {
                    _pedidoRepository.AtualizarItem(pedido.PedidoItems.FirstOrDefault(p => p.ProdutoId == pedidoItem.ProdutoId));
                }
                else
                {
                    _pedidoRepository.AdicionarItem(pedidoItem);
                }                             
            }

            pedido.AdicionarEvento(new PedidoItemAdicionadoEvent(pedido.ClienteId, pedido.Id, message.ProdutoId,message.Nome, message.ValorUnitario, message.Quantidade));
            return await _pedidoRepository.UnitOfWork.Commit();
        }

        public async Task<bool> Handle(RemoverItemPedidoCommand message, CancellationToken cancellationToken)
        {
            var pedido = await  _pedidoRepository.ObterPedidoRascunhoPorClienteId(message.ClienteId);

            if(pedido == null)
            {
                await _mediatorHandler.PublicarNotificacao(new DomainNotification("pedido", "Pedido não encontrado!"));
                return false;
            }

            var pedidoItem = await _pedidoRepository.ObterItemPorPedido(pedido.Id,message.ProdutoId);

            if(pedidoItem != null && !pedido.PedidoItemExistente(pedidoItem))
            {
                await _mediatorHandler.PublicarNotificacao(new DomainNotification("pedido", "Item do pedido não encontrado!"));
                return false;
            }

            pedido.RemoverItem(pedidoItem);         
            pedido.AdicionarEvento(new PedidoProdutoRemovidoEvent(message.ClienteId, pedido.Id, message.ProdutoId));            

            _pedidoRepository.RemoverItem(pedidoItem);
            _pedidoRepository.Atualizar(pedido);

            return await _pedidoRepository.UnitOfWork.Commit();
        }

        public async Task<bool> Handle(AtualizarItemPedidoCommand message, CancellationToken cancellationToken)
        {
            if(!ValidarComando(message)) return false;

            var pedido = await _pedidoRepository.ObterPedidoRascunhoPorClienteId(message.ClienteId);

            if(pedido == null)
            {
                await _mediatorHandler.PublicarNotificacao(new DomainNotification("pedido", "Pedido não encotrado!"));
                return false;
            }

            var pedidoItem = await _pedidoRepository.ObterItemPorPedido(pedido.Id, message.ProdutoId);

            if (!pedido.PedidoItemExistente(pedidoItem))
            {
                await _mediatorHandler.PublicarNotificacao(new DomainNotification("pedido", "Item do pedido não encontrado!"));
                return false;
            }

            pedido.AtualizarUnidades(pedidoItem, message.Quantidade);           

            pedido.AdicionarEvento(new PedidoProdutoAtualizadoEvent(message.ClienteId, pedido.Id, message.ProdutoId, message.Quantidade));

            _pedidoRepository.AtualizarItem(pedidoItem);
            _pedidoRepository.Atualizar(pedido);

            return await _pedidoRepository.UnitOfWork.Commit();

        }

        public async Task<bool> Handle(AplicarVoucherPedidoCommand message, CancellationToken cancellationToken)
        {
            if (!ValidarComando(message)) return false;

            var pedido = await _pedidoRepository.ObterPedidoRascunhoPorClienteId(message.ClienteId);

            if(pedido == null)
            {
                await _mediatorHandler.PublicarNotificacao(new DomainNotification("pedido", "Pedido não encontrado!"));
                return false;
            }

            var voucher = await _pedidoRepository.ObterVoucherPorCodigo(message.CodigoVoucher);

            if(voucher == null)
            {
                await _mediatorHandler.PublicarNotificacao(new DomainNotification("pedido", "Voucher não encontrado"));
                return false;
            }

            var voucherAplicacaoValidation = pedido.AplicarVouchger(voucher);
            if (!voucherAplicacaoValidation.IsValid)
            {
                foreach(var error in voucherAplicacaoValidation.Errors)
                {
                    await _mediatorHandler.PublicarNotificacao(new DomainNotification(error.ErrorCode, error.ErrorMessage));
                }
                return false;
            }           
            pedido.AdicionarEvento(new VoucherAplicadoPedidoEvent(message.ClienteId, pedido.Id, voucher.Id));

            _pedidoRepository.Atualizar(pedido);

            return await _pedidoRepository.UnitOfWork.Commit();

        }

        public async Task<bool> Handle(IniciarPedidoCommand message, CancellationToken cancellationToken)
        {
            if (!ValidarComando(message)) return false;

            var pedido  = await _pedidoRepository.ObterPedidoRascunhoPorClienteId(message.ClienteId);
            pedido.IniciarPedido();

            var itensList = new List<Item>();
            pedido.PedidoItems.ForEach(i => itensList.Add(new Item { Id = i.ProdutoId, Quantidade = i.Quantidade }));
            var listaProdutosPedido = new ListaProdutosPedido { PedidoId = pedido.Id, Itens = itensList };

            pedido.AdicionarEvento(new PedidoIniciadoEvent(pedido.Id,pedido.ClienteId, pedido.ValorTotal, listaProdutosPedido, message.NomeCartao, message.NumeroCartao, message.ExpiracaoCartao, message.CvvCartao));

            _pedidoRepository.Atualizar(pedido);
            return await _pedidoRepository.UnitOfWork.Commit();

        }

        public async Task<bool> Handle(FinalizarPedidoCommand message, CancellationToken cancellationToken)
        {
            var pedido = await _pedidoRepository.ObterPorId(message.PedidoId);

            if(pedido == null)
            {
                await _mediatorHandler.PublicarNotificacao(new DomainNotification("pedido", "Pedido não encontrado!"));
                return false;
            }

            pedido.FinalizarPedido();

            pedido.AdicionarEvento(new PedidoFinalizadoEvent(message.PedidoId));
            return await _pedidoRepository.UnitOfWork.Commit();

        }

        public async Task<bool> Handle(CancelarProcessamentoPedidoEstornarEstoqueCommand message, CancellationToken cancellationToken)
        {
            var pedido = await _pedidoRepository.ObterPorId(message.PedidoId);

            if(pedido == null)
            {
                await _mediatorHandler.PublicarNotificacao(new DomainNotification("pedido", "Pedido não encontrado!"));
                return false;
            }

            var itensList = new List<Item>();
            pedido.PedidoItems.ForEach(i => itensList.Add(new Item { Id = i.ProdutoId, Quantidade = i.Quantidade }));
            var listaProdutosPedido = new ListaProdutosPedido { PedidoId = pedido.Id, Itens = itensList };

            pedido.AdicionarEvento(new PedidoProcessamentoCanceladoEvent(pedido.Id, pedido.ClienteId, listaProdutosPedido));
            pedido.TornarRascunho();

            return await _pedidoRepository.UnitOfWork.Commit();

        }


        private bool ValidarComando(Command message)
        {
            if (message.EhValido()) return true;

            foreach(var erro in message.ValidationResult.Errors)
            {
                _mediatorHandler.PublicarNotificacao(new DomainNotification(message.MessageType, erro.ErrorMessage));
            }

            return false;
        }
    }
}
