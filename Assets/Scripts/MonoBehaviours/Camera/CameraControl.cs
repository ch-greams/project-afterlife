using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;


public class CameraControl : MonoBehaviour
{
    public bool rotateEnabled = true;
    public float rotateSpeed = 400f;

    public List<Renderer> wallsOnLeft = new List<Renderer>();
    public List<Renderer> wallsOnRight = new List<Renderer>();
    public List<Renderer> wallsOnFront = new List<Renderer>();
    public List<Renderer> wallsOnBack = new List<Renderer>();
    public List<Renderer> walls = new List<Renderer>();


    private Vector3 startRotation;
    private Vector3 endRotation;

    private float startTime;
    private float deltaAngle;

    private Direction currentDirection = Direction.Front;
    private enum Direction { Front, Right, Back, Left }


    private void Awake() 
    {
        this.endRotation = this.startRotation = base.transform.localEulerAngles;

        this.walls = this.wallsOnBack
            .Concat(this.wallsOnLeft)
            .Concat(this.wallsOnFront)
            .Concat(this.wallsOnRight).ToList();
    }

    private void Start()
    {
        this.SetShadowCastingMode();
    }

    private void Update() 
    {
        if (this.rotateEnabled)
        {
            this.RotateCamera();
        }
    }

    private void RotateCamera() 
    {
        if (this.Compare(base.transform.localEulerAngles, this.endRotation)) 
        {
            // Rotate left
            if (Input.GetKeyDown(KeyCode.Q)) 
            {
                this.startTime = Time.time;
                this.startRotation = base.transform.localEulerAngles;

                this.endRotation = new Vector3(this.startRotation.x, this.startRotation.y + 90, this.startRotation.z);
                this.deltaAngle = Vector3.Distance(this.startRotation, this.endRotation);

                this.UpdateCurrentDirection(true);
                this.SetShadowCastingMode();
            }
            // Rotate right
            if (Input.GetKeyDown(KeyCode.E)) 
            {
                this.startTime = Time.time;
                this.startRotation = base.transform.localEulerAngles;

                this.endRotation = new Vector3(this.startRotation.x, this.startRotation.y - 90, this.startRotation.z);
                this.deltaAngle = Vector3.Distance(this.startRotation, this.endRotation);

                this.UpdateCurrentDirection(false);
                this.SetShadowCastingMode();
            }
        }

        if (this.startRotation != this.endRotation) 
        {
            float fraction = ((Time.time - this.startTime) * (this.rotateSpeed)) / this.deltaAngle;
            base.transform.localEulerAngles = Vector3.Lerp(this.startRotation, this.endRotation, fraction);
        }
    }

    private bool Compare(Vector3 current, Vector3 final)
    {
        int fin = Mathf.RoundToInt(final.y);
        int cur = Mathf.RoundToInt(current.y);

        fin = fin > 270 ? fin - 360 : (fin < 0 ? fin + 360 : fin);
        cur = cur > 270 ? cur - 360 : (cur < 0 ? cur + 360 : cur);

        return fin == cur;
    }


    private void SetShadowCastingMode()
    {
        List<Renderer> wallsToHide = this.ListToHide(this.currentDirection);

        foreach (Renderer wall in this.walls)
        {
            wall.shadowCastingMode = wallsToHide.Contains(wall) ? ShadowCastingMode.ShadowsOnly : ShadowCastingMode.On;
        }
    }

    private List<Renderer> ListToHide(Direction direction)
    {
        List<Renderer> result;

        switch (direction)
        {
            case Direction.Front:
                result = this.wallsOnBack.Concat(this.wallsOnLeft).ToList();
                break;
            case Direction.Right:
                result = this.wallsOnFront.Concat(this.wallsOnLeft).ToList();
                break;
            case Direction.Back:
                result = this.wallsOnFront.Concat(this.wallsOnRight).ToList();
                break;
            case Direction.Left:
                result = this.wallsOnBack.Concat(this.wallsOnRight).ToList();
                break;
            default:
                result = new List<Renderer>();
                break;
        }

        return result;
    }

    private void UpdateCurrentDirection(bool isRight)
    {
        switch (this.currentDirection)
        {
            case Direction.Front:
                this.currentDirection = isRight ? Direction.Right : Direction.Left;
                break;
            case Direction.Right:
                this.currentDirection = isRight ? Direction.Back : Direction.Front;
                break;
            case Direction.Back:
                this.currentDirection = isRight ? Direction.Left : Direction.Right;
                break;
            case Direction.Left:
                this.currentDirection = isRight ? Direction.Front : Direction.Back;
                break;
        }
    }
}
