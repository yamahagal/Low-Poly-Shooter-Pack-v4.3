using UnityEngine;

namespace WeaponShop
{
    /// <summary>
    /// ScriptableObject для хранения данных об оружии.
    /// </summary>
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapon Shop/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("Basic Info")]
        [Tooltip("Уникальный идентификатор оружия.")]
        [SerializeField]
        private string weaponId;

        [Tooltip("Название оружия.")]
        [SerializeField]
        private string weaponName;

        [Tooltip("Тип оружия.")]
        [SerializeField]
        private WeaponType weaponType;

        [Tooltip("Префаб оружия.")]
        [SerializeField]
        private GameObject weaponPrefab;

        [Tooltip("Изображение оружия для UI.")]
        [SerializeField]
        private Sprite weaponIcon;

        [Tooltip("Базовая стоимость оружия.")]
        [SerializeField]
        private int baseCost;

        [Header("Stats")]
        [Tooltip("Урон оружия.")]
        [Range(0f, 500f)]
        [SerializeField]
        private float damage = 100f;

        [Tooltip("Скорострельность (выстрелов в минуту).")]
        [Range(10, 1200)]
        [SerializeField]
        private int fireRate = 200;

        [Tooltip("Ёмкость магазина.")]
        [Range(1, 100)]
        [SerializeField]
        private int magazineCapacity = 30;

        [Tooltip("Время перезарядки (в секундах).")]
        [Range(0.5f, 10f)]
        [SerializeField]
        private float reloadTime = 2.5f;

        [Tooltip("Точность (меньше = точнее).")]
        [Range(0f, 1f)]
        [SerializeField]
        private float accuracy = 0.25f;

        [Tooltip("Дальность стрельбы.")]
        [Range(10f, 500f)]
        [SerializeField]
        private float range = 100f;

        #region PROPERTIES

        /// <summary>
        /// Уникальный идентификатор оружия.
        /// </summary>
        public string WeaponId => weaponId;

        /// <summary>
        /// Название оружия.
        /// </summary>
        public string WeaponName => weaponName;

        /// <summary>
        /// Тип оружия.
        /// </summary>
        public WeaponType WeaponType => weaponType;

        /// <summary>
        /// Префаб оружия.
        /// </summary>
        public GameObject WeaponPrefab => weaponPrefab;

        /// <summary>
        /// Изображение оружия для UI.
        /// </summary>
        public Sprite WeaponIcon => weaponIcon;

        /// <summary>
        /// Базовая стоимость оружия.
        /// </summary>
        public int BaseCost => baseCost;

        /// <summary>
        /// Урон оружия.
        /// </summary>
        public float Damage => damage;

        /// <summary>
        /// Скорострельность (выстрелов в минуту).
        /// </summary>
        public int FireRate => fireRate;

        /// <summary>
        /// Ёмкость магазина.
        /// </summary>
        public int MagazineCapacity => magazineCapacity;

        /// <summary>
        /// Время перезарядки (в секундах).
        /// </summary>
        public float ReloadTime => reloadTime;

        /// <summary>
        /// Точность (меньше = точнее).
        /// </summary>
        public float Accuracy => accuracy;

        /// <summary>
        /// Дальность стрельбы.
        /// </summary>
        public float Range => range;

        #endregion
    }

    /// <summary>
    /// Перечисление типов оружия.
    /// </summary>
    public enum WeaponType
    {
        Handgun,
        SMG,
        Shotgun,
        AssaultRifle,
        Sniper,
        RocketLauncher,
        GrenadeLauncher
    }
}
