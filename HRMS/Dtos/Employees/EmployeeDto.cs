namespace HRMS.Dtos.Employees
{
    public class EmployeeDto
    {
        // DTO : Data Transfer Object 
        public long? Id { get; set; }
        
        public string? Name { get; set; }
        public string? Email { get; set; } // optional / nullable 
        public string? Position { get; set; }
        public DateTime? BirthDate { get; set; }
    }
    }

