using UnityEngine;


public class Interactable : MonoBehaviour
{
    private const string PLAYER_NAME = "Player";

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0))
        {
            this.MoveToThisCell();
        }
    }

    private void MoveToThisCell()
    {
        this.GetComponent<AnimateTiledTexture>().Play();
        GameObject.Find(PLAYER_NAME).GetComponent<PlayerControl>().MoveTo(base.transform.position);
    }
}
