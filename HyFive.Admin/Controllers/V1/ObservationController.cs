using HyFive.Models.V1.Overview;
using HyFive.Models.V1.Session;
using HyFive.Services.Facility;
using HyFive.Services.Session;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HyFive.Models.V1.Constants;
using HyFive.Models.V1.Observation;
using HyFive.Models.V1.Observation.ProtectiveEquipment;
using HyFive.Models.V1.Observation.Gloves;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.ProtectiveEquipment;
using HyFive.Services.FiveIndication;
using HyFive.Services.HandJewelry;
using HyFive.Services.Glove;
using HyFive.Services;

namespace HyFive.Admin.Controllers.V1
{
    [Authorize(HandhygienePolicy.AdminOrCoordinator)]
    [Route("api/v1/observation")]
    public class ObservationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IUserService _userService;

        public ObservationController(IMediator mediator, IUserService userService)
        {
            _mediator = mediator;
            _userService = userService;
        }

        /// <summary>
        /// Create report for facilities <see cref="FacilityOverviewReport"/>
        /// </summary>
        /// <returns></returns>
        [HttpGet("facilitiesWithSessions")]
        public async Task<ActionResult<IEnumerable<FacilityOverviewReport>>> GetFacilitiesWithSessions(
            [FromQuery] string facilityId,
            [FromQuery] SessionType? sessionType,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] AuthorizedRole role)
        {
            int? facilityid = null;

            if (facilityId != null)
                facilityid = int.Parse(facilityId);

            string transferStatusType;
            if (role == AuthorizedRole.Administrator)
            {
                transferStatusType = TransferStatusTypeConstants.TransferredToAdmin;
                if (!_userService.IsFhiAdmin())
                    return Forbid();
            }
            else if (role == AuthorizedRole.Coordinator)
            {
                transferStatusType = TransferStatusTypeConstants.TransferredToCoordinator;
                if (!_userService.IsCoordinatorForFacility(facilityid.Value))
                    return Forbid();
            }
            else
            {
                return Forbid();
            }

            DateTime? utcFromDate = fromDate.HasValue
               ? DateTime.SpecifyKind(fromDate.Value, DateTimeKind.Utc)
               : (DateTime?)null;

            DateTime? utcToDate = toDate.HasValue
                ? DateTime.SpecifyKind(toDate.Value, DateTimeKind.Utc)
                : (DateTime?)null;

            return await _mediator.Send(new GetFacilitiesWithSessions.Query
            {
                SessionType = sessionType,
                FromDate = utcFromDate,
                ToDate = utcToDate,
                FacilityId = facilityid,
                TransferStatusType = transferStatusType
            });
        }

        /// <summary>
        /// Get all sessions for a department. <see cref="SessionOverviewReport"/>
        /// </summary>
        /// <returns></returns>

        [HttpGet("department")]
        public async Task<ActionResult<IEnumerable<SessionOverviewReport>>> GetSessionsForDepartment(
            [FromQuery] int departmentId,
            [FromQuery] SessionType? sessionType,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] AuthorizedRole role)
        {
            string transferStatusType;
            if (role == AuthorizedRole.Administrator)
            {
                transferStatusType = TransferStatusTypeConstants.TransferredToAdmin;
                if (!_userService.IsFhiAdmin())
                    return Forbid();
            }
            else if (role == AuthorizedRole.Coordinator)
            {
                transferStatusType = TransferStatusTypeConstants.TransferredToCoordinator;
                if (!_userService.IsCoordinatorForDepartment(departmentId))
                    return Forbid();
            }
            else
            {
                return Forbid();
            }

            if (_userService.IsCoordinatorForDepartment(departmentId) || _userService.IsFhiAdmin())
            {
                var result = await _mediator.Send(new GetSessionsForDepartmentOverview.Query()
                {
                    DepartmentId = departmentId,
                    SessionType = sessionType,
                    FromDate = fromDate,
                    ToDate = toDate,
                    TransferStatusType = transferStatusType
                });
                return Ok(result);
            }

            return Unauthorized();
        }

        /// <summary>
        /// Get all sessions for an facility <see cref="SessionOverviewReport"/>
        /// </summary>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.Coordinator)]
        [HttpGet("facility")]
        public async Task<ActionResult<IEnumerable<SessionOverviewReport>>> GetSessionsForFacility(
            [FromQuery] int facilityId,
            [FromQuery] int? observerId,
            [FromQuery] SessionType? sessionType,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            if (_userService.IsCoordinatorForFacility(facilityId))
            {
                var resultat = await _mediator.Send(new GetSessionsForFacility.Query()
                {
                    FacilityId = facilityId,
                    ObservatorId = observerId,
                    SessionType = sessionType,
                    FromDate = fromDate,
                    ToDate = toDate
                });
                return Ok(resultat);
            }

            return Unauthorized();
        }

        /// <summary>
        /// Send/transfer session to FHI
        /// </summary>
        /// <returns></returns>
        [Authorize(HandhygienePolicy.Coordinator)]
        [HttpGet("transfer")]
        public async Task<ActionResult<SessionOverviewReport>> TransferSessionToFhi(
            [FromQuery] int facilityId,
            [FromQuery] Guid sessionId)
        {
            if (_userService.IsCoordinatorForFacility(facilityId))
            {
                var resultat = await _mediator.Send(new TransferSessionToAdmin.Query()
                {
                    SessionId = sessionId
                });
                return Ok(resultat);
            }

            return Unauthorized();
        }

        [Authorize(HandhygienePolicy.Coordinator)]
        [HttpPut("fiveIndications/update")]
        public async Task<ActionResult<bool>> UpdateFiveIndicationsObservation([FromBody] FiveIndicatorsObservation observation)
        {
            if (_userService.IsCoordinatorForSession(observation.SessionId))
            {
                try
                {
                    var result = await _mediator.Send(new UpdateFiveIndicationsObservation.Command
                    {
                        Observation = observation
                    });

                    return Ok(result);
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }

            return Unauthorized();

        }

        [Authorize(HandhygienePolicy.Coordinator)]
        [HttpDelete("fiveIndications/delete")]
        public async Task<ActionResult<bool>> DeleteFiveIndicationsObservation([FromQuery] string observtionId, [FromQuery] string sessionId)
        {
            if (_userService.IsCoordinatorForSession(sessionId))
            {
                try
                {
                    var result = await _mediator.Send(new DeleteFiveIndicationObservation.Command
                    {
                        ObservationId = observtionId,
                        SessionId = sessionId
                    });

                    return Ok(result);
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }    
            }

            return Unauthorized();

        }

        [Authorize(HandhygienePolicy.Coordinator)]
        [HttpPut("handJewelry/update")]
        public async Task<ActionResult<bool>> UpdateHandJewelryObservation([FromBody] HandJewelryObservation observation)
        {
            if (_userService.IsCoordinatorForSession(observation.SessionId))
            {
                try
                {
                    var result = await _mediator.Send(new UpdateHandJewelryObservation.Command
                    {
                        Observation = observation
                    });

                    return Ok(result);
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }

            return Unauthorized();
        }

        [Authorize(HandhygienePolicy.Coordinator)]
        [HttpDelete("handJewelry/delete")]
        public async Task<ActionResult<bool>> DeleteHandJewelryObservation([FromQuery] string observationId, [FromQuery] string sessionId)
        {
            if (_userService.IsCoordinatorForSession(sessionId))
            {
                try
                {
                    var result = await _mediator.Send(new DeleteHandJewelryObservation.Command
                    {
                        ObservationId = observationId,
                        SessionId = sessionId
                    });
                    
                    return Ok(result);
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }

            return Unauthorized();
        }

        [Authorize(HandhygienePolicy.Coordinator)]
        [HttpPut("glove/update")]
        public async Task<ActionResult<bool>> UpdateGloveObservation([FromBody] GloveObservation observation)
        {
            if (_userService.IsCoordinatorForSession(observation.SessionId))
            {
                try
                {
                    var result = await _mediator.Send(new UpdateGloveObservation.Command
                    {
                        Observation = observation
                    });

                    return Ok(result);
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }

            return Unauthorized();
        }

        [Authorize(HandhygienePolicy.Coordinator)]
        [HttpDelete("glove/delete")]
        public async Task<ActionResult<bool>> DeleteGloveObservation([FromQuery] string observationId, [FromQuery] string sessionId)
        {
            if (_userService.IsCoordinatorForSession(sessionId))
            {
                try
                {
                    var result = await _mediator.Send(new DeleteGloveObservation.Command
                    {
                        ObservationId = observationId,
                        SessionId = sessionId
                    });

                    return Ok(result);
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }

            return Unauthorized();
        }

        [Authorize(HandhygienePolicy.Coordinator)]
        [HttpPut("protectiveEquipment/update")]
        public async Task<ActionResult<bool>> UpdateProtectiveEquipmentObservation([FromBody] ProtectiveEquipmentObservation observation)
        {
            if (_userService.IsCoordinatorForSession(observation.SessionId))
            {
                try
                {
                    var result = await _mediator.Send(new UpdateProtectiveEquipmentObservation.Command
                    {
                        Observation = observation
                    });

                    return Ok(result);
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }

            return Unauthorized();
        }

        [Authorize(HandhygienePolicy.Coordinator)]
        [HttpDelete("protectiveEquipment/delete")]
        public async Task<ActionResult<bool>> DeleteProtectiveEquipmentObservation([FromQuery] string observationId, [FromQuery] string sessionId)
        {
            if (_userService.IsCoordinatorForSession(sessionId))
            {
                try
                {
                    var result = await _mediator.Send(new DeleteProtectiveEquipmentObservation.Command
                    {
                        ObservationId = observationId,
                        SessionId = sessionId
                    });

                    return Ok(result);
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }

            return Unauthorized();
        }

        [Authorize(HandhygienePolicy.Coordinator)]
        [HttpGet("protectiveEquipment")]
        public async Task<ActionResult<ProtectiveEquipmentObservation>> GetProtectiveEquipment([FromQuery] string observationId, [FromQuery] string sessionId)
        {
            if (_userService.IsCoordinatorForSession(sessionId))
            {
                try
                {
                    var result = await _mediator.Send(new GetProtectiveEquipmentObservation.Query()
                    {
                        ObservationId = observationId
                    });

                    return Ok(result);
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }

            return Unauthorized();
        }
        
    }
}
