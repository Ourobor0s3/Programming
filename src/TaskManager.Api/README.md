# TaskManager API

ASP.NET Core Web API для управления задачами с поддержкой PostgreSQL, фонового экспорта и поиска.

## Возможности

- ✅ CRUD операции для задач (GET, POST, PUT, DELETE)
- ✅ Razor Pages для веб-интерфейса
- ✅ Endpoint экспорта задач в JSON (`GET /api/tasks/export`)
- ✅ Endpoint поиска с пагинацией (`GET /api/tasks/search?q=...&page=...&size=...`)
- ✅ BackgroundService для автоматического периодического экспорта
- ✅ Swagger UI для тестирования API
- ✅ Валидация моделей на сервере

## Требования

- .NET 8.0 SDK
- PostgreSQL (порт 5433)
- База данных `data-tasks` должна существовать

## Настройка

### 1. База данных

Убедитесь, что PostgreSQL запущен и доступен на `localhost:5433`. 

Строка подключения настраивается в `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5433;Database=data-tasks;Username=postgres;Password=admin"
  }
}
```

### 2. Миграции

Миграции применяются автоматически при запуске приложения. Если нужно применить вручную:

```bash
dotnet ef database update --context TaskDbContext
```

### 3. Настройка BackgroundService

В `appsettings.json` можно настроить интервал экспорта:

```json
{
  "ExportService": {
    "ExportIntervalSeconds": 60,
    "ExportFilePath": "tasks_export.json"
  }
}
```

## Запуск

### Вариант 1: Через .bat файл

Из корневой директории `src`:
```bash
run-server.bat
```

### Вариант 2: Через dotnet CLI

```bash
cd TaskManager.Api
dotnet run
```

### Вариант 3: Через Visual Studio / Rider

Просто запустите проект `TaskManager.Api`.

## Endpoints API

### Получить все задачи
```
GET /api/tasks
```

### Получить задачу по ID
```
GET /api/tasks/{id}
```

### Создать задачу
```
POST /api/tasks
Content-Type: application/json

{
  "title": "Новая задача",
  "description": "Описание задачи",
  "isDone": false,
  "dueDate": "2024-12-01T10:00:00Z"
}
```

### Обновить задачу
```
PUT /api/tasks/{id}
Content-Type: application/json

{
  "id": 1,
  "title": "Обновленная задача",
  "description": "Новое описание",
  "isDone": true,
  "dueDate": "2024-12-01T10:00:00Z"
}
```

### Удалить задачу
```
DELETE /api/tasks/{id}
```

### Экспорт задач
```
GET /api/tasks/export
```
Возвращает JSON со всеми задачами и сохраняет их в файл `tasks_export.json`.

### Поиск задач с пагинацией
```
GET /api/tasks/search?q=задача&page=1&size=10
```

Параметры:
- `q` (опционально) - поисковый запрос (поиск в названии и описании)
- `page` (по умолчанию 1) - номер страницы
- `size` (по умолчанию 10, максимум 100) - размер страницы

Ответ:
```json
{
  "items": [...],
  "totalCount": 25,
  "page": 1,
  "pageSize": 10,
  "totalPages": 3
}
```

## Веб-интерфейс

После запуска сервера доступны следующие страницы:

- `http://localhost:5233/` - Главная страница со списком задач (Razor Pages)
- `http://localhost:5233/Create` - Создание новой задачи
- `http://localhost:5233/Edit/{id}` - Редактирование задачи
- `http://localhost:5233/swagger` - Swagger UI для тестирования API

## BackgroundService

`ExportBackgroundService` автоматически экспортирует все задачи в файл `tasks_export.json` с заданным интервалом (по умолчанию каждые 60 секунд).

Логи экспорта можно увидеть в консоли или в логах приложения.

При остановке приложения сервис корректно завершает работу.

## Структура проекта

```
TaskManager.Api/
├── Data/
│   └── TaskDbContext.cs          # Контекст базы данных
├── Models/
│   └── TaskItem.cs               # Модель задачи
├── Services/
│   ├── ITaskService.cs           # Интерфейс сервиса
│   ├── Impl/
│   │   └── TaskService.cs        # Реализация сервиса
│   └── ExportBackgroundService.cs # Фоновый сервис экспорта
├── Pages/                         # Razor Pages
├── TasksController.cs             # API контроллер
├── Program.cs                     # Точка входа
└── appsettings.json               # Конфигурация
```

## Логирование

Логирование настраивается в `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "TaskManager.Api.Services.ExportBackgroundService": "Information"
    }
  }
}
```

## Разработка

### Добавление новой миграции

```bash
dotnet ef migrations add MigrationName --context TaskDbContext
```

### Применение миграций

```bash
dotnet ef database update --context TaskDbContext
```

### Откат миграции

```bash
dotnet ef database update PreviousMigrationName --context TaskDbContext
```

## Примечания

- При первом запуске автоматически создается таблица `tasks` в базе данных
- BackgroundService начинает работу сразу после запуска приложения
- Файл экспорта создается в корневой директории проекта (`tasks_export.json`)
