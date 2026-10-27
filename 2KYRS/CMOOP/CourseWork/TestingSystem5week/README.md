# TestingSystem — тиждень 5 (ТЗ розд. 6, етап 4)

Проведення тестування: таймер, античит, автоперевірка.

## Що тут (з фінального `TestingSystem`, без змін)
- Усе з тижня 4.
- `Abstractions/IResultRepository.cs`, `Persistence/InMemoryResultRepository.cs`.
- `Services/GradingEngine.cs` — автоперевірка всіх типів (Single/Multiple/Text/Matching/Sequence; файл і ручний текст — на перевірку).
- `Services/TestSessionService.cs` — доступ за групами, спроби, пін-код, перемішування.
- `Services/TestSessionController.cs` — спроба: таймер, відповіді, кінцевий автомат статусів.
- `Services/AntiCheatMonitor.cs` — втрата фокуса/згортання (WPF `Deactivated`).
- `Services/AppFiles.cs` — вкладення (10 МБ, дозволені типи).

## Своє для тижня 5
- `Views/StudentWindow` — картки тестів (час, питання, прохідний, спроби, мітка PIN), старт, «Мої результати».
- `Views/AccessCodeDialog` — пін-код.
- `Views/TestRunWindow` — навігація, лічильник, таймер («менше хвилини»), підтвердження здачі, античит (5 порушень → автоздача), усі 6 типів вводу.
- `Views/ResultWindow` — підсумок: складено/не складено або попередній результат.

## Збірка
```
dotnet build TestingSystem.csproj
```
Демо-пін захищеного тесту: `1234`.

## Далі
- Тиждень 6: перевірка робіт, журнал, аналітика, експорт, сертифікати.
