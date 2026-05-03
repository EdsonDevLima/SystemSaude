using System.ComponentModel.DataAnnotations;

namespace SystemSaude.Entities
{
    public class Pacient
    {
        [Key]
        public Guid Id {get; set;}
        [Required(ErrorMessage ="Email Obrigatorio")]
        public string Email {get; set;} = string.Empty;
        [Required(ErrorMessage ="Cpf Obrigatorio")]
        public string Cpf  {get; set;} = string.Empty;
        [Required(ErrorMessage ="Documento Obrigatorio")]
        public string Document  {get; set;} = string.Empty;
        [Required(ErrorMessage ="Numero Obrigatorio")]
        public string Phone  {get; set;} = string.Empty;
        [Required(ErrorMessage ="Endereço")]
        public Address Address {get; set;} = null!;
        public ICollection<Consult> Consults { get; set; } = new List<Consult>();

    }
}
