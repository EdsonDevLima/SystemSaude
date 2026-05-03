using System.ComponentModel.DataAnnotations;

namespace SystemSaude.Entities
{
    public class Address
    {
        [Key]
        public Guid id{get; set;}
        [Required(ErrorMessage = "Rua obrigatoria")]
        public string street{get; set;} = string.Empty;
        [Required(ErrorMessage = "Bairro obrigatorio")]
        public string neighborhood{get;set;} = string.Empty;
        [Required(ErrorMessage = "Estado obrigatorio")]
        public string state{get;set;} = string.Empty;
        [Required(ErrorMessage = "Pais obrigatorio")]
        public string country{get;set;} = string.Empty;
        [Required(ErrorMessage = "Complemento obrigatorio")]
        public string complement{get;set;} = string.Empty;
        public Pacient Pacient { get; set; } = null!;
    }
}
