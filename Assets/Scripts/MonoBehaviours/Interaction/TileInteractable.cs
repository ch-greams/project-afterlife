using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;


public class TileInteractable : Interactable
{
    [BoxGroup("Tile Editor")]
    [InlineEditor(Expanded = true)]
    public Tile tile;

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

#if UNITY_EDITOR

    [BoxGroup("Tile Editor")]
    [ButtonGroup("Tile Editor/Passable")]
    [Button("Make Possible", ButtonSizes.Medium)]
    public void MakePossible()
    {
        this.tile.passable = true;
    }

    [BoxGroup("Tile Editor")]
    [ButtonGroup("Tile Editor/Passable")]
    [Button("Make Impossible", ButtonSizes.Medium)]
    public void MakeImpossible()
    {
        this.tile.passable = false;
    }

#endif
}
