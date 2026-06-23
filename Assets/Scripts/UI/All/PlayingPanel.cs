/** This is an automatically generated class by FairyGUI. Please do not modify it. **/

using FairyGUI;
using FairyGUI.Utils;

namespace All
{
    public partial class PlayingPanel : GComponent
    {
        public Controller playStart;
        public Controller hasBoss;
        public Controller hasPassiveSkill;
        public GButton pauseButton;
        public GLabel coinCountLabel;
        public GLabel killCountLabel;
        public GTextField levelText;
        public GProgressBar passiveSkillBar;
        public GProgressBar bossHPBar;
        public GButton playerInfoButton;
        public HealthSegment hpBar;
        public GProgressBar xpBar;
        public GGroup top;
        public Joystick joystick;
        public const string URL = "ui://6hwzb14vtza289";

        public static PlayingPanel CreateInstance()
        {
            return (PlayingPanel)UIPackage.CreateObject("All", "PlayingPanel");
        }

        public override void ConstructFromXML(XML xml)
        {
            base.ConstructFromXML(xml);

            playStart = GetControllerAt(0);
            hasBoss = GetControllerAt(1);
            hasPassiveSkill = GetControllerAt(2);
            pauseButton = (GButton)GetChildAt(0);
            coinCountLabel = (GLabel)GetChildAt(1);
            killCountLabel = (GLabel)GetChildAt(2);
            levelText = (GTextField)GetChildAt(3);
            passiveSkillBar = (GProgressBar)GetChildAt(4);
            bossHPBar = (GProgressBar)GetChildAt(5);
            playerInfoButton = (GButton)GetChildAt(6);
            hpBar = (HealthSegment)GetChildAt(7);
            xpBar = (GProgressBar)GetChildAt(8);
            top = (GGroup)GetChildAt(9);
            joystick = (Joystick)GetChildAt(10);
        }
    }
}