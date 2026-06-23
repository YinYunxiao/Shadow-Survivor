/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace All
{
    public partial class CharacterPanel : GComponent
    {
        public Controller canSelect;
        public GList characterList;
        public GButton goBackButton;
        public GButton confirmButton;
        public GTextField characterName;
        public GTextField characterInfo;
        public const string URL = "ui://6hwzb14voeai8j";

        public static CharacterPanel CreateInstance()
        {
            return (CharacterPanel)UIPackage.CreateObject("All", "CharacterPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            canSelect = GetControllerAt(0);
            characterList = (GList)GetChildAt(2);
            goBackButton = (GButton)GetChildAt(3);
            confirmButton = (GButton)GetChildAt(6);
            characterName = (GTextField)GetChildAt(7);
            characterInfo = (GTextField)GetChildAt(8);
        }
    }
}