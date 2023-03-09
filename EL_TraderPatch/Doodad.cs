using System;
using UnityEngine;
using UnityEngine.Serialization;
using EpicLoot;
using EpicLoot.Adventure;

public class Doodad : MonoBehaviour
{
        public bool mIsSetFromHaldor = false;

        public void SetPanel()
        {
                switch (mIsSetFromHaldor)
                {
                        case true:
                                if(StoreGui_Patch.MerchantPanel.gameObject.activeSelf)return;
                                StoreGui_Patch.MerchantPanel.gameObject.SetActive(true);
                                mIsSetFromHaldor = false;
                                break;
                        case false:
                                StoreGui_Patch.MerchantPanel.gameObject.SetActive(false);
                                break;
                }
        }
}