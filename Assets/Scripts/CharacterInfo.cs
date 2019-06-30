using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CharacterInfo : MonoBehaviour
{
    public Text Name;
    public Text Level;
    public Text Souls;
    public Text MapName;

    public Image Image;

    public Button Button;

    public string Guid;

    public CharacterPanel Panel;

    public void Init(string guid, JObject json, CharacterPanel panel)
    {
        Panel = panel;
        Guid = guid;
        Name.text = json["name"].ToString();
        Level.text = json["level"].ToString();
        Souls.text = json["souls"].ToString();
        MapName.text = Global.MapName[Convert.ToInt32(json["mapID"])];

        int index = Convert.ToInt32(json["outward"]) + Convert.ToInt32(json["color"]) * 10;

        Image.sprite = RunBehaviour.Get.CreateChar.CharList[index];

        Button.onClick.AddListener(() => panel.Select(guid, this.transform));
    }

    public void Init(string guid, Hashtable json, CharacterPanel panel)
    {
        Panel = panel;
        Guid = guid;
        Name.text = json["name"].ToString();
        Level.text = json["level"].ToString();
        Souls.text = json["souls"].ToString();
        MapName.text = Global.MapName[Convert.ToInt32(json["mapID"])];

        int index = Convert.ToInt32(json["outward"]) + Convert.ToInt32(json["color"]) * 10;

        Image.sprite = RunBehaviour.Get.CreateChar.CharList[index];

        Button.onClick.AddListener(() => panel.Select(guid, this.transform));
    }

    public void ReButton()
    {
        Button.onClick.RemoveAllListeners();

        Button.onClick.AddListener(() => Panel.Select(Guid, this.transform));
    }

}
