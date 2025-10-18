# Лабораторная работа 6 — «Склад и отчёты»

## Описание задачи (общая концепция)
Создать приложение **InventoryManager**, которое:
- Импортирует прайсы и остатки из `catalog.csv` и `supplier_feed.xml`.
- Сохраняет/обновляет данные в БД (`Products`, `PriceHistory` и т.д.).
- Реализует перемещения между складами с транзакционной гарантией.
- Формирует XML-отчёт и применяет XSLT-преобразование для получения HTML-страницы отчёта.
- Экспортирует JSON.

## Структура данных — ключевые сущности
Примеры сущностей (сокращённо):
```csharp
public class Product { public string ProductCode {get;set;} public string Name {get;set;} public decimal Price {get;set;} public int Stock {get;set;} }
public class Warehouse { public int Id {get;set;} public string Name {get;set;} }
public class StockMovement { public int Id {get;set;} public string ProductCode {get;set;} public int FromWarehouseId {get;set;} public int ToWarehouseId {get;set;} public int Quantity {get;set;} public DateTime When {get;set;} }
```

## Задания
### Задание 1
**Цель:** импорт CSV с UPSERT-логикой и экспорт в JSON/XML.
**Требуется:**
1. Прочитать `catalog.csv` (ProductCode,Name,Price,Stock) и выполнить UPSERT в таблицу `Products`. Для SQLite используйте `INSERT OR REPLACE`/`ON CONFLICT` или альтернативную реализацию.
2. Сгенерировать `inventory.json` и `inventory.xml` (полный список товаров).
3. Логировать изменения и количество обновлённых/вставленных записей.

---
### Задание 2
**Цель:** реализовать модель с EF Core и отчёты через ADO.NET.
**Требуется:**
1. Создать EF Core модель (`Product`, `Warehouse`, `StockMovement`) и миграции.
2. Реализовать сценарий перемещения товара между складами (транзакция через EF) с проверкой остатков. Перемещение осуществляется пользователем путём нажатия кнопок или ввода команд.
3. Реализовать быстрый агрегатный отчёт наличий по складам через ADO.NET (агрегация SUM), экспорт результата в XML и JSON.
4. Применять XSLT (`inventory.xslt`) для генерации HTML-версии отчёта.

---
### Задание 3
**Цель:** ведение истории цен.
**Требуется:**
1. Реализовать импорт `supplier_feed.xml`, обновлять цены в `Products`. Если цена изменилась более чем на X% (X — параметр, который задаётся пользователем в консоли/интерфейсе), записывать запись в `PriceHistory` (old/new/timestamp).
2. Логировать все изменения и поддерживать идемпотентный импорт (повторный импорт тех же данных не должен дублировать историю без изменений цены).

