# Руководство по устранению неполадок

## Проблема: Включил scope_red_dot, но на автомате другой прицел

### Возможные причины и решения:

### 1. Неправильное сопоставление ID с индексами

**Проблема:** ID `scope_red_dot` сопоставлен с неправильным индексом.

**Решение:**
1. Откройте префаб оружия в Unity
2. Найдите компонент `WeaponAttachmentManager`
3. Посмотрите на массив `Scope Array`
4. Запомните порядок прицелов в массиве:
   ```
   Scope Array (Size: 2)
   ├── [0] P_LPSP_WEP_ATT_Scope_01 (Красная точка)
   └── [1] P_LPSP_WEP_ATT_Scope_02 (ACOG)
   ```
5. В компоненте `AttachmentConfigManager` или `AttachmentConfigManagerDebug` настройте сопоставления:
   ```
   Scope Mappings:
   ├── Attachment Id: scope_red_dot, Array Index: 0
   └── Attachment Id: scope_acog, Array Index: 1
   ```

### 2. Использование отладочной версии

**Решение:**
1. Замените компонент `AttachmentConfigManager` на `AttachmentConfigManagerDebug`
2. Убедитесь, что опция `Show Debug Messages` включена
3. Запустите игру и посмотрите на консоль Unity

**Что искать в консоли:**
```
[AttachmentConfigManagerDebug] Текущий прицел из JSON: scope_red_dot
[AttachmentConfigManagerDebug] Полученный индекс прицела: 0
[AttachmentConfigManagerDebug] Установлено новое значение scopeIndex: 0
[AttachmentConfigManagerDebug] Установленный прицел: P_LPSP_WEP_ATT_Scope_01
```

Если индекс неправильный, проверьте сопоставления.

### 3. Рандомизация всё ещё включена

**Проблема:** Рандомизация прицелов не отключена.

**Решение:**
1. В компоненте `WeaponAttachmentManager` убедитесь, что `Scope Index Random` отключена
2. Или используйте `AttachmentConfigManagerDebug`, который автоматически отключает рандомизацию

### 4. Неправильный ID оружия

**Проблема:** `currentWeaponId` не соответствует ID в JSON файле.

**Решение:**
1. Проверьте `currentWeaponId` в компоненте `AttachmentConfigManager`
2. Убедитесь, что этот ID существует в `weapons_attachments_config.json`:
   ```json
   {
     "weaponsAttachments": {
       "ar_01": {  // ← Этот ID должен совпадать
         "weaponId": "ar_01",
         ...
       }
     }
   }
   ```

### 5. JSON файл не загружается

**Проблема:** Путь к JSON файлу неправильный.

**Решение:**
1. Проверьте путь к файлу в компоненте `AttachmentConfigManager`
2. Убедитесь, что файл существует по указанному пути
3. Используйте `AttachmentConfigManagerDebug` для проверки загрузки:
   ```
   [AttachmentConfigManagerDebug] Конфигурация загружена успешно
   ```

### 6. Неправильный порядок вызова методов

**Проблема:** Конфигурация применяется до того, как `WeaponAttachmentManager` инициализирован.

**Решение:**
1. Убедитесь, что `AttachmentConfigManager` и `WeaponAttachmentManager` на одном объекте
2. В `AttachmentConfigManagerDebug` проверьте, что `WeaponAttachmentManager` найден:
   ```
   [AttachmentConfigManagerDebug] WeaponAttachmentManager найден успешно
   ```

## Пошаговая диагностика

### Шаг 1: Проверьте JSON файл

Откройте `weapons_attachments_config.json` и убедитесь, что:
```json
{
  "weaponsAttachments": {
    "ar_01": {
      "attachments": {
        "scope": {
          "currentAttachment": "scope_red_dot",
          "availableAttachments": ["scope_red_dot", "scope_acog"],
          "selected": true
        }
      }
    }
  }
}
```

### Шаг 2: Проверьте массив прицелов в Unity

Откройте префаб оружия и проверьте `Scope Array`:
```
Scope Array (Size: 2)
├── [0] P_LPSP_WEP_ATT_Scope_01
└── [1] P_LPSP_WEP_ATT_Scope_02
```

### Шаг 3: Настройте сопоставления

В компоненте `AttachmentConfigManager` настройте:
```
Scope Mappings:
├── Attachment Id: scope_red_dot, Array Index: 0
└── Attachment Id: scope_acog, Array Index: 1
```

### Шаг 4: Используйте отладочную версию

Замените `AttachmentConfigManager` на `AttachmentConfigManagerDebug` и запустите игру.

### Шаг 5: Проанализируйте логи

Посмотрите на консоль Unity и найдите сообщения:
```
[AttachmentConfigManagerDebug] Текущий прицел из JSON: scope_red_dot
[AttachmentConfigManagerDebug] Полученный индекс прицела: 0
[AttachmentConfigManagerDebug] Установленный прицел: P_LPSP_WEP_ATT_Scope_01
```

## Частые ошибки

### Ошибка 1: "Сопоставление не найдено для scope_red_dot"

**Причина:** ID `scope_red_dot` не добавлен в список сопоставлений.

**Решение:** Добавьте сопоставление в `Scope Mappings`.

### Ошибка 2: "Недопустимый индекс прицела: -1"

**Причина:** Сопоставление не найдено или индекс отрицательный.

**Решение:** Проверьте, что индекс в сопоставлении >= 0.

### Ошибка 3: "Конфигурация для оружия ar_01 не найдена"

**Причина:** ID оружия не существует в JSON файле.

**Решение:** Проверьте `currentWeaponId` и убедитесь, что он существует в JSON.

### Ошибка 4: "Файл не найден: Assets/Data/weapons_attachments_config.json"

**Причина:** Путь к файлу неправильный.

**Решение:** Проверьте путь к файлу в компоненте.

## Пример правильной настройки

### 1. JSON конфигурация

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
      }
    }
  }
}
```

### 2. Массив прицелов в Unity

```
Scope Array (Size: 2)
├── [0] P_LPSP_WEP_ATT_Scope_01 (Красная точка)
└── [1] P_LPSP_WEP_ATT_Scope_02 (ACOG)
```

### 3. Сопоставления в AttachmentConfigManager

```
Scope Mappings:
├── Attachment Id: scope_red_dot, Array Index: 0
└── Attachment Id: scope_acog, Array Index: 1
```

### 4. Настройки в AttachmentConfigManager

```
Current Weapon Id: ar_01
Auto Load On Start: true
Show Debug Messages: true
```

## Дополнительная помощь

Если проблема не решена:

1. **Используйте отладочную версию:** `AttachmentConfigManagerDebug`
2. **Включите отладочные сообщения:** `Show Debug Messages = true`
3. **Проверьте консоль Unity:** Ищите сообщения с префиксом `[AttachmentConfigManagerDebug]`
4. **Сделайте скриншот:** Консоли Unity и настроек компонента
5. **Проверьте логи:** Сравните ожидаемые и фактические значения

## Связанная документация

- [Объяснение ID обвесов](ATTACHMENT_IDS_EXPLANATION.md)
- [Интеграция с WeaponAttachmentManager](../Infima%20Games/Low%20Poly%20Shooter%20Pack/Code/Weapons/ATTACHMENT_CONFIG_INTEGRATION.md)
- [Структура JSON файлов](ATTACHMENTS_JSON_STRUCTURE.md)
