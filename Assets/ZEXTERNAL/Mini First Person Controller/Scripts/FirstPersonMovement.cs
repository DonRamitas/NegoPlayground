using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    public float speed = 5;
    public float podometro = 0;
    public float lastStep = 0;

    [Header("Running")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 9;
    public KeyCode runningKey = KeyCode.LeftShift;

    public AudioClip[] stepSounds;

    public GroundCheck GroundCheckScript;

    Rigidbody rb;
    /// <summary> Functions to override movement speed. Will use the last added override. </summary>
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    GameObject PlayerCamGO;
    PlayerState PlayerStateScript;

    void Awake()
    {
        // Get the rb on this.
        rb = GetComponent<Rigidbody>();

        PlayerCamGO = GameObject.FindObjectOfType<PlayerState>().gameObject;
        PlayerStateScript = PlayerCamGO.GetComponent<PlayerState>();
    }

    void FixedUpdate()
    {
        podometro += Time.deltaTime * rb.velocity.magnitude;

        if(podometro - lastStep > 2 && GroundCheckScript.isGrounded){
            Debug.Log(GroundCheckScript.currentFloor);
            GetComponent<AudioSource>().pitch = 1 - Random.Range(0,3)*0.05f;
            switch(GroundCheckScript.currentFloor){
                case "DirtTerrain":
                    int dirtstep = Random.Range(0, 3);
                    GetComponent<AudioSource>().PlayOneShot(stepSounds[dirtstep]);
                    break;
                case "Stone":
                    int stonestep = Random.Range(3, 6);
                    GetComponent<AudioSource>().PlayOneShot(stepSounds[stonestep]);
                    break;
                default:
                    Debug.Log("No sound for this terrain");
                    break;
            }
            lastStep = podometro;
        }

        // Update IsRunning from input.
        IsRunning = canRun && Input.GetKey(runningKey);

        // Get targetMovingSpeed.
        float targetMovingSpeed = IsRunning ? runSpeed : speed;
        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        // Get targetVelocity from input.
        Vector2 targetVelocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        float max = Mathf.Max(Mathf.Abs(Input.GetAxis("Horizontal")), Mathf.Abs(Input.GetAxis("Vertical")));
        float magnitud = max/1;
        targetVelocity.Normalize();
        targetVelocity = new Vector2(targetVelocity.x * targetMovingSpeed* magnitud, targetVelocity.y * targetMovingSpeed * magnitud);

        // Apply movement.
        if(PlayerStateScript.PlayerStatus[0] || PlayerStateScript.PlayerStatus[1] || PlayerStateScript.PlayerStatus[2]){
            rb.velocity = transform.rotation * new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.y);
        }else{
            rb.velocity = new Vector3(0.0f,rb.velocity.y,0.0f);
        }
    }
}