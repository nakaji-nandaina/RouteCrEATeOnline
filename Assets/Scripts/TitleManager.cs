using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public static TitleManager Instance;
    public Button MatchButton;
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    private void Start()
    {
        MatchButton.onClick.AddListener(() => {ToBattleScene(); }) ;
    }
    public void ToBattleScene()
    {
        SceneManager.LoadScene("Rule1");
    }
}
