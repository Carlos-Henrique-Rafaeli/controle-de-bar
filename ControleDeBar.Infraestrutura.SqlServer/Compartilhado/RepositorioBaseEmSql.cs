using ControleDeBar.Dominio.Compartilhado;
using ControleDeBar.Dominio.ModuloConta;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ControleDeBar.Infraestrutura.SqlServer.Compartilhado;

public abstract class RepositorioBaseEmSql<T> where T : EntidadeBase<T>
{
    protected readonly IDbConnection conexaoComBanco;

    public RepositorioBaseEmSql(IDbConnection conexaoComBanco)
    {
        this.conexaoComBanco = conexaoComBanco;
    }

    public void CadastrarRegistro(T novoRegistro)
    {
        var sqlInserir = ObterSqlInserir();

        var comandoInsercao = conexaoComBanco.CreateCommand();
        comandoInsercao.CommandText = sqlInserir;

        ConfigurarParametrosRegistro(comandoInsercao, novoRegistro);

        conexaoComBanco.Open();

        comandoInsercao.ExecuteNonQuery();

        conexaoComBanco.Close();
    }

    public bool EditarRegistro(Guid idRegistro, T registroEditado)
    {
        var sqlEditar = ObterSqlEditar();

        var comandoEdicao = conexaoComBanco.CreateCommand();
        comandoEdicao.CommandText = sqlEditar;

        registroEditado.Id = idRegistro;

        ConfigurarParametrosRegistro(comandoEdicao, registroEditado);

        conexaoComBanco.Open();

        var linhasAfetadas = comandoEdicao.ExecuteNonQuery();

        conexaoComBanco.Close();

        return linhasAfetadas > 0;
    }

    public bool ExcluirRegistro(Guid idRegistro)
    {
        var sqlExcluir = ObterSqlExcluir();

        var comandoExclusao = conexaoComBanco.CreateCommand();
        comandoExclusao.CommandText = sqlExcluir;

        comandoExclusao.AdicionarParametro("ID", idRegistro);

        conexaoComBanco.Open();

        var linhasAfetadas = comandoExclusao.ExecuteNonQuery();

        conexaoComBanco.Close();

        return linhasAfetadas > 0;
    }

    public T? SelecionarRegistroPorId(Guid idRegistro)
    {
        var sqlSelecionarPorId = ObterSqlSelecionarPorId();

        var comandoSelecao = conexaoComBanco.CreateCommand();
        comandoSelecao.CommandText = sqlSelecionarPorId;

        comandoSelecao.AdicionarParametro("ID", idRegistro);

        conexaoComBanco.Open();

        var leitor = comandoSelecao.ExecuteReader();

        T? registro = null;

        if (leitor.Read())
            registro = ConverterParaRegistro(leitor);

        conexaoComBanco.Close();

        return registro;
    }

    public List<T> SelecionarRegistros()
    {
        var sqlSelecionarTodos = ObterSqlSelecionarTodos();

        var comandoSelecao = conexaoComBanco.CreateCommand();
        comandoSelecao.CommandText = sqlSelecionarTodos;

        conexaoComBanco.Open();

        var leitor = comandoSelecao.ExecuteReader();

        var contatos = new List<T>();

        while (leitor.Read())
        {
            var contato = ConverterParaRegistro(leitor);

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
    
    public abstract void ConfigurarParametrosRegistro(IDbCommand comando, T registro);
    
    public abstract T ConverterParaRegistro(IDataReader leitor);
}
