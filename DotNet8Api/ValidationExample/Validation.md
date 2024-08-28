**FluentValidation** — это популярная библиотека для .NET, которая предоставляет удобный способ реализации сложной валидации данных. Вместо использования атрибутов, как в `DataAnnotations`, FluentValidation позволяет создавать мощные и гибкие валидаторы с четко определенными правилами.

Чтобы создать валидатор для модели, достаточно унаследоваться от `AbstractValidator<T>` и определить правила в конструкторе класса.

```csharp
using FluentValidation;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(user => user.Username)
            .NotEmpty().WithMessage("Username is required.")
            .Length(3, 50).WithMessage("Username must be between 3 and 50 characters.");

        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(user => user.Age)
            .InclusiveBetween(18, 99).WithMessage("Age must be between 18 and 99.");
    }
}
```
### Продвинутые возможности и модификации валидаторов
#### 2. Условная валидация
Иногда требуется, чтобы определенные правила применялись только при выполнении определенного условия.

``` csharp
RuleFor(user => user.Discount)
    .GreaterThan(0).When(user => user.IsPremiumMember)
    .WithMessage("Only premium members can have a discount.");
```
#### 3. Комплексная валидация
FluentValidation позволяет объединять несколько правил в цепочки для создания более сложной логики.

``` csharp
RuleFor(user => user.Password)
    .NotEmpty().WithMessage("Password is required.")
    .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
    .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
    .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
    .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
    .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
```
#### 4. Использование кастомных валидаторов
Мы можем создавать собственные методы валидации и использовать их в правилах.

```csharp
RuleFor(user => user.Email)
    .Must(BeAValidDomain).WithMessage("Email domain must be valid.");

private bool BeAValidDomain(string email)
{
    var domain = email.Split('@').Last();
    return domain == "example.com"; // Пример простого кастомного валидатора
}
```
#### 5. Валидация зависимых объектов (Complex Object Validation)
FluentValidation поддерживает валидацию сложных объектов, включая вложенные объекты и коллекции.

``` csharp
RuleFor(user => user.Address)
    .SetValidator(new AddressValidator());

RuleForEach(user => user.PhoneNumbers)
    .NotEmpty().WithMessage("Phone number is required.")
    .Matches(@"^\d{10}$").WithMessage("Phone number must be 10 digits.");
```
#### 6. Создание асинхронных валидаторов
FluentValidation поддерживает асинхронные правила валидации, что полезно для проверки данных, зависящих от внешних источников, например, базы данных.

``` csharp
RuleFor(user => user.Username)
    .MustAsync(async (username, cancellation) => 
    {
        return await IsUniqueUsername(username);
    })
    .WithMessage("Username must be unique.");

private async Task<bool> IsUniqueUsername(string username)
{
    // Логика для проверки уникальности имени пользователя
    return await _userRepository.IsUniqueUsername(username);
}
```
#### 7. Использование кастомных сообщений и локализация
FluentValidation позволяет гибко настраивать сообщения об ошибках и поддерживает локализацию.

``` csharp
RuleFor(user => user.Email)
    .NotEmpty().WithMessage(user => $"Email is required for user {user.Username}.")
    .EmailAddress().WithMessage("Please provide a valid email address.");
```
#### 8. Группировка и использование правил валидации
FluentValidation позволяет группировать правила, чтобы они применялись в разных контекстах, например, при создании или обновлении данных.

``` csharp
RuleSet("CreateUser", () =>
{
    RuleFor(user => user.Username).NotEmpty();
    RuleFor(user => user.Password).NotEmpty();
});

RuleSet("UpdateUser", () =>
{
    RuleFor(user => user.Email).NotEmpty();
});
```
#### 9. Валидация в зависимости от контекста (Validation Context)
Вы можете использовать контекст валидации для передачи дополнительной информации в валидатор, что позволяет более гибко управлять процессом валидации.

``` csharp
RuleFor(user => user.Age)
    .GreaterThan(18)
    .When(user => context.RootContextData.ContainsKey("IsAdultOnly") && (bool)context.RootContextData["IsAdultOnly"]);
```


### Как это использовать в коде:
``` csharp
public class CreateUserCommand
{
    public string Username { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
}

public class CreateUserCommandHandler
{
    private readonly IValidator<CreateUserCommand> _validator;

    public CreateUserCommandHandler(IValidator<CreateUserCommand> validator)
    {
        _validator = validator;
    }

    public async Task Handle(CreateUserCommand command)
    {
        var validationResult = await _validator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Логика создания пользователя
    }
}
```