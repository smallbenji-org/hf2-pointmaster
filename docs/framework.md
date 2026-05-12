# Framework

Jeg har valgt at bruge asp.net core + vue 3 til mit GUI projekt. Grunden til dette valg er da jeg har arbejdet med det før,
men gerne har ville lære mere om det.
Generelt har min oplevelse været meget god. Kan virkelig godt lide ideen om stores og at man henter dataen via et API endpoint og gemmer dataen i ens cache,
så man ikke hele tiden skal hente dataen.

## asp.net core
Her har jeg valgt at bruge Npgsql + Dapper, en virkelig kraftfuld combo, det har gjort min udviklingstid meget nemmere.
Dapper gør at man meget nemt kan caste queries du laver om til modeller, uden du skal mappe en masse.

Desuden har jeg brugt DbUp til migreringer. Migrations mappen indeholder alle mine ændringer til databasen.

Til auth har jeg brugt asp.net cores identity library. Det fungerer meget godt, dog meget tidskrævende at få til at virke helt som forventet.

Jeg har gået efter at lave repositories til alt data behandling og controllers til at lave endpoints.

## Vue 3
I Vue 3 har jeg valgt at bruge UI framework'et Buefy, som er en tilbygning på CSS biblioteket Bulma, dette har gjort min GUI rejse mega nem.
Jeg har ikke været nødt til at genopfinde den dybe talerken flere gange, knapper, tabeller osv. har været lavet på forhånd, jeg har bare skulle suplere den med en design guide.

Til datastores har jeg valgt at bruge Pinia, som sammen med vue-router gør at man ikke skal reloade data hver gang du går på en ny side.
Her definerer jeg bare en "store" og Pinia sørger for resten. Så kan jeg trække den data jeg har brug for ind i de respektive komponenter

## Vil jeg have valgt noget andet?
Jeg ville nok ikke have valgt en anden kombi til dette projekt.
asp.net core og Vue 3 er simpelt og læseligt i min optik og er mine 'frameworks of choice'

Når jeg bygger Vue projektet bliver "dist" mappen automatisk smidt i wwwroot mappen, så asp.net core kan hoste frontenden.
Dette gør at Vue og asp.net core spiller super godt sammen.
