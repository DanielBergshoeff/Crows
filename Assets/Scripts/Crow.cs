using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crow : MonoBehaviour
{
    public float Speed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move() {
        Vector3 moveDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) {
            Vector3 camForward = Camera.main.transform.forward;
            moveDir += new Vector3(camForward.x, 0f, camForward.z);
        }
        if (Input.GetKey(KeyCode.S)) {
            Vector3 camBackward = -Camera.main.transform.forward;
            moveDir += new Vector3(camBackward.x, 0f, camBackward.z);
        }
        if (Input.GetKey(KeyCode.A)) {
            Vector3 leftCam = -Camera.main.transform.right;
            moveDir += new Vector3(leftCam.x, 0f, leftCam.z);
        }
        if (Input.GetKey(KeyCode.D)) {
            Vector3 rightCam = Camera.main.transform.right;
            moveDir += new Vector3(rightCam.x, 0f, rightCam.z);
        }

        transform.position = transform.position + moveDir.normalized * Time.deltaTime * Speed;
    }
}
