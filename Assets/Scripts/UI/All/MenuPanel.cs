/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace All
{
    public partial class MenuPanel : GComponent
    {
        public Controller language;
        public GGraph startGameGraph;
        public GLabel coinDisplayLabel;
        public GButton characterButton;
        public GButton shopButton;
        public GButton settingButton;
        public const string URL = "ui://6hwzb14vtza26";

        public static MenuPanel CreateInstance()
        {
            return (MenuPanel)UIPackage.CreateObject("All", "MenuPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            language = GetControllerAt(0);
            startGameGraph = (GGraph)GetChildAt(0);
            coinDisplayLabel = (GLabel)GetChildAt(3);
            characterButton = (GButton)GetChildAt(4);
            shopButton = (GButton)GetChildAt(5);
            settingButton = (GButton)GetChildAt(6);
        }
    }
}