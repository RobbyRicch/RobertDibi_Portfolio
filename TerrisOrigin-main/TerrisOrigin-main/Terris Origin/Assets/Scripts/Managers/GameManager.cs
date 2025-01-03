using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    private static int kills = 0;
    private static int XP =0;
    private static int levelkills = 5;

    private static SaveData data= new SaveData();
    private static GameObject player;
    private bool flag = false;
    private static List<Level> Levels=new List<Level>();
    private Dictionary<int, List<Level>> levelsKills = new Dictionary<int, List<Level>>();
    private static int currentlevel = 0;
    public TMP_Text XP_txt;
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        player = GameObject.Find("Player");
        loadfromdata();
        LoadLevels();
        //initializeDict();

    }
    // Update is called once per frame
    void Update()
    {
        UnlockLevel();
        if (flag==false)
        {
            flag = true;
        }
        //XP_txt.text = ("XP "+XP.ToString());
    }
  
    public static void IncreaseKills()
    {
        kills++;
    }
    public static void xpIncrease(string enemy)
    {
        switch (enemy)
        {
            case "Q":
                {
                    XP += 50;
                    
                    break;
                }
            case "R":
                {
                    XP += 100;
                    break;
                }
            default:
                {
                    break;
                }
        }
        
    }
    public static void UnlockLevel()
    {
        if (currentlevel >= Levels.Count)
            return;
        Level level = Levels[currentlevel];
        print(level.Getbarrier());
        GameObject Barrier = GameObject.Find(level.Getbarrier());
        if (Barrier==null)
        {
            return;
        }
        else
        {

            Renderer rend=Barrier.GetComponent<Renderer>();
            Material mat = rend.material;
            Color color = mat.color;
            float a=(float)((float)kills / (float)level.Getkills());
            
            color.a =1.0f-a;
            
            if (color.a > 0.8f  )
                color.a = 0.8f  ;
            else if (color.a < 0.2f  )
                color.a = 0.2f  ;
            
            mat.color = color;

            if (a - 1 >= 0)
            {
                Barrier.SetActive(false);
                if(currentlevel+1<Levels.Count)
                    currentlevel++;
                
                level.pass();
                savedata();
            }
        }
    }
    public static void savedata()
    {
        data.SetKills(kills);
        data.SetPlayer(GameObject.Find("Player"));
        data.SetLevel(currentlevel);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Application.dataPath + "/Scenes/AshenRuins/Data/Save.jason", json);

    }
    public static void loadfromdata()
    {
        string json = File.ReadAllText(Application.dataPath + "/Scenes/AshenRuins/Data/Save.jason");
        data = JsonUtility.FromJson<SaveData>(json);
        //GameObject.Find("Player").transform.position = data.GetPlayer().transform.position;
        //GameObject.Find("Player").transform.rotation = data.GetPlayer().transform.rotation;
        kills = data.kills;
        currentlevel = data.GetLevel();
    }
    public void initializeDict()
    {
        for (int i = 0; i < Levels.Count; i++)
        {
            if (levelsKills.ContainsKey(Levels[i].Getkills()))
            {
                levelsKills[Levels[i].Getkills()].Add(Levels[i]);
            }
            else
            {
                List<Level> lev = new List<Level>();
                lev.Add(Levels[i]);
                levelsKills.Add(Levels[i].Getkills(), lev);
            }
        }
    }
    public void LoadLevels()
    {
        string[] files = Directory.GetFiles(Application.dataPath + "/Scenes/AshenRuins/Data/Levels");

        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].IndexOf("Level") > 0 && files[i].EndsWith("jason"))
            {
                string json = File.ReadAllText(files[i]);
                Levels.Add(JsonUtility.FromJson<Level>(json));
            }

        }
    }
}
