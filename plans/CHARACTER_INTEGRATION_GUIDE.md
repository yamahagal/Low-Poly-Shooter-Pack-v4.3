# Инструкция по интеграции системы обвесов в Character.cs

## Обзор

Для автоматического применения обвесов при смене оружия необходимо добавить интеграцию в класс `Character.cs`. Это позволит автоматически устанавливать обвесы из инвентаря на текущее оружие.

## Шаги интеграции

### 1. Добавьте поле для хранения weaponId текущего оружия

В классе `Character` добавьте поле для хранения идентификатора текущего оружия:

```csharp
/// <summary>
/// Идентификатор текущего оружия для системы обвесов.
/// </summary>
private string currentWeaponId;
```

### 2. Обновите метод RefreshWeaponSetup()

Добавьте код в метод `RefreshWeaponSetup()` после строки 785 (после получения magazine):

```csharp
//Apply Attachments from Inventory
//Получаем идентификатор текущего оружия из инвентаря
currentWeaponId = inventory.GetCurrentWeaponId();

//Проверяем, что у нас есть идентификатор оружия
if (!string.IsNullOrEmpty(currentWeaponId))
{
    //Устанавливаем ссылку на AttachmentInventoryManager
    AttachmentInventoryManager.Instance.SetCurrentWeaponAttachmentManager(weaponAttachmentManager);
    
    //Применяем обвесы из инвентаря к оружию
    AttachmentInventoryManager.Instance.ApplyAttachmentsToWeapon(currentWeaponId);
    
    //Рассчитываем и применяем итоговые характеристики
    ApplyWeaponStats(currentWeaponId);
}
```

### 3. Добавьте метод ApplyWeaponStats()

Добавьте новый метод для применения характеристик оружия:

```csharp
/// <summary>
/// Применяет характеристики оружия с учётом обвесов.
/// </summary>
/// <param name="weaponId">Идентификатор оружия.</param>
private void ApplyWeaponStats(string weaponId)
{
    //Получаем итоговые характеристики с учётом обвесов
    WeaponStats finalStats = AttachmentInventoryManager.Instance.CalculateFinalStats(weaponId);
    
    if (finalStats == null || equippedWeapon == null)
        return;
    
    //Применяем характеристики к оружию
    equippedWeapon.damage = finalStats.damage;
    equippedWeapon.fireRate = finalStats.fireRate;
    equippedWeapon.magazineCapacity = finalStats.magazineCapacity;
    equippedWeapon.reloadTime = finalStats.reloadTime;
    equippedWeapon.accuracy = finalStats.accuracy;
    equippedWeapon.range = finalStats.range;
    
    Debug.Log($"[Character] Применены характеристики для {weaponId}: {finalStats}");
}
```

### 4. Добавьте метод GetCurrentWeaponId() в Inventory

Если метод `GetCurrentWeaponId()` ещё не существует в классе `Inventory`, добавьте его:

```csharp
/// <summary>
/// Получает идентификатор текущего оружия.
/// </summary>
/// <returns>Идентификатор текущего оружия или пустая строка.</returns>
public string GetCurrentWeaponId()
{
    return inventoryData?.currentWeaponId ?? "";
}
```

## Полный пример обновлённого метода RefreshWeaponSetup()

```csharp
/// <summary>
/// Refresh all weapon things to make sure we're all set up!
/// </summary>
public void RefreshWeaponSetup()
{
    //Make sure we have a weapon. We don't want errors!
    if ((equippedWeapon = inventory.GetEquipped()) == null)
        return;

    //Reset shots fired when switching weapons.
    //This ensures recoil starts fresh with each weapon.
    shotsFired = 0;

    //Update Animator Controller. We do this to update all animations to a specific weapon's set.
    characterAnimator.runtimeAnimatorController = equippedWeapon.GetAnimatorController();

    //Get the attachment manager so we can use it to get all the attachments!
    weaponAttachmentManager = equippedWeapon.GetAttachmentManager();
    if (weaponAttachmentManager == null)
        return;

    //Get equipped scope. We need this one for its settings!
    equippedWeaponScope = weaponAttachmentManager.GetEquippedScope();
    //Get equipped magazine. We need this one for its settings!
    equippedWeaponMagazine = weaponAttachmentManager.GetEquippedMagazine();
    
    //=== ИНТЕГРАЦИЯ СИСТЕМЫ ОБВЕСОВ ===
    
    //Получаем идентификатор текущего оружия из инвентаря
    currentWeaponId = inventory.GetCurrentWeaponId();
    
    //Проверяем, что у нас есть идентификатор оружия
    if (!string.IsNullOrEmpty(currentWeaponId))
    {
        //Устанавливаем ссылку на AttachmentInventoryManager
        AttachmentInventoryManager.Instance.SetCurrentWeaponAttachmentManager(weaponAttachmentManager);
        
        //Применяем обвесы из инвентаря к оружию
        AttachmentInventoryManager.Instance.ApplyAttachmentsToWeapon(currentWeaponId);
        
        //Рассчитываем и применяем итоговые характеристики
        ApplyWeaponStats(currentWeaponId);
    }
}
```

## Примечания

1. **Порядок применения**: Сначала применяются визуальные обвесы (через WeaponAttachmentManager), затем характеристики (через WeaponStats).

2. **Сохранение**: Все изменения в инвентаре (покупка, установка, снятие обвесов) автоматически сохраняются через `AttachmentInventoryManager`.

3. **Идентификатор оружия**: Для корректной работы системы обвесов важно, чтобы weaponId в инвентаре соответствовал идентификаторам в `attachments_config.json`.

4. **Магазины**: При смене магазина через систему обвесов, ёмкость магазина автоматически обновляется в оружии.

## Тестирование

После интеграции проверьте следующее:

1. При смене оружия автоматически применяются установленные обвесы
2. Характеристики оружия (урон, скорострельность, ёмкость магазина) обновляются корректно
3. Визуальные обвесы (прицел, надульник, лазер, цевье, магазин) отображаются на модели оружия
4. При покупке/установке/снятии обвесов изменения сразу отражаются в игре

## Следующие шаги

После интеграции в Character.cs:

1. **Создание UI магазина**: Разработать интерфейс для покупки и выбора обвесов
2. **Добавление attachmentId в Behaviour классы**: Для более надёжной интеграции добавить поле `attachmentId` в `ScopeBehaviour`, `MuzzleBehaviour`, `LaserBehaviour`, `GripBehaviour`, `MagazineBehaviour`
3. **Тестирование**: Провести полное тестирование всех сценариев использования обвесов
