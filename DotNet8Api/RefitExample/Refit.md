## Как работает Refit?

Основная идея Refit — это использование интерфейсов для определения API и атрибутов для указания соответствующих HTTP-запросов. Пример ниже показывает, как можно настроить простой API-клиент для получения информации о пользователе с GitHub:

```csharp
using Refit;
using System.Threading.Tasks;

public interface IGitHubApi
{
    [Get("/users/{username}")]
    Task<GitHubUser> GetUserAsync(string username);
}
```

Приммер использования GithubApi в контроллере:
```csharp
[HttpGet("{username}")]
public async Task<IActionResult> GetUser(string username)
{
    var user = await _gitHubApi.GetUserAsync(username);

    return Ok(user);
}
```
## Преимущества использования Refit
1. Упрощение кода
Refit значительно уменьшает объем кода, необходимого для выполнения HTTP-запросов. Вам больше не нужно вручную создавать и настраивать экземпляры HttpClient, а также обрабатывать сериализацию и десериализацию данных.

2. Читаемость и поддерживаемость
API-интерфейсы с использованием Refit легко читаются и поддерживаются, так как они представлены в виде интерфейсов с четко определенными методами и атрибутами. Такой подход упрощает навигацию и модификацию кода.

3. Интеграция с Dependency Injection
Refit отлично интегрируется с механизмом Dependency Injection в .NET, что упрощает тестирование и позволяет легко заменять реальные API-клиенты на mock-версии для unit-тестов.

## Пример использования Refit с двумя внутренними API

Предположим, у вас есть два внутренних API в вашем проекте: `IInternalApi1` и `IInternalApi2`. Вместо того чтобы вручную управлять вызовами к этим API, вы можете использовать Refit для их инкапсуляции в интерфейсах и работы с ними так, как если бы они были внешними сервисами.

### 1. Определение интерфейсов для API

```csharp
public interface IInternalApi1
{
    [Get("/data/{id}")]
    Task<ApiResponse<DataModel>> GetDataAsync(string id);
}

public interface IInternalApi2
{
    [Post("/process")]
    Task<ApiResponse<ProcessResult>> ProcessDataAsync([Body] DataModel data);
}
```

### 2. Настройка и использование Refit-клиентов
```csharp
public class MyService
{
    private readonly IInternalApi1 _api1;
    private readonly IInternalApi2 _api2;

    public MyService(IInternalApi1 api1, IInternalApi2 api2)
    {
        _api1 = api1;
        _api2 = api2;
    }

    public async Task<ProcessResult> ExecuteAsync(string id)
    {
        // Вызов первого API
        var data = await _api1.GetDataAsync(id);
        
        // Обработка данных с помощью второго API
        var result = await _api2.ProcessDataAsync(data.Content);

        return result;
    }
}
```
### 3. Регистрация клиентов в DI-контейнере

```csharp
builder.Services.AddRefitClient<IInternalApi1>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://internalapi1.com"));

builder.Services.AddRefitClient<IInternalApi2>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://internalapi2.com"));
```
