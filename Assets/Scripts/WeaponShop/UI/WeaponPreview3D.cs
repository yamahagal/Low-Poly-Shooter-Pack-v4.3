/*
// ВРЕМЕННО ЗАКОММЕНТИРОВАНО - ФУНКЦИОНАЛЬНОСТЬ МАГАЗИНА ОТКЛЮЧЕНА
// Этот файл содержит 3D предпросмотр оружия для магазина

using UnityEngine;
using WeaponShop;

namespace WeaponShop
{
    /// <summary>
    /// Компонент для 3D предпросмотра оружия в магазине.
    /// </summary>
    public class WeaponPreview3D : MonoBehaviour
    {
        #region FIELDS

        [Header("Preview Setup")]
        [Tooltip("Точка спавна оружия.")]
        [SerializeField]
        private Transform previewSpawnPoint;

        [Tooltip("Камера для предпросмотра.")]
        [SerializeField]
        private Camera previewCamera;

        [Header("Rotation Settings")]
        [Tooltip("Автоматическое вращение.")]
        [SerializeField]
        private bool autoRotate = true;

        [Tooltip("Скорость вращения.")]
        [SerializeField]
        [Range(10f, 360f)]
        private float rotationSpeed = 30f;

        [Header("Lighting")]
        [Tooltip("Направленный свет.")]
        [SerializeField]
        private Light directionalLight;

        [Tooltip("Интенсивность света.")]
        [SerializeField]
        [Range(0f, 2f)]
        private float lightIntensity = 1f;

        #endregion

        #region FIELDS PRIVATE

        private GameObject currentPreview;
        private WeaponData currentWeaponData;
        private IWeaponShopService weaponShopService;

        #endregion

        #region UNITY METHODS

        private void Awake()
        {
            Initialize();
        }

        private void Update()
        {
            HandleRotation();
        }

        private void OnDestroy()
        {
            Cleanup();
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Показывает оружие в предпросмотре.
        /// </summary>
        /// <param name="weaponData">Данные оружия.</param>
        public void ShowWeapon(WeaponData weaponData)
        {
            if (weaponData == null)
            {
                Debug.LogError("Данные оружия не могут быть null.");
                return;
            }

            ClearPreview();

            currentWeaponData = weaponData;

            if (weaponData.WeaponPrefab != null && previewSpawnPoint != null)
            {
                currentPreview = Instantiate(weaponData.WeaponPrefab, previewSpawnPoint);
                currentPreview.transform.localPosition = Vector3.zero;
                currentPreview.transform.localRotation = Quaternion.identity;

                // Настраиваем слой рендеринга для предпросмотра
                SetPreviewLayer(currentPreview);
            }
        }

        /// <summary>
        /// Обновляет предпросмотр.
        /// </summary>
        public void RefreshPreview()
        {
            if (currentWeaponData != null)
            {
                ShowWeapon(currentWeaponData);
            }
        }

        /// <summary>
        /// Очищает предпросмотр.
        /// </summary>
        public void ClearPreview()
        {
            if (currentPreview != null)
            {
                Destroy(currentPreview);
                currentPreview = null;
            }

            currentWeaponData = null;
        }

        /// <summary>
        /// Устанавливает автоматическое вращение.
        /// </summary>
        /// <param name="enabled">Включено ли вращение.</param>
        public void SetAutoRotate(bool enabled)
        {
            autoRotate = enabled;
        }

        /// <summary>
        /// Устанавливает скорость вращения.
        /// </summary>
        /// <param name="speed">Скорость вращения.</param>
        public void SetRotationSpeed(float speed)
        {
            rotationSpeed = Mathf.Clamp(speed, 10f, 360f);
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
            }
            else
            {
                weaponShopService.OnWeaponSelected += HandleWeaponSelected;
            }

            // Настраиваем освещение
            if (directionalLight != null)
            {
                directionalLight.intensity = lightIntensity;
            }
        }

        /// <summary>
        /// Очищает подписки.
        /// </summary>
        private void Cleanup()
        {
            ClearPreview();

            if (weaponShopService != null)
            {
                weaponShopService.OnWeaponSelected -= HandleWeaponSelected;
            }
        }

        /// <summary>
        /// Устанавливает слой рендеринга для предпросмотра.
        /// </summary>
        private void SetPreviewLayer(GameObject obj)
        {
            if (previewCamera != null)
            {
                int previewLayer = LayerMask.NameToLayer("UI");
                SetLayerRecursive(obj, previewLayer);
            }
        }

        /// <summary>
        /// Рекурсивно устанавливает слой для всех дочерних объектов.
        /// </summary>
        private void SetLayerRecursive(GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach (Transform child in obj.transform)
            {
                SetLayerRecursive(child.gameObject, layer);
            }
        }

        /// <summary>
        /// Обрабатывает вращение.
        /// </summary>
        private void HandleRotation()
        {
            if (currentPreview != null && autoRotate)
            {
                currentPreview.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
            }
        }

        #endregion

        #region EVENT HANDLERS

        /// <summary>
        /// Обрабатывает выбор оружия.
        /// </summary>
        private void HandleWeaponSelected(string weaponId)
        {
            if (weaponShopService != null)
            {
                WeaponData weaponData = weaponShopService.GetWeaponData(weaponId);
                ShowWeapon(weaponData);
            }
        }

        #endregion
    }
}
*/
