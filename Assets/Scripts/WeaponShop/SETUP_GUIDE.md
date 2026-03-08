# Инструкция по настройке сцены магазина оружия

> **📘 Подробная инструкция по настройке оружия через JSON**: см. файл [`JSON_WEAPON_SETUP.md`](Assets/Scripts/WeaponShop/JSON_WEAPON_SETUP.md)

## JSON режим (рекомендуется для редактирования оружий в файле)

### Шаг 1: Создание пустой сцены

1. В Unity перейдите в `File > New Scene`
2. Сохраните сцену как `Assets/Scenes/WeaponShop.unity`

### Шаг 2: Добавление компонентов на сцену

Создайте пустой GameObject "WeaponShopManager" и добавьте следующие компоненты:

#### 2.1. CurrencyService
1. На GameObject "WeaponShopManager" добавьте компонент `CurrencyService`
2. В Inspector:
   - Starting Balance: 1000
   - Save Path: `saves/currency_save.json`

#### 2.2. WeaponShopServiceJSON (для JSON режима)
1. На GameObject "WeaponShopManager" добавьте компонент `WeaponShopServiceJSON`
2. В Inspector:
   - Inventory File Path: `saves/inventory_test.json`

#### 2.3. WeaponShopScene
1. На GameObject "WeaponShopManager" добавьте компонент `WeaponShopScene`
2. В Inspector заполните ссылки (создадим их позже):
   - Weapon List UI: (пока пусто)
   - Weapon Preview 3D: (пока пусто)
   - Weapon Stats Panel: (пока пусто)
   - Currency Balance UI: (пока пусто)
   - Exit Button: (пока пусто)

### Шаг 3: Создание UI

#### 3.1. Создание Canvas
1. В Hierarchy: `Right Click > UI > Canvas`
2. В Canvas Inspector:
   - Render Mode: Screen Space - Overlay
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1920 x 1080

#### 3.2. Создание Panel для магазина
1. В Canvas: `ight Click > UI > Panel`
2. Назовите "ShopPanel"
3. Установите Anchor Presets: Stretch/Stretch

#### 3.3. Создание WeaponListUI
1. В ShopPanel: `Right Click > UI > Scroll View`
2. Назовите "WeaponList"
3. В Scroll View Inspector:
   - Content: добавьте компонент `Vertical Layout Group`
   - Content: добавьте компонент `Content Size Fitter`
     - Vertical Fit: Preferred Size

4. На объекте "WeaponList" (родитель Scroll View) добавьте компонент `WeaponListUI`
5. В Inspector:
   - Weapon List Container: перетащите объект "Content" из Scroll View
   - Weapon Card Prefab: (создадим позже)

#### 3.4. Создание CurrencyBalanceUI
1. В ShopPanel: `Right Click > UI > Text - TextMeshPro`
2. Назовите "BalanceText"
3. В Inspector:
   - Text: "Баланс: 0"
   - Font Size: 24
   - Alignment: Right

4. На объекте "BalanceText" добавьте компонент `CurrencyBalanceUI`
5. В Inspector:
   - Balance Text: перетащите сам объект "BalanceText"

### Шаг 4: Создание префаба карточки оружия

#### 4.1. Создание карточки
1. В Hierarchy: `Right Click > UI > Panel`
2. Назовите "WeaponCard"
3. В Inspector:
   - Width: 300
   - Height: 400

4. В WeaponCard добавьте компонент `WeaponCardUI`
5. В Inspector создайте ссылки (пока пустые):
   - Weapon Image: (пусто)
   - Name Text: (пусто)
   - Type Text: (пусто)
   - Cost Text: (пусто)
   - Buy Button: (пусто)
   - Select Button: (пусто)
   - Purchased Indicator: (пусто)
   - Selected Indicator: (пусто)

6. Перетащите "WeaponCard" из Hierarchy в папку `Assets/Prefabs/` для создания префаба
7. Удалите "WeaponCard" из сцены

#### 4.2. Заполнение карточки
1. Откройте префаб "WeaponCard"
2. Добавьте Image для фона (уже есть на Panel)
3. Добавьте TextMeshProUGUI для названия - "NameText"
4. Добавьте TextMeshProUGUI для типа - "TypeText"
5. Добавьте TextMeshProUGUI для стоимости - "CostText"
6. Добавьте Button для покупки - "BuyButton"
7. Добавьте Button для выбора - "SelectButton"
8. Добавьте GameObject для индикатора купленного - "PurchasedIndicator"
9. Добавьте GameObject для индикатора выбранного - "SelectedIndicator"

10. В компоненте `WeaponCardUI` перетащите созданные элементы:
    - Weapon Image: (опционально, можно добавить Image)
    - Name Text: NameText
    - Type Text: TypeText
    - Cost Text: CostText
    - Buy Button: BuyButton
    - Select Button: SelectButton
    - Purchased Indicator: PurchasedIndicator
    - Selected Indicator: SelectedIndicator

### Шаг 5: Связывание компонентов

1. Откройте объект "WeaponList" на сцене
2. В компоненте `WeaponListUI`:
   - Weapon Card Prefab: перетащите префаб "WeaponCard"

3. Откройте объект "WeaponShopManager" на сцене
4. В компоненте `WeaponShopScene`:
   - Weapon List UI: перетащите объект "WeaponList"
   - Weapon Preview 3D: (создадим позже)
   - Weapon Stats Panel: (создадим позже)
   - Currency Balance UI: перетащите объект "BalanceText"
   - Exit Button: (создадим позже)

### Шаг 6: Сохранение и тестирование

1. Сохраните сцену: `Ctrl+S`
2. Запустите игру: `Ctrl+P`
3. В консоли должны увидеть сообщения о загрузке инвентаря

## Минимальная конфигурация для быстрого теста

Если вы хотите быстро протестировать систему, создайте простую сцену:

1. Создайте пустой GameObject "GameManager"
2. Добавьте компонент `CurrencyService`
3. Добавьте компонент `WeaponShopServiceJSON`
4. Добавьте компонент `WeaponShopScene` (оставьте ссылки пустыми)

5. Запустите игру - система загрузит данные из `Assets/saves/inventory_test.json`

## Проверка работоспособности

После настройки:
1. Запустите игру
2. В консоли должны быть сообщения:
   - "Инвентарь загружен из Assets. Оружий: 5"
3. Ошибок не должно быть

## Устранение проблем

### Проблема: "Сервис магазина оружия не найден в сцене"
**Решение:** Убедитесь, что на сцене есть компонент `WeaponShopServiceJSON`

### Проблема: "Сервис валюты не найден в сцене"
**Решение:** Убедитесь, что на сцене есть компонент `CurrencyService`

### Проблема: "Инвентарь загружен из persistentDataPath"
**Решение:** Это нормально. Система сначала пробует загрузить из Assets, затем из persistentDataPath

### Проблема: "Создан пустой инвентарь"
**Решение:** Убедитесь, что файл `Assets/saves/inventory_test.json` существует и содержит секцию `weaponDefinitions`
