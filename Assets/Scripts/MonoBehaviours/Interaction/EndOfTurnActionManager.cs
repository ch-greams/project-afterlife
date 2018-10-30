using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEngine;

public class EndOfTurnActionManager : SerializedMonoBehaviour
{
    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "name", OnTitleBarGUI = "DrawRefreshButton", NumberOfItemsPerPage = 5)]
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
        Debug.Log("Lock Player Controls");

        foreach (EndOfTurnAction endOfTurnAction in this.endOfTurnActions)
        {
            if (endOfTurnAction.IsValid()) {
                yield return endOfTurnAction.React();
            }
        }

        Debug.Log("Unlock Player Controls");
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
}
