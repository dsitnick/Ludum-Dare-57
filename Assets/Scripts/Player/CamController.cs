using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CamController : MonoBehaviour
{

    [Range(0, 10)]
    public float speed = 3;
    [Range(1, 10)]
    public float sprintScale = 3;
    [Range(0, 10)]
    public float mouseSens = 3;

    public float swimIntertia = 30;

    public KeyCode flashlightKey = KeyCode.F;

    public new Camera camera;
    public LightSource flashlight;

    public PlayerInfoScriptableObject playerInfo;
    public SubController subController;

    public HoldPressButton boardSubButton;

    public Rigidbody rb;
    public CapsuleCollider capsuleCollider;
    public LayerMask collisionMask;

    public UnityEvent onDie;

    [SerializeField] public UnityEvent<bool> onSetActive;

    Vector2 aim;

    float moveSpeed => (Input.GetKey(KeyCode.LeftShift) ? sprintScale : 1) * speed;

    Vector3 velocity;

    bool isActive;

    void Start()
    {
        SetActive(false);
    }

    public void SetActive(bool isActive)
    {
        this.isActive = isActive;
        camera.enabled = camera.GetComponent<AudioListener>().enabled = isActive;
        onSetActive.Invoke(isActive);
        boardSubButton.SetActive(isActive);
        SetFlashlightActive(isActive);
    }

    public void SpawnAtPosition(Vector3 position)
    {
        position += subController.transform.up * -10;
        velocity = subController.transform.up * -8;
        transform.position = playerInfo.position = position;
    }

    void Update()
    {
        if (!isActive)
        {
            return;
        }
        aim += new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y")) * mouseSens;
        aim.y = Mathf.Clamp(aim.y, -90, 90);

        transform.eulerAngles = Vector3.up * aim.x;
        camera.transform.localEulerAngles = Vector3.right * aim.y;

        //Vector3 moveDir = Quaternion.Euler(0, aim.x, 0) * new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Distal"), Input.GetAxisRaw("Vertical")).normalized;
        Vector3 moveDir = (
            camera.transform.right * Input.GetAxisRaw("Horizontal") +
            camera.transform.forward * Input.GetAxisRaw("Vertical") +
            camera.transform.up * Input.GetAxisRaw("Distal")
        ).normalized;

        velocity = Vector3.Lerp(velocity, moveDir * moveSpeed, swimIntertia * Time.deltaTime);

        /*float speed = velocity.magnitude;

        if (speed > 0)
        {
            float collisionDistance = CollisionDistance(speed, velocity / speed, Time.deltaTime);
            transform.position += velocity / speed * collisionDistance;
        }*/

        playerInfo.position = transform.position;

        if (Input.GetKeyDown(flashlightKey))
        {
            SetFlashlightActive(!flaslightOn);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    public void BoardSubmarine()
    {
        SetActive(false);
        subController.SetActive(true);
    }

    bool wasSubProximity;
    void FixedUpdate()
    {
        bool subProximity = CheckSubmarineProximity();
        if (subProximity != wasSubProximity)
        {
            wasSubProximity = subProximity;
            boardSubButton.SetActive(subProximity);
        }

        rb.position += velocity * Time.fixedDeltaTime;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.rotation = Quaternion.Euler(Vector3.up * aim.x);

    }

    const float SUB_BOARD_DISTANCE = 3;
    bool CheckSubmarineProximity() => Vector3.SqrMagnitude(transform.position - subController.transform.position) <= SUB_BOARD_DISTANCE * SUB_BOARD_DISTANCE;

    bool flaslightOn;
    void SetFlashlightActive(bool flaslightOn)
    {
        this.flaslightOn = flaslightOn;
        flashlight.SetLightActive(flaslightOn);
    }

    public void Die()
    {
        Debug.Log("Got eaten");
        onDie.Invoke();
    }

}
