# Руководство по системе сопоставлений обвесов (Версия 1.0)

## Обзор

Система сопоставлений обвесов связывает ID обвесов из JSON конфигурации с индексами в массивах [`WeaponAttachmentManager`](Assets/Infima%20Games/Low%20Poly%20Shooter%20Pack/Code/Weapons/WeaponAttachmentManager.cs). **Версия 1.0** использует глобальные сопоставления для всех оружий, что идеально подходит, когда у всех оружий одинаковые прицелы и другие обвесы в массивах.

Все сопоставления хранятся в файле [`attachment_mappings.json`](Assets/Data/attachment_mappings.json).

---

## Структура файла сопоставлений

Файл: `Assets/Data/attachment_mappings.json`

```json
{
  "version": "1.0",
  "description": "Сопоставление ID обвесов с индексами в массивах WeaponAttachmentManager (глобальные сопоставления для всех оружий)",
  "mappings": {
    "scopes": [
      {
        "attachmentId": "scope_red_dot",
        "arrayIndex": 0,
        "name": "Red Dot Sight"
      },
      {
        "attachmentId": "scope_acog",
        "arrayIndex": 1,
        "name": "ACOG Scope"
      }
    ],
    "muzzles": [...],
    "lasers": [...],
    "grips": [...],
    "magazines": [...]
  }
}
```

### Поля

- **`version`**: Версия формата файла (текущая: "1.0")
- **`description`**: Описание файла
- **`mappings`**: Словарь сопоставлений для каждого типа обвесов
  - Ключи: `"scopes"`, `"muzzles"`, `"lasers"`, `"grips"`, `"magazines"`
  - Значения: Массивы сопоставлений

### Сопоставление одного обвеса

```json
{
  "attachmentId": "scope_red_dot",  // ID из JSON конфигурации
  "arrayIndex": 0,                    // Индекс в массиве WeaponAttachmentManager
  "name": "Red Dot Sight"             // Название для отображения
}
```

---

## Как работает система

### Поток данных

```mermaid
flowchart TD
    A[weapons_attachments_config.json<br/>ID обвеса] -->|scope_acog| B[AttachmentConfigManager]
    C[attachment_mappings.json<br/>Глобальные сопоставления] -->|scope_acog -> 1| B
    B -->|Индекс 1| D[WeaponAttachmentManager]
    D -->|scopeArray[1]| E[Объект прицела ACOG]
```

### Процесс загрузки

1. [`AttachmentConfigManager.LoadAllConfigs()`](Assets/Infima%20Games/Low%20Poly%20Shooter%20Pack/Code/Weapons/AttachmentConfigManager.cs:94) вызывается при старте
2. [`LoadMappingsConfig()`](Assets/Infima%20Games/Low%20Poly%20Shooter%20Pack/Code/Weapons/AttachmentConfigManager.cs:337) загружает глобальные сопоставления из JSON
3. [`LoadWeaponsConfig()`](Assets/Infima%20Games/Low%20Poly%20Shooter%20Pack/Code/Weapons/AttachmentConfigManager.cs:301) загружает конфигурацию оружия
4. [`ApplyConfigToWeapon()`](Assets/Infima%20Games/Low%20Poly%20Shooter%20Pack/Code/Weapons/AttachmentConfigManager.cs:332) применяет конфигурацию
5. [`GetAttachmentIndex()`](Assets/Infima%20Games/Low%20Poly%20Shooter%20Pack/Code/Weapons/AttachmentConfigManager.cs:490) находит индекс по ID обвеса
6. Индекс устанавливается в [`WeaponAttachmentManager`](Assets/Infima%20Games/Low%20Poly%20Shooter%20Pack/Code/Weapons/WeaponAttachmentManager.cs)

---

## Добавление нового обвеса

### Шаг 1: Создайте объект обвеса в Unity

1. Создайте префаб обвеса в Unity
2. Добавьте компонент (например, [`ScopeBehaviour`](Assets/Infima%20Games/Low%20Poly%20Shooter%20Pack/Code/Weapons/ScopeBehaviour.cs))
3. Добавьте префаб в соответствующий массив в [`WeaponAttachmentManager`](Assets/Infima%20Games/Low%20Poly%20Shooter%20Pack/Code/Weapons/WeaponAttachmentManager.cs)

### Шаг 2: Добавьте сопоставление в JSON

Откройте [`attachment_mappings.json`](Assets/Data/attachment_mappings.json) и добавьте новое сопоставление:

```json
{
  "mappings": {
    "scopes": [
      {
        "attachmentId": "scope_new_scope",
        "arrayIndex": 3,
        "name": "New Scope"
      }
    ]
  }
}
```

**Важно:** `arrayIndex` должен соответствовать индексу обвеса в массиве в Unity!

### Шаг 3: Добавьте обвес в конфигурацию оружия

Откройте [`weapons_attachments_config.json`](Assets/Data/weapons_attachments_config.json) и добавьте обвес:

```json
{
  "ar_01": {
    "attachments": {
      "scope": {
        "availableAttachments": ["scope_red_dot", "scope_acog", "scope_new_scope"]
      }
    }
  }
}
```

### Шаг 4: Установите обвес как текущий

```json
{
  "scope": {
    "currentAttachment": "scope_new_scope",
    "availableAttachments": ["scope_red_dot", "scope_acog", "scope_new_scope"]
  }
}
```

---

## Изменение порядка обвесов

Если вы изменили порядок обвесов в массивах Unity, обновите `arrayIndex` в [`attachment_mappings.json`](Assets/Data/attachment_mappings.json):

```json
{
  "mappings": {
    "scopes": [
      {
        "attachmentId": "scope_acog",
        "arrayIndex": 0  // Было 1, стало 0
      },
      {
        "attachmentId": "scope_red_dot",
        "arrayIndex": 1  // Было 0, стало 1
      }
    ]
  }
}
```

---

## Типы слотов

Система поддерживает следующие типы слотов:

| Тип слота | Массив в WeaponAttachmentManager | Ключ в JSON |
|-----------|----------------------------------|-------------|
| Прицелы | `scopeArray` | `"scopes"` |
| Дульные насадки | `muzzleArray` | `"muzzles"` |
| Лазеры | `laserArray` | `"lasers"` |
| Рукоятки | `gripArray` | `"grips"` |
| Магазины | `magazineArray` | `"magazines"` |

---

## Отладка

### Включение отладочных сообщений

В [`AttachmentConfigManager`](Assets/Infima%20Games/Low%20Poly%20Shooter%20Pack/Code/Weapons/AttachmentConfigManager.cs) установите `Show Debug Messages = true` в Inspector.

### Логи

При загрузке вы увидите логи:

```
[AttachmentConfigManager] Сопоставления загружены успешно (версия: 1.0)
[AttachmentConfigManager] Типов обвесов в сопоставлениях: 5
[AttachmentConfigManager] Поиск индекса для scopes.scope_acog
[AttachmentConfigManager] Доступные сопоставления: 4
[AttachmentConfigManager]   - scope_red_dot -> 0
[AttachmentConfigManager]   - scope_acog -> 1
[AttachmentConfigManager]   - scope_sniper -> 2
[AttachmentConfigManager]   - scope_holographic -> 3
[AttachmentConfigManager] Найден индекс: 1
```

### Ошибки

Если сопоставление не найдено:

```
[AttachmentConfigManager] Сопоставление не найдено для scope_unknown
```

Проверьте:
1. Правильность ID обвеса в [`weapons_attachments_config.json`](Assets/Data/weapons_attachments_config.json)
2. Наличие сопоставления в [`attachment_mappings.json`](Assets/Data/attachment_mappings.json)
3. Соответствие типа слота (например, `"scope"` вместо `"scopes"`)

---

## Преимущества глобальных сопоставлений

### По сравнению с ручным сопоставлением в Inspector

✅ **Единое хранилище:** Все сопоставления в одном JSON файле  
✅ **Контроль версий:** Легко отслеживать изменения через Git  
✅ **Удобство:** Редактирование без Unity  
✅ **Масштабируемость:** Легко добавлять новые типы обвесов  
✅ **Автоматизация:** Можно создать скрипты для автоматического обновления  

### Когда использовать глобальные сопоставления

✅ У всех оружий одинаковые прицелы и другие обвесы в массивах  
✅ Индексы обвесов одинаковы для всех оружий  
✅ Хотите упростить управление сопоставлениями  

### Когда использовать индивидуальные сопоставления (версия 2.0)

❌ У разных оружий разные прицелы и другие обвесы в массивах  
❌ Индексы обвесов отличаются для разных оружий  
❌ Нужна максимальная гибкость для каждого оружия  

---

## Классы данных

### [`AttachmentMapping`](Assets/Infima%20Games/Low%20Poly%20Shooter%20Pack/Code/Weapons/AttachmentMappingsData.cs:10)

```csharp
[Serializable]
public class AttachmentMapping
{
    public string attachmentId;  // ID обвеса из JSON
    public int arrayIndex;       // Индекс в массиве
    public string name;           // Название для отображения
}
```

### [`AttachmentMappingsData`](Assets/Infima%20Games/Low%20Poly%20Shooter%20Pack/Code/Weapons/AttachmentMappingsData.cs:21)

```csharp
[Serializable]
public class AttachmentMappingsData
{
    public string version;
    public string description;
    public Dictionary<string, List<AttachmentMapping>> mappings;
}
```

---

## API

### [`AttachmentConfigManager.LoadMappingsConfig()`](Assets/Infima%20Games/Low%20Poly%20Shooter%20Pack/Code/Weapons/AttachmentConfigManager.cs:337)

Загружает глобальные сопоставления из JSON файла.

```csharp
public void LoadMappingsConfig()
```

### [`AttachmentConfigManager.GetAttachmentIndex()`](Assets/Infima%20Games/Low%20Poly%20Shooter%20Pack/Code/Weapons/AttachmentConfigManager.cs:490)

Получает индекс обвеса по ID.

```csharp
public int GetAttachmentIndex(string slotType, string attachmentId)
```

**Параметры:**
- `slotType`: Тип слота (`"scope"`, `"muzzle"`, `"laser"`, `"grip"`, `"magazine"`)
- `attachmentId`: ID обвеса из JSON конфигурации

**Возвращает:** Индекс в массиве или `-1`, если сопоставление не найдено

---

## Частые вопросы

### Q: Что делать, если индекс обвеса изменился?

A: Обновите `arrayIndex` в [`attachment_mappings.json`](Assets/Data/attachment_mappings.json).

### Q: Можно ли использовать один и тот же обвес для разных оружий?

A: Да, ID обвеса в [`weapons_attachments_config.json`](Assets/Data/weapons_attachments_config.json) должен быть одинаковым.

### Q: В чём разница между версией 1.0 и 2.0?

A: Версия 1.0 использует глобальные сопоставления для всех оружий. Версия 2.0 использует индивидуальные сопоставления для каждого оружия.

### Q: Что делать, если файл сопоставлений не найден?

A: Проверьте путь в поле `Mappings Config Path` в Inspector [`AttachmentConfigManager`](Assets/Infima%20Games/Low%20Poly%20Shooter%20Pack/Code/Weapons/AttachmentConfigManager.cs).

### Q: Когда использовать версию 1.0, а когда 2.0?

A: Используйте версию 1.0, если у всех оружий одинаковые прицелы и другие обвесы в массивах. Используйте версию 2.0, если у разных оружий разные прицелы и другие обвесы с разными индексами.

---

## История версий

- **1.0** - Глобальные сопоставления для всех оружий (текущая версия)
- **2.0** - Индивидуальные сопоставления для каждого оружия (доступно при необходимости)

---

## Связанные файлы

- [`AttachmentConfigManager.cs`](Assets/Infima%20Games/Low%20Poly%20Shooter%20Pack/Code/Weapons/AttachmentConfigManager.cs) - Менеджер конфигурации обвесов
- [`AttachmentMappingsData.cs`](Assets/Infima%20Games/Low%20Poly%20Shooter%20Pack/Code/Weapons/AttachmentMappingsData.cs) - Классы данных для сопоставлений
- [`WeaponAttachmentManager.cs`](Assets/Infima%20Games/Low%20Poly%20Shooter%20Pack/Code/Weapons/WeaponAttachmentManager.cs) - Менеджер обвесов оружия
- [`attachment_mappings.json`](Assets/Data/attachment_mappings.json) - Файл сопоставлений
- [`weapons_attachments_config.json`](Assets/Data/weapons_attachments_config.json) - Конфигурация обвесов оружия
- [`ATTACHMENT_MAPPINGS_REFACTOR_PLAN.md`](plans/ATTACHMENT_MAPPINGS_REFACTOR_PLAN.md) - План рефакторинга
