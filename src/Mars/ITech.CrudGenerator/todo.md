Сделано:
* Проработать возвращаемые значения в эндпоинтах, статус коды ответов 
* Сделать фильтры, сортировки в списке 
* Автоматически форматировать сгенерированый код
* Сделать наименование plural для property массивов в фильтрах
* Сделать фильтры зависимые от БД на Like
* Прокинуть пользовательский DbContext
* Сделать дефолтную сортировку конфигурируемую откуда-нибудь
* Сделать nullable enable в сгенерированном коде
* Сделать включение отключение определенных генераторов
* Critical: Id сущности не находит, если перед Id стоит название сущности

Сделать:
* Bug: Конфигурация не считывается если указывать new() вместо полного названия типа new
* EntityGeneratorGetListOperationConfiguration()
* Bug: При изменении сущности, если обновляемый ключ зависит от связанной сущности, провалидировать, что связанная
* сущность существует
* Bug: В CreateCustomizedManageEntityEndpoint, endpoint не возвращает Get Route
* Bug: Что если существительное в единственном и множественном числе пишется одинаково?
* Bug: Название endpoint'ов (сам роут) должен использовать CamelCase ("/customoperationnameentity/{id}/customopupdate")
* Improve: В default sort направление задавать через enum
* Improve: сделать возможность прокидывать устанавливать Id на сущность через Create (из-за того, что в монго не работает
* ?: Проверить сущность без ID
* генерация int id, и потому что иногда для избавления от дубликации запросов Guid создают с фронта)
* Сделать валидаторы на create, update, delete
* Сделать using в фильтрах зависимый от используемых фильтров, а не стандартный на всё
* Сделать фильтры через связанные сущности, один ко многим и многие ко многим
* Сделать сортировки через связанные сущности один ко многим
* Сделать Patch запрос
* Прокинуть пользователя, который выполняет действие, в handler
* Сделать проверку с явным уведомлением, когда при кастомизации конфигурации у разных кофигураций получились одинаковые названия классов

Протестировано:
* Проработать возвращаемые значения в эндпоинтах, статус коды ответов
* Сделать включение отключение определенных генераторов и ендпоинтов
* Critical: Id сущности не находит, если перед Id стоит название сущности

Протестировать:
* Что Create эндпоинты возвращают ссылку на get
* Сделать наименование plural для property массивов в фильтрах
* Сделать фильтры зависимые от БД на Like
* Сделать nullable enable в сгенерированном коде

# Ограничения:
### Сортировка по GUID разная в БД и C#
Сортировка по GUID в БД отличается от сортировки в C#
https://github.com/dotnet/efcore/issues/10198,
https://github.com/dotnet/efcore/issues/10265
соответственно корретно протестировать сортировку GUID полей автоматически нельзя,
можно только задавая конкретную последовательность GUID'ов (пока не будет делатся)

### Поля с типом DateTimeOffset нужно дополнительно конфигурировать 
В MongoDB DateTimeOffset сохраняются как-то странно,
поэтому к полям с этим типам нужно применять конфигурацию
.HasBsonRepresentation(BsonType.DateTime) или .HasBsonRepresentation(BsonType.String);

### [Mongo] Int ID

В MongoDB для Id с типом Int нет генерации поля, соответственно, в качестве временного обхода был добавлен генератор

```csharp
public class MongoEfIntIdSequenceGenerator<T> : ValueGenerator<int> where T : class
{
    public override bool GeneratesTemporaryValues => false;

    public override int Next(EntityEntry entry)
    {
        var currInd = entry.Context.Set<T>().Count();

        return currInd + 1;
    }
}

// Регистрация
public class TestMongoDb : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ...
        modelBuilder.Entity<IntIdEntity>().Property(x => x.Id)
            .HasValueGenerator<MongoEfIntIdSequenceGenerator<IntIdEntity>>();
        // ...
    }
}
```