using AutoMapper;
using NerdStore.Catalago.Application.ViewModels;
using NerdStore.Catalago.Domain;
using NerdStore.Core.DomainObjects;

namespace NerdStore.Catalago.Application.Services
{
    public class ProdutoAppServices : IProdutoAppServices
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IEstoqueService _estoqueService;
        private readonly IMapper _mapper;

        public ProdutoAppServices(IProdutoRepository produtoRepository, IEstoqueService estoqueService, IMapper mapper)
        {
            _produtoRepository = produtoRepository;
            _estoqueService = estoqueService;
            _mapper = mapper;
        }

        public async Task AdicionarProduto(ProdutoViewModel produtoViewModel)
        {
            var produto = _mapper.Map<Produto>(produtoViewModel);
            _produtoRepository.Adicionar(produto);

            await _produtoRepository.UnitOfWork.Commit();
        }

        public async Task AtualizarProduto(ProdutoViewModel produtoViewModel)
        {
            var produto = _mapper.Map<Produto>(produtoViewModel);
            _produtoRepository.Atualizar(produto);

            await _produtoRepository.UnitOfWork.Commit();
        }

        public async Task<ProdutoViewModel> DebitarEstoque(Guid id, int quantidade)
        {
            if (!_estoqueService.DebitarEstoque(id, quantidade).Result)
            {
                throw new DomainException("Falha ao debitar estoque");
            }

            return _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterPorId(id));

        }
        public async Task<IEnumerable<CategoriaViewModel>> ObterCategorias()
        {
            return _mapper.Map<IEnumerable<CategoriaViewModel>>(await _produtoRepository.ObterCategorias());
        }

        public async Task<IEnumerable<ProdutoViewModel>> ObterPorCategoria(int codigo)
        {
            return _mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRepository.ObterPorCategoria(codigo));
        }

        public async Task<ProdutoViewModel> ObterPorId(Guid id)
        {
            return _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterPorId(id));
        }

        public async Task<IEnumerable<ProdutoViewModel>> ObterTodos()
        {
           return _mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRepository.ObterTodos());
        }

        public async Task<ProdutoViewModel> ReporEstoque(Guid id, int quantidade)
        {
            if (!_estoqueService.ReporEstoque(id, quantidade).Result)
            {
                throw new Exception("Falha ao repor estoque");
            }
            return _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterPorId(id));
        }
        public void Dispose()
        {
            _produtoRepository?.Dispose();
            _estoqueService?.Dispose();
        }
    }
}
