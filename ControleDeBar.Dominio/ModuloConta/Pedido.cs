using ControleDeBar.Dominio.ModuloProduto;

namespace ControleDeBar.Dominio.ModuloConta;

public class Pedido
{
    public Guid Id { get; set; }
    public Produto Produto { get; set; }
    public int Quantidade { get; set; }
    public Conta Conta { get; set; }

    public Pedido() { }

    public Pedido(Produto produto, int quantidade, Conta conta) : this()
    {
        Id = Guid.NewGuid();
        Produto = produto;
        Quantidade = quantidade;
        Conta = conta;
    }

    public decimal CalcularTotalParcial()
    {
        return Produto.Valor * Quantidade;
    }

    public override string ToString()
    {
        return $"{Quantidade}x {Produto}";
    }
}
