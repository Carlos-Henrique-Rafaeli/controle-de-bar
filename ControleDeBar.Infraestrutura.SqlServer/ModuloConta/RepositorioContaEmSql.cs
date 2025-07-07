using ControleDeBar.Dominio.ModuloConta;
using ControleDeBar.Dominio.ModuloGarcom;
using ControleDeBar.Dominio.ModuloMesa;
using ControleDeBar.Dominio.ModuloProduto;
using Microsoft.Data.SqlClient;

namespace ControleDeBar.Infraestrutura.SqlServer.ModuloConta;

public class RepositorioContaEmSql : IRepositorioConta
{
    private readonly string connectionString =
        "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=ControleDeBarDb;Integrated Security=True";

    public void CadastrarConta(Conta conta)
    {
        var sqlInserir =
            @"INSERT INTO [TBCONTA] 
		    (
                [ID],
			    [TITULAR],
			    [MESAID],
			    [GARCOMID],
			    [DATAABERTURA],
			    [DATATERMINO],
			    [ESTAABERTA]
		    )
		    VALUES
		    (
                @ID,
			    @TITULAR,
			    @MESAID,
			    @GARCOMID,
			    @DATAABERTURA,
			    @DATATERMINO,
			    @ESTAABERTA
		    );";

        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoInsercao = new SqlCommand(sqlInserir, conexaoComBanco);

        ConfigurarParametrosConta(comandoInsercao, conta);

        conexaoComBanco.Open();

        comandoInsercao.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    public bool AtualizarConta(Conta conta, Guid idConta)
    {
        var sqlEditar =
            @"UPDATE [TBCONTA]	
		    SET
			    [TITULAR] = @TITULAR,
			    [MESAID] = @MESAID,
			    [GARCOMID] = @GARCOMID,
			    [DATAABERTURA] = @DATAABERTURA,
			    [DATATERMINO] = @DATATERMINO,
			    [ESTAABERTA] = @ESTAABERTA
		    WHERE
			    [ID] = @ID";

        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoEdicao = new SqlCommand(sqlEditar, conexaoComBanco);

        conta.Id = idConta;

        ConfigurarParametrosConta(comandoEdicao, conta);

        conexaoComBanco.Open();

        var linhasAfetadas = comandoEdicao.ExecuteNonQuery();

        conexaoComBanco.Close();

        return linhasAfetadas > 0;
    }

    public void AdicionarPedido(Pedido pedido)
    {
        var sqlInserir =
            @"INSERT INTO [TBPEDIDO] 
		    (
                [ID],
			    [PRODUTOID],
			    [QUANTIDADE],
			    [CONTAID]
		    )
		    VALUES
		    (
                @ID,
			    @PRODUTOID,
			    @QUANTIDADE,
			    @CONTAID
		    );";

        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoInsercao = new SqlCommand(sqlInserir, conexaoComBanco);

        ConfigurarParametrosPedido(comandoInsercao, pedido);

        conexaoComBanco.Open();

        comandoInsercao.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    public bool RemoverPedido(Pedido produto)
    {
        var sqlExcluir =
           @"DELETE FROM [TBPEDIDO]
		    WHERE
			    [ID] = @ID";

        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoExclusao = new SqlCommand(sqlExcluir, conexaoComBanco);

        comandoExclusao.Parameters.AddWithValue("ID", produto.Id);

        conexaoComBanco.Open();

        var numeroRegistrosExcluidos = comandoExclusao.ExecuteNonQuery();

        conexaoComBanco.Close();

        return numeroRegistrosExcluidos > 0;
    }

    public List<Conta> SelecionarContas()
    {
        var sqlSelecionarTodos = 
            @"SELECT
                [ID],
			    [TITULAR],
			    [MESAID],
			    [GARCOMID],
			    [DATAABERTURA],
			    [DATATERMINO],
			    [ESTAABERTA] 
            FROM 
                [TBCONTA];";


        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarTodos, conexaoComBanco);

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        var contatos = new List<Conta>();

        while (leitor.Read())
        {
            var contato = ConverterParaConta(leitor);

            contatos.Add(contato);
        }

        conexaoComBanco.Close();

        return contatos;
    }

    public Conta? SelecionarPorId(Guid idRegistro)
    {
        var sqlSelecionarPorId =
            @"SELECT 
		        [ID],
			    [TITULAR],
			    [MESAID],
			    [GARCOMID],
			    [DATAABERTURA],
			    [DATATERMINO],
			    [ESTAABERTA]
	        FROM 
		        [TBCONTA]
            WHERE
                [ID] = @ID";

        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoSelecao =
            new SqlCommand(sqlSelecionarPorId, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("ID", idRegistro);

        conexaoComBanco.Open();

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        Conta? registro = null;

        if (leitor.Read())
            registro = ConverterParaConta(leitor);

        conexaoComBanco.Close();

        return registro;
    }

    public Mesa? SelecionarMesaPorId(Guid idRegistro)
    {
        var sqlSelecionarPorId =
            @"SELECT 
		        [ID],
			    [NUMERO],
			    [CAPACIDADE],
			    [ESTAOCUPADA]
	        FROM 
		        [TBMESA]
            WHERE
                [ID] = @ID";

        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoSelecao =
            new SqlCommand(sqlSelecionarPorId, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("ID", idRegistro);

        conexaoComBanco.Open();

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        Mesa? registro = null;

        if (leitor.Read())
            registro = ConverterParaMesa(leitor);

        conexaoComBanco.Close();

        return registro;
    }

    public Garcom? SelecionarGarcomPorId(Guid idRegistro)
    {
        var sqlSelecionarPorId =
            @"SELECT 
		        [ID],
			    [NOME],
			    [CPF]
	        FROM 
		        [TBGARCOM]
            WHERE
                [ID] = @ID";

        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoSelecao =
            new SqlCommand(sqlSelecionarPorId, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("ID", idRegistro);

        conexaoComBanco.Open();

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        Garcom? registro = null;

        if (leitor.Read())
            registro = ConverterParaGarcom(leitor);

        conexaoComBanco.Close();

        return registro;
    }

    public Produto? SelecionarProdutoPorId(Guid idRegistro)
    {
        var sqlSelecionarPorId =
            @"SELECT 
		        [ID],
			    [NOME],
			    [PRECO]
	        FROM 
		        [TBPRODUTO]
            WHERE
                [ID] = @ID";

        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoSelecao =
            new SqlCommand(sqlSelecionarPorId, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("ID", idRegistro);

        conexaoComBanco.Open();

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        Produto? registro = null;

        if (leitor.Read())
            registro = ConverterParaProduto(leitor);

        conexaoComBanco.Close();

        return registro;
    }

    private Conta ConverterParaConta(SqlDataReader leitor)
    {
        var mesaId = Guid.Parse(leitor["MESAID"].ToString()!);
        var garcomId = Guid.Parse(leitor["GARCOMID"].ToString()!);

        var mesa = SelecionarMesaPorId(mesaId);
        var garcom = SelecionarGarcomPorId(garcomId);

        DateTime? dataConclusao = null;

        if (!leitor["DATATERMINO"].Equals(DBNull.Value))
            dataConclusao = Convert.ToDateTime(leitor["DATATERMINO"]);

        var conta = new Conta
        {
            Titular = Convert.ToString(leitor["TITULAR"])!,
            Mesa = mesa!,
            Garcom = garcom!,
            Id = Guid.Parse(leitor["ID"].ToString()!),
            Abertura = Convert.ToDateTime(leitor["DATAABERTURA"]),
            Fechamento = dataConclusao,
            EstaAberta = Convert.ToBoolean(leitor["ESTAABERTA"])
        };

        CarregarPedidos(conta);

        return conta;
    }

    private Mesa ConverterParaMesa(SqlDataReader leitor)
    {
        var mesa = new Mesa
        {
            Id = Guid.Parse(leitor["ID"].ToString()!),
            Numero = Convert.ToInt32(leitor["NUMERO"]),
            Capacidade = Convert.ToInt32(leitor["CAPACIDADE"]),
            EstaOcupada = Convert.ToBoolean(leitor["ESTAOCUPADA"])
        };

        return mesa;
    }

    private Garcom ConverterParaGarcom(SqlDataReader leitor)
    {
        var garcom = new Garcom
        {
            Nome = Convert.ToString(leitor["NOME"])!,
            CPF = Convert.ToString(leitor["CPF"])!,
            Id = Guid.Parse(leitor["ID"].ToString()!)
        };

        return garcom;
    }

    private Produto ConverterParaProduto(SqlDataReader leitor)
    {
        var produto = new Produto
        {
            Id = Guid.Parse(leitor["ID"].ToString()!),
            Nome = Convert.ToString(leitor["NOME"])!,
            Valor = Convert.ToDecimal(leitor["PRECO"])
        };

        return produto;
    }

    private Pedido ConverterParaPedido(SqlDataReader leitor, Conta conta)
    {
        var produtoId = Guid.Parse(leitor["PRODUTOID"].ToString()!);

        var produto = SelecionarProdutoPorId(produtoId);

        var pedido = new Pedido
        {
            Id = Guid.Parse(leitor["ID"].ToString()!),
            Produto = produto,
            Conta = conta,
            Quantidade = Convert.ToInt32(leitor["QUANTIDADE"])
        };

        return pedido;
    }

    public List<Conta> SelecionarContasAbertas()
    {
        var sqlSelecionarPorId =
            @"SELECT 
		        [ID],
			    [TITULAR],
			    [MESAID],
			    [GARCOMID],
			    [DATAABERTURA],
			    [DATATERMINO],
			    [ESTAABERTA]
	        FROM 
		        [TBCONTA]
            WHERE
                [ESTAABERTA] = 1";

        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoSelecao =
            new SqlCommand(sqlSelecionarPorId, conexaoComBanco);


        conexaoComBanco.Open();

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        var registros = new List<Conta>();

        while (leitor.Read())
        {
            var conta = ConverterParaConta(leitor);

            registros.Add(conta);
        }

        conexaoComBanco.Close();

        return registros;    
    }

    public List<Conta> SelecionarContasFechadas()
    {
        var sqlSelecionarPorId =
            @"SELECT 
		        [ID],
			    [TITULAR],
			    [MESAID],
			    [GARCOMID],
			    [DATAABERTURA],
			    [DATATERMINO],
			    [ESTAABERTA]
	        FROM 
		        [TBCONTA]
            WHERE
                [ESTAABERTA] = 0";

        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoSelecao =
            new SqlCommand(sqlSelecionarPorId, conexaoComBanco);


        conexaoComBanco.Open();

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        var registros = new List<Conta>();

        while (leitor.Read())
        {
            var conta = ConverterParaConta(leitor);

            registros.Add(conta);
        }

        conexaoComBanco.Close();

        return registros;
    }

    private void ConfigurarParametrosConta(SqlCommand comandoInsercao, Conta conta)
    {
        comandoInsercao.Parameters.AddWithValue("ID", conta.Id);
        comandoInsercao.Parameters.AddWithValue("TITULAR", conta.Titular);
        comandoInsercao.Parameters.AddWithValue("MESAID", conta.Mesa.Id);
        comandoInsercao.Parameters.AddWithValue("GARCOMID", conta.Garcom.Id);
        comandoInsercao.Parameters.AddWithValue("DATAABERTURA", conta.Abertura);
        comandoInsercao.Parameters.AddWithValue("DATATERMINO", conta.Fechamento ?? (object)DBNull.Value);
        comandoInsercao.Parameters.AddWithValue("ESTAABERTA", conta.EstaAberta);
    }
    
    private void ConfigurarParametrosPedido(SqlCommand comando, Pedido pedido)
    {
        comando.Parameters.AddWithValue("ID", pedido.Id);
        comando.Parameters.AddWithValue("PRODUTOID", pedido.Produto.Id);
        comando.Parameters.AddWithValue("QUANTIDADE", pedido.Quantidade);
        comando.Parameters.AddWithValue("CONTAID", pedido.Conta.Id);
    }

    private void CarregarPedidos(Conta conta)
    {
        var sqlSelecionarItensTarefa =
             @"SELECT 
		            [ID],
			        [PRODUTOID],
			        [QUANTIDADE],
			        [CONTAID]
	            FROM 
		            [TBPEDIDO]
	            WHERE 
		            [CONTAID] = @CONTAID";

        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoSelecao =
            new SqlCommand(sqlSelecionarItensTarefa, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("CONTAID", conta.Id);

        conexaoComBanco.Open();

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        while (leitor.Read())
        {
            var pedido = ConverterParaPedido(leitor, conta);

            conta.Pedidos.Add(pedido);
        }

        conexaoComBanco.Close();
    }
}
