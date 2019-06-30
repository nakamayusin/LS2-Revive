using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform ButtonPanel;
    // Start is called before the first frame update
    Tweener tweener;

    public void OnPointerEnter(PointerEventData eventData)
    {
        tweener.Kill();
        tweener = ButtonPanel.DOLocalMove(new Vector2(0, 12f), 0.5f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tweener.Kill();
        tweener = ButtonPanel.DOLocalMove(new Vector2(0, -24f), 0.5f);
    }
}
