using Sirenix.OdinInspector;
using UnityEngine;


public class ReachablePoint : SerializedMonoBehaviour
{
    public bool showGizmo = true;

    public Point point;

    [BoxGroup("Current Point Refresh")]
    public SceneController sceneCtrl;

    [BoxGroup("Current Point Refresh"), ShowInInspector]
    private Vector3 offset { get { return (
        (this.sceneCtrl != null)
            ? new Vector3(x: this.sceneCtrl.transform.position.x, y: 0.5F, z: this.sceneCtrl.transform.position.z)
            : Vector3.zero
    ); } }



    private void OnDrawGizmos()
    {
        if (this.showGizmo)
        {
            Gizmos.DrawIcon(transform.position, "md-pin");
        }
    }


    [BoxGroup("Current Point Refresh"), Button(ButtonSizes.Medium)]
    public void RefreshCurrentPoint()
    {
        this.point = new Point(this.transform.position - this.offset);
        this.transform.position = point.CalcWorldCoord(new Vector3(
            x: this.offset.x + 0.5F, y: this.offset.y, z: this.offset.z + 0.5F
        ));
        this.transform.name = string.Format("reachable_point {0}", this.point);
    }

}
