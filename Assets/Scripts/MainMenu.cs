using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Amentia เกมเกๆล่าสุด"); // เปลี่ยนชื่อ Scene วันแรกตามที่คุณใช้จริง
    }

    public void ExitGame()
    {
        Debug.Log("Game Closed");
        Application.Quit();
    }
}
