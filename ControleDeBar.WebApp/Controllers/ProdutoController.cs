using ControleDeBar.Dominio.ModuloConta;
using ControleDeBar.Dominio.ModuloGarcom;
using ControleDeBar.Dominio.ModuloProduto;
using ControleDeBar.Infraestrura.Arquivos.Compartilhado;
using ControleDeBar.Infraestrutura.Arquivos.ModuloConta;
using ControleDeBar.Infraestrutura.Arquivos.ModuloProduto;
using ControleDeBar.WebApp.Extensions;
using ControleDeBar.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeBar.WebApp.Controllers;

[Route("produtos")]
public class ProdutoController : Controller
{
    private readonly ContextoDados contextoDados;
    private readonly IRepositorioProduto repositorioProduto;
    private readonly IRepositorioConta repositorioConta;

    public ProdutoController(
        ContextoDados contextoDados, 
        IRepositorioProduto repositorioProduto, 
        IRepositorioConta repositorioConta)
    {
        this.contextoDados = contextoDados;
        this.repositorioProduto = repositorioProduto;
        this.repositorioConta = repositorioConta;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var registros = repositorioProduto.SelecionarRegistros();

        var visualizarVM = new VisualizarProdutosViewModel(registros);

        return View(visualizarVM);
    }

    [HttpGet("cadastrar")]
    public IActionResult Cadastrar()
    {
        var cadastrarVM = new CadastrarProdutoViewModel();

        return View(cadastrarVM);
    }

    [HttpPost("cadastrar")]
    [ValidateAntiForgeryToken]
    public IActionResult Cadastrar(CadastrarProdutoViewModel cadastrarVM)
    {
        var registros = repositorioProduto.SelecionarRegistros();

        foreach (var item in registros)
        {
            if (item.Nome.Equals(cadastrarVM.Nome))
            {
                ModelState.AddModelError("CadastroUnico", "Já existe um produto registrado com este nome.");
                break;
            }
        }

        if (!ModelState.IsValid)
            return View(cadastrarVM);

        var entidade = cadastrarVM.ParaEntidade();

        repositorioProduto.CadastrarRegistro(entidade);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("editar/{id:guid}")]
    public IActionResult Editar(Guid id)
    {
        var registroSelecionado = repositorioProduto.SelecionarRegistroPorId(id);

        var editarVM = new EditarProdutoViewModel(
            id,
            registroSelecionado.Nome,
            registroSelecionado.Valor
        );

        return View(editarVM);
    }

    [HttpPost("editar/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(Guid id, EditarProdutoViewModel editarVM)
    {
        var registros = repositorioProduto.SelecionarRegistros();

        foreach (var item in registros)
        {
            if (!item.Id.Equals(id) && item.Nome.Equals(editarVM.Nome))
            {
                ModelState.AddModelError("CadastroUnico", "Já existe um produto registrado com este nome.");
                break;
            }
        }

        if (!ModelState.IsValid)
            return View(editarVM);

        var entidadeEditada = editarVM.ParaEntidade();

        repositorioProduto.EditarRegistro(id, entidadeEditada);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("excluir/{id:guid}")]
    public IActionResult Excluir(Guid id)
    {
        var registroSelecionado = repositorioProduto.SelecionarRegistroPorId(id);

        var excluirVM = new ExcluirProdutoViewModel(registroSelecionado.Id, registroSelecionado.Nome);

        return View(excluirVM);
    }

    [HttpPost("excluir/{id:guid}")]
    public IActionResult Excluir(Guid id, ExcluirProdutoViewModel excluirVM)
    {
        var registros = repositorioConta.SelecionarContas();

        var produtoAtual = repositorioProduto.SelecionarRegistroPorId(id);

        foreach (var i in registros)
        {
            if (!i.EstaAberta) continue;

            foreach (var j in i.Pedidos)
            {
                if (j.Produto.Equals(produtoAtual))
                {
                    ModelState.AddModelError("CadastroUnico", "Existe uma conta ativa registrada com este produto.");
                    break;
                }
            }
        }

        if (!ModelState.IsValid)
            return View(excluirVM);

        repositorioProduto.ExcluirRegistro(id);

        return RedirectToAction(nameof(Index));
    }
}
