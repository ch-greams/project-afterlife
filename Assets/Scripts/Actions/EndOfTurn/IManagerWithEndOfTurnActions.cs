using System.Collections;
using System.Collections.Generic;


public interface IManagerWithEndOfTurnActions
{
    List<EndOfTurnAction> endOfTurnActions { get; }
}
