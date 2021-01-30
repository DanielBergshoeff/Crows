using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Crow : MonoBehaviour
{
    public Light MyLight;

    public float Speed = 1f;
    public float RotateSpeed = 3f;
    public float SwoopTime = 1f;
    public float SwoopAmount = 0.5f;
    public float Power = 2f;
    public float Height = 0.5f;
    public float TimePerFlap = 1f;
    public float MinTimeYap = 3f;
    public float MaxTimeYap = 5f;

    public AudioClip PickupSound;
    public List<AudioClip> DiveSounds;
    public List<AudioClip> WingFlapSounds;
    public List<AudioClip> YapSounds;
    public AudioClip DeathSound;

    public Transform CrowImage;

    private Vector3 swoopDir;
    public float swoopTimer = 0f;
    Vector3 moveDir = Vector3.zero;
    Vector3 targetMoveDir = Vector3.zero;
    private ShinyObject objectHeld;
    private bool inAltar = false;
    private Rigidbody myRigidbody;
    private AudioSource myAudioSource;

    private float flapTimer = 0.1f;
    private float yapTimer = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        myAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Narrative.InPoem)
            return;

        UpdateLight();
        Move();
        Swoop();
        UpdateTimers();
    }

    private void UpdateTimers() {
        if (flapTimer > 0f) {
            flapTimer -= Time.deltaTime;
            if (flapTimer <= 0f) {
                myAudioSource.PlayOneShot(WingFlapSounds[Random.Range(0, WingFlapSounds.Count)]);
                flapTimer = TimePerFlap;
            }

        }
        if (yapTimer > 0f) {
            yapTimer -= Time.deltaTime;
            if(yapTimer <= 0f) {
                myAudioSource.PlayOneShot(YapSounds[Random.Range(0, YapSounds.Count)]);
                yapTimer = Random.Range(MinTimeYap, MaxTimeYap);
            }
        }
    }

    public void GetHit() {
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        Invoke("RestartGame", 3f);
        myAudioSource.PlayOneShot(DeathSound);
        enabled = false;
    }

    private void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void UpdateLight() {
        if (Input.GetKeyDown(KeyCode.Return))
            MyLight.enabled = true;
        else if (Input.GetKeyUp(KeyCode.Return)) {
            MyLight.enabled = false;
        }
    }

    private void DropObject() {
        objectHeld.transform.parent = null;
        objectHeld.GetComponent<Rigidbody>().isKinematic = false;
        objectHeld.GetComponent<Collider>().enabled = true;
        objectHeld = null;
    }

    private void Swoop() {
        if (swoopTimer <= 0f)
            return;

        if (inAltar && objectHeld != null) {
            DropObject();
        }

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

    private void ChangeDir() {/*
        targetMoveDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) {
            Vector3 camForward = Camera.main.transform.forward;
            targetMoveDir += new Vector3(camForward.x, 0f, camForward.z);
        }
        if (Input.GetKey(KeyCode.S)) {
            Vector3 camBackward = -Camera.main.transform.forward;
            targetMoveDir += new Vector3(camBackward.x, 0f, camBackward.z);
        }
        if (Input.GetKey(KeyCode.A)) {
            Vector3 leftCam = -Camera.main.transform.right;
            targetMoveDir += new Vector3(leftCam.x, 0f, leftCam.z);
        }
        if (Input.GetKey(KeyCode.D)) {
            Vector3 rightCam = Camera.main.transform.right;
            targetMoveDir += new Vector3(rightCam.x, 0f, rightCam.z);
        }

        targetMoveDir = targetMoveDir.normalized;

        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, targetMoveDir, Time.deltaTime * RotateSpeed, 0f));*/

        if (Input.GetKey(KeyCode.A)) {
            transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, -transform.right, Time.deltaTime * RotateSpeed, 0f));
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, transform.right, Time.deltaTime * RotateSpeed, 0f));
        }
        //CrowImage.rotation = Quaternion.LookRotation(moveDir, Vector3.up);
        //transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up);
    }

    private void Move() {
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) {
            ChangeDir();
        }

        if (Input.GetKeyDown(KeyCode.Space) && swoopTimer <= 0f) {
            Swoop(moveDir);
        }

        transform.position = transform.position + transform.forward * Time.deltaTime * Speed;
        myRigidbody.velocity = Vector3.zero;
    }

    private void Swoop(Vector3 dir) {
        swoopDir = dir;
        swoopTimer = SwoopTime;
        myAudioSource.PlayOneShot(DiveSounds[Random.Range(0, DiveSounds.Count)]);
    }

    private void OnCollisionEnter(Collision collision) {
        if (swoopTimer <= 0f || objectHeld != null)
            return;

        if (!collision.collider.CompareTag("Shiny"))
            return;

        objectHeld = collision.collider.gameObject.GetComponent<ShinyObject>();
        objectHeld.transform.parent = transform;
        objectHeld.transform.localPosition = Vector3.zero;
        objectHeld.GetComponent<Rigidbody>().isKinematic = true;
        objectHeld.GetComponent<Collider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Altar")) {
            inAltar = true;
        }
        else if (other.CompareTag("Glimmer") && objectHeld == null) {
            Glimmer g = other.gameObject.GetComponent<Glimmer>();
            GameObject go = Instantiate(g.MyShinyObjectPrefab);

            objectHeld = go.GetComponent<ShinyObject>();
            objectHeld.transform.parent = transform;
            objectHeld.transform.localPosition = Vector3.zero;
            objectHeld.GetComponent<Rigidbody>().isKinematic = true;
            objectHeld.GetComponent<Collider>().enabled = false;

            myAudioSource.PlayOneShot(PickupSound);

            Destroy(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Altar")) {
            inAltar = false;
        }
    }
}
