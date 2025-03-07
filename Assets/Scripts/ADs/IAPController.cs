using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAPController : MonoBehaviour
{
  public void OnPurchaseComplete(string productID)
    {
        Debug.Log("Purchased");
    }
}
