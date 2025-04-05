using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "DeepSeaPostProcessingMaterials", menuName = "Game/DeepSeaPostProcessingMaterials")]
public class DeepSeaPostProcessingMaterials : UnityEngine.ScriptableObject
{
    //---Your Materials---
    public Material customEffect;
    
    //---Accessing the data from the Pass---
    static DeepSeaPostProcessingMaterials _instance;

    public static DeepSeaPostProcessingMaterials Instance
    {
        get
        {
            if (_instance != null) return _instance;
            // TODO check if application is quitting
            // and avoid loading if that is the case
            
            _instance = UnityEngine.Resources.Load("DeepSeaPostProcessingMaterials") as DeepSeaPostProcessingMaterials;
            return _instance;
        }
    }
} 