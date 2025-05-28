--På prod var serveren satt opp med UTC-tid.  Den skal være satt opp med norsk tid
--Dette scriptet juster tid på sessions og observasjoner så allerede sessions/observasjoner får norsk opprettidspunkt
 
 --SESJON
 --vintertid 2022/2023 - problemet startet 1.november
 update Sesjon
 set Opprettettidspunkt = DATEADD(hour, 1, Opprettettidspunkt)
 where Opprettettidspunkt > '2022-11-01'
 and Opprettettidspunkt < '2023-03-26 03:00:00' 

  --sommertid 2023
 update Sesjon
 set Opprettettidspunkt = DATEADD(hour, 2, Opprettettidspunkt)
 where Opprettettidspunkt > '2023-03-26 03:00:00' 
 and Opprettettidspunkt < '2023-10-29 03:00:00'

  --vintertid 2023/2024 
 update Sesjon
 set Opprettettidspunkt = DATEADD(hour, 1, Opprettettidspunkt)
 where Opprettettidspunkt > '2023-10-29 03:00:00' 
 and Opprettettidspunkt < '2024-02-05 10:39:00'    --tidspunkt når NHN har stilt riktig tidssone


 --BESKYTTELSESUTSTYROBSERVASJON

 update BeskyttelsesutstyrObservasjon
 set Opprettettidspunkt = DATEADD(hour, 1, Opprettettidspunkt)
 where Opprettettidspunkt > '2022-11-01'
 and Opprettettidspunkt < '2023-03-26 03:00:00'   

  --sommertid 2023
 update BeskyttelsesutstyrObservasjon
 set Opprettettidspunkt = DATEADD(hour, 2, Opprettettidspunkt)
 where Opprettettidspunkt > '2023-03-26 03:00:00' 
 and Opprettettidspunkt < '2023-10-29 03:00:00'   


 update BeskyttelsesutstyrObservasjon
 set Opprettettidspunkt = DATEADD(hour, 1, Opprettettidspunkt)
 where Opprettettidspunkt > '2023-10-29 03:00:00' 
 and Opprettettidspunkt < '2024-02-05 10:39:00'   --tidspunkt når NHN har stilt riktig tidssone

 --FIREINDIKASJONEROBSERVASJON

 update FireIndikasjonerObservasjon
 set Opprettettidspunkt = DATEADD(hour, 1, Opprettettidspunkt)
 where Opprettettidspunkt > '2022-11-01'
 and Opprettettidspunkt < '2023-03-26 03:00:00'   

  --sommertid 2023
 update FireIndikasjonerObservasjon
 set Opprettettidspunkt = DATEADD(hour, 2, Opprettettidspunkt)
 where Opprettettidspunkt > '2023-03-26 03:00:00' 
 and Opprettettidspunkt < '2023-10-29 03:00:00' 

 update FireIndikasjonerObservasjon
 set Opprettettidspunkt = DATEADD(hour, 1, Opprettettidspunkt)
 where Opprettettidspunkt > '2023-10-29 03:00:00' 
 and Opprettettidspunkt < '2024-02-05 10:39:00'    -- tidspunkt når NHN har stilt riktig tidssone

 --HANDSMYKKEOBSERVASJON

 update HandsmykkeObservasjon
 set Opprettettidspunkt = DATEADD(hour, 1, Opprettettidspunkt)
 where Opprettettidspunkt > '2022-11-01'
 and Opprettettidspunkt < '2023-03-26 03:00:00'

  --sommertid 2023
 update HandsmykkeObservasjon
 set Opprettettidspunkt = DATEADD(hour, 2, Opprettettidspunkt)
 where Opprettettidspunkt > '2023-03-26 03:00:00' 
 and Opprettettidspunkt < '2023-10-29 03:00:00'

 update HandsmykkeObservasjon
 set Opprettettidspunkt = DATEADD(hour, 1, Opprettettidspunkt)
 where Opprettettidspunkt > '2023-10-29 03:00:00' 
 and Opprettettidspunkt < '2024-02-05 10:39:00'   -- tidspunkt når NHN har stilt riktig tidssone 

 --HANSKEOBSERVASJON

 update HanskeObservasjon
 set Opprettettidspunkt = DATEADD(hour, 1, Opprettettidspunkt)
 where Opprettettidspunkt > '2022-11-01'
 and Opprettettidspunkt < '2023-03-26 03:00:00'

  --sommertid 2023
 update HanskeObservasjon
 set Opprettettidspunkt = DATEADD(hour, 2, Opprettettidspunkt)
 where Opprettettidspunkt > '2023-03-26 03:00:00' 
 and Opprettettidspunkt < '2023-10-29 03:00:00'

 update HanskeObservasjon
 set Opprettettidspunkt = DATEADD(hour, 1, Opprettettidspunkt)
 where Opprettettidspunkt > '2023-10-29 03:00:00'
 and Opprettettidspunkt < '2024-02-05 10:39:00'   --tidspunkt når NHN har stilt riktig tidssone