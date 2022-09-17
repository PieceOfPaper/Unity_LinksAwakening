using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayData : SingletonTemplate<GamePlayData>
{

    #region  Player Entity Controller

    PlayerEntityController m_PlayerCtrler;
    public static PlayerEntityController PlayerCtrler => Instance.m_PlayerCtrler;

    public static void RegistPlayerEntityController(PlayerEntityController playerCtrler)
    {
        Instance.m_PlayerCtrler = playerCtrler;
    }

    public static void UnregistPlayerEntityController(PlayerEntityController playerCtrler)
    {
        if (Instance.m_PlayerCtrler != playerCtrler)
            return;

        Instance.m_PlayerCtrler = null;
    }

    #endregion
}
