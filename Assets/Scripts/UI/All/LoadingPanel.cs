/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace All
{
    public partial class LoadingPanel : GComponent
    {
        public GImage loadingBack;
        public GTextField loadingText;
        public const string URL = "ui://6hwzb14vhfmw0";

        public static LoadingPanel CreateInstance()
        {
            return (LoadingPanel)UIPackage.CreateObject("All", "LoadingPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            loadingBack = (GImage)GetChildAt(0);
            loadingText = (GTextField)GetChildAt(1);
        }
    }
}