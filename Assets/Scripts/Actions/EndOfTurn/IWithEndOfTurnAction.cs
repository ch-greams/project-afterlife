using System.Collections;
using System.Collections.Generic;


// NOTE: IManager ?
public interface IWithEndOfTurnAction
{
    // TODO: Remove all references of OnTurnChange()
    IEnumerator OnTurnChange();

    List<EndOfTurnAction> endOfTurnActions { get; }
}
