using UnityEngine;
using UnityEngine.SceneManagement;
public class GameSceneManager : MonoBehaviour
{
    public void ToTitleScene()
    {
        SceneManager.LoadScene("GameScene");
    }
}
