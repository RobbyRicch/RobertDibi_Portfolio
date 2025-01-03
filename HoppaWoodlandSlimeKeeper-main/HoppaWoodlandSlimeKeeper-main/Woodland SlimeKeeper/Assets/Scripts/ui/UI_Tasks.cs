using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_Tasks : MonoBehaviour
{ //on Tasks_Canvas gameobject
    public GameObject colorTaskPrefab;
    public GameObject toppingsTaskPrefab;
    public List<Texture2D> toppingsTextures;
    public Transform parentTransform;
    
    public Color taskColor;
   
    public List<GameObject> taskGOColors;
    public List<GameObject> taskGOToppings;
   // public List<string> taskToppings;

    public Timed_ui UI_Task_done;

    private void OnEnable()
    {
        Actions.PlayerTaskDone += WhenTaskDone;
    }

    private void OnDisable()
    {
        Actions.PlayerTaskDone -= WhenTaskDone;
    }

    public void RemoveTaskColorUI(int taskIndex)
    {
        //Debug.Log("RemoveTaskColorUI" + taskIndex);
        if (taskGOColors[taskIndex].gameObject == null)
            return;
        Destroy(taskGOColors[taskIndex].gameObject);
        if (taskGOColors[taskIndex] == null)
            return;
        taskGOColors.RemoveAt(taskIndex); //remove item from the list
    }

    public void RemoveTaskToppingUI(int taskIndex)
    {
        //Debug.Log("RemoveTaskToppingUI" + taskIndex);
        if (taskGOToppings[taskIndex].gameObject == null)
            return;
        Destroy(taskGOToppings[taskIndex].gameObject);
        if (taskGOToppings[taskIndex] == null)
            return;
        taskGOToppings.RemoveAt(taskIndex); //remove item from the list
    }

    /*
    public void CheckToppingAndRemove(string toppingPicked)
    {
        //string [] getTmp;
        string  getTmp;
        for (int i = 0; i < taskToppings.Count; i++)
        {
            //getTmp = taskToppings[i].Split('.');
            getTmp = taskToppings[i];
           // Debug.Log(getTmp[1] +"|" + toppingPicked);
            if (getTmp.ToLower()==toppingPicked.ToLower())
            {
                //RemoveTaskUI(int.Parse(getTmp[0]));
                RemoveTaskToppingUI(i);
                break;
            }

        }
    }*/
    public void SetTaskColor(Color deTaskColor)
    {
        taskColor = deTaskColor;
        GameObject go= Instantiate(colorTaskPrefab, parentTransform);
        // Handle vial touch
        ImageColor imgC = go.GetComponentInChildren<ImageColor>();
        //Debug.Log("VialColor: " + imgC.color);
        imgC.color = deTaskColor;
        taskGOColors.Add(go);
        
    }

    public void SetTaskTopping(string deTopping)//starPINK
    {

        GameObject go = Instantiate(toppingsTaskPrefab, parentTransform);
        //set Toppings in task
        RawImage m_RawImage = go.GetComponentInChildren<RawImage>();

        for (int i = 0; i < toppingsTextures.Count; i++)
        {
            if (ThisIsTheTopping(deTopping,toppingsTextures[i].name))
            {
                //Debug.Log("HHH" +  toppingsTextures[i].name+"JJJ"+ (deTopping));
                m_RawImage.texture = toppingsTextures[i];
                //deTopping = toppingsTextures[i].name;
                break;
            }
            
        }
        
        taskGOToppings.Add(go);
        //taskToppings.Add(deTopping);
        //taskToppings.Add(taskGOToppings.Count-1+ "."+deTopping);
    }
    
    private bool ThisIsTheTopping(string strTopping,string strCompare)
    {
        string chck = "";
        strTopping = strTopping.ToLower();
        switch (strTopping)
        {
            case "starpink":
                chck = "star_pink";
                break;
            case "starmint":
                chck = "star_mint";
                break;
            case "starpurple":
                chck = "star_purp";
                break;
            default:
                break;
        }
        return strCompare.Contains(chck);
        //return false;
    }
    private void WhenTaskDone(bool isDone)
    {
        if (isDone)
        {
            UI_Task_done.ShowMe_UI();
            //set the next task when done
            Actions.SetNewTask(true);
        }
    }
}
