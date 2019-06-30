using Assets.Scripts.EasyPathFinder;
using Photon.MmoDemo.Client;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MouseManager : MonoBehaviour
{
    public RunBehaviour MyRun;

    public Tilemap Map => MyRun.Maps[(int)MapType.map];

    public void Init(RunBehaviour run)
    {
        MyRun = run;
        PathFinder.Init(Map, MyRun.Maps[(int)MapType.collider]);
    }

    readonly float cdWalk = 0.2f;
    readonly float cdAttack = 0.8f;
    readonly float allowMoveWhenAttack = 0.2f;
    float nextWalk;

    //暫時的
    public Animator animator;
    public List<RuntimeAnimatorController> controllers = new List<RuntimeAnimatorController>();

    private bool dragging = false;
    private Vector2 originalPosition;
    private Transform objectToDrag;
    private Image objectToDragImage;
    List<RaycastResult> hitObjects = new List<RaycastResult>();

    float rest;

    void Update()
    {
        if (MyRun == null) return;

        if (Time.time > MyRun.nextAttack && MyRun.Status == CharStatus.Atk)
            MyRun.Status = CharStatus.Idle;

        if (dragging)
        {
            objectToDrag.position = Input.mousePosition;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (objectToDrag != null)
            {
                var objectToReplace = GetMouseObj(true);

                if (objectToReplace == null && dragging)//嘗試丟地板
                {
                    var gearList = RunBehaviour.Get.Gui.GuiList[4].GetComponent<Inventory>().Gears;
                    int index = gearList.FindIndex(a => a.gameObject == objectToDrag.gameObject);
                    Operations.DropToGround(RunBehaviour.Get.Game, 0, index);
                }
                else if (objectToReplace != null && objectToDrag.tag != objectToReplace.tag)
                {
                    objectToDrag.position = objectToReplace.transform.position;
                    objectToReplace.transform.position = originalPosition;
                }
                else
                {
                    objectToDrag.position = originalPosition;
                }

                objectToDragImage.raycastTarget = true;
                objectToDrag = null;
                dragging = false;
                rest = Time.time + 0.5f;
                return;
            }
            else
            {
                var obj = GetMouseObj(true);
                objectToDrag = obj?.transform;

                if (objectToDrag != null)
                {
                    objectToDrag.transform.SetAsLastSibling();

                    originalPosition = objectToDrag.transform.position;
                    objectToDragImage = objectToDrag.GetComponent<Image>();
                    objectToDragImage.raycastTarget = false;
                    dragging = true;
                    return;
                }
            }
        }

        if (dragging)
            return;

        if (Input.GetMouseButton(0) && GetMouseObj(false) == null && Time.time > rest)
            Act();

        if (Input.GetMouseButton(1) && Time.time > MyRun.nextAttack)
        {
            if (MyRun.LookedGroundProp != null)
            {
                Operations.PickUp(MyRun.Game, MyRun.LookedGroundProp.Id);
                return;
            }

            if (MyRun.ItemB.queue != null && MyRun.path != null)
            {
                if (MyRun.path.Count > MyRun.nowPathIndex + 1)
                {
                    for (int i = MyRun.path.Count - 1; i > MyRun.nowPathIndex; i--)
                        MyRun.path.RemoveAt(i);
                }
            }

            var rnd = Random.Range(0, 10);
            animator.runtimeAnimatorController = controllers[rnd];

            var pos = Map.CellToWorld(MyRun.Cursor.NowPoint);
            var direction = new Vector2(pos.x, pos.y) - new Vector2(transform.position.x, transform.position.y);
            int dir = CharRender.DirectionToIndex(direction, 8);

            MyRun.Status = CharStatus.Atk;

            MyRun.ItemB.isoRenderer.SetDirection(CharStatus.Atk, dir);
            MyRun.nextAttack = Time.time + cdAttack;
        }

        WalkNext();
    }

    void Act()
    {
        if (Time.time > nextWalk && Time.time > MyRun.nextAttack - allowMoveWhenAttack)
        {
            var dis = Vector2.Distance(transform.position, MyRun.transform.position);

            if (MyRun.Status == CharStatus.Moving && dis > 0.08f)
                return;

            var nowCell = MyRun.NowCell();
            var mouseCell = Map.layoutGrid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0.3f));

            MyRun.path = PathFinder.FindPath(nowCell, mouseCell);

            if (MyRun.path.Count > 0)
                MyRun.nowPathIndex = 0;

            nextWalk = Time.time + cdWalk;
        }
    }

    void WalkNext()
    {
        if (MyRun.path == null)
            return;

        if (MyRun.nowPathIndex >= MyRun.path.Count)
        {
            MyRun.path = null;
            return;
        }

        var nowPath = MyRun.path[MyRun.nowPathIndex];

        var dis = Vector2.Distance(MyRun.ItemB.Rbody.position, transform.position);
        if (((MyRun.Status == CharStatus.Moving && dis <= 0.08f) || MyRun.Status == CharStatus.Idle) && 
            Time.time > MyRun.nextAttack - Global.SliceAttack)
        {
            var tar = Map.CellToWorld(new Vector3Int(nowPath.x, nowPath.y, 0));

            var direction = tar - transform.position;
            MyRun.newDirection = CharRender.DirectionToIndex(direction, 8);

            transform.position = tar;
            MyRun.nowPathIndex++;

            if (MyRun.Status != CharStatus.Atk)
                MyRun.Status = CharStatus.Moving;
        }
    }

    private GameObject GetMouseObj(bool find)
    {
        var pointer = new PointerEventData(EventSystem.current);

        pointer.position = Input.mousePosition;

        EventSystem.current.RaycastAll(pointer, hitObjects);

        if (hitObjects.Count <= 0) return null;

        if(find)
            return hitObjects.Find(a => a.gameObject.tag == "Gear" || a.gameObject.tag == "Slot").gameObject;
        else
            return hitObjects.Find(a => a.gameObject.layer == LayerMask.NameToLayer("UI")).gameObject;
    }
}
