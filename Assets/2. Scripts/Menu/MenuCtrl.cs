using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCtrl : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadGame()
    {

    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}
