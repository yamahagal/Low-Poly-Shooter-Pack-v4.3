//Copyright 2024, Infima Games. All Rights Reserved.

// Условная компиляция - скрипт будет работать только когда Emerald AI установлен
#if EMERALD_AI_PRESENT

using UnityEngine;
using EmeraldAI;

namespace InfimaGames.LowPolyShooterPack
{
    /// <summary>
    /// Пример врага с использованием Emerald AI
    /// Этот скрипт демонстрирует базовую интеграцию Emerald AI с существующей системой игры
    /// </summary>
    public class EmeraldAIEnemy : MonoBehaviour, IDamageable
    {
        [Header("Emerald AI Components")]
        [SerializeField] private EmeraldSystem emeraldAI;
        
        [Header("References")]
        [SerializeField] private Transform playerTarget;
        
        [Header("Enemy Settings")]
        [SerializeField] private float enemyHealth = 100f;
        [SerializeField] private float damage = 10f;
        [SerializeField] private float attackRange = 15f;
        [SerializeField] private float detectionRange = 20f;
        
        [Header("Debug")]
        [SerializeField] private bool debugMode = true;
        
        private void Start()
        {
            // Получаем ссылку на Emerald AI компонент
            if (emeraldAI == null)
            {
                emeraldAI = GetComponent<EmeraldSystem>();
            }
            
            // Настраиваем Emerald AI
            SetupEmeraldAI();
            
            if (debugMode)
            {
                Debug.Log($"[EmeraldAIEnemy] {gameObject.name} инициализирован с Emerald AI");
            }
        }
        
        private void SetupEmeraldAI()
        {
            // Настраиваем параметры Emerald AI
            // В реальном проекте эти параметры будут загружаться из ScriptableObjects
            
            // Устанавливаем цель
            if (playerTarget != null)
            {
                emeraldAI.CombatTarget = playerTarget;
            }
            
            // Настраиваем дальность обнаружения
            if (emeraldAI.DetectionComponent != null)
            {
                emeraldAI.DetectionComponent.DetectionRadius = (int)detectionRange;
            }
            
            // Настраиваем здоровье
            if (emeraldAI.HealthComponent != null)
            {
                emeraldAI.HealthComponent.StartingHealth = (int)enemyHealth;
                emeraldAI.HealthComponent.CurrentHealth = (int)enemyHealth;
            }
            
            // Настраиваем скорость движения через NavMeshAgent
            if (emeraldAI.m_NavMeshAgent != null)
            {
                emeraldAI.m_NavMeshAgent.speed = 3.5f;
            }
        }
        
        private void Update()
        {
            // Проверяем, жив ли враг
            if (!IsDead())
            {
                // Проверяем, видит ли игрок
                bool canSeePlayer = CheckPlayerVisibility();
                
                if (canSeePlayer)
                {
                    // Игрок виден - атакуем
                    if (debugMode)
                    {
                        Debug.Log($"[EmeraldAIEnemy] {gameObject.name} видит игрока, атакуем!");
                    }
                    
                    // Emerald AI автоматически управляет атакой
                    // Нам не нужно вызывать методы атаки напрямую
                }
                else
                {
                    // Игрок не виден - патрулируем
                    if (debugMode)
                    {
                        Debug.Log($"[EmeraldAIEnemy] {gameObject.name} не видит игрока, патрулируем");
                    }
                }
            }
        }
        
        private bool CheckPlayerVisibility()
        {
            // Проверяем видимость игрока через Emerald AI
            if (playerTarget == null || emeraldAI.DetectionComponent == null)
                return false;
            
            // Проверяем расстояние до игрока
            float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
            
            // Проверяем, есть ли препятствия
            Vector3 directionToPlayer = (playerTarget.position - transform.position).normalized;
            RaycastHit hit;
            
            if (Physics.Raycast(transform.position + Vector3.up * 1.5f, directionToPlayer, out hit, 
                distanceToPlayer, emeraldAI.DetectionComponent.ObstructionDetectionLayerMask))
            {
                // Если луч попадает в игрока, враг его видит
                if (hit.transform == playerTarget)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Проверяет, мертв ли враг
        /// </summary>
        private bool IsDead()
        {
            if (emeraldAI == null || emeraldAI.AnimationComponent == null)
                return false;
            
            return emeraldAI.AnimationComponent.IsDead;
        }
        
        /// <summary>
        /// Получает текущее здоровье врага
        /// </summary>
        private float GetCurrentHealth()
        {
            if (emeraldAI == null || emeraldAI.HealthComponent == null)
                return 0f;
            
            return emeraldAI.HealthComponent.CurrentHealth;
        }
        
        /// <summary>
        /// Получает текущее состояние врага
        /// </summary>
        private string GetCurrentState()
        {
            if (emeraldAI == null)
                return "Unknown";
            
            if (IsDead())
                return "Dead";
            
            if (emeraldAI.CombatComponent != null && emeraldAI.CombatComponent.CombatState)
                return "Attacking";
            
            if (emeraldAI.DetectionComponent != null && 
                emeraldAI.DetectionComponent.CurrentDetectionState == EmeraldDetection.DetectionStates.Alert)
                return "Alert";
            
            return "Patrolling";
        }
        
        /// <summary>
        /// Обработка получения урона от игрока (реализация IDamageable)
        /// </summary>
        public void Damage(int damageAmount, Vector3 hitPosition, Transform attacker)
        {
            // Передаём урон в Emerald AI через HealthComponent
            if (emeraldAI != null && emeraldAI.HealthComponent != null)
            {
                emeraldAI.HealthComponent.CurrentHealth -= damageAmount;
                
                if (debugMode)
                {
                    Debug.Log($"[EmeraldAIEnemy] {gameObject.name} получил урон: {damageAmount}");
                }
            }
        }
        
        /// <summary>
        /// Обработка получения урона от игрока (упрощённая версия)
        /// </summary>
        public void TakeDamage(float damageAmount, Vector3 hitPosition)
        {
            Damage((int)damageAmount, hitPosition, null);
        }
        
        /// <summary>
        /// Отладка - рисование состояния врага
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!debugMode || emeraldAI == null)
                return;
            
            // Рисуем состояние врага
            Color stateColor = IsDead() ? Color.red : 
                              GetCurrentState() == "Attacking" ? Color.yellow : Color.green;
            
            // Рисуем линию взгляда врага
            Gizmos.color = stateColor;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 3f);
            
            // Рисуем сферу обнаружения
            if (!IsDead() && emeraldAI.DetectionComponent != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, emeraldAI.DetectionComponent.DetectionRadius);
            }
            
            // Рисуем сферу атаки
            if (!IsDead())
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, attackRange);
            }
        }
        
        private void OnGUI()
        {
            if (!debugMode)
                return;
            
            // Отображаем информацию о состоянии врага
            GUILayout.Label($"Enemy: {gameObject.name}");
            GUILayout.Label($"Health: {GetCurrentHealth():F0}/{enemyHealth:F0}");
            GUILayout.Label($"State: {GetCurrentState()}");
            GUILayout.Label($"Can See Player: {CheckPlayerVisibility()}");
            
            GUILayout.Space(10);
            
            // Кнопки для тестирования
            if (GUILayout.Button("Take Damage"))
            {
                TakeDamage(20f, transform.position);
            }
            
            if (GUILayout.Button("Die"))
            {
                if (emeraldAI != null && emeraldAI.HealthComponent != null)
                {
                    emeraldAI.HealthComponent.CurrentHealth = 0;
                }
            }
        }
    }
}

#else

// Если Emerald AI не установлен, этот код не будет компилироваться
// Используйте ImprovedEnemyAI.cs вместо этого

using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    /// <summary>
    /// Emerald AI не установлен. Используйте ImprovedEnemyAI.cs вместо этого.
    /// </summary>
    public class EmeraldAIEnemy : MonoBehaviour
    {
        [Header("Notice")]
        [SerializeField] private string notice = "Emerald AI не установлен. Используйте ImprovedEnemyAI.cs вместо этого.";
        
        private void Awake()
        {
            Debug.LogWarning("[EmeraldAIEnemy] Emerald AI не установлен. Используйте ImprovedEnemyAI.cs вместо этого.");
            Debug.LogWarning("[EmeraldAIEnemy] Для использования Emerald AI установите пакет и определите символ EMERALD_AI_PRESENT в Player Settings.");
            this.enabled = false;
        }
    }
}

#endif
