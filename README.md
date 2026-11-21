
# Weather App – Flaschenpost Coding Challenge

## Voraussetzungen

- .NET 10 SDK
- Android Studio mit Android-Emulator
- Internetverbindung

## Einrichtung

Repository klonen oder als ZIP herunterladen und entpacken.

## Backend

Die Projektmappe `backend/FlaschenpostChallenge.slnx` in Visual Studio öffnen und das Backend mit dem Startprofil `http` starten.

Das Backend ist unter `http://localhost:5152` erreichbar.

## Android

Den Ordner `android` in Android Studio öffnen, das Gradle-Projekt synchronisieren und die App auf einem Android-Emulator ausführen.

Das Backend muss während der Ausführung erreichbar sein. Die Base-URL `http://10.0.2.2:5152/` ist bereits konfiguriert.

## Backend-Tests

Die Tests können über den Test-Explorer in Visual Studio ausgeführt werden.


## Hinweise

Städte und Wetterdaten werden im Arbeitsspeicher gespeichert und beim Neustart des Backends zurückgesetzt.
