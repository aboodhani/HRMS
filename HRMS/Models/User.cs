using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Models
{
    public class User
    {
        public long Id { get; set; }
      
        public string UserName { get; set; } // Unique 
        public string HashedPassword { get; set; }
        public bool IsAdmin { get; set; }



    }
}
