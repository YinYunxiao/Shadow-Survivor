/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace All
{
    public partial class ShopPanel : GComponent
    {
        public Controller canBuy;
        public GButton goBackButton;
        public GButton confirmButton;
        public GTextField characterInfo;
        public GList characterShopList;
        public const string URL = "ui://6hwzb14voeai8m";

        public static ShopPanel CreateInstance()
        {
            return (ShopPanel)UIPackage.CreateObject("All", "ShopPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            canBuy = GetControllerAt(0);
            goBackButton = (GButton)GetChildAt(2);
            confirmButton = (GButton)GetChildAt(5);
            characterInfo = (GTextField)GetChildAt(6);
            characterShopList = (GList)GetChildAt(7);
        }
    }
}