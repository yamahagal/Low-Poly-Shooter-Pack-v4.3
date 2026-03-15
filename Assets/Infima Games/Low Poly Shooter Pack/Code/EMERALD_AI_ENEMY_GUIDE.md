# Emerald AI Enemy Guide

## Обзор

Этот документ содержит примеры использования Emerald AI для создания врагов в Unity с интеграцией в Low Poly Shooter Pack.

## Предварительные требования

1. Установленный Emerald AI пакет
2. Low Poly Shooter Pack
3. NavMesh на сцене
4. Префаб игрока с тегом "Player"

## Создание базового врага

### Шаг 1: Создайте префаб врага

1. Создайте новый GameObject в сцене
2. Добавьте компонент `EmeraldSystem`
3. Emerald AI автоматически добавит все необходимые компоненты:
   - EmeraldAnimation
   - EmeraldDetection
   - EmeraldCombat
   - EmeraldBehaviors
   - EmeraldMovement
   - EmeraldHealth
   - EmeraldSounds
   - NavMeshAgent
   - BoxCollider
   - Animator
   - AudioSource

### Шаг 2: Добавьте скрипт EmeraldAIEnemy

Скопируйте файл [`EmeraldAIEnemy.cs`](Assets/Infima Games/Low Poly Shooter Pack/Code/EmeraldAIEnemy.cs:1) в ваш проект и добавьте его к префабу врага.

### Шаг 3: Настройте Inspector

**EmeraldAIEnemy:**
- **Emerald AI**: перетащите объект с EmeraldSystem (обычно это тот же объект)
- **Player Target**: перетащите префаб игрока
- **Enemy Health**: 100
- **Damage**: 10
- **Attack Range**: 15
- **Detection Range**: 20
- **Debug Mode**: true (для отладки)

**EmeraldDetection:**
- **Detection Radius**: 20
- **Field Of View Angle**: 270
- **Player Tag**: "Player"

**EmeraldHealth:**
- **Starting Health**: 100
- **Current Health**: 100

**NavMeshAgent:**
- **Speed**: 3.5
- **Acceleration**: 8
- **Angular Speed**: 120

### Шаг 4: Создайте NavMesh

1. Window > AI > Navigation
2. Выберите статические объекты сцены (пол, стены и т.д.)
3. Нажмите "Bake"
4. Убедитесь, что враг находится на NavMesh

### Шаг 5: Сохраните как префаб

1. Перетащите врага из сцены в папку Assets
2. Удалите врага из сцены

## Создание разных типов врагов

### Разведчик (Scout)

**Характеристики:**
- Высокая скорость
- Низкое здоровье
- Большой радиус обнаружения
- Быстрая реакция

**Настройки:**
```csharp
// В Inspector или через код:
emeraldAI.m_NavMeshAgent.speed = 5f;
emeraldAI.HealthComponent.StartingHealth = 50;
emeraldAI.DetectionComponent.DetectionRadius = 30;
emeraldAI.DetectionComponent.FieldOfViewAngle = 300;
```

**Inspector настройки:**
- **EmeraldAIEnemy**:
  - Enemy Health: 50
  - Detection Range: 30
- **EmeraldDetection**:
  - Detection Radius: 30
  - Field Of View Angle: 300
- **EmeraldHealth**:
  - Starting Health: 50
- **NavMeshAgent**:
  - Speed: 5
  - Acceleration: 10

### Штурмовик (Assault)

**Характеристики:**
- Средняя скорость
- Среднее здоровье
- Средний радиус обнаружения
- Балансированный боец

**Настройки:**
```csharp
emeraldAI.m_NavMeshAgent.speed = 3.5f;
emeraldAI.HealthComponent.StartingHealth = 100;
emeraldAI.DetectionComponent.DetectionRadius = 20;
emeraldAI.DetectionComponent.FieldOfViewAngle = 270;
```

**Inspector настройки:**
- **EmeraldAIEnemy**:
  - Enemy Health: 100
  - Detection Range: 20
- **EmeraldDetection**:
  - Detection Radius: 20
  - Field Of View Angle: 270
- **EmeraldHealth**:
  - Starting Health: 100
- **NavMeshAgent**:
  - Speed: 3.5
  - Acceleration: 8

### Снайпер (Sniper)

**Характеристики:**
- Низкая скорость
- Высокое здоровье
- Большой радиус обнаружения
- Дальний радиус атаки

**Настройки:**
```csharp
emeraldAI.m_NavMeshAgent.speed = 2f;
emeraldAI.HealthComponent.StartingHealth = 150;
emeraldAI.DetectionComponent.DetectionRadius = 40;
emeraldAI.DetectionComponent.FieldOfViewAngle = 90;
```

**Inspector настройки:**
- **EmeraldAIEnemy**:
  - Enemy Health: 150
  - Detection Range: 40
  - Attack Range: 30
- **EmeraldDetection**:
  - Detection Radius: 40
  - Field Of View Angle: 90
- **EmeraldHealth**:
  - Starting Health: 150
- **NavMeshAgent**:
  - Speed: 2
  - Acceleration: 5

### Поддержка (Support)

**Характеристики:**
- Низкая скорость
- Высокое здоровье
- Малый радиус обнаружения
- Высокий урон

**Настройки:**
```csharp
emeraldAI.m_NavMeshAgent.speed = 2.5f;
emeraldAI.HealthComponent.StartingHealth = 200;
emeraldAI.DetectionComponent.DetectionRadius = 15;
emeraldAI.DetectionComponent.FieldOfViewAngle = 180;
```

**Inspector настройки:**
- **EmeraldAIEnemy**:
  - Enemy Health: 200
  - Damage: 20
  - Detection Range: 15
- **EmeraldDetection**:
  - Detection Radius: 15
  - Field Of View Angle: 180
- **EmeraldHealth**:
  - Starting Health: 200
- **NavMeshAgent**:
  - Speed: 2.5
  - Acceleration: 6

## Программная настройка врага

### Создание скрипта для разных типов врагов

```csharp
using UnityEngine;
using EmeraldAI;

namespace InfimaGames.LowPolyShooterPack
{
    public class EnemyTypeConfigurator : MonoBehaviour
    {
        public enum EnemyType
        {
            Scout,
            Assault,
            Sniper,
            Support
        }
        
        [Header("Enemy Type")]
        [SerializeField] private EnemyType enemyType;
        
        [Header("References")]
        [SerializeField] private EmeraldSystem emeraldAI;
        [SerializeField] private EmeraldAIEnemy enemyController;
        
        private void Start()
        {
            ConfigureEnemy();
        }
        
        private void ConfigureEnemy()
        {
            if (emeraldAI == null || enemyController == null)
            {
                Debug.LogError("EmeraldAI or EnemyController not assigned!");
                return;
            }
            
            switch (enemyType)
            {
                case EnemyType.Scout:
                    ConfigureScout();
                    break;
                case EnemyType.Assault:
                    ConfigureAssault();
                    break;
                case EnemyType.Sniper:
                    ConfigureSniper();
                    break;
                case EnemyType.Support:
                    ConfigureSupport();
                    break;
            }
        }
        
        private void ConfigureScout()
        {
            // Высокая скорость
            emeraldAI.m_NavMeshAgent.speed = 5f;
            emeraldAI.m_NavMeshAgent.acceleration = 10f;
            
            // Низкое здоровье
            emeraldAI.HealthComponent.StartingHealth = 50;
            emeraldAI.HealthComponent.CurrentHealth = 50;
            
            // Большой радиус обнаружения
            emeraldAI.DetectionComponent.DetectionRadius = 30;
            emeraldAI.DetectionComponent.FieldOfViewAngle = 300;
            
            Debug.Log($"{gameObject.name} configured as Scout");
        }
        
        private void ConfigureAssault()
        {
            // Средняя скорость
            emeraldAI.m_NavMeshAgent.speed = 3.5f;
            emeraldAI.m_NavMeshAgent.acceleration = 8f;
            
            // Среднее здоровье
            emeraldAI.HealthComponent.StartingHealth = 100;
            emeraldAI.HealthComponent.CurrentHealth = 100;
            
            // Средний радиус обнаружения
            emeraldAI.DetectionComponent.DetectionRadius = 20;
            emeraldAI.DetectionComponent.FieldOfViewAngle = 270;
            
            Debug.Log($"{gameObject.name} configured as Assault");
        }
        
        private void ConfigureSniper()
        {
            // Низкая скорость
            emeraldAI.m_NavMeshAgent.speed = 2f;
            emeraldAI.m_NavMeshAgent.acceleration = 5f;
            
            // Высокое здоровье
            emeraldAI.HealthComponent.StartingHealth = 150;
            emeraldAI.HealthComponent.CurrentHealth = 150;
            
            // Большой радиус обнаружения
            emeraldAI.DetectionComponent.DetectionRadius = 40;
            emeraldAI.DetectionComponent.FieldOfViewAngle = 90;
            
            Debug.Log($"{gameObject.name} configured as Sniper");
        }
        
        private void ConfigureSupport()
        {
            // Низкая скорость
            emeraldAI.m_NavMeshAgent.speed = 2.5f;
            emeraldAI.m_NavMeshAgent.acceleration = 6f;
            
            // Высокое здоровье
            emeraldAI.HealthComponent.StartingHealth = 200;
            emeraldAI.HealthComponent.CurrentHealth = 200;
            
            // Малый радиус обнаружения
            emeraldAI.DetectionComponent.DetectionRadius = 15;
            emeraldAI.DetectionComponent.FieldOfViewAngle = 180;
            
            Debug.Log($"{gameObject.name} configured as Support");
        }
    }
}
```

## Интеграция с системой оружия

### Получение урона от оружия

Скрипт [`EmeraldAIEnemy.cs`](Assets/Infima Games/Low Poly Shooter Pack/Code/EmeraldAIEnemy.cs:1) реализует интерфейс `IDamageable`, поэтому он автоматически работает с системой оружия Low Poly Shooter Pack.

```csharp
public void Damage(int damageAmount, Vector3 hitPosition, Transform attacker)
{
    if (emeraldAI != null && emeraldAI.HealthComponent != null)
    {
        emeraldAI.HealthComponent.CurrentHealth -= damageAmount;
        
        if (debugMode)
        {
            Debug.Log($"[EmeraldAIEnemy] {gameObject.name} получил урон: {damageAmount}");
        }
    }
}
```

### Проверка смерти

Emerald AI автоматически обрабатывает смерть через компонент `EmeraldAnimation`:

```csharp
private bool IsDead()
{
    if (emeraldAI == null || emeraldAI.AnimationComponent == null)
        return false;
    
    return emeraldAI.AnimationComponent.IsDead;
}
```

## Спавн врагов

### Простой спавнер

```csharp
using UnityEngine;
using EmeraldAI;

namespace InfimaGames.LowPolyShooterPack
{
    public class EmeraldEnemySpawner : MonoBehaviour
    {
        [Header("Enemy Prefab")]
        [SerializeField] private GameObject enemyPrefab;
        
        [Header("Spawn Points")]
        [SerializeField] private Transform[] spawnPoints;
        
        [Header("Spawn Settings")]
        [SerializeField] private int maxEnemies = 5;
        [SerializeField] private float spawnInterval = 10f;
        [SerializeField] private bool autoSpawn = true;
        
        private int currentEnemyCount = 0;
        
        private void Start()
        {
            if (autoSpawn)
            {
                StartCoroutine(SpawnRoutine());
            }
        }
        
        private IEnumerator SpawnRoutine()
        {
            while (true)
            {
                if (currentEnemyCount < maxEnemies)
                {
                    SpawnEnemy();
                }
                
                yield return new WaitForSeconds(spawnInterval);
            }
        }
        
        private void SpawnEnemy()
        {
            // Выбираем случайную точку спавна
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            
            // Создаём врага
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            
            // Получаем ссылку на EmeraldAIEnemy
            EmeraldAIEnemy enemyController = enemy.GetComponent<EmeraldAIEnemy>();
            
            // Настраиваем игрока как цель
            if (enemyController != null)
            {
                // Здесь можно найти игрока по тегу или другим способом
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    enemyController.SetPlayerTarget(player.transform);
                }
            }
            
            currentEnemyCount++;
            
            Debug.Log($"Spawned enemy at {spawnPoint.position}. Total enemies: {currentEnemyCount}");
        }
        
        public void OnEnemyDeath()
        {
            currentEnemyCount--;
            Debug.Log($"Enemy died. Remaining enemies: {currentEnemyCount}");
        }
    }
}
```

## События Emerald AI

### Подписка на события

```csharp
private void SubscribeToEvents()
{
    if (emeraldAI != null && emeraldAI.HealthComponent != null)
    {
        // Событие при получении урона
        emeraldAI.HealthComponent.OnTakeDamage += OnTakeDamage;
        
        // Событие при смерти
        emeraldAI.HealthComponent.OnDeath += OnDeath;
    }
}

private void OnTakeDamage()
{
    Debug.Log($"{gameObject.name} получил урон!");
    // Здесь можно добавить звук, эффект и т.д.
}

private void OnDeath()
{
    Debug.Log($"{gameObject.name} умер!");
    // Здесь можно добавить лут, опыт и т.д.
}

private void OnDestroy()
{
    // Отписываемся от событий
    if (emeraldAI != null && emeraldAI.HealthComponent != null)
    {
        emeraldAI.HealthComponent.OnTakeDamage -= OnTakeDamage;
        emeraldAI.HealthComponent.OnDeath -= OnDeath;
    }
}
```

## Отладка

### Включение отладки

В Inspector включите `Debug Mode` в компоненте `EmeraldAIEnemy` для просмотра информации:

- Текущее здоровье
- Состояние врага
- Видимость игрока

### Gizmos

В Scene View вы увидите:

- **Жёлтая сфера**: радиус обнаружения
- **Красная сфера**: радиус атаки
- **Линия**: направление взгляда врага
- **Цвет линии**: зависит от состояния
  - Зелёный: патрулирование
  - Жёлтый: атака
  - Красный: смерть

### Кастомная отладка

```csharp
private void OnGUI()
{
    if (!debugMode)
        return;
    
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
```

## Советы и лучшие практики

1. **Используйте ScriptableObjects** для хранения конфигураций разных типов врагов
2. **Создайте базовый класс** для всех врагов с Emerald AI
3. **Используйте события** Emerald AI для интеграции с другими системами
4. **Тестируйте на NavMesh**, убедитесь, что враги могут перемещаться
5. **Настройте слои** правильно для системы обнаружения
6. **Используйте теги** для идентификации игрока и целей

## Устранение проблем

### Враг не двигается
- Проверьте наличие NavMesh на сцене
- Убедитесь, что NavMeshAgent настроен правильно
- Проверьте, что враг находится на NavMesh слое

### Враг не обнаруживает игрока
- Проверьте тег игрока (должен быть "Player")
- Проверьте DetectionRadius и FieldOfViewAngle
- Убедитесь, что слои настроены правильно

### Враг не атакует
- Проверьте, что CombatActions настроены
- Убедитесь, что AttackClass содержит атаки
- Проверьте, что цель установлена (CombatTarget)

### Враг не получает урон
- Убедитесь, что класс реализует IDamageable
- Проверьте, что метод Damage() вызывает HealthComponent.CurrentHealth
- Убедитесь, что оружие игрока вызывает Damage() на правильном объекте

## Дополнительные ресурсы

- [Подробное руководство по интеграции](Assets/Data/EMERALD_AI_INTEGRATION_GUIDE.md)
- [Краткое руководство](Assets/Data/README_EMERALD_AI.md)
- [Анализ и улучшения Enemy AI](plans/ENEMY_AI_ANALYSIS_AND_IMPROVEMENTS.md)
- [Emerald AI Documentation](https://black-horizon-studios.gitbook.io/emerald-ai-wiki/)
