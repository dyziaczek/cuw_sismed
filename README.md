\# CUW\_SISMED

Centrum Medyczne SISMED

Aplikacja dla Centrum Medycznego do obsługi Pacjentów, umawiania wizyt lekarskich, zarządzania personelem, kalendarzem wizyt oraz dokumentami wewnętrznymi placówki.

Projekt został stworzony przeze mnie w celach Projektu Indywidualnego.



## Funkcje systemu

> logowanie do systemu

> obsługa ról użytkowników: Administrator oraz Rejestrator

> wyszukiwanie pacjentów po PESEL, imieniu, nazwisku, dacie urodzenia, telefonie oraz adresie e-mail

> dodawanie nowych pacjentów

> edycja pełnych danych pacjenta

> podgląd karty pacjenta

> obsługa adresu pacjenta z podziałem na miasto, kod pocztowy, ulicę, numer domu oraz numer lokalu

> prowadzenie wiadomości i notatek pacjenta

> rezerwacja wizyt

> anulowanie wizyt

> zamiana pacjenta przypisanego do wizyty

> system ostrzeżeń

> blokada rezerwacji po 3 ostrzeżeniach

> kalendarz lekarzy z filtrowaniem

> filtrowanie wizyt według lekarza, specjalizacji, daty oraz statusu

> zarządzanie personelem

> dodawanie kont pracowników

> edycja kont pracowników przez administratora

> zmiana hasła pracownika przez administratora

> oznaczanie pracowników jako aktywnych lub nieaktywnych

> obsługa lekarzy i specjalizacji

> podgląd listy wszystkich pacjentów

> obsługa dokumentów wewnętrznych

> lokalny zapis danych w bazie SQLite

## Moduły aplikacji

> SZUKAJ [moduł uruchamiany domyślnie po zalogowaniu do aplikacji]
  Pozwala wyszukać pacjenta po podstawowych danych:
  1. PESEL
  2. Imię
  3. Nazwisko
  4. Data urodzenia
  5. Adres e-mail
  6. Numer telefonu
  
  Z poziomu tego modułu można również dodać nowego pacjenta.

> RECEPCJA [moduł służący do pracy z wybranym pacjentem]
  Zawiera:
  1. dzisiejsze wizyty
  2. zarezerwowane wizyty
  3. panel pacjenta
  4. kartę pacjenta
  5. adres pacjenta
  6. wiadomości i notatki
  7. możliwość umówienia wizyty
  8. możliwość przejścia do historii wizyt
  9. możliwość anulowania wizyty
  10. możliwość zamiany wizyty

> KALENDARZ WIZYT [moduł prezentuje grafik wizyt lekarzy]
  Pozwala filtrować wizyty według:
  1. lekarza
  2. specjalizacji lub usługi
  3. daty
  4. statusu wizyty
  
  Po kliknięciu wizyty możliwe jest przejście do karty pacjenta.

> PERSONEL [moduł przeznaczony do obsługi kont pracowników]
  Administrator może:
  1. dodawać pracowników
  2. edytować dane pracowników
  3. zmieniać hasła pracowników
  4. aktywować i dezaktywować konta
  5. przypisywać role
  6. oznaczać pracowników jako lekarzy
  7. przypisywać specjalizacje lekarzom
  
  Rejestrator posiada dostęp informacyjny bez możliwości zarządzania kontami.

> PACJENCI [moduł informacyjny prezentujący listę wszystkich pacjentów zapisanych w systemie]
  Widok zawiera dane pacjentów, takie jak:
  1. PESEL
  2. Imię
  3. Nazwisko
  4. Data urodzenia
  5. Adres e-mail
  6. Numer telefonu
  7. Adres
  8. Liczba ostrzeżeń
  9. Informacja o blokadzie rezerwacji
  
  Moduł nie pozwala edytować ani usuwać pacjentów. Edycja danych pacjenta odbywa się w module RECEPCJA.

> DOKUMENTY [moduł służy do obsługi dokumentów wewnętrznych placówki]
  Pozwala na:
  1. dodawanie dokumentów
  2. przeglądanie dokumentów
  3. edycję dokumentów
  4. wyszukiwanie dokumentów
  5. archiwizowanie dokumentów

## Godziny pracy

Przychodnia pracuje od poniedziałku do soboty w godzinach 7:00 - 18:00.
Wizyty są umawiane w ramach godzin pracy placówki.

## Zasady rezerwacji wizyt

> pacjent musi posiadać uzupełnione podstawowe dane

> przed umówieniem wizyty wymagany jest adres pacjenta

> system sprawdza dostępność wybranego terminu

> nie można zarezerwować zajętego terminu

> pacjent z aktywną blokadą nie może zostać zapisany na wizytę

> po trzech ostrzeżeniach system blokuje możliwość rezerwacji wizyt

## Konta demonstracyjne
<img width="731" height="340" alt="image" src="https://github.com/user-attachments/assets/3c207471-3650-478e-a443-a5fb3495fa47" />


Dane pacjentów są przykładowe i służą wyłącznie do testowania aplikacji.

## Wykorzystane technologie

> C#

> Windows Forms

> .NET Framework 4.8

> SQLite

> Visual Studio

> GitHub

> Baza danych [aplikacja korzysta z lokalnej bazy danych SQLite]

> Baza danych teraz jest tworzona automatycznie przy pierwszym uruchomieniu aplikacji.

Domyślna lokalizacja bazy:

%LOCALAPPDATA%\CUW_SISMED\sismed.db

## W bazie przechowywane są między innymi:

> konta pracowników

> dane pacjentów

> lekarze

> specjalizacje

> wizyty

> ostrzeżenia

> blokady rezerwacji

> notatki pacjentów

> dokumenty wewnętrzne

## Jak uruchomić projekt:

Sklonuj repozytorium będąc w branch feature-updates.
Otwórz projekt w Visual Studio.
Uruchom plik rozwiązania lub projekt aplikacji.
Ustaw projekt VS_CUWSISMED jako projekt startowy.
Uruchom aplikację przyciskiem F5.
Struktura projektu

## Najważniejsze pliki:

> Program.cs — plik startowy aplikacji

> login_page.cs — ekran logowania

> main_app.cs — główne okno aplikacji

> Models.cs — modele danych

> Services.cs — obsługa danych i logiki systemowej

> SampleData.cs — dane demonstracyjne

> InputValidation.cs — walidacja danych wejściowych

> AddPatientDialog.cs — formularz dodawania pacjenta

> PatientEditDialog.cs — formularz edycji danych pacjenta

> PatientSwapDialog.cs — formularz zamiany pacjenta przypisanego do wizyty

> RegisterEmployeeDialog.cs — formularz dodawania pracownika

> DocumentDialog.cs — formularz dokumentu

> AppointmentCalendarWindow.cs — kalendarz wizyt

> SismedTheme.cs — wygląd i styl aplikacji

## Zrealizowane

> ekran logowania

> role administratora i rejestratora

> wyszukiwanie pacjentów

> dodawanie pacjentów

> edycja danych pacjenta

> karta pacjenta

> adres pacjenta jako osobny blok

> wiadomości i notatki pacjenta

> rezerwacja wizyt

> anulowanie wizyt

> zamiana wizyt

> system ostrzeżeń

> blokada rezerwacji po 3 ostrzeżeniach

> kalendarz lekarzy

> zarządzanie personelem

> edycja kont pracowników

> zmiana haseł pracowników przez administratora

> moduł pacjentów

> moduł dokumentów

> baza danych SQLite

> odświeżony interfejs użytkownika

> skalowanie aplikacji do różnych rozdzielczości

## Plany na przyszłość

> powiadomienia e-mail dla pacjentów

> panel lekarza

> raporty statystyczne

> rozbudowany system uprawnień

> wersja webowa aplikacji

## Autor

131143, Jakub Niezgodziński
