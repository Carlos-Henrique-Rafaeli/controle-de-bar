using ControleDeBar.Dominio.ModuloConta;
using ControleDeBar.Dominio.ModuloGarcom;
using ControleDeBar.Dominio.ModuloMesa;
using ControleDeBar.Infraestrura.Arquivos.Compartilhado;
using ControleDeBar.Infraestrutura.Arquivos.ModuloConta;
using ControleDeBar.Infraestrutura.Arquivos.ModuloGarcom;
using ControleDeBar.WebApp.Extensions;
using ControleDeBar.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;

namespace ControleDeBar.WebApp.Controllers;

[Route("garcons")]
public class GarcomController : Controller
{
    private readonly ContextoDados contextoDados;
    private readonly IRepositorioGarcom repositorioGarcom;
    private readonly IRepositorioConta repositorioConta;

    public GarcomController()
    {
        contextoDados = new ContextoDados(true);
        repositorioGarcom = new RepositorioGarcomEmArquivo(contextoDados);
        repositorioConta = new RepositorioContaEmArquivo(contextoDados);
    }

    public IActionResult Index()
    {
        var registros = repositorioGarcom.SelecionarRegistros();

        var visualizarVM = new VisualizarGarconsViewModel(registros);

        return View(visualizarVM);
    }

    [HttpGet("cadastrar")]
    public IActionResult Cadastrar()
    {
        var cadastrarVM = new CadastrarGarcomViewModel();

        return View(cadastrarVM);
    }

    [HttpPost("cadastrar")]
    [ValidateAntiForgeryToken]
    public IActionResult Cadastrar(CadastrarGarcomViewModel cadastrarVM)
    {
        var registros = repositorioGarcom.SelecionarRegistros();

        foreach (var item in registros)
        {
            if (item.Nome.Equals(cadastrarVM.Nome))
            {
                ModelState.AddModelError("CadastroUnico", "Já existe um garçom registrado com este nome.");
                break;
            }

            if (item.CPF.Equals(cadastrarVM.CPF))
            {
                ModelState.AddModelError("CadastroUnico", "Já existe um garçom registrado com este CPF.");
                break;
            }
        }

        if (!ModelState.IsValid)
            return View(cadastrarVM);

        var entidade = cadastrarVM.ParaEntidade();

        repositorioGarcom.CadastrarRegistro(entidade);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("editar/{id:guid}")]
    public IActionResult Editar(Guid id)
    {
        var registroSelecionado = repositorioGarcom.SelecionarRegistroPorId(id);

        var editarVM = new EditarGarcomViewModel(
            id,
            registroSelecionado.Nome,
            registroSelecionado.CPF
        );

        return View(editarVM);
    }

    [HttpPost("editar/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(Guid id, EditarGarcomViewModel editarVM)
    {
        var registros = repositorioGarcom.SelecionarRegistros();

        foreach (var item in registros)
        {
            if (!item.Id.Equals(id) && item.Nome.Equals(editarVM.Nome))
            {
                ModelState.AddModelError("CadastroUnico", "Já existe um garçom registrado com este nome.");
                break;
            }

            if (!item.Id.Equals(id) && item.CPF.Equals(editarVM.CPF))
            {
                ModelState.AddModelError("CadastroUnico", "Já existe um garçom registrado com este CPF.");
                break;
            }
        }

        if (!ModelState.IsValid)
            return View(editarVM);

        var entidadeEditada = editarVM.ParaEntidade();

        repositorioGarcom.EditarRegistro(id, entidadeEditada);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("excluir/{id:guid}")]
    public IActionResult Excluir(Guid id)
    {
        var registroSelecionado = repositorioGarcom.SelecionarRegistroPorId(id);

        var excluirVM = new ExcluirGarcomViewModel(registroSelecionado.Id, registroSelecionado.Nome);

        return View(excluirVM);
    }

    [HttpPost("excluir/{id:guid}")]
    public IActionResult Excluir(Guid id, ExcluirGarcomViewModel excluirVM)
    {
        var registros = repositorioConta.SelecionarContas();

        var garcomAtual = repositorioGarcom.SelecionarRegistroPorId(id);

        foreach (var i in registros)
        {
            if (!i.EstaAberta) continue;

            if (i.Garcom.Equals(garcomAtual))
            {
                ModelState.AddModelError("CadastroUnico", "Existe uma conta ativa registrada com este garçom.");
                break;
            }
        }

        if (!ModelState.IsValid)
            return View(excluirVM);

        repositorioGarcom.ExcluirRegistro(id);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("detalhes/{id:guid}")]
    public IActionResult Detalhes(Guid id)
    {
        var registroSelecionado = repositorioGarcom.SelecionarRegistroPorId(id);

        var detalhesVM = new DetalhesGarcomViewModel(
            id,
            registroSelecionado.Nome,
            registroSelecionado.CPF
        );

        return View(detalhesVM);
    }
}
