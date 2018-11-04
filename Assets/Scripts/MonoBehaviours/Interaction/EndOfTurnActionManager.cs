using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;


public class EndOfTurnActionManager : SerializedMonoBehaviour
{
    [ListDrawerSettings(
        Expanded = false, NumberOfItemsPerPage = 5, OnTitleBarGUI = "DrawRefreshButton",
        OnBeginListElementGUI = "BeginDrawListElement", OnEndListElementGUI = "EndDrawListElement"
    )]
    public List<EndOfTurnAction> endOfTurnActions = new List<EndOfTurnAction>();


    public void Init(GlobalController globalCtrl)
    {
        foreach (EndOfTurnAction endOfTurnAction in this.endOfTurnActions)
        {
            endOfTurnAction.Init(globalCtrl);   
        }
    }


    public IEnumerator ReactOnValidActions()
    {
        // Debug.Log("Lock Player Controls");

        foreach (EndOfTurnAction endOfTurnAction in this.endOfTurnActions)
        {
            if (endOfTurnAction.IsValid()) {
                yield return endOfTurnAction.React();
            }
        }

        // Debug.Log("Unlock Player Controls");
    }


    private void DrawRefreshButton()
    {
        if (SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))
        {
            endOfTurnActions.Sort();

            for (int i = 0; i < endOfTurnActions.Count; i++)
            {
                endOfTurnActions[i].index = i;
            }
        }
    }

    private void BeginDrawListElement(int index)
    {
        EndOfTurnAction action = this.endOfTurnActions[index];
        string title = (
            index == action.index
                ? string.Format("{0} - {1}", index, action.name)
                : string.Format("{0} -> {1} - {2}", index, action.index, action.name)
        );
        SirenixEditorGUI.BeginBox(title);
    }

    private void EndDrawListElement(int index)
    {
        SirenixEditorGUI.EndBox();
    }
}
