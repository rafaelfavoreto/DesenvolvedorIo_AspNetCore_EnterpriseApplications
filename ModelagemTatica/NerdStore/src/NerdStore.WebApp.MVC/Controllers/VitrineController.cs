using Microsoft.AspNetCore.Mvc;
using NerdStore.Catalago.Application.Services;

namespace NerdStore.WebApp.MVC.Controllers
{
    public class VitrineController : Controller
    {
        private readonly IProdutoAppServices _produtoAppService;

        public VitrineController(IProdutoAppServices produtoAppService)
        {
            _produtoAppService = produtoAppService;
        }

        [HttpGet]
        [Route("")]
        [Route("vitrine")]
        public async Task<IActionResult> Index()
        {
            return View(await _produtoAppService.ObterTodos());
        }

        [HttpGet]
        [Route("produto-detalhe/{id}")]
        public async Task<IActionResult> ProdutoDetalhe(Guid id)
        {
            return View(await _produtoAppService.ObterPorId(id));
        }
    }
}