using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Institution;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Avdeling
{
    public class OppdaterAvdeling
    {
        public class Command : IRequest<Models.V1.Institution.Department>
        {
            public Command() { Roller = new List<Models.V1.Observation.Role>(); }

            public int Id { get; set; }
            public string Navn { get; set; }
            public int AvdelingTypeId { get; set; }
            public List<Models.V1.Observation.Role> Roller { get; set; }
        }

        public class Handler : IRequestHandler<Command, Models.V1.Institution.Department>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Models.V1.Institution.Department> Handle(Command command, CancellationToken cancellationToken)
            {
                var avdeling = await _context.Department
                                             .Include(a => a.Roller)
                                             .FirstOrDefaultAsync(a => a.Id == command.Id);

                if (command.AvdelingTypeId > 0)
                {
                    var avdelingtype = _context.SectionType.FirstOrDefault(a => a.Id == command.AvdelingTypeId);
                    if (avdelingtype == null)
                        throw new Exception("Kunne ikke finne avdelingtype med ID " + command.AvdelingTypeId);
                    avdeling.Avdelingtype = avdelingtype;
                }

                if (command.Roller.Any())
                {
                    var rolleIderForAvdeling = command.Roller.Select(ar => ar.Id).ToList();
                    var rollerForAvdeling = _context.Role
                                                             .Include(r => r.Departments)
                                                             .Where(r => rolleIderForAvdeling.Contains(r.Id))
                                                             .ToList();
                    avdeling.Roller = rollerForAvdeling;
                }

                avdeling.Navn = command.Navn;

                _context.Update(avdeling);
                await _context.SaveChangesAsync();
                var mapped = _mapper.Map<Models.V1.Institution.Department>(avdeling);
                return mapped;
            }
        }
    }
}
