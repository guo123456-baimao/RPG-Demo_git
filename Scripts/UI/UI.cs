using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour,ISaveManager
{
    [Header("End screen")] 
    [SerializeField] private UI_FadeScreen endScreen;
    [SerializeField] private GameObject endText;
    [SerializeField] private GameObject youWinText;
    [SerializeField] private GameObject restartButton;
    [Space]

    [Header("MainMenu")]
    [SerializeField] private string sceneName = "MainMenu";
    [SerializeField] private UI_FadeScreen fadeScreen;
    [Space]

    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skilltreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionsUI;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject comfirmationPage;

    public UI_skillToolTip skillToolTip;
    public UI_ItemToolTip itemToolTip;
    public UI_StatToolTip statToolTip;
    public UI_craftWindow craftWindow;

    [SerializeField] private UI_VolumeSlider[] volumeSettings;                              //slider数组

    private void Awake()
    {
        SwitchTo(inGameUI);
        comfirmationPage.SetActive(false);
        endScreen.gameObject.SetActive(true);
        
    }

   

    void Start()
    {
        SwitchTo(inGameUI);

        itemToolTip.gameObject.SetActive(false);
        statToolTip.gameObject.SetActive(false);
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwitchToCharaterUI();

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SwitchToSkilltreeUI();

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SwitchToCraftUI();

        if (Input.GetKeyDown(KeyCode.Alpha4))
            SwitchToOptionsUI();

    }

    public void SwitchToOptionsUI()
    {
        SaveManager.instance.SaveGame();
        SwitchWithKeyTo(optionsUI);
    }

    public void SwitchToSkilltreeUI()
    {
        SaveManager.instance.SaveGame();
        SwitchWithKeyTo(skilltreeUI);
    }

    public void SwitchToCraftUI()
    {
        SaveManager.instance.SaveGame();
        SwitchWithKeyTo(craftUI);
    }

    public void SwitchToCharaterUI()
    {
        SaveManager.instance.SaveGame();
        SwitchWithKeyTo(characterUI);
    }

    public void SwitchTo(GameObject _menu)
    {

        for (int i = 0; i < transform.childCount; i++)                                        //遍历当前UI子级，关闭子级
        {
            bool fadeScreen =transform.GetChild(i).GetComponent<UI_FadeScreen>() != null;   
            
            if(fadeScreen==false)                                                              //如果不是黑布的话就关闭
                transform.GetChild(i).gameObject.SetActive(false);
        }

        if(_menu != null)                                                                    //如果指定UI不为空，则打开指定UI
        {
            AudioManager.instance.PlaySFX(7, null);
            _menu.SetActive(true);
        }

        if (GameManager.instence != null)                                                        //打开UI面板，时停
        {
            if (_menu == inGameUI||_menu==null)
                GameManager.instence.PauseGame(false);
            else
                GameManager.instence.PauseGame(true);
        }

       
    }

    public void SwitchWithKeyTo(GameObject _menu)
    {
        if (_menu != null&&_menu.activeSelf)                   //如果当前指定UI处于打开状态
        {
            _menu.SetActive(false);                           //关闭
            SaveManager.instance.SaveGame();                  //打开UI面板时保存游戏   
            CheckForInGameUI();
            return;
        }

        SwitchTo(_menu); 
    }

    private void CheckForInGameUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf&&transform.GetChild(i).GetComponent<UI_FadeScreen>()==null)
                return;
        }

        SwitchTo(inGameUI);

    }


    public void SwitchOnEndScreen()
    {
        SwitchTo(null);
        endScreen.gameObject.SetActive(true);
        endScreen.FadeOut();
        StartCoroutine("EndScreenContinue");
    }

    public void SwitchYouWin()
    {
        SwitchTo(null);
        endScreen.gameObject.SetActive(true);
        endScreen.FadeOut();
        StartCoroutine("YouWinContinue");
    }

    IEnumerator YouWinContinue()
    {
        yield return new WaitForSeconds(1);
        youWinText.SetActive(true);
        yield return new WaitForSeconds(1);
    }

    IEnumerator EndScreenContinue()
    {
        yield return new WaitForSeconds(1);
        endText.SetActive(true);
        yield return new WaitForSeconds(1);
        restartButton.SetActive(true);

    }


    public void RestartGameButton()=>GameManager.instence.RestartScene();

    public void LoadData(GameData _data)
    {
        foreach(KeyValuePair<string,float> pair in _data.volumeSettings)               //遍历字典
        {
            foreach(UI_VolumeSlider item in volumeSettings)                            //遍历列表
            {
                if (item.parametr == pair.Key)                                         //若匹配成功
                {
                    item.LoadSlider(pair.Value);                                       //加载滑块值
                    StartCoroutine(DelaySync(item, pair.Value));                       //主动调用SliderValue，修改AudioMixer的暴露参数
                }
            }
        }
       
    }

    public void SaveData(ref GameData _data)
    {
        _data.volumeSettings.Clear();                                         //清空字典《暴露参数名，slider value》

        foreach (UI_VolumeSlider item in volumeSettings)                          //遍历slider列表
        {
            _data.volumeSettings.Add(item.parametr, item.slider.value);            //添加进字典
        }
    }

    private IEnumerator DelaySync(UI_VolumeSlider slider, float value)
    {
        yield return null; // 等待一帧
        slider.SliderValue(value); // 此时Slider已初始化
    }


    public void SaveAndExit()                                  //SaveAndExit按钮订阅
    {
        SaveManager.instance.SaveGame();
        SwitchWithKeyTo(optionsUI);
    }


    public void OpenComfirmationPage()                            //打开确认页面
    {
        AudioManager.instance.PlaySFX(7, null);
        comfirmationPage.SetActive(true);
        GameManager.instence.PauseGame(true);
    }


    public void CloseComfirmationPage()                           //关闭确认页面
    {
        AudioManager.instance.PlaySFX(7, null);
        comfirmationPage.SetActive(false);
        GameManager.instence.PauseGame(false);
    }



    public void ExitGame()                                                    //quit页面yes按钮订阅，保存游戏后退出
    {
        SaveManager.instance.SaveGame();
        CloseComfirmationPage();
        StartCoroutine(LoadSceneWithFadeEffect(1.5f));
#if UNITY_EDITOR
        //UnityEditor.EditorApplication.isPlaying = false;       // 在编辑器中停止游戏
#else
        StartCoroutine(LoadSceneWithFadeEffect(1.5f));
        //Application.Quit();                                    // 在构建的应用程序中退出游戏
#endif
    }

    IEnumerator LoadSceneWithFadeEffect(float _delay)
    {
        fadeScreen.FadeOut();                                     //黑布转黑

        yield return new WaitForSeconds(_delay);

        SceneManager.LoadScene(sceneName);                       //切换场景
    }


}
