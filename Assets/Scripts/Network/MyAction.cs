using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Photon.MmoDemo.Common;
using Photon.MmoDemo.Common.Model;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 和自己遊戲中有關的封包接收
/// </summary>
public partial class RunBehaviour
{
    /// <summary>
    /// 登入角色的入口
    /// </summary>
    /// <param name="attr">狀態細節資料</param>
    /// <param name="position">座標</param>
    /// <param name="dir">面向</param>
    /// <param name="gears">穿著中的裝備</param>
    /// <param name="inventory">背包內的道具</param>
    public void OnWorldEntered(Hashtable attr, Vector position, int dir, Hashtable gears, Hashtable inventory)
    {
        // (int)attr[Attr.slots];  //-> 顯示出我的道具總格數

        //這個是裝備中的，登入時直接到裝備欄位上顯示
        Wear(gears);

        ////要丟進背包內的已存在道具
        //foreach (var it in inventory)
        //{
        //    Prop prop = JsonConvert.DeserializeObject<Prop>(it.Value.ToString());
        //    BagUI.Add(prop.Index, prop);//還要依照 prop.Index 做排序
        //}

        Gui.GuiList[1].SetActive(false);
        Gui.GuiList[3].SetActive(true);
        var mapID = (int)attr[Attr.mapID];
        var mapInfo = (Instantiate(Resources.Load("Maps/" + mapID + "/Map " + mapID)) as GameObject).GetComponent<MapInfo>();
        Maps[(int)MapType.map] = mapInfo.Map;
        Maps[(int)MapType.collider] = mapInfo.Collider;


        Cursor = Instantiate(Resources.Load("Cursor") as GameObject).GetComponent<CursorFollower>();
        Cursor.Init(Maps[(int)MapType.map]);

        mouseManager = gameObject.AddComponent<MouseManager>();
        mouseManager.Init(this);


        transform.position = Maps[(int)MapType.map].layoutGrid.CellToWorld(Global.VtV3i(position));

        CreateActor(game.Avatar);

        Camera.main.GetComponent<BasicCameraFollow>().followTarget = ItemB.transform;
    }

    /// <summary>
    /// 裝備穿著表現
    /// </summary>
    /// <param name="gears"></param>
    void Wear(Hashtable gears)
    {
        Image nowSlot;
        foreach (var gear in gears)
        {
            Prop prop = JsonConvert.DeserializeObject<Prop>(gear.Value.ToString());
            nowSlot = Gui.GuiList[4].GetComponent<Inventory>().Gears[(int)gear.Key];

            nowSlot.sprite = Gui.Images.Single(s => s.name == prop.Image.ToString());
            nowSlot.color = new Color(1, 1, 1, 1);
            nowSlot.SetNativeSize();
        }
    }

    /// <summary>
    /// 丟棄物品到地板成功
    /// </summary>
    /// <param name="type">0=裝備中的物品、1=道具欄內的物品</param>
    /// <param name="index">物品位置編號</param>
    public void OnPropDropped(int type, int index)
    {
        var inven = Gui.GuiList[4].GetComponent<Inventory>();

        if (type == 0)
        {
            inven.Gears[index].sprite = null;
            inven.Gears[index].color = new Color(1, 1, 1, 0);
        }
        else
        {

        }
    }

    /// <summary>
    /// 未來作用於千里眼  遠距離偵查通緝犯或某人，暫時無視
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="itemType"></param>
    /// <param name="position"></param>
    /// <param name="remove"></param>
    public void OnRadarUpdate(string itemId, ItemType itemType, Vector position, bool remove)
    {
        //Radar r = GetComponent<Radar>();
        //r.OnRadarUpdate(itemId, itemType, position);
    }


}