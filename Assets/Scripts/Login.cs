using Photon.MmoDemo.Client;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public Text username;
    public Text password;
    public Text Error;

    public static string acc, pwd;

    public void SignIn()
    {
        if (username.text.Equals("") || password.text.Equals(""))
        {
            Error.text = "請輸入帳號密碼";
        }
        else if (username.text.Length < 6 || password.text.Length < 6)
        {
            Error.text = "帳號和密碼至少需6個字元";
        }
        else
        {
            acc = username.text;
            pwd = Global.ToMD5(password.text);


            if(!RunBehaviour.Get.Connected)
                RunBehaviour.Get.Game.Connect();//※按下登入按鈕瞬間送出，OnConnect後
            else
                Operations.Login(RunBehaviour.Get.Game, acc, pwd);
        }
    }
}
