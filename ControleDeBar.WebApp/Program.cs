using ControleDeBar.Dominio.ModuloConta;
using ControleDeBar.Dominio.ModuloGarcom;
using ControleDeBar.Dominio.ModuloMesa;
using ControleDeBar.Dominio.ModuloProduto;
using ControleDeBar.Infraestrura.Arquivos.Compartilhado;
using ControleDeBar.Infraestrutura.Arquivos.ModuloConta;
using ControleDeBar.Infraestrutura.SqlServer.ModuloConta;
using ControleDeBar.Infraestrutura.SqlServer.ModuloGarcom;
using ControleDeBar.Infraestrutura.SqlServer.ModuloMesa;
using ControleDeBar.Infraestrutura.SqlServer.ModuloProduto;
using ControleDeBar.WebApp.ActionFilters;
using ControleDeBar.WebApp.DependencyInjection;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ControleDeBar.WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews(options =>
        {
            options.Filters.Add<ValidarModeloAttribute>();
            options.Filters.Add<LogarAcaoAttribute>();
        });

        builder.Services.AddScoped<IDbConnection>(provider =>
        {
            var connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=ControleDeBarDb;Integrated Security=True";

            return new SqlConnection(connectionString);
        });

        builder.Services.AddScoped<ContextoDados>((_) => new ContextoDados(true));
        builder.Services.AddScoped<IRepositorioProduto, RepositorioProdutoEmSql>();
        builder.Services.AddScoped<IRepositorioConta, RepositorioContaEmSql>();
        builder.Services.AddScoped<IRepositorioGarcom, RepositorioGarcomEmSql>();
        builder.Services.AddScoped<IRepositorioMesa, RepositorioMesaEmSql>();

        builder.Services.AddSerilogConfig(builder.Logging);

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
            app.UseExceptionHandler("/erro");
        else
            app.UseDeveloperExceptionPage();

        app.UseAntiforgery();
        app.UseStaticFiles();
        app.UseHttpsRedirection();

        app.UseRouting();
        app.MapDefaultControllerRoute();

        app.Run();
    }
}
