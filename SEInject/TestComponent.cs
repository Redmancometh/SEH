using Sandbox.Common;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRageRender;
using VRageMath;

namespace SEInject
{
    class TestComponent : MySessionComponentBase
    {
        public override void Draw()
        {
            List<IMyPlayer> players = new List<IMyPlayer>();
            MyAPIGateway.Players.GetPlayers(players);

            Vector3 playerPos = MyAPIGateway.Session.Player.GetPosition();

            foreach (IMyPlayer player in players)
            {
                MyRenderProxy.DebugDrawLine3D(playerPos, player.GetPosition(), Color.Lime, Color.Lime, false);
            }
            HashSet<IMyEntity> entities = new HashSet<IMyEntity>();
            MyAPIGateway.Entities.GetEntities(entities);
        }
    }
}
