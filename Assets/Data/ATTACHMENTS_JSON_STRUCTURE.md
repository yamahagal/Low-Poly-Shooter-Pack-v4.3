# Структура JSON файлов для управления обвесами

## Обзор

Создана система из двух JSON файлов для управления обвесами в игре. Каждый файл выполняет определённую функцию:

1. **attachments_config.json** - Справочник обвесов (уже существовал)
2. **attachments_availability_config.json** - Управление доступностью обвесов
3. **weapons_attachments_config.json** - Конфигурация обвесов для каждого оружия

---

## 1. attachments_availability_config.json

### Назначение
Управляет глобальной доступностью и статусом покупки обвесов в игре.

### Структура

```json
{
  "attachmentsAvailability": {
    "attachment_id": {
      "purchased": true,            // Куплен ли обвес
      "selected": true,             // Выбран ли обвес
      "purchaseDate": "ISO_8601"    // Дата покупки (null если не куплен)
    }
  },
  "globalSettings": {
    "allAttachmentsPurchased": true, // Глобальная покупка всех обвесов
    "allowExperimentalAttachments": false,
    "maxAttachmentsPerWeapon": 4,   // Максимум обвесов на оружие
    "attachmentCategories": {
      "CategoryName": {
        "maxSlots": 1,              // Максимум слотов для категории
        "selected": true            // Выбрана ли категория
      }
    }
  }
}
```

### Пример использования

**Купить и выбрать обвес:**
```json
"scope_red_dot": {
  "purchased": true,
  "selected": true,
  "purchaseDate": "2024-01-15T10:30:00Z"
}
```

**Не купленный обвес:**
```json
"scope_red_dot": {
  "purchased": false,
  "selected": false,
  "purchaseDate": null
}
```

---

## 2. weapons_attachments_config.json

### Назначение
Управляет конфигурацией обвесов для конкретного оружия, включая пресеты и текущее состояние.

### Структура

```json
{
  "weaponsAttachments": {
    "weapon_id": {
      "weaponId": "weapon_id",
      "weaponName": "Название оружия",
      "attachments": {
        "slot_name": {
          "slotType": "CategoryType",
          "currentAttachment": "attachment_id",  // null если нет обвеса
          "availableAttachments": ["id1", "id2"],
          "selected": true                       // Выбран ли слот
        }
      },
      "presetConfigurations": {
        "preset_name": {
          "name": "Название пресета",
          "attachments": {
            "slot_name": "attachment_id"  // null если нет обвеса
          }
        }
      },
      "activePreset": "preset_name"
    }
  },
  "globalWeaponSettings": {
    "autoApplyPresets": true,
    "saveAttachmentsOnWeaponChange": true,
    "allowMixedAttachments": true,
    "attachmentCompatibilityCheck": true
  }
}
```

### Пример использования

**Установить обвес на оружие:**
```json
"scope": {
  "slotType": "Scope",
  "currentAttachment": "scope_red_dot",
  "availableAttachments": ["scope_red_dot", "scope_acog"],
  "selected": true
}
```

**Убрать обвес с оружия:**
```json
"scope": {
  "slotType": "Scope",
  "currentAttachment": null,
  "availableAttachments": ["scope_red_dot", "scope_acog"],
  "selected": true
}
```

**Отключить слот полностью:**
```json
"grip": {
  "slotType": "Grip",
  "currentAttachment": null,
  "availableAttachments": [],
  "selected": false
}
```

---

## Категории обвесов

- **Scope** - Прицелы и оптические прицелы
- **Muzzle** - Дульные насадки и глушители
- **Laser** - Лазерные целеуказатели
- **Grip** - Рукоятки и передние рукоятки
- **Magazine** - Магазины различной вместимости
- **Bipod** - Сошки для стабилизации

---

## ID обвесов

- `scope_red_dot` - Красная точка
- `scope_acog` - ACOG 4x
- `muzzle_compensator` - Компенсатор
- `muzzle_silencer` - Глушитель
- `laser_basic` - Лазер
- `grip_vertical` - Вертикальная рукоятка
- `grip_angled` - Угловая рукоятка
- `magazine_extended` - Расширенный магазин
- `magazine_drum` - Барабанный магазин
- `bipod_basic` - Сошки

---

## ID оружия

- `ar_01` - Автомат AR-01
- `ar_02` - Автомат AR-02
- `ar_03` - Автомат AR-03
- `handgun_01` - Пистолет HG-01
- `handgun_02` - Пистолет HG-02
- `handgun_03` - Пистолет HG-03
- `handgun_04` - Пистолет HG-04
- `smg_01` - ПП SMG-01
- `smg_02` - ПП SMG-02
- `smg_03` - ПП SMG-03
- `sniper_01` - Снайперская винтовка SN-01
- `sniper_02` - Снайперская винтовка SN-02
- `sniper_03` - Снайперская винтовка SN-03

---

## Рекомендации по использованию

### Для полноценной игры:
Используйте комбинацию:
- **attachments_availability_config.json** для управления доступностью
- **weapons_attachments_config.json** для конфигурации оружия

### Для сохранения прогресса игрока:
Создайте отдельный файл с сохранёнными конфигурациями для каждого игрока.

---

## Примечания

- Все даты в формате ISO 8601 (например: `2024-01-15T10:30:00Z`)
- `null` значения означают отсутствие объекта или даты
- Поля `purchased` и `selected` работают независимо в разных файлах
- Пресеты позволяют быстро переключаться между наборами обвесов
