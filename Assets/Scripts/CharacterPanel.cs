using Newtonsoft.Json.Linq;
using Photon.MmoDemo.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CharacterPanel : MonoBehaviour
{
    public Dictionary<string, CharacterInfo> CharInfoList = new Dictionary<string, CharacterInfo>();

    public string SelectedCharID;

    public CharacterInfo infoPrefab;

    public GameObject CharGrid;

    public GameObject SelectBorder;

    public Text DeleteCharName;
    public GameObject DeleteCharPanel;

    public void Show(Hashtable table)
    {
        if(table != null)
        foreach (DictionaryEntry Char in table)
        {
            var info = JObject.Parse(Char.Value.ToString());
            CharacterInfo characterInfo = Instantiate(infoPrefab);
            characterInfo.Init(Char.Key.ToString(), info, this);
            CharInfoList.Add(Char.Key.ToString(), characterInfo);
            characterInfo.gameObject.transform.SetParent(CharGrid.transform, false);
            characterInfo.Image.SetNativeSize();
        }
    }

    public void AddChar(Hashtable table)
    {
        CharacterInfo characterInfo = Instantiate(infoPrefab);
        characterInfo.Init(table["guid"].ToString(), table, this);
        CharInfoList.Add(table["guid"].ToString(), characterInfo);
        characterInfo.gameObject.transform.SetParent(CharGrid.transform, false);
        characterInfo.Image.SetNativeSize();
    }

    public void CreateChar()
    {
        if (CharInfoList.Count >= 8)
            return;

        RunBehaviour.Get.Gui.GuiList[1].SetActive(false);
        RunBehaviour.Get.Gui.GuiList[2].SetActive(true);
    }

    public void Select(string guid, Transform selectObj)
    {
        SelectedCharID = guid;
        SelectBorder.transform.SetParent(selectObj, false);
    }

    public void DeleteWindow()
    {
        if (SelectedCharID == null)
            return;

        DeleteCharName.text = CharInfoList[SelectedCharID].Name.text;
        DeleteCharPanel.SetActive(true);
    }

    public void DeleteChar()
    {
        if (SelectedCharID == null)
            return;

        string delID = SelectedCharID;

        SelectBorder.transform.SetParent(this.transform, false);
        Destroy(CharInfoList[SelectedCharID].gameObject);
        CharInfoList.Remove(SelectedCharID);

        SelectedCharID = null;

        foreach (var charI in CharInfoList)
        {
            charI.Value.ReButton();
        }

        Operations.DeleteChar(RunBehaviour.Get.Game, delID, Login.acc, Login.pwd);
    }

    public void StartGame()
    {
        if (SelectedCharID == null)
            return;

        RunBehaviour.Get.Game.EnterWorld(SelectedCharID, Login.acc, Login.pwd);
    }

}
