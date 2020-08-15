using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    #region Singleton

    public static PlayerManager instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    public GameObject player;


    public static void RestartScene()
    {
        instance.StartCoroutine("SceneRestartDelay");
    }

    IEnumerator SceneRestartDelay()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
