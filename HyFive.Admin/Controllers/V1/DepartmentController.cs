using CsvHelper.Configuration.Attributes;
using HyFive.Models.V1.Observation;
using HyFive.Models.V1.OrganisationUnit;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Authentication.User;
using HyFive.Services.Department;
using HyFive.Services.Roles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.AdminOrCoordinator)]
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
        [ProducesResponseType(typeof(OrganisationUnit), StatusCodes.Status200OK)]
        public async Task<ActionResult<OrganisationUnit>> GetDepartment(int id)
        {
            if (await _userService.IsCoordinatorForDepartmentOrAdmin(id))
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
        [ProducesResponseType(typeof(OrganisationUnit), StatusCodes.Status201Created)]
        public async Task<ActionResult<OrganisationUnit>> CreateDepartment([FromBody] CreateDepartmentRequest request)
        {
            if (await _userService.IsCoordinatorForFacilityOrAdmin(request.FacilityId))
            {
                var result = await _mediator.Send(new CreateDepartment.Command() { Request = request });
                return CreatedAtRoute("GetDepartment", new { id = result.Id }, result);
            }
            return Unauthorized();
        }

        /// <summary>
        /// Update department 
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        [HttpPut("update")]
        public async Task<ActionResult<OrganisationUnit>> UpdateDepartment([FromBody] UpdateDepartmentRequest department)
        {
            if (await _userService.IsCoordinatorForFacilityOrAdmin(department.FacilityId))
            {
                var result = await _mediator.Send(new UpdateDepartment.Command()
                {
                    Id = department.Id,
                    FacilityId = department.FacilityId,
                    OrganisationUnitTypeId = department.DepartmentTypeId,
                    Name = department.Name,
                    RoleIds = department.Roles.Select(r => r.Id).ToList()
                });
                return Ok(result);
            }
            return Unauthorized();
        }

        [HttpGet("departmentTypes")]
        public async Task<ActionResult<List<OrganisationUnitType>>> GetDepartmentTypes()
        {
            var result = await _mediator.Send(new GetDepartmentTypes.Query() { });
            return Ok(result);
        }

        /// <summary>
        /// Create department type
        /// </summary>
        /// <param name="departmentType"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.Admin)]
        [HttpPost("departmentType/create")]
        [ProducesResponseType(typeof(OrganisationUnitType), StatusCodes.Status201Created)]
        public async Task<ActionResult<OrganisationUnitType>> CreateDepartmentType([FromBody] OrganisationUnitType departmentType)
        {
            
            var result = await _mediator.Send(new CreateOrganisationUnitType.Command() { Type = departmentType });

            return Ok(result);
            
        }

        /// <summary>
        /// Update department type
        /// </summary>
        /// <param name="departmentType"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.Admin)]
        [HttpPut("departmentTypes/update")]
        [ProducesResponseType(typeof(OrganisationUnitType), StatusCodes.Status200OK)]
        public async Task<ActionResult<OrganisationUnitType>> UpdateDepartmentType([FromBody] OrganisationUnitType departmentType)
        {
            var result = await _mediator.Send(new UpdateDepartmentType.Command() { Type = departmentType });
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
            if (await _userService.IsCoordinatorForDepartmentOrAdmin(id))
            {
                return await _mediator.Send(new GetRolesForDepartment.Query { OrganisationUnitId = id });
            }
            return Unauthorized();

        }


        /// <summary>
        /// Delete department
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.AdminOrCoordinator)]
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
