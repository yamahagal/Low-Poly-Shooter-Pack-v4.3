using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
    public Shop shop;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenMenuScene() {
        SceneManager.LoadScene("Menu");
    }

    public void OpenGameScene() {
        SceneManager.LoadScene("Game");
    }

    public void OpenShopScene() {
        SceneManager.LoadScene("Shop");
    }
}
