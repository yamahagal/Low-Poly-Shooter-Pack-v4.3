# Настройка Current Weapon ID

## Где находится currentWeaponId?

В компоненте `AttachmentConfigManager` в инспекторе Unity вы найдёте поле **"Manual Weapon Id"** в секции **"Настройки"**.

## Как настроить?

### Вариант 1: Автоматическое определение (Рекомендуется)

1. **Оставьте поле "Manual Weapon Id" пустым**
2. **Включите опцию** "Auto Detect Weapon"
3. **Запустите игру**

Система автоматически определит текущее оружие по имени префаба и преобразует его в ID для JSON файла.

**Пример:**
```
Manual Weapon Id: (пусто)
Auto Detect Weapon: true
```

**В консоли вы увидите:**
```
[AttachmentConfigManager] Автоматически определено оружие: AR-02 -> ar_02
```

### Вариант 2: Ручное задание ID

1. **Введите ID оружия** в поле "Manual Weapon Id"
2. **Отключите опцию** "Auto Detect Weapon" (или оставьте включённой - manualWeaponId имеет приоритет)

**Доступные ID оружия:**
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

**Пример для AR-02:**
```
Manual Weapon Id: ar_02
Auto Detect Weapon: false
```

## Приоритет настройки

Система использует следующую логику для определения `currentWeaponId`:

1. **Если "Manual Weapon Id" не пустой** → Используется это значение
2. **Если "Auto Detect Weapon" включен** → Автоматическое определение по имени префаба
3. **Иначе** → Используется значение по умолчанию ("ar_01")

## Как работает автоматическое определение?

Система ищет компонент `Weapon` на том же объекте и получает значение поля `weaponName`.

Затем имя оружия преобразуется в ID для JSON файла:

| Имя оружия (weaponName) | ID в JSON |
|------------------------|-----------|
| AR-01 | ar_01 |
| AR-02 | ar_02 |
| AR-03 | ar_03 |
| HG-01 | handgun_01 |
| HG-02 | handgun_02 |
| HG-03 | handgun_03 |
| HG-04 | handgun_04 |
| SMG-01 | smg_01 |
| SMG-02 | smg_02 |
| SMG-03 | smg_03 |
| SN-01 | sniper_01 |
| SN-02 | sniper_02 |
| SN-03 | sniper_03 |

## Проблема: В руках AR-02, а пишет "Текущее оружие: ar_01"

### Решение 1: Проверьте поле weaponName в префабе

1. Откройте префаб оружия AR-02
2. Найдите компонент `Weapon`
3. Проверьте поле `Weapon Name`
4. Убедитесь, что оно указано правильно

### Решение 2: Используйте ручное задание ID

1. В компоненте `AttachmentConfigManager` введите `ar_02` в поле "Manual Weapon Id"
2. Отключите "Auto Detect Weapon" (или оставьте включённой)

### Решение 3: Добавьте новое правило преобразования

Если автоматическое определение работает неправильно, вы можете добавить новое правило в методе `ConvertWeaponNameToId` в файле [`AttachmentConfigManager.cs`](../Infima%20Games/Low%20Poly%20Shooter%20Pack/Code/Weapons/AttachmentConfigManager.cs):

```csharp
if (lowerName.Contains("ar_02") || lowerName.Contains("ar02"))
    return "ar_02";
```

## Отладка

Включите опцию **"Show Debug Messages"** в компоненте `AttachmentConfigManager` для просмотра подробной информации в консоли Unity:

```
[AttachmentConfigManager] Используется вручную заданный ID оружия: ar_02
[AttachmentConfigManager] Начало загрузки конфигураций. Текущее оружие: ar_02
```

или

```
[AttachmentConfigManager] Автоматически определено оружие: AR-02 -> ar_02
[AttachmentConfigManager] Начало загрузки конфигураций. Текущее оружие: ar_02
```

## Частые проблемы

### Проблема: "Не удалось определить ID для оружия"

**Причина:** Имя оружия не соответствует ни одному из правил преобразования.

**Решение:** 
1. Используйте ручное задание ID
2. Или добавьте новое правило в метод `ConvertWeaponNameToId`

### Проблема: "Компонент Weapon не найден"

**Причина:** Компонент `Weapon` не находится на том же объекте, что и `AttachmentConfigManager`.

**Решение:** Убедитесь, что компонент `Weapon` находится на том же объекте, что и `AttachmentConfigManager`.

### Проблема: ID оружия не найден в JSON файле

**Причина:** Указанный ID не существует в `weapons_attachments_config.json`.

**Решение:** Проверьте, что ID существует в JSON файле и добавьте его при необходимости.

## Дополнительная документация

- [Объяснение ID обвесов](ATTACHMENT_IDS_EXPLANATION.md)
- [Руководство по устранению неполадок](TROUBLESHOOTING_GUIDE.md)
- [Интеграция с WeaponAttachmentManager](../Infima%20Games/Low%20Poly%20Shooter%20Pack/Code/Weapons/ATTACHMENT_CONFIG_INTEGRATION.md)
