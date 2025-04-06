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
    public UnityEvent<bool> onSetHeld, onSetActive;
    public bool isActive = true;
    public bool resetProgress;

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
            if (isActive && Input.GetKeyDown(button))
            {
                Begin();
            }
        }
        else if (!isActive || Input.GetKeyUp(button))
        {
            Cancel();
        }
        else
        {

            progress += Time.deltaTime / heldDuration;

            if (progress > 1)
            {
                Complete();
            }
            onProgress.Invoke(progress);

        }
    }

    void Begin()
    {
        if (resetProgress)
        {
            progress = 0;
        }
        else if (progress >= 1)
        {
            return;
        }
        isHeld = true;
        onProgress.Invoke(progress);
        onSetHeld.Invoke(true);


    }

    void Cancel()
    {
        onSetHeld.Invoke(false);
        isHeld = false;
    }

    void Complete()
    {
        progress = 1;
        onHoldPress.Invoke();
        onSetHeld.Invoke(false);
        isHeld = false;
    }

    public void SetActive(bool isActive)
    {
        this.isActive = isActive;
        onSetActive.Invoke(isActive);
    }

}
