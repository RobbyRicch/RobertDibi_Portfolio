using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/*manage the user tasks*/
public class SlimePot_Tasks : MonoBehaviour
{ //on SlimeTasks gameobject
    [Header("General")]
    public List<Color> useColor = new List<Color>(); //holds the colors from database
    public List<string> useTexture = new List<string>();
    public List<string> useTopping = new List<string>();

    private string[] splitArrayTasks;

    public int taskNumber;
    //[Mix Color 1 Red and Blue + Texture soft+ Toppings 2 Stars and 1 Melons => 1,3|1|2*1,1*3]
   // public enum Mixing { Colors, Textures, Toppings } /*1-Colors|2-Textures|3-Toppings*/
  //  public enum MixTextures { Soft, Hard, Fluff, Crunch }
  //  public enum MixToppings { mint star, starpink, starpurple }

    //public bool IsTaskComplete;

    [Header("UI")]
    public UI_Tasks uiTasks; //controls the ui of tasks 

    private void OnEnable()
    {
        Actions.SetNewTask += WhenTaskDone;
    }

    private void OnDisable()
    {
        Actions.SetNewTask -= WhenTaskDone;
    }
    void Start()
    {
        //pull tasks from database?
        SetTasksForUser();
        //IsTaskComplete = false;
    }

    /*database or dictionary per user of tasks*/
    private void SetTasksForUser()//set id user and task for user by id?
    {
        /*numbers _*///current task number will be the first numbers before the sign '_'
        /*XXXX_[COLOR][COLOR]|TEXTURE|TOPPING,TOPPING*/
        //list of tasks will be seperated with '~'
        //Mix Color 1 Red and Green + Toppings 2 Stars pink

        /*red|||blue|||*/
        string textFromFile = "1_[1.000,0.000,0.000,0.784]||~[0.000,0.000,1.000,0.784]||~[1.000,0.000,0.000,0.784][0.000,1.000,0.000,0.784]||~[1.000,0.000,0.000,0.784][0.000,1.000,0.000,0.784]||starpink,starmint,starpurple~";
        
        taskNumber = int.Parse(textFromFile.Split("_")[0]); 
        string tasksText = textFromFile.Split("_")[1];

        //Split the tasks 
        splitArrayTasks = tasksText.Split("~");

        SetTaskTextUI(splitArrayTasks[taskNumber]);
    }

    private void SetTaskTextUI(string taskString)
    {
        if (taskString == "")
            return;
        string[] tasksText = taskString.Split("|");
        string[] deColors = tasksText[0].Split("[");
        if (tasksText[0] != "")/*COLORs*/
        {
            for (int i = 0; i < deColors.Length; i++)
            {
                
                if (deColors[i] != "")
                {
                    string str = deColors[i].Substring(0, deColors[i].Length - 1);
                    string[] ss = str.Split(",");
                    Color deRGBcolor = new Color(float.Parse(ss[0]), float.Parse(ss[1]), float.Parse(ss[2]), float.Parse(ss[3]));
                    useColor.Add(deRGBcolor);
                    uiTasks.SetTaskColor(deRGBcolor);
                    //Debug.Log("color:" + useColor.Last());
                    
                }
            }

        }

        if (tasksText[1] != "")/*TEXTURE*/
            useTexture.Add(tasksText[1]);
        
        if (tasksText[2] != "")/*TOPPINGs*/
        { 
            string[] deToppings = tasksText[2].Split(",");
            
            for (int j = 0; j < deToppings.Length; j++)
            {
                if (deToppings[j] != "")
                {
                    uiTasks.SetTaskTopping(deToppings[j]);//starpink
                    useTopping.Add(deToppings[j]);
                    
                }
            }
        }
        
    }
 
    public void IsColorInTasks(Color colorPicked)
    {
        for (int i = 0; i < useColor.Count; i++)//look in task colors list
        {
           // Debug.Log((useColor[i].ToString()) + "WWW" + (colorPicked.ToString("F3")));
            if (useColor[i].ToString() == (colorPicked.ToString("F3")))
            {
                useColor.RemoveAt(i); //remove item from the list            
                uiTasks.RemoveTaskColorUI(i); //remove the item in ui
                
                if (IsTaskEnded()){
                    // IsTaskComplete = true;

                    Actions.PlayerTaskDone(true);
                    

                }
                break;
            }
        }
    }

    public void IsToppingInTasks(string toppingPicked)
    {
        toppingPicked = toppingPicked.ToString().ToLower();
        string tmpStr = "";
        for (int i = 0; i < useTopping.Count; i++)//look in task colors list
        {
            tmpStr = useTopping[i].ToString().ToLower();
          //  Debug.Log(tmpStr + "??" + toppingPicked);
            if ( tmpStr== toppingPicked)
            {
                useTopping.RemoveAt(i); //remove item from the list
                uiTasks.RemoveTaskToppingUI(i);
                //Debug.Log("this is it");
                if (IsTaskEnded())
                {
                    // IsTaskComplete = true;

                    Actions.PlayerTaskDone(true);


                }
                break;
            }
        }
    }

    /*check lists count is zero-colors and textures and toppings*/
    private bool IsTaskEnded()
    {
        //Debug.Log("useColor.Count: "+useColor.Count);
        //Debug.Log(useTexture.Count);
        //Debug.Log(useTopping.Count);
        if (useColor.Count != 0)
            return false;
        if (useTexture.Count != 0)
            return false;
        if (useTopping.Count !=0)
            return false;
        return true;
    }

    private void WhenTaskDone(bool isDone)
    {
        if (isDone)
        {
            if (splitArrayTasks.Length == taskNumber)
                return;
            //set next task
            taskNumber++;
            Debug.Log("Next Task:"+splitArrayTasks[taskNumber]);
            SetTaskTextUI(splitArrayTasks[taskNumber]);
        }
    }
}
