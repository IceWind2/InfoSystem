using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfoSystem
{
    [Table("medicine")]
    public class Medicine
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {  get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Patient>? Patients{ get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
