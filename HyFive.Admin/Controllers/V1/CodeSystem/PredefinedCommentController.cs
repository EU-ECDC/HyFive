using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Models.V1.Institution;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Institution;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.FhiAdminOrCoordinator)]
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
        public async Task<ActionResult<List<PredefinedComment>>> GetPredefinedComments(int institutionId)
        {
            if (_userService.IsCoordinatorForInstitutionOrFhiAdmin(institutionId))
            {
                return await _mediator.Send(new GetPredefinedCommentsForCoordinator.Query
                {
                    InstitutionId = institutionId
                });
            }
            return Unauthorized();
        }

        /// <summary>
        /// Update Predefined Municipality
        /// </summary>
        /// <param name="predefinedComment"></param>
        /// <param name="institutionId"></param>
        /// <returns></returns>
        [HttpPut("{institutionId}/update")]
        public async Task<ActionResult<bool>> UpdatePredefinedComment(
            int institutionId,
            [FromBody] PredefinedComment predefinedComment)
        {
            if (_userService.IsCoordinatorForInstitutionOrFhiAdmin(institutionId))
            {
                var isupdated = await _mediator.Send(new UpdatePredefinedComment.Command
                {
                    PredefinedComment = predefinedComment,
                    InstitutionId = institutionId
                });
                return isupdated;
            }

            return Unauthorized();
        }

        /// <summary>
        /// Create Predefined Municipality
        /// </summary>
        /// <param name="newPredefinedComment"></param>
        /// <param name="institutionId"></param>
        /// <returns></returns>
        [HttpPost("{institutionId}/Create")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status201Created)]
        public async Task<ActionResult<bool>> CreatePredefinedComment(
            int institutionId,
            [FromBody] CreatePredefinedCommentRequest newPredefinedComment)
        {
            if (_userService.IsCoordinatorForInstitutionOrFhiAdmin(institutionId))
            {
                var isCreated = await _mediator.Send(new CreatePredefinedComment.Command
                {
                    NewPredefinedComment = newPredefinedComment,
                    InstitutionId = institutionId
                });

                return CreatedAtRoute("GetPredefinedComments", isCreated);
            }
            return Unauthorized();
        }
    }
}
