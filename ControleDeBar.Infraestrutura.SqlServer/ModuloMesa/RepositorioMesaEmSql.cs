using ControleDeBar.Dominio.ModuloMesa;
using ControleDeBar.Infraestrutura.SqlServer.Compartilhado;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ControleDeBar.Infraestrutura.SqlServer.ModuloMesa;

public class RepositorioMesaEmSql : RepositorioBaseEmSql<Mesa>, IRepositorioMesa
{
    public RepositorioMesaEmSql(IDbConnection conexaoComBanco) : base(conexaoComBanco)
    {
    }

    public override string ObterSqlInserir()
    {
        return @"INSERT INTO [TBMESA] 
		    (
                [ID],
			    [NUMERO],
			    [CAPACIDADE],
			    [ESTAOCUPADA]
		    )
		    VALUES
		    (
                @ID,
			    @NUMERO,
			    @CAPACIDADE,
			    @ESTAOCUPADA
		    );";
    }

    public override string ObterSqlEditar()
    {
        return @"UPDATE [TBMESA]	
		    SET
			    [ID] = @ID,
			    [NUMERO] = @NUMERO,
			    [CAPACIDADE] = @CAPACIDADE,
			    [ESTAOCUPADA] = @ESTAOCUPADA
		    WHERE
			    [ID] = @ID";
    }

    public override string ObterSqlExcluir()
    {
        return @"DELETE FROM [TBMESA]
		    WHERE
			    [ID] = @ID";
    }
    
    public override string ObterSqlSelecionarPorId()
    {
        return @"SELECT 
		        [ID],
			    [NUMERO],
			    [CAPACIDADE],
			    [ESTAOCUPADA]
	        FROM 
		        [TBMESA]
            WHERE
                [ID] = @ID";
    }

    public override string ObterSqlSelecionarTodos()
    {
        return @"SELECT 
		        [ID],
			    [NUMERO],
			    [CAPACIDADE],
			    [ESTAOCUPADA]
	        FROM 
		        [TBMESA]";
    }

    public override void ConfigurarParametrosRegistro(IDbCommand comando, Mesa registro)
    {
        comando.AdicionarParametro("ID", registro.Id);
        comando.AdicionarParametro("NUMERO", registro.Numero);
        comando.AdicionarParametro("CAPACIDADE", registro.Capacidade);
        comando.AdicionarParametro("ESTAOCUPADA", registro.EstaOcupada ? 1 : 0);
    }

    public override Mesa ConverterParaRegistro(IDataReader leitor)
    {
        var mesa = new Mesa
        {
            Numero = Convert.ToInt32(leitor["NUMERO"]),
            Capacidade = Convert.ToInt32(leitor["CAPACIDADE"]),
            EstaOcupada = Convert.ToBoolean(leitor["ESTAOCUPADA"]) ? true : false,
            Id = Guid.Parse(leitor["ID"].ToString()!)
        };

        return mesa;
    }
}
