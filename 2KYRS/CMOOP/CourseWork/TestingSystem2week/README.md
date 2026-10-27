# TestingSystem — тиждень 2 (ТЗ розд. 6, етап 2)

Архітектура (шари, DI), автентифікація, ролі, демо-дані.

## Що тут (з фінального `TestingSystem`, без змін)
- Усе з тижня 1 (`Models/Entities`, `Models/Enums`, репозиторії користувачів/груп).
- `Models/Lecture.cs` — потрібен контракту `IAuthService.CanManage(Lecture)` (UI лекцій — тиждень 7).
- `Abstractions/IAuthService.cs` — контракт.
- `Services/PasswordHasher.cs` — PBKDF2 із сіллю (ТЗ п.3.2.2).
- `Services/AuthService.cs` — вхід, реєстрація лише студентів, профілі, зміна пароля, права `CanManage`.

## Своє для тижня 2
- `Persistence/DemoDataSeeder.cs` — спрощений сід: групи + `admin/teacher/student1/student2` (1234). Повний сідер фіналу (тести, результати, лекції) прийде пізніше.
- `App.xaml.cs` — DI: репозиторії + `IAuthService`; навігація вхід → реєстрація → головна.
- `Views/LoginWindow`, `Views/RegisterWindow`, `Views/MainWindow` — спрощені попередники фінальних `LoginWindow` / `StudentWindow` / `TeacherWindow`.

## Збірка
```
dotnet build TestingSystem.csproj
```
Демо: `teacher`, `student1`, `admin` — пароль `1234`.

## Далі
- Тижні 3–4: конструктор тестів (6 типів питань, валідація).
