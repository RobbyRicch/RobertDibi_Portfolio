using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    [Header("Table Properties")]
    [SerializeField] private List<GameObject> TableComponents;
    [SerializeField] private GameObject _tableCam;
    [SerializeField] private bool _viewingTable;
    [SerializeField] private bool _hasSwitchedComponentsToTable;

    [Header("Creature Barn Properties")]
    [SerializeField] private List<GameObject> BarnComponents;
    [SerializeField] private GameObject _barnCam;
    [SerializeField] private bool _viewingBarn;
    [SerializeField] private bool _hasSwitchedComponentsToBarn;

    // Update is called once per frame
    void Update()
    {
        if (_viewingBarn && !_hasSwitchedComponentsToBarn)
        {

            SwitchToBarn();
        }

        if (_viewingTable && !_hasSwitchedComponentsToTable)
        {
            SwitchToTable();
        }


    }

    private void SwitchToBarn()
    {
        foreach (GameObject BarnObjects in BarnComponents)
        {
            BarnObjects.SetActive(true);
        }

        foreach (GameObject TableObjects in TableComponents)
        {
            TableObjects.SetActive(false);
        }

        _tableCam.SetActive(false);
        _barnCam.SetActive(true);
        _hasSwitchedComponentsToBarn = true;
        _hasSwitchedComponentsToTable = false;
    }

    private void SwitchToTable()
    {
        foreach (GameObject BarnObjects in BarnComponents)
        {
            BarnObjects.SetActive(false);
        }

        foreach (GameObject TableObjects in TableComponents)
        {
            TableObjects.SetActive(true);
        }

        _tableCam.SetActive(true);
        _barnCam.SetActive(false);
        _hasSwitchedComponentsToBarn = false;
        _hasSwitchedComponentsToTable = true;
    }

    public void SwitchBool(bool barnOrTable)
    {
        if (barnOrTable)
        {
            _viewingBarn = true;
            _viewingTable = false;
        }
        else
        {
            _viewingBarn = false;
            _viewingTable = true;
        }

    }


}
