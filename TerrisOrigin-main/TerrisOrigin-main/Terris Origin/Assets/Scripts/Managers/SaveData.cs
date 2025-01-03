using UnityEngine;
using System.Collections;

public class SaveData
{
	
    public int kills = 0;
    public int Level = 0;
    public Vector3 playerPos=new Vector3();
    public Quaternion playerRotate= new Quaternion();

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetKills(int _kills)
    {
        kills = _kills;
    }
    public void SetLevel(int _Level)
    {
        Level = _Level;
    }

    public void SetPlayer(GameObject _player)
    {
        
        playerPos = _player.transform.position;
        playerRotate = _player.transform.rotation;

    }

    public int GetKills()
    {
        return kills;
    }

    public int GetLevel()
    {
        return Level;
    }

    public GameObject GetPlayer()
    {
        GameObject player=new GameObject();
        player.transform.position=playerPos;
        player.transform.rotation = playerRotate;
        return player;
    }
}

