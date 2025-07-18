﻿using ControleDeBar.Dominio.ModuloConta;
using ControleDeBar.Dominio.ModuloMesa;
using ControleDeBar.Infraestrura.Arquivos.Compartilhado;
using ControleDeBar.Infraestrura.Arquivos.ModuloMesa;
using ControleDeBar.Infraestrutura.Arquivos.ModuloConta;
using ControleDeBar.WebApp.Extensions;
using ControleDeBar.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeBar.WebApp.Controllers;

[Route("mesas")]
public class MesaController : Controller
{
    private readonly ContextoDados contextoDados;
    private readonly IRepositorioMesa repositorioMesa;
    private readonly IRepositorioConta repositorioConta;

    public MesaController(
        ContextoDados contextoDados, 
        IRepositorioMesa repositorioMesa, 
        IRepositorioConta repositorioConta)
    {
        this.contextoDados = contextoDados;
        this.repositorioMesa = repositorioMesa;
        this.repositorioConta = repositorioConta;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var registros = repositorioMesa.SelecionarRegistros();

        var visualizarVM = new VisualizarMesasViewModel(registros);

        return View(visualizarVM);
    }


    [HttpGet("cadastrar")]
    public IActionResult Cadastrar()
    {
        var cadastrarVM = new CadastrarMesaViewModel();

        return View(cadastrarVM);
    }

    [HttpPost("cadastrar")]
    [ValidateAntiForgeryToken]
    public IActionResult Cadastrar(CadastrarMesaViewModel cadastrarVM)
    {
        var registros = repositorioMesa.SelecionarRegistros();

        foreach (var item in registros)
        {
            if (item.Numero.Equals(cadastrarVM.Numero))
            {
                ModelState.AddModelError("CadastroUnico", "Já existe uma mesa registrada com este número.");
                break;
            }
        }

        if (!ModelState.IsValid)
            return View(cadastrarVM);

        var entidade = cadastrarVM.ParaEntidade();

        repositorioMesa.CadastrarRegistro(entidade);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("editar/{id:guid}")]
    public IActionResult Editar(Guid id)
    {
        var registroSelecionado = repositorioMesa.SelecionarRegistroPorId(id);

        var editarVM = new EditarMesaViewModel(
            id,
            registroSelecionado.Numero,
            registroSelecionado.Capacidade
        );

        return View(editarVM);
    }

    [HttpPost("editar/{id:guid}")]
    [ValidateAntiForgeryToken]
    public ActionResult Editar(Guid id, EditarMesaViewModel editarVM)
    {
        var registros = repositorioMesa.SelecionarRegistros();

        foreach (var item in registros)
        {
            if (!item.Id.Equals(id) && item.Numero.Equals(editarVM.Numero))
            {
                ModelState.AddModelError("CadastroUnico", "Já existe uma mesa registrada com este número.");
                break;
            }
        }

        if (!ModelState.IsValid)
            return View(editarVM);

        var entidadeEditada = editarVM.ParaEntidade();

        repositorioMesa.EditarRegistro(id, entidadeEditada);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("excluir/{id:guid}")]
    public IActionResult Excluir(Guid id)
    {
        var registroSelecionado = repositorioMesa.SelecionarRegistroPorId(id);

        var excluirVM = new ExcluirMesaViewModel(registroSelecionado.Id, registroSelecionado.Numero);

        return View(excluirVM);
    }

    [HttpPost("excluir/{id:guid}")]
    public IActionResult Excluir(Guid id, ExcluirMesaViewModel excluirVM)
    {
        var registros = repositorioConta.SelecionarContas();

        var mesaAtual = repositorioMesa.SelecionarRegistroPorId(id);

        foreach (var i in registros)
        {
            if (!i.EstaAberta) continue;

            if (i.Mesa.Equals(mesaAtual))
            {
                ModelState.AddModelError("CadastroUnico", "Existe uma conta ativa registrada com esta mesa.");
                break;
            }
        }

        if (!ModelState.IsValid)
            return View(excluirVM);

        repositorioMesa.ExcluirRegistro(id);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("detalhes/{id:guid}")]
    public IActionResult Detalhes(Guid id)
    {
        var registroSelecionado = repositorioMesa.SelecionarRegistroPorId(id);

        var contas = repositorioConta.SelecionarContas();

        Conta contaSelecionada = null;

        foreach (var i in contas)
        {
            if (i.Mesa.Equals(registroSelecionado))
            {
                contaSelecionada = i;
                break;
            }
        }

        var detalhesVM = new DetalhesMesaViewModel(
            id,
            registroSelecionado.Numero,
            registroSelecionado.Capacidade,
            contaSelecionada
        );

        return View(detalhesVM);
    }
}
