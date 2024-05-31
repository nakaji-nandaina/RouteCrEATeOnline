using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Photon;
using UnityFx.Outline;
using UnityChan;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameManager Instance;
    public int[] CardArray;

    [SerializeField]
    public List<CardHolder> cardHolders;
    public List<Texture> textures;
    public List<ChoosedCard> ChooseCards;
    public int[] degs;
    public List<List<char>> RootMap;
    public List<string> StraightRoot;
    public List<string> CurveRoot;
    public List<string> StopRoot;
    public List<string> CrossRoot;
    public TextMeshProUGUI RouteMaptext;

    public bool IsRotate;
    public int nowturn=0;
    public int PlayerId;
    public GameObject WinPanel;
    public GameObject TurnUI;
    public GameObject MatchingText;
    public Text playerText;
    public AudioClip[] clips;
    public AudioSource audio;

    bool over = false;
    int[] dx = { 0, 1, 0, -1 };
    int[] dy = { 1, 0, -1, 0 };
    public int Goaled;

    public float prestartTime;

    float sendcount;
    float sendtime = 1f;

    public GameObject UnityChanObj;

    public Button TitleButton;

    public void LoadTitle()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Title");
    }

    public void DisConnectOther()
    {
        PhotonNetwork.Disconnect();
        MatchingText.GetComponent<Text>().text = "Disconnected";
        MatchingText.SetActive(true);
        TitleButton.gameObject.SetActive(true);
    }

    void Awake()
    {
        //Instance = this;
    }
    void Start()
    {
        Instance = this;
        TitleButton.onClick.AddListener(()=> { LoadTitle();});
        audio=this.gameObject.AddComponent<AudioSource>();
        sendtime = 1f;
        sendcount = 0f;
    }
    void Update()
    {
        sendcount += Time.deltaTime;
        if (PhotonNetwork.PlayerList.Length < 2&&Goaled==-1)
        {
            MatchingText.SetActive(true);
        }
        else
        {
            MatchingText.SetActive(false);
        }
        /*
        if (this.GetComponent<PhotonView>().IsMine)
        {
            this.GetComponent<PhotonView>().RPC(nameof(SyncGame),RpcTarget.Others,nowturn,CardArray,degs,Goaled);
        }
        */
    }

    void SyncGame(int nt,int[] ca,int[] dg,int G)
    {
        nowturn = nt;
        CardArray = ca;
        degs = dg;
        Goaled = G;
        UpdateGame();
        return;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //throw new System.NotImplementedException();
        
        if (stream.IsWriting)
        {
            //Debug.Log("here");
            if (sendcount < sendtime) return;
            sendcount = 0;
            stream.SendNext(nowturn);
            stream.SendNext(CardArray);
            stream.SendNext(degs);
            stream.SendNext(Goaled);

        }
        else
        {
            //Debug.Log("Received");
            nowturn = (int)stream.ReceiveNext();
            CardArray = (int[])stream.ReceiveNext();
            degs = (int[])stream.ReceiveNext();
            Goaled = (int)stream.ReceiveNext();
            UpdateGame();
        }
        
    }

    public void initGame()
    {
        TurnUI.GetComponent<Text>().text = "Turn: 1";
        Goaled = -1;
        nowturn = 0;
        ChooseCards = new List<ChoosedCard>();
        ChooseCards.Add(new ChoosedCard());
        ChooseCards.Add(new ChoosedCard());
        ChooseCards[0].point[0] = -1; ChooseCards[0].point[1] = -1;
        ChooseCards[1].point[0] = -1; ChooseCards[1].point[1] = -1;
        IsRotate = false;
        degs = new int[64];
        CardArray = new int[64];
        int crossnum = 16;
        int stnum = 18;
        int curvenum = 20;
        int stopnum = 10;
        int nowind = 0;
        for (int i = 0; i < crossnum; i++)
        {
            CardArray[nowind] = 0;
            nowind++;
        }
        for (int i = 0; i < stnum; i++)
        {
            CardArray[nowind] = 1;
            nowind++;
        }
        for (int i = 0; i < curvenum; i++)
        {
            CardArray[nowind] = 2;
            nowind++;
        }
        for (int i = 0; i < stopnum; i++)
        {
            CardArray[nowind] = 3;
            nowind++;
        }
        SetRootCard();
        RootMap = new List<List<char>>();
        for (int i = 0; i < 24; i++)
        {
            //Debug.LogError(i);
            RootMap.Add(new List<char>() { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' });
        }
        CardArray = CardArray.OrderBy(x => System.Guid.NewGuid()).ToArray();
        CardArray[0] = 0;CardArray[7] = 0;CardArray[63] = 0;CardArray[56] = 0;
        MakeGame();
    }
    
    public void SetRootCard()
    {
        StraightRoot=new List<string>();
        CurveRoot=new List<string>();
        StopRoot=new List<string>();
        CrossRoot=new List<string>();
        StraightRoot.Add("#.#");
        StraightRoot.Add("#.#");
        StraightRoot.Add("#.#");

        CurveRoot.Add("#.#");
        CurveRoot.Add("..#");
        CurveRoot.Add("###");
  
        StopRoot.Add("#.#");
        StopRoot.Add("#.#");
        StopRoot.Add("###");
        
        CrossRoot.Add("#.#");
        CrossRoot.Add("...");
        CrossRoot.Add("#.#");
    }

    public int CanChoose(int y,int x)
    {
        if (IsRotate) return -1;
        if (ChooseCards[0].point[0] == y && ChooseCards[0].point[1] == x) return 0;
        if (ChooseCards[1].point[0] == y && ChooseCards[1].point[1] == x) return 1;
        if (ChooseCards[0].point[0] == -1 && ChooseCards[0].point[1] == -1) return 0;
        if (ChooseCards[1].point[0] == -1 && ChooseCards[1].point[1] == -1) return 1;
        return -1;
    }
    public bool CanRotate()
    {
        if (ChooseCards[1].point[0] == -1) return true;
        return false;
    }

    public void ChooseCard(int ind,int y, int x)
    {
        ChooseCards[ind].point[0] = y;
        ChooseCards[ind].point[1] = x;
        //Debug.LogError("変更"+ChooseCards[ind].point[0].ToString()+" "+ChooseCards[ind].point[1].ToString());
    }
    void MakeGame()
    {
        for (int i = 0; i < 8; i++)
        {
            for(int j=0; j < 8; j++)
            {
                if((i==0&&j==0)|| (i == 0 && j == 7)|| (i == 7 && j == 0)|| (i == 7 && j ==7))
                {
                    CardPointController cpc= cardHolders[i].CardPoints[j].GetComponent<CardPointController>();
                    cpc.Cardinit(textures[0], 0,i,j,true);
                    SetRoot(0, 0, i, j);
                    continue;
                }
                CardPointController cpc2 = cardHolders[i].CardPoints[j].GetComponent<CardPointController>();
                int deg=Random.Range(0, 4);
                degs[i*8+j] = deg;
                SetRoot(CardArray[i * 8 + j], deg, i, j);
                cpc2.Cardinit(textures[CardArray[i*8+j]], deg, i, j, false);
            }
        }
        UIforMap();
    }

    void UIforMap()
    {
        RouteMaptext.text = "";
        for (int i = 0; i < 24; i++)
        {
            string s = "";
            for (int j = 0; j < 24; j++)
            {
                s = s + RootMap[i][j].ToString();
            }
            RouteMaptext.text += s + "\r\n";
        }
    }

    void SetRoot(int card,int deg,int _y,int _x)
    {
        List<string> newRoot = new List<string>();
        switch (card)
        {
            case 0:
                newRoot.Add(CrossRoot[0]);
                newRoot.Add(CrossRoot[1]);
                newRoot.Add(CrossRoot[2]);
                break;
            case 1:
                newRoot.Add(StraightRoot[0]);
                newRoot.Add(StraightRoot[1]);
                newRoot.Add(StraightRoot[2]);
                break;
            case 2:
                newRoot.Add(CurveRoot[0]);
                newRoot.Add(CurveRoot[1]);
                newRoot.Add(CurveRoot[2]);
                break;
            case 3:
                newRoot.Add(StopRoot[0]);
                newRoot.Add(StopRoot[1]);
                newRoot.Add(StopRoot[2]);
                break;
        }
        for(int r = 0; r < deg; r++)
        {
            List<List<char>> nextRoot=new List<List<char>>();
            nextRoot.Add(new List<char>() { '#', '#', '#' });
            nextRoot.Add(new List<char>() { '#', '#', '#' });
            nextRoot.Add(new List<char>() { '#', '#', '#' });
            for (int i = 0; i < 3; i++)for(int j = 0; j < 3; j++)
            {
                nextRoot[i][j]=newRoot[2-j][i];
            }
            for(int i=0; i<3;i++)
            {
                newRoot[i] = nextRoot[i][0].ToString() + nextRoot[i][1].ToString() + nextRoot[i][2].ToString();
            }
        }
        for (int i = 0; i < 3; i++) for (int j = 0; j < 3; j++) RootMap[3 * _y + i][3 * _x + j] = newRoot[i][j];
    }

    bool GoalCheck(int _y, int _x)
    {
        int[,] _ok = new int[24,24];
        Queue<int> qy= new Queue<int>(); 
        Queue<int> qx= new Queue<int>();
        qy.Enqueue(_y);qx.Enqueue(_x);
        _ok[_y, _x]= 1;
        while (qy.Count != 0)
        {
            int cy = qy.Dequeue();
            int cx = qx.Dequeue();
            for(int i = 0; i < 4; i++)
            {
                int ny = cy + dy[i];
                int nx = cx + dx[i];
                if (nx < 0 || nx > 23 || ny < 0 || ny > 23) continue;
                if (RootMap[ny][nx] == '#') continue;
                if (_ok[ny,nx] == 1) continue;
                qy.Enqueue(ny); qx.Enqueue(nx);
                _ok[ny, nx] = 1;
                if (ny == (23 - _y) && nx == (23 - _x)) return true;
            }
        }
        return _ok[23-_y,23-_x]==1;
    }

    List<int> GetGoalRoot(int _y, int _x)
    {
        int[,] distance = new int[24, 24];
        for (int i = 0; i < 24; i++) for (int j = 0; j < 24; j++) distance[i, j] = 10000;
        Queue<int> qy = new Queue<int>();
        Queue<int> qx = new Queue<int>();
        qy.Enqueue(_y); qx.Enqueue(_x);
        distance[_y, _x] = 0;
        while (qy.Count != 0)
        {
            int cy = qy.Dequeue();
            int cx = qx.Dequeue();
            for (int i = 0; i < 4; i++)
            {
                int ny = cy + dy[i];
                int nx = cx + dx[i];
                if (nx < 0 || nx > 23 || ny < 0 || ny > 23) continue;
                if (RootMap[ny][nx] == '#') continue;
                if (distance[ny, nx] <= distance[cy,cx]+1) continue;
                qy.Enqueue(ny); qx.Enqueue(nx);
                distance[ny, nx] = distance[cy, cx] +1;
            }
        }
        string log="";
        for(int i = 0; i < 24; i++)
        {
            for (int j = 0; j < 24; j++) log += distance[i,j].ToString() + " ";
            log += "\n";
        }
        //Debug.LogError(log);
        List<int> rRoot=new List<int>();
        Queue<int> ry = new Queue<int>();
        Queue<int> rx = new Queue<int>();
        ry.Enqueue(23 - _y); rx.Enqueue(23 - _x);
        rRoot.Add((23 - _y) / 3 * 8 + (23 - _x) / 3);
        while (ry.Count != 0)
        {
            int cy = ry.Dequeue();
            int cx = rx.Dequeue();
            //Debug.LogError("通った道"+cy.ToString()+" "+cx.ToString());
            for (int i = 0; i < 4; i++)
            {
                int ny = cy + dy[i];
                int nx = cx + dx[i];
                if (nx < 0 || nx > 23 || ny < 0 || ny > 23) continue;
                if (RootMap[ny][nx] == '#') continue;
                if (distance[ny, nx] >= distance[cy, cx] ) continue;
                ry.Enqueue(ny); rx.Enqueue(nx);
                if (ny % 3 == 1 && nx % 3 == 1) rRoot.Add(ny / 3*8+nx/3);
                break;
            }
        }
        List<int> Root=new List<int>();
        //Debug.LogError("ルートサイズ" + rRoot.Count.ToString());
        string s = "";
        for(int i = rRoot.Count - 1; i >= 0; i--)
        {
            Root.Add(rRoot[i]);
            s =s+ rRoot[i].ToString() + " ";
            cardHolders[rRoot[i] / 8].CardPoints[rRoot[i] % 8].GetComponent<OutlineBehaviour>().enabled=true;
            cardHolders[rRoot[i] / 8].CardPoints[rRoot[i] % 8].GetComponent<OutlineBehaviour>().OutlineColor=Color.white;
            cardHolders[rRoot[i] / 8].CardPoints[rRoot[i] % 8].GetComponent<OutlineBehaviour>().OutlineWidth = 5;
            cardHolders[rRoot[i] / 8].CardPoints[rRoot[i] % 8].GetComponent<OutlineBehaviour>().OutlineRenderMode = OutlineRenderFlags.EnableDepthTesting;
            //cardHolders[rRoot[i] / 8].CardPoints[rRoot[i] % 8].GetComponent<OutlineBehaviour>().OutlineRenderMode = OutlineRenderFlags.EnableAlphaTesting;
            //cardHolders[rRoot[i] / 8].CardPoints[rRoot[i] % 8].GetComponent<OutlineBehaviour>().

        }
        //Debug.LogError(s);
        return Root;
    }

    public void ResetSelectedCard()
    {
        if (nowturn % 2 != PlayerId) return;
        if (ChooseCards[0].point[0]!=-1)
        {
            CardPointController cpc = cardHolders[ChooseCards[0].point[0]].CardPoints[ChooseCards[0].point[1]].GetComponent<CardPointController>();
            cpc.Cardinit(textures[CardArray[ChooseCards[0].point[0] * 8 + ChooseCards[0].point[1]]], degs[ChooseCards[0].point[0] * 8 + ChooseCards[0].point[1]], ChooseCards[0].point[0], ChooseCards[0].point[1], false);
        }
        if (ChooseCards[1].point[0] != -1)
        {
            CardPointController cpc = cardHolders[ChooseCards[1].point[0]].CardPoints[ChooseCards[1].point[1]].GetComponent<CardPointController>();
            cpc.Cardinit(textures[CardArray[ChooseCards[1].point[0] * 8 + ChooseCards[1].point[1]]], degs[ChooseCards[1].point[0] * 8 + ChooseCards[1].point[1]], ChooseCards[1].point[0], ChooseCards[1].point[1], false);
        }
        IsRotate = false;
        ChooseCard(0, -1, -1);
        ChooseCard(1, -1, -1);
    }
    public void ChangeGameTurnOver()
    {
        //Debug.LogError((PhotonNetwork.LocalPlayer.ActorNumber - 1).ToString()+" P");
        if (nowturn % 2 != PlayerId) return;
        if (IsRotate)
        {
            CardPointController cpc = cardHolders[ChooseCards[0].point[0]].CardPoints[ChooseCards[0].point[1]].GetComponent<CardPointController>();
            degs[ChooseCards[0].point[0] * 8 + ChooseCards[0].point[1]] = cpc.dir;
            SetRoot(CardArray[ChooseCards[0].point[0] * 8 + ChooseCards[0].point[1]], degs[ChooseCards[0].point[0] * 8 + ChooseCards[0].point[1]], ChooseCards[0].point[0], ChooseCards[0].point[1]);
            cpc.ChangeCS(CardPointController.CardState.Lock);
        }
        else if (CanChoose(-2, -2) == -1)
        {
            //カード入れ替え処理
            int tcard = CardArray[ChooseCards[0].point[0] * 8 + ChooseCards[0].point[1]];
            CardArray[ChooseCards[0].point[0] * 8 + ChooseCards[0].point[1]] = CardArray[ChooseCards[1].point[0] * 8 + ChooseCards[1].point[1]];
            CardArray[ChooseCards[1].point[0] * 8 + ChooseCards[1].point[1]] = tcard;
            tcard = degs[ChooseCards[1].point[0] * 8 + ChooseCards[1].point[1]];
            degs[ChooseCards[1].point[0] * 8 + ChooseCards[1].point[1]] = degs[ChooseCards[0].point[0] * 8 + ChooseCards[0].point[1]];
            degs[ChooseCards[0].point[0] * 8 + ChooseCards[0].point[1]] = tcard;
            SetRoot(CardArray[ChooseCards[0].point[0] * 8 + ChooseCards[0].point[1]], degs[ChooseCards[0].point[0] * 8 + ChooseCards[0].point[1]], ChooseCards[0].point[0], ChooseCards[0].point[1]);
            SetRoot(CardArray[ChooseCards[1].point[0] * 8 + ChooseCards[1].point[1]], degs[ChooseCards[1].point[0] * 8 + ChooseCards[1].point[1]], ChooseCards[1].point[0], ChooseCards[1].point[1]);
            CardPointController cpcf = cardHolders[ChooseCards[0].point[0]].CardPoints[ChooseCards[0].point[1]].GetComponent<CardPointController>();
            CardPointController cpcs = cardHolders[ChooseCards[1].point[0]].CardPoints[ChooseCards[1].point[1]].GetComponent<CardPointController>();
            cpcf.Cardinit(textures[CardArray[ChooseCards[0].point[0] * 8 + ChooseCards[0].point[1]]], degs[ChooseCards[0].point[0] * 8 + ChooseCards[0].point[1]], ChooseCards[0].point[0], ChooseCards[0].point[1], true);
            cpcs.Cardinit(textures[CardArray[ChooseCards[1].point[0] * 8 + ChooseCards[1].point[1]]], degs[ChooseCards[1].point[0] * 8 + ChooseCards[1].point[1]], ChooseCards[1].point[0], ChooseCards[1].point[1], true);
        }
        else return;
        UIforMap();
        sendcount = 0;
        ChooseCard(0, -1, -1);
        ChooseCard(1, -1, -1);
        IsRotate = false;
        nowturn+=1;
        //for (int i = 0; i < 8; i++) for (int j = 0; j < 8; j++) cardHolders[i].CardPoints[j].GetComponent<BoxCollider>().enabled = false;
        switch (PlayerId)
        {
            case 0:
                if (GoalCheck(1, 1))
                {
                    Goaled = 0;
                    WinPanel.SetActive(true);
                    WinPanel.GetComponent<Text>().text = "Player1 Win!!";
                    UnityChanObj.GetComponent<UnityChanControlScriptWithRgidBody>().WalkToGoal(GetGoalRoot(1, 1));
                    Debug.Log("ゴール！");
                }
                break;
            case 1:
                if (GoalCheck(1, 22))
                {
                    Goaled = 1;
                    WinPanel.SetActive(true);
                    WinPanel.GetComponent<Text>().text = "Player2 Win!!";
                    UnityChanObj.GetComponent<UnityChanControlScriptWithRgidBody>().WalkToGoal(GetGoalRoot(1, 22));
                    Debug.Log("ゴール！");
                }
                break;
        }
        
        Debug.Log("ターンオーバー");
    }

    public void UpdateGame()
    {
        if (CardArray.Length != 64) return;
        if (degs.Length != 64) return;
        ChooseCards = new List<ChoosedCard>();
        ChooseCards.Add(new ChoosedCard());
        ChooseCards.Add(new ChoosedCard());
        ChooseCards[0].point[0] = -1; ChooseCards[0].point[1] = -1;
        ChooseCards[1].point[0] = -1; ChooseCards[1].point[1] = -1;
        IsRotate = false;
        SetRootCard();
        TurnUI.GetComponent<Text>().text = "Turn: " + (nowturn+1).ToString();
        RootMap = new List<List<char>>();
        for (int i = 0; i < 24; i++)
        {
            //Debug.LogError(i);
            RootMap.Add(new List<char>() { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' });
        }
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                
                CardPointController cpc = cardHolders[i].CardPoints[j].GetComponent<CardPointController>();
                int deg = degs[i*8+j];
                SetRoot(CardArray[i * 8 + j], deg, i, j);
                cpc.Cardinit(textures[CardArray[i * 8 + j]], deg, i, j, cpc.Locked());
            }
        }
        UIforMap();
        if (over) return;
        switch (Goaled)
        {
            case -1:
                break;
            case 0:
                WinPanel.SetActive(true);
                WinPanel.GetComponent<Text>().text = "Player1 Win!!";
                over = true;
                GetGoalRoot(1, 1);
                return;
            case 1:
                WinPanel.SetActive(true);
                WinPanel.GetComponent<Text>().text = "Player2 Win!!";
                over = true;
                GetGoalRoot(1, 22);
                return;
        }
        if (nowturn % 2 != PlayerId) return;
        for (int i = 0; i < 8; i++) for (int j = 0; j < 8; j++)
        {
            cardHolders[i].CardPoints[j].GetComponent<PhotonView>().RequestOwnership();
            //cardHolders[i].CardPoints[j].GetComponent<BoxCollider>().enabled = true;
        }
        this.GetComponent<PhotonView>().RequestOwnership();
    }

    
}
[System.Serializable]
public class CardHolder
{
    public List<GameObject> CardPoints;
}
public class ChoosedCard 
{
    public int[] point = new int[2];
}


