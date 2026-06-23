/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace All
{
    public partial class PlayerInfoPanel : GComponent
    {
        public GList buffList;
        public GButton goBackButton;
        public GTextField baseInfoText;
        public const string URL = "ui://6hwzb14vlqw88w";

        public static PlayerInfoPanel CreateInstance()
        {
            return (PlayerInfoPanel)UIPackage.CreateObject("All", "PlayerInfoPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            buffList = (GList)GetChildAt(3);
            goBackButton = (GButton)GetChildAt(4);
            baseInfoText = (GTextField)GetChildAt(5);
        }
    }
}