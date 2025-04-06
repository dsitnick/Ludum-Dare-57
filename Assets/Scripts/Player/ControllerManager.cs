using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{

    public enum Controller { Swim, Sub }
    private static ControllerManager singleton;
    void Awake()
    {
        singleton = this;
    }

    void Start()
    {
        ActivateController(defaultController);
    }

    public Controller defaultController;
    public PlayerController[] controllers;

    public static void ActivateController(Controller controller) => singleton.ActivateController_Internal(controller);

    private void ActivateController_Internal(Controller controller)
    {
        int index = (int)controller;

        for (int i = 0; i < controllers.Length; i++)
        {
            controllers[i].SetActive(i == index);
        }
    }

}

public abstract class PlayerController : MonoBehaviour
{

    public abstract void SetActive(bool isActive);

}