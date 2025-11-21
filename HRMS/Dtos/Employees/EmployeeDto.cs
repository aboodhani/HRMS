namespace HRMS.Dtos.Employees
{
    public class EmployeeDto
    {
        // DTO : Data Transfer Object 
        public long? Id { get; set; }
        
        public string? Name { get; set; }
        public string? Email { get; set; } // optional / nullable 
        public long? PositionId { get; set; }
        public string? PositionName { get; set; }
        public DateTime? BirthDate { get; set; }
        public decimal? Salary { get;  set; }
        public long? ManagerId { get;  set; }
        public long? DepartmentId { get;  set; }
        public string? DepartmentName { get;  set; }
        public string? ManagerName { get;  set; }
    }
    }

