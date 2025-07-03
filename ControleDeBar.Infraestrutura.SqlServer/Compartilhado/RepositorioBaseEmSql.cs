using ControleDeBar.Dominio.Compartilhado;
using ControleDeBar.Dominio.ModuloConta;
using Microsoft.Data.SqlClient;

namespace ControleDeBar.Infraestrutura.SqlServer.Compartilhado;

public abstract class RepositorioBaseEmSql<T> where T : EntidadeBase<T>
{
    private readonly string connectionString =
        "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=ControleDeBarDb;Integrated Security=True";

    public void CadastrarRegistro(T novoRegistro)
    {
        var sqlInserir = ObterSqlInserir();

        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoInsercao = new SqlCommand(sqlInserir, conexaoComBanco);

        ConfigurarParametros(comandoInsercao, novoRegistro);

        conexaoComBanco.Open();

        comandoInsercao.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    public bool EditarRegistro(Guid idRegistro, T registroEditado)
    {
        var sqlEditar = ObterSqlEditar();

        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoEdicao = new SqlCommand(sqlEditar, conexaoComBanco);

        registroEditado.Id = idRegistro;

        ConfigurarParametros(comandoEdicao, registroEditado);

        conexaoComBanco.Open();

        var linhasAfetadas = comandoEdicao.ExecuteNonQuery();

        conexaoComBanco.Close();

        return linhasAfetadas > 0;
    }

    public bool ExcluirRegistro(Guid idRegistro)
    {
        var sqlExcluir = ObterSqlExcluir();

        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoExclusao = new SqlCommand(sqlExcluir, conexaoComBanco);

        comandoExclusao.Parameters.AddWithValue("ID", idRegistro);

        conexaoComBanco.Open();

        var linhasAfetadas = comandoExclusao.ExecuteNonQuery();

        conexaoComBanco.Close();

        return linhasAfetadas > 0;
    }

    public T? SelecionarRegistroPorId(Guid idRegistro)
    {
        var sqlSelecionarPorId = ObterSqlSelecionarPorId();

        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        SqlCommand comandoSelecao =
            new SqlCommand(sqlSelecionarPorId, conexaoComBanco);

        comandoSelecao.Parameters.AddWithValue("ID", idRegistro);

        conexaoComBanco.Open();

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        T? registro = null;

        if (leitor.Read())
            registro = ConverterParaEntidade(leitor);

        conexaoComBanco.Close();

        return registro;
    }

    public List<T> SelecionarRegistros()
    {
        var sqlSelecionarTodos = ObterSqlSelecionarTodos();

        SqlConnection conexaoComBanco = new SqlConnection(connectionString);

        conexaoComBanco.Open();

        SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarTodos, conexaoComBanco);

        SqlDataReader leitor = comandoSelecao.ExecuteReader();

        var contatos = new List<T>();

        while (leitor.Read())
        {
            var contato = ConverterParaEntidade(leitor);

            contatos.Add(contato);
        }

        conexaoComBanco.Close();

        return contatos;
    }

    public abstract string ObterSqlInserir();
    
    public abstract string ObterSqlEditar();

    public abstract string ObterSqlExcluir();
    
    public abstract string ObterSqlSelecionarPorId();
    
    public abstract string ObterSqlSelecionarTodos();
    
    public abstract void ConfigurarParametros(SqlCommand comando, T registro);
    
    public abstract T ConverterParaEntidade(SqlDataReader leitor);
}
