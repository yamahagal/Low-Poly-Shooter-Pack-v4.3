# Обновлённая структура JSON файлов

## Изменения

### Проблема
`JsonUtility.FromJson` в Unity не поддерживает десериализацию `Dictionary<string, WeaponAttachments>` напрямую.

### Решение
Изменена структура JSON файла с Dictionary на List для совместимости с Unity JsonUtility.

## Новая структура JSON

### weapons_attachments_config.json

**Старая структура (Dictionary):**
```json
{
  "weaponsAttachments": {
    "ar_01": {
      "weaponId": "ar_01",
      "attachments": {...}
    }
  }
}
```

**Новая структура (List):**
```json
{
  "weaponsAttachments": [
    {
      "weaponId": "ar_01",
      "weaponName": "Автомат AR-01",
      "attachments": {...}
    },
    {
      "weaponId": "ar_02",
      "weaponName": "Автомат AR-02",
      "attachments": {...}
    }
  ]
}
```

## Преимущества новой структуры

✅ **Совместимость с Unity JsonUtility**
✅ **Простая десериализация**
✅ **Поддержка порядка элементов**
✅ **Легкое добавление новых оружий**

## Изменения в коде

### AttachmentConfigLoader.cs
- `WeaponsAttachmentsData.weaponsAttachments` изменён с `Dictionary<string, WeaponAttachments>` на `List<WeaponAttachments>`
- Добавлен вспомогательный класс `WeaponsAttachmentsDataHelper` для десериализации

### AttachmentConfigManager.cs
- `LoadWeaponsConfig()` обновлён для работы с List
- `ApplyConfigToWeapon()` обновлён для поиска в List вместо Dictionary

## Использование

### Поиск оружия по ID

**Раньше (Dictionary):**
```csharp
if (weaponsData.weaponsAttachments.ContainsKey(weaponId))
{
    var weaponConfig = weaponsData.weaponsAttachments[weaponId];
}
```

**Теперь (List):**
```csharp
var weaponConfig = weaponsData.weaponsAttachmentsList.Find(w => w.weaponId == weaponId);
```

### Получение всех оружий

**Раньше (Dictionary):**
```csharp
foreach (var weaponId in weaponsData.weaponsAttachments.Keys)
{
    var weaponConfig = weaponsData.weaponsAttachments[weaponId];
}
```

**Теперь (List):**
```csharp
foreach (var weaponConfig in weaponsData.weaponsAttachmentsList)
{
    // Работаем напрямую с weaponConfig
}
```

## Пример полного JSON файла

```json
{
  "weaponsAttachments": [
    {
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
      },
      "presetConfigurations": {
        "default": {
          "name": "Базовая комплектация",
          "attachments": {
            "scope": null,
            "muzzle": null
          }
        }
      },
      "activePreset": "default"
    },
    {
      "weaponId": "ar_02",
      "weaponName": "Автомат AR-02",
      "attachments": {
        "scope": {
          "slotType": "Scope",
          "currentAttachment": "scope_acog",
          "availableAttachments": ["scope_red_dot", "scope_acog"],
          "selected": true
        },
        "muzzle": {
          "slotType": "Muzzle",
          "currentAttachment": "muzzle_silencer",
          "availableAttachments": ["muzzle_compensator", "muzzle_silencer"],
          "selected": true
        }
      },
      "presetConfigurations": {
        "tactical": {
          "name": "Тактическая комплектация",
          "attachments": {
            "scope": "scope_acog",
            "muzzle": "muzzle_silencer"
          }
        }
      },
      "activePreset": "tactical"
    }
  ],
  "globalWeaponSettings": {
    "autoApplyPresets": true,
    "saveAttachmentsOnWeaponChange": true,
    "allowMixedAttachments": true,
    "attachmentCompatibilityCheck": true
  }
}
```

## Отладка

При включённой отладке вы увидите:

```
[AttachmentConfigManager] Загружено записей из JSON: 3
[AttachmentConfigManager] Обработка записи: ar_01
[AttachmentConfigManager] Добавлено оружие: ar_01
[AttachmentConfigManager] Обработка записи: ar_02
[AttachmentConfigManager] Добавлено оружие: ar_02
[AttachmentConfigManager] Обработка записи: handgun_01
[AttachmentConfigManager] Добавлено оружие: handgun_01
[AttachmentConfigManager] Конфигурация загружена успешно
[AttachmentConfigManager] Оружий в конфигурации: 3
[AttachmentConfigManager] Применение конфигурации для оружия: ar_02
```

## Совместимость

✅ **Обратная совместимость:** Старые JSON файлы с Dictionary больше не поддерживаются
✅ **Новая структура:** Только JSON файлы с List поддерживаются

## Миграция

Если у вас есть старые JSON файлы с Dictionary, их нужно обновить до новой структуры:

1. Замените `{` на `[`
2. Замените `}` на `]`
3. Добавьте запятые между объектами

**Пример миграции:**
```json
// Было:
{
  "weaponsAttachments": {
    "ar_01": {...},
    "ar_02": {...}
  }
}

// Стало:
{
  "weaponsAttachments": [
    {"weaponId": "ar_01", ...},
    {"weaponId": "ar_02", ...}
  ]
}
```

## Дополнительная документация

- [Структура JSON файлов](ATTACHMENTS_JSON_STRUCTURE.md)
- [Объяснение ID обвесов](ATTACHMENT_IDS_EXPLANATION.md)
- [Настройка currentWeaponId](CURRENT_WEAPON_ID_SETUP.md)
- [Руководство по устранению неполадок](TROUBLESHOOTING_GUIDE.md)
