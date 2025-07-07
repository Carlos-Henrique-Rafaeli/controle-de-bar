using ControleDeBar.Dominio.ModuloGarcom;
using ControleDeBar.Infraestrutura.SqlServer.Compartilhado;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ControleDeBar.Infraestrutura.SqlServer.ModuloGarcom;

public class RepositorioGarcomEmSql : RepositorioBaseEmSql<Garcom>, IRepositorioGarcom
{
    public RepositorioGarcomEmSql(IDbConnection conexaoComBanco):base(conexaoComBanco)
    {
    }

    public override string ObterSqlInserir()
    {
        return @"INSERT INTO [TBGARCOM] 
		    (
                [ID],
			    [NOME],
			    [CPF]
		    )
		    VALUES
		    (
                @ID,
			    @NOME,
			    @CPF
		    );";
    }

    public override string ObterSqlEditar()
    {
        return @"UPDATE [TBGARCOM]	
		    SET
			    [ID] = @ID,
			    [NOME] = @NOME,
			    [CPF] = @CPF
		    WHERE
			    [ID] = @ID";
    }

    public override string ObterSqlExcluir()
    {
        return @"DELETE FROM [TBGARCOM]
		    WHERE
			    [ID] = @ID";
    }

    public override string ObterSqlSelecionarPorId()
    {
        return @"SELECT 
		        [ID],
			    [NOME],
			    [CPF]
	        FROM 
		        [TBGARCOM]
            WHERE
                [ID] = @ID";
    }

    public override string ObterSqlSelecionarTodos()
    {
        return @"SELECT 
		        [ID],
			    [NOME],
			    [CPF]
	        FROM 
		        [TBGARCOM]";
    }

    public override void ConfigurarParametrosRegistro(IDbCommand comando, Garcom registro)
    {
        comando.AdicionarParametro("ID", registro.Id);
        comando.AdicionarParametro("NOME", registro.Nome);
        comando.AdicionarParametro("CPF", registro.CPF);
    }

    public override Garcom ConverterParaRegistro(IDataReader leitor)
    {
        var garcom = new Garcom
        {
            Nome = Convert.ToString(leitor["NOME"])!,
            CPF = Convert.ToString(leitor["CPF"])!,
            Id = Guid.Parse(leitor["ID"].ToString()!)
        };
            
        return garcom;
    }
}
