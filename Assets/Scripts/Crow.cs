using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crow : MonoBehaviour
{
    public float Speed = 1f;
    public float SwoopTime = 1f;
    public float SwoopAmount = 0.5f;
    public float Power = 2f;
    public float Height = 0.5f;

    private Vector3 swoopDir;
    private float swoopTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Swoop();
    }

    private void Swoop() {
        if (swoopTimer <= 0f)
            return;

        swoopTimer -= Time.deltaTime;

        float f = swoopTimer / SwoopTime;
        if (f > 0.5f) {
            f = f - 0.5f;
        }
        else {
            f = 0.5f - f;
        }
        f = f * 2f;
        f = Mathf.Pow(f, Power);
        f = f * SwoopAmount;

        transform.position = new Vector3(transform.position.x, f + Height, transform.position.z);
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

        moveDir = moveDir.normalized;

        if (Input.GetKeyDown(KeyCode.Space)) {
            Swoop(moveDir);
        }

        transform.position = transform.position + moveDir * Time.deltaTime * Speed;
    }

    private void Swoop(Vector3 dir) {
        swoopDir = dir;
        swoopTimer = SwoopTime;
    }
}
