using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;


public class SaveManager
{
    [InlineButton("ToggleSaveMenulInEditor", "Toggle")]
    public GameObject saveMenu;

    [ListDrawerSettings(ListElementLabelName = "name", DraggableItems = false, Expanded = false)]
    public List<RecordSlot> recordSlots = new List<RecordSlot>();


    private GlobalState globalState;
    private GlobalController globalCtrl;
    private string saveDataFolder;
    private const string SAVE_DATA_FOLDER_NAME = "SaveData";


    public void Init(GlobalController globalCtrl)
    {
        this.globalCtrl = globalCtrl;
        this.globalState = globalCtrl.globalState;
        this.saveDataFolder = string.Format("{0}/{1}", Application.persistentDataPath, SaveManager.SAVE_DATA_FOLDER_NAME);

        if (!Directory.Exists(this.saveDataFolder))
        {
            Directory.CreateDirectory(this.saveDataFolder);
        }

        this.recordSlots.ForEach(rs => rs.Init(this));
    }

    public bool ExistsSave(string fileName)
    {
        return File.Exists(string.Format("{0}/{1}.save", this.saveDataFolder, fileName));
    }

    public void DeleteSave(string fileName)
    {
        File.Delete(string.Format("{0}/{1}.save", this.saveDataFolder, fileName));
    }

    public void CreateSave(string fileName)
    {
        string filePath = string.Format("{0}/{1}.save", this.saveDataFolder, fileName);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(filePath);
        bf.Serialize(file, new GlobalStateSerializable(this.globalState));
        file.Close();
    }

    public void LoadSave(string fileName)
    {
        string filePath = string.Format("{0}/{1}.save", this.saveDataFolder, fileName);

        if (File.Exists(filePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filePath, FileMode.Open);
            GlobalStateSerializable serializableGlobalState = (GlobalStateSerializable)bf.Deserialize(file);
            file.Close();

            this.globalState.LoadFromSerializable(serializableGlobalState);
            this.globalCtrl.LoadFromState();
        }
    }

    public void ToggleSaveMenulInEditor()
    {
        this.saveMenu.SetActive(!this.saveMenu.activeSelf);
    }
}

public class RecordSlot
{
    public string name;

    [BoxGroup("Save Action")]
    public GameObject savePanel;
    [BoxGroup("Save Action")]
    public Button saveButton;

    [BoxGroup("Load Action")]
    public GameObject loadPanel;
    [BoxGroup("Load Action")]
    public Text title;
    [BoxGroup("Load Action")]
    public Button loadButton;
    [BoxGroup("Load Action")]
    public Button deleteButton;

    private SaveManager saveManager;


    public void Init(SaveManager saveManager)
    {
        this.saveManager = saveManager;
        this.title.text = this.name;

        bool fileExists = this.saveManager.ExistsSave(this.name);
        this.savePanel.SetActive(!fileExists);
        this.loadPanel.SetActive(fileExists);

        // Save panel

        this.saveButton.onClick.AddListener(() => {
            this.saveManager.CreateSave(this.name);
            this.savePanel.SetActive(false);
            this.loadPanel.SetActive(true);
        });

        // Load panel

        this.loadButton.onClick.AddListener(() => this.saveManager.LoadSave(this.name));
        this.deleteButton.onClick.AddListener(() => {
            this.saveManager.DeleteSave(this.name);
            this.savePanel.SetActive(true);
            this.loadPanel.SetActive(false);
        });
    }
}
