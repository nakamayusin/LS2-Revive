using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlManager : MonoBehaviour
{
    public Transform CastSwicher;

    public List<Transform> CastBtnList = new List<Transform>();

    Event e;
    void OnGUI()
    {
        e = Event.current;
        if (e != null && e.type == EventType.KeyDown)
        {
            switch (e.keyCode)
            {
                case KeyCode.F1:
                    CastSwicher.SetParent(CastBtnList[0], false);
                    break;
                case KeyCode.F2:
                    CastSwicher.SetParent(CastBtnList[1], false);
                    break;
                case KeyCode.F3:
                    CastSwicher.SetParent(CastBtnList[2], false);
                    break;
                case KeyCode.F4:
                    CastSwicher.SetParent(CastBtnList[3], false);
                    break;
                case KeyCode.F5:
                    CastSwicher.SetParent(CastBtnList[4], false);
                    break;
                case KeyCode.F6:
                    CastSwicher.SetParent(CastBtnList[5], false);
                    break;
                case KeyCode.F7:
                    CastSwicher.SetParent(CastBtnList[6], false);
                    break;
                case KeyCode.F8:
                    CastSwicher.SetParent(CastBtnList[7], false);
                    break;
                case KeyCode.F9:
                    CastSwicher.SetParent(CastBtnList[8], false);
                    break;
                case KeyCode.F10:
                    CastSwicher.SetParent(CastBtnList[9], false);
                    break;
                case KeyCode.F11:
                    CastSwicher.SetParent(CastBtnList[10], false);
                    break;
                case KeyCode.F12:
                    CastSwicher.SetParent(CastBtnList[11], false);
                    break;
                default:
                    return;
            }
        }
    }
}
