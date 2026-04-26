using System.ComponentModel.DataAnnotations;

namespace SystemSaude.Entities
{
    public class Pacient
    {
        [Key]
        public Guid Id {get; set;}
        [Required(ErrorMessage ="Email Obrigatorio")]
        public string Email {get; set;}
        [Required(ErrorMessage ="Cpf Obrigatorio")]
        public string Cpf  {get; set;}
        [Required(ErrorMessage ="Documento Obrigatorio")]
        public string Document  {get; set;}
        [Required(ErrorMessage ="Numero Obrigatorio")]
        public string Phone  {get; set;}
        [Required(ErrorMessage ="Endereço")]
        public Address Address {get; set;}

    }
}