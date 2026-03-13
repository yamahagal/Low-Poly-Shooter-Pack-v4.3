//Copyright 2024, Infima Games. All Rights Reserved.

using UnityEngine;
using InfimaGames.LowPolyShooterPack;

/// <summary>
/// Пример использования AttachmentConfigManager
/// </summary>
public class AttachmentConfigExample : MonoBehaviour
{
    [Header("Ссылки")]
    [Tooltip("Менеджер конфигурации обвесов")]
    [SerializeField]
    private AttachmentConfigManager configManager;

    [Header("Настройки")]
    [Tooltip("ID оружия для примера")]
    [SerializeField]
    private string exampleWeaponId = "ar_01";

    private void Start()
    {
        // Получить менеджер конфигурации, если не назначен
        if (configManager == null)
        {
            configManager = GetComponent<AttachmentConfigManager>();
        }

        // Примеры использования
        DemonstrateUsage();
    }

    /// <summary>
    /// Демонстрация использования AttachmentConfigManager
    /// </summary>
    private void DemonstrateUsage()
    {
        Debug.Log("=== Примеры использования AttachmentConfigManager ===");

        // 1. Загрузка конфигурации
        LoadConfigurationExample();

        // 2. Проверка статуса обвеса
        CheckAttachmentStatusExample();

        // 3. Получение доступных обвесов
        GetAvailableAttachmentsExample();

        // 4. Применение пресета
        ApplyPresetExample();

        // 5. Динамическое переключение оружия
        SwitchWeaponExample();

        // 6. Добавление сопоставлений
        AddMappingsExample();
    }

    /// <summary>
    /// Пример 1: Загрузка конфигурации
    /// </summary>
    private void LoadConfigurationExample()
    {
        Debug.Log("\n--- Пример 1: Загрузка конфигурации ---");

        // Загрузить все конфигурации
        configManager.LoadAllConfigs();

        Debug.Log("Конфигурации загружены успешно");
    }

    /// <summary>
    /// Пример 2: Проверка статуса обвеса
    /// </summary>
    private void CheckAttachmentStatusExample()
    {
        Debug.Log("\n--- Пример 2: Проверка статуса обвеса ---");

        string attachmentId = "scope_red_dot";

        // Проверить, куплен ли обвес
        bool isPurchased = configManager.IsAttachmentPurchased(attachmentId);
        Debug.Log($"Обвес {attachmentId} куплен: {isPurchased}");

        // Проверить, выбран ли обвес
        bool isSelected = configManager.IsAttachmentSelected(attachmentId);
        Debug.Log($"Обвес {attachmentId} выбран: {isSelected}");
    }

    /// <summary>
    /// Пример 3: Получение доступных обвесов
    /// </summary>
    private void GetAvailableAttachmentsExample()
    {
        Debug.Log("\n--- Пример 3: Получение доступных обвесов ---");

        // Получить доступные прицелы для оружия
        var availableScopes = configManager.GetAvailableAttachmentsForWeapon(exampleWeaponId, "Scope");
        Debug.Log($"Доступные прицелы для {exampleWeaponId}:");
        foreach (var scope in availableScopes)
        {
            Debug.Log($"  - {scope}");
        }

        // Получить текущий прицел
        string currentScope = configManager.GetCurrentAttachmentForSlot(exampleWeaponId, "scope");
        Debug.Log($"Текущий прицел: {currentScope}");
    }

    /// <summary>
    /// Пример 4: Применение пресета
    /// </summary>
    private void ApplyPresetExample()
    {
        Debug.Log("\n--- Пример 4: Применение пресета ---");

        // Получить все пресеты для оружия
        var presets = configManager.GetPresetsForWeapon(exampleWeaponId);
        Debug.Log($"Доступные пресеты для {exampleWeaponId}:");
        foreach (var preset in presets)
        {
            Debug.Log($"  - {preset.Key}: {preset.Value.name}");
        }

        // Применить пресет "cqb"
        if (presets.ContainsKey("cqb"))
        {
            configManager.ApplyPreset(exampleWeaponId, "cqb");
            Debug.Log("Пресет 'cqb' применён");
        }
    }

    /// <summary>
    /// Пример 5: Динамическое переключение оружия
    /// </summary>
    private void SwitchWeaponExample()
    {
        Debug.Log("\n--- Пример 5: Динамическое переключение оружия ---");

        // Переключиться на другое оружие
        string newWeaponId = "handgun_01";
        configManager.SetWeapon(newWeaponId);
        Debug.Log($"Переключено на оружие: {newWeaponId}");

        // Вернуться к исходному оружию
        configManager.SetWeapon(exampleWeaponId);
        Debug.Log($"Вернулись к оружию: {exampleWeaponId}");
    }

    /// <summary>
    /// Пример 6: Добавление сопоставлений
    /// </summary>
    private void AddMappingsExample()
    {
        Debug.Log("\n--- Пример 6: Добавление сопоставлений ---");

        // Добавить сопоставление для нового прицела
        configManager.AddMapping("scope", "scope_new", 2);
        Debug.Log("Добавлено сопоставление для scope_new -> индекс 2");

        // Добавить сопоставление для новой дульной насадки
        configManager.AddMapping("muzzle", "muzzle_new", 2);
        Debug.Log("Добавлено сопоставление для muzzle_new -> индекс 2");

        // Получить индекс по ID
        int scopeIndex = configManager.GetAttachmentIndex("scope", "scope_new");
        Debug.Log($"Индекс для scope_new: {scopeIndex}");
    }

    /// <summary>
    /// Пример использования в UI
    /// </summary>
    public void OnAttachmentButtonClick(string attachmentId)
    {
        Debug.Log($"Нажата кнопка обвеса: {attachmentId}");

        // Проверить, куплен ли обвес
        if (configManager.IsAttachmentPurchased(attachmentId))
        {
            // Обвес куплен, можно применить
            Debug.Log($"Обвес {attachmentId} уже куплен");
            // Здесь можно добавить логику применения обвеса
        }
        else
        {
            // Обвес не куплен, показать магазин
            Debug.Log($"Обвес {attachmentId} не куплен. Открыть магазин.");
            // Здесь можно добавить логику открытия магазина
        }
    }

    /// <summary>
    /// Пример использования при покупке обвеса
    /// </summary>
    public void OnAttachmentPurchased(string attachmentId)
    {
        Debug.Log($"Обвес куплен: {attachmentId}");

        // Перезагрузить конфигурацию доступности
        configManager.LoadAvailabilityConfig();

        // Применить конфигурацию к текущему оружию
        configManager.ApplyConfigToWeapon(exampleWeaponId);

        Debug.Log($"Конфигурация обновлена для обвеса: {attachmentId}");
    }

    /// <summary>
    /// Пример использования при смене оружия
    /// </summary>
    public void OnWeaponChanged(string newWeaponId)
    {
        Debug.Log($"Оружие изменено на: {newWeaponId}");

        // Применить конфигурацию для нового оружия
        configManager.SetWeapon(newWeaponId);

        Debug.Log($"Конфигурация применена для оружия: {newWeaponId}");
    }

    /// <summary>
    /// Пример создания пользовательского пресета
    /// </summary>
    public void CreateCustomPreset(string presetName, string weaponId)
    {
        Debug.Log($"Создание пользовательского пресета: {presetName}");

        // Здесь можно добавить логику создания пресета
        // Например, сохранить текущие обвесы в JSON файл

        Debug.Log($"Пресет {presetName} создан для оружия {weaponId}");
    }

    /// <summary>
    /// Пример получения информации об обвесе
    /// </summary>
    public void DisplayAttachmentInfo(string attachmentId)
    {
        Debug.Log($"=== Информация об обвесе: {attachmentId} ===");

        bool purchased = configManager.IsAttachmentPurchased(attachmentId);
        bool selected = configManager.IsAttachmentSelected(attachmentId);

        Debug.Log($"Куплен: {purchased}");
        Debug.Log($"Выбран: {selected}");

        // Получить текущий обвес для слота
        string currentAttachment = configManager.GetCurrentAttachmentForSlot(exampleWeaponId, "scope");
        Debug.Log($"Текущий обвес: {currentAttachment}");
    }
}
