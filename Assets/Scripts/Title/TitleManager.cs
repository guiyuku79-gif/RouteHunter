using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class TitleManager : MonoBehaviour
{
    [SerializeField] Image howToPlay;

    bool isShowing = false;
    public void ToGameScene()
    {
        if (isShowing) return;
        SceneManager.LoadScene("GameScene");
    }

    private int showFrame;

    public void ShowHowToPlay()
    {
        if (isShowing) return;
        isShowing = true;
        howToPlay.enabled = true;

        showFrame = Time.frameCount;
    }

    void Update()
    {
        if (isShowing &&
            Time.frameCount > showFrame &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            isShowing = false;
            howToPlay.enabled = false;
        }
    }
}
