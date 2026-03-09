# Техническое задание: Система сохранения и интеграция (Часть 5 - Сохранение и интеграция)

## Обзор

Создать систему сохранения прогресса для магазина оружия, скинов и обвесов, а также интегрировать систему магазина с существующими системами игры "Low Poly Shooter Pack v4.3".

## Требования

### 1. Система сохранения

**Формат данных (JSON):**

```json
{
  "version": "1.0",
  "currency": 1000,
  "weapons": {
    "weapon_id_1": {
      "purchased": true,
      "selected": true,
      "skins": {
        "skin_id_1": {
          "purchased": true,
          "selected": true
        },
        "skin_id_2": {
          "purchased": false,
          "selected": false
        }
      },
      "attachments": {
        "scope": {
          "attachment_id": "scope_01",
          "installed": true
        },
        "muzzle": {
          "attachment_id": null,
          "installed": false
        },
        "laser": {
          "attachment_id": "laser_01",
          "installed": true
        },
        "grip": {
          "attachment_id": null,
          "installed": false
        },
        "magazine": {
          "attachment_id": "magazine_01",
          "installed": true
        },
        "bipod": {
          "attachment_id": null,
          "installed": false
        }
      },
      "purchased_attachments": [
        "scope_01",
        "laser_01",
        "magazine_01"
      ]
    },
    "weapon_id_2": {
      "purchased": false,
      "selected": false,
      "skins": {},
      "attachments": {
        "scope": {
          "attachment_id": null,
          "installed": false
        },
        "muzzle": {
          "attachment_id": null,
          "installed": false
        },
        "laser": {
          "attachment_id": null,
          "installed": false
        },
        "grip": {
          "attachment_id": null,
          "installed": false
        },
        "magazine": {
          "attachment_id": null,
          "installed": false
        },
        "bipod": {
          "attachment_id": null,
          "installed": false
        }
      },
      "purchased_attachments": []
    }
  },
  "last_save_time": "2024-01-01T12:00:00Z"
}
```

**Места сохранения:**
- Windows: `%USERPROFILE%/AppData/LocalLow/[CompanyName]/[ProductName]/saves/`
- macOS: `~/Library/Application Support/[CompanyName]/[ProductName]/saves/`
- Linux: `~/.config/unity3d/[CompanyName]/[ProductName]/saves/`

**Автосохранение:**
- Сохранение после каждой покупки
- Сохранение после каждого выбора скина/обвеса
- Сохранение при выходе из магазина
- Сохранение при выходе из игры

### 2. Система загрузки

**Время загрузки:**
- При запуске игры
- При входе в магазин
- При необходимости восстановления данных

**Обработка ошибок:**
- Создание новых данных, если файл сохранения не найден
- Создание резервной копии при повреждении файла
- Логирование ошибок загрузки
- Сообщение пользователю при критической ошибке

### 3. Резервное копирование

**Автоматическое резервирование:**
- Создание резервной копии каждые 10 сохранений
- Хранение последних 5 резервных копий
- Автоматическое удаление старых резервных копий

**Формат имени файла:**
- Основной файл: `shop_save.json`
- Резервные копии: `shop_save_backup_1.json`, `shop_save_backup_2.json`, и т.д.

### 4. Интеграция с Inventory

**Требования:**
- Добавление купленного оружия в инвентарь
- Отображение выбранного скина в инвентаре
- Отображение установленных обвесов в инвентаре
- Применение скинов и обвесов при выборе оружия

**Методы для интеграции:**
```csharp
// Добавление оружия в инвентарь
void AddWeaponToInventory(string weaponId);

// Применение скина к оружию
void ApplySkinToWeapon(string weaponId, string skinId);

// Применение обвесов к оружию
void ApplyAttachmentsToWeapon(string weaponId, Dictionary<AttachmentType, string> attachments);

// Получение характеристик оружия с учётом обвесов
WeaponStats GetWeaponStatsWithAttachments(string weaponId, Dictionary<AttachmentType, string> attachments);
```

### 5. Интеграция с Weapon

**Требования:**
- Динамическое изменение характеристик при установке/снятии обвесов
- Применение материала скина к оружию
- Визуализация установленных обвесов
- Сохранение состояния оружия

**Методы для интеграции:**
```csharp
// Применение материала скина
void ApplySkinMaterial(Material skinMaterial);

// Установка обвеса
void InstallAttachment(AttachmentData attachment);

// Снятие обвеса
void UninstallAttachment(AttachmentType attachmentType);

// Получение базовых характеристик
WeaponStats GetBaseStats();

// Получение финальных характеристик с учётом обвесов
WeaponStats GetFinalStats();
```

### 6. Интеграция с ServiceLocator

**Требования:**
- Регистрация сервисов магазина при запуске игры
- Доступ к сервисам из любого места в коде
- Автоматическая инициализация сервисов

**Сервисы для регистрации:**
- `ICurrencyService` - Сервис валюты
- `IWeaponShopService` - Сервис магазина оружия
- `ISkinShopService` - Сервис магазина скинов
- `IAttachmentShopService` - Сервис магазина обвесов

**Пример регистрации:**
```csharp
// При запуске игры
ServiceLocator.Register<ICurrencyService>(currencyService);
ServiceLocator.Register<IWeaponShopService>(weaponShopService);
ServiceLocator.Register<ISkinShopService>(skinShopService);
ServiceLocator.Register<IAttachmentShopService>(attachmentShopService);

// Доступ к сервису
var currencyService = ServiceLocator.Get<ICurrencyService>();
```

### 7. Интеграция с существующими скриптами

**Интеграция с Shop.cs:**
- Использование существующей логики магазина
- Расширение функционала для скинов и обвесов
- Совместимость с существующими префабами

**Интеграция с SkinsMenu.cs:**
- Использование существующего UI скинов
- Расширение функционала для новой системы скинов
- Совместимость с существующими материалами

**Интеграция с SaveSystem.cs:**
- Расширение существующей системы сохранения
- Добавление новых полей для скинов и обвесов
- Совместимость с существующим форматом сохранения

### 8. Система событий

**События для подписки:**
```csharp
// Изменение баланса валюты
event Action<int> OnCurrencyChanged;

// Покупка оружия
event Action<string> OnWeaponPurchased;

// Выбор оружия
event Action<string> OnWeaponSelected;

// Покупка скина
event Action<string, string> OnSkinPurchased;

// Выбор скина
event Action<string, string> OnSkinSelected;

// Покупка обвеса
event Action<string, string> OnAttachmentPurchased;

// Установка обвеса
event Action<string, AttachmentType, string> OnAttachmentInstalled;

// Снятие обвеса
event Action<string, AttachmentType> OnAttachmentUninstalled;

// Сохранение данных
event Action OnDataSaved;

// Загрузка данных
event Action OnDataLoaded;
```

### 9. Отладка и логирование

**Логирование:**
- Логирование всех операций покупки
- Логирование операций сохранения/загрузки
- Логирование ошибок и исключений
- Логирование интеграционных операций

**Отладочные инструменты:**
- Консольные команды для управления валютой
- Консольные команды для управления покупками
- Отображение информации о сохранении в UI
- Возможность сброса прогресса

### 10. Безопасность данных

**Защита данных:**
- Шифрование файла сохранения (опционально)
- Проверка целостности данных
- Валидация загруженных данных
- Защита от читерства (на сервере, если есть)

## Технические требования

### Библиотеки
- Newtonsoft.Json для сериализации/десериализации JSON

### Паттерны
- Singleton для сервисов
- Observer для событий
- Repository для доступа к данным

### Файлы для создания

**Persistence Layer:**
- `ShopSaveSystem.cs` - Основная система сохранения
- `ShopSaveData.cs` - Класс данных сохранения
- `ShopBackupSystem.cs` - Система резервного копирования

**Integration Layer:**
- `ShopIntegration.cs` - Основной класс интеграции
- `InventoryIntegration.cs` - Интеграция с Inventory
- `WeaponIntegration.cs` - Интеграция с Weapon
- `ServiceLocatorIntegration.cs` - Интеграция с ServiceLocator

**Debug Layer:**
- `ShopDebugCommands.cs` - Консольные команды для отладки
- `ShopLogger.cs` - Система логирования

## Приоритет задач

1. **Высокий приоритет:**
   - Реализация системы сохранения
   - Реализация системы загрузки
   - Интеграция с Inventory
   - Интеграция с ServiceLocator

2. **Средний приоритет:**
   - Система резервного копирования
   - Интеграция с Weapon
   - Система событий
   - Логирование

3. **Низкий приоритет:**
   - Шифрование данных
   - Отладочные инструменты
   - Защита от читерства

## Тестирование

**Тест-кейсы:**
1. Сохранение и загрузка прогресса
2. Резервное копирование данных
3. Восстановление из резервной копии
4. Обработка повреждённых файлов
5. Интеграция с Inventory
6. Интеграция с Weapon
7. Работа событий
8. Логирование операций

## Документация

Создать документацию:
- Руководство по системе сохранения
- Руководство по интеграции с существующими системами
- Руководство по системе событий
- Руководство по отладке
- API документация для всех сервисов
