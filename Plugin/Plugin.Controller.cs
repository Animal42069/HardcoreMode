using System.Collections.Generic;

namespace HardcoreMode
{
    public partial class HardcoreMode
    {
        public static LifeStatsController playerController;
        static readonly List<LifeStatsController> controllersQueue = new List<LifeStatsController>();
        static readonly List<LifeStatsController> agentControllers = new List<LifeStatsController>();
        static readonly List<LifeStatsController> agentControllersDump = new List<LifeStatsController>();

        public void Update()
        {
            Status.Update();
            Sleep.Update();
        }

        public void LateUpdate()
        {
            Dead.LateUpdate();
        }

        public void OnGUI()
        {
            UpdateControllers();

            Sleep.OnGUI();
            Dead.OnGUI();
        }
    }
}
