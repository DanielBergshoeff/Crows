using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimWing : MonoBehaviour
{
    public Crow MyCrow;

    // Start is called before the first frame update
    void Start()
    {
        MyCrow = GetComponentInParent<Crow>();
    }

    private void BoostNow() {
        MyCrow.AddBoost();
    }
}
