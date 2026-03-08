# Настройка оружия через JSON

Эта инструкция объясняет, как настраивать оружие в магазине через JSON файл.

## Обзор системы

Система магазина оружия поддерживает два режима работы:
1. **ScriptableObject режим** - настройка через Unity Editor (оригинальный режим)
2. **JSON режим** - настройка через JSON файл (новый режим)

В этой инструкции мы рассмотрим **JSON режим**.

## Структура JSON файла

Файл конфигурации оружия находится по пути: `Assets/saves/inventory_test.json`

### Основная структура

```json
{
  "playerId": "test_player",
  "currencyBalance": 1000,
  "currentWeaponId": "handgun_01",
  "maxWeapons": 5,
  "weapons": {
    // Состояние купленного оружия
  },
  "weaponDefinitions": {
    // Определения всех доступных оружий
  }
}
```

### Раздел `weapons` - состояние инвентаря

Этот раздел содержит информацию о купленном оружии:

```json
"weapons": {
  "handgun_01": {
    "weaponId": "handgun_01",
    "purchased": true,
    "selected": true,
    "addedTime": "2024-01-01T10:00:00Z"
  }
}
```

**Поля:**
- `weaponId` - уникальный идентификатор оружия (должен совпадать с ключом)
- `purchased` - куплено ли оружие (true/false)
- `selected` - выбрано ли оружие (true/false)
- `addedTime` - время добавления оружия в инвентарь (ISO 8601 формат)

### Раздел `weaponDefinitions` - определения оружия

Этот раздел содержит все доступные для покупки оружия:

```json
"weaponDefinitions": {
  "handgun_01": {
    "weaponId": "handgun_01",
    "weaponName": "Пистолет",
    "weaponType": "Handgun",
    "weaponPrefabPath": "",
    "weaponIconPath": "",
    "baseCost": 100,
    "damage": 25.0,
    "fireRate": 300,
    "magazineCapacity": 12,
    "reloadTime": 1.5,
    "accuracy": 0.1,
    "range": 50.0
  }
}
```

**Поля:**

| Поле | Тип | Описание |
|-------|------|----------|
| `weaponId` | string | Уникальный идентификатор оружия (должен совпадать с ключом) |
| `weaponName` | string | Отображаемое название оружия |
| `weaponType` | string | Тип оружия (см. список ниже) |
| `weaponPrefabPath` | string | Путь к префабу оружия (опционально) |
| `weaponIconPath` | string | Путь к иконке оружия (опционально) |
| `baseCost` | int | Стоимость оружия в магазине |
| `damage` | float | Урон оружия |
| `fireRate` | int | Скорострельность (выстрелов в минуту) |
| `magazineCapacity` | int | Ёмкость магазина |
| `reloadTime` | float | Время перезарядки (в секундах) |
| `accuracy` | float | Точность (меньше = точнее, диапазон 0-1) |
| `range` | float | Дальность стрельбы |

### Доступные типы оружия (weaponType)

- `Handgun` - Пистолет
- `SMG` - Пистолет-пулемёт
- `Shotgun` - Дробовик
- `AssaultRifle` - Штурмовая винтовка
- `Sniper` - Снайперская винтовка
- `RocketLauncher` - Ракетница
- `GrenadeLauncher` - Гранатомёт

## Пример полного JSON файла

```json
{
  "playerId": "test_player",
  "currencyBalance": 1000,
  "currentWeaponId": "handgun_01",
  "maxWeapons": 5,
  "weapons": {
    "handgun_01": {
      "weaponId": "handgun_01",
      "purchased": true,
      "selected": true,
      "addedTime": "2024-01-01T10:00:00Z"
    },
    "smg_01": {
      "weaponId": "smg_01",
      "purchased": true,
      "selected": false,
      "addedTime": "2024-01-01T10:00:01Z"
    }
  },
  "weaponDefinitions": {
    "handgun_01": {
      "weaponId": "handgun_01",
      "weaponName": "Пистолет M9",
      "weaponType": "Handgun",
      "weaponPrefabPath": "",
      "weaponIconPath": "",
      "baseCost": 100,
      "damage": 25.0,
      "fireRate": 300,
      "magazineCapacity": 12,
      "reloadTime": 1.5,
      "accuracy": 0.1,
      "range": 50.0
    },
    "smg_01": {
      "weaponId": "smg_01",
      "weaponName": "ПП-19",
      "weaponType": "SMG",
      "weaponPrefabPath": "",
      "weaponIconPath": "",
      "baseCost": 250,
      "damage": 18.0,
      "fireRate": 700,
      "magazineCapacity": 30,
      "reloadTime": 2.0,
      "accuracy": 0.15,
      "range": 40.0
    },
    "assault_rifle_01": {
      "weaponId": "assault_rifle_01",
      "weaponName": "АК-47",
      "weaponType": "AssaultRifle",
      "weaponPrefabPath": "",
      "weaponIconPath": "",
      "baseCost": 500,
      "damage": 35.0,
      "fireRate": 600,
      "magazineCapacity": 30,
      "reloadTime": 2.5,
      "accuracy": 0.12,
      "range": 100.0
    },
    "shotgun_01": {
      "weaponId": "shotgun_01",
      "weaponName": "Ремингтон 870",
      "weaponType": "Shotgun",
      "weaponPrefabPath": "",
      "weaponIconPath": "",
      "baseCost": 400,
      "damage": 80.0,
      "fireRate": 60,
      "magazineCapacity": 8,
      "reloadTime": 3.0,
      "accuracy": 0.3,
      "range": 25.0
    },
    "sniper_01": {
      "weaponId": "sniper_01",
      "weaponName": "AWP",
      "weaponType": "Sniper",
      "weaponPrefabPath": "",
      "weaponIconPath": "",
      "baseCost": 800,
      "damage": 100.0,
      "fireRate": 40,
      "magazineCapacity": 5,
      "reloadTime": 4.0,
      "accuracy": 0.02,
      "range": 200.0
    }
  }
}
```

## Как добавить новое оружие

1. Откройте файл `Assets/saves/inventory_test.json`
2. В разделе `weaponDefinitions` добавьте новую запись:

```json
"my_new_weapon": {
  "weaponId": "my_new_weapon",
  "weaponName": "Мое новое оружие",
  "weaponType": "AssaultRifle",
  "weaponPrefabPath": "",
  "weaponIconPath": "",
  "baseCost": 350,
  "damage": 30.0,
  "fireRate": 550,
  "magazineCapacity": 25,
  "reloadTime": 2.2,
  "accuracy": 0.13,
  "range": 90.0
}
```

3. Сохраните файл
4. Запустите игру - оружие автоматически появится в магазине

## Как изменить параметры существующего оружия

1. Откройте файл `Assets/saves/inventory_test.json`
2. Найдите нужное оружие в разделе `weaponDefinitions`
3. Измените нужные параметры
4. Сохраните файл
5. Запустите игру - изменения применятся автоматически

## Как дать игроку оружие бесплатно

Если вы хотите дать игроку оружие без покупки:

1. Откройте файл `Assets/saves/inventory_test.json`
2. В разделе `weapons` добавьте запись:

```json
"my_weapon_id": {
  "weaponId": "my_weapon_id",
  "purchased": true,
  "selected": false,
  "addedTime": "2024-01-01T10:00:00Z"
}
```

3. Убедитесь, что это оружие определено в разделе `weaponDefinitions`
4. Сохраните файл

## Как изменить баланс валюты

Измените поле `currencyBalance` в корне JSON файла:

```json
"currencyBalance": 5000
```

## Как изменить текущее выбранное оружие

Измените поле `currentWeaponId` в корне JSON файла:

```json
"currentWeaponId": "assault_rifle_01"
```

Убедитесь, что это оружие куплено (`purchased: true`).

## Валидация JSON

JSON файл должен быть валидным. Используйте онлайн-валидаторы JSON для проверки:
- https://jsonlint.com/
- https://jsonformatter.curiousconcept.com/

## Частые ошибки

### 1. Несовпадение weaponId

Убедитесь, что `weaponId` внутри объекта совпадает с ключом в словаре:

```json
"weaponDefinitions": {
  "handgun_01": {  // ← Ключ
    "weaponId": "handgun_01",  // ← Должен совпадать
    ...
  }
}
```

### 2. Неверный тип оружия

Убедитесь, что `weaponType` соответствует одному из допустимых значений:
- Handgun
- SMG
- Shotgun
- AssaultRifle
- Sniper
- RocketLauncher
- GrenadeLauncher

### 3. Неверный формат даты

Поле `addedTime` должно быть в формате ISO 8601:
- Правильно: `"2024-01-01T10:00:00Z"`
- Неправильно: `"01/01/2024"`

### 4. Отсутствующее определение оружия

Если вы добавили оружие в раздел `weapons`, убедитесь, что оно определено в разделе `weaponDefinitions`.

## Интеграция с Unity

### Путь к файлу

По умолчанию файл находится в `Assets/saves/inventory_test.json`.

Вы можете изменить путь в инспекторе компонента `WeaponShopServiceSimple`:
1. Выберите GameObject с компонентом `WeaponShopServiceSimple`
2. В инспекторе найдите поле `Inventory File Path`
3. Укажите нужный путь (относительно папки Assets)

### Автосохранение

Система автоматически сохраняет изменения при:
- Покупке оружия
- Выборе оружия
- Закрытии игры

## Рекомендации по балансировке

### Стоимость оружия

| Тип оружия | Рекомендуемая стоимость |
|------------|------------------------|
| Handgun | 50-200 |
| SMG | 200-400 |
| Shotgun | 300-500 |
| AssaultRifle | 400-700 |
| Sniper | 600-1000 |

### Параметры урона

| Тип оружия | Рекомендуемый урон |
|------------|-------------------|
| Handgun | 15-30 |
| SMG | 15-25 |
| Shotgun | 60-100 |
| AssaultRifle | 30-45 |
| Sniper | 80-120 |

### Точность

Меньшее значение = выше точность:
- Handgun: 0.08-0.15
- SMG: 0.12-0.20
- Shotgun: 0.25-0.40
- AssaultRifle: 0.10-0.18
- Sniper: 0.01-0.05

## Поддержка

Если у вас возникли проблемы:
1. Проверьте валидность JSON файла
2. Проверьте консоль Unity на ошибки
3. Убедитесь, что компонент `WeaponShopServiceSimple` добавлен на сцену
4. Проверьте, что файл `inventory_test.json` существует в папке `Assets/saves/`
