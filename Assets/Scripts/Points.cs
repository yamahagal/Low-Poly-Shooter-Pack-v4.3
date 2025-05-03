using UnityEngine;
using UnityEngine.AI;

public class Points : MonoBehaviour
{
    public NavMeshAgent agent;
    [SerializeField] public Transform[] points_list;
    public int index = 0;
    void Start()
    {
        points_list = GetComponentsInChildren<Transform>();
        agent.SetDestination(points_list[index].position);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void UpdateNextPoint()
    {
        if (index != points_list.Length)
        {
            index++;
            if (agent != null)
            {
                agent.SetDestination(points_list[index].position);
            }

        }
        else
        {
            index = 0;
            if (agent != null)
            {
                agent.SetDestination(points_list[index].position);
            }
        }
    }
}
