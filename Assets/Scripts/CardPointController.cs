using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;
using Photon;

public class CardPointController : MonoBehaviourPunCallbacks, IPointerClickHandler,IPunObservable
{
    // Start is called before the first frame update
    Animator anim;
    Material material;
    public int dir;
    List<float> CardDir;
    int cardPointY;
    int cardPointX;
    public int stateid;
    public  enum CardState
    {
        Null,
        Set,
        Choose,
        Rotate,
        Lock,
    }


    CardState cs;
    public void ChangeCS(CardState next)
    {
        switch (next) {
            case CardState.Set:
                anim.SetBool("Choose", false);
                anim.SetBool("LockCard", false);
                anim.SetBool("SetCard", true);
                cs = next;
                stateid = 0;
                break;
            case CardState.Choose:
                anim.SetBool("Choose", true);
                cs = next;
                stateid = 1;
                break;
            case CardState.Rotate:
                cs = next;

                stateid = 2;
                break;
            case CardState.Lock:
                anim.SetBool("LockCard", true);
                cs = next;
                stateid = 3;
                break;
        }
    }
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool Locked()
    {
        return cs == CardState.Lock;
    }
    public void Cardinit(Texture tx,int deg,int _y,int _x,bool Lock)
    {
        cardPointX = _x;
        cardPointY = _y;
        material = this.GetComponent<MeshRenderer>().material;
        anim = this.GetComponent<Animator>();
        CardDir = new List<float>() { 0, 90, 180, -90 };
        dir = deg;
        material.SetTexture("_MainTex", tx);
        this.gameObject.transform.eulerAngles =new Vector3(0f, CardDir[dir],0f);
        ChangeCS(CardState.Set);
        //カードをロックかセット状態にする
        if (!Lock)return;
        ChangeCS(CardState.Lock);
    }

    public void OnClick()
    {
        if (cs == CardState.Rotate) {
            Debug.LogError("回転ステート");
            Rotate();
            return;
        }
        int ind = GameManager.Instance.CanChoose(cardPointY, cardPointX);
        if (ind==-1) return;
        Debug.LogError(cardPointY.ToString() + " " + cardPointX.ToString());
        switch (cs)
        {
            case CardState.Set:
                Debug.LogError("Set");
                GameManager.Instance.ChooseCard(ind,cardPointY, cardPointX);
                ChangeCS(CardState.Choose);
                break;
            case CardState.Choose:
                if (GameManager.Instance.CanRotate())
                {
                    GameManager.Instance.IsRotate = true;
                    ChangeCS(CardState.Rotate);
                    Rotate();
                    return;
                }
                GameManager.Instance.ChooseCard(ind, -1, -1);
                ChangeCS(CardState.Set);
                break;
            case CardState.Lock:
                return;
        }
    }

    public void Rotate()
    {
        dir = (dir + 1) % 4;
        this.gameObject.transform.eulerAngles = new Vector3(0f, CardDir[dir], 0f);
        if (dir == GameManager.Instance.degs[cardPointY*8+cardPointX])
        {
            GameManager.Instance.ChooseCard(0, -1, -1);
            GameManager.Instance.IsRotate = false;
            ChangeCS(CardState.Set);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.nowturn % 2 != GameManager.Instance.PlayerId) return;
        if (!canChange(GameManager.Instance.PlayerId)) return;
        Debug.LogError(cardPointY.ToString() + " " + cardPointX.ToString());
        OnClick();
    }
    
    bool canChange(int Pid)
    {
        switch (Pid)
        {
            case 1:
                if ((cardPointX < 2 && cardPointY < 2) || (cardPointX > 5 && cardPointY > 5)) return false;
                break;
            case 0:
                if ((cardPointX >5 && cardPointY < 2) || (cardPointX < 2 && cardPointY > 5)) return false;
                break;
        }
        return true;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting)
        {
            stream.SendNext(stateid);
        }
        else
        {
            stateid = (int)stream.ReceiveNext();
            switch (stateid)
            {
                case 3:
                    ChangeCS(CardState.Lock);
                    break;
                default:
                    break;
            }
        }
        //throw new System.NotImplementedException();
    }
    
}