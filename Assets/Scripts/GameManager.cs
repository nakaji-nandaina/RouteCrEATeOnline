using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Photon;

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

    int[] dx = { 0, 1, 0, -1 };
    int[] dy = { 1, 0, -1, 0 };


    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        
    }


    public void initGame()
    {
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
            Debug.LogError(i);
            RootMap.Add(new List<char>() { '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#', '#' });
        }
        CardArray = CardArray.OrderBy(x => System.Guid.NewGuid()).ToArray();
        MakeGame();
    }
    // Update is called once per frame
    void Update()
    {
        
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
        qy.Enqueue(1);qx.Enqueue(1);
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

    public void ChangeGameTurnOver()
    {
        Debug.LogError((PhotonNetwork.LocalPlayer.ActorNumber - 1).ToString()+" P");
        if (nowturn % 2 != PhotonNetwork.LocalPlayer.ActorNumber-1) return;
        if (IsRotate)
        {
            CardPointController cpc = cardHolders[ChooseCards[0].point[0]].CardPoints[ChooseCards[0].point[1]].GetComponent<CardPointController>();
            degs[ChooseCards[0].point[0]*8+ChooseCards[0].point[1]] = cpc.dir;
            SetRoot(CardArray[ChooseCards[0].point[0] * 8 + ChooseCards[0].point[1]], degs[ChooseCards[0].point[0]*8+ChooseCards[0].point[1]], ChooseCards[0].point[0], ChooseCards[0].point[1]);
            cpc.ChangeCS(CardPointController.CardState.Lock);
        }
        else if (CanChoose(-2, -2) == -1)
        {
            //カード入れ替え処理
            int tcard = CardArray[ChooseCards[0].point[0] * 8 + ChooseCards[0].point[1]];
            CardArray[ChooseCards[0].point[0] * 8 + ChooseCards[0].point[1]] = CardArray[ChooseCards[1].point[0] * 8 + ChooseCards[1].point[1]];
            CardArray[ChooseCards[1].point[0] * 8 + ChooseCards[1].point[1]] = tcard;
            tcard = degs[ChooseCards[1].point[0]*8+ChooseCards[1].point[1]];
            degs[ChooseCards[1].point[0]*8+ChooseCards[1].point[1]] = degs[ChooseCards[0].point[0]*8+ChooseCards[0].point[1]];
            degs[ChooseCards[0].point[0]*8+ChooseCards[0].point[1]] = tcard;
            SetRoot(CardArray[ChooseCards[0].point[0] * 8 + ChooseCards[0].point[1]], degs[ChooseCards[0].point[0]*8+ChooseCards[0].point[1]], ChooseCards[0].point[0], ChooseCards[0].point[1]);
            SetRoot(CardArray[ChooseCards[1].point[0] * 8 + ChooseCards[1].point[1]], degs[ChooseCards[1].point[0]*8+ChooseCards[1].point[1]], ChooseCards[1].point[0], ChooseCards[1].point[1]);
            CardPointController cpcf = cardHolders[ChooseCards[0].point[0]].CardPoints[ChooseCards[0].point[1]].GetComponent<CardPointController>();
            CardPointController cpcs = cardHolders[ChooseCards[1].point[0]].CardPoints[ChooseCards[1].point[1]].GetComponent<CardPointController>();
            cpcf.Cardinit(textures[CardArray[ChooseCards[0].point[0] * 8 + ChooseCards[0].point[1]]], degs[ChooseCards[0].point[0]*8+ChooseCards[0].point[1]], ChooseCards[0].point[0], ChooseCards[0].point[1], true);
            cpcs.Cardinit(textures[CardArray[ChooseCards[1].point[0] * 8 + ChooseCards[1].point[1]]], degs[ChooseCards[1].point[0]*8+ChooseCards[1].point[1]], ChooseCards[1].point[0], ChooseCards[1].point[1], true);
        }
        UIforMap();
        ChooseCard(0, -1, -1);
        ChooseCard(1, -1, -1);
        IsRotate = false;
        nowturn+=1;
        if (GoalCheck(1,1)) Debug.Log("ゴール！");
        Debug.Log("ターンオーバー");
    }

    public void UpdateGame()
    {
        ChooseCards = new List<ChoosedCard>();
        ChooseCards.Add(new ChoosedCard());
        ChooseCards.Add(new ChoosedCard());
        ChooseCards[0].point[0] = -1; ChooseCards[0].point[1] = -1;
        ChooseCards[1].point[0] = -1; ChooseCards[1].point[1] = -1;
        IsRotate = false;
        SetRootCard();
        RootMap = new List<List<char>>();
        for (int i = 0; i < 24; i++)
        {
            Debug.LogError(i);
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
        if (nowturn % 2 != PlayerId) return;
        this.GetComponent<PhotonView>().RequestOwnership();
        for (int i = 0; i < 8; i++) for (int j = 0; j < 8; j++) cardHolders[i].CardPoints[j].GetComponent<PhotonView>().RequestOwnership();


    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //throw new System.NotImplementedException();
        if (stream.IsWriting)
        {
            Debug.Log("here");
            stream.SendNext(nowturn);
            stream.SendNext(CardArray);
            stream.SendNext(degs);
        }
        else
        {
            Debug.Log("Received");
            nowturn = (int)stream.ReceiveNext();
            CardArray = (int[])stream.ReceiveNext();
            degs = (int[])stream.ReceiveNext();
            UpdateGame();
        }
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


