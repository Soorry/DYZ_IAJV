using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI_BehaviorTree_AIGameUtility;
using LibraryCommon;
using UnityEngine;

namespace LibraryRemiDugenet
{
    public class MoveToPlayer : INoeud
    {
        public EtatNoeud Execute(ref BehaviourTree bTree)
        {
            List<PlayerInformations> playerInfos = bTree.gameWorld.GetPlayerInfosList();
            bTree.position = playerInfos[0].Transform.Position;
            NoeudsMoveTo move = new NoeudsMoveTo();
            return move.Execute(ref bTree);
        }
    }

    public class LookAtPlayer : INoeud
    {
        public EtatNoeud Execute(ref BehaviourTree bTree)
        {
            List<PlayerInformations> playerInfos = bTree.gameWorld.GetPlayerInfosList();
            var rot = playerInfos[0].Transform.Rotation;
            var pos = playerInfos[0].Transform.Position;
            var val = pos + rot * new Vector3(-5, 0, 0);
            bTree.position = val;
            NoeudsLookAt look = new NoeudsLookAt();
            return look.Execute(ref bTree);
        }
    }

    public class Fire : INoeud
    {
        public EtatNoeud Execute(ref BehaviourTree bTree)
        {
            NoeudsFire look = new NoeudsFire();
            return look.Execute(ref bTree);
        }
    }
}
