declare @Institution varchar(50) = 'haukeland%'
declare @Dato varchar(50) = '2024-5-21'

-- Finn sessions for institusjon på en dato
select 
* 
from Sesjon s
join Department avd on avd.Id = s.DepartmentId
join Institution inst on inst.Id = avd.InstitutionId
where inst.Name like @Institution
and CONVERT(date, s.Opprettettidspunkt) = @Dato

-- Finn avdelingene for institusjonen
select 
--* 
inst.Id InstId, inst.Name Institution, inst.HERId, Avd.Id AvdId, avd.Name Department
from Institution inst
join Department avd on avd.InstitutionId = inst.Id
where inst.Name like @Institution

/*
-- Endre avdeling på sesjon
update Sesjon
set DepartmentId = XX where Id = 'SesjonId'
*/