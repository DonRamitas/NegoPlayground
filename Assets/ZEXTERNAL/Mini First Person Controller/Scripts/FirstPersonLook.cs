using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    [SerializeField]
    Transform character;
    public float sensitivity = 2;
    float MainSensitivity;
    public float smoothing = 1.5f;

    public Vector2 velocity;
    public Vector2 frameVelocity;

    GameObject PlayerCamGO;
    PlayerState PlayerStateScript;

    void Reset()
    {
        // Get the character from the FirstPersonMovement in parents.
        character = GetComponentInParent<FirstPersonMovement>().transform;
    }

    void Awake(){
        PlayerCamGO = GameObject.FindObjectOfType<PlayerState>().gameObject;
        PlayerStateScript = PlayerCamGO.GetComponent<PlayerState>();
    }


    void Start()
    {
        // Lock the mouse cursor to the game screen.
        Cursor.lockState = CursorLockMode.Locked;
        MainSensitivity = sensitivity;
    }

    void Update()
    {
        if((Cursor.lockState == CursorLockMode.None) || PlayerStateScript.PlayerStatus[3]){
            sensitivity = 0;
        }else{
            sensitivity = MainSensitivity;
        }
        // Get smooth velocity.
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -90, 90);
        // Rotate camera up-down and controller left-right from velocity.
        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
        
        
    }
}
