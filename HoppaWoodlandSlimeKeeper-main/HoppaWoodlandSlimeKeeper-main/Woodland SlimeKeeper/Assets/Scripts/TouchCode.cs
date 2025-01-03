using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchCode : MonoBehaviour
{
    [SerializeField]GameObject target;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray;
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouse = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            //transform.position = new Vector3(mouse.x, mouse.y, transform.position.z);
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("touch");
                target.transform.position = hit.point;
            }

        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            //Vector2 touchPosition = touch.position;

            if(touch.phase == TouchPhase.Ended) //TouchPhase.Began
            {
                ray = Camera.main.ScreenPointToRay(touch.position);
                
                if(Physics.Raycast(ray,out hit))
                {
                    Debug.Log("touch");
                    target.transform.position = hit.point;
                }
            }
        }
    }
}
