using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Models
{
    public class Employee
    {
        [Key]
        public long Id { get; set; }


        [MaxLength(50)]
        public string FirstName { get; set; }
        

        [MaxLength(50)]
        public string LastName { get; set; }


        [MaxLength(100)]
        public string? Email { get; set; } // optional / nullable 


        public DateTime? BirthDate { get; set; }
        public decimal Salary { get; set; }



        [ForeignKey ("Lookup") ]
        public long PositionId { get; set;}
        public Lookup Lookup { get; set; } // navigation property

          

        [ForeignKey("Department")]
        public long? DepartmentId { get; set; }
        public Department? Department { get; set; } // navigation property 



        [ForeignKey("manager")]
        public long? ManagerId { get; set; }
        public Employee? manager { get; set; } // navigation property 

    }
}
