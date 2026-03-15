//Copyright 2024, Infima Games. All Rights Reserved.

using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace InfimaGames.LowPolyShooterPack
{
    /// <summary>
    /// Улучшенный ИИ врага с машиной состояний
    /// Работает с существующей системой NavMesh и не требует Emerald AI
    /// </summary>
    public class ImprovedEnemyAI : MonoBehaviour
    {
        [Header("Navigation")]
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField] private Transform playerTarget;
        
        [Header("Combat Settings")]
        [SerializeField] private float enemyHealth = 100f;
        [SerializeField] private float damage = 10f;
        [SerializeField] private float attackRange = 15f;
        [SerializeField] private float detectionRange = 20f;
        [SerializeField] private float attackCooldown = 1.5f;
        [SerializeField] private int attackDamage = 10;
        
        [Header("AI Settings")]
        [SerializeField] private float patrolSpeed = 2f;
        [SerializeField] private float chaseSpeed = 4f;
        [SerializeField] private float fieldOfViewAngle = 120f;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private LayerMask obstructionLayer;
        
        [Header("Debug")]
        [SerializeField] private bool debugMode = true;
        [SerializeField] private bool showGizmos = true;
        
        // Состояния ИИ
        public enum AIState
        {
            Idle,
            Patrolling,
            Chasing,
            Attacking,
            Reloading,
            Investigating,
            Fleeing,
            Dead
        }
        
        private AIState currentState = AIState.Idle;
        private int currentPatrolIndex = 0;
        private float currentHealth;
        private float lastAttackTime;
        private Vector3 lastKnownPlayerPosition;
        private Animator animator;
        private bool isDead = false;
        
        private void Start()
        {
            InitializeComponents();
            InitializeAI();
        }
        
        private void InitializeComponents()
        {
            // Получаем или добавляем NavMeshAgent
            if (navMeshAgent == null)
            {
                navMeshAgent = GetComponent<NavMeshAgent>();
                if (navMeshAgent == null)
                {
                    navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
                }
            }
            
            // Настраиваем NavMeshAgent
            navMeshAgent.speed = patrolSpeed;
            navMeshAgent.angularSpeed = 120f;
            navMeshAgent.acceleration = 8f;
            navMeshAgent.autoBraking = true;
            
            // Получаем Animator
            animator = GetComponent<Animator>();
            
            // Инициализируем здоровье
            currentHealth = enemyHealth;
            
            // Находим игрока если не задан
            if (playerTarget == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerTarget = player.transform;
                }
            }
        }
        
        private void InitializeAI()
        {
            // Начинаем с патрулирования
            if (patrolPoints != null && patrolPoints.Length > 0)
            {
                currentState = AIState.Patrolling;
                SetDestination(patrolPoints[currentPatrolIndex].position);
            }
            else
            {
                currentState = AIState.Idle;
            }
            
            if (debugMode)
            {
                Debug.Log($"[ImprovedEnemyAI] {gameObject.name} инициализирован. Начальное состояние: {currentState}");
            }
        }
        
        private void Update()
        {
            if (isDead)
                return;
            
            UpdateAIState();
            UpdateAnimations();
        }
        
        private void UpdateAIState()
        {
            switch (currentState)
            {
                case AIState.Idle:
                    UpdateIdleState();
                    break;
                case AIState.Patrolling:
                    UpdatePatrollingState();
                    break;
                case AIState.Chasing:
                    UpdateChasingState();
                    break;
                case AIState.Attacking:
                    UpdateAttackingState();
                    break;
                case AIState.Investigating:
                    UpdateInvestigatingState();
                    break;
                case AIState.Fleeing:
                    UpdateFleeingState();
                    break;
            }
        }
        
        private void UpdateIdleState()
        {
            // Проверяем, видит ли враг игрока
            if (CanSeePlayer())
            {
                TransitionToState(AIState.Chasing);
            }
            else if (patrolPoints != null && patrolPoints.Length > 0)
            {
                TransitionToState(AIState.Patrolling);
            }
        }
        
        private void UpdatePatrollingState()
        {
            // Проверяем, видит ли враг игрока
            if (CanSeePlayer())
            {
                TransitionToState(AIState.Chasing);
                return;
            }
            
            // Проверяем, достиг ли враг точки патруля
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
            {
                // Переходим к следующей точке
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                SetDestination(patrolPoints[currentPatrolIndex].position);
                
                if (debugMode)
                {
                    Debug.Log($"[ImprovedEnemyAI] {gameObject.name} переходит к точке патруля {currentPatrolIndex}");
                }
            }
        }
        
        private void UpdateChasingState()
        {
            if (playerTarget == null)
            {
                TransitionToState(AIState.Investigating);
                return;
            }
            
            // Проверяем, видит ли враг игрока
            if (!CanSeePlayer())
            {
                // Запоминаем последнюю позицию игрока
                lastKnownPlayerPosition = playerTarget.position;
                TransitionToState(AIState.Investigating);
                return;
            }
            
            // Проверяем расстояние до игрока
            float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
            
            if (distanceToPlayer <= attackRange)
            {
                TransitionToState(AIState.Attacking);
            }
            else
            {
                // Преследуем игрока
                SetDestination(playerTarget.position);
                navMeshAgent.speed = chaseSpeed;
            }
        }
        
        private void UpdateAttackingState()
        {
            if (playerTarget == null)
            {
                TransitionToState(AIState.Investigating);
                return;
            }
            
            // Проверяем, видит ли враг игрока
            if (!CanSeePlayer())
            {
                lastKnownPlayerPosition = playerTarget.position;
                TransitionToState(AIState.Investigating);
                return;
            }
            
            // Проверяем расстояние до игрока
            float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
            
            if (distanceToPlayer > attackRange)
            {
                TransitionToState(AIState.Chasing);
                return;
            }
            
            // Останавливаемся и поворачиваемся к игроку
            navMeshAgent.isStopped = true;
            RotateTowardsTarget(playerTarget.position);
            
            // Атакуем если прошло время перезарядки
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                Attack();
            }
        }
        
        private void UpdateInvestigatingState()
        {
            // Двигаемся к последней известной позиции игрока
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
            {
                // Если достигли позиции, возвращаемся к патрулированию
                TransitionToState(AIState.Patrolling);
            }
            
            // Проверяем, видит ли враг игрока во время расследования
            if (CanSeePlayer())
            {
                TransitionToState(AIState.Chasing);
            }
        }
        
        private void UpdateFleeingState()
        {
            // Убегаем от игрока
            if (playerTarget != null)
            {
                Vector3 fleeDirection = (transform.position - playerTarget.position).normalized;
                Vector3 fleePosition = transform.position + fleeDirection * 10f;
                SetDestination(fleePosition);
                navMeshAgent.speed = chaseSpeed * 1.2f;
            }
            
            // Проверяем, достаточно ли далеко убежали
            if (playerTarget == null || Vector3.Distance(transform.position, playerTarget.position) > detectionRange * 1.5f)
            {
                TransitionToState(AIState.Patrolling);
            }
        }
        
        private void TransitionToState(AIState newState)
        {
            if (currentState == newState)
                return;
            
            if (debugMode)
            {
                Debug.Log($"[ImprovedEnemyAI] {gameObject.name} переходит из {currentState} в {newState}");
            }
            
            // Выполняем действия при выходе из состояния
            OnExitState(currentState);
            
            // Обновляем состояние
            currentState = newState;
            
            // Выполняем действия при входе в состояние
            OnEnterState(newState);
        }
        
        private void OnExitState(AIState state)
        {
            switch (state)
            {
                case AIState.Attacking:
                    navMeshAgent.isStopped = false;
                    break;
            }
        }
        
        private void OnEnterState(AIState state)
        {
            switch (state)
            {
                case AIState.Patrolling:
                    navMeshAgent.speed = patrolSpeed;
                    if (patrolPoints != null && patrolPoints.Length > 0)
                    {
                        SetDestination(patrolPoints[currentPatrolIndex].position);
                    }
                    break;
                case AIState.Chasing:
                    navMeshAgent.speed = chaseSpeed;
                    break;
                case AIState.Investigating:
                    navMeshAgent.speed = patrolSpeed;
                    SetDestination(lastKnownPlayerPosition);
                    break;
            }
        }
        
        private void SetDestination(Vector3 destination)
        {
            if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.SetDestination(destination);
            }
        }
        
        private void RotateTowardsTarget(Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            direction.y = 0;
            
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
        
        private bool CanSeePlayer()
        {
            if (playerTarget == null)
                return false;
            
            // Проверяем расстояние
            float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
            if (distanceToPlayer > detectionRange)
                return false;
            
            // Проверяем угол обзора
            Vector3 directionToPlayer = (playerTarget.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleToPlayer > fieldOfViewAngle / 2f)
                return false;
            
            // Проверяем препятствия
            RaycastHit hit;
            Vector3 rayOrigin = transform.position + Vector3.up * 1.5f;
            Vector3 rayDirection = (playerTarget.position - rayOrigin).normalized;
            
            if (Physics.Raycast(rayOrigin, rayDirection, out hit, distanceToPlayer, obstructionLayer))
            {
                if (hit.transform != playerTarget)
                    return false;
            }
            
            return true;
        }
        
        private void Attack()
        {
            lastAttackTime = Time.time;
            
            // Поворачиваемся к игроку
            if (playerTarget != null)
            {
                RotateTowardsTarget(playerTarget.position);
                
                // Наносим урон игроку
                // Здесь можно добавить вызов метода получения урона на игроке
                // Например: playerTarget.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
                
                if (debugMode)
                {
                    Debug.Log($"[ImprovedEnemyAI] {gameObject.name} атакует игрока! Урон: {attackDamage}");
                }
                
                // Запускаем анимацию атаки
                if (animator != null)
                {
                    animator.SetTrigger("Attack");
                }
            }
        }
        
        private void UpdateAnimations()
        {
            if (animator == null)
                return;
            
            // Обновляем параметры анимации
            bool isMoving = navMeshAgent.velocity.magnitude > 0.1f;
            animator.SetBool("IsMoving", isMoving);
            animator.SetBool("IsAttacking", currentState == AIState.Attacking);
            animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
        }
        
        /// <summary>
        /// Получение урона
        /// </summary>
        public void TakeDamage(float damageAmount, Vector3 hitPosition)
        {
            if (isDead)
                return;
            
            currentHealth -= damageAmount;
            
            if (debugMode)
            {
                Debug.Log($"[ImprovedEnemyAI] {gameObject.name} получил урон: {damageAmount}. Текущее здоровье: {currentHealth}");
            }
            
            // Запускаем анимацию получения урона
            if (animator != null)
            {
                animator.SetTrigger("Hit");
            }
            
            // Проверяем смерть
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        /// <summary>
        /// Смерть врага
        /// </summary>
        private void Die()
        {
            isDead = true;
            currentState = AIState.Dead;
            
            // Останавливаем NavMeshAgent
            if (navMeshAgent != null)
            {
                navMeshAgent.isStopped = true;
                navMeshAgent.enabled = false;
            }
            
            // Запускаем анимацию смерти
            if (animator != null)
            {
                animator.SetTrigger("Die");
            }
            
            if (debugMode)
            {
                Debug.Log($"[ImprovedEnemyAI] {gameObject.name} умер");
            }
            
            // Удаляем врага через 3 секунды
            Destroy(gameObject, 3f);
        }
        
        /// <summary>
        /// Отладка - рисование Gizmos
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!showGizmos)
                return;
            
            // Рисуем сферу обнаружения
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
            
            // Рисуем сферу атаки
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            
            // Рисуем направление взгляда
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
            
            // Рисуем точки патруля
            if (patrolPoints != null)
            {
                Gizmos.color = Color.green;
                for (int i = 0; i < patrolPoints.Length; i++)
                {
                    if (patrolPoints[i] != null)
                    {
                        Gizmos.DrawWireSphere(patrolPoints[i].position, 0.5f);
                        
                        // Рисуем линии между точками патруля
                        if (i < patrolPoints.Length - 1 && patrolPoints[i + 1] != null)
                        {
                            Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                        }
                        else if (patrolPoints.Length > 1 && patrolPoints[0] != null)
                        {
                            Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[0].position);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Отладка - OnGUI
        /// </summary>
        private void OnGUI()
        {
            if (!debugMode)
                return;
            
            // Отображаем информацию о состоянии врага
            GUILayout.Label($"Enemy: {gameObject.name}");
            GUILayout.Label($"Health: {currentHealth:F0}/{enemyHealth:F0}");
            GUILayout.Label($"State: {currentState}");
            GUILayout.Label($"Can See Player: {CanSeePlayer()}");
            
            if (playerTarget != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
                GUILayout.Label($"Distance to Player: {distanceToPlayer:F1}m");
            }
            
            GUILayout.Space(10);
            
            // Кнопки для тестирования
            if (GUILayout.Button("Take Damage"))
            {
                TakeDamage(20f, transform.position);
            }
            
            if (GUILayout.Button("Die"))
            {
                Die();
            }
            
            if (GUILayout.Button("Toggle Debug"))
            {
                debugMode = !debugMode;
            }
        }
        
        // Публичные методы для внешнего управления
        
        public void SetPlayerTarget(Transform target)
        {
            playerTarget = target;
        }
        
        public void SetPatrolPoints(Transform[] points)
        {
            patrolPoints = points;
        }
        
        public AIState GetCurrentState()
        {
            return currentState;
        }
        
        public float GetCurrentHealth()
        {
            return currentHealth;
        }
        
        public bool IsDead()
        {
            return isDead;
        }
    }
}
