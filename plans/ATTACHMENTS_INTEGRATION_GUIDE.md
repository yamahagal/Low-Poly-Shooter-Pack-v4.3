# Инструкция по интеграции системы обвесов в проект

## Обзор

Система обвесов полностью реализована и готова к интеграции в существующий проект. Все компоненты работают через JSON файлы без использования UI магазина.

## Структура файлов

### Основные файлы данных:
- `Assets/Data/attachments_config.json` - конфигурация всех обвесов
- `Assets/saves/inventory_test.json` - инвентарь игрока с обвесами

### C# классы системы обвесов:
- `Assets/Infima Games/Low Poly Shooter Pack/Code/Attachments/AttachmentType.cs` - enum типов обвесов
- `Assets/Infima Games/Low Poly Shooter Pack/Code/Attachments/AttachmentModifiers.cs` - класс модификаторов
- `Assets/Infima Games/Low Poly Shooter Pack/Code/Attachments/AttachmentData.cs` - классы данных обвесов
- `Assets/Infima Games/Low Poly Shooter Pack/Code/Attachments/WeaponStats.cs` - класс характеристик оружия

### Менеджеры:
- `Assets/Infima Games/Low Poly Shooter Pack/Code/Attachments/AttachmentManager.cs` - менеджер обвесов (Singleton)
- `Assets/Infima Games/Low Poly Shooter Pack/Code/Attachments/AttachmentModifierSystem.cs` - система модификаторов
- `Assets/Infima Games/Low Poly Shooter Pack/Code/Attachments/AttachmentInventoryManager.cs` - менеджер инвентаря обвесов (Singleton)

### Интеграция с инвентарём:
- `Assets/Infima Games/Low Poly Shooter Pack/Code/Character/InventoryData.cs` - обновлён с методами для работы с обвесами

### UI компоненты (для будущего магазина):
- `Assets/Infima Games/Low Poly Shooter Pack/UI/Attachments/AttachmentCard.cs` - карточка обвеса
- `Assets/Infima Games/Low Poly Shooter Pack/UI/Attachments/AttachmentPanel.cs` - панель выбора обвесов

## Шаг 1: Инициализация AttachmentManager

Добавьте `AttachmentManager` в сцену как Singleton. Он автоматически загрузит конфигурацию обвесов из `attachments_config.json`.

```csharp
// В классе Character или при старте игры
AttachmentManager.Instance.LoadConfig();
```

## Шаг 2: Инициализация AttachmentInventoryManager

Добавьте `AttachmentInventoryManager` в сцену. Он автоматически загрузит инвентарь из `inventory_test.json`.

```csharp
// В классе Character или при старте игры
AttachmentInventoryManager.Instance.LoadInventory();
```

## Шаг 3: Получение и установка обвесов

### Проверка купленного обвеса:
```csharp
var inventoryManager = AttachmentInventoryManager.Instance;
bool hasAttachment = inventoryManager.HasAttachment("handgun_01", "scope_red_dot");
```

### Покупка обвеса:
```csharp
bool purchased = inventoryManager.PurchaseAttachment("handgun_01", "scope_red_dot");
```

### Установка обвеса:
```csharp
bool equipped = inventoryManager.EquipAttachment("handgun_01", AttachmentType.Scope, "scope_red_dot");
```

### Снятие обвеса:
```csharp
bool unequipped = inventoryManager.UnequipAttachment("handgun_01", AttachmentType.Scope);
```

### Получение списка купленных обвесов:
```csharp
List<string> purchased = inventoryManager.GetPurchasedAttachments("handgun_01");
```

### Получение установленных обвесов:
```csharp
Dictionary<AttachmentType, string> equipped = inventoryManager.GetEquippedAttachments("handgun_01");
```

### Получение списка обвесов для оружия:
```csharp
var attachmentManager = AttachmentManager.Instance;
List<AttachmentData> attachments = attachmentManager.GetAttachmentsForWeapon("handgun_01");
```

### Получение обвесов по типу:
```csharp
List<AttachmentData> scopes = attachmentManager.GetAttachmentsForWeaponByType("handgun_01", AttachmentType.Scope);
```

### Проверка совместимости:
```csharp
bool isCompatible = attachmentManager.IsCompatible("scope_red_dot", "handgun_01");
```

## Шаг 4: Расчёт характеристик с модификаторами

### Получение базовых характеристик оружия:
```csharp
var weaponData = inventoryManager.GetWeaponData("handgun_01");
```

### Расчёт итоговых характеристик:
```csharp
WeaponStats finalStats = inventoryManager.CalculateFinalStats("handgun_01");
```

### Получение разницы характеристик:
```csharp
var modifierSystem = new AttachmentModifierSystem();
var baseStats = new WeaponStats { /* базовые значения */ };
var finalStats = modifierSystem.CalculateFinalStats(baseStats, equippedAttachments);
var differences = modifierSystem.GetStatsDifference(baseStats, finalStats);
```

## Шаг 5: Применение обвесов к оружию

### Применение установленных обвесов к WeaponAttachmentManager:
```csharp
// Получите ссылку на WeaponAttachmentManager текущего оружия
var weaponAttachmentManager = weaponGameObject.GetComponent<WeaponAttachmentManager>();

// Установите ссылку на AttachmentInventoryManager
AttachmentInventoryManager.Instance.SetCurrentWeaponAttachmentManager(weaponAttachmentManager);

// Примените обвесы
AttachmentInventoryManager.Instance.ApplyAttachmentsToWeapon("handgun_01");
```

## Шаг 6: Применение модификаторов к характеристикам оружия

### Получение модифицированных характеристик:
```csharp
var inventoryManager = AttachmentInventoryManager.Instance;
var weaponStats = inventoryManager.CalculateFinalStats("handgun_01");

// Примените к оружию
weapon.damage = weaponStats.damage;
weapon.fireRate = weaponStats.fireRate;
weapon.magazineCapacity = weaponStats.magazineCapacity;
weapon.reloadTime = weaponStats.reloadTime;
weapon.accuracy = weaponStats.accuracy;
weapon.range = weaponStats.range;
```

## Работа с магазинами (важное изменение!)

### Логика работы магазинов:
- `magazineCapacitySet: 0` - без изменений, используется базовая ёмкость оружия
- `magazineCapacitySet: 45` - устанавливает 45 патронов
- `magazineCapacitySet: 75` - устанавливает 75 патронов
- `magazineCapacitySet: 100` - устанавливает 100 патронов

### Примеры магазинов в attachments_config.json:

```json
"magazine_extended": {
  "attachmentName": "Расширенный магазин",
  "description": "Устанавливает 45 патронов",
  "modifiers": {
    "magazineCapacitySet": 45
  }
}
```

```json
"magazine_drum": {
  "attachmentName": "Барабанный магазин",
  "description": "Устанавливает 75 патронов",
  "modifiers": {
    "magazineCapacitySet": 75
  }
}
```

## Структура JSON инвентаря

### weaponAttachments - состояние обвесов для оружия:
```json
"weaponAttachments": {
  "handgun_01": {
    "weaponId": "handgun_01",
    "purchasedAttachments": {
      "scope_red_dot": true,
      "muzzle_compensator": true,
      "laser_basic": false
    },
    "equippedAttachments": {
      "Scope": "scope_red_dot",
      "Muzzle": "muzzle_compensator",
      "Laser": ""
    }
  }
}
```

### attachmentDefinitions - определения всех обвесов:
```json
"attachmentDefinitions": {
  "scope_red_dot": {
    "attachmentId": "scope_red_dot",
    "attachmentName": "Красная точка",
    "attachmentType": "Scope",
    "cost": 500,
    "modifiers": {
      "magazineCapacitySet": 0,
      "damageMultiplier": 1.0,
      "accuracyMultiplier": 0.9,
      "rangeMultiplier": 1.1
    },
    "compatibleWeapons": ["handgun_01", "handgun_02", "smg_01", "ar_01"]
  }
}
```

## Пример полного цикла работы с обвесами

```csharp
// 1. При старте игры инициализируем менеджеры
AttachmentManager.Instance.LoadConfig();
AttachmentInventoryManager.Instance.LoadInventory();

// 2. Получаем текущее оружие
string currentWeaponId = "handgun_01";

// 3. Проверяем, есть ли купленные обвесы
var inventoryManager = AttachmentInventoryManager.Instance;
bool hasScope = inventoryManager.HasAttachment(currentWeaponId, "scope_red_dot");

// 4. Если нет, покупаем
if (!hasScope)
{
    bool purchased = inventoryManager.PurchaseAttachment(currentWeaponId, "scope_red_dot");
}

// 5. Устанавливаем обвес
bool equipped = inventoryManager.EquipAttachment(currentWeaponId, AttachmentType.Scope, "scope_red_dot");

// 6. Применяем обвесы к оружию
AttachmentInventoryManager.Instance.ApplyAttachmentsToWeapon(currentWeaponId);

// 7. Рассчитываем итоговые характеристики
WeaponStats finalStats = inventoryManager.CalculateFinalStats(currentWeaponId);

// 8. Применяем к оружию
Weapon weapon = GetComponent<Weapon>();
weapon.damage = finalStats.damage;
weapon.fireRate = finalStats.fireRate;
weapon.magazineCapacity = finalStats.magazineCapacity;
// и т.д.

// 9. Сохраняем инвентарь при изменениях
AttachmentInventoryManager.Instance.SaveInventory();
```

## Типы обвесов

| Тип | Enum | Описание | Модификаторы |
|------|--------|----------|---------------|
| Прицел | Scope | Улучшает точность и дальность |
| Надульник | Muzzle | Уменьшает отдачу |
| Лазер | Laser | Улучшает точность с бедра |
| Цевье | Grip | Уменьшает отдачу, ускоряет перезарядку |
| Магазин | Magazine | Устанавливает фиксированное количество патронов |
| Сошки | Bipod | Улучшает точность лёжа |

## События для интеграции с UI магазина

При создании UI магазина используйте следующие методы:

```csharp
// Получить список обвесов типа Magazine
var attachmentManager = AttachmentManager.Instance;
List<AttachmentData> magazines = attachmentManager.GetAttachmentsForWeaponByType(weaponId, AttachmentType.Magazine);

// Для каждого магазина:
foreach (var magazine in magazines)
{
    int capacity = magazine.modifiers.magazineCapacitySet;
    string description = magazine.attachmentName;
    
    // Отобразить: "Магазин: 45 патронов"
    // или: "Магазин: 75 патронов"
}
```

## Сохранение состояния

Все изменения в инвентаре (покупка, установка, снятие обвесов) должны сохраняться:

```csharp
// После покупки
inventoryManager.PurchaseAttachment(weaponId, attachmentId);
inventoryManager.SaveInventory();

// После установки
inventoryManager.EquipAttachment(weaponId, type, attachmentId);
inventoryManager.SaveInventory();

// После снятия
inventoryManager.UnequipAttachment(weaponId, type);
inventoryManager.SaveInventory();
```

## Отладка

Для отладки используйте Debug.Log:

```csharp
Debug.Log($"[Attachments] Обвес {attachmentId} куплен для оружия {weaponId}");
Debug.Log($"[Attachments] Обвес {attachmentId} установлен на оружие {weaponId}");
Debug.Log($"[Attachments] Итоговые характеристики: {finalStats}");
```

## Следующие шаги

1. **Интеграция с WeaponAttachmentManager**: ✅ Реализованы методы для синхронизации установленных обвесов с массивами обвесов в `WeaponAttachmentManager`.

2. **Создание UI магазина**: Разработать интерфейс для покупки и выбора обвесов с использованием созданных классов.

3. **Тестирование**: Проверить работу всех методов на различных сценариях (покупка, установка, снятие, расчёт характеристик).

4. **Оптимизация**: Добавить кэширование для быстрого доступа к часто используемым данным.

## Интеграция с WeaponAttachmentManager

### Новые методы WeaponAttachmentManager

В [`WeaponAttachmentManager`](../Assets/Infima%20Games/Low%20Poly%20Shooter%20Pack/Code/Weapons/WeaponAttachmentManager.cs:1) добавлены следующие методы для интеграции с системой обвесов:

#### SetAttachment(AttachmentType, string)
Устанавливает обвес по типу и идентификатору.

```csharp
var weaponAttachmentManager = weaponGameObject.GetComponent<WeaponAttachmentManager>();
weaponAttachmentManager.SetAttachment(AttachmentType.Scope, "scope_red_dot");
weaponAttachmentManager.SetAttachment(AttachmentType.Muzzle, "muzzle_compensator");
```

#### SetAttachmentIndex(AttachmentType, int)
Устанавливает индекс обвеса по типу.

```csharp
weaponAttachmentManager.SetAttachmentIndex(AttachmentType.Scope, 0); // Первый прицел
weaponAttachmentManager.SetAttachmentIndex(AttachmentType.Muzzle, -1); // Снять надульник
```

#### ApplyAttachments(Dictionary<AttachmentType, string>)
Применяет все обвесы из словаря.

```csharp
var equippedAttachments = inventoryManager.GetEquippedAttachments("handgun_01");
weaponAttachmentManager.ApplyAttachments(equippedAttachments);
```

#### ResetAttachments()
Сбрасывает все обвесы на значения по умолчанию.

```csharp
weaponAttachmentManager.ResetAttachments();
```

### Применение обвесов к оружию

Полный пример применения обвесов к оружию:

```csharp
// 1. Получаем WeaponAttachmentManager текущего оружия
var weaponAttachmentManager = currentWeapon.GetComponent<WeaponAttachmentManager>();

// 2. Получаем установленные обвесы из инвентаря
var equippedAttachments = AttachmentInventoryManager.Instance.GetEquippedAttachments("handgun_01");

// 3. Применяем обвесы к WeaponAttachmentManager
weaponAttachmentManager.ApplyAttachments(equippedAttachments);

// 4. Рассчитываем итоговые характеристики
var finalStats = AttachmentInventoryManager.Instance.CalculateFinalStats("handgun_01");

// 5. Применяем характеристики к оружию
var weapon = currentWeapon.GetComponent<Weapon>();
weapon.damage = finalStats.damage;
weapon.fireRate = finalStats.fireRate;
weapon.magazineCapacity = finalStats.magazineCapacity;
weapon.reloadTime = finalStats.reloadTime;
weapon.accuracy = finalStats.accuracy;
weapon.range = finalStats.range;
```

### Примечание по поиску обвесов

Метод `FindAttachmentIndex` использует временное решение - поиск по имени GameObject. Для более надёжной интеграции рекомендуется:

1. Добавить поле `attachmentId` в классы `ScopeBehaviour`, `MuzzleBehaviour`, `LaserBehaviour`, `GripBehaviour`, `MagazineBehaviour`
2. Изменить метод `FindAttachmentIndex` для сравнения `attachmentId` вместо имени GameObject

Пример добавления `attachmentId` в `ScopeBehaviour`:

```csharp
public class ScopeBehaviour : MonoBehaviour
{
    [Tooltip("Идентификатор обвеса из attachments_config.json")]
    public string attachmentId;
    
    // ... остальной код
}
```
