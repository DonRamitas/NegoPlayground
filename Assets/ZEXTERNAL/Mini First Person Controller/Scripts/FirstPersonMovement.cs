using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    public float speed = 5;

    [Header("Running")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 9;
    public KeyCode runningKey = KeyCode.LeftShift;

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
        // Update IsRunning from input.
        IsRunning = canRun && Input.GetKey(runningKey);

        // Get targetMovingSpeed.
        float targetMovingSpeed = IsRunning ? runSpeed : speed;
        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        // Get targetVelocity from input.
        Vector2 targetVelocity =new Vector2( Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed);

        // Apply movement.
        if(PlayerStateScript.PlayerStatus[0] || PlayerStateScript.PlayerStatus[1] || PlayerStateScript.PlayerStatus[2]){
            rb.velocity = transform.rotation * new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.y);
        }else{
            rb.velocity = new Vector3(0.0f,rb.velocity.y,0.0f);
        }
    }
}