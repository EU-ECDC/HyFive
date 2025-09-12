using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.User;
using HyFive.Models.V1.Constants;
using HyFive.Services.Authentication.User;
using HyFive.Services.Glove.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using GloveSession = HyFive.Models.V1.Session.GloveSession;

namespace HyFive.Services.Glove
{
    public class SaveSession
    {
        public class Command : IRequest<Guid>
        {
            public string Email { get; set; }
            public GloveSession Session { get; set; }
        }

        public class Handler : IRequestHandler<Command, Guid>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;
            private readonly IUserService _userService;

            public Handler(HandHygieneContext context, IMapper mapper, ILogger<Handler> logger, IUserService userService)
            {
                _context = context;
                _mapper = mapper;
                _logger = logger;
                _userService = userService;
            }

            public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
            {
                var observator = await GetObserver(request, cancellationToken);
                if (observator == null)
                    throw new Exception(
                        $"Did not find an observer with email { request.Email } at institution with ID: {request.Session.Department.InstitutionId}");

                var gloveWithIndicationTypes = _context.GloveWithIndicationType.ToList();
                var gloveWithoutIndicationTypes = _context.GloveWithoutIndicationType.ToList();
                var handHygieneAfterGloveUseTypes = _context.HandHygieneAfterGloveUseType.ToList();

                var session = _mapper.Map<Domain.Session.GloveSession>(request.Session);
                session.CreatedDate = DateTime.UtcNow;
                session.StartDate = DateTime.UtcNow;
                session.Department = await GetDepartment(request, cancellationToken);
                session.Observer = observator;

                // This is the way we want to handle the error if we try to save a session with a department that no longer exists
                if (session.Department == null)
                {
                    _logger.LogWarning($"Did not find department with ID: {request.Session.Department.Id}");
                    return session.Id;
                }

                foreach (var observation in session.Observations)
                {
                    observation.CreatedTime = DateTime.UtcNow;
                    observation.RegisteredTime = DateTime.UtcNow;
                    observation.Role = session.Department.Roles.FirstOrDefault(r => r.Id == observation.Role.Id);
                    observation.GloveWithIndicationTypes = gloveWithIndicationTypes
                                                            .Where(hmi => observation.GloveWithIndicationTypes.Select(ohmi => ohmi.Id).Contains(hmi.Id))
                                                            .ToList();
                    observation.GloveWithoutIndicationTypes = gloveWithoutIndicationTypes
                                                            .Where(hui => observation.GloveWithoutIndicationTypes.Select(ohui => ohui.Id).Contains(hui.Id))
                                                            .ToList();
                    observation.PostGloveHandHygieneType = observation.PostGloveHandHygieneType != null
                                                                ? handHygieneAfterGloveUseTypes.FirstOrDefault(he => he.Id == observation.PostGloveHandHygieneType.Id)
                                                                : null;
                    GloveObservationValidator.ValidateObservation(observation);
                }

                var transferStatuses = _context.TransferStatusType.ToList();
                session.TransferStatus = transferStatuses.First(o => o.Code == TransferStatusTypeConstants.TransferredToCoordinator);

                _context.Add(session);
                _context.SaveChanges();

                return session.Id;
            }

            private async Task<Domain.Place.Department> GetDepartment(Command request, CancellationToken cancellationToken)
            {
                return await _context.Department.Include(a => a.Roles)
                    .FirstOrDefaultAsync(a => a.Id == request.Session.Department.Id, cancellationToken);
            }

            private async Task<Observer> GetObserver(Command request, CancellationToken cancellationToken)
            {
                var institution = await _context.Institution
                    .Include(i => i.Users)
                    .FirstOrDefaultAsync(i => i.Id == request.Session.Department.InstitutionId);

                if (institution == null)
                    throw new Exception(
                        $"Did not find the specified institution with ID: {request.Session.Department.InstitutionId}");

                return institution.Users.OfType<Observer>().Where(
                    _userService
                        .HasEmailAndIsActive<Observer>(request.Email).Compile()).FirstOrDefault();
            }
        }
    }
}