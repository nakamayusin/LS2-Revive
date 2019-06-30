using ExitGames.Client.Photon;

/// <summary>
/// 和自己登入有關的封包接收
/// </summary>
public partial class RunBehaviour
{

    /// <summary>
    /// 登入成功
    /// </summary>
    /// <param name="charList"></param>
    public void OnLogin(Hashtable charList)
    {
        this.Gui.GuiList[0].SetActive(false);
        Gui.GuiList[1].SetActive(true);
        Gui.GuiList[1].GetComponent<CharacterPanel>().Show(charList);
    }

    /// <summary>
    /// 角色創建成功，出現在選角列表上
    /// </summary>
    /// <param name="newChar">新創的角色資訊</param>
    public void OnCharAdded(Hashtable newChar)
    {
        Gui.GuiList[1].GetComponent<CharacterPanel>().AddChar(newChar);
    }
}