using System.ComponentModel.DataAnnotations;

namespace SystemSaude.Entities
{
    public class Doctor
    {
        [Key]
        public Guid Id {get; set;}
        [Required(ErrorMessage = "Nome obrigatorio")]
        public string Name {get; set;} = string.Empty;
        [Required(ErrorMessage = "Especialidade obrigatoria")]        
        public string Speciality {get; set;} = string.Empty;
        [Required(ErrorMessage = "Status obrigatorio")]
        public bool Status {get; set;}
        [Required(ErrorMessage = "Licença obrigatoria")]
        public string LicenseNumber{get;set;} = string.Empty;
        [Required(ErrorMessage = "Numero obrigatorio")]
        public string Phone{get;set;} = string.Empty;
        public ICollection<Consult> Consults { get; set; } = new List<Consult>();
        public ICollection<Schedules> Schedules { get; set; } = new List<Schedules>();

    }
}
