using System.Collections;
using UnityEngine;


public abstract class Interactable : MonoBehaviour
{
    private IEnumerator OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0))
        {
            yield return base.StartCoroutine(this.OnLeftClick());
        }

        if (Input.GetMouseButtonUp(1))
        {
            yield return base.StartCoroutine(this.OnRightClick());
        }
    }
    private IEnumerator OnMouseEnter()
    {
        yield return base.StartCoroutine(this.OnHoverStart());
    }
    private IEnumerator OnMouseExit()
    {
        yield return base.StartCoroutine(this.OnHoverEnd());
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
