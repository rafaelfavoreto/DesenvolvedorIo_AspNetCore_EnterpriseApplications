using NerdStore.Core.DomainObjects;

namespace NerdStore.Catalago.Domain
{
    public class Categoria : Entity
    { 
        public string Nome { get; private set; }
        public int Codigo { get; private set; }

        public Categoria(string nome, int codigo)
        {
            Nome = nome;
            Codigo = codigo;

            Validar();
        }

        // EF Relation
        public ICollection<Produto> Produtos { get; set; }

        protected Categoria() { }
        public override string ToString()
        {
            return $"{Nome} - {Codigo}";
        }

        public void Validar()
        {
            Validacoes.ValidarSeVazio(Nome, "O campo Nome da categoria não pode estar vazio");
            Validacoes.ValidarSeIgual(Codigo, 0, "O campo Codigo não pode ser 0");
        }
    }
}
