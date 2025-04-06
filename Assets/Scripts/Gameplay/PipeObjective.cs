using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PipeObjective : MonoBehaviour
{

    public static HashSet<PipeObjective> objectives = new HashSet<PipeObjective>();

    public Transform root, slideRoot, location;
    public Vector3 position => location.position;
    public PlayerInfoScriptableObject playerInfo;

    public float repairRange = 6;

    public HoldPressButton repairButton;
    
    public UnityEvent onStartRepair, onComplete;

    void OnEnable()
    {
        objectives.Add(this);
    }
    void OnDisable()
    {
        objectives.Remove(this);
    }

    void Start()
    {
        repairButton.SetActive(false);
    }

    public void Setup(Vector3 position, float scale)
    {
        root.position = position;
        switch (Random.Range(0, 3))
        {
            case 0: root.eulerAngles = Vector3.zero; break;
            case 1: root.eulerAngles = Vector3.up * 90; break;
            case 2: root.eulerAngles = Vector3.right * 90; break;
        }
        slideRoot.localPosition = Vector3.forward * Random.Range(0.25f, 0.5f) * scale * Mathf.Sign(Random.value - 0.5f);
        slideRoot.localEulerAngles = Vector3.forward * Random.Range(-180f, 180f);
    }

    public static PipeObjective GetClosestObjective(Vector3 position)
    {
        PipeObjective result = null;
        float distance = float.MaxValue;
        foreach (PipeObjective objective in objectives)
        {
            float d = Vector3.Distance(objective.position, position);
            if (d < distance)
            {
                distance = d;
                result = objective;
            }
        }
        return result;
    }

    public bool CheckIfPlayerInRange()
    {
        return Vector3.SqrMagnitude(playerInfo.position - location.position) <= repairRange * repairRange;
    }

    bool wasInRange;

    void FixedUpdate()
    {
        bool inRange = CheckIfPlayerInRange();

        if (inRange != wasInRange)
        {
            wasInRange = inRange;
            repairButton.SetActive(inRange);
        }
    }
    
    public void CompleteObjective(){
        onComplete.Invoke();
    }
    
    bool hasStarted = false;
    public void OnSetRepairing(bool isRepairing){
        if (isRepairing && !hasStarted){
            hasStarted = true;
            onStartRepair.Invoke();
        }
    }

}
