using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_UserIcon : MonoBehaviour
{
   [SerializeField]
    private GameObject deImgPrefab;
    [SerializeField]
    private GameObject deContainer;
    [SerializeField]
    private Image m_Image;
    [SerializeField]
    private Sprite m_ImagePrefab;

    [SerializeField]
    private Sprite[] m_SpriteArray;

    private int svIconChoosen;
    private int tmpSvIconChoosen;
    private GameObject[] m_ButtonArray;



    private void Awake()
    {
        m_ButtonArray = new GameObject[m_SpriteArray.Length];
        for (int i = 0; i < m_SpriteArray.Length; i++)
        {
            //Debug.Log(i + "|" + m_SpriteArray[i].name);
            m_ButtonArray[i] = Instantiate(deImgPrefab,deContainer.transform);
            m_ButtonArray[i].name = "Icon" + i;

            m_ButtonArray[i].GetComponent<UI_BtnIcon>().SetIconNum(i);
            m_ButtonArray[i].GetComponent<UI_BtnIcon>().SetSpriteIcon(m_SpriteArray[i]);

        }
    }
    private void Start()
    {
        if (PlayerPrefs.HasKey("LastIcon"))
        {
            svIconChoosen = int.Parse( PlayerPrefs.GetString("LastIcon"));
            m_Image.sprite = m_SpriteArray[svIconChoosen];
        }
        else
            svIconChoosen = 0;
        
    }

    private void OnEnable()
    {
        Actions.IconChange += SetIconChosen;
    }

    private void OnDisable()
    {
        Actions.IconChange -= SetIconChosen;
    }

    private void OnApplicationQuit()
    {
        string str = PlayerPrefs.GetString("deletekeyall");
        //Debug.Log(str + "__");
        if (str == "1")
        {
            // Debug.Log("HasKey: "+PlayerPrefs.HasKey("LastLvl"));
            //PlayerPrefs.DeleteKey("LastLvl");
        }
        else
        {

           
            PlayerPrefs.SetString("LastIcon", "" + svIconChoosen);

        }

    }
    private void SetIconChosen(int _deNum)
    {
        tmpSvIconChoosen = _deNum;
       // Debug.Log("_denum:"+ _deNum);
    }
    public void BTNIconOK()
    {
        svIconChoosen = tmpSvIconChoosen;
        m_Image.sprite = m_SpriteArray[svIconChoosen];
        PlayerPrefs.SetString("LastIcon", "" + svIconChoosen);
        //Debug.Log("HHH" + svIconChoosen);
        /* for (int i = 0; i < m_ButtonArray.Length; i++)
         {
             bool answear = m_ButtonArray[i].GetComponent<UI_BtnIcon>().GetIsSelected();
            // Debug.Log(i + "|" + answear);
             if (answear)
             {
                 m_Image.sprite = m_SpriteArray[i];

             }
         }*/
    }

    
}
