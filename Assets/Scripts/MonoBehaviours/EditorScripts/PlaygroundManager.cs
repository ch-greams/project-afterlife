using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class PlaygroundManager : MonoBehaviour
{
    public Button walkButton;
    public Button flashlightButton;

    public GlobalController globalCtrl;


    private void Update()
    {
        if (Input.GetButtonDown("Button A"))
        {
            Debug.Log("Button A");
        }

        if (Input.GetButtonDown("Button B"))
        {
            Debug.Log("Button B");

            this.walkButton.onClick.Invoke();
        }

        if (Input.GetButtonDown("Button X"))
        {
            Debug.Log("Button X");

            this.flashlightButton.onClick.Invoke();
        }

        if (Input.GetButtonDown("Button Y"))
        {
            Debug.Log("Button Y");
        }

        // if (Input.GetAxis("Left Trigger") > 0)
        // {
        //     Debug.Log("Left Trigger: " + Input.GetAxis("Left Trigger"));
        // }

        // if (Input.GetAxis("Right Trigger") > 0)
        // {
        //     Debug.Log("Right Trigger: " + Input.GetAxis("Right Trigger"));
        // }

        if (Input.GetAxis("Left Stick Horizontal") != 0)
        {
            Debug.Log("Left Stick Horizontal: " + Input.GetAxis("Left Stick Horizontal"));
        }

        if (Input.GetAxis("Left Stick Vertical") != 0)
        {
            Debug.Log("Left Stick Vertical: " + Input.GetAxis("Left Stick Vertical"));
        }
    }


    public void Walk()
    {
        Debug.Log("Walk Button Triggered - Select Tile");

        Tile firstTile = this.globalCtrl.sceneCtrl.player.tile.activeNeighbours.First();

        firstTile.obj.GetComponent<Interactable>().OnPointerEnter(null);
        // TODO: Save as selectedTile

        // firstTile.obj.GetComponent<Interactable>().OnPointerClick(null);

    }

    public void UseFlashlight()
    {
        Debug.Log("Flashlight Button Triggered - Select Direction");
    }
}
