using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntityController : EntityController
{
    
    protected override void OnEnable()
    {
        GamePlayData.RegistPlayerEntityController(this);
    }

    protected override void OnDisable()
    {
        GamePlayData.UnregistPlayerEntityController(this);
    }

    protected override void Start()
    {
        base.Start();

        InitPlayerInput(GetComponent<UnityEngine.InputSystem.PlayerInput>());
    }


    #region Input System

    protected UnityEngine.InputSystem.PlayerInput m_PlayerInput;
    protected UnityEngine.InputSystem.InputAction m_InputAction_Move;

    public void InitPlayerInput(UnityEngine.InputSystem.PlayerInput playerInput)
    {
        if (m_PlayerInput == playerInput)
            return;

        //이전 액션에 콜백 제거
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

        //신규 액션에 콜백 등록
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
