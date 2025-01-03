using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimePot_Manager : MonoBehaviour
{
    [Header("General")]
    public Camera mainCamera; // Reference to the main camera
    public GameObject cauldron; // Reference to the cauldron
    public Transform dropPoint; // Reference to the cauldron
    public GameObject[] toppingPrefabs; // Array of topping prefabs
    public SkinnedMeshRenderer slimeMesh;
    [Header("Toppings")]
    public List<MeshFilter> toppings;
    public List<MeshRenderer> toppingColors;

    [Header("Animators")]
    public Animator _spoonAnimator;
    public Animator _cameraAnimator;

    [Header("Timers")]
    public float _timetoStir;
    public float _timeToSwitchToSlime;
    private List<Color> currentColors = new List<Color>(); // List to store the current colors

    //Danit
    [Header("Tasks")]
    public SlimePot_Tasks slimeTasks;

    [Header("UI")]
    public UI_Manager ui_manager;
    public UI_Energy ui_energy;
    private int deEnergy;
    private int MinusEnergy=4;

    
 

    private void Awake()
    {
        //PlayerPrefs.SetString("LastEnergy", "100");
    }

    private void OnEnable()
    {

        Actions.PlayerTaskDone += TaskDoneCallUI;
    }
    private void OnDisable()
    {

        Actions.PlayerTaskDone -= TaskDoneCallUI;
    }

    private void Update()
    {
#if UNITY_EDITOR
        // Simulate touch with mouse input for editor testing
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse click detected at position: " + Input.mousePosition);
            HandleTouch(Input.mousePosition);
        }
#else
        // Check if there are any touches on the device
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Debug.Log("Touch detected at position: " + touch.position);
                HandleTouch(touch.position);
            }
        }
#endif
    }

    private void HandleTouch(Vector3 touchPosition)
    {
        // Convert touch position to a ray
        Ray ray = mainCamera.ScreenPointToRay(touchPosition);
        RaycastHit hit;

        // Perform raycast to detect touched object
        if (Physics.Raycast(ray, out hit))
        {
            GameObject touchedObject = hit.collider.gameObject;
            Debug.Log("Touched object: " + touchedObject.name);

            // Check if the touched object is a vial or a topping
            if (touchedObject.CompareTag("Vial"))
            {
                Debug.Log("Vial touched: " + touchedObject.name);
                // Handle vial touch
                Vial vial = touchedObject.GetComponent<Vial>();
                if (vial != null)
                {
                    
                    AddColor(vial.color);
                }
                else
                {
                    Debug.LogError("Vial script not found on touched object");
                }
            }
            else if (touchedObject.CompareTag("Topping"))
            {
                Debug.Log("Topping touched: " + touchedObject.name);
                // Handle topping touch
                Topping topping = touchedObject.GetComponent<Topping>();
                if (topping != null)
                {
                    InstantiateTopping(topping.toppingName);
                }
                else
                {
                    Debug.LogError("Topping script not found on touched object");
                }
            }
            else if (touchedObject.CompareTag("Cauldron"))/*mix it all*/
            {
                if (ui_manager.IsUIOpen())//if a UI window is open, do not mix
                    return;
                ui_manager.SetUIOnSlime();
                deEnergy = ui_energy.GetUI_Energy();
               
                if(deEnergy - MinusEnergy < 0)
                {
                    ui_manager.OpenUIOutOfEnergy();
                    return;
                }
                
                deEnergy = deEnergy - MinusEnergy; 

                ui_energy.SetUI_Energy(deEnergy);
                
                StartCoroutine(TransitionToSlime());
            }

        }
        else
        {
            Debug.Log("No object hit by raycast");
        }
    }

   /* public void TransitionFromSlime()//on click BtnBackToMix
    {
        ui_manager.BTNStart();
        //Debug.Log("animate back to cauldron");
    }
   */
    private IEnumerator TransitionToSlime()
    {
        _spoonAnimator.SetTrigger("Stir");
        _cameraAnimator.SetTrigger("Cauldron");
        yield return new WaitForSeconds(_timetoStir);
        _cameraAnimator.SetTrigger("Slime");
    }

    private void AddColor(Color color)
    {
        slimeTasks.IsColorInTasks(color);
        if (currentColors.Count < 3)
        {
            currentColors.Add(color);
        }
        else
        {
            currentColors[0] = currentColors[1];
            currentColors[1] = currentColors[2];
            currentColors[2] = color;
        }

        UpdateCauldronAppearance();
    }

    private void UpdateCauldronAppearance()
    {
        if (currentColors.Count == 0)
            return;

        Color blendedColor = BlendColors(currentColors);
        
        //slimeTasks.IsColorInTasks(blendedColor);
        Debug.Log("Updating cauldron appearance: color = " + blendedColor);
        // Update the cauldron's appearance
        Renderer cauldronRenderer = cauldron.GetComponent<Renderer>();
        if (cauldronRenderer != null)
        {
            cauldronRenderer.material.color = blendedColor;
            slimeMesh.material.color = blendedColor;
        }
        else
        {
            Debug.LogError("Cauldron Renderer not found");
        }
    }

    private Color BlendColors(List<Color> colors)
    {
        Color blendedColor = new Color(0, 0, 0, 0);

        foreach (Color color in colors)
        {
            blendedColor += color;
        }

        blendedColor /= colors.Count;

        return blendedColor;
    }

    private void InstantiateTopping(string toppingName)
    {
        Debug.Log("Instantiating topping: " + toppingName);
        slimeTasks.IsToppingInTasks(toppingName);
        /// Find the correct topping prefab by name
        GameObject toppingPrefab = System.Array.Find(toppingPrefabs, prefab => prefab.name == toppingName);
        if (toppingPrefab != null)
        {
            foreach (MeshFilter toppingmesh in toppings)
            {
                toppingmesh.mesh = toppingPrefab.GetComponent<MeshFilter>().mesh;
            }

            foreach (MeshRenderer toppingcolors in toppingColors)
            {
                toppingcolors.material = toppingPrefab.gameObject.GetComponent<Renderer>().material;
            }
        }
        else
        {
            Debug.LogError("Topping prefab not found for: " + toppingName);
        }
    }

  
    private void TaskDoneCallUI(bool taskDone)
    {
        if (taskDone)
        {
           
            Debug.Log("Task Done!");
        }
    }


}
