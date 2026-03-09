using UnityEngine;
using UnityEngine.AI;

namespace InfimaGames.LowPolyShooterPack
{
    public class EnemySpawner : MonoBehaviour {

    public int count;
    public GameObject enemy;
    public Transform SpawnPoint;

    public Transform points;
    public Transform Target;
    public Transform MainCamera;


    void Start() {
        for (int i = 0; i < count; i++) {
            GameObject go = Instantiate(enemy, SpawnPoint.position, SpawnPoint.rotation);
            go.GetComponent<NavMeshAgent>().avoidancePriority = i;
            go.GetComponent<EnemyNavigation>().points = points;
            go.GetComponent<EnemyNavigation>().Target = Target;
            go.GetComponent<EnemyNavigation>().MainCamera = MainCamera;
        }
    }
    }
}
