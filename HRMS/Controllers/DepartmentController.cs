using HRMS.Dtos.Department;
using HRMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace HRMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {

        public List<Department> departments = new List<Department>()
        {
        new Department() { Id = 1 , Name = "IT" , Description = "It consultaion", FloorNumber = 5},
        new Department() { Id = 2 , Name = "Services" , Description = "Services consultaion", FloorNumber = 4},
        new Department() { Id = 3 , Name = "HR" , Description = "HR consultaion", FloorNumber = 2},
        new Department() { Id = 4 , Name = "Qa" , Description = " Quality assurance" , FloorNumber = 1}
        };

        [HttpGet]
        public IActionResult GetAll([FromQuery] FilterDepartmentDto filterDepartmentDto)
        {
            var result = from dep in departments
                         where (filterDepartmentDto.Name == null || dep.Name.ToUpper().Contains(filterDepartmentDto.Name.ToUpper()))
                         && (filterDepartmentDto.FloorNumber == null || dep.FloorNumber == filterDepartmentDto.FloorNumber)
                         select new DepartmentDto { Id = dep.Id, Name = dep.Name, Description = dep.Description, FloorNumber = dep.FloorNumber };
            return Ok(result);
        }

        
        [HttpGet("GetById/{id}")] // route parameter 
        public IActionResult GetById(long id)
        {
            var department = departments.Select(x => new DepartmentDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                FloorNumber = x.FloorNumber
            }).FirstOrDefault(x => x.Id == id);

            if (department == null)
            {
                return NotFound("error");
            }    
            return Ok(department);

        }

        [HttpPost("Add")]
        public IActionResult Add ([FromBody] SaveDepartmentDto saveDto)
        {
            var dep = new Department
            {
                Id = (departments.LastOrDefault()?.Id ?? 0) + 1,
                FloorNumber = saveDto.FloorNumber,
                Description = saveDto.Description,
                Name = saveDto.Name
            };
            departments.Add(dep);
            return Ok();
            
        }
        [HttpPut("update")]
        public IActionResult update ([FromBody] SaveDepartmentDto saveDto)
        {
            var department = departments.FirstOrDefault(x => x.Id == saveDto.Id);
            if (department == null )
            {
                return NotFound("department doesn't exist"); 
            }
            department.Name = saveDto.Name;
            department.Description = saveDto.Description;
            department.FloorNumber = saveDto.FloorNumber;
            return Ok();
        }

        [HttpDelete("Delete")]
        public IActionResult Delete (long id)

        {
            var dep = departments.FirstOrDefault(x => x.Id == id);
            if (dep == null)
            {
                return NotFound("department doesn't exist");
            }
            departments.Remove(dep);
            return Ok();
        }

       
    }


}

