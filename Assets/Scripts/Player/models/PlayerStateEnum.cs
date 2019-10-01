using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.hermitGames.rp
{
    [System.Serializable]
    public enum PlayerStateEnum
    {
        IDLE,
        MOVE,
        JUMP,
        READY_TO_PUNCH,
        PUNCH
    }
}