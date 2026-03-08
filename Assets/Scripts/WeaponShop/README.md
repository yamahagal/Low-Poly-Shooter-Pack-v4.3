# Магазин оружия - Руководство по настройке

## Обзор

Система магазина оружия для игры "Low Poly Shooter Pack v4.3" позволяет игрокам покупать оружие за игровую валюту.

**ДВА РЕЖИМА РАБОТЫ:**

1. **ScriptableObject режим** (оригинальный) - использует WeaponShopService
   - Оружия настраиваются через Unity Editor (Create > Weapon Shop > Weapon Data)
   - Подходит для редактирования в Unity Editor

2. **JSON режим** (новый) - использует WeaponShopServiceJSON
    - Оружия настраиваются через JSON файл
    - Вы можете редактировать список оружий прямо в файле
    - Подходит для динамического изменения доступного оружия
    - 📘 **Подробная инструкция**: [`JSON_WEAPON_SETUP.md`](Assets/Scripts/WeaponShop/JSON_WEAPON_SETUP.md)

## Структура проекта

```
Assets/Scripts/WeaponShop/
├── WeaponShop.asmdef              # Файл определения сборки (для TextMeshPro и других зависимостей)
├── Data/
│   ├── WeaponData.cs              # ScriptableObject конфигурации оружия
│   ├── WeaponDataJSON.cs          # Данные оружия для загрузки из JSON
│   └── InventoryData.cs           # Класс данных для инвентаря
├── Services/
│   ├── ICurrencyService.cs          # Интерфейс сервиса валюты
│   ├── CurrencyService.cs           # Реализация сервиса валюты
│   ├── IWeaponShopService.cs       # Интерфейс сервиса магазина оружия
│   ├── WeaponShopService.cs        # Реализация сервиса магазина (ScriptableObject режим)
│   └── WeaponShopServiceJSON.cs  # Реализация сервиса магазина (JSON режим)
├── UI/
│   ├── CurrencyBalanceUI.cs         # UI баланса валюты
│   ├── WeaponCardUI.cs             # UI карточки оружия
│   ├── WeaponListUI.cs             # UI списка оружия
│   ├── WeaponPreview3D.cs          # 3D предпросмотр оружия
│   ├── WeaponStatsPanel.cs          # UI панели параметров
│   └── WeaponShopScene.cs         # Основной скрипт сцены
├── Integration/
│   ├── InventoryIntegration.cs      # Интеграция с Inventory
│   └── InventoryManager.cs           # Менеджер инвентаря
└── README.md                      # Этот файл
```

## Быстрый старт - JSON режим

### 1. Редактирование JSON файла

Откройте файл [`Assets/saves/inventory_test.json`](Assets/saves/inventory_test.json) и добавьте/измените оружия в секции `weaponDefinitions`.

Структура оружия в JSON:
```json
{
  "weaponDefinitions": {
    "handgun_01": {
      "weaponId": "handgun_01",
      "weaponName": "Пистолет",
      "weaponType": "Handgun",
      "weaponPrefabPath": "Assets/Path/To/Weapon.prefab",
      "weaponIconPath": "Assets/Path/To/Icon.png",
      "baseCost": 0,
      "damage": 25,
      "fireRate": 300,
      "magazineCapacity": 12,
      "reloadTime": 1.5,
      "accuracy": 0.15,
      "range": 50
    }
  }
}
```

### 2. Настройка сцены магазина (JSON режим)

1. Создайте новую сцену или откройте существующую
2. Добавьте следующие компоненты:
   - **CurrencyService**: Сервис валюты
   - **WeaponShopServiceJSON**: Сервис магазина оружия (JSON режим)
   - **WeaponShopScene**: Основной скрипт сцены
   - **WeaponListUI**: UI списка оружия
   - **WeaponPreview3D**: 3D предпросмотр
   - **WeaponStatsPanel**: Панель параметров
   - **CurrencyBalanceUI**: UI баланса

3. Настройте ссылки в Inspector:
   - В `WeaponShopScene` добавьте ссылки на все UI компоненты
   - В `WeaponListUI` добавьте префаб карточки оружия
   - В `WeaponPreview3D` добавьте точку спавна и камеру
   - В `WeaponStatsPanel` настройте максимальные значения параметров
   - В `WeaponShopServiceJSON` настройте путь к файлу `saves/inventory_test.json`

## Быстрый старт - ScriptableObject режим

### 1. Создание конфигураций оружия

1. В Unity перейдите в `Create > Weapon Shop > Weapon Data`
2. Заполните поля конфигурации:
   - **Weapon ID**: Уникальный идентификатор (например, "handgun_01")
   - **Weapon Name**: Название оружия
   - **Weapon Type**: Тип оружия
   - **Weapon Prefab**: Префаб оружия
   - **Weapon Icon**: Изображение для UI
   - **Base Cost**: Стоимость оружия
   - **Stats**: Параметры оружия (урон, скорострельность и т.д.)

### 2. Настройка сцены магазина (ScriptableObject режим)

1. Создайте новую сцену или откройте существующую
2. Добавьте следующие компоненты:
   - **CurrencyService**: Сервис валюты
   - **WeaponShopService**: Сервис магазина оружия (ScriptableObject режим)
   - **WeaponShopScene**: Основной скрипт сцены
   - **WeaponListUI**: UI списка оружия
   - **WeaponPreview3D**: 3D предпросмотр
   - **WeaponStatsPanel**: Панель параметров
   - **CurrencyBalanceUI**: UI баланса
   - **InventoryManager**: Менеджер инвентаря (для тестирования)
   - **InventoryIntegration**: Интеграция с Inventory

3. Настройте ссылки в Inspector:
   - В `WeaponShopScene` добавьте ссылки на все UI компоненты
   - В `WeaponShopService` добавьте список WeaponData
   - В `WeaponListUI` добавьте префаб карточки оружия
   - В `WeaponPreview3D` добавьте точку спавна и камеру
   - В `WeaponStatsPanel` настройте максимальные значения параметров
   - В `InventoryManager` настройте путь к файлу `saves/inventory_test.json`

### 3. Создание UI префабов

#### Карточка оружия (WeaponCardUI)
Создайте префаб карточки оружия с следующими элементами:
- **Image**: Изображение оружия
- **TextMeshProUGUI**: Название оружия
- **TextMeshProUGUI**: Тип оружия
- **TextMeshProUGUI**: Стоимость
- **Button**: Кнопка покупки
- **Button**: Кнопка выбора
- **GameObject**: Индикатор купленного оружия
- **GameObject**: Индикатор выбранного оружия

#### Список оружия (WeaponListUI)
Создайте контейнер для карточек:
- **Scroll View**: Для прокрутки списка
- **Grid Layout Group**: Для автоматического размещения карточек
- **Content Sizer**: Для правильного размера контента

#### Предпросмотр оружия (WeaponPreview3D)
Создайте сцену для предпросмотра:
- **Camera**: Камера для предпросмотра
- **Transform**: Точка спавна оружия
- **Light**: Освещение для предпросмотра

#### Панель параметров (WeaponStatsPanel)
Создайте UI для отображения параметров:
- **TextMeshProUGUI**: Название оружия
- **TextMeshProUGUI**: Тип оружия
- **TextMeshProUGUI + Image**: Урон
- **TextMeshProUGUI + Image**: Скорострельность
- **TextMeshProUGUI + Image**: Ёмкость магазина
- **TextMeshProUGUI + Image**: Время перезарядки
- **TextMeshProUGUI + Image**: Точность
- **TextMeshProUGUI + Image**: Дальность

#### Баланс валюты (CurrencyBalanceUI)
Создайте UI для отображения баланса:
- **TextMeshProUGUI**: Текст баланса
- **Image**: Иконка валюты

## API

### ICurrencyService

```csharp
// Получить текущий баланс
int balance = currencyService.GetBalance();

// Добавить валюту
currencyService.AddCurrency(100);

// Потратить валюту
bool success = currencyService.SpendCurrency(100);

// Проверить достаточно ли валюты
bool hasEnough = currencyService.HasEnoughCurrency(100);

// Установить баланс
currencyService.SetBalance(1000);

// Подписаться на изменения баланса
currencyService.OnBalanceChanged += (newBalance) => {
    Debug.Log($"Новый баланс: {newBalance}");
};
```

### IWeaponShopService

```csharp
// Купить оружие
bool success = weaponShopService.BuyWeapon("weapon_id");

// Выбрать оружие
weaponShopService.SelectWeapon("weapon_id");

// Проверить куплено ли оружие
bool isPurchased = weaponShopService.IsWeaponPurchased("weapon_id");

// Проверить выбрано ли оружие
bool isSelected = weaponShopService.IsWeaponSelected("weapon_id");

// Получить данные оружия
WeaponData weaponData = weaponShopService.GetWeaponData("weapon_id");

// Получить список всех оружий
List<WeaponData> allWeapons = weaponShopService.GetAllWeapons();

// Получить список купленных оружий
List<WeaponData> purchasedWeapons = weaponShopService.GetPurchasedWeapons();

// Получить выбранное оружие
WeaponData selectedWeapon = weaponShopService.GetSelectedWeapon();

// Добавить конфигурацию оружия
weaponShopService.AddWeaponData(weaponData);

// Подписаться на события
weaponShopService.OnWeaponPurchased += (weaponId) => {
    Debug.Log($"Оружие {weaponId} куплено");
};

weaponShopService.OnWeaponSelected += (weaponId) => {
    Debug.Log($"Оружие {weaponId} выбрано");
};

weaponShopService.OnDataChanged += () => {
    Debug.Log("Данные магазина изменены");
};
```

### InventoryManager (для тестирования)

```csharp
// Получить данные инвентаря
InventoryData data = inventoryManager.GetInventoryData();

// Получить список всех оружий
List<WeaponInventoryItem> allWeapons = data.weapons;

// Получить текущее оружие
WeaponInventoryItem currentWeapon = data.currentWeapon;

// Проверить, есть ли оружие в инвентаре
bool hasWeapon = data.HasWeapon("weapon_id");

// Добавить оружие в инвентарь
inventoryManager.AddWeapon("weapon_id");

// Удалить оружие из инвентаря
inventoryManager.RemoveWeapon("weapon_id");

// Выбрать оружие
inventoryManager.EquipWeapon("weapon_id");

// Сбросить выбор оружия
inventoryManager.UnequipWeapon();

// Увеличить количество использований
inventoryManager.IncreaseWeaponUsage("weapon_id");

// Получить количество использований
int usageCount = inventoryManager.GetWeaponUsage("weapon_id");
```

## Сохранение данных

Данные сохраняются в формате JSON в следующую директорию:
- **Windows**: `%USERPROFILE%/AppData/LocalLow/[CompanyName]/[ProductName]/saves/`
- **macOS**: `~/Library/Application Support/[CompanyName]/[ProductName]/saves/`
- **Linux**: `~/.config/unity3d/[CompanyName]/[ProductName]/saves`

### Файлы сохранения
- `currency_save.json` - Данные валюты
- `weapon_shop_save.json` - Данные магазина оружия
- `saves/inventory_test.json` - Тестовые данные инвентаря

## Интеграция с существующими системами

### Inventory

Для интеграции с существующей системой Inventory:

1. Откройте `InventoryIntegration.cs`
2. В методе `AddWeaponToInventory` добавьте код для добавления оружия в инвентарь
3. Пример интеграции закомментирован в файле

### Weapon

Для интеграции с системой Weapon:

1. При выборе оружия в магазине, префаб оружия будет доступен через `WeaponData.WeaponPrefab`
2. Используйте этот префаб для создания экземпляра оружия в игре

## Тестирование инвентаря

### Тест-кейсы:

1. **Добавление оружия**
   - Создайте конфигурацию оружия
   - Убедитесь, что баланс достаточно для покупки
   - Нажмите кнопку "Купить"
   - Проверьте, что оружие добавлено в инвентарь

2. **Выбор оружия**
   - Купите несколько оружий
   - Нажмите кнопку "Выбрать" на одном из них
   - Проверьте, что выбранное оружие отображается корректно

3. **Сохранение данных**
   - Купите оружие
   - Перезапустите игру
   - Проверьте, что оружие осталось купленным

4. **Проверка наличия оружия**
   - Используйте метод `inventoryManager.HasWeapon("weapon_id")` для проверки

## JSON структура для инвентаря

```json
{
  "playerId": "test_player",
  "currencyBalance": 1000,
  "weapons": {
    "weapon_id_1": {
      "weaponId": "weapon_id_1",
      "purchased": true,
      "selected": true,
      "addedTime": "2024-01-01T10:00:00Z"
    },
    "weapon_id_2": {
      "weaponId": "weapon_id_2",
      "purchased": true,
      "selected": false,
      "addedTime": "2024-01-01T10:00:01Z"
    },
    "currentWeaponId": "weapon_id_1"
  }
}
```

## Известные проблемы

### Editor Toolbox

Если у вас установлен Editor Toolbox и возникают ошибки при отображении ScriptableObject:
1. Создайте кастомный Editor для WeaponData
2. Используйте `DrawDefaultInspector()` вместо кастомного отображения

### TextMeshPro

Для работы с TextMeshPro:
1. Убедитесь, что пакет TextMeshPro установлен в проекте
2. Импортируйте `using TMPro;` в файлах UI

## Дополнительная документация

- [`TZ_PART_1_WEAPON_SHOP.md`](../../plans/TZ_PART_1_WEAPON_SHOP.md) - Полное техническое задание
- [`TZ_PART_2_SKINS.md`](../../plans/TZ_PART_2_SKINS.md) - Техническое задание для скинов
- [`TZ_PART_3_ATTACHMENTS.md`](../../plans/TZ_PART_3_ATTACHMENTS.md) - Техническое задание для обвесов
- [`TZ_PART_4_UI_UX.md`](../../plans/TZ_PART_4_UI_UX.md) - Техническое задание для UI/UX
- [`TZ_PART_5_SAVE_INTEGRATION.md`](../../plans/TZ_PART_5_SAVE_INTEGRATION.md) - Техническое задание для сохранения и интеграции
- [`TZ_PART_6_TESTING_DEPLOYMENT.md`](../../plans/TZ_PART_6_TESTING_DEPLOYMENT.md) - Техническое задание для тестирования и развёртывания

## Поддержка

При возникновении проблем:
1. Проверьте консоль Unity на наличие ошибок
2. Убедитесь, что все сервисы добавлены в сцену
3. Проверьте, что все ссылки в Inspector настроены корректно
4. Ознакомьтесь с документацией в папке `plans/`
