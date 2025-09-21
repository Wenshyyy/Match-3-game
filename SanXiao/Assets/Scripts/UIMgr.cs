using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMgr : Singleton<UIMgr>
{
    public GameObject pauseBar;
    public Text killCountText;
    public Text scoreText;
    public Text HarmInfo;
    //public Slider playerHpSlider;
    public AudioClip buttonCilp;

    public Stack<Image> heartsimage = new Stack<Image>();
    public Image heart1;
    public Image heart2;
    public Image heart3;

    public Sprite grayHeart;
    
   

    // Start is called before the first frame update
    void Start()
    {
        pauseBar.SetActive(false);

        heartsimage.Push(heart3);
        heartsimage.Push(heart2);
        heartsimage.Push(heart1);
    }
    public void LostHeart()
    {
        Image h = heartsimage.Pop();    
        h.sprite = grayHeart;

        if (heartsimage.Count == 0)
        {
            //激活游戏结束面板
            SceneManager.LoadScene(2);

        }
    }   

    private void OnEnable()
    {
        ResumeGame();
    }

    public void OnPauseButtonClick()
    {
        PauseGame();
    }

    private void PauseGame()
    {
        pauseBar.SetActive(true);
        Time.timeScale = 0f;
        PlayerMgr.Instance.isPause = true;
    }

    private void ResumeGame()
    {
        pauseBar.SetActive(false);
        Time.timeScale = 1f;
        PlayerMgr.Instance.isPause = false;
    }

    //按钮被点击播放音效
    public void OnClick()
    {
        AudioSource.PlayClipAtPoint(buttonCilp,transform.position);
    }

    #region button点击事件

   

    public void OnResumeButtonClick()
    {
        ResumeGame();
    }

    public void OnMenuClick()
    {        
        SceneManager.LoadScene("Menu");
        PlayerMgr.Instance.isPause = false;

    }
    #endregion
    public void UpdateKillCount(int num)
    {
        string info = "kill count : " + PlayerMgr.Instance.killCount;
        string scoreinfo = "score : " + PlayerMgr.Instance.score;
        killCountText.text = info;
        scoreText.text = scoreinfo;
    }
}
