using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Models.V1.OrganisationUnit;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Facility;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.AdminOrCoordinator)]
    [Route("api/v1/predefinedComment")]
    public class PredefinedCommentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _userService;

        public PredefinedCommentController(IMediator mediator, IUserService userService)
        {
            _mediator = mediator;
            _userService = userService;
        }

        /// <summary>
        /// Get PredefinedComment
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetPredefinedComments")]
        public async Task<ActionResult<List<PredefinedComment>>> GetPredefinedComments(int facilityId)
        {
            if (await _userService.IsCoordinatorForFacilityOrAdmin(facilityId))
            {
                return await _mediator.Send(new GetPredefinedCommentsForCoordinator.Query
                {
                    OrganisationUnitId = facilityId
                });
            }
            return Unauthorized();
        }

        /// <summary>
        /// Update Predefined Comment
        /// </summary>
        /// <param name="predefinedComment"></param>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        [HttpPut("{facilityId}/update")]
        public async Task<ActionResult<bool>> UpdatePredefinedComment(
            int facilityId,
            [FromBody] PredefinedComment predefinedComment)
        {
            if (await _userService.IsCoordinatorForFacilityOrAdmin(facilityId))
            {
                var isUpdated = await _mediator.Send(new UpdatePredefinedComment.Command
                {
                    PredefinedComment = predefinedComment,
                    OrganisationUnitId = facilityId
                });
                return isUpdated;
            }

            return Unauthorized();
        }

        /// <summary>
        /// Create Predefined Comment
        /// </summary>
        /// <param name="newPredefinedComment"></param>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        [HttpPost("{facilityId}/Create")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status201Created)]
        public async Task<ActionResult<bool>> CreatePredefinedComment(
            int facilityId,
            [FromBody] CreatePredefinedCommentRequest newPredefinedComment)
        {
            if (await _userService.IsCoordinatorForFacilityOrAdmin(facilityId))
            {
                var isCreated = await _mediator.Send(new CreatePredefinedComment.Command
                {
                    NewPredefinedComment = newPredefinedComment,
                    FacilityId = facilityId
                });

                return CreatedAtRoute("GetPredefinedComments", isCreated);
            }
            return Unauthorized();
        }
    }
}
