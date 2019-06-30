using DG.Tweening;
using Photon.MmoDemo.Client;
using Photon.MmoDemo.Common;
using UnityEngine;

/// <summary>
/// 玩家同步
/// </summary>
public class ItemBehaviour : MonoBehaviour
{
    public Item item;
    public Rigidbody2D Rbody;
    public CharRender isoRenderer;
    Vector3Int lastPos;
    bool inited;
    public Tweener queue;

    float nextIdle;

    private Vector2 worldPos { get => RunBehaviour.Get.Maps[(int)MapType.map].layoutGrid.CellToWorld(Global.VtV3i(item.Position)); }


    public void Initialize(Item actorItem, string name)
    {
        this.item = actorItem;
        this.name = name;
        //※更新移動
        lastPos = Global.VtV3i(item.Position);
        Rbody.position = RunBehaviour.Get.Maps[(int)MapType.map].layoutGrid.CellToWorld(lastPos);
        isoRenderer.SetDirection(Vector2.zero, item.Direction);
        inited = true;
    }

    public void Update()
    {
        if (this.item == null || !inited)
            return;

        //※更新移動
        var dis = Vector2.Distance(worldPos, Rbody.position);
        if (lastPos != Global.VtV3i(item.Position)) //  || (queue != null && dis <= 0.08f)
        {
            if (queue != null)
                queue.Kill();

            lastPos = Global.VtV3i(item.Position);
            isoRenderer.SetDirection(worldPos - Rbody.position, item.Direction);
            
            if(!RunBehaviour.Get.NearItems.ContainsKey(item.Id))
                RunBehaviour.Get.NearItems.Add(item.Id, Global.VtV3i(item.Position));
            else
                RunBehaviour.Get.NearItems[item.Id] = Global.VtV3i(item.Position);

            queue = Rbody.DOMove(worldPos, dis / 4f ).OnComplete(() =>
            {
                queue = null;

                if (item.IsMine)
                {
                    if (RunBehaviour.Get.Status != CharStatus.Atk)
                        RunBehaviour.Get.Status = CharStatus.Idle;
                }
                
            });
            nextIdle = Time.time + (dis / 4f) +0.2f;

        } else if (Time.time > nextIdle && queue == null)
        {
            isoRenderer.SetDirection(Vector2.zero, item.Direction);
        }

    }

    void OnMouseEnter()
    {
        RunBehaviour.Get.Gui.NameInfo.text = item.attr[Attr.name].ToString();
        RunBehaviour.Get.Gui.NameInfo.gameObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        RunBehaviour.Get.Gui.NameInfo.text = "";
        RunBehaviour.Get.Gui.NameInfo.gameObject.SetActive(false);
    }

}