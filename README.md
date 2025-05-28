# Beskrivelse 

Digitalisert observasjon av h�ndhygiene.
L�sningen best�r av 2 komponenter: Admin og Observasjon.

## Observasjon
Et grensesnitt hvor observat�rer (Observat�r) kan logge inn med HelseId og , med mer.

"Observat�r" kan:
- Se sine tilknyttede institusjoner
- Foreta ulike typer observasjoner (Fire indikasjoner, Hanskebruk, H�ndsmykke, Beskyttelsesutstyr) p� sine institusjoner avdelinger 
- Sende inn observasjonene iform av Sesjoner til "Coordinator" for institusjon


## Admin
Et administrativt grensesnitt hvor FHI (FHI Admin) og ulike helseforetak/institusjoners koordinatorer (Coordinator) kan logge inn.

"FHI Admin" kan:
- Redigere kodeverk
- Se alle observasjonsdata
- Redigere institusjoner 
- Opprette/redigere "Coordinator"-brukere og "Observasjon"-brukere for institusjoner. 

"Coordinator" kan:
- Se sin(e) institusjon(er)s observasjoner
- Sende observasjonssesjoner videre til FHI
- Opprette Observat�rer og Koordinatorer for sin(e) institusjon(er)

## Views

 En del av datasettet eksponeres read-only i form av views. View'ene ligger i HyFive.Dataaksess/Views.
 Disse view'ene tas ibruk direkte i databasen via Azure AD tilgang
 Ved endring av returverdiene i et view: 
  1. Kj�r alle unit-tester for � se at ikke integrasjonen mellom endret view og HandhygieneContext blir �delagt.
  2. Husk � oppdater view'ene du endrer i alle milj�er du deployer til. 

Ved endring av tabellene som et view bruker:
Korriger view'et og f�lg deretter punkt 1-2 over.



## Autentisering
Autentisering foretas ved bruk av HelseId gjennom komponenten Fhi.HelseId (https://github.com/folkehelseinstituttet/fhi.helseid). 

## Autorisering
Autorisering foretas ved bruk av informasjon utstedt i et HelseId-token (HPR-nummer og PidPseudonym) sammen med data lagret i en brukertabell (Bruker) med diskriminerte brukertyper for "FHI Admin", "Coordinator" og "Observat�r".
Hvert API-endepunkt sikres ved bruk av en policy.
Der det er n�dvendig gj�res ytterligere tilgangskontroll ved � inspisere API-requesten og nekte tilgang basert p� query-parametere, f eks. hvis en Coordinator fors�ker � sp�rre om data fra en institusjon den ikke er Coordinator for.

## Seeding av applikasjon
Ved f�rste gangs oppsett av applikasjonen m� den seedes med data.
Dette gj�res ved et kall til Seed-metoden i API'et i HyFive.Observasjon l�sningen.
1. Start opp l�sningen HyFive.Observasjon
2. Navig�r til '/swagger'
3. Bla deg ned til 'Seed' metoden og kj�r denne.

## Testbrukere inkludert i 'Seed' dataene
N�r du seed'er applikasjonen s� opprettes det brukere for FHI Admin, Coordinator og Observat�r som du kan logge inn med via HelseId.
Under f�lger en oversikt.

Ved innlogging i b�de Admin-grensesnitt og Observasjon-grensesnitt s� kan du velge "Test IDP" og legge inn "F�dselsnummer" for rollen du �nsker � logge inn med. 

## Testmilj�

### Admin-grensesnitt
Adresse: https://test-admin-handhygiene.azurewebsites.net
Logg p� som FHI Admin eller Coordinator. Se innloggingsopplysninger for FHI Admin og Coordinator under.

### Observasjon-grensesnitt
Adresse: https://test-handhygiene.azurewebsites.net
Logg p� som Observat�r. Se innloggingsopplysninger for Observat�r under.

### FHI Admin
Name: FELIX M�RK  
F�dselsnummer: 22127113177  
HPR-nummer: 0  

### Coordinator
Name: ANNE MARKUSSEN  
F�dselsnummer: 15037104229  
HPR-nummer: 4909402  

### Observat�r
Name: LINE DANSER  
F�dselsnummer: 13116900216  
HPR-nummer: 9383840  

## Ikoner

Ved � f�lge stegene under, kan du legge til custom ikoner i font-awesome biblioteket til applikasjonen, og bruke dem akkurat som et font-awesome ikon.

 1. Definer ikon
    - Ikoner kan kun v�re en enkel svg path eller flere paths in array (ingen circle, graph osv osv)
    - Det m� eksperimenteres med verdiene til width og height for � finne en passende st�rrelse p� ikonet

 2. Legg til din tekst-verdi for iconName i typen IconName i filen index.d.ts i pakken @fortawesome/fontawesome-common-types
    - Du kan komme deg til den via IconDefinition
    - Navnet du velger for ikonet m� v�re unikt, dvs ingen andre font-awesome ikoner eller custom ikoner kan ha det navnet fra f�r

 3. Legg til ikon-const i biblioteket nederst i denne filen

 4. Kj�r denne commandoen i en terminal for � f� med de manuelle endringene i node_modulen fra punkt 2:
    
    ```sh
    npx patch-package @fortawesome/fontawesome-common-types
    ```

    NB: Denne kommandoen oppretter en mappe "patches", og inneholder de manuelle
    endringene i node_moduler slik at CI / CD kan oppdage og legge til disse manuelle endringene etter npm install.
    Det har derfor blitt lagt til en verdi for "postinstall" i packages.json for � f� til dette.

 5. Deploy!

#### I tilfelle du f�r feil ang. verdier i IconName
 F�lg disse stegene:
    1. Slett node_modules
    2. Unstage/reset alle filer fra ClientApp/patches-mappa i GIT dersom du har noe der
    3. Kj�r ```npm install```
    4. Kj�r opp l�sningen p� nytt