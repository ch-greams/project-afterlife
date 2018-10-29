

public class LightSourceCondition : IInteractableCondition
{
    public LightSourceConditionType type;
    private SceneState sceneState;
    private LightSourceData data;


    public void Init(Interactable interactable)
    {
        this.sceneState = interactable.sceneCtrl.sceneState;
        this.data = interactable.data as LightSourceData;
    }

    public bool IsValid()
    {
        switch (this.type)
        {
            case LightSourceConditionType.IS_ENABLED:
                return this.sceneState.lightSources[this.data.id];
            case LightSourceConditionType.IS_DISABLED:
                return !this.sceneState.lightSources[this.data.id];
            default:
                return false;
        }
    }
}

public enum LightSourceConditionType
{
    IS_ENABLED,
    IS_DISABLED,
}
