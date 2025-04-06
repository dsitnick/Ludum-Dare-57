using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SubController : MonoBehaviour
{
    public bool skipIntro;

    public PlayerInfoScriptableObject playerInfo;

    public float speed = 3;

    public float turnSpeed = 3;

    public float damageBounceSpeed = 5;
    public float collisionRadius = 5;
    public LayerMask collisionMask;

    public float turnInertia = 30;
    public float swimIntertia = 30;
    public float boostScale = 3;

    public UnityEvent onCollide;

    public KeyCode lightsKey = KeyCode.F;

    public LightSource lights;

    public Camera subCamera;

    public bool controlsActive;
    public bool damageActive = true;

    public UnityEvent onFinishDescent, onVictory;
    public AlarmSwitcher proximityAlarm;
    public CamController swimController;
    public HoldPressButton exitButton;

    public UnityEvent onActivateCamera;

    public float targetDepth;
    public float[] warningProximities;

    Vector2 aim, aimSpin;
    Vector3 velocity;
    public Vector3 overrideVelocity;

    bool isBoosting;

    bool isActive;
    bool hasStarted = false;

    void Start()
    {
        playerInfo.isObjectiveComplete = false;
        SetActive(true);
        exitButton.SetActive(false);

        if (skipIntro)
        {
            FinishDescent();
            Vector3 v = transform.position;
            v.y = targetDepth;
            transform.position = v;
        }
    }

    public void SetActive(bool isActive)
    {
        this.isActive = isActive;
        subCamera.enabled = subCamera.GetComponent<AudioListener>().enabled = isActive;
        exitButton.SetActive(isActive);

        if (isActive) onActivateCamera.Invoke();

        if (isActive && playerInfo.isObjectiveComplete)
        {
            onVictory.Invoke();
        }
    }

    void FinishDescent()
    {
        onFinishDescent.Invoke();
        hasStarted = true;
        exitButton.SetActive(true);
    }

    void Update()
    {
        if (controlsActive)
        {

            Vector3 targetVelocity = Vector3.zero;

            if (isActive)
            {
                if (Input.GetKeyDown(lightsKey))
                {
                    lights.SetLightActive(!lights.isOn);
                }
                Vector2 turnInput = new Vector2(
                    Input.GetAxisRaw("Horizontal"),
                    -Input.GetAxisRaw("Vertical")
                );

                aimSpin = Vector2.Lerp(aimSpin, turnInput * turnSpeed, turnInertia * Time.deltaTime);

                aim += aimSpin * Time.deltaTime;
                aim.y = Mathf.Clamp(aim.y, -90, 90);

                transform.rotation = Quaternion.Euler(aim.y, aim.x, 0);

                isBoosting = Input.GetKey(KeyCode.LeftShift);

                targetVelocity = transform.forward * GetDriveSpeed() * speed;
            }

            velocity = Vector3.Lerp(velocity, targetVelocity, swimIntertia * Time.deltaTime);
        }
        else
        {
            velocity = overrideVelocity;
            if (!hasStarted && transform.position.y <= targetDepth)
            {
                FinishDescent();
            }
        }
        transform.position += velocity * Time.deltaTime;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (isActive) playerInfo.position = transform.position;

    }

    float GetDriveSpeed()
    {
        if (isBoosting) return boostScale;

        float result = Input.GetAxisRaw("Distal");
        return result < 0 ? result * 0.5f : result;
    }

    public void ExitSub()
    {
        SetActive(false);
        swimController.SetActive(true);
        swimController.SpawnAtPosition(transform.position);
    }

    Collider[] collisionBuffer = new Collider[1];

    int currentProximityLevel;

    void FixedUpdate()
    {
        int count = 0;

        int lastProximityLevel = currentProximityLevel;
        currentProximityLevel = -1;
        if (controlsActive)
        {
            for (int i = 0; i < warningProximities.Length; i++)
            {
                count = Physics.OverlapSphereNonAlloc(transform.position, warningProximities[i], collisionBuffer, collisionMask);
                if (count > 0)
                {
                    currentProximityLevel = i;
                }
                else
                {
                    break;
                }
            }
        }


        if (lastProximityLevel != currentProximityLevel)
        {
            proximityAlarm.SetAlarmLevel(currentProximityLevel);
        }

        if (isBlockingcollision) return;

        count = Physics.OverlapSphereNonAlloc(transform.position, collisionRadius, collisionBuffer, collisionMask);
        if (count > 0)
        {
            Collider c = collisionBuffer[0];
            Vector3 point = c.ClosestPoint(transform.position);
            velocity = (transform.position - point).normalized * damageBounceSpeed;
            StartCoroutine(CollisionBlocker());

            if (damageActive)
            {
                onCollide.Invoke();
            }

            FishBehavior fish = c.GetComponentInParent<FishBehavior>();
            if (fish != null)
            {
                fish.Hit();
            }
        }
    }

    bool isBlockingcollision;
    IEnumerator CollisionBlocker()
    {
        isBlockingcollision = true;
        yield return new WaitForSeconds(1.5f);
        isBlockingcollision = false;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
