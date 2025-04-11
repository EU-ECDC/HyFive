using AutoMapper;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Modeller.V1.Institution;

namespace HyFive.Tjenester.Avdeling
{
    public class OpprettAvdeling
    {
        public class Command : IRequest<Modeller.V1.Institution.Department>
        {
            public CreateDepartmentRequest Request { get; set; }
        }

        public class Handler : IRequestHandler<Command, Modeller.V1.Institution.Department>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Modeller.V1.Institution.Department> Handle(Command command, CancellationToken cancellationToken)
            {
                var institusjon = await _context
                    .Institution
                    .Include(i => i.Departments)
                    .FirstOrDefaultAsync(i => i.Id == command.Request.InstitutionId);

                if (institusjon == null)
                {
                    throw new Exception("Kunne ikke finne institusjon med ID " + command.Request.InstitutionId);
                }

                if (command.Request.RoleIds.Any() == false)
                    throw new Exception($"Rolle-liste er tom. Avdeling må opprettes med minst en rolle.");

                var avdelingtype = _context.SectionType.FirstOrDefault(a => a.Id == command.Request.DepartmentTypeId);
                if (avdelingtype == null)
                {
                    throw new Exception("Kunne ikke finne avdelingtype med ID " + command.Request.DepartmentTypeId);
                }

                var avdeling = new Domene.Place.Avdeling()
                {
                    InstitusjonId = institusjon.Id,
                    Navn = command.Request.Name,
                    Roller = HentRoller(command.Request.RoleIds),
                    Avdelingtype = avdelingtype
                };

                _context.Department.Add(avdeling);
                await _context.SaveChangesAsync();
                return _mapper.Map<Modeller.V1.Institution.Department>(avdeling);
            }

            private ICollection<Domene.Observation.Role> HentRoller(List<int> requestRolleIder)
            {
                var roller = _context.Role.Where(r => requestRolleIder.Contains(r.Id)).ToList();
                return roller;
            }
        }
    }
}
