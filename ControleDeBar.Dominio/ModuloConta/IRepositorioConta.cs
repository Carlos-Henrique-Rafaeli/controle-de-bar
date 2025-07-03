namespace ControleDeBar.Dominio.ModuloConta;

public interface IRepositorioConta
{
    void CadastrarConta(Conta conta);
    public bool AtualizarConta(Conta conta, Guid idConta);
    Conta? SelecionarPorId(Guid idRegistro);
    public void AdicionarPedido(Pedido pedido);
    public bool RemoverPedido(Pedido pedido);
    List<Conta> SelecionarContas();
    List<Conta> SelecionarContasAbertas();
    List<Conta> SelecionarContasFechadas();
}
