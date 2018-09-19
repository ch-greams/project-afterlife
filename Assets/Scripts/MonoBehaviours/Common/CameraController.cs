using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;


public class CameraController : MonoBehaviour
{
    public GameObject cameraPivot;
    public bool rotateEnabled = true;
    public float rotateSpeed = 400F;
    [ValueDropdown("GetLayers")]
    public int layer;

    public SceneController sceneCtrl;

    private List<Renderer> hiddenWalls = new List<Renderer>();

    private Vector3 startRotation;
    private Vector3 endRotation;

    private float startTime;
    private float deltaAngle;

    public Direction currentDirection = Direction.Front;

    public enum Direction { Front, Right, Back, Left }


    private void Awake() 
    {
        this.endRotation = this.startRotation = cameraPivot.transform.localEulerAngles;
    }

    private void Update()
    {
        if (this.rotateEnabled)
        {
            this.RotateCamera();
        }
        
        this.HideObstructingWalls();
    }

    private void HideObstructingWalls()
    {
        int layerMask = 1 << layer;
        Vector3 direction = cameraPivot.transform.position - transform.position;

        List<Renderer> wallsToHide = Physics
            .RaycastAll(transform.position, direction, Mathf.Infinity, layerMask)
            .Select(hit => hit.transform.GetComponent<Renderer>())
            .ToList();

        foreach (Renderer hiddenWall in this.hiddenWalls.ToList())
        {
            if (!wallsToHide.Contains(hiddenWall))
            {
                hiddenWall.shadowCastingMode = ShadowCastingMode.On;
                this.hiddenWalls.Remove(hiddenWall);
            }
        }

        foreach (Renderer wallToHide in wallsToHide)
        {
            this.hiddenWalls.Add(wallToHide);
            wallToHide.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        }
    }


    private void RotateCamera() 
    {
        if (this.Compare(cameraPivot.transform.localEulerAngles, this.endRotation)) 
        {
            // Rotate left
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetAxis("Right Trigger") == 1) 
            {
                this.startTime = Time.time;
                this.startRotation = cameraPivot.transform.localEulerAngles;

                this.endRotation = new Vector3(this.startRotation.x, this.startRotation.y + 90, this.startRotation.z);
                this.deltaAngle = Vector3.Distance(this.startRotation, this.endRotation);

                this.SetCurrentDirection(true);
            }
            // Rotate right
            if (Input.GetKeyDown(KeyCode.E) || Input.GetAxis("Left Trigger") == 1) 
            {
                this.startTime = Time.time;
                this.startRotation = cameraPivot.transform.localEulerAngles;

                this.endRotation = new Vector3(this.startRotation.x, this.startRotation.y - 90, this.startRotation.z);
                this.deltaAngle = Vector3.Distance(this.startRotation, this.endRotation);

                this.SetCurrentDirection(false);
            }
        }

        if (this.startRotation != this.endRotation) 
        {
            float fraction = ((Time.time - this.startTime) * (this.rotateSpeed)) / this.deltaAngle;
            cameraPivot.transform.localEulerAngles = Vector3.Lerp(this.startRotation, this.endRotation, fraction);
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

    private void SetDirectionSwitches()
    {
        switch (this.currentDirection)
        {
            case Direction.Front:
                this.sceneCtrl.globalCtrl.directionSwitch = false;
                this.sceneCtrl.globalCtrl.directionVerticalSignSwitch = false;
                this.sceneCtrl.globalCtrl.directionHorizontalSignSwitch = false;
                break;
            case Direction.Right:
                this.sceneCtrl.globalCtrl.directionSwitch = true;
                this.sceneCtrl.globalCtrl.directionVerticalSignSwitch = false;
                this.sceneCtrl.globalCtrl.directionHorizontalSignSwitch = true;
                break;
            case Direction.Back:
                this.sceneCtrl.globalCtrl.directionSwitch = false;
                this.sceneCtrl.globalCtrl.directionVerticalSignSwitch = true;
                this.sceneCtrl.globalCtrl.directionHorizontalSignSwitch = true;
                break;
            case Direction.Left:
                this.sceneCtrl.globalCtrl.directionSwitch = true;
                this.sceneCtrl.globalCtrl.directionVerticalSignSwitch = true;
                this.sceneCtrl.globalCtrl.directionHorizontalSignSwitch = false;
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

        this.SetDirectionSwitches();
    }

#if UNITY_EDITOR

    private static ValueDropdownList<int> GetLayers()
    {
        ValueDropdownList<int> layers = new ValueDropdownList<int>();

        for (int layerIndex = 0; layerIndex < 32; layerIndex++)
        {
            string layerName = LayerMask.LayerToName(layerIndex);

            if (layerName.Length > 0) {
                layers.Add(layerName, layerIndex);
            }
        }

        return layers;
    }

#endif
}
