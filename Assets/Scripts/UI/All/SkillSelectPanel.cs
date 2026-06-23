/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace All
{
    public partial class SkillSelectPanel : GComponent
    {
        public GImage contentArea;
        public GList skillList;
        public const string URL = "ui://6hwzb14voeai8t";

        public static SkillSelectPanel CreateInstance()
        {
            return (SkillSelectPanel)UIPackage.CreateObject("All", "SkillSelectPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            contentArea = (GImage)GetChildAt(1);
            skillList = (GList)GetChildAt(3);
        }
    }
}