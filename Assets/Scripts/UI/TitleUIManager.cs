using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI titleUI;
    [SerializeField] Button gameStartButton;
    [SerializeField] Button howToPlayButton;
    [SerializeField] Button returnTitleButton;
    [SerializeField] Camera camera;

    [SerializeField] GameObject Caption;
    [SerializeField] GameObject tools;
    [SerializeField] GameObject scores;
    [SerializeField] GameObject selectedSign;

    void Awake()
    {
        Caption.SetActive(false);
        tools.SetActive(false);
        returnTitleButton.gameObject.SetActive(false);
        scores.SetActive(false);
        selectedSign.SetActive(false);
    }

    public void RemoveTitleUI()
    {
        tools.SetActive(true);
        scores.SetActive(true);
        selectedSign.SetActive(true);
        StartCoroutine(Remove());
    }
    IEnumerator Remove()
    {
        gameStartButton.interactable = false;
        howToPlayButton.interactable = false;
        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(0.05f);

            gameStartButton.transform.position += Vector3.down * i * 0.1f;
            howToPlayButton.transform.position += Vector3.down * i * 0.1f;
            titleUI.transform.position += Vector3.up * i * 0.1f;
        }
    }

    public void ShowHowToPlay()
    {
        gameStartButton.gameObject.SetActive(false);
        howToPlayButton.gameObject.SetActive(false);
        titleUI.enabled = false;

        StartCoroutine(ChangeCameraSize(5, 7.5f, false));
    }

    IEnumerator ChangeCameraSize(float start, float end, bool showUI)
    {
        if (showUI)
        {
            Caption.SetActive(false);
            returnTitleButton.gameObject.SetActive(false);
            tools.SetActive(false);
            scores.SetActive(false);
            selectedSign.SetActive(false);
        }

        for (int i = 1; i < 21; i++)
        {
            yield return new WaitForSeconds(0.03f);

            camera.orthographicSize = start + (end - start) * i / 20;
        }

        if (showUI)
        {
            gameStartButton.gameObject.SetActive(true);
            howToPlayButton.gameObject.SetActive(true);
            titleUI.enabled = true;

        }
        else
        {
            returnTitleButton.gameObject.SetActive(true);
            Caption.SetActive(true);
            tools.SetActive(true);
            scores.SetActive(true);
            selectedSign.SetActive(true);
        }
    }

    public void CloseHowToPlay()
    {
        StartCoroutine(ChangeCameraSize(7.5f, 5, true));
    }


}
