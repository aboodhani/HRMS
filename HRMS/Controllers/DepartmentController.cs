using HRMS.DbContexts;
using HRMS.Dtos.Department;
using HRMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly HRMSContext _dbcontext;

        public DepartmentController(HRMSContext hRMSContext)
        {
            _dbcontext = hRMSContext;
        }

        // =========================
        // GET ALL
        // =========================
        [HttpGet]
        public IActionResult GetAll([FromQuery] FilterDepartmentDto filterDepartmentDto)
        {
            try
            {
                var result = (
                    from dep in _dbcontext.Departments
                    where
                        (filterDepartmentDto.Name == null ||
                         dep.Name.ToUpper().Contains(filterDepartmentDto.Name.ToUpper()))
                        &&
                        (filterDepartmentDto.FloorNumber == null ||
                         dep.FloorNumber == filterDepartmentDto.FloorNumber)
                    select new DepartmentDto
                    {
                        Id = dep.Id,
                        Name = dep.Name,
                        Description = dep.Description,
                        FloorNumber = dep.FloorNumber
                    }
                ).ToList(); // ✅ FIX: force execution

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // =========================
        // GET BY ID
        // =========================
        [HttpGet("GetById/{id}")]
        public IActionResult GetById(long id)
        {
            try
            {
                var department = _dbcontext.Departments
                    .Select(x => new DepartmentDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        FloorNumber = x.FloorNumber
                    })
                    .FirstOrDefault(x => x.Id == id);

                if (department == null)
                {
                    return NotFound("error");
                }

                return Ok(department);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // =========================
        // ADD
        // =========================
        [Authorize(Roles = "HR,Admin")]
        [HttpPost("Add")]
        public IActionResult Add([FromBody] SaveDepartmentDto saveDto)
        {
            var dep = new Department
            {
                Id = 0,
                FloorNumber = saveDto.FloorNumber,
                Description = saveDto.Description,
                Name = saveDto.Name
            };

            _dbcontext.Departments.Add(dep);
            _dbcontext.SaveChanges();

            return Ok();
        }

        // =========================
        // UPDATE
        // =========================
        [Authorize(Roles = "HR,Admin")]
        [HttpPut("update")]
        public IActionResult update([FromBody] UpdateDepartmentDto saveDto)
        {
            try
            {
                var department = _dbcontext.Departments
                    .FirstOrDefault(x => x.Id == saveDto.Id);

                if (department == null)
                {
                    return NotFound("department doesn't exist");
                }

                department.Name = saveDto.Name;
                department.Description = saveDto.Description;
                department.FloorNumber = saveDto.FloorNumber;

                _dbcontext.SaveChanges(); // ✅ FIX: persist update

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // =========================
        // DELETE
        // =========================
        [Authorize(Roles = "HR,Admin")]
        [HttpDelete("Delete")]
        public IActionResult Delete(long id)
        {
            try
            {
                var dep = _dbcontext.Departments
                    .FirstOrDefault(x => x.Id == id);

                if (dep == null)
                {
                    return NotFound("department doesn't exist");
                }

                _dbcontext.Departments.Remove(dep);
                _dbcontext.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
