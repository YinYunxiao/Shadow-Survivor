/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace All
{
    public partial class GameOverPanel : GComponent
    {
        public GImage contentArea;
        public GLabel coinCountLabel;
        public GLabel killCountLabel;
        public GButton cofirmButton;
        public const string URL = "ui://6hwzb14voeai8s";

        public static GameOverPanel CreateInstance()
        {
            return (GameOverPanel)UIPackage.CreateObject("All", "GameOverPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            contentArea = (GImage)GetChildAt(1);
            coinCountLabel = (GLabel)GetChildAt(3);
            killCountLabel = (GLabel)GetChildAt(4);
            cofirmButton = (GButton)GetChildAt(5);
        }
    }
}