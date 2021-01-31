using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Arm : MonoBehaviour
{
    public AudioClip HitSound;
    public AudioClip MoveSound;
    public Vector2 HitRange;

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
            bool inXRange = IsBetween(playerInTrigger.transform.position.x, transform.position.x - HitRange.x, transform.position.x + HitRange.x);
            bool inZRange = IsBetween(playerInTrigger.transform.position.z, transform.position.z - HitRange.y, transform.position.z + HitRange.y);
            if (inXRange && inZRange) {
                playerInTrigger.GetHit();
                myAudioSource.PlayOneShot(HitSound);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player"))
            return;

        if(other.transform.position.x > transform.position.x) {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
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

    public bool IsBetween(float testValue, float bound1, float bound2) {
        return (testValue >= Mathf.Min(bound1, bound2) && testValue <= Mathf.Max(bound1, bound2));
    }
}
