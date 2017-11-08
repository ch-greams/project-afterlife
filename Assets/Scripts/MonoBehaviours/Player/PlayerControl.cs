using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerControl : MonoBehaviour
{
    public Vector3 startPosition;
    public Vector3 endPosition;
    public float speed = 5.0F;

    public GameObject playerObject;
    public float animationSpeed = 5.0F;

    private Animator playerAnimator;
    private float startTime;
    private float journeyLength = 0F;

    private readonly int SPEED_PARAM_HASH = Animator.StringToHash("Speed");


    private void Start()
    {
        this.playerAnimator = this.playerObject.GetComponent<Animator>();
        this.ResetStartPosition();
    }

    private void Update()
    {
        if (this.startPosition != this.endPosition)
        {
            if (this.endPosition == base.transform.position)
            {
                this.ResetStartPosition();
            }
            else
            {
                this.SetCurrentPosition();
            }
        }
    }

    public void MoveTo(Vector3 position)
    {
        this.playerObject.transform.LookAt(position);

        this.startTime = Time.time;
        this.endPosition = position;
        this.journeyLength = Vector3.Distance(this.startPosition, this.endPosition);
    }

    public IEnumerator MoveToAsync(Vector3 position)
    {
        this.playerAnimator.SetFloat(SPEED_PARAM_HASH, this.animationSpeed);

        this.MoveTo(position);

        while (this.endPosition != base.transform.position)
        {
            yield return null;
        }

        this.playerAnimator.SetFloat(SPEED_PARAM_HASH, 0F);
    }

    public void Interact(int animationHash, Vector3 targetLocation)
    {
        this.playerObject.transform.LookAt(targetLocation);
        float delta = -1 * this.playerObject.transform.rotation.eulerAngles.x;
        this.playerObject.transform.Rotate(delta, 0, 0);
        this.playerAnimator.SetTrigger(animationHash);
    }

    public void SetCurrentPosition()
    {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;
        base.transform.position = Vector3.Lerp(this.startPosition, this.endPosition, fracJourney);
    }

    public void ResetStartPosition()
    {
        this.startPosition = this.endPosition = base.transform.position;
    }
}
