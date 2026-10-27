# TestingSystem — тиждень 3 (ТЗ розд. 6, етап 3, частина 1)

Конструктор тестів: перші 3 типи питань + валідація.

## Що тут (з фінального `TestingSystem`, без змін)
- Усе з тижня 2.
- `Abstractions/ITestRepository.cs`, `Abstractions/ICategoryRepository.cs`.
- `Persistence/InMemoryTestRepository.cs`, `Persistence/InMemoryCategoryRepository.cs`.

## Своє для тижня 3
- `Services/TestValidator.cs` — правила ТЗ п.3.3.4 (у фіналі їх дублюють `Views/TestEditorWindow` і `Services/Ai/AiTestMapper`).
- `Persistence/DemoDataSeeder.cs` — додано категорії + демо-тест «Основи баз даних».
- `Views/TeacherWindow` (список тестів), `Views/TestEditorWindow` (типи один/кілька/текст), `Views/StudentWindow` (заглушка списку).

## Збірка
```
dotnet build TestingSystem.csproj
```

## Далі
- Тиждень 4: типи відповідність / послідовність / файл + повні налаштування (час, спроби, пін-код, групи, перемішування).
