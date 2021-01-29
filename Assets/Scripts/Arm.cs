using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Arm : MonoBehaviour
{
    private Animator myAnimator;
    private Crow playerInTrigger;

    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInTrigger == null)
            return;

        if(playerInTrigger.swoopTimer > 0f || playerInTrigger.MyLight.enabled) {
            myAnimator.SetTrigger("Rise");
        }
    }

    private void CheckForHit() {
        if(playerInTrigger != null) {
            playerInTrigger.GetComponent<Rigidbody>().useGravity = true;
            playerInTrigger.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            playerInTrigger.enabled = false;
            Invoke("RestartGame", 3f);
        }
    }

    private void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player"))
            return;

        playerInTrigger = other.gameObject.GetComponent<Crow>();
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Player"))
            return;

        playerInTrigger = null;
    }

    private void OnCollisionEnter(Collision collision) {
        if (!collision.collider.CompareTag("Player"))
            return;

        CheckForHit();
    }
}
