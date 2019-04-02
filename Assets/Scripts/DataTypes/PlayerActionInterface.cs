using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;


public class PlayerActionInterface : SerializedMonoBehaviour
{
    [BoxGroup("Group Configuration")]
    public GameObject playerActionsGroup;
    [BoxGroup("Group Configuration")]
    public GameObject statsPanelGroup;


    [BoxGroup("[B] Walk Button")]
    public Button walkButton;
    [BoxGroup("[B] Walk Button")]
    public Sprite walkButtonActive;
    [BoxGroup("[B] Walk Button")]
    public Sprite walkButtonInactive;
    [BoxGroup("[B] Walk Button")]
    public Image walkButtonProc;
    [BoxGroup("[B] Walk Button")]
    public Sprite walkButtonProcActive;
    [BoxGroup("[B] Walk Button")]
    public Sprite walkButtonProcInactive;


    [BoxGroup("[X] Flashlight Button")]
    public Button flashlightButton;
    [BoxGroup("[X] Flashlight Button")]
    public Sprite flashlightButtonActive;
    [BoxGroup("[X] Flashlight Button")]
    public Sprite flashlightButtonInactive;

    [BoxGroup("[A] Torch Button")]
    public Button torchButton;
    [BoxGroup("[A] Torch Button")]
    public Sprite torchButtonActive;
    [BoxGroup("[A] Torch Button")]
    public Sprite torchButtonInactive;

    [BoxGroup("[Y] Granade Button")]
    public Button granadeButton;
    [BoxGroup("[Y] Granade Button")]
    public Sprite granadeButtonActive;
    [BoxGroup("[Y] Granade Button")]
    public Sprite granadeButtonInactive;
    [BoxGroup("[Y] Granade Button")]
    public Text granadeButtonCooldownLabel;

    [BoxGroup("[LT] Skip Turn Button")]
    public Button skipTurnButton;
    [BoxGroup("[LT] Skip Turn Button")]
    public RectTransform skipTurnButtonProgress;
    [BoxGroup("[LT] Skip Turn Button"), ReadOnly]
    public float skipTurnButtonProgressCounter = 0;
    [BoxGroup("[LT] Skip Turn Button")]
    public float skipTurnButtonProgressMax = 100;

    [BoxGroup("[RT] Interaction Button")]
    public Button interactionButton;
    [BoxGroup("[RT] Interaction Button")]
    public RectTransform interactionButtonProgress;
    [BoxGroup("[RT] Interaction Button"), ReadOnly]
    public float interactionButtonProgressCounter = 0;
    [BoxGroup("[RT] Interaction Button")]
    public float interactionButtonProgressMax = 100;

    [BoxGroup("[RT] World Interaction Button")]
    public Button wInteractionButton;
    [BoxGroup("[RT] World Interaction Button")]
    public RectTransform wInteractionButtonProgress;
    [BoxGroup("[RT] World Interaction Button")]
    public Text wInteractionButtonLabel;
    [BoxGroup("[RT] World Interaction Button"), ReadOnly]
    public float wInteractionButtonProgressCounter = 0;
    [BoxGroup("[RT] World Interaction Button")]
    public float wInteractionButtonProgressMax = 100;



}
