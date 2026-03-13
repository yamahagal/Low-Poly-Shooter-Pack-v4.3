# Интеграция JSON конфигурации обвесов с WeaponAttachmentManager

## Обзор

Создана система для связывания JSON конфигураций обвесов с существующим скриптом [`WeaponAttachmentManager.cs`](WeaponAttachmentManager.cs).

## Созданные файлы

### 1. [`AttachmentConfigLoader.cs`](AttachmentConfigLoader.cs)
Базовый загрузчик конфигураций обвесов из JSON файлов.

**Основные возможности:**
- Загрузка данных из JSON файлов
- Проверка статуса покупки и выбора обвесов
- Получение доступных обвесов для оружия
- Управление пресетами

### 2. [`AttachmentConfigManager.cs`](AttachmentConfigManager.cs) (Рекомендуемый)
Расширенный менеджер конфигураций с полной интеграцией с [`WeaponAttachmentManager.cs`](WeaponAttachmentManager.cs).

**Основные возможности:**
- Всё из [`AttachmentConfigLoader.cs`](AttachmentConfigLoader.cs)
- Сопоставление ID обвесов с индексами массивов
- Автоматическое применение конфигурации к оружию
- Использование рефлексии для установки приватных полей
- Поддержка всех типов обвесов (Scope, Muzzle, Laser, Grip, Magazine)

## Установка

### Шаг 1: Добавьте компонент на префаб оружия

1. Откройте префаб оружия (например, `P_LPSP_WEP_AR_01.prefab`)
2. Добавьте компонент [`AttachmentConfigManager`](AttachmentConfigManager.cs) на тот же объект, где находится [`WeaponAttachmentManager`](WeaponAttachmentManager.cs)
3. Настройте пути к JSON файлам в инспекторе

### Шаг 2: Настройте сопоставление ID обвесов

В инспекторе [`AttachmentConfigManager`](AttachmentConfigManager.cs) вы увидите секцию "Сопоставление ID обвесов":

**Для прицелов (Scope Mappings):**
- `attachmentId`: ID из JSON (например, "scope_red_dot")
- `arrayIndex`: Индекс в массиве `scopeArray` в [`WeaponAttachmentManager`](WeaponAttachmentManager.cs)

**Пример:**
```
scope_red_dot -> 0
scope_acog -> 1
```

Аналогично настройте сопоставления для:
- Muzzle Mappings (дульные насадки)
- Laser Mappings (лазеры)
- Grip Mappings (рукоятки)
- Magazine Mappings (магазины)

### Шаг 3: Настройте ID оружия

Установите `currentWeaponId` в инспекторе [`AttachmentConfigManager`](AttachmentConfigManager.cs):
- `ar_01` - для автомата AR-01
- `handgun_01` - для пистолета HG-01
- `sniper_01` - для снайперской винтовки SN-01

## Использование

### Автоматическая загрузка

Если включена опция `Auto Load On Start` в инспекторе, конфигурация загрузится автоматически при старте игры.

### Ручная загрузка

```csharp
// Получить компонент
AttachmentConfigManager configManager = GetComponent<AttachmentConfigManager>();

// Загрузить все конфигурации
configManager.LoadAllConfigs();

// Установить оружие
configManager.SetWeapon("ar_01");

// Применить пресет
configManager.ApplyPreset("ar_01", "cqb");
```

### Проверка статуса обвеса

```csharp
// Проверить, куплен ли обвес
bool isPurchased = configManager.IsAttachmentPurchased("scope_red_dot");

// Проверить, выбран ли обвес
bool isSelected = configManager.IsAttachmentSelected("scope_red_dot");
```

### Получение доступных обвесов

```csharp
// Получить доступные прицелы для оружия
List<string> availableScopes = configManager.GetAvailableAttachmentsForWeapon("ar_01", "Scope");

// Получить текущий прицел
string currentScope = configManager.GetCurrentAttachmentForSlot("ar_01", "scope");
```

### Программное добавление сопоставлений

```csharp
// Добавить сопоставление для прицела
configManager.AddMapping("scope", "scope_red_dot", 0);
configManager.AddMapping("scope", "scope_acog", 1);
```

## JSON структура

### attachments_availability_config.json

```json
{
  "attachmentsAvailability": {
    "scope_red_dot": {
      "purchased": true,
      "selected": true,
      "purchaseDate": "2024-01-15T10:30:00Z"
    }
  },
  "globalSettings": {
    "allAttachmentsPurchased": true,
    "maxAttachmentsPerWeapon": 4
  }
}
```

### weapons_attachments_config.json

```json
{
  "weaponsAttachments": {
    "ar_01": {
      "weaponId": "ar_01",
      "weaponName": "Автомат AR-01",
      "attachments": {
        "scope": {
          "slotType": "Scope",
          "currentAttachment": "scope_red_dot",
          "availableAttachments": ["scope_red_dot", "scope_acog"],
          "selected": true
        }
      },
      "presetConfigurations": {
        "cqb": {
          "name": "Боевая комплектация",
          "attachments": {
            "scope": "scope_red_dot",
            "muzzle": "muzzle_compensator"
          }
        }
      },
      "activePreset": "cqb"
    }
  }
}
```

## Принцип работы

### 1. Загрузка конфигурации

Скрипт [`AttachmentConfigManager`](AttachmentConfigManager.cs) загружает данные из JSON файлов при старте или при вызове метода `LoadAllConfigs()`.

### 2. Сопоставление ID с индексами

JSON использует строковые ID (например, "scope_red_dot"), а [`WeaponAttachmentManager`](WeaponAttachmentManager.cs) использует числовые индексы массивов. Скрипт [`AttachmentConfigManager`](AttachmentConfigManager.cs) сопоставляет эти значения через списки сопоставлений в инспекторе.

### 3. Применение конфигурации

Когда конфигурация применяется к оружию, скрипт:
1. Получает текущий обвес из JSON по ID
2. Находит соответствующий индекс в массиве через сопоставления
3. Использует рефлексию для установки приватных полей в [`WeaponAttachmentManager`](WeaponAttachmentManager.cs)
4. Вызывает метод `SetAttachments()` для применения изменений

### 4. Отключение рандомизации

Скрипт автоматически отключает рандомизацию обвесов (`scopeIndexRandom`, `muzzleIndexRandom` и т.д.) при применении конфигурации из JSON.

## Пример полного рабочего процесса

### 1. Настройка JSON

В файле [`weapons_attachments_config.json`](../../../Data/weapons_attachments_config.json):
```json
{
  "weaponsAttachments": {
    "ar_01": {
      "weaponId": "ar_01",
      "weaponName": "Автомат AR-01",
      "attachments": {
        "scope": {
          "slotType": "Scope",
          "currentAttachment": "scope_red_dot",
          "availableAttachments": ["scope_red_dot", "scope_acog"],
          "selected": true
        },
        "muzzle": {
          "slotType": "Muzzle",
          "currentAttachment": "muzzle_compensator",
          "availableAttachments": ["muzzle_compensator", "muzzle_silencer"],
          "selected": true
        }
      }
    }
  }
}
```

### 2. Настройка префаба оружия

1. Откройте префаб `P_LPSP_WEP_AR_01.prefab`
2. Добавьте компонент [`AttachmentConfigManager`](AttachmentConfigManager.cs)
3. Настройте сопоставления:
   - Scope Mappings:
     - attachmentId: "scope_red_dot", arrayIndex: 0
     - attachmentId: "scope_acog", arrayIndex: 1
   - Muzzle Mappings:
     - attachmentId: "muzzle_compensator", arrayIndex: 0
     - attachmentId: "muzzle_silencer", arrayIndex: 1
4. Установите `currentWeaponId` = "ar_01"
5. Включите `Auto Load On Start`

### 3. Результат

При запуске игры оружие автоматически загрузит конфигурацию из JSON и установит:
- Прицел "scope_red_dot" (индекс 0)
- Дульную насадку "muzzle_compensator" (индекс 0)

## Отладка

### Включение отладочных сообщений

В инспекторе [`AttachmentConfigManager`](AttachmentConfigManager.cs) включите опцию `Show Debug Messages`.

Вы увидите сообщения в консоли Unity:
```
[AttachmentConfigManager] Загружена конфигурация доступности обвесов
[AttachmentConfigManager] Загружена конфигурация обвесов оружия
[AttachmentConfigManager] Установлен прицел: scope_red_dot (индекс: 0)
[AttachmentConfigManager] Установлена дульная насадка: muzzle_compensator (индекс: 0)
[AttachmentConfigManager] Применена конфигурация для оружия ar_01
```

## Расширенные возможности

### Динамическое переключение оружия

```csharp
// При смене оружия
void OnWeaponChanged(string newWeaponId)
{
    AttachmentConfigManager configManager = GetComponent<AttachmentConfigManager>();
    configManager.SetWeapon(newWeaponId);
}
```

### Сохранение настроек игрока

```csharp
// При покупке обвеса
void OnAttachmentPurchased(string attachmentId)
{
    // Обновить JSON файл
    // Перезагрузить конфигурацию
    AttachmentConfigManager configManager = GetComponent<AttachmentConfigManager>();
    configManager.LoadAvailabilityConfig();
    configManager.ApplyConfigToWeapon(currentWeaponId);
}
```

### Создание пользовательских пресетов

```csharp
// Создать новый пресет
void CreateCustomPreset(string weaponId, string presetName, Dictionary<string, string> attachments)
{
    // Добавить пресет в weapons_attachments_config.json
    // Применить пресет
    AttachmentConfigManager configManager = GetComponent<AttachmentConfigManager>();
    configManager.ApplyPreset(weaponId, presetName);
}
```

## Ограничения

1. **Сопоставление ID**: Необходимо вручную настроить сопоставление ID обвесов с индексами массивов
2. **Рефлексия**: Использование рефлексии для установки приватных полей может быть медленнее прямого доступа
3. **JSON формат**: Требуется корректный формат JSON файлов

## Рекомендации

1. Используйте [`AttachmentConfigManager`](AttachmentConfigManager.cs) вместо [`AttachmentConfigLoader`](AttachmentConfigLoader.cs) для полной интеграции с [`WeaponAttachmentManager`](WeaponAttachmentManager.cs)
2. Настройте все сопоставления ID перед запуском игры
3. Используйте пресеты для быстрого переключения между наборами обвесов
4. Включите отладочные сообщения во время разработки
5. Храните JSON файлы в папке `Assets/Data/` для удобства

## Дополнительная документация

- [Структура JSON файлов](../../../Data/ATTACHMENTS_JSON_STRUCTURE.md)
- [Краткое руководство](../../../Data/README_ATTACHMENTS.md)
- [WeaponAttachmentManager.cs](WeaponAttachmentManager.cs)
