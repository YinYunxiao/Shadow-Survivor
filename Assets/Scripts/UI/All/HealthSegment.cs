/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace All
{
    public partial class HealthSegment : GComponent
    {
        public GLoader hp;
        public GLoader shield;
        public GImage frame;
        public const string URL = "ui://6hwzb14vkhuk8v";

        public static HealthSegment CreateInstance()
        {
            return (HealthSegment)UIPackage.CreateObject("All", "HealthSegment");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            hp = (GLoader)GetChildAt(0);
            shield = (GLoader)GetChildAt(1);
            frame = (GImage)GetChildAt(2);
        }
    }
}