# Release notes

Elk platform wordt apart gereleased, met een zelf gekozen versie.

## Stappen

1. Voeg indien nodig release notes toe (zie hieronder) in de PR.
2. Merge de PR naar `main` met label `android:<versie>` en/of `ios:<versie>`, bv. `android:1.1.3` en `ios:1.2.0`
   (label eerst aanmaken: `gh label create android:1.1.3`).
   Handmatig starten via "Run workflow" kan ook: vul de versie(s) in, leeg laten = dat platform overslaan.

Enkel de gelabelde platformen worden gebouwd en geüpload; het andere platform blijft ongemoeid.
Terugzoeken kan via de gesloten PR's met dat label, de tags (`android-v1.1.3`) of de GitHub Releases.

## Gedrag per versie

| Versie            | Gedrag                                                                                              | Notes          |
|-------------------|-----------------------------------------------------------------------------------------------------|----------------|
| `x.y.z` met z > 0 | Automatisch gepubliceerd (Play: production/completed, iOS: submit for review + auto release)        | Optioneel; default "Bugfixes en algemene verbeteringen" |
| `x.y.0` / `x.0.0` | Enkel geüpload (Play: draft, iOS: nieuwe versie in App Store Connect); **handmatig publiceren**     | **Verplicht**  |
| `x.y.z-beta.n`    | Beta: Play **internal track** (direct beschikbaar) / **TestFlight** (interne testers); nooit productie | Optioneel; default "Testversie" |

De stores zien enkel `x.y.z`; het buildnummer (run-nummer) maakt elke build uniek. Het suffix zit in de tag en notes-bestandsnaam (`1.2.0-beta.1.md`). Een beta moet vóór de finale van die versie komen.

Een bestaande tag (`android-v1.1.3` / `ios-v1.2.0`) wordt niet opnieuw gereleased; de workflow faalt dan vóór het bouwen.
De tag en GitHub Release worden aangemaakt na een geslaagde store-upload van dat platform.

## Beta testen vanaf een featurebranch

Start de Release Pipeline via **Run workflow** en selecteer de gewenste branch bij **Use workflow from**.
Vul voor elk te bouwen platform een beta-versie in, bv. `1.2.0-beta.1`; laat het andere platform leeg.
De broncode en release notes komen van de geselecteerde branch, vastgepind op de commit waarmee de workflow is gestart.
Beide buildjobs en de releasetags gebruiken diezelfde commit.

Buiten `main` zijn uitsluitend beta-versies toegestaan. Ook een combinatie van een beta voor het ene platform
en een niet-beta voor het andere wordt geweigerd voordat er gebouwd of geüpload wordt.
Niet-beta-releases moeten vanaf `main` starten. Handmatig starten vanaf een tag is niet toegestaan.
De geselecteerde branch moet deze versie van de workflow en het releasescript bevatten;
werk oudere featurebranches eerst bij vanuit `main`. Gebruik voor iedere beta een nieuwe, nog niet gebruikte versie.

## Bestanden

Zoekvolgorde:
1. `releasenotes/android/<versie>.md` of `releasenotes/ios/<versie>.md` (platformspecifiek)
2. `releasenotes/<versie>.md` (gedeeld)

Platte tekst die als "What's New" in de store en als GitHub Release-tekst wordt gebruikt (max. 500 tekens voor Google Play).
Locales: `ANDROID_LOCALE` en `IOS_LOCALE` repository variables (default `nl-NL`).

## Benodigde secrets en variables

- Android: `ANDROID_KEYSTORE` (base64), `ANDROID_KEY_ALIAS`, `ANDROID_KEY_PASSWORD`, `ANDROID_KEYSTORE_PASSWORD`,
  `GOOGLE_PLAY_SERVICE_ACCOUNT_JSON`; variable `ANDROID_PACKAGE_NAME`.
- iOS: `IOS_CERTIFICATE_BASE64`, `IOS_CERTIFICATE_PASSWORD`, `IOS_PROVISIONING_PROFILE_BASE64`, `IOS_PROVISIONING_PROFILE_NAME`,
  `IOS_SIGNING_IDENTITY`, `KEYCHAIN_PASSWORD`, en voor App Store Connect: `APPSTORE_CONNECT_KEY_ID`,
  `APPSTORE_CONNECT_ISSUER_ID`, `APPSTORE_CONNECT_PRIVATE_KEY` (volledige inhoud van het .p8-bestand);
  optionele variable `IOS_BUNDLE_ID` (default `be.bullit.storeit`).
