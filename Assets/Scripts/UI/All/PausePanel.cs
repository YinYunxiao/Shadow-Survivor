/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace All
{
    public partial class PausePanel : GComponent
    {
        public Controller isPlaying;
        public GImage contentArea;
        public GButton cancelButton;
        public GButton cofirmButton;
        public GSlider volueSlider;
        public GButton finishGameButton;
        public const string URL = "ui://6hwzb14vtza28e";

        public static PausePanel CreateInstance()
        {
            return (PausePanel)UIPackage.CreateObject("All", "PausePanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            isPlaying = GetControllerAt(0);
            contentArea = (GImage)GetChildAt(1);
            cancelButton = (GButton)GetChildAt(2);
            cofirmButton = (GButton)GetChildAt(3);
            volueSlider = (GSlider)GetChildAt(4);
            finishGameButton = (GButton)GetChildAt(8);
        }
    }
}