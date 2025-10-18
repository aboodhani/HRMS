using HRMS.Dtos.Department;
using HRMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        public List<Department> department = new List<Department>()
        { 
        new Department() { Id = 1 , Name = "IT" , Description = "It consultaion", FloorNumber = 5},
        new Department() { Id = 1 , Name = "Services" , Description = "Services consultaion", FloorNumber = 4},
        new Department() { Id = 1 , Name = "HR" , Description = "HR consultaion", FloorNumber = 2}
        };

        [HttpGet("GetByCriteria")]
        public IActionResult GetByCriteria (string? Name)
        {
            var result = from dep in department
                         where (dep.Name == Name || Name == null)
                         orderby dep.Id descending
                         select new DepartmentDto
                         {
                             Id = dep.Id,
                             Name = dep.Name,
                             Description = dep.Description,
                             FloorNumber = dep.FloorNumber,
                         };
                    return Ok(result);
            
        }
    }
}
