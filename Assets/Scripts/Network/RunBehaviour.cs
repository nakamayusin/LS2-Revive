using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.MmoDemo.Client;
using Photon.MmoDemo.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

/// <summary>
/// 主要同步接收區
/// </summary>
public partial class RunBehaviour : MonoBehaviour, IGameListener
{
    public static RunBehaviour Get;
    public Login Login;
    public bool DebugLog;
    
    private Game game;

    #region Basic
    /// <summary> 自己的物件 </summary>
    public ItemBehaviour ItemB;
    /// <summary> 跟隨游標 </summary>
    public CursorFollower Cursor;
    /// <summary> (滑鼠)：攻擊、移動、拾取、施法 </summary>
    public MouseManager mouseManager;
    #endregion

    /// <summary> 當前的地圖 </summary>
    public Tilemap[] Maps = new Tilemap[3];

    /// <summary> 當前角色狀態 </summary>
    public CharStatus Status = CharStatus.Idle;

    /// <summary> 已看見的玩家或怪物、NPC </summary>
    public Dictionary<string,Vector3Int> NearItems = new Dictionary<string, Vector3Int>();

    #region Movement 移動相關參數，不需理會
    public List<Vector3Int> path;
    public int nowPathIndex = 0;
    Vector2 inputVector;
    public int newDirection;
    public float nextAttack;
    float NextMoveOperaction = 0f;
    public Vector3Int NowCell()
    {
        return Maps[(int)MapType.map].WorldToCell(transform.position);
    }

    public Vector NowCellVt()
    {
        var vt3i = Maps[(int)MapType.map].WorldToCell(transform.position);
        return new Vector(vt3i.x, vt3i.y);
    }
    #endregion

    /// <summary> 一切UI接口 </summary>
    public GUIManager Gui;

    /// <summary> 創角介面 </summary>
    public CreateChar CreateChar;

    /// <summary> 滑鼠指到的物品，按右鍵申請拾取時使用 </summary>
    public Item LookedGroundProp;


    public Game Game
    {
        get { return this.game; }
        set { this.game = value; }
    }

    public bool IsDebugLogEnabled
    {
        get { return this.DebugLog; }
    }

    public void Awake()
    {
        Get = this;
        Settings settings = Settings.GetDefaultSettings();
        this.Game = new Game(this, settings);
        var peer = new PhotonPeer(this.Game, ConnectionProtocol.Udp) { ChannelCount = 3 };
        this.Game.Initialize(peer);
    }

    public void Start()
    {
        Application.runInBackground = true;
        Application.targetFrameRate = 30;
    }


    public void Update()
    {
        Game.Update();
        if (Game.WorldEntered)
        {
            inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical") * 0.8f);
            var dis = Vector2.Distance(ItemB.Rbody.position, transform.position);
            if (inputVector.magnitude > 0f && ((Status == CharStatus.Moving && dis <= 0.08f) || Status == CharStatus.Idle) && Time.time > nextAttack - Global.SliceAttack)
            {
                newDirection = CharRender.DirectionToIndex(inputVector, 8);
                path = null;
                Move(transform.position, newDirection);
            }

            if (dis > 0f || (Status == CharStatus.Moving && dis <= 0.08f) || newDirection !=Game.Avatar.Direction) // && Time.time >= NextMoveOperaction
            {
                NextMoveOperaction = Time.time + 0.2f;
                //向伺服器發送移動申請
                Operations.Move(game, Game.Avatar.Id, NowCellVt(), newDirection, true);
            }
        }
    }

    /// <summary>
    /// 自己的移動操作，WASD
    /// </summary>
    /// <param name="cur"></param>
    /// <param name="dir"></param>
    void Move(Vector2 cur, int dir)
    {
        Vector3Int vt3Int = Maps[(int)MapType.map].WorldToCell(new Vector3(cur.x, cur.y, 0)) + Global.DirValue[dir];

        var type = Maps[(int)MapType.map].GetColliderType(vt3Int);
        var typeobj = Maps[(int)MapType.collider].GetColliderType(vt3Int);
        if ((type == Tile.ColliderType.Grid && typeobj != Tile.ColliderType.Sprite) ||
            (type == Tile.ColliderType.Sprite && typeobj == Tile.ColliderType.Grid) ||
            NearItems.ContainsValue(vt3Int)
            )
            return;

        transform.position = Maps[(int)MapType.map].CellToWorld(vt3Int);

        if (Status != CharStatus.Atk)
            Status = CharStatus.Moving;
    }

    /// <summary>
    /// 主動關閉遊戲
    /// </summary>
    public void OnApplicationQuit()
    {
        this.Game.Disconnect();       
    }

    #region IGameListener
    public void LogDebug(object message) { Debug.Log(message); }
    public void LogError(object message) { Debug.LogError(message); }
    public void LogInfo(object message) { Debug.Log(message); }

    public void OnConnect()
    {
        Debug.Log("connected");
        Connected = true;
        Operations.Login(game, Login.acc, Login.pwd);
    }

    public bool Connected = false;

    public void OnDisconnect(StatusCode returnCode)
    {
        Debug.Log("disconnected");
        if(!Connected)
            Login.Error.text = "伺服器連接失敗。";
        else
        {
            Gui.Disconnected.SetActive(true);
        }
    }

    public void RestartGame()
    {
        Gui.Disconnected.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion


    /// <summary>
    /// 不重要
    /// </summary>
    /// <param name="operationCode"></param>
    /// <param name="debugMessage"></param>
    public void ShowErrorMessage(byte operationCode, string debugMessage)
    {
        switch ((OperationCode)operationCode)
        {
            case OperationCode.Login:
                {
                    Login.Error.text = debugMessage;
                }
                return;
        }
    }

}