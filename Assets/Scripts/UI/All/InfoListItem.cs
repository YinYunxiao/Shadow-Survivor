/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace All
{
    public partial class InfoListItem : GComponent
    {
        public GTextField title;
        public const string URL = "ui://6hwzb14vlqw88x";

        public static InfoListItem CreateInstance()
        {
            return (InfoListItem)UIPackage.CreateObject("All", "InfoListItem");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            title = (GTextField)GetChildAt(1);
        }
    }
}