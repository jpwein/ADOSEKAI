# lsnSekai24

Итоговый вариант задания 24 на ASP.NET Core и Entity Framework Core.

В проекте есть модели и таблицы `Students`, `Groups`, `Subjects`, `Teachers`, `Grades`, CRUD-маршруты `Get`, `Post`, `Put`, `Delete` для каждой модели и фронтенд с `fetch`.

Запуск:

```powershell
cd .\lsnSekai24
dotnet restore
dotnet run --launch-profile http
```

Открыть: http://localhost:5240/ или http://localhost:5240/sexai

Строка подключения задаётся в `appsettings.json` либо через переменную `ConnectionStrings__DefaultConnection`. Для текущего компьютера используется сервер `WEIN\SEKAIITD`.

Если таблицы ещё не созданы, выполните скрипт `Database/CollegeDB.sql` в SQL Server Management Studio.

Для исправления ошибки доступа выполните `Database/GrantCollegeDbAccess.sql` в SSMS от имени администратора. Скрипт выдаёт учётной записи приложения права на чтение, добавление, изменение и удаление. Посетителям сайта отдельные SQL-учётные записи не нужны: они работают через открытые CRUD-маршруты приложения.
