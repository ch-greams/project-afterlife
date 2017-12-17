using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;


public class DoorInteractable : Interactable
{
    [Required]
    public Door door;

    public List<Tile> attachedTiles = new List<Tile>();

    [ListDrawerSettings(ListElementLabelName = "name", DraggableItems = false)]
    public List<Action> leftClickActions = new List<Action>();
    [ListDrawerSettings(ListElementLabelName = "name", DraggableItems = false)]
    public List<Action> hoverStartActions = new List<Action>();
    [ListDrawerSettings(ListElementLabelName = "name", DraggableItems = false)]
    public List<Action> hoverEndActions = new List<Action>();

    protected override IEnumerator OnInit()
    {
        yield return null;

        this.leftClickActions.ForEach(action => action.Init(this));
        this.hoverStartActions.ForEach(action => action.Init(this));
        this.hoverEndActions.ForEach(action => action.Init(this));
    }

    protected override IEnumerator OnLeftClick()
    {
        Action action = this.leftClickActions.Find(a => a.IsValid());

        if (action != null)
        {
            yield return action.React();
        }
    }

    protected override IEnumerator OnHoverStart()
    {
        Action action = this.hoverStartActions.Find(a => a.IsValid());

        if (action != null)
        {
            yield return action.React();
        }
    }

    protected override IEnumerator OnHoverEnd()
    {
        Action action = this.hoverEndActions.Find(a => a.IsValid());

        if (action != null)
        {
            yield return action.React();
        }
    }
}
