using System.ComponentModel.DataAnnotations;

namespace Vistoria_projeto.Models
{
    public class Imovel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do imóvel é obrigatório")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O endereço é obrigatório")]
        public string Endereco { get; set; } = string.Empty;
    }
}

