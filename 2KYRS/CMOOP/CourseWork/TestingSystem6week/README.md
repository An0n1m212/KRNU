# TestingSystem — тиждень 6 (ТЗ розд. 6, етап 5)

Перевірка робіт, журнал, аналітика, експорт, сертифікати.

## Що тут (з фінального `TestingSystem`, без змін)
- Усе з тижня 5.
- `Models/Reports.cs` — рядки експорту, складність, сертифікат, звіт.
- `Abstractions/IReportExportService.cs`, `Services/ReportExportService.cs` — Excel/CSV/PDF (`QuestPDF`), сертифікати.
- `Services/StatisticsService.cs` — середні бали, % складання, групи, динаміка, складність (поріг 50%).
- `Abstractions/ILocalizationService.cs`, `Services/LocalizationService.cs`, `Services/Strings.cs` — поки лише як залежність експорту (повноцінне перемикання UA/RU/EN — тиждень 7).

## Своє для тижня 6
- `Views/ReviewQueueWindow` + `Views/ReviewWindow` — черга, файл відповіді, бал 0..макс, примітка, завершення (ТЗ п.3.5.1).
- `Views/JournalWindow` — фільтри група/тест, складність, експорт (ТЗ п.3.8).
- `Views/AnalyticsWindow` — статистика груп і динаміка студента.
- `Views/ResultDetailsWindow`, сертифікат у `Views/StudentWindow` (ТЗ п.3.8.3).

## Збірка
```
dotnet build TestingSystem.csproj
```

## Далі
- Тиждень 7: лекції (Markdown), групи, локалізація, теми, блокування сеансу.
