using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Crow : MonoBehaviour
{
    [Header("Light")]
    public Light MyLight;
    public Material LanternMat;
    public Texture2D LanternOn;
    public Texture2D LanternOff;

    [Header("Movement")]
    public float Speed = 1f;
    public float DiveSpeed = 1.5f;
    public float BonusSpeed = 1f;
    public int MaxBonusFlaps = 3;
    public float ReduceBonusSpeed = 0.5f;
    public float BonusFlapRecoveryTime = 1f;
    public float RotateSpeed = 3f;
    public float SwoopTime = 1f;
    public float SwoopAmount = 0.5f;
    public float Power = 2f;
    public float Height = 0.5f;

    [Header("Audio")]
    public float TimePerFlap = 1f;
    public float MinTimeYap = 3f;
    public float MaxTimeYap = 5f;

    public GameObject DeathPrefab;
    public Animator MyAnimator;

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
    public ShinyObject ObjectHeld;
    private bool inAltar = false;
    private Rigidbody myRigidbody;
    private AudioSource myAudioSource;

    private float flapTimer = 0.1f;
    private float yapTimer = 0.1f;
    private float currentBonusSpeed = 0f;
    private int currentBonusFlaps = 3;
    private float bonusFlapRecovery = 1f;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        myAudioSource = GetComponent<AudioSource>();
        moveDir = transform.forward;
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
        /*if (flapTimer > 0f) {
            flapTimer -= Time.deltaTime;
            if (flapTimer <= 0f) {
                myAudioSource.PlayOneShot(WingFlapSounds[Random.Range(0, WingFlapSounds.Count)]);
                flapTimer = TimePerFlap;
            }
        }*/
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
        GameManager.RestartGame();
        myAudioSource.PlayOneShot(DeathSound);
        GameObject go = Instantiate(DeathPrefab);
        go.transform.position = transform.position;
        Destroy(gameObject);
    }

    private void UpdateLight() {
        if (Input.GetKeyDown(KeyCode.Return)) {
            MyLight.enabled = true;
            LanternMat.SetTexture("_MainTex", LanternOn);
        }
        else if (Input.GetKeyUp(KeyCode.Return)) {
            MyLight.enabled = false;
            LanternMat.SetTexture("_MainTex", LanternOff);
        }
    }

    public void DropObject() {
        ObjectHeld.transform.parent = null;
        ObjectHeld.GetComponent<Rigidbody>().isKinematic = false;
        ObjectHeld.GetComponent<Collider>().enabled = true;
        ObjectHeld = null;
    }

    private void Swoop() {
        if (swoopTimer <= 0f)
            return;

        if (inAltar && ObjectHeld != null) {
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

    private void ChangeDir() {
        if (Input.GetKey(KeyCode.A)) {
            transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, -transform.right, Time.deltaTime * RotateSpeed, 0f));
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, transform.right, Time.deltaTime * RotateSpeed, 0f));
        }
    }

    public void AddBoost() {
        currentBonusSpeed += BonusSpeed;
    }

    private void Move() {
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) {
            ChangeDir();
        }

        if (Input.GetKeyDown(KeyCode.Space) && swoopTimer <= 0f) {
            Swoop(moveDir);
        }

        if((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && currentBonusFlaps > 0) {
            currentBonusFlaps--;
            myAudioSource.PlayOneShot(WingFlapSounds[Random.Range(0, WingFlapSounds.Count)]);
            MyAnimator.SetTrigger("WingFlap");
        }

        if (currentBonusSpeed > 0f)
            currentBonusSpeed -= Time.deltaTime * ReduceBonusSpeed;

        if (bonusFlapRecovery > 0f) {
            bonusFlapRecovery -= Time.deltaTime;
            if(bonusFlapRecovery <= 0f) {
                bonusFlapRecovery = BonusFlapRecoveryTime;
                if (currentBonusFlaps < MaxBonusFlaps)
                    currentBonusFlaps++;
            }
                
        }

        float s = (swoopTimer <= 0f) ? Speed : DiveSpeed;
        transform.position = transform.position + transform.forward * Time.deltaTime * (s + currentBonusSpeed);
        myRigidbody.velocity = Vector3.zero;
    }

    private void Swoop(Vector3 dir) {
        swoopDir = dir;
        swoopTimer = SwoopTime;
        myAudioSource.PlayOneShot(DiveSounds[Random.Range(0, DiveSounds.Count)]);
        MyAnimator.SetTrigger("Dive");
    }

    private void OnCollisionEnter(Collision collision) {
        if (swoopTimer <= 0f || ObjectHeld != null)
            return;

        if (!collision.collider.CompareTag("Shiny"))
            return;

        ObjectHeld = collision.collider.gameObject.GetComponent<ShinyObject>();
        ObjectHeld.transform.parent = transform;
        ObjectHeld.transform.localPosition = Vector3.zero;
        ObjectHeld.GetComponent<Rigidbody>().isKinematic = true;
        ObjectHeld.GetComponent<Collider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Altar")) {
            inAltar = true;
        }
        else if (other.CompareTag("Glimmer") && ObjectHeld == null) {
            Glimmer g = other.gameObject.GetComponent<Glimmer>();
            GameObject go = Instantiate(g.MyShinyObjectPrefab);

            ObjectHeld = go.GetComponent<ShinyObject>();
            ObjectHeld.transform.parent = transform;
            ObjectHeld.transform.localPosition = Vector3.zero;
            ObjectHeld.GetComponent<Rigidbody>().isKinematic = true;
            ObjectHeld.GetComponent<Collider>().enabled = false;

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
