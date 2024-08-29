# Паттерн IOptions в .NET

## Что такое IOptions?

`IOptions` — это встроенный в .NET механизм, который предоставляет простой способ доступа к настройкам конфигурации вашего приложения. Он позволяет вам организованно и безопасно работать с конфигурационными данными, избегая использования магических строк и жестко закодированных значений.

## Пример использования IOptions

### Шаг 1: Создание класса настроек

Создайте класс, который будет представлять вашу конфигурацию:

```csharp
public class GitHubSettings
{
    public string BaseAddress { get; set; }
    public string AccessToken { get; set; }
    public string UserAgent { get; set; }
}

```

### Шаг 2: Настройка конфигурации в appsettings.json
Добавьте соответствующие настройки в appsettings.json:

```json
{
  "GitHubSettings": {
    "BaseAddress": "https://api.github.com/",
    "AccessToken": "your-access-token",
    "UserAgent": "DotNet-Client"
  }
}
```

### Шаг 3: Регистрация IOptions в DI-контейнере
Зарегистрируйте настройки в контейнере внедрения зависимостей (DI) в Program.cs:
```csharp
builder.Services.Configure<GitHubSettings>(builder.Configuration.GetSection("GitHubSettings"));
```

### Шаг 4: Внедрение и использование IOptions
Внедрите IOptions<GitHubSettings> в ваш класс и используйте его для доступа к настройкам:
```csharp
public class GitHubService
{
    private readonly GitHubSettings _settings;

    public GitHubService(IOptions<GitHubSettings> options)
    {
        _settings = options.Value;
    }

    public void DisplaySettings()
    {
        Console.WriteLine($"BaseAddress: {_settings.BaseAddress}");
        Console.WriteLine($"UserAgent: {_settings.UserAgent}");
    }
}


```
