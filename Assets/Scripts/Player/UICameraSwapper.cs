using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICameraSwapper : MonoBehaviour
{

    public Camera cam;

    public Canvas canvas;

    public void SetCamera()
    {
        canvas.worldCamera = cam;

    }

}
