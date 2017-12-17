using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ICondition
{
    void Init(Interactable interactable);
    bool IsValid();
}
