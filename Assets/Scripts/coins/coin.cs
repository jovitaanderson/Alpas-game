using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coin : MonoBehaviour
{
   
    public int coinValue = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
           
            AudioManager.i.PlaySfx(AudioId.ItemObtained, pauseMusic: true);
            //coinsManager.instance.ChangeMoney(coinValue);
            Wallet.i.AddMoney(coinValue);
        }
    }

   
}
