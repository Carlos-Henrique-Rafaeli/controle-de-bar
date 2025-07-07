using ControleDeBar.Dominio.ModuloProduto;
using ControleDeBar.Infraestrutura.SqlServer.Compartilhado;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ControleDeBar.Infraestrutura.SqlServer.ModuloProduto;

public class RepositorioProdutoEmSql : RepositorioBaseEmSql<Produto>, IRepositorioProduto
{
    public RepositorioProdutoEmSql(IDbConnection conexaoComBanco) : base(conexaoComBanco)
    {
    }

    public override string ObterSqlInserir()
    {
        return @"INSERT INTO [TBPRODUTO] 
		    (
                [ID],
			    [NOME],
			    [PRECO]
		    )
		    VALUES
		    (
                @ID,
			    @NOME,
			    @PRECO
		    );";
    }

    public override string ObterSqlEditar()
    {
        return @"UPDATE [TBPRODUTO]	
		    SET
			    [ID] = @ID,
			    [NOME] = @NOME,
			    [PRECO] = @PRECO
		    WHERE
			    [ID] = @ID";
    }

    public override string ObterSqlExcluir()
    {
        return @"DELETE FROM [TBPRODUTO]
		    WHERE
			    [ID] = @ID";
    }

    public override string ObterSqlSelecionarPorId()
    {
        return @"SELECT 
		        [ID],
			    [NOME],
			    [PRECO]
	        FROM 
		        [TBPRODUTO]
            WHERE
                [ID] = @ID";
    }

    public override string ObterSqlSelecionarTodos()
    {
        return @"SELECT 
		        [ID],
			    [NOME],
			    [PRECO]
	        FROM 
		        [TBPRODUTO]";
    }

    public override void ConfigurarParametrosRegistro(IDbCommand comando, Produto registro)
    {
        comando.AdicionarParametro("ID", registro.Id);
        comando.AdicionarParametro("NOME", registro.Nome);
        comando.AdicionarParametro("PRECO", registro.Valor);
    }

    public override Produto ConverterParaRegistro(IDataReader leitor)
    {
        var produto = new Produto(
            Convert.ToString(leitor["NOME"])!,
            Convert.ToDecimal(leitor["PRECO"])
        );

        produto.Id = Guid.Parse(leitor["ID"].ToString()!);

        return produto;
    }
}
