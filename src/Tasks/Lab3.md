# Лабораторная работа — «Умный дом»

**Тема:** Работа с классами и объектами — инкапсуляция, наследование, полиморфизм, интерфейсы и абстрактные классы.

---

## Краткое задание

Создать мини‑систему **SmartHome**: абстрактный `Device`, конкретные устройства, комнаты (`Room`), контроллер дома (`HomeController`) и простые сценарии управления. Четыре задания: первые два — обязательные (1 — простой, 2 — средний), задания 3 и 4 — *опциональные*.

---

## Общие требования
- Код читаемый: понятные имена.  
- Инкапсуляция: приватные поля + публичные свойства.   
- README: краткая инструкция по запуску.

---

# Задания


## Задание 1 — Базовый SmartHome

**Цель:** реализовать минимальную архитектуру устройств и комнаты, продемонстрировать всё необходимое ООП-поведение.

**Требуется реализовать (минимум):**

1. `abstract class Device`
   - приватные поля: `_id` (string), `_name` (string), `_isOn` (bool);
   - свойства:
     - `public string Id { get; }` — устанавливается в конструкторе, например `Guid.NewGuid().ToString()`;
     - `public string Name { get; init; }` — нельзя null/пустую строку;
     - `public bool IsOn { get; protected set; }`;
   - абстрактное свойство `public abstract string Type { get; }`;
   - виртуальные методы `public virtual void TurnOn()` и `public virtual void TurnOff()` — по умолчанию переключают `IsOn` и пишут короткий лог; можно переопределять;
   - виртуальный метод `public virtual string Status()` — возвращает строку состояния.

2. `interface IControllable`
   - `void TurnOn(); void TurnOff();`.

3. Два подкласса: `Light` и `Thermostat` (наследуют `Device` и реализуют `IControllable`)
   - `Light`:
     - приватное поле `_brightness` и свойство `Brightness` с валидацией;
     - переопределяет `Type => "Light"` и `Status()`;
   - `Thermostat`:
     - приватное поле `_temperature` и свойство `Temperature` с валидацией разумного диапазона;
     - реализует `Type => "Thermostat"` и `Status()`.

4. `class Room`
   - приватный `List<Device> _devices`;
   - публичное `string Name { get; }`;
   - методы: `void AddDevice(Device d)`, `bool RemoveDevice(string id)`, `IReadOnlyList<Device> GetDevices()`;
   - метод `void TurnAllOn()` — проходит по устройствам и вызывает `TurnOn()` через интерфейс `IControllable` (если устройство его реализует).

5. **Demo (`Program.Main`)**
   - создать комнату, добавить `Light` и `Thermostat`, вывести `Status()` для всех устройств, вызвать `TurnAllOn()`, снова вывести `Status()`. Сделать добавление устройств и комнат пользователем. Добавить команду, которая показывает состояния всех устройств во всех комнатах с группировкой по комнатам.

---


## Задание 2* — Контроллер и простая автоматизация

**Цель:** добавить контроллер для автоматической логики (температурное правило), показать абстрактный базовый контроллер и интерфейс для сенсоров.

**Требуется реализовать:**

1. `abstract class ControllerBase`
   - приватное поле `_id`; `public string Id { get; }`;
   - защищённое поле или свойство `protected IReadOnlyList<Room> Rooms { get; }` или `protected List<Room>` (если нужно модифицировать в контроллере);
   - абстрактный метод `public abstract void Execute();`.

2. `interface ISensor`
   - метод `bool TryRead(out decimal value);` — безопасный способ получить числовое значение (для `Thermostat` реализовать этот интерфейс).

3. `class HomeController : ControllerBase`
   - конструктор принимает `IEnumerable<Room>`;
   - реализация `Execute()`:
     - обходит `Rooms` → во всех устройствах ищет реализацию `ISensor` (через `is ISensor s`) и `IControllable` (через `is IControllable c`);
     - если `Thermostat` (или другой сенсор) сообщает `value < threshold`, то найти устройство типа `Heater` в той же комнате и вызвать `TurnOn()`;
   - метод `void Run()` вызывает `Execute()` и логирует результат.

4. `Heater : Device, IControllable`
   - простая модель: `TargetTemperature` и логика включения; `Type => "Heater"`.

5. **Demo:** создать две комнаты; в одной — `Thermostat` с `Temperature = 18.0`, `Heater` выключен; вызвать `HomeController.Run()` → `Heater` включается.


## Задание 3** — Сцены и простая шина событий (опционально)

**Цель:** добавить механизмы групповых команд (сцен) и событий (простая шина) — продемонстрировать интерфейсы и абстрактную базу задач.

**Требуется:**
1. `abstract class SceneBase` с `public abstract void Execute();`.
   - `EveningScene : SceneBase` — включает свет во всех комнатах и устанавливает яркость.

2. `interface IEventListener { void OnEvent(string eventName, object? payload); }` и `class EventBus`:
   - `void Subscribe(string eventName, IEventListener listener)`;
   - `void Publish(string eventName, object? payload)` — вызывает `OnEvent` у подписанных слушателей.

3. Подписка: `Room` или `HomeController` может реализовать `IEventListener` и реагировать на событие `MotionDetected`, включая соответствующий `Light`.

4. **Demo:**  `MotionDetected` для комнаты — показать, что свет включился.
