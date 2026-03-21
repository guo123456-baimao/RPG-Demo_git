using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneName = "MainScene";
    [SerializeField] private GameObject continueButton;
    [SerializeField] private UI_FadeScreen fadeScreen;
    [SerializeField] private AudioManager audioManager;

    private void Start()
    {
        if (SaveManager.instance.HasSaveData() == false)
            continueButton.SetActive(false);    
    }

    public void ContinueGame()
    {
        StartCoroutine(LoadSceneWithFadeEffect(1.5f));
    }

    public void NewGame()
    {
        SaveManager.instance.DeleteSaveData();
        StartCoroutine(LoadSceneWithFadeEffect(1.5f));
        
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
          Application.Quit();
#endif
    }


    IEnumerator LoadSceneWithFadeEffect(float _delay)
    {
        fadeScreen.FadeOut();                                     //ºÚ²¼×ªºÚ

        yield return new WaitForSeconds(_delay);

        SceneManager.LoadScene(sceneName);                       //ÇÐ»»³¡¾°
    }


    public void PlaySFX()
    {
        audioManager.PlaySFX(7, null);
    }


}
