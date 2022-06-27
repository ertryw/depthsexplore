using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(UserPreferences))]
public class UserPreferencesEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Force Save"))
        {
            (target as UserPreferences).Save();
        }

        if (GUILayout.Button("Reset Save"))
        {
            (target as UserPreferences).ResetSave();
        }

    }
}
#endif

public class UserPreferences : MonoBehaviour
{
    public delegate void FirstPlayDelegate();
    public static event FirstPlayDelegate onFirstPlay;
    public delegate void OnNextPlay();
    public static event OnNextPlay onNextPlay;

    private string userPreferencesFile = @"/userPreferences.dat";
    private static UserPreferences _instance;
    [Space(10)]
    public PlayerData playerData;
    System.DateTime lastRemoteSaveDate = new System.DateTime(1900, 1, 1);
    public float remoteSaveInterval = 60;
    public bool init;

    [DllImport("__Internal")]
    private static extern void JS_FileSystem_Sync();

    public static UserPreferences instance
    {
        get
        {
            return GetInstance();
        }
        private set
        {
            _instance = value;
        }
    }

    private static UserPreferences GetInstance()
    {
        if (_instance == null)
        {
            FindOrCreateInstance();
        }
        return _instance;
    }

    private static void FindOrCreateInstance()
    {
        UserPreferences uPrefs = FindObjectOfType<UserPreferences>();
        if (uPrefs != null)
        {
            _instance = uPrefs;
        }
        else
        {
            CreateNewUserPreferences();
        }
    }

    private static void CreateNewUserPreferences()
    {
        GameObject utilities = GetUtilitiesGameObject();
        UserPreferences userPreferences = utilities.AddComponent<UserPreferences>();
        _instance = userPreferences;
    }

    private static GameObject GetUtilitiesGameObject()
    {
        GameObject utilities = GameObject.Find("Utilities");
        if (utilities == null)
        {
            utilities = new GameObject("Utilities");
            DontDestroyOnLoad(utilities);
        }
        return utilities;
    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        InitializePlayerData();
    }

    public void Load()
    {
        string filePath = Application.persistentDataPath + userPreferencesFile;

        Debug.Log(filePath);

        if (CryptedFileSaver.IsFileExists(filePath))
        {
            PlayerData data = CryptedFileSaver.LoadFileWithBinaryFormater<PlayerData>(filePath);
            playerData = data;
            onNextPlay?.Invoke();
        }
        else
        {
            if (playerData == null)
            {
                playerData = new PlayerData();
            }
            
            onFirstPlay?.Invoke();
        }


    }

    private void InitializePlayerData()
    {
        Load();
    }

    [ContextMenu("Force Save")]
    public void Save()
    {
        UpdateDateOfPlayerDataSave();
        SaveLocal();
    }

    public void ResetSave() 
    {
        playerData = new PlayerData();
        Save();
    }

    public void ResetData()
    {
        Save();
    }

    public void DeleteLocal()
    {
        playerData = new PlayerData();
        File.Delete(Application.persistentDataPath + userPreferencesFile);
        Load();
    }

    protected void SaveLocal()
    {
        string filePath = Application.persistentDataPath + userPreferencesFile;
        CryptedFileSaver.SaveFileWithBinaryFormater(filePath, playerData);
    }

    protected void UpdateDateOfPlayerDataSave()
    {
        playerData.saveDate = System.DateTime.Now.ToString();
    }
}


