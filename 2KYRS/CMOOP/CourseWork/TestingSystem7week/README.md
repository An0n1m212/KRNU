# TestingSystem — тиждень 7 (ТЗ розд. 6, етап 6)

Лекції (Markdown), керування групами, локалізація, теми, блокування сеансу.

## Що тут (з фінального `TestingSystem`, без змін)
- Усе з тижня 6.
- `Abstractions/ILectureRepository.cs`, `Abstractions/ILectureService.cs`, `Persistence/InMemoryLectureRepository.cs`, `Services/LectureService.cs`.
- `Infrastructure/MarkdownRenderer.cs`, `Infrastructure/MarkdownBehavior.cs` — безпечний рендер (заголовки, списки, код).
- `Abstractions/IThemeService.cs`, `Services/ThemeService.cs`, `Themes/LightColors.xaml`, `Themes/DarkColors.xaml`.
- `Infrastructure/TExtension.cs` — `{loc:T …}` (мова інтерфейсу = мова AI у тижні 8).
- `Abstractions/ISessionLock.cs`, `Services/SystemClock.cs`, `Services/InactivityMonitor.cs`, `Services/SessionLockService.cs`, `Infrastructure/DispatcherTicker.cs`, `Infrastructure/WpfInputActivitySource.cs`.

## Своє для тижня 7
- `Views/LecturesView` + `Views/LectureEditorWindow` (чернетка/публікація, прив'язка тесту), `Views/StudentLecturesView` (читання + перехід до тесту).
- `Views/GroupsView` — лише адмін, дублікати/порожні/непорожні групи (ТЗ п.3.9).
- Перемикачі UA/RU/EN і світла/темної теми у вікнах; `Views/SessionLockWindow` + `Views/SimpleLockPresenter` (30 хв, ліміт спроб, вихід).

## Збірка
```
dotnet build TestingSystem.csproj
```

## Далі
- Тиждень 8: AI-помічник (провайдери, генерація, оцінювання).
