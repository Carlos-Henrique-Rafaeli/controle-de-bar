using Serilog.Events;
using Serilog;

namespace ControleDeBar.WebApp.DependencyInjection;

public static class SerilogConfig
{
    public static void AddSerilogConfig(this IServiceCollection services, ILoggingBuilder logging)
    {
        var pastaRaiz = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "AcademiaProgramador2025");

        var caminhoLog = Path.Combine(pastaRaiz, "ControleDeBar", "erro.log");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File(caminhoLog, LogEventLevel.Error)
            .CreateLogger();

        logging.ClearProviders();

        services.AddSerilog();
    }
}
