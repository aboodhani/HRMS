using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HRMS.Models;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using HRMS.Dtos.Employees;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using HRMS.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Data;

// Nuget Package
namespace HRMS.Controllers
{

    // HTTP Get : Can not use body request [from body], We can Only use Query Parameters 
    // HTTP post/put : Can use Both Body and Query, But We will always use [from body]
    // HTTP Delete : Can use Both Body Request and Query Request but we will always use [from query]


    // anything we receive or send to the user must be from the Dto and not the model 
    [Authorize]
    [Route("api/[controller]")] // Data Annotation   
    [ApiController] // Data Annotation
    public class EmployeesController : ControllerBase
    {

        
        // Dependency injection 
        // creating a general object to pass the info to any class that request  
        private readonly HRMSContext _dbContext;

        public EmployeesController(HRMSContext dbContext)
        {
            _dbContext = dbContext;
        }



        // CRUD operation 

        [HttpGet("GetByCriteria")] // (data annotation) Method --> api Endpoint
        public IActionResult GetByCriteria ( [FromQuery] SearchEmployeeDto employeeDto) // this is called query parameter
        {
            try
            {
                // how to get the current loged in user? 
                // we get these from token 
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


                var result = from employee in _dbContext.Employees
                             from Department in _dbContext.Departments.Where(x => x.Id == employee.DepartmentId).DefaultIfEmpty() // left join 
                             from manager in _dbContext.Employees.Where(x => x.Id == employee.ManagerId).DefaultIfEmpty()
                             from lookup in _dbContext.Lookups.Where(x => x.Id == employee.PositionId) // no left join because it's required to have a positionId 
                             where
                            (employeeDto.PositionId == null || employee.PositionId == employeeDto.PositionId) &&
                             (employeeDto.Name == null || employee.FirstName.ToUpper().Contains(employeeDto.Name.ToUpper())) // to upper to avoid the uppercase edges and Contains for typing an instance of the word and find it 
                             orderby employee.Id descending
                             select new EmployeeDto
                             {
                                 Id = employee.Id,
                                 Name = employee.FirstName + " " + employee.LastName,
                                 PositionId = employee.PositionId,
                                 PositionName = lookup.Name,
                                 BirthDate = employee.BirthDate,
                                 Email = employee.Email,
                                 Salary = employee.Salary,
                                 DepartmentId = employee.DepartmentId,
                                 DepartmentName = Department.Name,
                                 ManagerId = employee.ManagerId,
                                 ManagerName = manager.FirstName,
                                 UserId = employee.UserId
                             };

                if(role?.ToUpper() != "ADMIN" && role?.ToUpper() != "HR")
                {
                    result = result.Where(x => x.UserId == long.Parse(userId));
                }







                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




        [HttpGet("GetById/{id:long}")]
        public IActionResult GetById(long id)
        {
            try
            {
                // from token 
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (id <= 0)
                    return BadRequest("Id Value is invalid");

                var result = _dbContext.Employees
                    .Where(e => e.Id == id)
                    .Select(x => new EmployeeDto // projection using select and it's the best way to get information 
                    {
                        Id = x.Id,
                        Name = x.FirstName + " " + x.LastName,
                        PositionId = x.PositionId,
                        PositionName = x.Lookup.Name, // join using the navigation property 
                        BirthDate = x.BirthDate,
                        Email = x.Email,
                        Salary = x.Salary,
                        DepartmentId = x.DepartmentId,
                        DepartmentName = x.Department.Name,
                        ManagerId = x.ManagerId,
                        ManagerName = x.manager.FirstName
                    })
                    .FirstOrDefault();



                // Eager loading using Include() wich uses join 
                var result2 = _dbContext.Employees.Include(x => x.Lookup).Include(x => x.manager).ThenInclude(x => x.Lookup).FirstOrDefault(x => x.Id.Equals(id));

                // lazy loading --> ?? 

                if (result == null)
                    return NotFound("Employee is not found");

                if (role?.ToUpper() != "ADMIN" && role?.ToUpper() != "HR")
                {
                    if (result.UserId != long.Parse(userId))
                    {
                        return Forbid();
                    }
                }
                return Ok(result2);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Roles = "HR,Admin")] // authorization (to access who can use this endpoint)
        [HttpPost("Add")] // in post methods i don't need the id from the user (i will control the id and not the user )
        public IActionResult Add([FromBody] SaveEmployeeDto employeeDto) // we have to make another Dto for the post process because we shouldn't access the model at any point from the controller 
        {
            try
            {
                // creating a user with every Employee we add to the system  
                var user = new User()
                {
                    Id = 0,
                    UserName = $"{employeeDto.FirstName}_{employeeDto.LastName}_HRMS", // Ahmad_Khalid_HRMS
                    HashedPassword = BCrypt.Net.BCrypt.HashPassword($"{employeeDto.FirstName}@123"), //AHmad@123
                    IsAdmin = false 
                }; 

                var isUserName = _dbContext.users.Any(x => x.UserName.ToUpper() == user.UserName.ToUpper());

                if(isUserName)
                {
                    return BadRequest("UserName Already Exist, PLease choose another one"); 
                }

                _dbContext.users.Add(user);
                _dbContext.SaveChanges();


                var emp = new Employee() // here we make an object from the model because the list is from type Employee and we put in it the Dto details 
                {
                    Id = 0,//(emplyoees.LastOrDefault()?.Id ?? 0) + 1,
                    FirstName = employeeDto.FirstName,
                    LastName = employeeDto.LastName,
                    Email = employeeDto.Email,
                    BirthDate = employeeDto.BirthDate,
                    PositionId = employeeDto.PositionId,
                    Salary = employeeDto.Salary,
                    DepartmentId = employeeDto.DepartmentId,
                    ManagerId = employeeDto.ManagerId,
                    // UserId = user.Id
                    User = user
                };

                _dbContext.Employees.Add(emp); // here we added the emp (from type model) but the info from the dto and add it to the list 
                _dbContext.SaveChanges(); // saving changes (commit)

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Roles = "HR,Admin")]
        [HttpPut("Edit")]
        public IActionResult Edit([FromBody] SaveEmployeeDto employeeDto) // same parameter as the post method
        {
         try
            {
                var emplyoee = _dbContext.Employees.FirstOrDefault(x => x.Id == employeeDto.Id); // get the id to update its information 

                if (emplyoee == null)
                    return NotFound("Employee Does not Exist"); // we use this if even though there is a catch because we have to protect our code from being broke as much as i can 

                emplyoee.FirstName = employeeDto.FirstName;
                emplyoee.LastName = employeeDto.LastName;
                emplyoee.Email = employeeDto.Email;
                emplyoee.BirthDate = employeeDto.BirthDate;
                emplyoee.PositionId = employeeDto.PositionId;
                emplyoee.Salary = employeeDto.Salary;
                emplyoee.DepartmentId = employeeDto.DepartmentId;
                emplyoee.ManagerId = employeeDto.ManagerId;

                _dbContext.SaveChanges();
                return Ok("Your information has been successfully updated");
            }

            catch (NullReferenceException) 
            {
                return NotFound("Employee doesn't Exist");
            }  

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




        [Authorize(Roles = "HR,Admin")]
        [HttpDelete("Delete")]
        public IActionResult Delete(long id)
        {
            try
            {
                var employee = _dbContext.Employees.FirstOrDefault(x => x.Id == id);

                if (employee == null)
                {
                    return NotFound();
                }

                _dbContext.Employees.Remove(employee);
                _dbContext.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }

}
