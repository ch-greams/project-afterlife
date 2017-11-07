using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerControl : MonoBehaviour
{
    public Vector3 startPosition;
    public Vector3 endPosition;
    public float speed = 5.0F;

    private float startTime;
    private float journeyLength = 0F;


    private void Start()
    {
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
        this.startTime = Time.time;
        this.endPosition = position;
        this.journeyLength = Vector3.Distance(this.startPosition, this.endPosition);
    }

    public IEnumerator MoveToAsync(Vector3 position)
    {
        this.MoveTo(position);

        while (this.endPosition != base.transform.position)
        {
            yield return null;
        }
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
