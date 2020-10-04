using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;


[System.Serializable]
public class Action{
    public string name;
    public string description;
    public int currentLevel;
    public List<float> timeTaken = new List<float>();
    public List<float> moneyEarned = new List<float>();
    public List<float> spiritGained = new List<float>();
    public List<float> healthGained = new List<float>();
    public List<float> shipGained = new List<float>();
    public List<float> expReq = new List<float>();
    public List<GameObject> GO = new List<GameObject>();
    public List<Sprite> sprite = new List<Sprite>();
    public bool requireHealthAndHappiness;
    public AudioClip sfx;
}


public class HUD : MonoBehaviour
{
    public static HUD Instance {get;set;}

    [SerializeField] FirstPersonController fpc;

    public Text timeText, moneyText, spiritText, healthText, shipText;
    public CanvasGroup overlay, winUI, playAgainCG, noteCG, pauseCG, exitCG;
    [SerializeField] GameObject playAgainGO, HUDGO, pauseScreen, confirmExitScreen;
    [SerializeField] Text noteText;
    [SerializeField] AudioSource audioSource;
 
    float totalTime = 1440.0f;
    float startTime =  480.0f;
    float currentTime;
    float timeSpeed = 50.0f;
    float dailyExpenses = 50.0f;

    [HideInInspector] public float currentMoney = 0;
    float startingMoney = 50.0f;
    float startSpirit = 100.0f;
    float startHealth = 100.0f;
    float startShip = 100.0f;
    [HideInInspector] public float currentSpirit;
    [HideInInspector] public float timeLeftInDay;
    [HideInInspector] public float currentHealth;
    [HideInInspector] public float currentShip;

    [SerializeField] RectTransform happinessRect, healthRect, shipRect;
    float happinessWidth, healthWidth, shipWidth;
    float maxWidth = 150.0f;
    float rectHeight = 10.0f;

    float maxHealth=100.0f;
    
    float randomizationPercentage = 0.10f;

    public List<Action> allActions = new List<Action>();

    // DOTween notification;

    void Awake(){
        if(Instance==null){
            Instance=this;
        }
    }

    void Start(){
        StartNew();
    }

    void PausePlayer(){
        HideHUD();
        Static.paused=true;
        fpc.enabled=false;
    }
    void ResumePlayer(){
        ShowHUD();
        Static.paused=false;
        fpc.enabled=true;
    }

    public void PerformAction(int actionNum){

        PlaySFX(allActions[actionNum].sfx);
        PausePlayer();
        overlay.DOFade(1.0f, 1.0f).OnComplete(()=>{
            if(actionNum==10){
                WinGame();
            } else{
                currentTime += allActions[actionNum].timeTaken[allActions[actionNum].currentLevel]*60.0f;

                // RANDOM FACTOR
                float moneyAmount = allActions[actionNum].moneyEarned[allActions[actionNum].currentLevel];
                float percentage = moneyAmount*randomizationPercentage;
                float rand = Random.Range(-percentage, percentage);
                currentMoney += moneyAmount + rand;
                // currentMoney += allActions[actionNum].moneyEarned[allActions[actionNum].currentLevel];


                
                // HAPPINESS 
                float spiritAmount = allActions[actionNum].spiritGained[allActions[actionNum].currentLevel];
                float percentage2 = spiritAmount*randomizationPercentage;
                float rand2 = Random.Range(-percentage2, percentage2);
                currentSpirit +=  spiritAmount + rand2;
                // currentSpirit += allActions[actionNum].spiritGained[allActions[actionNum].currentLevel];
                if(currentSpirit>100.0f){
                    currentSpirit=100.0f;
                }
                if(currentSpirit<0){
                    currentSpirit=0;
                }


                // HEALTH 
                float healthAmount = allActions[actionNum].healthGained[allActions[actionNum].currentLevel];
                float percentage3 = healthAmount*randomizationPercentage;
                float rand3 = Random.Range(-percentage3, percentage3);
                currentHealth += healthAmount+rand3;
                // currentHealth += allActions[actionNum].healthGained[allActions[actionNum].currentLevel];
                
                if(currentHealth>100.0f){
                    currentHealth=100.0f;
                }
                if(currentHealth<0){
                    currentHealth=0;
                }


                // SHIP
                
                // HEALTH 
                float shipAmount = allActions[actionNum].shipGained[allActions[actionNum].currentLevel];
                float percentage4 = shipAmount*randomizationPercentage;
                float rand4 = Random.Range(-percentage4, percentage4);
                currentShip += shipAmount+rand4;
                // currentShip += allActions[actionNum].shipGained[allActions[actionNum].currentLevel];
                if(currentShip>100.0f){
                    currentShip=100.0f;
                }
                
                if(currentShip<0){
                    currentShip=0;
                }

                Static.paused=false;
                UpdateHUD();
                ResumePlayer();
                overlay.DOFade(0.0f, 1.0f);
            }
           
        });
    }

    public void Sleep(){
        PlaySFX(allActions[0].sfx);
        PausePlayer();
        overlay.DOFade(1.0f, 1.0f).OnComplete(()=>{
                currentTime = startTime;
                currentMoney -= dailyExpenses;
                Static.paused=false;
                UpdateHUD();
                ResumePlayer();
                overlay.DOFade(0.0f, 1.0f);
        });
    }

    public void UpdateHUD(){
        if((currentTime/60.0f)<=11){
            timeText.text = (currentTime/60.0f).ToString() + " AM";
        } else if((currentTime/60.0f)==12) {
            timeText.text = (currentTime/60.0f).ToString() + " PM";
        } else if((currentTime/60.0f)==24) {
            timeText.text = ((currentTime/60.0f)-(12.0f)).ToString() + " AM";
        } else {
            timeText.text = ((currentTime/60.0f)-(12.0f)).ToString() + " PM";
        }

        happinessRect.sizeDelta = new Vector2((currentSpirit/maxHealth)*maxWidth, rectHeight);
        healthRect.sizeDelta = new Vector2((currentHealth/maxHealth)*maxWidth, rectHeight);
        shipRect.sizeDelta = new Vector2((currentShip/maxHealth)*maxWidth, rectHeight);

        moneyText.text = "$" + currentMoney.ToString("F0");
        spiritText.text = currentSpirit.ToString("F0") + "%";
        healthText.text = currentHealth.ToString("F0") + "%";
        shipText.text = currentShip.ToString("F0") + "%";
        timeLeftInDay = totalTime-currentTime;
    }


    public void PlayAgain(){
        winUI.DOFade(0.0f, 0.5f);
        playAgainCG.DOFade(0.0f, 0.5f).OnComplete(()=>{
            SceneManager.LoadScene("Title");
        });
    }

    public void SwitchToMouse(){
        PausePlayer();
        Cursor.lockState=CursorLockMode.None;
        Cursor.visible=true;
    }

    public void SwitchToPlayer(){
        ResumePlayer();
        Cursor.lockState=CursorLockMode.Locked;
        Cursor.visible=false;

    }

    void StartNew(){
        currentSpirit = startSpirit;
        currentHealth = startHealth;
        currentShip = startShip;
        currentMoney = startingMoney;
        currentTime = startTime;
        timeLeftInDay = totalTime-currentTime;

        happinessRect.sizeDelta = new Vector2(maxWidth, rectHeight);
        healthRect.sizeDelta = new Vector2(maxWidth, rectHeight);
        shipRect.sizeDelta = new Vector2(maxWidth, rectHeight);


        UpdateHUD();
        SwitchToPlayer();
        overlay.DOFade(0.0f,1.0f);
    }

    void HideHUD(){
        HUDGO.SetActive(false);
    }

    void ShowHUD(){
        HUDGO.SetActive(true);
    }

    void WinGame(){
        Static.paused=true;
        winUI.DOFade(1.0f, 1.0f).OnComplete(()=>{
            SwitchToMouse();
            playAgainGO.SetActive(true);
            playAgainCG.DOFade(1.0f, 1.0f);
        });
    }

    // void ShowNotification(string text){
    //     noteText.text = text;
    //     Sequence statchange = 
    //     noteCG.DOFade(1.0f, 1.0f);
    // }


    public void PauseGame(){
        pauseScreen.SetActive(true);
        SwitchToMouse();
    
    }

    public void ResumeGame(){
        SwitchToPlayer();
        pauseScreen.SetActive(false);
        confirmExitScreen.SetActive(false);
    }

    public void ConfirmExit(){
        pauseScreen.SetActive(false);
        confirmExitScreen.SetActive(true);

    }

    public void Quit(){
        pauseScreen.SetActive(false);
        confirmExitScreen.SetActive(false);
        overlay.DOFade(1.0f, 1.0f).OnComplete(()=>{
            SceneManager.LoadScene("Title");
        });
    }


    void PlaySFX(AudioClip sfx){
        audioSource.clip = sfx;
        audioSource.Play();
    }

}
