# JSON конфигурации для управления обвесами

## 📁 Созданные файлы

### 1. [`attachments_availability_config.json`](attachments_availability_config.json)
Управляет глобальной доступностью и статусом покупки обвесов.

**Основные возможности:**
- Покупка/выбор конкретных обвесов
- Отслеживание статуса покупки
- Управление датами покупки
- Глобальные настройки категорий

**Пример покупки обвеса:**
```json
"scope_red_dot": {
  "purchased": true,
  "selected": true,
  "purchaseDate": "2024-01-15T10:30:00Z"
}
```

### 2. [`weapons_attachments_config.json`](weapons_attachments_config.json)
Управляет конфигурацией обвесов для конкретного оружия.

**Основные возможности:**
- Установка обвесов на конкретное оружие
- Управление слотами для каждого оружия
- Система пресетов для быстрого переключения
- Совместимость обвесов с оружием

**Пример установки обвеса:**
```json
"scope": {
  "slotType": "Scope",
  "currentAttachment": "scope_red_dot",
  "availableAttachments": ["scope_red_dot", "scope_acog"],
  "selected": true
}
```

## 🚀 Быстрый старт

### Купить и выбрать обвес
В файле [`attachments_availability_config.json`](attachments_availability_config.json):
```json
"scope_red_dot": {
  "purchased": true,
  "selected": true,
  "purchaseDate": "2024-01-15T10:30:00Z"
}
```

### Не купленный обвес
В файле [`attachments_availability_config.json`](attachments_availability_config.json):
```json
"scope_acog": {
  "purchased": false,
  "selected": false,
  "purchaseDate": null
}
```

### Установить обвес на оружие
В файле [`weapons_attachments_config.json`](weapons_attachments_config.json):
```json
"ar_01": {
  "attachments": {
    "scope": {
      "currentAttachment": "scope_red_dot"
    }
  }
}
```

## 📋 Категории обвесов

- **Scope** - Прицелы и оптические прицелы
- **Muzzle** - Дульные насадки и глушители
- **Laser** - Лазерные целеуказатели
- **Grip** - Рукоятки и передние рукоятки
- **Magazine** - Магазины различной вместимости
- **Bipod** - Сошки для стабилизации

## 🔧 ID обвесов

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

## 🎯 Рекомендации

### Для полноценной игры
Используйте комбинацию:
- [`attachments_availability_config.json`](attachments_availability_config.json) для управления доступностью
- [`weapons_attachments_config.json`](weapons_attachments_config.json) для конфигурации оружия

## 📖 Документация

Подробная документация по структуре всех файлов находится в [`ATTACHMENTS_JSON_STRUCTURE.md`](ATTACHMENTS_JSON_STRUCTURE.md).

## ⚠️ Примечания

- Все даты в формате ISO 8601 (например: `2024-01-15T10:30:00Z`)
- `null` значения означают отсутствие объекта или даты
- Поля `purchased` и `selected` работают независимо в разных файлах
- Пресеты позволяют быстро переключаться между наборами обвесов
