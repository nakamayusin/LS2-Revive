using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Photon.MmoDemo.Client;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateChar : MonoBehaviour
{
    public Image ShowChar;
    public int Selected = 0;
    public List<Sprite> CharList = new List<Sprite>();

    int style, color;

    public Text Name;
    public Text Error;

    public Text Point;
    public List<Text> attrText = new List<Text>(4);
    readonly int[] attr = new int[4];

    int remainTalents = 3;

    int total = 30;

    List<int> talents = new List<int>();

    public List<Button> remainImg = new List<Button>();

    public List<Sprite> stars = new List<Sprite>(2);

    public List<Image> Tstart = new List<Image>(3);

    public void Talents(int i)
    {
        if (talents.Contains(i))//取消
        {
            if (talents.Count <= 0)
                return;

            talents.Remove(i);
            remainImg[i].image.sprite = stars[0];
            Tstart[remainTalents].sprite = stars[1];
            remainTalents++;
            Debug.Log(remainTalents);
        }
        else//選擇
        {
            if (talents.Count >= 3)
                return;

            talents.Add(i);
            remainImg[i].image.sprite = stars[1];
            remainTalents--;
            Debug.Log(remainTalents);
            Tstart[remainTalents].sprite = stars[0];
        }
    }


    int Used()
    {
        int x = 0;
        for (int i = 0; i < 4; i++)
            x += attr[i];

        return x;
    }

    public void Higher(int type)
    {
        if (total-1 < 0)
            return;

        attr[type]++;
        total--;
        attrText[type].text = attr[type].ToString();
        Point.text = total.ToString();
    }

    public void Lower(int type)
    {
        if (attr[type] <= 0)
            return;

        attr[type]--;
        total++;
        attrText[type].text = attr[type].ToString();
        Point.text = total.ToString();
    }

    public void Style(int i)
    {

        style += i;
        if (style >= 10)
            style -= 10;
        else if (style < 0)
            style += 10;

        Selected = color * 10 + style;

        ShowChar.sprite = CharList[Selected];
        ShowChar.SetNativeSize();
    }

    public void Color(int i)
    {
        color += i;
        if (color >= 4)
            color -= 4;
        else if (color < 0)
            color += 4;

        Selected = color * 10 + style;

        ShowChar.sprite = CharList[Selected];
        ShowChar.SetNativeSize();
    }

    public void Return()
    {
        RunBehaviour.Get.Gui.GuiList[2].SetActive(false);
        RunBehaviour.Get.Gui.GuiList[1].SetActive(true);
    }

    public void GoCreate()
    {
        if (total != 0 || Used()　!= 30)
        {
            Error.text = "點數配置異常。";
            return;
        }

        if (talents.Count != 3)
        {
            Error.text = "天賦配置異常。";
            return;
        }

        var tJ = JsonConvert.SerializeObject(talents);

        var table = new Hashtable
        {
            { "style", style },
            { "color", color },
            { "name", Name.text },
            { "attr", attr },
            { "talents", tJ }
        };

        Operations.CreateChar(RunBehaviour.Get.Game, table , Login.acc, Login.pwd);

        Return();
    }

}
