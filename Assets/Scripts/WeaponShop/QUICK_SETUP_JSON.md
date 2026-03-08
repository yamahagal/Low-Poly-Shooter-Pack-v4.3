# Быстрая настройка JSON режима

## Минимальная настройка для тестирования

### Шаг 1: Создайте пустой GameObject

1. В Unity Hierarchy: `Right Click > Create Empty`
2. Назовите его "GameManager"

### Шаг 2: Добавьте компоненты

На GameObject "GameManager" добавьте следующие компоненты:

#### 2.1. CurrencyService
1. `Add Component > Currency Service`
2. В Inspector:
   - **Starting Balance**: 1000
   - **Save Path**: `saves/currency_save.json`

#### 2.2. WeaponShopServiceSimple
1. `Add Component > Weapon Shop Service Simple`
2. В Inspector:
   - **Inventory File Path**: `saves/inventory_test.json`

### Шаг 3: Создайте JSON файл

1. Создайте папку `Assets/saves/` (если её нет)
2. Создайте файл `inventory_test.json` в этой папке
3. Скопируйте содержимое из примера ниже:

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
    }
  },
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

### Шаг 4: Запустите игру

1. Нажмите `Play`
2. Система загрузит данные из JSON файла
3. Вы можете редактировать JSON файл и изменения применятся при следующем запуске

## Полная настройка с UI

Если вам нужен полноценный магазин с UI, добавьте следующие компоненты:

### Дополнительные компоненты на "GameManager":

#### 3. WeaponShopScene
1. `Add Component > Weapon Shop Scene`
2. В Inspector заполните ссылки (после создания UI):
   - **Weapon List UI**: (ссылка на WeaponListUI)
   - **Weapon Preview 3D**: (ссылка на WeaponPreview3D)
   - **Weapon Stats Panel**: (ссылка на WeaponStatsPanel)
   - **Currency Balance UI**: (ссылка на CurrencyBalanceUI)
   - **Exit Button**: (ссылка на кнопку выхода)

### Создание UI

#### 1. Создайте Canvas
1. `Right Click > UI > Canvas`
2. В Inspector:
   - **Render Mode**: Screen Space - Overlay
   - **UI Scale Mode**: Scale With Screen Size
   - **Reference Resolution**: 1920 x 1080

#### 2. Создайте CurrencyBalanceUI
1. В Canvas: `Right Click > UI > Text - TextMeshPro`
2. Назовите "BalanceText"
3. Добавьте компонент `Currency Balance UI`
4. В Inspector:
   - **Balance Text**: перетащите сам объект "BalanceText"

#### 3. Создайте WeaponListUI
1. В Canvas: `Right Click > UI > Scroll View`
2. Назовите "WeaponList"
3. В Content (внутри Scroll View) добавьте компоненты:
   - `Vertical Layout Group`
   - `Content Size Fitter` → Vertical Fit: Preferred Size
4. На объекте "WeaponList" добавьте компонент `Weapon List UI`
5. В Inspector:
   - **Weapon List Container**: перетащите объект "Content"

#### 4. Создайте префаб карточки оружия
1. В Hierarchy: `Right Click > UI > Panel`
2. Назовите "WeaponCard"
3. Добавьте компонент `Weapon Card UI`
4. Создайте дочерние элементы:
   - Image для фона (уже есть на Panel)
   - TextMeshProUGUI для названия - "NameText"
   - TextMeshProUGUI для типа - "TypeText"
   - TextMeshProUGUI для стоимости - "CostText"
   - Button для покупки - "BuyButton"
   - Button для выбора - "SelectButton"
5. В Inspector компонента `Weapon Card UI` заполните ссылки
6. Перетащите "WeaponCard" в папку `Assets/Prefabs/`
7. Удалите "WeaponCard" из сцены

#### 5. Создайте WeaponStatsPanel
1. В Canvas: `Right Click > UI > Panel`
2. Назовите "StatsPanel"
3. Добавьте компонент `Weapon Stats Panel`
4. Создайте дочерние элементы для отображения статистики
5. В Inspector компонента заполните ссылки

#### 6. Создайте WeaponPreview3D
1. В Canvas: `Right Click > UI > Raw Image`
2. Назовите "PreviewImage"
3. Добавьте компонент `Weapon Preview 3D`
4. В Inspector:
   - **Preview Image**: перетащите сам объект "PreviewImage"

### Подключение ссылок

После создания всех UI элементов:

1. Выберите "GameManager"
2. В компоненте `Weapon Shop Scene`:
   - Перетащите "WeaponList" в поле **Weapon List UI**
   - Перетащите "PreviewImage" в поле **Weapon Preview 3D**
   - Перетащите "StatsPanel" в поле **Weapon Stats Panel**
   - Перетащите "BalanceText" в поле **Currency Balance UI**
   - Создайте кнопку выхода и перетащите её в поле **Exit Button**

3. Выберите "WeaponList"
4. В компоненте `Weapon List UI`:
   - Перетащите префаб "WeaponCard" в поле **Weapon Card Prefab**

## Проверка работы

1. Запустите игру
2. Проверьте консоль Unity на наличие ошибок
3. Проверьте, что отображается баланс валюты
4. Проверьте, что список оружия загружается из JSON

## Сравнение сервисов

| Сервис | Режим | Использование |
|--------|--------|--------------|
| `WeaponShopService` | ScriptableObject | Для редактирования в Unity Editor |
| `WeaponShopServiceSimple` | JSON | Для редактирования через JSON файл |
| `WeaponShopServiceJSON` | JSON | Расширенная версия с рефлексией |

**Рекомендация**: Используйте `WeaponShopServiceSimple` для JSON режима - он проще и не использует рефлексию.

## Интеграция с инвентарем игрока

### Как добавить оружие в руки игрока

После покупки оружие автоматически добавляется в инвентарь игрока через компонент `InventoryIntegration`.

**Шаги настройки:**

1. На GameObject "GameManager" добавьте компонент `Inventory Integration`
2. В Inspector компонента `Inventory Integration`:
   - **Auto Add To Inventory**: включено (по умолчанию)
   - **Weapon Prefabs**: перетащите все префабы оружия, которые могут быть куплены:
     - P_LPSP_WEP_Handgun_01.prefab
     - P_LPSP_WEP_SMG_01.prefab
     - P_LPSP_WEP_RL_01.prefab
     - P_LPSP_WEP_Shotgun_01.prefab
     - P_LPSP_WEP_Sniper_01.prefab

**Как работает:**

1. При покупке оружия в магазине срабатывает событие `OnWeaponPurchased`
2. Компонент `InventoryIntegration` обрабатывает это событие
3. Оружие добавляется в инвентарь игрока через `Instantiate`
4. Инвентарь игрока (из Low Poly Shooter Pack) автоматически активирует оружие

**Примечание:**

- Префабы должны находиться в папке `Assets/Infima Games/Low Poly Shooter Pack/prefabs/Weapons/`
- Название префаба должно совпадать с `weaponName` в JSON файле

## Частые проблемы

### 1. Файл JSON не найден
- Убедитесь, что файл находится в папке `Assets/saves/`
- Путь в Inspector должен быть: `saves/inventory_test.json`

### 2. Компонент не отображается в списке
- Проверьте, что нет ошибок компиляции в консоли
- Перезагрузите Unity

### 3. Ошибки в консоли при запуске
- Проверьте валидность JSON файла
- Убедитесь, что все поля заполнены правильно
