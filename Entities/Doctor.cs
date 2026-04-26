using System.ComponentModel.DataAnnotations;

namespace SystemSaude.Entities
{
    public class Doctor
    {
        [Key]
        public Guid Id {get; set;}
        [Required(ErrorMessage = "Nome obrigatorio")]
        public string Name {get; set;}
        [Required(ErrorMessage = "Especialidade obrigatoria")]        
        public string Speciality {get; set;}
        [Required(ErrorMessage = "Status obrigatorio")]
        public bool Status {get; set;}
        [Required(ErrorMessage = "Licença obrigatoria")]
        public string LicenseNumber{get;set;}
        [Required(ErrorMessage = "Numero obrigatorio")]
        public string Phone{get;set;}

    }
}