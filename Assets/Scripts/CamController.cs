using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CamController : PlayerController {
    
    [Range(0, 10)]
    public float speed = 3;
    [Range(1, 10)]
    public float sprintScale = 3;
    [Range(0, 10)]
    public float mouseSens = 3;
    
    public float swimIntertia = 30;
    
    public KeyCode flashlightKey = KeyCode.F;
    
    public new Camera camera;
    public Light[] flashlight;
    
    [SerializeField] public UnityEvent<bool> onSetActive;
    
    Vector2 aim;
    
    float moveSpeed => (Input.GetKey(KeyCode.LeftShift) ? sprintScale : 1) * speed;
    
    Vector3 velocity;
    
    bool isActive;

    public override void SetActive(bool isActive)
    {
        this.isActive = isActive;
        camera.enabled = isActive;
        onSetActive.Invoke(isActive);
    }
    
    void Update() {
        if (!isActive){
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
        
        transform.position += velocity * Time.deltaTime;
        
        if (Input.GetKeyDown(flashlightKey)){
            foreach (Light f in flashlight) f.enabled = !f.enabled;
        }
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
    }
    
}
