using Photon.MmoDemo.Client;
using Photon.MmoDemo.Common;
using UnityEngine;

public class GroundProp : MonoBehaviour
{
    public Item Item;

    public SpriteRenderer Render;

    private void Start()
    {
        Render = GetComponent<SpriteRenderer>();
    }

    void OnMouseEnter()
    {
        RunBehaviour.Get.Gui.NameInfo.text = Item.attr[Attr.name].ToString();
        RunBehaviour.Get.Gui.NameInfo.gameObject.SetActive(true);

        if (Vector2.Distance(new Vector2(RunBehaviour.Get.Game.Avatar.Position.X, RunBehaviour.Get.Game.Avatar.Position.Y),
            new Vector2(Item.Position.X, Item.Position.Y)) < 1.5)
        {
            RunBehaviour.Get.LookedGroundProp = Item;
        }
    }

    private void OnMouseExit()
    {
        RunBehaviour.Get.Gui.NameInfo.text = "";
        RunBehaviour.Get.Gui.NameInfo.gameObject.SetActive(false);
        RunBehaviour.Get.LookedGroundProp = null;
    }

    public void Init(Sprite sprite, Item item, Vector2 position)
    {
        Item = item;
        Render.sprite = sprite;
        transform.position = position;
    }
}