using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;


public class Container
{
    public ContainerType type;
    public GameObject obj;
    public Renderer renderer;
    public Animator animator;


    public Container() { }

}


public enum ContainerType
{
    AptN1_Bedroom_Table,
    AptN1_Bedroom_Bed,
}
