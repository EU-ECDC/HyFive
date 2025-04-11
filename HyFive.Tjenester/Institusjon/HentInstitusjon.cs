using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace HyFive.Tjenester.Institusjon
{
    public class HentInstitusjon
    {
        public class Query : IRequest<Modeller.V1.Institution.Institution>
        {
            public int InstitusjonId = 0;
        }

        public class Handler : IRequestHandler<Query, Modeller.V1.Institution.Institution>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }



            public async Task<Modeller.V1.Institution.Institution> Handle(Query request, CancellationToken cancellationToken)
            {
                var institusjon = await _context.Institution
                    .AsNoTracking()
                    .Include(i => i.Departments)
                    .ThenInclude(a => a.Roller)
                    .Include(i => i.PredefinerteKommmentarer)
                    .Include(i => i.Institusjontype)
                    .ProjectTo<Modeller.V1.Institution.Institution>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(i => i.Id == request.InstitusjonId, cancellationToken);
                
                if (institusjon == null)
                {
                    throw new Exception($"Kunne ikke finne institusjon med id {request.InstitusjonId}");
                }
                
                institusjon.HarObservasjoner = 
                    institusjon.Avdelinger != null 
                    && institusjon.Avdelinger.Any() 
                    && _context.Sesjon.Include(s => s.Department)
                        .Any(s => institusjon.Avdelinger.Select(a => a.Id).Contains(s.Avdeling.Id));

                institusjon.Avdelinger = institusjon.Avdelinger.OrderBy(a => a.Navn).ToList();

                return institusjon;
            }
        }
    }
}
