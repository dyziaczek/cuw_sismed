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

## Aplikacja

| _EKRAN LOGOWANIA_ |

> Logowanie:
<img width="760" height="452" alt="image" src="https://github.com/user-attachments/assets/9055a573-500d-4101-a4d7-c9f2edd6e2b2" />

| _GŁÓWNA APLIKACJA_ |

> Szukaj:
<img width="2555" height="1431" alt="image" src="https://github.com/user-attachments/assets/6ef5e71a-49e1-49fc-9057-44d862181ccf" />

> Recepcja:
<img width="2554" height="1438" alt="image" src="https://github.com/user-attachments/assets/fb992891-e0dc-4c3c-a363-75e4133afb68" />

> Kalendarz Wizyt:
<img width="2553" height="1436" alt="image" src="https://github.com/user-attachments/assets/9335e756-4e09-438f-a28b-267b26145756" />

> Dokumenty:
<img width="2555" height="1438" alt="image" src="https://github.com/user-attachments/assets/dc81efa3-a2bf-46a2-8fed-fb629545e310" />

> Personel:
<img width="2558" height="1438" alt="image" src="https://github.com/user-attachments/assets/65f7a046-4e91-4c4f-aa63-1be505138ac2" />

> Pacjenci:
<img width="2555" height="1429" alt="image" src="https://github.com/user-attachments/assets/fa784015-556a-4d59-92a2-d267dd9ed4ae" />

## Diagramy

<img width="3491" height="5007" alt="DiagramUML_SISMED" src="https://github.com/user-attachments/assets/fac6a51a-52c2-4541-b335-568d6a528045" />

<img width="5861" height="7334" alt="DiagramBlokowy_SISMED" src="https://github.com/user-attachments/assets/71be570c-5e9a-4431-871e-6d167703530f" />

## Plany na przyszłość

> powiadomienia e-mail dla pacjentów

> panel lekarza

> raporty statystyczne

> rozbudowany system uprawnień

> wersja webowa aplikacji

## Wykorzystanie AI

W projekcie korzystałem z AI jako narzędzia pomocniczego. AI wspierało mnie głównie przy analizie błędów, porządkowaniu wymagań, przygotowywaniu propozycji fragmentów kodu, poprawie interfejsu oraz tworzeniu dokumentacji.

Samodzielnie określiłem temat projektu, wymagania systemu, strukturę modułów, wygląd aplikacji, kod (korzystając ze źródeł w internecie, google, przykładów innych projektów stworzonych przez innych ludzi w Windows Forms, korzystając też z podpowiedzi na zajęciach) oraz sposób działania najważniejszych funkcji.

Przykładowe moje pytania do chata:

> Super wygląda na stacjonarnym, natomiast na laptopie będę prezentował aplikację, ponadto będzie ona udostępniona na rzutniku i nie chciałbym żeby to się wykrzaczyło.
 (znalazł mi rozwiązanie z problemem skalowania)

> Po wpisaniu loginu i hasła w ogóle nie uruchamia się główna aplikacja i wywala błąd.
  (w trakcie popełniłem błąd i pomógł mi go znaleźć)

> Jak logicznie podzielić aplikację na moduły?

> Jak uporządkować widok karty pacjenta, żeby dane osobowe i adres były czytelne?
  (w trakcie niektóre fragmenty zaprojektowane przeze mnie źle się przeskalowały i przesunęły, pomógł mi to narpawić)

> Jak ograniczyć pole PESEL do 11 cyfr? Jak ograniczyć numer telefonu do 9 cyfr?
  (finalnie mi nie pomógł - dałem sobie spokój)

> Jak zaprojektować lokalną bazę SQLite dla aplikacji do rejestracji wizyt lekarskich?

> Jakie tabele powinny znaleźć się w bazie danych dla pacjentów, pracowników, lekarzy, wizyt, ostrzeżeń, notatek i dokumentów?

> Jak automatycznie tworzyć bazę danych przy pierwszym uruchomieniu aplikacji?

> Jak dodać dane testowe do aplikacji demonstracyjnej?

> Jak bezpiecznie aktualizować dane pacjenta bez tworzenia duplikatu?

> Jak poprawić skalowanie aplikacji Windows Forms na ekranie 2560x1440 i 1920x1080?

> Jak sprawić, żeby panele nie nachodziły na siebie na mniejszym ekranie?

> Pomóż mi stworzyć diagramUML i diagram blokowy

Wykorzystane AI: Claude, CHAT GPT, Gemini

## Autor

131143, Jakub Niezgodziński
