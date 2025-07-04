﻿using ControleDeBar.Dominio.ModuloConta;
using ControleDeBar.Dominio.ModuloGarcom;
using ControleDeBar.Dominio.ModuloMesa;
using ControleDeBar.Dominio.ModuloProduto;
using ControleDeBar.Infraestrura.Arquivos.Compartilhado;
using ControleDeBar.Infraestrura.Arquivos.ModuloMesa;
using ControleDeBar.Infraestrutura.Arquivos.ModuloConta;
using ControleDeBar.Infraestrutura.Arquivos.ModuloGarcom;
using ControleDeBar.Infraestrutura.Arquivos.ModuloProduto;
using ControleDeBar.WebApp.Extensions;
using ControleDeBar.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ControleDeBar.WebApp.Controllers;

[Route("contas")]
public class ContaController : Controller
{
    private readonly IRepositorioConta repositorioConta;
    private readonly IRepositorioMesa repositorioMesa;
    private readonly IRepositorioGarcom repositorioGarcom;
    private readonly IRepositorioProduto repositorioProduto;

    public ContaController(
        IRepositorioConta repositorioConta, 
        IRepositorioMesa repositorioMesa, 
        IRepositorioGarcom repositorioGarcom, 
        IRepositorioProduto repositorioProduto)
    {
        this.repositorioConta = repositorioConta;
        this.repositorioMesa = repositorioMesa;
        this.repositorioGarcom = repositorioGarcom;
        this.repositorioProduto = repositorioProduto;
    }

    [HttpGet]
    public IActionResult Index(string? status)
    {
        List<Conta> registros;

        switch (status)
        {
            case "abertas": registros = repositorioConta.SelecionarContasAbertas(); break;
            case "fechadas": registros = repositorioConta.SelecionarContasFechadas(); break;
            case "faturas": registros = repositorioConta.SelecionarContasFechadas(); break;
            default: registros = repositorioConta.SelecionarContas(); break;
        }

        ViewBag.Status = status;

        var visualizarVM = new VisualizarContasViewModel(registros);

        return View(visualizarVM);
    }

    [HttpGet("abrir")]
    public IActionResult Abrir()
    {
        var mesas = repositorioMesa.SelecionarRegistros();
        var garcons = repositorioGarcom.SelecionarRegistros();

        var abrirVM = new AbrirContaViewModel(mesas, garcons);

        return View(abrirVM);
    }

    [HttpPost("abrir")]
    [ValidateAntiForgeryToken]
    public IActionResult Abrir(AbrirContaViewModel abrirVM)
    {
        var registros = repositorioConta.SelecionarContas();

        foreach (var conta in registros)
        {
            if (conta.Titular.Equals(abrirVM.Titular) && conta.EstaAberta)
            {
                ModelState.AddModelError("CadastroUnico", "Já existe uma conta aberta para este titular.");
                break;
            }

            if (conta.Mesa.EstaOcupada)
            {
                ModelState.AddModelError("CadastroUnico", "A mesa selecionada já esta ocupada.");
                break;
            }
        }

        if (!ModelState.IsValid)
            return View(abrirVM);

        var mesas = repositorioMesa.SelecionarRegistros();
        var garcons = repositorioGarcom.SelecionarRegistros();

        var entidade = abrirVM.ParaEntidade(mesas, garcons);

        repositorioConta.CadastrarConta(entidade);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet, Route("/contas/{id:guid}/fechar")]
    public IActionResult Fechar(Guid id)
    {
        var registro = repositorioConta.SelecionarPorId(id);

        var fecharContaVM = new FecharContaViewModel(
            registro.Id,
            registro.Titular,
            registro.Mesa.Numero,
            registro.Garcom.Nome,
            registro.CalcularValorTotal()
        );

        return View(fecharContaVM);
    }

    [HttpPost, Route("/contas/{id:guid}/fechar")]
    public IActionResult FecharConfirmado(Guid id)
    {
        var registroSelecionado = repositorioConta.SelecionarPorId(id);

        registroSelecionado.Fechar();

        repositorioConta.AtualizarConta(registroSelecionado, id);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet, Route("/contas/{id:guid}/gerenciar-pedidos")]
    public IActionResult GerenciarPedidos(Guid id)
    {
        var contaSelecionada = repositorioConta.SelecionarPorId(id);
        var produtos = repositorioProduto.SelecionarRegistros();

        var gerenciarPedidosVm = new GerenciarPedidosViewModel(contaSelecionada, produtos);

        return View(gerenciarPedidosVm);
    }

    [HttpPost, Route("/contas/{id:guid}/adicionar-pedido")]
    public IActionResult AdicionarPedido(Guid id, AdicionarPedidoViewModel adicionarPedidoVm)
    {
        var contaSelecionada = repositorioConta.SelecionarPorId(id);
        var produtoSelecionado = repositorioProduto.SelecionarRegistroPorId(adicionarPedidoVm.IdProduto);

        contaSelecionada.RegistrarPedido(
            produtoSelecionado,
            adicionarPedidoVm.Quantidade
        );

        repositorioConta.AdicionarPedido(contaSelecionada.Pedidos.Last());

        var produtos = repositorioProduto.SelecionarRegistros();

        var gerenciarPedidosVm = new GerenciarPedidosViewModel(contaSelecionada, produtos);

        return View("GerenciarPedidos", gerenciarPedidosVm);
    }

    [HttpPost, Route("/contas/{id:guid}/remover-pedido/{idPedido:guid}")]
    public IActionResult RemoverPedido(Guid id, Guid idPedido)
    {
        var contaSelecionada = repositorioConta.SelecionarPorId(id);

        var pedidoRemovido = contaSelecionada.RemoverPedido(idPedido);

        repositorioConta.RemoverPedido(pedidoRemovido);

        var produtos = repositorioProduto.SelecionarRegistros();

        var gerenciarPedidosVm = new GerenciarPedidosViewModel(contaSelecionada, produtos);

        return View("GerenciarPedidos", gerenciarPedidosVm);
    }

    [HttpGet("detalhes/{id:guid}")]
    public IActionResult Detalhes(Guid id)
    {
        var registroSelecionado = repositorioConta.SelecionarPorId(id);

        var detalhesVM = new DetalhesContaViewModel(
            id,
            registroSelecionado.Titular,
            registroSelecionado.Mesa.Numero,
            registroSelecionado.Garcom.Nome,
            registroSelecionado.EstaAberta,
            registroSelecionado.CalcularValorTotal(),
            registroSelecionado.Pedidos
        );

        return View(detalhesVM);
    }
}
