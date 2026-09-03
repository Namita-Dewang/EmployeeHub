using employee.api.model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace employee.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeMasterController : ControllerBase
    {
        private readonly employeeDbContext _context;

        public EmployeeMasterController(employeeDbContext context)
        {
            _context = context;
        }


        // =========================================================
        // GET ALL EMPLOYEES
        // =========================================================

        [HttpGet("GetEmployees")]
        public async Task<IActionResult> GetEmployees()
        {
            try
            {
                var employees = await _context.employees
                    .AsNoTracking()
                    .ToListAsync();

                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    "Error while fetching employees: " + ex.Message);
            }
        }


        // =========================================================
        // GET EMPLOYEE BY ID
        // =========================================================

        [HttpGet("GetEmployee/{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid employee ID");
                }

                var employee = await _context.employees
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.employeeId == id);

                if (employee == null)
                {
                    return NotFound("Employee not found");
                }

                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    "Error while fetching employee: " + ex.Message);
            }
        }


        // =========================================================
        // ADD EMPLOYEE
        // =========================================================

        [HttpPost("AddEmployee")]
        public async Task<IActionResult> AddEmployee(
            [FromBody] employeemodel emp)
        {
            try
            {
                // Model validation
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check duplicate email
                bool emailExists = await _context.employees
                    .AnyAsync(e => e.email == emp.email);

                if (emailExists)
                {
                    return BadRequest("Email already exists");
                }

                // Check duplicate contact
                bool contactExists = await _context.employees
                    .AnyAsync(e => e.contact == emp.contact);

                if (contactExists)
                {
                    return BadRequest("Contact already exists");
                }

                // Server controlled date
                emp.createdDate = DateTime.Now;

                _context.employees.Add(emp);

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Employee added successfully",
                    employeeId = emp.employeeId
                });
            }
            catch (DbUpdateException)
            {
                return StatusCode(
                    500,
                    "Database error while adding employee");
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    "Error while adding employee: " + ex.Message);
            }
        }


        // =========================================================
        // UPDATE EMPLOYEE
        // =========================================================

        [HttpPut("UpdateEmployee")]
        public async Task<IActionResult> UpdateEmployee(
            [FromBody] employeemodel emp)
        {
            try
            {
                // Model validation
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (emp.employeeId <= 0)
                {
                    return BadRequest("Invalid employee ID");
                }

                // Find existing employee
                var existingEmployee = await _context.employees
                    .FirstOrDefaultAsync(
                        e => e.employeeId == emp.employeeId);

                if (existingEmployee == null)
                {
                    return NotFound("Employee not found");
                }

                // Check duplicate email
                // Exclude current employee
                bool emailExists = await _context.employees
                    .AnyAsync(e =>
                        e.email == emp.email &&
                        e.employeeId != emp.employeeId);

                if (emailExists)
                {
                    return BadRequest("Email already exists");
                }

                // Check duplicate contact
                // Exclude current employee
                bool contactExists = await _context.employees
                    .AnyAsync(e =>
                        e.contact == emp.contact &&
                        e.employeeId != emp.employeeId);

                if (contactExists)
                {
                    return BadRequest("Contact already exists");
                }

                // Update fields
                existingEmployee.name = emp.name;
                existingEmployee.contact = emp.contact;
                existingEmployee.email = emp.email;
                existingEmployee.city = emp.city;
                existingEmployee.state = emp.state;
                existingEmployee.pincode = emp.pincode;
                existingEmployee.altContact = emp.altContact;
                existingEmployee.address = emp.address;
                existingEmployee.designationId = emp.designationId;

                // Server controlled date
                existingEmployee.modifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok("Employee updated successfully");
            }
            catch (DbUpdateException)
            {
                return StatusCode(
                    500,
                    "Database error while updating employee");
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    "Error while updating employee: " + ex.Message);
            }
        }


        // =========================================================
        // DELETE EMPLOYEE
        // =========================================================

        [HttpDelete("DeleteEmployee/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid employee ID");
                }

                var employee = await _context.employees
                    .FirstOrDefaultAsync(
                        e => e.employeeId == id);

                if (employee == null)
                {
                    return NotFound("Employee not found");
                }

                _context.employees.Remove(employee);

                await _context.SaveChangesAsync();

                return Ok("Employee deleted successfully");
            }
            catch (DbUpdateException)
            {
                return StatusCode(
                    500,
                    "Employee cannot be deleted because it is being used by another record");
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    "Error while deleting employee: " + ex.Message);
            }
        }


        // =========================================================
        // FILTER + SORT + PAGINATION
        // =========================================================

        [HttpGet("FilterEmployees")]
        public async Task<IActionResult> FilterEmployees(
            string? name,
            string? email,
            string? contact,
            string? city,
            string? state,
            string? pincode,
            int? designationId,
            string? sortBy = "employeeId",
            string? sortOrder = "asc",
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                // =================================================
                // PAGINATION VALIDATION
                // =================================================

                if (pageNumber < 1)
                {
                    return BadRequest(
                        "Page number must be greater than 0");
                }

                if (pageSize < 1 || pageSize > 100)
                {
                    return BadRequest(
                        "Page size must be between 1 and 100");
                }


                // =================================================
                // BASE QUERY
                // =================================================

                var query = _context.employees
                    .AsNoTracking()
                    .AsQueryable();


                // =================================================
                // FILTERING
                // =================================================

                if (!string.IsNullOrWhiteSpace(name))
                {
                    query = query.Where(e =>
                        e.name.Contains(name));
                }

                if (!string.IsNullOrWhiteSpace(email))
                {
                    query = query.Where(e =>
                        e.email.Contains(email));
                }

                if (!string.IsNullOrWhiteSpace(contact))
                {
                    query = query.Where(e =>
                        e.contact.Contains(contact));
                }

                if (!string.IsNullOrWhiteSpace(city))
                {
                    query = query.Where(e =>
                        e.city.Contains(city));
                }

                if (!string.IsNullOrWhiteSpace(state))
                {
                    query = query.Where(e =>
                        e.state.Contains(state));
                }

                if (!string.IsNullOrWhiteSpace(pincode))
                {
                    query = query.Where(e =>
                        e.pincode.Contains(pincode));
                }

                if (designationId.HasValue)
                {
                    query = query.Where(e =>
                        e.designationId == designationId.Value);
                }


                // =================================================
                // SORTING
                // =================================================

                sortBy = sortBy?.Trim().ToLower();
                sortOrder = sortOrder?.Trim().ToLower();

                bool descending = sortOrder == "desc";

                switch (sortBy)
                {
                    case "name":

                        query = descending
                            ? query.OrderByDescending(e => e.name)
                            : query.OrderBy(e => e.name);

                        break;


                    case "email":

                        query = descending
                            ? query.OrderByDescending(e => e.email)
                            : query.OrderBy(e => e.email);

                        break;


                    case "contact":

                        query = descending
                            ? query.OrderByDescending(e => e.contact)
                            : query.OrderBy(e => e.contact);

                        break;


                    case "city":

                        query = descending
                            ? query.OrderByDescending(e => e.city)
                            : query.OrderBy(e => e.city);

                        break;


                    case "state":

                        query = descending
                            ? query.OrderByDescending(e => e.state)
                            : query.OrderBy(e => e.state);

                        break;


                    case "pincode":

                        query = descending
                            ? query.OrderByDescending(e => e.pincode)
                            : query.OrderBy(e => e.pincode);

                        break;


                    case "createddate":

                        query = descending
                            ? query.OrderByDescending(e => e.createdDate)
                            : query.OrderBy(e => e.createdDate);

                        break;


                    case "modifieddate":

                        query = descending
                            ? query.OrderByDescending(e => e.modifiedDate)
                            : query.OrderBy(e => e.modifiedDate);

                        break;


                    case "employeeid":
                    case null:
                    case "":

                        query = descending
                            ? query.OrderByDescending(e => e.employeeId)
                            : query.OrderBy(e => e.employeeId);

                        break;


                    default:

                        return BadRequest(
                            "Invalid sort field. " +
                            "Allowed values: employeeId, name, email, " +
                            "contact, city, state, pincode, createdDate, modifiedDate");
                }


                // =================================================
                // TOTAL RECORDS
                // =================================================

                var totalRecords = await query.CountAsync();


                // =================================================
                // PAGINATION
                // =================================================

                var employees = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();


                // =================================================
                // TOTAL PAGES
                // =================================================

                var totalPages = (int)Math.Ceiling(
                    (double)totalRecords / pageSize);


                // =================================================
                // RESPONSE
                // =================================================

                return Ok(new
                {
                    totalRecords,
                    pageNumber,
                    pageSize,
                    totalPages,
                    data = employees
                });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    "Error while filtering employees: " + ex.Message);
            }
        }

        // =========================================================
        // LOGIN
        // =========================================================

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var employee = await _context.employees
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e =>
                        e.email == loginRequest.email &&
                        e.contact == loginRequest.contact);

                if (employee == null)
                {
                    return Unauthorized("Invalid email or contact");
                }

                return Ok(new
                {
                    message = "Login successful",
                    employeeId = employee.employeeId,
                    name = employee.name,
                    email = employee.email,
                    designationId = employee.designationId,
                    role = employee.role
                });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    "Error while logging in: " + ex.Message);
            }
        }
    }
}