using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public bool Enemy = false;

    private void OnDrawGizmosSelected() {
        Gizmos.color = Enemy ? Color.red : Color.white;
        Gizmos.DrawWireCube(transform.position + transform.up * 0.05f, Vector3.one * 0.1f);
    }
}
