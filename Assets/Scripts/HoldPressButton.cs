using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HoldPressButton : MonoBehaviour
{

    public KeyCode button;

    public float heldDuration;

    public UnityEvent onHoldPress;
    public UnityEventFloat onProgress;
    public UnityEvent<bool> onSetHeld;

    bool isHeld = false;
    float progress = 0;

    void Start()
    {
        onSetHeld.Invoke(false);
        onProgress.Invoke(0);
    }

    void Update()
    {
        if (!isHeld)
        {
            if (Input.GetKeyDown(button))
            {
                isHeld = true;
                progress = 0;
                onProgress.Invoke(progress);
                onSetHeld.Invoke(true);

            }
        }
        else if (Input.GetKeyUp(button))
        {
            onSetHeld.Invoke(false);
            isHeld = false;
        }
        else
        {

            progress += Time.deltaTime / heldDuration;

            if (progress > 1)
            {
                progress = 1;
                onHoldPress.Invoke();
                onSetHeld.Invoke(false);
                isHeld = false;
            }
            onProgress.Invoke(progress);

        }
    }

}
