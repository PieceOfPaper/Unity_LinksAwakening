using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    protected Animator m_Animator;
    protected UnityEngine.AI.NavMeshAgent m_NavMeshAgent;
    protected bool m_IsWalking = false;
    protected Vector2 m_MoveDir;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    protected virtual void OnEnable()
    {
        
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    protected virtual void OnDisable()
    {
        
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_NavMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (m_MoveDir.sqrMagnitude > 0f)
        {
            if (m_NavMeshAgent.isOnNavMesh == true)
            {
                m_NavMeshAgent.isStopped = false;
                m_NavMeshAgent.updateRotation = false;
                transform.rotation = Quaternion.LookRotation(Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * new Vector3(m_MoveDir.x, 0f, m_MoveDir.y));
                m_NavMeshAgent.SetDestination(transform.position + transform.rotation * Vector3.forward);
            }
            if (m_IsWalking == false)
            {
                m_Animator.Play("Walk");
            }
            m_IsWalking = true;
        }
        else
        {
            if (m_NavMeshAgent.isOnNavMesh == true) m_NavMeshAgent.isStopped = true;

            if (m_IsWalking == true)
            {
                m_Animator.Play("Idle");
            }
            m_IsWalking = false;
        }
    }
}
