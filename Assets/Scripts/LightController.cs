using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public Light MyLight;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            MyLight.enabled = true;
        else if (Input.GetKeyUp(KeyCode.E)) {
            MyLight.enabled = false;
        }
    }
}
