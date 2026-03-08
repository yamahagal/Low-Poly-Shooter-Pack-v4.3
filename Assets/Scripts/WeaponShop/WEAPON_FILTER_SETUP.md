# Настройка WeaponShopFilter

Эта инструкция объясняет, как настроить компонент `WeaponShopFilter` для фильтрации оружий по статусу `selected` в JSON файле.

**ВАЖНО:** Для работы фильтрации также требуется компонент `CharacterMod` с методами для фильтрованного переключения. См. раздел "Использование CharacterMod" ниже.

## Обзор

Компонент `WeaponShopFilter` работает параллельно с компонентом `Inventory` и предоставляет методы с фильтрацией оружий при переключении.

## Как работает

1. **Все оружия добавляются в инвентарь** - компонент `WeaponShopInventoryIntegration` добавляет ВСЕ оружия из JSON в инвентарь при запуске
2. **Фильтрация при переключении** - при переключении оружия (колесо мыши или клавиши 1-9, 0) проверяется статус `selected` в JSON
3. **Пропуск не выбранных** - если следующее оружие не выбрано (`selected: false`), система пропускает его и ищет следующее выбранное
4. **Публичные методы** - компонент предоставляет методы `GetNextIndexFiltered()` и `GetLastIndexFiltered()` для использования в других скриптах

## Настройка в Unity

### Шаг 1: Добавьте компонент WeaponShopFilter

1. Найдите GameObject игрока с компонентом `Inventory`
2. Добавьте компонент `WeaponShopFilter` на тот же GameObject

### Шаг 2: Убедитесь, что WeaponShopInventoryIntegration настроен

На GameObject "GameManager" должен быть компонент `WeaponShopInventoryIntegration` с настройками:
- ✅ `Add All Weapons On Start` - включено
- ✅ `Initialization Delay` - 0.5 (рекомендуется)
- ✅ `Weapons Folder Path` - `Infima Games/Low Poly Shooter Pack/prefabs/Weapons` (по умолчанию)

### Шаг 3: Настройте JSON файл

В файле `Assets/saves/inventory_test.json` установите статус `selected` для нужных оружий:

```json
"handgun_01": {
  "weaponId": "handgun_01",
  "purchased": true,
  "selected": true,  // Это оружие будет доступно для переключения
  "addedTime": "2024-01-01T10:00:00Z"
},
"smg_01": {
  "weaponId": "smg_01",
  "purchased": true,
  "selected": false,  // Это оружие НЕ будет доступно для переключения
  "addedTime": null
}
```

## Использование в других скриптах

Если у вас есть свой скрипт управления, который переключает оружия, используйте методы `WeaponShopFilter`:

```csharp
// Получите ссылку на компонент
WeaponShopFilter weaponFilter = FindObjectOfType<WeaponShopFilter>();

// Используйте фильтрованные методы
int nextIndex = weaponFilter.GetNextIndexFiltered();
int lastIndex = weaponFilter.GetLastIndexFiltered();

// Передайте индекс в компонент Inventory
playerInventory.Equip(nextIndex);
```

## Как проверить работу

1. Запустите игру
2. Проверьте консоль Unity на сообщения:
   - "WeaponShopFilter: Сервис магазина найден: WeaponShopServiceSimple"
   - "Добавлено оружий в инвентарь: X, пропущено (уже есть): Y"

3. Попробуйте переключать оружия колесом мыши
4. Убедитесь, что переключаются только оружия с `selected: true`

## Преимущества

- ✅ **Все оружия в инвентаре** - не нужно удалять и пересоздавать объекты
- ✅ **Фильтрация при переключении** - только выбранные оружия доступны для переключения
- ✅ **Простая настройка** - просто измените `selected` в JSON файле
- ✅ **Производительность** - нет лишних операций удаления/создания объектов
- ✅ **Совместимость** - работает параллельно с существующим `Inventory`

## Использование CharacterMod

Для работы фильтрации по статусу `selected` используйте компонент `CharacterMod`, который предоставляет методы для фильтрованного переключения оружия.

**Настройка:**
1. Найдите GameObject игрока с компонентом `Inventory`
2. Добавьте компонент `WeaponShopFilter` на тот же GameObject
3. Добавьте компонент `CharacterMod` на тот же GameObject

**Примечание:** CharacterMod использует reflection для вызова методов WeaponShopFilter, чтобы избежать циклической зависимости между сборками.

### Прямое использование WeaponShopFilter

Если у вас есть свой скрипт управления, который переключает оружия, используйте методы `WeaponShopFilter` напрямую:

```csharp
// Получите ссылку на компонент
WeaponShopFilter weaponFilter = GetComponent<WeaponShopFilter>();

// Используйте фильтрованные методы
int nextIndex = weaponFilter.GetNextIndexFiltered();
int lastIndex = weaponFilter.GetLastIndexFiltered();

// Передайте индекс в компонент Inventory
inventory.Equip(nextIndex);
```
1. Проверьте, что файл `WeaponShopFilter.cs` находится в папке `Assets/Scripts/WeaponShop/Integration/`
2. Перезагрузите Unity (`Assets > Reimport All`)
3. Проверьте консоль на ошибки компиляции

### Все оружия доступны для переключения

Если все оружия доступны для переключения:
1. Убедитесь, что на игроке стоит компонент `WeaponShopFilter`
2. Убедитесь, что на игроке стоит компонент `CharacterMod`
3. Проверьте консоль на сообщение "Найден WeaponShopFilter. Переключение будет работать с фильтрацией по selected."
4. Проверьте, что в JSON файле установлены правильные статусы `selected`

### Переключение не работает

Если переключение не работает:
1. Убедитесь, что в инвентаре есть оружия (проверьте в Hierarchy)
2. Проверьте, что компонент `WeaponShopFilter` инициализирован
3. Проверьте консоль на ошибки
4. Убедитесь, что скрипт управления использует методы `GetNextIndexFiltered()` или `GetLastIndexFiltered()` из `WeaponShopFilter`
