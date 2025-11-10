using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Vistoria_projeto.Models
{
    public class Vistoria
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O responsável é obrigatório")]
        public string Responsavel { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data é obrigatória")]
        [Display(Name = "Data da Vistoria")]
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "O horário é obrigatório")]
        public string Horario { get; set; } = string.Empty;

        [Required(ErrorMessage = "O imóvel é obrigatório")]
        public string Imovel { get; set; } = string.Empty;

        [Display(Name = "Status")]
        public string Status { get; set; } = "Agendada";
    }
}

