/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace All
{
    public partial class Joystick : GComponent
    {
        public GGraph back;
        public GGraph stick;
        public const string URL = "ui://6hwzb14vtza28d";

        public static Joystick CreateInstance()
        {
            return (Joystick)UIPackage.CreateObject("All", "Joystick");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            back = (GGraph)GetChildAt(0);
            stick = (GGraph)GetChildAt(1);
        }
    }
}