using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollisions : MonoBehaviour
{
    [SerializeField]
    public UIManager uiManager;

    private void OnCollisionEnter(Collision collision)
    {
        uiManager.BlockTouchedGround();
    }
}
