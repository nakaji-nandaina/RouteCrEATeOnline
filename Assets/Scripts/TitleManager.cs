using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public static TitleManager Instance;
    public Button MatchButton;
    public GameObject HelpPanel;
    public Button OpenHelpButton;
    public Button CloseHelpButton;
    public Button HelpNextButton;
    public Button HelpPreviousButton;
    public Image CurrentHelpImage;
    public List<Sprite> HelpSprites;
    private int CurrentHelpPage;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    private void Start()
    {
        MatchButton.onClick.AddListener(() => {ToBattleScene(); }) ;
        OpenHelpButton.onClick.AddListener(() => { OpenHelp(); });
        CloseHelpButton.onClick.AddListener(() => { CloseHelp(); });
        HelpNextButton.onClick.AddListener(() => { HelpNext(); });
        HelpPreviousButton.onClick.AddListener(()=> { HelpPrevious(); });
        CurrentHelpPage = 0;
    }
    public void ToBattleScene()
    {
        SceneManager.LoadScene("Rule1");
    }
    public void OpenHelp()
    {
        CurrentHelpPage = 0;
        CurrentHelpImage.sprite = HelpSprites[CurrentHelpPage];
        HelpPanel.SetActive(true);
    }
    public void CloseHelp()
    {
        HelpPanel.SetActive(false);
    }
    public void HelpNext()
    {
        CurrentHelpPage = (CurrentHelpPage + 1) % HelpSprites.Count;
        CurrentHelpImage.sprite= HelpSprites[CurrentHelpPage];
    }
    public void HelpPrevious()
    {
        CurrentHelpPage = (CurrentHelpPage - 1+ HelpSprites.Count) % HelpSprites.Count;
        CurrentHelpImage.sprite = HelpSprites[CurrentHelpPage];
    }

}
