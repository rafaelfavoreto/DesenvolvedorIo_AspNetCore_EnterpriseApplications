using EventSourcing;
using MediatR;
using NerdStore.Catalago.Application.Services;
using NerdStore.Catalago.Domain;
using NerdStore.Catalago.Domain.Events;
using NerdStore.Catalogo.Data;
using NerdStore.Catalogo.Data.Repository;
using NerdStore.Catalogo.Domain.Events;
using NerdStore.Core.Communication.Mediator;
using NerdStore.Core.Data.EventSourcing;
using NerdStore.Core.Messagens.CommonMessages.IntegrationEvents;
using NerdStore.Core.Messagens.CommonMessages.Notifications;
using NerdStore.Core.Messages.CommonMessages.IntegrationEvents;
using NerdStore.Pagamentos.AntiCorruption;
using NerdStore.Pagamentos.Business;
using NerdStore.Pagamentos.Business.Events;
using NerdStore.Pagamentos.Data;
using NerdStore.Pagamentos.Data.Repository;
using NerdStore.Vendas.Application.Commands;
using NerdStore.Vendas.Application.Events;
using NerdStore.Vendas.Application.Queries;
using NerdStore.Vendas.Data;
using NerdStore.Vendas.Data.Repository;
using NerdStore.Vendas.Domain;
using ConfigurationManager = NerdStore.Pagamentos.AntiCorruption.ConfigurationManager;

namespace NerdStore.WebApp.MVC.Setup
{
    public static class DependencyInjection
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            // Mediator
            services.AddScoped<IMediatrHandler, MediatrHandler>();

            // Notifications
            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();

            // Event Sourcing
            services.AddSingleton<IEventStoreService, EventStoreService>();
            services.AddSingleton<IEventSourcingRepository, EventSourcingRepository>();

            // Catalogo
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<IProdutoAppServices, ProdutoAppServices>();
            services.AddScoped<IEstoqueService, EstoqueService>();
            services.AddScoped<CatalogoContext>();

            services.AddScoped<INotificationHandler<ProdutoAbaixoEstoqueEvent>, ProdutoEventHandler>();
            services.AddScoped<INotificationHandler<PedidoIniciadoEvent>, ProdutoEventHandler>();
            services.AddScoped<INotificationHandler<PedidoProcessamentoCanceladoEvent>, ProdutoEventHandler>();

            //Vendas            
            services.AddScoped<IPedidoRepository, PedidoRepository>();
            services.AddScoped<IPedidoQueries, PedidoQueries>();
            services.AddScoped<VendasContext>();

            services.AddScoped<IRequestHandler<AdicionarItemPedidoCommand, bool>, PedidoCommandHandler>();
            services.AddScoped<IRequestHandler<AtualizarItemPedidoCommand, bool>, PedidoCommandHandler>();
            services.AddScoped<IRequestHandler<RemoverItemPedidoCommand, bool>, PedidoCommandHandler>();
            services.AddScoped<IRequestHandler<AplicarVoucherPedidoCommand, bool>, PedidoCommandHandler>();

            services.AddScoped<INotificationHandler<PedidoRascunhoIniciadoEvent>, PedidoEventHandler>();
            services.AddScoped<INotificationHandler<PedidoAtualizadoEvent>, PedidoEventHandler>();
            services.AddScoped<INotificationHandler<PedidoItemAdicionadoEvent>, PedidoEventHandler>();

            services.AddScoped<IRequestHandler<AdicionarItemPedidoCommand, bool>, PedidoCommandHandler>();

            // Pagamento
            services.AddScoped<IPagamentoRepository, PagamentoRepository>();
            services.AddScoped<IPagamentoService, PagamentoService>();
            services.AddScoped<IPagamentoCartaoCreditoFacade, PagamentoCartaoCreditoFacade>();
            services.AddScoped<IPayPalGateway, PayPalGateway>();
            services.AddScoped<IConfigurationManager, ConfigurationManager>();
            services.AddScoped<PagamentoContext>();

            services.AddScoped<INotificationHandler<PedidoEstoqueConfirmadoEvent>, PagamentoEventHandler>();
        }
    }
}