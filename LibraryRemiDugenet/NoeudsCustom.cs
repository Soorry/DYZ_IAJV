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
            Debug.Log("Move");
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
            Debug.Log("prev count " + bTree.PrevPlayersPos.Count);
            if (bTree.PrevPlayersPos.Count == 0)
                return EtatNoeud.Fail;
            Vector3 ennemyPos = bTree.TargetPlayer.Transform.Position;
            Vector3 playerPos = bTree.myPlayer.Transform.Position;
            Vector3 velocity = ennemyPos - bTree.PrevPlayersPos[bTree.TargetPlayer.PlayerId];
            //float bulletV = 1;
            //t optimal
            /*
            float a = (velocity.x * velocity.x + velocity.y * velocity.y + velocity.z * velocity.z) - (bulletV * bulletV);
            float b = 2 * ((ennemyPos.x - playerPos.x) * velocity.x + (ennemyPos.y - playerPos.y) * velocity.y + (ennemyPos.z - playerPos.z) * velocity.z);
            float c = (ennemyPos.x - playerPos.x) * (ennemyPos.x - playerPos.x) + (ennemyPos.y - playerPos.y) * (ennemyPos.y - playerPos.y) + (ennemyPos.z - playerPos.z) * (ennemyPos.z - playerPos.z);
            //Debug.Log("a: " + a + " , b: " + b + ", c: " + c);
            if (b * b - 4 * a * c <= 0)
                return EtatNoeud.Fail;
            float t = (-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
            if(t<0)
                t = (-b - Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
            Debug.Log(" t : " + t);
            */
            Debug.Log("velocity : " + velocity);
            float alpha = .1f;
            float t = UnityEngine.Random.Range(-1f, 1f) * Vector3.Distance(ennemyPos, playerPos) * alpha;
            var val = new Vector3(ennemyPos.x + velocity.x*t, ennemyPos.y + velocity.y * t, ennemyPos.z + velocity.z * t);
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

    public class AvoidBullet : INoeud
    {
        public EtatNoeud Execute(ref BehaviourTree bTree)
        {
            Vector3 dir = Vector3.zero;
            foreach(var b in bTree.gameWorld.GetProjectileInfosList())
            {
                if (b.PlayerId == bTree.myPlayer.PlayerId)
                    continue;
                if(Vector3.Distance(b.Transform.Position, bTree.myPlayer.Transform.Position) < 10)
                {
                    dir += (bTree.myPlayer.Transform.Position - b.Transform.Position);
                }
            }
            if (dir == Vector3.zero)
                return EtatNoeud.Fail;
            dir = dir.normalized;
            bTree.position.x = dir.z;
            bTree.position.z = dir.x;
            bTree.position.y = 0;
            /*
            if (bTree.myPlayer.IsDashAvailable)
            {
                NoeudsDash dash = new NoeudsDash();
                return dash.Execute(ref bTree);
            }
            */
            NoeudsMoveTo move = new NoeudsMoveTo();
            move.Execute(ref bTree);
            return EtatNoeud.Fail;
        }
    }
    public class CheckAngle : INoeud
    {
        public EtatNoeud Execute(ref BehaviourTree bTree)
        {
            Debug.Log("ANGLEEEEEE");
            List<PlayerInformations> playerInfos = bTree.gameWorld.GetPlayerInfosList();
            Vector3 ennemyPos = playerInfos[0].Transform.Position;
            Vector3 playerPos = bTree.myPlayer.Transform.Position;
            Vector3 forward = playerPos + bTree.myPlayer.Transform.Rotation * new Vector3(0,0,1);
            if (Vector3.Angle(playerPos - forward, playerPos - ennemyPos) < 5)
                return EtatNoeud.Success;
            return EtatNoeud.Fail;
        }
    }

    public class SetTarget : INoeud
    {
        public EtatNoeud Execute(ref BehaviourTree bTree)
        {
            List<PlayerInformations> playerInfos = bTree.gameWorld.GetPlayerInfosList();
            bTree.TargetPlayer = null;
            foreach (var player in playerInfos)
            {
                if (player.PlayerId == bTree.myPlayer.PlayerId)
                    continue;
                if(!Physics.Raycast(bTree.myPlayer.Transform.Position, bTree.myPlayer.Transform.Position - player.Transform.Position,
                    Vector3.Distance(bTree.myPlayer.Transform.Position, player.Transform.Position) + 5, bTree.gameWorld.PlayerLayerMask))
                {
                    if (bTree.TargetPlayer == null)
                        bTree.TargetPlayer = player;
                    else if(bTree.TargetPlayer.CurrentHealth > player.CurrentHealth)
                        bTree.TargetPlayer = player;
                }
            }
            Debug.Log("Cur target : " + bTree.TargetPlayer == null? " NULL " : " GOOD ");
            if (bTree.TargetPlayer == null)
                return EtatNoeud.Fail;
            return EtatNoeud.Success;
        }
    }

    public class MoveToBonus : INoeud
    {
        public EtatNoeud Execute(ref BehaviourTree bTree)
        {
            Vector3 bonusPos = Vector3.positiveInfinity;
            foreach (var bonus in bTree.gameWorld.GetBonusInfosList())
            {
                if (bTree.myPlayer.CurrentHealth < bTree.myPlayer.MaxHealth / 2 && bonus.Type == EBonusType.Health)
                    bonusPos = bonus.Position;
                else if (Vector3.Distance(bTree.myPlayer.Transform.Position, bonus.Position) < Vector3.Distance(bTree.myPlayer.Transform.Position, bonusPos))
                    bonusPos = bonus.Position;
            }
            if(bonusPos != Vector3.positiveInfinity)
            {
                bTree.position = bonusPos;
                NoeudsMoveTo move = new NoeudsMoveTo();
                move.Execute(ref bTree);
            }
            return EtatNoeud.Fail;
        }
    }
}
