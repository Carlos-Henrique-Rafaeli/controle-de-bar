﻿using System.Text.Json.Serialization;
using System.Text.Json;
using ControleDeBar.Dominio.ModuloMesa;
using ControleDeBar.Dominio.ModuloGarcom;
using ControleDeBar.Dominio.ModuloProduto;
using ControleDeBar.Dominio.ModuloConta;

namespace ControleDeBar.Infraestrura.Arquivos.Compartilhado;

public class ContextoDados
{
    private string pastaRaiz = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "AcademiaProgramador2025");
    private string arquivoArmazenamento = "dados.json";
    private string pastaPrincipal = "ControleDeBar";

    public List<Mesa> Mesas { get; set; }
    public List<Garcom> Garcons { get; set; }
    public List<Produto> Produtos { get; set; }
    public List<Conta> Contas { get; set; }

    public ContextoDados()
    {
        Mesas = new List<Mesa>();
        Garcons = new List<Garcom>();
        Produtos = new List<Produto>();
        Contas = new List<Conta>();
    }

    public ContextoDados(bool carregarDados) : this()
    {
        if (carregarDados)
            Carregar();
    }

    public void Salvar()
    {
        if (!Directory.Exists(pastaRaiz))
            Directory.CreateDirectory(pastaRaiz);

        string pastaProjeto = Path.Combine(pastaRaiz, pastaPrincipal);

        if (!Directory.Exists(pastaProjeto))
            Directory.CreateDirectory(pastaProjeto);

        JsonSerializerOptions jsonOptions = new JsonSerializerOptions();
        jsonOptions.WriteIndented = true;
        jsonOptions.ReferenceHandler = ReferenceHandler.Preserve;

        string caminhoCompleto = Path.Combine(pastaProjeto, arquivoArmazenamento);

        string json = JsonSerializer.Serialize(this, jsonOptions);

        File.WriteAllText(caminhoCompleto, json);
    }

    public void Carregar()
    {
        string pastaProjeto = Path.Combine(pastaRaiz, pastaPrincipal);

        string caminhoCompleto = Path.Combine(pastaProjeto, arquivoArmazenamento);

        if (!File.Exists(caminhoCompleto)) return;

        string json = File.ReadAllText(caminhoCompleto);

        if (string.IsNullOrWhiteSpace(json)) return;

        JsonSerializerOptions jsonOptions = new JsonSerializerOptions();
        jsonOptions.ReferenceHandler = ReferenceHandler.Preserve;

        ContextoDados contextoArmazenado = JsonSerializer.Deserialize<ContextoDados>(
            json, 
            jsonOptions
        )!;

        if (contextoArmazenado == null) return;

        Mesas = contextoArmazenado.Mesas;
        Garcons = contextoArmazenado.Garcons;
        Produtos = contextoArmazenado.Produtos;
        Contas = contextoArmazenado.Contas;
    }
}
