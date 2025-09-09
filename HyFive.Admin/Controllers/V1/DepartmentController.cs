using System.Collections.Generic;
using HyFive.Models.V1.Institution;
using HyFive.Services.Department;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HyFive.Models.V1.Observation;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Roles;
using System;
using CsvHelper.Configuration.Attributes;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdminOrCoordinator)]
    [Route("api/v1/department")]
    public class DepartmentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _userService;

        public DepartmentController(IMediator mediator, IUserService userService)
        {
            _mediator = mediator;
            _userService = userService;
        }

        /// <summary>
        /// Get department
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetDepartment")]
        [ProducesResponseType(typeof(Department), StatusCodes.Status200OK)]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            if (_userService.IsCoordinatorForDepartmentOrFhiAdmin(id))
            {
                return await _mediator.Send(new GetDepartment.Query() { Id = id });
            }
            return Unauthorized();

        }

        /// <summary>
        /// Create department with roles
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("create")]
        [ProducesResponseType(typeof(Department), StatusCodes.Status201Created)]
        public async Task<ActionResult<Department>> CreateDepartment([FromBody] CreateDepartmentRequest request)
        {
            if (_userService.IsCoordinatorForHealthcareProviderOrFhiAdmin(request.InstitutionId))
            {
                var result = await _mediator.Send(new CreateDepartment.Command() { Request = request });
                return CreatedAtRoute("GetDepartment", new { id = result.InstitutionId }, result);
            }
            return Unauthorized();
        }

        /// <summary>
        /// Update department 
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        [HttpPut("update")]
        public async Task<ActionResult<Department>> UpdateDepartment([FromBody] Department department)
        {
            if (_userService.IsCoordinatorForHealthcareProviderOrFhiAdmin(department.InstitutionId))
            {
                var result = await _mediator.Send(new UpdateDepartment.Command()
                {
                    Id = department.Id,
                    DepartmentTypeId = department.DepartmentTypeId,
                    Name = department.Name,
                    Role = department.Roles
                });
                return Ok(result);
            }
            return Unauthorized();
        }

        [HttpGet("departmentTypes")]
        public async Task<ActionResult<List<DepartmentType>>> GetDepartmentTypes()
        {
            var result = await _mediator.Send(new GetDepartmentTypes.Query() { });
            return Ok(result);
        }

        /// <summary>
        /// Create department type
        /// </summary>
        /// <param name="departmentType"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPost("departmentType/create")]
        [ProducesResponseType(typeof(DepartmentType), StatusCodes.Status201Created)]
        public async Task<ActionResult<DepartmentType>> CreateDepartmentType([FromBody] DepartmentType departmentType)
        {
            try
            {
                var result = await _mediator.Send(new CreateDepartmentType.Command() { DepartmentType = departmentType });

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Update department type
        /// </summary>
        /// <param name="departmentType"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdmin)]
        [HttpPut("departmentTypes/update")]
        [ProducesResponseType(typeof(DepartmentType), StatusCodes.Status200OK)]
        public async Task<ActionResult<DepartmentType>> UpdateDepartmentType([FromBody] DepartmentType departmentType)
        {
            var result = await _mediator.Send(new UpdateDepartmentType.Command() { DepartmentType = departmentType });
            return Ok(result);
        }

        /// <summary>
        /// Get Roles
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/roles")]
        [ProducesResponseType(typeof(Role), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Role>>> GetRoles(int id)
        {
            if (_userService.IsCoordinatorForDepartmentOrFhiAdmin(id))
            {
                return await _mediator.Send(new GetRolesForDepartment.Query { DepartmentId = id });
            }
            return Unauthorized();

        }


        /// <summary>
        /// Delete department
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.FhiAdminOrCoordinator)]
        [HttpDelete("delete/{id}")]
        public async Task<bool> DeleteDepartment(int id)
        {
            var result = await _mediator.Send(new DeleteDepartment.Command()
            {
                DepartmentId = id
            });
            return result;
        }

        /// <summary>
        /// Checking if the department has sessions transferred to FHI.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("hasTransferredSessionToFHI/{id}")]
        public async Task<IActionResult> HasTransferredSessionToFHI(int id)
        {

            var result = await _mediator.Send(new HasTransferredSessionToAdmin.Command
            {
                DepartmentId = id
            });

            return Ok(result);
        }


    }
}
