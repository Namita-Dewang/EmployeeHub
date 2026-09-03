using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using employee.api.model;

namespace employee.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignationMasterController : ControllerBase
    {
        private readonly employeeDbContext _context;

        public DesignationMasterController(employeeDbContext context)
        {
            _context = context;
        }

        // GET: api/DesignationMaster/GetDesignations
        [HttpGet("GetDesignations")]
        public IActionResult GetDesignations()
        {
            try
            {
                var designationList = _context.designations.ToList();

                return Ok(designationList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error while fetching designations: " + ex.Message);
            }
        }


        // GET: api/DesignationMaster/GetDesignation/1
        [HttpGet("GetDesignation/{id}")]
        public IActionResult GetDesignation(int id)
        {
            try
            {
                var designation = _context.designations.Find(id);

                if (designation == null)
                {
                    return NotFound("Designation not found");
                }

                return Ok(designation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error while fetching designation: " + ex.Message);
            }
        }


        // POST: api/DesignationMaster/AddDesignation
        [HttpPost("AddDesignation")]
        public IActionResult AddDesignation([FromBody] designation desg)
        {
            try
            {
                // Model validation
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check duplicate designation name
                var existingDesignation = _context.designations
                    .Any(d => d.designationName == desg.designationName);

                if (existingDesignation)
                {
                    return BadRequest("Designation name already exists");
                }

                _context.designations.Add(desg);
                _context.SaveChanges();

                return Ok("Designation added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error while adding designation: " + ex.Message);
            }
        }


        // PUT: api/DesignationMaster/UpdateDesignation
        [HttpPut("UpdateDesignation")]
        public IActionResult UpdateDesignation([FromBody] designation desg)
        {
            try
            {
                // Model validation
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingDesignation = _context.designations
                    .Find(desg.designationId);

                if (existingDesignation == null)
                {
                    return NotFound("Designation not found");
                }

                // Check duplicate designation name
                // Exclude current designation
                var duplicateDesignation = _context.designations
                    .Any(d => d.designationName == desg.designationName
                           && d.designationId != desg.designationId);

                if (duplicateDesignation)
                {
                    return BadRequest("Designation name already exists");
                }

                existingDesignation.departmentId = desg.departmentId;
                existingDesignation.designationName = desg.designationName;

                _context.SaveChanges();

                return Ok("Designation updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error while updating designation: " + ex.Message);
            }
        }


        // DELETE: api/DesignationMaster/DeleteDesignation/1
        [HttpDelete("DeleteDesignation/{id}")]
        public IActionResult DeleteDesignation(int id)
        {
            try
            {
                var designation = _context.designations.Find(id);

                if (designation == null)
                {
                    return NotFound("Designation not found");
                }

                _context.designations.Remove(designation);
                _context.SaveChanges();

                return Ok("Designation deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error while deleting designation: " + ex.Message);
            }
        }


        // GET: api/DesignationMaster/FilterDesignations?departmentId=1&designationName=Manager
        [HttpGet("FilterDesignations")]
        public IActionResult FilterDesignations(
            int? departmentId,
            string? designationName)
        {
            try
            {
                var query = _context.designations.AsQueryable();

                if (departmentId.HasValue)
                {
                    query = query.Where(d =>
                        d.departmentId == departmentId.Value);
                }

                if (!string.IsNullOrEmpty(designationName))
                {
                    query = query.Where(d =>
                        d.designationName.Contains(designationName));
                }

                var result = query.ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error while filtering designations: " + ex.Message);
            }
        }
    }
}