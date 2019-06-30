using Photon.MmoDemo.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorFollower : MonoBehaviour
{
    public GridLayout NowMap;

    public Vector3Int NowPoint;

    public void Init(GridLayout nowMap)
    {
        NowMap = nowMap;
    }
    GameObject mouseObj;

    private void Update()
    {
        if (!NowMap) return;

        NowPoint = NowMap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition ) + new Vector3(0, 0.3f));
        transform.position = NowMap.CellToWorld(NowPoint);
    }
}
