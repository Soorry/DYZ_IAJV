using AI_BehaviorTree_AIGameUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using LibraryCommon;
using UnityEngine.Assertions;

namespace LibraryZHENGDenis
{
    public class MoveBonusClosest : INoeud
    {
        public EtatNoeud Execute(ref BehaviourTree bTree)
        {
            List<BonusInformations> bonusInformations = bTree.gameWorld.GetBonusInfosList();
            if (bonusInformations == null || bonusInformations.Count == 0)
            {
                return EtatNoeud.Fail;
            }

            BonusInformations closestBonus = null;
            float closestDistance = float.MaxValue;

            foreach (var bonus in bonusInformations)
            {
                if (bTree.myplayerInformations.CurrentHealth > 7  && bonus.Type == EBonusType.Health)
                {
                  continue;
                }
                float distance = Vector3.Distance(bTree.myplayerInformations.Transform.Position, bonus.Position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBonus = bonus;
                }
            }

            if (closestBonus == null)
            {
                return EtatNoeud.Fail;
            }

            bTree.position = closestBonus.Position;
            /*if (bTree.myplayerInformations.IsDashAvailable)
            {
                NoeudsDash noeudsDash = new NoeudsDash();
                return noeudsDash.Execute(ref bTree);
            }*/
            NoeudsMoveTo noeudsMoveTo = new NoeudsMoveTo();
            return noeudsMoveTo.Execute(ref bTree);
        }
    }

    public class LookAtPlayerClosest : INoeud
    {
        public EtatNoeud Execute(ref BehaviourTree bTree)
        {
            List<PlayerInformations> playerInformations = bTree.gameWorld.GetPlayerInfosList();
            if (bTree.PrevPlayersPos.Count <= 0)
            {
                return EtatNoeud.Fail;
            }
            
            if (playerInformations == null || playerInformations.Count == 0)
            {

                return EtatNoeud.Fail;
            }

            PlayerInformations closestPlayer = null;
            float closestDistance = float.MaxValue;

            foreach (var player in playerInformations)
            {
                if (player.PlayerId == bTree.myplayerInformations.PlayerId)
                    continue;
                if (player.IsActive == false) continue;
                if (bTree.alliedNames.Contains(player.Name)) continue;
                float distance = Vector3.Distance(bTree.myplayerInformations.Transform.Position, player.Transform.Position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlayer = player;
                }
            }
            if (closestPlayer == null)
            {
                return EtatNoeud.Fail;
            }
            Vector3 ennemyPos = closestPlayer.Transform.Position;
            Vector3 playerPos = bTree.myplayerInformations.Transform.Position;
            
            Vector3 velocity = ennemyPos - bTree.PrevPlayersPos[closestPlayer.PlayerId];
            
            float alpha = -.01f;
            float t = Vector3.Distance(ennemyPos, playerPos) * alpha;
            var val = new Vector3(ennemyPos.x + velocity.x*t, ennemyPos.y + velocity.y* t, ennemyPos.z + velocity.z * t);
            bTree.position = val;
            NoeudsLookAt noeudsLookAt = new NoeudsLookAt();
            return noeudsLookAt.Execute(ref bTree);
        }
    }



        public class EchapProjectileMove : INoeud
        {
            public EtatNoeud Execute(ref BehaviourTree bTree)
            {
                List<ProjectileInformations> projectileInformations = bTree.gameWorld.GetProjectileInfosList();

                if (projectileInformations == null || projectileInformations.Count == 0)
                {
                    return EtatNoeud.Fail;
                }

                Vector3 escapePosition = CalculateEscapePosition(bTree.myplayerInformations.Transform.Position, projectileInformations, bTree.myplayerInformations);

                if (escapePosition == Vector3.zero)
                {
                    return EtatNoeud.Fail;
                }
                bTree.position = escapePosition;
                if (bTree.myplayerInformations.IsDashAvailable)
                {
                    NoeudsDash noeudsDash = new NoeudsDash();
                    return noeudsDash.Execute(ref bTree);
                }
                NoeudsMoveTo noeudsMoveTo = new NoeudsMoveTo();
                return noeudsMoveTo.Execute(ref bTree);
            }

            private Vector3 CalculateEscapePosition(Vector3 currentPosition, List<ProjectileInformations> projectileInformations, PlayerInformations player)
            {
                Vector3 escapeDirection = Vector3.zero;

                foreach (var projectile in projectileInformations)
                {
                    float distanceToProjectile = Vector3.Distance(currentPosition, projectile.Transform.Position);

                    if (distanceToProjectile <= 2)
                    {
                        if (projectile.PlayerId != player.PlayerId)
                        {
                            Vector3 directionToProjectile = currentPosition - projectile.Transform.Position;
                            // Calculate a perpendicular direction
                            Vector3 perpendicularDirection = Vector3.Cross(directionToProjectile, Vector3.up).normalized;
                            // Randomly choose left or right for the escape direction
                            escapeDirection = UnityEngine.Random.Range(0f,1f) > 0.5f ? perpendicularDirection : -perpendicularDirection;
                        }
                    }
                }

                if (escapeDirection == Vector3.zero)
                {
                    return Vector3.zero;
                }
                return currentPosition + escapeDirection * 2;
            }
        }
    

    public class MoveToPlayerClosest : INoeud
    {
        public EtatNoeud Execute(ref BehaviourTree bTree)
        {
            List<PlayerInformations> playerInformations = bTree.gameWorld.GetPlayerInfosList();

            if (playerInformations == null || playerInformations.Count == 0)
            {
                return EtatNoeud.Fail;
            }

            PlayerInformations closestPlayer = null;
            float closestDistance = float.MaxValue;

            foreach (var player in playerInformations)
            {
                if (player.PlayerId == bTree.myplayerInformations.PlayerId || !player.IsActive)
                    continue;

                float distance = Vector3.Distance(bTree.myplayerInformations.Transform.Position, player.Transform.Position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlayer = player;
                }
            }

            if (closestPlayer == null)
            {
                return EtatNoeud.Fail;
            }

            bTree.position = closestPlayer.Transform.Position;
            if (bTree.myplayerInformations.IsDashAvailable)
            {
                NoeudsDash noeudsDash = new NoeudsDash();
                return noeudsDash.Execute(ref bTree);
            }
            NoeudsMoveTo noeudsMoveTo = new NoeudsMoveTo();
            return noeudsMoveTo.Execute(ref bTree);
        }
    }
    public class NoeudFireWhenSeesPlayer : INoeud
    {
        public EtatNoeud Execute(ref BehaviourTree bTree)
        {
            List<PlayerInformations> playerInfos = bTree.gameWorld.GetPlayerInfosList();

            if (playerInfos == null || playerInfos.Count == 0)
            {
                return EtatNoeud.Fail;
            }

            PlayerInformations closestPlayer = null;
            float closestDistance = float.MaxValue;

            foreach (var player in playerInfos)
            {
                if (player.PlayerId == bTree.myplayerInformations.PlayerId)
                    continue;
                if (!player.IsActive)
                    continue;

                float distance = Vector3.Distance(bTree.myplayerInformations.Transform.Position, player.Transform.Position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlayer = player;
                }
            }

            if (closestPlayer == null)
            {
                return EtatNoeud.Fail;
            }

            Vector3 ennemyPos = closestPlayer.Transform.Position;
            Vector3 playerPos = bTree.myplayerInformations.Transform.Position;
            Vector3 forward = playerPos + bTree.myplayerInformations.Transform.Rotation * Vector3.forward;

            if (Vector3.Angle(playerPos - forward, playerPos - ennemyPos) < 3)
            {
                bTree.actions.Add(new AIActionFire());
                return EtatNoeud.Success;
            }
            return EtatNoeud.Fail;
        }
    }
    public class NoeudReloadIfNecessary : INoeud
    {
        public EtatNoeud Execute(ref BehaviourTree bTree)
        {
            if (bTree.myplayerInformations.SalvoRemainingAmount < 5)
            {
                bTree.actions.Add(new AIActionReload());
                return EtatNoeud.Success;
            }
            return EtatNoeud.Fail;
        }
    }
    public class NoeudAlwaysFail : INoeud
    {
        public EtatNoeud Execute(ref BehaviourTree bTree)
        {
            return EtatNoeud.Fail;
        }
    }


}


