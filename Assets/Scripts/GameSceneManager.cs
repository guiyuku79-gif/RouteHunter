using UnityEngine;
using UnityEngine.SceneManagement;
public class GameSceneManager : MonoBehaviour
{
    public void ToGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ToTitleScene()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
