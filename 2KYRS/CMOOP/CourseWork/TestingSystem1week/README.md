# TestingSystem — тиждень 1 (ТЗ розд. 6, етап 1)

Аналіз предметної області, моделі даних, ТЗ.

## Що тут (з фінального `TestingSystem`, без змін)
- `Models/Entities.cs`, `Models/Enums.cs` — сутності `User`, `Group`, `Category`, `Test`, `Question`, `Answer`, `TestSession`, `Result`, `StudentAnswerDetail` + енуми `Role`, `QuestionType`, `TextEvaluationType`, `ResultStatus`.
- `Abstractions/IUserRepository.cs`, `Abstractions/IGroupRepository.cs` — контракти сховища.
- `Persistence/InMemoryUserRepository.cs`, `Persistence/InMemoryGroupRepository.cs` — in-memory реалізації (заміна на SQLite/EF Core — лише реєстрації в `App.xaml.cs`).
- `App.xaml` — як у фіналі.

## Своє для тижня 1
- `App.xaml.cs` — мінімальний Composition Root: лише два репозиторії + сід двох груп.
- `Views/MainWindow` — перевірка шарів: списки груп/користувачів, додавання групи.

## Збірка
```
dotnet build TestingSystem.csproj
```

## Далі
- Тиждень 2: архітектура DI повністю, `AuthService` (PBKDF2), ролі, вхід/реєстрація, `DemoDataSeeder` (користувачі+групи).
