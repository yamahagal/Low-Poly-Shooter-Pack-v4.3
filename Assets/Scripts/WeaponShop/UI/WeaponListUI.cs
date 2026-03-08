/*
// ВРЕМЕННО ЗАКОММЕНТИРОВАНО - ФУНКЦИОНАЛЬНОСТЬ МАГАЗИНА ОТКЛЮЧЕНА
// Этот файл содержит UI списка оружия для магазина

using System.Collections.Generic;
using UnityEngine;

namespace WeaponShop
{
    /// <summary>
    /// UI компонент для отображения списка оружия в магазине.
    /// </summary>
    public class WeaponListUI : MonoBehaviour
    {
        #region FIELDS

        [Header("UI Elements")]
        [Tooltip("Контейнер для карточек оружия.")]
        [SerializeField]
        private Transform weaponListContainer;

        [Tooltip("Префаб карточки оружия.")]
        [SerializeField]
        private GameObject weaponCardPrefab;

        [Header("Settings")]
        [Tooltip("Автоматически выбирать первое купленное оружие.")]
        [SerializeField]
        private bool autoSelectFirstPurchased = true;

        #endregion

        #region FIELDS PRIVATE

        private IWeaponShopService weaponShopService;
        private List<WeaponCardUI> weaponCards = new List<WeaponCardUI>();
        private bool isInitialized = false;

        #endregion

        #region UNITY METHODS

        private void Start()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            Cleanup();
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Обновляет список оружия.
        /// </summary>
        public void RefreshWeaponList()
        {
            ClearList();
            GenerateWeaponList();
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Инициализирует компонент.
        /// </summary>
        private void Initialize()
        {
            weaponShopService = FindObjectOfType<WeaponShopService>();

            if (weaponShopService == null)
            {
                Debug.LogError("Сервис магазина оружия не найден в сцене.");
                return;
            }

            // Подписываемся на события
            weaponShopService.OnWeaponPurchased += HandleWeaponPurchased;
            weaponShopService.OnWeaponSelected += HandleWeaponSelected;
            weaponShopService.OnDataChanged += HandleDataChanged;

            // Генерируем список оружия
            GenerateWeaponList();

            // Выбираем первое купленное оружие
            if (autoSelectFirstPurchased)
            {
                SelectFirstPurchasedWeapon();
            }

            isInitialized = true;
        }

        /// <summary>
        /// Очищает подписки.
        /// </summary>
        private void Cleanup()
        {
            if (weaponShopService != null)
            {
                weaponShopService.OnWeaponPurchased -= HandleWeaponPurchased;
                weaponShopService.OnWeaponSelected -= HandleWeaponSelected;
                weaponShopService.OnDataChanged -= HandleDataChanged;
            }
        }

        /// <summary>
        /// Генерирует список оружия.
        /// </summary>
        private void GenerateWeaponList()
        {
            if (weaponShopService == null || weaponCardPrefab == null || weaponListContainer == null)
            {
                return;
            }

            List<WeaponData> weapons = weaponShopService.GetAllWeapons();

            foreach (WeaponData weaponData in weapons)
            {
                GameObject cardObject = Instantiate(weaponCardPrefab, weaponListContainer);
                WeaponCardUI cardUI = cardObject.GetComponent<WeaponCardUI>();

                if (cardUI != null)
                {
                    cardUI.Setup(weaponData);
                    weaponCards.Add(cardUI);
                }
                else
                {
                    Debug.LogError("Префаб карточки оружия не содержит компонент WeaponCardUI.");
                    Destroy(cardObject);
                }
            }

            Debug.Log($"Сгенерировано {weaponCards.Count} карточек оружия.");
        }

        /// <summary>
        /// Очищает список оружия.
        /// </summary>
        private void ClearList()
        {
            foreach (WeaponCardUI cardUI in weaponCards)
            {
                if (cardUI != null && cardUI.gameObject != null)
                {
                    Destroy(cardUI.gameObject);
                }
            }

            weaponCards.Clear();
        }

        /// <summary>
        /// Выбирает первое купленное оружие.
        /// </summary>
        private void SelectFirstPurchasedWeapon()
        {
            if (weaponShopService == null)
            {
                return;
            }

            List<WeaponData> purchasedWeapons = weaponShopService.GetPurchasedWeapons();

            if (purchasedWeapons.Count > 0)
            {
                weaponShopService.SelectWeapon(purchasedWeapons[0].WeaponId);
            }
        }

        #endregion

        #region EVENT HANDLERS

        /// <summary>
        /// Обрабатывает покупку оружия.
        /// </summary>
        private void HandleWeaponPurchased(string weaponId)
        {
            // Обновляем все карточки
            foreach (WeaponCardUI cardUI in weaponCards)
            {
                cardUI.Refresh();
            }
        }

        /// <summary>
        /// Обрабатывает выбор оружия.
        /// </summary>
        private void HandleWeaponSelected(string weaponId)
        {
            // Обновляем все карточки
            foreach (WeaponCardUI cardUI in weaponCards)
            {
                cardUI.Refresh();
            }
        }

        /// <summary>
        /// Обрабатывает изменение данных.
        /// </summary>
        private void HandleDataChanged()
        {
            if (!isInitialized)
            {
                return;
            }

            RefreshWeaponList();
        }

        #endregion
    }
}
*/
