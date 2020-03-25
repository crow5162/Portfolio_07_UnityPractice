using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMgr : MonoBehaviour
{
    public void OnClickStartButton()
    {
        Debug.Log("Click Start Button !");

        SceneManager.LoadScene("Play");

    }
}
