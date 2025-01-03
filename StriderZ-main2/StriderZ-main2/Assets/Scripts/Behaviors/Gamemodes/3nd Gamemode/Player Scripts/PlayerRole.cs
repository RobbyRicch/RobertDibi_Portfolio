using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRole : MonoBehaviour
{
    public enum Roles
    {
        Encryptor,
        Infiltrator1,
        Infiltrator2,
        Disruptor
    }

    public Roles Role;
    public int RoleNum;
    public void SetRoles()
    {
        switch (RoleNum)
        {
            case 0:
                Role = Roles.Encryptor;
                break;
            case 1:
                Role = Roles.Infiltrator1;
                break;
            case 2:
                Role = Roles.Infiltrator2;
                break;
            case 3:
                Role = Roles.Disruptor;
                break;
        }
    }
}
