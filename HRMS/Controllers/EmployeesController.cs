using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HRMS.Models;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using HRMS.Dtos.Employees;
using Microsoft.AspNetCore.Http.HttpResults;
namespace HRMS.Controllers
{
    [Route("api/[controller]")] // Data Annotation   
    [ApiController] // Data Annotation
    public class EmployeesController : ControllerBase
    {
        public static List<Employee> emplyoees = new List<Employee>()
        {

        new Employee() {Id=1 ,FirstName = "Ahmad"  ,LastName = "salameh" ,Email="aboodhani373@gmail.com",Position = "Developer", BirthDate = new DateTime(2000,1,25)},
        new Employee() {Id=2 ,FirstName = "layla"  ,LastName = "attyat"  ,Email="ahmdsamr@gmail.com"    , Position = "Developer", BirthDate = new DateTime(2004,5,12)},
        new Employee() {Id=3 ,FirstName = "saleem" ,LastName = "tarifi"  , Position = "HR",BirthDate = new DateTime(1999,1,30)},
        new Employee() {Id=4 ,FirstName = "omar"   ,LastName = "alqady"  , Position = "QA", BirthDate = new DateTime(2005,8,24)},
        new Employee() {Id=5 ,FirstName = "abood"  ,LastName = "salahat" , Position = "Backend", BirthDate = new DateTime(1989,12,31)},

        };

        // CRUD operation 

        [HttpGet("GetByCriteria")] // (data annotation) Method --> api Endpoint
        public IActionResult GetByCriteria ( [FromQuery] SerchEmployeeDto employeeDto) // this is called query parameter
        {
            var result = from employee in emplyoees
                         where (employeeDto.Position == null || employee.Position.ToUpper().Contains(employeeDto.Position.ToUpper()) ) &&
                                (employeeDto.Name == null || employee.FirstName.ToUpper().Contains(employeeDto.Name.ToUpper())) // to upper to avoid the uppercase edges and Contains for typing an instance of the word and find it 
                         orderby employee.Id descending
                         select new EmployeeDto
                         {
                             Id = employee.Id,
                             Name = employee.FirstName + " " + employee.LastName,
                             Position = employee.Position,
                             BirthDate = employee.BirthDate,
                             Email = employee.Email
                         };
            return Ok(result);
        }

        [HttpGet("GetById/{id}")] // Route parameter 
        public IActionResult GetById(long Id)
        {
            if (Id == 0)
            {
                return BadRequest("Id Value is invalid");
            }

            var result = emplyoees.Select(x => new EmployeeDto
            {
                Id = x.Id,
                Name = x.FirstName + " " + x.LastName,
                Position = x.Position,
                BirthDate = x.BirthDate,
                Email = x.Email
            }).FirstOrDefault(x => x.Id == Id); // the arrow => is WHERE 

            if (result == null)
            {
                return NotFound("Employee is not found");
            }

            return Ok(result);
        }



        [HttpPost("Add")] // in post methods i don't need the id from the user (i will control the id and not the user )
        public IActionResult Add([FromBody] SaveEmployeeDto employeeDto) // we have to make another Dto for the post process because we shouldn't access the model at any point from the controller 
        {
            var emp = new Employee() // here we make an object from the model because the list is from type Employee and we put in it the Dto details 
            {
                Id = (emplyoees.LastOrDefault()?.Id ?? 0) + 1,
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Email = employeeDto.Email,
                BirthDate = employeeDto.BirthDate,
                Position = employeeDto.Position
            };
            emplyoees.Add(emp); // here we added the emp (from type model) but the info from the dto and add it to the list 

            return Ok();
        }

        [HttpPut("Edit")]
        public IActionResult Edit([FromBody] SaveEmployeeDto employeeDto) // same parameter as the post method
        {
            var emplyoee = emplyoees.FirstOrDefault(x => x.Id == employeeDto.Id); // get the id to update its information 

            if (emplyoee == null)                                                  
                return NotFound("employee doesn't exist");

            emplyoee.FirstName = employeeDto.FirstName;
            emplyoee.LastName = employeeDto.LastName;
            emplyoee.Email = employeeDto.Email;
            emplyoee.BirthDate = employeeDto.BirthDate;
            emplyoee.Position = employeeDto.Position;
            return Ok("Your information has been successfully updated");

        }

        [HttpDelete("Delete")]
        public IActionResult Delete( [FromBody] int id)
        {
            var employee = emplyoees.FirstOrDefault(x => x.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            emplyoees.Remove(employee);
            
            return Ok();
        }



    }

}
