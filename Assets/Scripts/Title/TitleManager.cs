using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public void ToGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }
}
