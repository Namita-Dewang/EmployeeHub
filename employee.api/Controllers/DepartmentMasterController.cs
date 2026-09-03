using employee.api.model;
using Microsoft.AspNetCore.Mvc;

namespace employee.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentMasterController : ControllerBase
    {
        private readonly employeeDbContext _context;
        public DepartmentMasterController(employeeDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAllDepartments")]
        public IActionResult GetAllDepartments()
        {
           var depList = _context.departments.ToList();
           return Ok(depList);
         }

        [HttpPost("AddDepartment")]
        public IActionResult AddDepartment([FromBody] department dept)
        {
            _context.departments.Add(dept);
            _context.SaveChanges();
            return Created("department added succesfully", dept);
        }

        [HttpPut("UpdateDepartment")]
        public IActionResult UpdateDepartment([FromBody] department dept)
        {
            var existingDept = _context.departments.Find(dept.departmentId);

            if (existingDept == null)
            {
                return NotFound("Department not found");
            }

            // Check if department name already exists
            var duplicateName = _context.departments
                .Any(d => d.departmentName == dept.departmentName
                      && d.departmentId != dept.departmentId);

            if (duplicateName)
            {
                return BadRequest("Department name already exists");
            }

            existingDept.departmentName = dept.departmentName;
            existingDept.isActive = dept.isActive;

            _context.SaveChanges();

            return Ok("Department updated successfully");
        }

        [HttpDelete("DeleteDepartment/{id}")]
        public IActionResult DeleteDepartment(int id)
        {
            var dept = _context.departments.Find(id);
            if(dept == null)
            {
                return NotFound("Department Not Found");
            }
            _context.departments.Remove(dept);
            _context.SaveChanges();
            return Ok("Department Deleted Successfully");
        }
    }
}
