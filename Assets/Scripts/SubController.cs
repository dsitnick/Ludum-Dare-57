using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubController : MonoBehaviour
{
    
    public float speed = 3;
    
    public float turnSpeed = 3;

    public float turnInertia = 30;
    public float swimIntertia = 30;
    public float boostScale = 3;

    Vector2 aim, aimSpin;
    Vector3 velocity;
    
    bool isBoosting;

    void Update()
    {
        Vector2 turnInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            -Input.GetAxisRaw("Vertical")
        );

        aimSpin = Vector2.Lerp(aimSpin, turnInput * turnSpeed, turnInertia * Time.deltaTime);

        aim += aimSpin * Time.deltaTime;
        aim.y = Mathf.Clamp(aim.y, -90, 90);
        
        transform.rotation = Quaternion.Euler(aim.y, aim.x, 0);
        
        isBoosting = Input.GetKey(KeyCode.LeftShift);
        
        Vector3 targetVelocity = transform.forward * GetDriveSpeed() * speed;
        
        velocity = Vector3.Lerp(velocity, targetVelocity, swimIntertia * Time.deltaTime);
        
        transform.position += velocity * Time.deltaTime;
        
    }
    
    float GetDriveSpeed(){
        if (isBoosting) return boostScale;
        
        float result = Input.GetAxisRaw("Distal");
        return result < 0 ? result * 0.5f : result;
    }

}
