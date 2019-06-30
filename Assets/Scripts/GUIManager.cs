using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public List<GameObject> GuiList = new List<GameObject>();

    public int NowGuiIndex = 0;

    public Sprite[] Images;

    public Text NameInfo;

    public GameObject Disconnected;

    void Start()
    {
        Images = Resources.LoadAll<Sprite>("props");
    }

    public void GuiChange(int index)
    {
        if (index >= 4)
        {
            for (int i = 4; i < GuiList.Count; i++)
            {
               GuiList[i].SetActive(index == i && NowGuiIndex != index);
            }
            NowGuiIndex = NowGuiIndex == index ? 0 : index;
        }
    }
}