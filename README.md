# MTCG
von Velid Heldic
https://github.com/vheldic/MTCG.git

## Design
Um Ordnung im Projekt zu garantieren, werden Ordner benutzt, um verschiedene Tätigkeitsbereiche zu trennen. Auch auf die Ordnung innerhalb der Klassen wurde geachtet, indem gezielt Attribute, Konstanten und Methoden in ihre jeweilige Klasse (bzw Methode) zugeteilt wurden. Ein UML Diagramm zum Projekt befindet sich im Design-Ordner

### GameClasses
Der GameClasses Ordner beinhaltet alle Klassen, die für das Spiel selbst wichtig sind. Klassen wie `User.cs` und die Card-Klassen befinden sich hier.
Für die Card Klassen wurde die Vererbung einer abstrakten Klasse verwendet, bei dem eine Karte entweder ein Monster oder ein Spell sein kann.
Die Klasse `Battle.cs` kümmert sich um den Ablauf der Kämpfe zwischen zwei Usern und entscheidet anschließend den Gewinner.

### Http
Im http Ordner befinden sich die Klassen, die für die Server-Client Kommunikation zuständig sind.

Dazu zählen:
  * `HttpServer.cs` - Server, welcher auf Clients Connetions wartet
  * `HttpRequest.cs` - Klasse, welche die Requests vom Client verarbeitet
  * `HttpResponse.cs` - Klasse, welche die Response zurückgibt, basierend auf dem Request vom Client

### Database
Die Database Klasse dient ausschließlich der Datenbankverbindung und der Manipulation der Daten in der Datenbank. Es war gedacht, einfache Dictionaries als Ersatz für die Datenbank zu nutzen, bis diese aufgesetzt wurde. Als schließlich die ersten Probleme und Komplikationen auftraten war ich gezwungen, die Datenbank zu erstellen und auf diese umzusteigen.

### Testing
Für jeden Anwendungsbereich gibt es eine eigene Testklasse, welche die wichtigsten Funktionen testet. Somit können geziehlte Bereiche getestet werden und für Ordnung gesorgt werden. Die verschiedenen Testfälle wurden so gewählt, dass die richtige Funktionsweise der Hauptfunktionen der Klassen getestet werden.

### Unique feature
*Daily Wheelspin*

Der angemeldete User kann einmal am Tag am Glücksrad drehen und bis zu 10 Coins gewinnen. Nach dem dreh kann erst am darauffolgenden Tag erneut gedreht werden.

## Lessons learned
Der Teil, bei dem ich die meisten Schwierigkeiten hatte, war das Konzept und der Aufbau des Projekts. Obwohl das grobe Ziel klar war, wusste ich nicht, wie ich die 
die verschiedenen Bereiche effizient trenne und diese trotzdem miteinander kommunizieren lasse. Zuerst hatte ich mir für die Card-Klassen ein Strategy-Pattern überlegt ohne an die einfache Vererbung einer abstrakten Klasse zu denken. Dieser Denkfehler hat mich die längste Zeit gekostet. Des weiteren ist mir aufgefallen, dass ich meist zu lange über kommende Funktionen nachgedacht habe und wie ich diese am besten vorbereite, statt in kleinen Schritten zu beginnen und darauf aufzubauen. 

Insgesamt hat das Projekt um die 100 Stunden gedauert
