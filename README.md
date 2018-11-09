# Demo
_Databases 3 SO, Week 4_
(In)efficiëntie van het entity framework. Dit gaan we monitoren met de profiler.

## 1. Complex query
We kunnen natuurlijk allemaal mooie LINQ-statements schrijven. Dit werkt ideaal omdat we nu geen queries hoeven te schrijven.
1. Bekijk de code van de methode ComplexQuery. We willen per state de bestellingen per kleur zien en de percentages hiervan in de state. Hiermee kunnen we inzichten krijgen over welke kleuren we vooraf op stock moeten hebben bijvoorbeeld.
2. Voer de code uit. Dit duurt even...jammer.
3. Open de SQL-Server profiler en voer de code nogmaals uit.
4. Je ziet dat het Entity Framework er een best flinke query van gemaakt heeft die slecht performt. Hoe kunnen we dit verbeteren?
5. Kopieer de query naar een nieuwe query window in SQL-server.
6. Vink 'include actual execution plan' aan.
7. Voer de query uit.
8. Zie in groen dat er een index kan gemaakt worden om deze query te verbeteren.<br/>
    Doe hier een rechtermuisklik op en kies 'missing index details'
9. Je ziet dat er een index gesuggereerd wordt op orderlines en dat de orderID mét stockitemID een grove verbeterslag kan opleveren.
10. Laten we deze index eens creëren.
11. Voer nu je code opnieuw uit en zie dat je ineens een stuk sneller resultaten hebt!

## 2. NaiveReporting
We gaan eens kijken of het Entity Framework wel zo netjes werkt als we denken.
1. Zet bovenin de call naar ComplexQuery in commentaar en haal NaiveReporting uit commentaar.
2. Bekijk de methode, deze ziet er best netjes uit toch? We willen gewoon van 50 InvoiceLines zien waar dat ze naartoe moeten en hoeveel dat het er zijn.
3. Start de SQL-Server profiler.
4. Start je code en houdt de profiler in de gaten.
5. Zie dat er enorm veel queries uitgevoerd worden! Waarom is dat?<br/>
_Dit komt omdat het Entity Framework default lazy loading gebruikt._
6. Dit kunnen we op 2 manieren oplossen:
   1. We schrijven een stored procedure.
   2. We gebruiken de code uit de methode 'NaiveReportingBetter'

## 3. NaiveReportingBetter
Wanneer je includes gebruikt geef je het Entity Framework de hint om het allemaal vooraf op te halen.
1. Zet het gebruik van NaiveReporting in commentaar en haal NaiveReportingBetter uit commentaar
2. Start SQL-Server profiler
3. Voer de code uit
4. Kijk welke query er uitgevoerd wordt.
5. Je ziet dat er nu slechts 1 query uitgevoerd wordt, dit is veel sneller en scheelt data dat over het lijntje gestuurd moet worden!
6. Deze haalt wel alle kolommen op omdat het Entity Framework niet weet welke je gaat gebruiken.
7. Mocht je deze informatie vaker nodig hebben dan kan je natuurlijk beter een View of een Stored Procedure maken.
## 4. Reporting best (extra)
Maak van bovenstaand probleem een view of een stored procedure en roep deze aan vanuit je code.