using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform Player;

    private Vector3 relativePos;

    // Start is called before the first frame update
    void Start()
    {
        relativePos = transform.position - Player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Player.transform.position + relativePos;
    }
}
