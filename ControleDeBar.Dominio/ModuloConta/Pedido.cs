using ControleDeBar.Dominio.ModuloProduto;

namespace ControleDeBar.Dominio.ModuloConta;

public class Pedido
{
    public Guid Id { get; set; }
    public Produto Produto { get; set; }
    public int Quantidade { get; set; }

    public Pedido() { }

    public Pedido(Produto produto, int quantidade) : this()
    {
        Id = Guid.NewGuid();
        Produto = produto;
        Quantidade = quantidade;
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
