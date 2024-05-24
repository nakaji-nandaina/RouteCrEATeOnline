
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //text �g���̂�
using Photon.Pun;
using Photon.Realtime;
using Photon;
public class NetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject UnityChan;
    private byte maxPlayersPerRoom = 2;
    //���i�͐�΂P�ɂ���
    string gameVersion = "1";
    // Start is called before the first frame update
    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    { //�Q�[���J�n���Ɏ����I�ɐڑ����鎞�Ɏg���B
      Connect();

    }
    public void Connect()
    {//Photon�̃l�b�g���[�N�ɐڑ�������
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();

        }
        //�����łȂ���΁A
        else
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    public override void OnConnectedToMaster()
    {//�}�X�^�[�T�[�o�[�֐ڑ������Ƃ�
        Debug.Log("Pun Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {//�ؒf�����Ƃ�
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {//���[���ւ̎Q���Ɏ��s�����Ƃ��i�����_���j
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
    }

    void OnGUI()
    {
        //���O�C���̏�Ԃ���ʏ�ɏo��
        GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
    }

    public override void OnJoinedRoom()
    {//���[���ɓ�������
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        if (PhotonNetwork.PlayerList.Length == 1)
        {//��l�ڂ̏ꍇ
            Debug.Log("Im first");
            Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber.ToString());
            GameManager.Instance.initGame();
            GameManager.Instance.playerText.text = "Player 1";
            GameManager.Instance.playerText.color = Color.blue;
            GameObject unitychan = PhotonNetwork.Instantiate("UnityChan", new Vector3(GameManager.Instance.cardHolders[0].CardPoints[0].transform.position.x, 1, GameManager.Instance.cardHolders[0].CardPoints[0].transform.position.z), new Quaternion(0, 180, 0, 0));
            //unitychan.transform.position = new Vector3(GameManager.Instance.cardHolders[0].CardPoints[0].transform.position.x, 1,GameManager.Instance.cardHolders[0].CardPoints[0].transform.position.z) ;
            GameManager.Instance.UnityChanObj = unitychan;
            GameManager.Instance.PlayerId = 0;
        }
        else
        {
            Debug.Log("Im Second");
            GameManager.Instance.PlayerId = 1;
            GameManager.Instance.playerText.text = "Player 2";
            GameManager.Instance.playerText.color = Color.red;
            GameObject unitychan = PhotonNetwork.Instantiate("UnityChan", new Vector3(GameManager.Instance.cardHolders[0].CardPoints[7].transform.position.x, 1, GameManager.Instance.cardHolders[0].CardPoints[0].transform.position.z),new Quaternion(0,180,0,0));
            GameManager.Instance.UnityChanObj = unitychan;
            //unitychan.transform.position = new Vector3(GameManager.Instance.cardHolders[0].CardPoints[7].transform.position.x, 1, GameManager.Instance.cardHolders[0].CardPoints[0].transform.position.z);
        }

    }

}
