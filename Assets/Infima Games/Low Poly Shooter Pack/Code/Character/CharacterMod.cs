using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    /// <summary>
    /// Character. Handles player input and movement.
    /// </summary>
    public class CharacterMod : MonoBehaviour
    {
        #region FIELDS

        [SerializeField]
        private int weaponIndexEquippedAtStart;

        [Tooltip("Inventory.")]
        [SerializeField]
        private InventoryBehaviour inventory;

        private object weaponShopFilter;

        #endregion

        #region UNITY METHODS

        private void Start()
        {
            // Ищем компонент WeaponShopFilter для фильтрации по selected через reflection
            if (weaponShopFilter == null)
            {
                weaponShopFilter = GetComponent("WeaponShopFilter");
            }
            
            if (weaponShopFilter == null)
            {
                Debug.LogWarning("WeaponShopFilter не найден. Переключение будет работать без фильтрации по selected.");
            }
            else
            {
                Debug.Log("Найден WeaponShopFilter. Переключение будет работать с фильтрацией по selected.");
            }
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Get filtered next index.
        /// </summary>
        private int GetFilteredNextIndex()
        {
            if (weaponShopFilter == null || inventory == null)
            {
                return inventory.GetNextIndex();
            }

            // Используем reflection для вызова метода GetNextIndexFiltered
            var method = weaponShopFilter.GetType().GetMethod("GetNextIndexFiltered");
            if (method != null)
            {
                return (int)method.Invoke(weaponShopFilter, null);
            }

            return inventory.GetNextIndex();
        }

        /// <summary>
        /// Get filtered last index.
        /// </summary>
        private int GetFilteredLastIndex()
        {
            if (weaponShopFilter == null || inventory == null)
            {
                return inventory.GetLastIndex();
            }

            // Используем reflection для вызова метода GetLastIndexFiltered
            var method = weaponShopFilter.GetType().GetMethod("GetLastIndexFiltered");
            if (method != null)
            {
                return (int)method.Invoke(weaponShopFilter, null);
            }

            return inventory.GetLastIndex();
        }

        #endregion
    }
}
