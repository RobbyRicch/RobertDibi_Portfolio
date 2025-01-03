using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IndiciumGardenManager : MonoBehaviour
{
    [SerializeField] private GameObject CollectorGO;
    [SerializeField] private GameObject BatGO;

    int rand1;
    int rand2;
    int rand3;
    int rand4;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in PlayerManager.Instance.AllPlayersAlive)
        {
            item.AddComponent<PlayerLives>();
            item.AddComponent<Bank>();
            item.AddComponent<PlayerRole>();
        }

        rand1 = Random.Range(0, 4);
        rand2 = Random.Range(0, 4);
        while (rand2 == rand1)
        {
            rand2 = Random.Range(0, 4);
        }
        rand3 = Random.Range(0, 4);
        while (rand3 == rand1 || rand3 == rand2)
        {
            rand3 = Random.Range(0, 4);
        }
        rand4 = Random.Range(0, 4);
        while (rand4 == rand1 || rand4 == rand2 || rand4 == rand3)
        {
            rand4 = Random.Range(0, 4);
        }
        PlayerManager.Instance.AllPlayersAlive[0].GetComponent<PlayerRole>().RoleNum = rand1;
        PlayerManager.Instance.AllPlayersAlive[1].GetComponent<PlayerRole>().RoleNum = rand2;
        PlayerManager.Instance.AllPlayersAlive[2].GetComponent<PlayerRole>().RoleNum = rand3;
        PlayerManager.Instance.AllPlayersAlive[3].GetComponent<PlayerRole>().RoleNum = rand4;
        foreach (PlayerInputHandler player in PlayerManager.Instance.AllPlayersAlive)
        {
            player.GetComponent<PlayerRole>().SetRoles();
            player.GetComponent<Bank>().SetBankSize();
            if (player.GetComponent<PlayerRole>().Role == PlayerRole.Roles.Encryptor)
            {
                // if not work probably due to changes in player model
                Instantiate(CollectorGO, player.transform.GetChild(0).position, Quaternion.identity, player.transform.GetChild(0));
            }
            else if (player.GetComponent<PlayerRole>().Role == PlayerRole.Roles.Infiltrator1)
            {
                Instantiate(BatGO, player.transform.GetChild(0).position, BatGO.transform.rotation, player.transform.GetChild(0));
            }
            else if (player.GetComponent<PlayerRole>().Role == PlayerRole.Roles.Infiltrator2)
            {
                Instantiate(BatGO, player.transform.GetChild(0).position, BatGO.transform.rotation, player.transform.GetChild(0));
            }
            else if (player.GetComponent<PlayerRole>().Role == PlayerRole.Roles.Disruptor)
            {
                Instantiate(BatGO, player.transform.GetChild(0).position, BatGO.transform.rotation, player.transform.GetChild(0));
            }
        }
    }
}
