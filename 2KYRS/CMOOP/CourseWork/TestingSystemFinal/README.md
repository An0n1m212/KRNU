# TestingSystem — фінал (ТЗ розд. 6, етап 8: тижні 9–10)

Тестування, виправлення, пояснювальна записка, захист.

Ця папка — **точна копія фінального проєкту `TestingSystem`** (той самий `TestingSystem.csproj`,
ті самі шари `Abstractions → Services → Persistence → ViewModels / Views`, DI в `App.xaml.cs`).
Жодних спрощень тижнів 1–8 тут немає: повні MVVM-вікна, `ViewModelFactory`, `WpfDialogService`,
`GradingEngine`, `TestSessionService/Controller`, `AntiCheatMonitor`, журнал/аналітика/експорт
(`QuestPDF`), лекції (Markdown), групи, UA/RU/EN + теми, блокування сеансу (30 хв, DPAPI),
AI-помічник (Mock / OpenAI-сумісний / Anthropic / Gemini).

## Збірка
```
dotnet build TestingSystem.csproj
```

## Приймання (ТЗ розд. 7)
- Сценарій: реєстрація студента → створення тесту з усіма типами → призначення групі →
  проходження (в т.ч. втрата фокуса) → автоперевірка → ручна перевірка → аналітика/експорт/сертифікат →
  генерація лекції й тесту через AI (Mock і реальний провайдер).
- Граничні випадки: порожній тест, вичерпані спроби, невірний пін-код, файл > 10 МБ,
  дублікат групи, видалення групи зі студентами, недоступний AI-сервіс, пошкоджений JSON від AI.
- Демо: `teacher` / `student1` / `student2` / `admin`, пароль `1234`; пін демо-тесту `1234`.

## Як читати історію по тижнях
- `TestingSystem1week` — етап 1: моделі, контракти, in-memory сховище.
- `TestingSystem2week` — етап 2: DI, `AuthService` (PBKDF2), ролі, демо-дані.
- `TestingSystem3week` — етап 3 (ч.1): конструктор, 3 типи, валідація.
- `TestingSystem4week` — етап 3 (ч.2): усі 6 типів + налаштування.
- `TestingSystem5week` — етап 4: проходження, таймер, античит, `GradingEngine`.
- `TestingSystem6week` — етап 5: перевірка, журнал, аналітика, експорт, сертифікати.
- `TestingSystem7week` — етап 6: лекції, групи, локалізація, теми, блокування.
- `TestingSystem8week` — етап 7: AI-помічник.
- `TestingSystemFinal` (ця папка) — етап 8: ревізія = `TestingSystem`.
