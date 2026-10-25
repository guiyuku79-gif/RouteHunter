using System.Collections;
using NUnit.Framework.Constraints;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TitleUIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI titleUI;
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject howToPlayButton;
    [SerializeField] GameObject returnTitleButton;
    [SerializeField] Camera camera;

    [SerializeField] GameObject Caption;
    [SerializeField] GameObject tools;
    [SerializeField] GameObject scores;
    [SerializeField] GameObject selectedSign;

    void Awake()
    {
        Caption.SetActive(false);
        tools.SetActive(false);
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
        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(0.05f);

            startButton.transform.position += Vector3.down * i * 0.1f;
            howToPlayButton.transform.position += Vector3.down * i * 0.1f;
            titleUI.transform.position += Vector3.up * i * 0.1f;
        }
    }

    public void ShowHowToPlay()
    {
        startButton.SetActive(false);
        howToPlayButton.SetActive(false);
        titleUI.enabled = false;

        StartCoroutine(ChangeCameraSize(5, 7, false));
    }

    IEnumerator ChangeCameraSize(float start, float end, bool showUI)
    {
        if (showUI)
        {
            Caption.SetActive(false);
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
            startButton.SetActive(true);
            howToPlayButton.SetActive(true);
            titleUI.enabled = true;

        }
        else
        {
            Caption.SetActive(true);
            tools.SetActive(true);
            scores.SetActive(true);
            selectedSign.SetActive(true);
        }
    }

    public void CloseHowToPlay()
    {
        StartCoroutine(ChangeCameraSize(7, 5, true));
    }


}
