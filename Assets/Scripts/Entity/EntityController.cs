using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    Animator m_Animator;
    UnityEngine.AI.NavMeshAgent m_NavMeshAgent;
    bool m_IsWalking = false;
    Vector2 m_MoveDir;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_NavMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        InitPlayerInput(GetComponent<UnityEngine.InputSystem.PlayerInput>());
    }

    // Update is called once per frame
    void Update()
    {
        if (m_MoveDir.sqrMagnitude > 0f)
        {
            m_NavMeshAgent.isStopped = false;
            m_NavMeshAgent.updateRotation = false;
            transform.rotation = Quaternion.LookRotation(Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * new Vector3(m_MoveDir.x, 0f, m_MoveDir.y));
            m_NavMeshAgent.SetDestination(transform.position + transform.rotation * Vector3.forward);

            if (m_IsWalking == false)
            {
                m_Animator.Play("Walk");
            }
            m_IsWalking = true;
        }
        else
        {
            m_NavMeshAgent.isStopped = true;

            if (m_IsWalking == true)
            {
                m_Animator.Play("Idle");
            }
            m_IsWalking = false;
        }
    }


    #region Input System

    protected UnityEngine.InputSystem.PlayerInput m_PlayerInput;
    protected UnityEngine.InputSystem.InputAction m_InputAction_Move;

    public void InitPlayerInput(UnityEngine.InputSystem.PlayerInput playerInput)
    {
        if (m_PlayerInput == playerInput)
            return;

        //���� �׼ǿ� �ݹ� ����
        if (m_InputAction_Move != null)
        {
            m_InputAction_Move.started -= OnInput_Move;
            m_InputAction_Move.canceled -= OnInput_Move;
            m_InputAction_Move.performed -= OnInput_Move;
            m_InputAction_Move = null;
        }

        m_PlayerInput = playerInput;
        if (m_PlayerInput != null)
        {
            m_InputAction_Move = m_PlayerInput.currentActionMap.FindAction("Move");
        }

        //�ű� �׼ǿ� �ݹ� ���
        if (m_InputAction_Move != null)
        {
            m_InputAction_Move.started += OnInput_Move;
            m_InputAction_Move.canceled += OnInput_Move;
            m_InputAction_Move.performed += OnInput_Move;
        }
    }

    void OnInput_Move(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.valueType == null)
        {
            m_MoveDir = Vector2.zero;
        }
        else
        {
            m_MoveDir = context.ReadValue<Vector2>();
        }
    }

    #endregion
}
