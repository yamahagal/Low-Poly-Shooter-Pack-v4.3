# Improved Enemy AI - Улучшенный ИИ врага

## Обзор

[`ImprovedEnemyAI.cs`](Assets/Infima Games/Low Poly Shooter Pack/Code/ImprovedEnemyAI.cs:1) - это улучшенный ИИ врага с машиной состояний, который работает с существующей системой NavMesh и не требует Emerald AI.

## Особенности

✅ **Машина состояний** - 8 состояний для реалистичного поведения:
- Idle (Ожидание)
- Patrolling (Патрулирование)
- Chasing (Преследование)
- Attacking (Атака)
- Reloading (Перезарядка)
- Investigating (Расследование)
- Fleeing (Бегство)
- Dead (Смерть)

✅ **Интеллектуальное обнаружение** - проверка расстояния, угла обзора и препятствий

✅ **Плавные переходы** между состояниями

✅ **Полная интеграция** с существующей системой NavMesh

✅ **Система анимаций** через Animator

✅ **Отладка** с Gizmos и OnGUI

✅ **Не требует Emerald AI** - работает сразу после создания

## Быстрый старт (3 минуты)

### Шаг 1: Создайте префаб врага

1. Создайте новый GameObject в сцене
2. Добавьте скрипт [`ImprovedEnemyAI.cs`](Assets/Infima Games/Low Poly Shooter Pack/Code/ImprovedEnemyAI.cs:1)
3. Скрипт автоматически добавит NavMeshAgent

### Шаг 2: Настройте Inspector

**Navigation:**
- **Patrol Points**: создайте пустые GameObjects и перетащите их сюда (минимум 2 точки)
- **Player Target**: перетащите префаб игрока (или оставьте пустым - будет найден по тегу "Player")

**Combat Settings:**
- **Enemy Health**: 100
- **Damage**: 10
- **Attack Range**: 15
- **Detection Range**: 20
- **Attack Cooldown**: 1.5
- **Attack Damage**: 10

**AI Settings:**
- **Patrol Speed**: 2
- **Chase Speed**: 4
- **Field Of View Angle**: 120
- **Player Layer**: выберите слой игрока
- **Obstruction Layer**: выберите слои препятствий (Default, Environment и т.д.)

**Debug:**
- **Debug Mode**: true (для отладки)
- **Show Gizmos**: true (для визуализации в Scene View)

### Шаг 3: Создайте NavMesh

1. Window > AI > Navigation
2. Выберите статические объекты сцены (пол, стены и т.д.)
3. Нажмите "Bake"

### Шаг 4: Создайте точки патруля

1. Создайте пустые GameObjects в местах, где должен патрулировать враг
2. Перетащите их в поле "Patrol Points" в Inspector
3. Минимум 2 точки для патрулирования

### Шаг 5: Сохраните как префаб

1. Перетащите врага из сцены в папку Assets
2. Удалите врага из сцены

### Шаг 6: Тестирование

1. Разместите префаб врага на сцене
2. Запустите игру
3. Проверьте, что враг патрулирует, обнаруживает и атакует игрока

## Состояния ИИ

### Idle (Ожидание)
- Враг стоит на месте
- Проверяет, видит ли игрока
- Переходит в Patrolling или Chasing

### Patrolling (Патрулирование)
- Враг движется между точками патруля
- Проверяет, видит ли игрока
- Переходит в Chasing при обнаружении игрока

### Chasing (Преследование)
- Враг преследует игрока
- Использует повышенную скорость (Chase Speed)
- Переходит в Attacking при достижении дистанции атаки
- Переходит в Investigating при потере игрока из виду

### Attacking (Атака)
- Враг останавливается и поворачивается к игроку
- Атакует с интервалом (Attack Cooldown)
- Переходит в Chasing если игрок уходит из дистанции атаки
- Переходит в Investigating при потере игрока из виду

### Investigating (Расследование)
- Враг движется к последней известной позиции игрока
- Проверяет, видит ли игрока
- Переходит в Chasing при обнаружении игрока
- Переходит в Patrolling если позиция достигнута

### Fleeing (Бегство)
- Враг убегает от игрока
- Использует повышенную скорость (Chase Speed * 1.2)
- Переходит в Patrolling если достаточно далеко убежал

### Dead (Смерть)
- Враг умер
- NavMeshAgent отключён
- Анимация смерти запущена
- Объект удаляется через 3 секунды

## Создание разных типов врагов

### Разведчик (Scout)

**Характеристики:**
- Высокая скорость
- Низкое здоровье
- Большой радиус обнаружения
- Большой угол обзора

**Настройки:**
```
Patrol Speed: 3
Chase Speed: 6
Enemy Health: 50
Attack Range: 10
Detection Range: 30
Field Of View Angle: 150
Attack Cooldown: 1.0
Attack Damage: 8
```

### Штурмовик (Assault)

**Характеристики:**
- Средняя скорость
- Среднее здоровье
- Средний радиус обнаружения
- Средний угол обзора

**Настройки:**
```
Patrol Speed: 2
Chase Speed: 4
Enemy Health: 100
Attack Range: 15
Detection Range: 20
Field Of View Angle: 120
Attack Cooldown: 1.5
Attack Damage: 10
```

### Снайпер (Sniper)

**Характеристики:**
- Низкая скорость
- Высокое здоровье
- Большой радиус обнаружения
- Малый угол обзора
- Большая дистанция атаки

**Настройки:**
```
Patrol Speed: 1
Chase Speed: 2
Enemy Health: 150
Attack Range: 30
Detection Range: 40
Field Of View Angle: 60
Attack Cooldown: 2.0
Attack Damage: 15
```

### Поддержка (Support)

**Характеристики:**
- Низкая скорость
- Высокое здоровье
- Малый радиус обнаружения
- Средний угол обзора
- Высокий урон

**Настройки:**
```
Patrol Speed: 1.5
Chase Speed: 3
Enemy Health: 200
Attack Range: 12
Detection Range: 15
Field Of View Angle: 100
Attack Cooldown: 2.5
Attack Damage: 20
```

## Получение урона

### Метод TakeDamage

```csharp
public void TakeDamage(float damageAmount, Vector3 hitPosition)
```

**Параметры:**
- `damageAmount`: количество урона
- `hitPosition`: позиция удара (для эффектов)

**Пример использования:**
```csharp
// Получение урона от оружия
enemy.TakeDamage(20f, hitPoint);

// Проверка смерти
if (enemy.IsDead())
{
    // Враг умер
}
```

### Интеграция с системой оружия

Чтобы враг получал урон от системы оружия Low Poly Shooter Pack, добавьте этот код в ваш скрипт оружия:

```csharp
private void DealDamage(GameObject target, Vector3 hitPoint, float damage)
{
    ImprovedEnemyAI enemy = target.GetComponent<ImprovedEnemyAI>();
    if (enemy != null)
    {
        enemy.TakeDamage(damage, hitPoint);
    }
}
```

## Публичные методы

```csharp
// Установить цель (игрока)
public void SetPlayerTarget(Transform target)

// Установить точки патруля
public void SetPatrolPoints(Transform[] points)

// Получить текущее состояние
public AIState GetCurrentState()

// Получить текущее здоровье
public float GetCurrentHealth()

// Проверить, мертв ли враг
public bool IsDead()
```

## Отладка

### Включение отладки

В Inspector включите `Debug Mode` и `Show Gizmos`.

### Что вы увидите:

**В Game View:**
- Имя врага
- Текущее здоровье
- Текущее состояние
- Видимость игрока
- Расстояние до игрока
- Кнопки для тестирования:
  - Take Damage (нанести урон)
  - Die (убить)
  - Toggle Debug (переключить отладку)

**В Scene View (Gizmos):**
- **Жёлтая сфера**: радиус обнаружения (Detection Range)
- **Красная сфера**: радиус атаки (Attack Range)
- **Синяя линия**: направление взгляда врага
- **Зелёные сферы**: точки патруля
- **Зелёные линии**: путь патрулирования

## Спавн врагов

### Простой спавнер

```csharp
using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    public class ImprovedEnemySpawner : MonoBehaviour
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
            
            // Получаем ссылку на ImprovedEnemyAI
            ImprovedEnemyAI enemyAI = enemy.GetComponent<ImprovedEnemyAI>();
            
            // Настраиваем игрока как цель
            if (enemyAI != null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    enemyAI.SetPlayerTarget(player.transform);
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

## Анимации

Скрипт поддерживает следующие параметры анимации:

**Параметры Animator:**
- `IsMoving` (bool): враг движется
- `IsAttacking` (bool): враг атакует
- `Speed` (float): скорость движения

**Триггеры Animator:**
- `Attack`: запуск атаки
- `Hit`: получение урона
- `Die`: смерть

### Пример настройки Animator Controller

1. Создайте новый Animator Controller
2. Добавьте параметры:
   - IsMoving (Bool)
   - IsAttacking (Bool)
   - Speed (Float)
   - Attack (Trigger)
   - Hit (Trigger)
   - Die (Trigger)
3. Создайте состояния анимации:
   - Idle
   - Walk
   - Run
   - Attack
   - Hit
   - Die
4. Настройте переходы между состояниями

## Сравнение с EnemyNavigation.cs

| Характеристика | EnemyNavigation.cs | ImprovedEnemyAI.cs |
|---------------|-------------------|-------------------|
| Состояния | 3 (Idle, Chasing, Attacking) | 8 (Idle, Patrolling, Chasing, Attacking, Reloading, Investigating, Fleeing, Dead) |
| Патрулирование | ❌ | ✅ |
| Расследование | ❌ | ✅ |
| Бегство | ❌ | ✅ |
| Отладка | Базовая | Полная (Gizmos + OnGUI) |
| Настраиваемость | Ограниченная | Полная |
| Анимации | Базовые | Полная поддержка |
| Emerald AI | ❌ | ❌ (но можно легко добавить) |

## Преимущества

✅ **Не требует Emerald AI** - работает сразу после создания
✅ **Машина состояний** - 8 состояний для реалистичного поведения
✅ **Полная настраиваемость** - все параметры доступны в Inspector
✅ **Интеграция с NavMesh** - работает с существующей системой
✅ **Система анимаций** - полная поддержка Animator
✅ **Отладка** - Gizmos и OnGUI для удобной отладки
✅ **Расширяемость** - легко добавлять новые состояния и поведения
✅ **Не ломает существующую систему** - работает параллельно с EnemyNavigation.cs

## Устранение проблем

### Враг не двигается
- ✅ Проверьте наличие NavMesh на сцене
- ✅ Убедитесь, что NavMeshAgent настроен правильно
- ✅ Проверьте, что враг находится на NavMesh слое
- ✅ Убедитесь, что точки патруля настроены

### Враг не обнаруживает игрока
- ✅ Проверьте тег игрока (должен быть "Player")
- ✅ Проверьте Detection Range и Field Of View Angle
- ✅ Убедитесь, что Player Layer выбран правильно
- ✅ Проверьте Obstruction Layer (должен включать слои препятствий)

### Враг не атакует
- ✅ Проверьте Attack Range
- ✅ Убедитесь, что Attack Cooldown настроен правильно
- ✅ Проверьте, что враг находится в состоянии Attacking

### Враг не получает урон
- ✅ Убедитесь, что метод TakeDamage вызывается правильно
- ✅ Проверьте, что объект имеет компонент ImprovedEnemyAI
- ✅ Убедитесь, что оружие вызывает TakeDamage на правильном объекте

### Враг застревает
- ✅ Проверьте NavMesh на наличие препятствий
- ✅ Убедитесь, что NavMeshAgent имеет правильные настройки
- ✅ Проверьте, что точки патруля достижимы

## Следующие шаги

1. ✅ Создайте префаб врага с ImprovedEnemyAI
2. ✅ Настройте NavMesh на вашей сцене
3. ✅ Создайте точки патруля
4. ✅ Протестируйте поведение врага
5. ✅ Создайте разные типы врагов с разными параметрами
6. ✅ Добавьте анимации через Animator
7. ✅ Интегрируйте с системой спавна врагов
8. ✅ Добавьте звуки и эффекты

## Документация

- [Анализ и улучшения Enemy AI](plans/ENEMY_AI_ANALYSIS_AND_IMPROVEMENTS.md)
- [Emerald AI Integration Guide](Assets/Data/EMERALD_AI_INTEGRATION_GUIDE.md) (если решите использовать Emerald AI)

## Заключение

ImprovedEnemyAI - это мощная и гибкая система ИИ для врагов, которая работает сразу после создания и не требует дополнительных пакетов. Рекомендуется использовать её для создания умных врагов с реалистичным поведением.
