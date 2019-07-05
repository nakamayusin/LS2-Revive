using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Photon.MmoDemo.Client;
using Photon.MmoDemo.Common.Model;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    //裝備中的道具
    public List<Image> Gears = new List<Image>();

    public List<Image> Bag = new List<Image>();

    public Image SlotPrefab;

    public Transform BagFather;

    /// <summary>
    /// 物品欄位添加
    /// </summary>
    /// <param name="table"></param>
    public void InitBag(Hashtable table, int slots)
    {
        foreach (var pp in Bag)
            Destroy(pp.gameObject);

        Bag.Clear();

        Bag.Capacity = slots;

        Image nowSlot;
        Prop prop;
        Image slot;
        for (int i=0; i < Bag.Capacity; i++)
        {
            slot = Instantiate(SlotPrefab, BagFather, false);
            Bag.Add(slot);
            slot.name = "Slot"+i;

            nowSlot = Bag[i].GetComponent<Slot>().Icon;

            if (!table.ContainsKey(i))
            {
                nowSlot.gameObject.SetActive(false);
                continue;
            }

            prop = JsonConvert.DeserializeObject<Prop>(table[i].ToString());
            

            nowSlot.sprite = RunBehaviour.Get.Gui.Images.Single(s => s.name == prop.Image.ToString());
            nowSlot.color = new Color(1, 1, 1, 1);
            nowSlot.SetNativeSize();
        }
    }

    public void AddProp(int index, Prop prop)
    {
        var nowSlot = Bag[index].GetComponent<Slot>().Icon;

        nowSlot.sprite = RunBehaviour.Get.Gui.Images.Single(s => s.name == prop.Image.ToString());
        nowSlot.color = new Color(1, 1, 1, 1);
        nowSlot.gameObject.SetActive(true);
        nowSlot.SetNativeSize();
    }

    /// <summary>
    /// 裝備穿著表現
    /// </summary>
    /// <param name="gears"></param>
    public void InitGears(Hashtable gears)
    {
        Image nowSlot;
        Prop prop;
        foreach (var gear in gears)
        {
            prop = JsonConvert.DeserializeObject<Prop>(gear.Value.ToString());
            nowSlot = Gears[(int)gear.Key];

            nowSlot.sprite = RunBehaviour.Get.Gui.Images.Single(s => s.name == prop.Image.ToString());
            nowSlot.color = new Color(1, 1, 1, 1);
            nowSlot.SetNativeSize();
        }
    }

    public void SortBag()
    {
        Operations.SortBag(RunBehaviour.Get.Game);
    }
}
