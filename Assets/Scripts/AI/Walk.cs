using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Walk : StateMachineBehaviour
{
    float timer;
    List<Transform> points = new List<Transform>();
    UnityEngine.AI.NavMeshAgent agent;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       timer = 0;
       Transform pointsObject = GameObject.FindGameObjectWithTag("Points").transform;
       foreach (Transform t in pointsObject)
        points.Add(t);

        agent = animator.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null && agent.isOnNavMesh && agent.isActiveAndEnabled)
        {
            agent.speed = 3.5f;
            agent.stoppingDistance = 0.1f;
            if (points.Count > 0)
            {
                agent.SetDestination(points[0].position);
                Debug.Log($"Направляюсь к точке: {points[0].position}");
            }
            else
            {
                Debug.LogWarning("Нет точек для перемещения!");
            }
        }
        else
        {
            Debug.LogError("NavMeshAgent не активен или не на NavMesh!");
        }
    }

   
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       if (agent != null && agent.isOnNavMesh && agent.isActiveAndEnabled)
       {
           if (agent.remainingDistance <= agent.stoppingDistance)
           {
               Vector3 nextPoint = points[Random.Range(0, points.Count)].position;
               agent.SetDestination(nextPoint);
               Debug.Log($"Иду к следующей точке: {nextPoint}");
           }
           
           if (agent.velocity.magnitude < 0.1f && !agent.pathStatus.Equals(UnityEngine.AI.NavMeshPathStatus.PathPartial))
           {
               Debug.LogWarning("Агент не двигается! Скорость: " + agent.velocity.magnitude);
           }
       }
       
       timer += Time.deltaTime;
       if (timer > 10)
           animator.SetBool("Walk", false);
    }

   
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       if (agent != null && agent.isOnNavMesh && agent.isActiveAndEnabled)
       {
           agent.SetDestination(agent.transform.position);
       }
    }

   
}
