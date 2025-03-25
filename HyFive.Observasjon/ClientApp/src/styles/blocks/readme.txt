Filer som skal lages her skal være på BEM-struktur og representere en 'block'.
Se http://getbem.com/introduction/

Se også eksempler på blocks i Folkehelseinstituttet.Style prosjektet:
https://github.com/folkehelseinstituttet/Fhi.Frontend.Style/tree/main/scss/fhi-felles/komponenter
(NB: 'komponenter' navnet skal bli 'blocks' ganske snart, så mulig brukket lenke)

 Filnavn skal være:
 _hh-blokknavn.block.scss

 Breakdown:
 hh = "Håndhygiene"
 blokknavn: Navn på blokken du lager
 block: suffix for å indikere at det er en blokk

 Legg gjerne med eksempel-HTML øverst i fila som viser hvordan blokken kan brukes.


NB: Husk på å importer blokken din i styles.scss

BEM in a nutshell:

.person {}
.person--female {}
.person__hand {}
.person__hand--left {}
