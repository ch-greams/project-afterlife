using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;


public abstract class Interactable : SerializedMonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public SceneController sceneCtrl;
    
    private void Awake()
    {
        base.StartCoroutine(this.OnInit());
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Input.GetMouseButtonUp(0))
        {
            base.StartCoroutine(this.OnLeftClick());
        }
        else if (Input.GetMouseButtonUp(1))
        {
            base.StartCoroutine(this.OnRightClick());
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        base.StartCoroutine(this.OnHoverStart());
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        base.StartCoroutine(this.OnHoverEnd());
    }

    protected virtual IEnumerator OnInit()
    {
        yield return null;
    }
    protected virtual IEnumerator OnLeftClick()
    {
        yield return null;
    }
    protected virtual IEnumerator OnRightClick()
    {
        yield return null;
    }
    protected virtual IEnumerator OnHoverStart()
    {
        yield return null;
    }
    protected virtual IEnumerator OnHoverEnd()
    {
        yield return null;
    }
}
