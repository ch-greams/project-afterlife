using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;


public class EndOfTurnActionManager : SerializedMonoBehaviour
{
    [ListDrawerSettings(
        Expanded = false, NumberOfItemsPerPage = 10, OnTitleBarGUI = "DrawWalkActionListRefreshButton",
        OnBeginListElementGUI = "BeginDrawWalkActionListElement", OnEndListElementGUI = "EndDrawListElement"
    )]
    public List<EndOfTurnAction> walkActions = new List<EndOfTurnAction>();

    [ListDrawerSettings(
        Expanded = false, NumberOfItemsPerPage = 10, OnTitleBarGUI = "DrawFlashlightActionListRefreshButton",
        OnBeginListElementGUI = "BeginDrawFlashlightActionListElement", OnEndListElementGUI = "EndDrawListElement"
    )]
    public List<EndOfTurnAction> flashlightActions = new List<EndOfTurnAction>();

    [ListDrawerSettings(
        Expanded = false, NumberOfItemsPerPage = 10, OnTitleBarGUI = "DrawGranadeActionListRefreshButton",
        OnBeginListElementGUI = "BeginDrawGranadeActionListElement", OnEndListElementGUI = "EndDrawListElement"
    )]
    public List<EndOfTurnAction> granadeActions = new List<EndOfTurnAction>();


    private GlobalController globalCtrl;


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;

        foreach (EndOfTurnAction action in this.walkActions)
        {
            action.Init(this.globalCtrl);   
        }

        foreach (EndOfTurnAction action in this.flashlightActions)
        {
            action.Init(this.globalCtrl);   
        }

        foreach (EndOfTurnAction action in this.granadeActions)
        {
            action.Init(this.globalCtrl);   
        }
    }


    public IEnumerator ReactOnValidActions()
    {
        PlayerActionType actionType = this.globalCtrl.playerActionManager.currentAction;

        foreach (EndOfTurnAction action in this.GetActionsByType(actionType))
        {
            if (action.IsValid()) {
                yield return action.React();
            }
        }
    }

    private List<EndOfTurnAction> GetActionsByType(PlayerActionType playerActionType)
    {
        switch (playerActionType)
        {
            case PlayerActionType.Walk:
                return this.walkActions;
            case PlayerActionType.Flashlight:
                return this.flashlightActions;
            case PlayerActionType.Granade:
                return this.granadeActions;
            case PlayerActionType.Torch:   
            case PlayerActionType.Undefined:   
            default:
                return new List<EndOfTurnAction>();
        }
    }


    private void DrawActionListRefreshButton(List<EndOfTurnAction> actions)
    {
        if (SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))
        {
            actions.Sort();

            for (int i = 0; i < actions.Count; i++)
            {
                actions[i].index = i;
            }
        }
    }

    private void DrawWalkActionListRefreshButton()
    {
        this.DrawActionListRefreshButton(this.walkActions);
    }
    
    private void DrawFlashlightActionListRefreshButton()
    {
        this.DrawActionListRefreshButton(this.flashlightActions);
    }

    private void DrawGranadeActionListRefreshButton()
    {
        this.DrawActionListRefreshButton(this.granadeActions);
    }

    private void BeginDrawActionListElement(EndOfTurnAction action, int index)
    {
        string title = (
            index == action.index
                ? string.Format("{0} - {1}", index, action.name)
                : string.Format("{0} -> {1} - {2}", index, action.index, action.name)
        );
        SirenixEditorGUI.BeginBox(title);
    }

    private void BeginDrawWalkActionListElement(int index)
    {
        this.BeginDrawActionListElement(this.walkActions[index], index);
    }

    private void BeginDrawFlashlightActionListElement(int index)
    {
        this.BeginDrawActionListElement(this.flashlightActions[index], index);
    }

    private void BeginDrawGranadeActionListElement(int index)
    {
        this.BeginDrawActionListElement(this.granadeActions[index], index);
    }

    private void EndDrawListElement(int index)
    {
        SirenixEditorGUI.EndBox();
    }
}
