using Photon.MmoDemo.Client;
using Photon.MmoDemo.Common;
using System.Linq;
using UnityEngine;
/// <summary>
/// 收到其他物件、玩家資料的處理，通常為同步廣播(如某人移動)
/// </summary>
public partial class RunBehaviour
{

    /// <summary>
    /// 當有物件被新增，需要表現出來
    /// </summary>
    /// <param name="item"></param>
    public void OnItemAdded(Item item)
    {
        if (Game == null || item.IsMine)
            return;

        switch (item.Type)
        {
            case ItemType.Avatar:
                {
                    CreateActor(item);
                }
                break;
            case ItemType.Prop:
                {
                    CreateGroundProp(item);
                }
                break;
            case ItemType.Bot:
                {

                }
                break;
        }
    }

    /// <summary>
    /// 新增玩家
    /// </summary>
    /// <param name="actorItem"></param>
    private void CreateActor(Item actorItem)
    {
        ItemBehaviour actorGo = Instantiate(Resources.Load("Player/Player") as GameObject).GetComponent<ItemBehaviour>();

        if (actorItem.IsMine)
            ItemB = actorGo;

        actorGo.Initialize(actorItem, "Item_" + actorItem.Id);

        actorGo.isoRenderer.SetController(0, (int)actorItem.attr[Attr.outward], (int)actorItem.attr[Attr.color]);
    }

    /// <summary>
    /// 新增地板道具
    /// </summary>
    /// <param name="item"></param>
    private void CreateGroundProp(Item item)
    {
        GroundProp propObj = Instantiate(Resources.Load("GroundProp") as GameObject).GetComponent<GroundProp>();

        var sprite = Gui.Images.Single(s => s.name == item.attr[Attr.modelID].ToString());

        var pos = Maps[(int)MapType.map].layoutGrid.CellToWorld(new Vector3Int((int)item.Position.X, (int)item.Position.Y, 0));
        propObj.Init(sprite, item, pos);
    }

    /// <summary>
    /// 當地面物品、玩家登出離開、怪物消失等移除動作
    /// </summary>
    /// <param name="item"></param>
    public void OnItemRemoved(Item item)
    {
        var objName = "Item_" + item.Id;
        GameObject obj = GameObject.Find(objName);
        if (obj != null)
        {
            if (NearItems.ContainsKey(item.Id))
                NearItems.Remove(item.Id);

            Destroy(obj);
        }
        else
        {
            Debug.LogError("destroy item not found " + item.Id);
        }
    }
}