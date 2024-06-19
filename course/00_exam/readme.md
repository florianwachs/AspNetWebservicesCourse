# Informationen zur Prüfung

Bei `Web Services` besteht die Gesamtnote aus der Bewertung einzelner Meilensteine die im Zuge einer Projektarbeit von Euch im Team erbracht wird. Folgende Komponenten gibt es.

## Vorstellung der Projektidee und Use-Cases

Die Termine der einzelnen Teams werden Euch noch mitgeteilt. Der Termin soll ca. 5-10min. dauern in der Ihr Eure Projektidee vorstellt. Wichtig, es geht nur um eure Idee, nicht um fertige Diagramme oder technische Details (die lernen wir ja erst noch während der Vorlesung). Der Termin dient auch hauptsächlich für mich, um den Schwierigkeitsgrad der Projekte zwischen den Teams anzugleichen und Euch (falls gewünscht) hinsichtlich möglicher Umsetzungsideen zu beraten.

### Zeitpunkt

vermutlich am Termin 3 nachmittags **TODO**

### Inhalte / Ablauf

- Vorstellung Eurer Projekt Idee als 5-10 minütige Präsentation.
- Eure geplanten Use-Cases / User-Stories, noch keine Technologie oder Architektur-Entscheidungen notwendig, das nehmen wir erst noch durch.

### Bewertung

5 Punkte

## Präsentation der Architektur und Schnittstellendefinition

### Zeitpunkt

Am Termin 5

### Inhalte / Ablauf

- 10 -15 min. Präsentation
- Spezifikation
- Schnittstellen
- Architektur
- UI-Workflow (falls vorhanden)
- Testspezifikation (falls vorhanden)

### Bewertung

25 Punkte

## Abschlusspräsentation und Source-Code-Bewertung

### Zeitpunkt

Am Prüfungstag (Termin 6)

### Inhalte / Ablauf

- ca. 45 min. Präsentation (inkl. 10-15min Fragen) (50 Punkte)
- Jedes Mitglied muss einen Teil präsentieren
- Sourcecode in einem Repository
  - Jedes Mitglied muss am Code mitgearbeitet haben

> Tips: Sprechzeit gut zwischen den Teammitgliedern aufteilen, Live-Demo, Ansprechen was gut / schlecht lief

## Bewertung

- Präsentation: 50 Punkte
- Source-Code: 20 Punkte

## Tips und Beispiele

- [Auzug von Projekten aus vergangenen Jahren](project_ideas.md)
- Beispiel [Abschlusspräsentation](Example%20Presentation1.pdf)
- Beispiel [Abschlusspräsentation](Example%20Presentation2.pdf)

## Teams Feedback

Hier ist ein kleines Feedback zu Euren Präsentationen. 

### Aufgabenmanagement
- Kategorie evtl. hervorheben
- Funktionalitätsseite: Bild etwas klein, vielleicht eigene Folie die nur weiß ist?
- Endpunkte: tasks, kleiner Schreibfehler
- Task Endpunkte: get Priority get category

### ArcadeCritics
- Umbrüche auf Folien bei Datenflow / Architektur


### Spieleabend
- Userstories haben viel Text auf der Seite
- Manche APIs nicht "resty" (snacks, movies, "Convert-To-Ical")

### Stundenplan
- Deviations vielleicht früher erklären

### Rezepte App

- Schnittstellen /api/v1/recipes/{id}/ratings (auf Position der Id achten)
- Schnittstellen Recipes: searchByIngriedients searchByNutrients, hier haben wir über Queryendpunkte gesprochen statt diesen RPC-Style zu verwenden. Z.B. ein /api/v1/recipes/queries Endpunkt gegen den ein Search-Objekt gepostet werden kann.
