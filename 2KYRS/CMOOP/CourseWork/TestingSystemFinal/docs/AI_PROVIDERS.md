# AI-провайдери: як підключити та як додати новий

Цей документ пояснює, як влаштована AI-підсистема застосунку,
як увімкнути **Google Gemini** (або OpenAI / Anthropic / локальну модель)
і як додати **новий провайдер** без змін бізнес-логіки.

## 1. Архітектура (коротко)

| Шар | Файл | Призначення |
|---|---|---|
| `AiProviderKind` | `Abstractions/Ai.cs` | Перелік провайдерів (`Mock`, `OpenAiCompatible`, `Anthropic`, `Gemini`) |
| `IAiProvider` | `Abstractions/Ai.cs` | Стратегія: один клас = один API. Метод `CompleteAsync(settings, request, ct)` повертає **сирий текст** відповіді моделі |
| `IAiClient` / `AiClientRouter` | `Services/Ai/AiClientRouter.cs` | Обирає провайдера за поточними налаштуваннями, накладає таймаут. **Не змінюється** при додаванні провайдера |
| `IAiSettingsStore` / `JsonAiSettingsStore` | `Services/Ai/JsonAiSettingsStore.cs` | Зберігає провайдер/модель/адресу в `%AppData%\TestingSystem\ai-settings.json`, ключ шифрує DPAPI |
| `IAiAssistant` / `AiAssistant` | `Services/Ai/AiAssistant.cs` | Предметна логіка: теми → лекція → тест → оцінка. Викликає `IAiClient`, парсить JSON |
| `AiPrompts` | `Services/Ai/AiPrompts.cs` | Тексти запитів. Усі провайдери отримують **ті самі** `SystemPrompt` / `UserPrompt` |
| `AiJson` | `Services/Ai/AiJson.cs` | Витягує JSON-об'єкт із відповіді (обрізає ```-огорожі, текст навколо) і десеріалізує |
| `AiTestMapper` | `Services/Ai/AiTestMapper.cs` | Валідує питання тими ж правилами, що й ручний конструктор; биті — відкидає (лічильник `SkippedQuestions`) |
| `AiHttp` | `Services/Ai/AiHttp.cs` | Спільна відправка HTTP + мапінг статусів у `AiErrorKind` (401/403 → Unauthorized, 429 → RateLimited, 5xx → ServerError). Ключі в повідомлення не потрапляють |
| Налаштування UI | `ViewModels/AiSettingsViewModel.cs`, `Views/AiSettingsWindow.xaml` | Вибір провайдера, адреса/модель/ключ/таймаут, кнопка «Перевірити з'єднання» |
| Рядки UI | `Services/Strings.cs` | `Ai_Provider_*`, `Ai_Err_*`, `Err_Ai_*` (UA/RU/EN) |

Потік запиту:

```
AiAssistant → AiPrompts (запит) → AiClientRouter (вибір провайдера + таймаут)
→ XxxProvider (HTTP до свого API, повертає текст) → AiJson.Parse → AiTestMapper (валідація)
```

Провайдер **не знає** про лекції/тести — він лише передає промпти і повертає текст.
Тому новий API = новий клас `IAiProvider`, решта не чіпається (принцип OCP).

## 2. Вбудовані провайдери

| Провайдер | Протокол | Ключ | Типова адреса | Типова модель |
|---|---|---|---|---|
| `Mock` | — (офлайн-шаблони) | не потрібен | — | — |
| `OpenAiCompatible` | `POST {BaseUrl}/chat/completions` | потрібен (крім localhost) | `https://api.openai.com/v1` | `gpt-4o-mini` |
| `Anthropic` | `POST {BaseUrl}/v1/messages` | обов'язковий | `https://api.anthropic.com` | `claude-sonnet-5-5` |
| `Gemini` | `POST {BaseUrl}/v1beta/models/{model}:generateContent?key=...` | обов'язковий | `https://generativelanguage.googleapis.com` | `gemini-2.0-flash` |

Через `OpenAiCompatible` працюють також **OpenRouter, Azure OpenAI, Ollama, LM Studio**:
достатньо вписати їх `BaseUrl` (для Ollama типово `http://localhost:11434/v1`, ключ можна лишити порожнім).

## 3. Як увімкнути Google Gemini

1. Отримайте безкоштовний ключ: https://aistudio.google.com/apikey
   (увійдіть Google-акаунтом → **Create API key**).
2. У застосунку: **AI-помічник → ⚙ Налаштування AI → Провайдер → Google Gemini**.
3. Поля заповняться самі:
   - адреса: `https://generativelanguage.googleapis.com` (лише базовий хост, без `/v1beta/models/…` —
     навіть якщо вставити повний URL ендпоінта, провайдер сам відріже зайве і візьме модель з поля «Модель»)
   - модель: `gemini-3.8-flash` (старі назви на кшталт `gemini-2.0-flash` Google періодично відключає —
     якщо у відповідь прийде 404 «model ... is no longer available», впишіть актуальну назву з https://ai.google.dev/models/gemini)
4. Вставте ключ у поле **API-ключ** (зберігається зашифрованим, DPAPI — лише ваш користувач Windows).
5. Натисніть **«Перевірити з'єднання»** — має з'явитись «З'єднання успішне (відповідь: OK)».
6. **Зберегти**.

Інші моделі вписуються вручну в поле «Модель» (актуальний список: https://ai.google.dev/models/gemini).

Запасний варіант без UI: змінна середовища `TESTING_SYSTEM_AI_KEY` підставляє ключ,
якщо файл налаштувань ще не створено.

## 4. Як додати новий AI-провайдер (покроково)

Приклад: провайдер `MyProvider` для API `https://api.example.com`.

**Крок 1 — enum.** `Abstractions/Ai.cs`:

```csharp
public enum AiProviderKind { Mock, OpenAiCompatible, Anthropic, Gemini, MyProvider }
```

**Крок 2 — клас провайдера.** Створіть `Services/Ai/MyProvider.cs`:

```csharp
public sealed class MyProvider : IAiProvider
{
    private readonly HttpClient _http;
    public MyProvider(HttpClient http) => _http = http;
    public AiProviderKind Kind => AiProviderKind.MyProvider;

    public async Task<string> CompleteAsync(AiSettings settings, AiRequest request, CancellationToken ct)
    {
        if (!settings.HasKey) throw new AiException(AiErrorKind.NotConfigured, "API key is empty.");
        if (string.IsNullOrWhiteSpace(settings.Model)) throw new AiException(AiErrorKind.NotConfigured, "Model is empty.");

        string baseUrl = (string.IsNullOrWhiteSpace(settings.BaseUrl) ? "https://api.example.com" : settings.BaseUrl).TrimEnd('/');
        var body = new { model = settings.Model, system = request.SystemPrompt, prompt = request.UserPrompt };

        using var message = new HttpRequestMessage(HttpMethod.Post, baseUrl + "/v1/complete")
        {
            Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
        };
        message.Headers.Add("x-api-key", settings.ApiKey);   // або Authorization: Bearer — як вимагає ваш API

        string json = await AiHttp.SendAsync(_http, message, ct);   // помилки HTTP → AiErrorKind без витоку ключа
        try
        {
            using var doc = JsonDocument.Parse(json);
            string? text = doc.RootElement.GetProperty("text").GetString();
            return string.IsNullOrWhiteSpace(text) ? throw new AiException(AiErrorKind.InvalidResponse, "Empty completion.") : text;
        }
        catch (Exception ex) when (ex is JsonException or KeyNotFoundException or InvalidOperationException)
        {
            throw new AiException(AiErrorKind.InvalidResponse, "Unexpected response format.", ex);
        }
    }
}
```

Правила:

- Використовуйте **лише** `request.SystemPrompt` / `request.UserPrompt` (поля `Topic`/`Count` — тільки для `Mock`).
- Відповідь має бути **сирим текстом** (зазвичай JSON, який згенерувала модель); парсинг робить `AiAssistant` через `AiJson`.
- Не логуйте і не включайте `settings.ApiKey` у винятки — `AiHttp` цього вже дотримується.
- Для перевірки з'єднання UI надсилає `AiPrompts.Ping()` і чекає текст `OK` — переконайтесь, що провайдер повертає його як є.

**Крок 3 — реєстрація.** `App.xaml.cs`, поруч з іншими провайдерами:

```csharp
sc.AddSingleton<IAiProvider, MyProvider>();
```

`AiClientRouter` підхопить його автоматично (шукає за `Kind`).

**Крок 4 — налаштування UI.** `ViewModels/AiSettingsViewModel.cs`:

- додайте опцію в `Providers`: `new Option<AiProviderKind>(AiProviderKind.MyProvider, loc["Ai_Provider_MyProvider"])`;
- додайте типові адресу/модель у `AiProviderDefaults.For(...)`;
- якщо ключ обов'язковий — розширте перевірку: `if (kind is AiProviderKind.Anthropic or AiProviderKind.Gemini or AiProviderKind.MyProvider && key.Length == 0) ...`.

**Крок 5 — рядки локалізації.** `Services/Strings.cs`:

```csharp
["Ai_Provider_MyProvider"] = ("Моя модель (опис)", "Моя модель (описание)", "My provider (description)"),
```

**Крок 6 — перевірка:**

1. `dotnet build` — має зібратись без помилок;
2. запустіть застосунок → AI-помічник → Налаштування → оберіть новий провайдер → **«Перевірити з'єднання»**;
3. згенеруйте теми → лекцію → тест у режимі нового провайдера.

Готово. `AiAssistant`, `AiClientRouter`, вікна та ViewModelʼі міняти **не потрібно**.

## 5. Діагностика

| Симптом | Причина → дія |
|---|---|
| «AI не налаштовано» (`NotConfigured`) | порожня модель або (для Anthropic/Gemini) ключ → заповніть поля |
| «Немає зв'язку» (`Network`) | немає інтернету / невірна адреса / проксі → перевірте `BaseUrl` |
| «Ключ недійсний» (`Unauthorized`, HTTP 401/403) | невірний/відкликаний ключ → перевипустіть; для Gemini ключ береться з aistudio.google.com |
| «Ліміт запитів» (`RateLimited`, HTTP 429) | вичерпано квоту (у Gemini free-tier — денні ліміти) → зачекайте або змініть модель/тариф |
| «Помилка на боці сервісу» (`ServerError`, 5xx) | збій провайдера → повторіть пізніше |
| «Неочікуваний формат» (`InvalidResponse`) | модель повернула не-JSON (наприклад, відмову або текст замість JSON) → спробуйте ще раз; якщо повторюється — подивіться, чи модель підтримує JSON-режим |
| «Час вичерпано» (`Timeout`) | повільна мережа/великий запит → збільште таймаут (до 300 с) |
| Пошкоджений `ai-settings.json` | застосунок мовчки падає в безпечний режим `Mock` → просто переналаштуйте провайдера |

Приватність: зовнішньому провайдеру (будь-якому, крім `Mock`) надсилаються тексти
лекцій/тестів і відповіді студентів **без імен**. Не використовуйте його для
конфіденційних матеріалів без дозволу закладу (попередження `Ai_Privacy` в UI).
