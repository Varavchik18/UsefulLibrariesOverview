## Что такое Polly 
Polly — это мощная библиотека для управления надежностью и отказоустойчивостью в .NET приложениях. Она предоставляет ряд политик для обработки ошибок и сбоев при взаимодействии с внешними сервисами, такими как повторные попытки, прерыватель цепи, тайм-ауты и другие.

## Основные политики Polly

### 1. Политика повторных попыток (Retry)

**Retry Policy** позволяет автоматически повторять попытку выполнения операции в случае, если она завершилась неудачно из-за временной ошибки, такой как `HttpRequestException`. Вы можете настроить количество попыток и интервал между ними.

```csharp
var retryPolicy = Policy
    .Handle<HttpRequestException>()
    .OrResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.InternalServerError)
    .RetryAsync(3); // Повторить 3 раза при ошибке
```
Политика с экспоненциальной задержкой между попытками:
```csharp
var retryPolicyWithDelay = Policy
    .Handle<HttpRequestException>()
    .OrResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.InternalServerError)
    .WaitAndRetryAsync(3, retryAttempt =>
        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Задержка 2, 4, 8 секунд
        (outcome, timespan, retryCount, context) =>
        {
            // Логирование попыток
            Log.Warning($"Retry {retryCount} implemented with {timespan} seconds delay due to {outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()}");
        });
```

### 2. Прерыватель цепи (Circuit Breaker)
Circuit Breaker Policy предназначена для защиты системы от излишнего износа при многократных сбоях. После определенного количества ошибок цепь "разрывается", и все последующие запросы немедленно отвергаются в течение заданного времени.

```csharp
var circuitBreakerPolicy = Policy
    .Handle<HttpRequestException>()
    .CircuitBreakerAsync(
        exceptionsAllowedBeforeBreaking: 3, // Допустимо 3 ошибки перед разрывом цепи
        durationOfBreak: TimeSpan.FromSeconds(30)); // Время разрыва цепи - 30 секунд
```

### 3. Тайм-аут (Timeout)
Timeout Policy задает максимальное время ожидания для выполнения операции. Если операция не завершилась за это время, она принудительно прерывается.

```csharp
var timeoutPolicy = Policy
    .TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10)); // Тайм-аут 10 секунд
```

### 4. Изоляция ресурсов (Bulkhead Isolation)
Bulkhead Policy позволяет ограничить количество параллельных операций, чтобы предотвратить исчерпание ресурсов и перегрузку системы.

``` csharp 
var bulkheadPolicy = Policy
    .BulkheadAsync<HttpResponseMessage>(maxParallelization: 10, maxQueuingActions: 5); 
    // Максимум 10 параллельных операций, 5 в очереди

```

### 5. Резервный вариант (Fallback)
Fallback Policy позволяет определить резервное действие или результат, который будет возвращен в случае сбоя основной операции.

``` csharp
var fallbackPolicy = Policy<HttpResponseMessage>
    .Handle<HttpRequestException>()
    .OrResult(msg => msg.StatusCode == HttpStatusCode.InternalServerError)
    .FallbackAsync(new HttpResponseMessage(HttpStatusCode.OK)
    {
        Content = new StringContent("This is a fallback response")
    }); 
```
### 6. Комбинирование политик (Policy Wrap)
Polly позволяет комбинировать несколько политик, создавая цепочку из них. Например, вы можете объединить политику повторных попыток с тайм-аутом и прерывателем цепи:

```csharp
var combinedPolicy = Policy.WrapAsync(
    retryPolicyWithDelay, circuitBreakerPolicy, timeoutPolicy);
```
