# TestingSystem — тиждень 8 (ТЗ розд. 6, етап 7)

AI-помічник: провайдери, генерація, оцінювання.

## Що тут (з фінального `TestingSystem`, без змін)
- Усе з тижня 7.
- `Models/AiModels.cs`, `Abstractions/Ai.cs` (`IAiProvider`, `IAiClient`, `IAiAssistant`, `IAiSettingsStore`).
- `Services/Ai/*` — `MockAiProvider` (офлайн-демо), `OpenAiCompatibleProvider` (OpenAI/OpenRouter/Ollama/LM Studio), `AnthropicProvider`, `GeminiProvider`, `AiClientRouter`, `AiAssistant`, `AiPrompts`, `AiTestMapper` (валідація тими ж правилами, биті відкидаються з лічильником), `JsonAiSettingsStore` (DPAPI), `AiHttp`, `AiJson`, `AiDtos`, `AiErrorFormatter`.
- `docs/AI_PROVIDERS.md` — підключення та додавання провайдерів.

## Своє для тижня 8
- `Views/AiAssistantWindow` — теми → лекція → тест (чернеткою в редактори), прогрес + скасування.
- `Views/AiSettingsWindow` — вибір провайдера, DPAPI-ключ, таймаут 5–300 с, «Перевірити з'єднання» (очікується `OK`), вбудована інструкція.
- AI-підказки оцінок у `Views/ReviewWindow` (лише пропозиції; файли AI не читає).

## Збірка
```
dotnet build TestingSystem.csproj
```

## Далі
- `TestingSystemFinal` — етап 8 (тестування, ревізія): точний паритет з фінальним `TestingSystem`.
