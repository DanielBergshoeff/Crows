using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Arm : MonoBehaviour
{
    public AudioClip HitSound;
    public AudioClip MoveSound;

    private Animator myAnimator;
    private Crow playerInTrigger;
    private AudioSource myAudioSource;
    private bool rising = false;

    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        myAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInTrigger == null)
            return;

        if((playerInTrigger.swoopTimer > 0f || playerInTrigger.MyLight.enabled) && !rising) {
            myAnimator.SetTrigger("Rise");
            rising = true;
            myAudioSource.PlayOneShot(MoveSound);
        }
    }

    private void ResetRising() {
        rising = false;
    }

    private void CheckForHit() {
        if(playerInTrigger != null) {
            playerInTrigger.GetHit();
            myAudioSource.PlayOneShot(HitSound);
        }
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
