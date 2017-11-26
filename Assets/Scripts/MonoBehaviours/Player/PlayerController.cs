using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public float speed = 5.0F;
    public float animationSpeed = 5.0F;

    public Transform playerTransform;
    public Animator playerAnimator;

    [InlineEditor(Expanded = true)]
    public Tile currentTile;

    private readonly int SPEED_PARAM_HASH = Animator.StringToHash("Speed");


    public IEnumerator MoveToTile(Tile targetTile)
    {
        foreach (Tile tile in targetTile.FindPathFrom(this.currentTile, true).Reverse())
        {
            this.playerAnimator.SetFloat(SPEED_PARAM_HASH, this.animationSpeed);

            float startTime = Time.time;
            Vector3 startPosition = base.transform.position;
            Vector3 endPosition = tile.obj.transform.position;
            float journeyLength = Vector3.Distance(startPosition, endPosition);

            this.playerTransform.LookAt(endPosition);

            while (endPosition != base.transform.position)
            {
                float distCovered = (Time.time - startTime) * speed;
                float fracJourney = distCovered / journeyLength;
                base.transform.position = Vector3.Lerp(startPosition, endPosition, fracJourney);

                yield return null;
            }

            this.currentTile = tile;
            this.playerAnimator.SetFloat(SPEED_PARAM_HASH, 0F);
        }
    }

    public void Interact(int animationHash, Vector3 targetLocation)
    {
        this.playerTransform.LookAt(targetLocation);
        float delta = -1 * this.playerTransform.rotation.eulerAngles.x;
        this.playerTransform.Rotate(delta, 0, 0);
        this.playerAnimator.SetTrigger(animationHash);
    }
}
