using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bank : MonoBehaviour
{
    public int BankCurrentAmount = 0;
    public int BankMaxSize;
    public int SpeedDebuff;
    [Range(1, 100)]
    public int AmountToLoseOnHit = 1;

    private PlayerRole PlayerRole;

    public void SetBankSize()
    {
        PlayerRole = GetComponent<PlayerRole>();
        switch (PlayerRole.Role)
        {
            case PlayerRole.Roles.Encryptor:
                BankMaxSize = 4000;
                break;
            case PlayerRole.Roles.Infiltrator1:
                BankMaxSize = 300;
                break;
            case PlayerRole.Roles.Infiltrator2:
                BankMaxSize = 300;
                break;
            case PlayerRole.Roles.Disruptor:
                BankMaxSize = 100;
                break;
        }
    }

    public void PlayerSpeedDebuff()
    {
        switch (PlayerRole.Role)
        {
            case PlayerRole.Roles.Encryptor:
                if (BankCurrentAmount >= (BankMaxSize / 2) && BankCurrentAmount < BankMaxSize)
                {
                    SpeedDebuff = 20;
                }
                else if (BankCurrentAmount == BankMaxSize)
                {
                    SpeedDebuff = 40;
                }
                break;
            case PlayerRole.Roles.Infiltrator1:
                if(BankCurrentAmount == BankMaxSize)
                {
                    SpeedDebuff = 20;
                }
                break;
            case PlayerRole.Roles.Infiltrator2:
                if (BankCurrentAmount == BankMaxSize)
                {
                    SpeedDebuff = 20;
                }
                break;
            case PlayerRole.Roles.Disruptor:
                if (BankCurrentAmount == BankMaxSize)
                {
                    SpeedDebuff = 20;
                }
                break;
        }
    }

    public void UseBank(int amount)
    {
        BankCurrentAmount += amount;
        if (BankCurrentAmount > BankMaxSize && PlayerRole.Role != PlayerRole.Roles.Encryptor)
        {
            BankCurrentAmount = BankMaxSize;
        }
        PlayerSpeedDebuff();
    }
}
