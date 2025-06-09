using ControleDeBar.Dominio.Compartilhado;

namespace ControleDeBar.Dominio.ModuloGarcom;

public class Garcom : EntidadeBase<Garcom>
{
    public string Nome { get; set; }
    public string CPF { get; set; }

    public Garcom() { }
    
    public Garcom(string nome, string cpf) : this()
    {
        Id = Guid.NewGuid();
        Nome = nome;
        CPF = cpf;
    }

    public override void AtualizarRegistro(Garcom registroEditado)
    {
        Nome = registroEditado.Nome;
        CPF = registroEditado.CPF;
    }
}
