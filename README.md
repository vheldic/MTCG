# MTCG
von Velid Heldic
https://github.com/vheldic/MTCG.git

## Design
Um Ordnung im Projekt zu garantieren, werden Ordner benutzt, um verschiedene Tätigkeitsbereiche zu trennen. Auch auf die Ordnung innerhalb der Klassen wurde geachtet, indem
gezielt Attribute, Konstanten und Methoden in ihre jeweilige Klasse (bzw Methode) zugeteilt wurden.  

### GameClasses
Der GameClasses Ordner beinhaltet alle Klassen, die für das Spiel selbst wichtig sind. Klassen wie `User.cs` und die Card-Klassen befinden sich hier.
Für die Card Klassen wurde ein Strategy-Pattern gedacht, bei dem eine Karte entweder ein Monster oder ein Spell sein kann.

### Http
Im http Ordner befinden sich die Klassen, die für die Server-Client Kommunikation zuständig sind.

Dazu zählen:
  * `HttpServer.cs` - Server, welcher auf Clients Connetions wartet
  * `HttpRequest.cs` - Klasse, welche die Requests vom Client verarbeitet
  * `HttpResponse.cs` - Klasse, welche die Response, basierend auf dem Request vom Client

### Testing
Für jeden Anwendungsbereich gibt es eine eigene Testklasse, welche die wichtigsten Funktionen testet. Somit können geziehlte Bereiche getestet werden und für Ordnung gesorgt werden.
