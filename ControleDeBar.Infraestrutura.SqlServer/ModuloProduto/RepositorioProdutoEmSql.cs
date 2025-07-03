using ControleDeBar.Dominio.ModuloProduto;
using ControleDeBar.Infraestrutura.SqlServer.Compartilhado;
using Microsoft.Data.SqlClient;

namespace ControleDeBar.Infraestrutura.SqlServer.ModuloProduto;

public class RepositorioProdutoEmSql : RepositorioBaseEmSql<Produto>, IRepositorioProduto
{
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

    public override void ConfigurarParametros(SqlCommand comando, Produto registro)
    {
        comando.Parameters.AddWithValue("ID", registro.Id);
        comando.Parameters.AddWithValue("NOME", registro.Nome);
        comando.Parameters.AddWithValue("PRECO", registro.Valor);
    }

    public override Produto ConverterParaEntidade(SqlDataReader leitor)
    {
        var produto = new Produto(
            Convert.ToString(leitor["NOME"])!,
            Convert.ToDecimal(leitor["PRECO"])
        );

        produto.Id = Guid.Parse(leitor["ID"].ToString()!);

        return produto;
    }
}
