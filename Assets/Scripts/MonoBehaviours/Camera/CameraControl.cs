using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;


public class CameraControl : MonoBehaviour
{
    public bool rotateEnabled = true;
    public float rotateSpeed = 400F;

    public List<Renderer> frontWalls = new List<Renderer>();
    public List<Renderer> rightWalls = new List<Renderer>();
    public List<Renderer> backWalls = new List<Renderer>();
    public List<Renderer> leftWalls = new List<Renderer>();


    private Vector3 startRotation;
    private Vector3 endRotation;

    private float startTime;
    private float deltaAngle;

    private Direction currentDirection = Direction.Front;
    private List<Renderer> hiddenWalls = new List<Renderer>();

    private enum Direction { Front, Right, Back, Left }


    private void Awake() 
    {
        this.endRotation = this.startRotation = base.transform.localEulerAngles;
    }

    private void Start()
    {
        this.UpdateWallRender();
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

                this.SetCurrentDirection(true);
                this.UpdateWallRender();
            }
            // Rotate right
            if (Input.GetKeyDown(KeyCode.E)) 
            {
                this.startTime = Time.time;
                this.startRotation = base.transform.localEulerAngles;

                this.endRotation = new Vector3(this.startRotation.x, this.startRotation.y - 90, this.startRotation.z);
                this.deltaAngle = Vector3.Distance(this.startRotation, this.endRotation);

                this.SetCurrentDirection(false);
                this.UpdateWallRender();
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


    private void UpdateWallRender()
    {
        foreach (Renderer wall in this.hiddenWalls)
        {
            wall.shadowCastingMode = ShadowCastingMode.On;
        }

        this.SetHiddenWalls(this.currentDirection);

        foreach (Renderer wall in this.hiddenWalls)
        {
            wall.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        }
    }

    private void SetHiddenWalls(Direction direction)
    {
        switch (direction)
        {
            case Direction.Front:
                this.hiddenWalls = this.backWalls.Concat(this.leftWalls).ToList();
                break;
            case Direction.Right:
                this.hiddenWalls = this.frontWalls.Concat(this.leftWalls).ToList();
                break;
            case Direction.Back:
                this.hiddenWalls = this.frontWalls.Concat(this.rightWalls).ToList();
                break;
            case Direction.Left:
                this.hiddenWalls = this.backWalls.Concat(this.rightWalls).ToList();
                break;
        }
    }

    private void SetCurrentDirection(bool isRight)
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
