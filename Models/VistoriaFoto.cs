using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vistoria_projeto.Models
{
    public class VistoriaFoto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Caminho da Imagem")]
        public string Caminho { get; set; } = string.Empty;

        [ForeignKey("Vistoria")]
        public int VistoriaId { get; set; }

        public Vistoria? Vistoria { get; set; }
    }
}

