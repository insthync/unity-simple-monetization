# unity-simple-monetization

## Enable purchasing monetization

You have to enable Unity Purchasing as following steps via link https://docs.unity3d.com/Manual/UnityIAPSettingUp.html

## Enable advertisement monetization

You have to enable Unity Advertisements as following steps via link https://unity3d.com/services/ads/quick-start-guide

## How to use

Add **Monetization Manager** component to any game object, you can set Unity Advertisement configs, IAP products, Virtual currencies here

You can create In-Game product data by right click on project tab, Choose **Create** -> **Monetization** -> **In Game Product**, you can extends this class (`InGameProductData`) to make in game items with stats

You can create IAP product data by right click on project tab, Choose **Create** -> **Monetization** -> **IAP Product**. In IAP product you can set title, description and price by managable IAP catalog, You can learn how to manage catalog here https://docs.unity3d.com/Manual/UnityIAPCodelessIAP.html. Moreover you can add amount of virtual currencies, in game products
