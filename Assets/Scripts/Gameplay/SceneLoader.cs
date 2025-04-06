using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    
    public void GoToMenu(){
        SceneManager.LoadScene(0);
    }
    public void StartGame(){
        SceneManager.LoadScene(1);
    }
    
}
