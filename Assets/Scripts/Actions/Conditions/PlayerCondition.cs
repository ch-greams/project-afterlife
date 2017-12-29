

public class PlayerCondition : ICondition
{
    private bool isMoving;

    public void Init(Interactable interactable)
    {
        this.isMoving = interactable.sceneCtrl.player.isMoving;
    }

    public bool IsValid()
    {
        return !this.isMoving;
    }
}
