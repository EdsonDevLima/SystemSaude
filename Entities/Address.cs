using System.ComponentModel.DataAnnotations;

namespace SystemSaude.Entities
{
    public class Address
    {
        [Key]
        public Guid id{get; set;}
        [Required(ErrorMessage = "Rua obrigatoria")]
        public string street{get; set;}
        [Required(ErrorMessage = "Bairro obrigatorio")]
        public string neighborhood{get;set;}
        [Required(ErrorMessage = "Estado obrigatorio")]
        public string state{get;set;}
        [Required(ErrorMessage = "Pais obrigatorio")]
        public string country{get;set;}
        [Required(ErrorMessage = "Complemento obrigatorio")]
        public string complement{get;set;}
    }
}