using System.Collections.Generic;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif


public class TurnActionList : SerializedMonoBehaviour
{
    [ListDrawerSettings(
        Expanded = true, NumberOfItemsPerPage = 10, OnTitleBarGUI = "DrawActionListRefreshButton",
        OnBeginListElementGUI = "BeginDrawActionListElement", OnEndListElementGUI = "EndDrawListElement"
    )]
    public List<TurnAction> actions = new List<TurnAction>();


    public void Init(GlobalController globalCtrl)
    {
        foreach (TurnAction action in this.actions)
        {
            action.Init(globalCtrl);   
        }
    }


#if UNITY_EDITOR

    private void DrawActionListRefreshButton()
    {
        if (SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))
        {
            this.actions.Sort();

            for (int i = 0; i < this.actions.Count; i++)
            {
                this.actions[i].index = i;
            }
        }

    }

    private void BeginDrawActionListElement(int index)
    {
        TurnAction action = this.actions[index];

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

#endif
}
