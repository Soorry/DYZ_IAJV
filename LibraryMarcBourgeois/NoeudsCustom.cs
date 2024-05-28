using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;
using UnityEngine;
using LibraryCommon;

namespace LibraryMarcBourgeois
{

    public class NoeudsAnticipateAndLookAtEnemy : INoeud
    {
        private float anticipationFactor = 0.005f; //

        public NoeudsAnticipateAndLookAtEnemy(float f)
        {
            anticipationFactor = f;
        }

        EtatNoeud INoeud.Execute(ref BehaviourTree bTree)
        {
            List<PlayerInformations> playerInfos = bTree.gameWorld.GetPlayerInfosList();
            List<PlayerInformations> previousPlayerInfos = bTree.previousGameWorld.GetPlayerInfosList();

            PlayerInformations closestEnemy = null;
            PlayerInformations previousClosestEnemy = null;
            float closestDistance = float.MaxValue;

            foreach (var playerInfo in playerInfos)
            {
                if (playerInfo.PlayerId != bTree.myPlayerInfos.PlayerId && playerInfo.IsActive)
                {
                    float distance = Vector3.Distance(bTree.myPlayerInfos.Transform.Position, playerInfo.Transform.Position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = playerInfo;
                    }
                }
            }

            if (closestEnemy != null)
            {
                foreach (var prevPlayerInfo in previousPlayerInfos)
                {
                    if (prevPlayerInfo.PlayerId == closestEnemy.PlayerId)
                    {
                        previousClosestEnemy = prevPlayerInfo;
                        break;
                    }
                }

                if (previousClosestEnemy != null)
                {
                    Vector3 previousPosition = previousClosestEnemy.Transform.Position;
                    Vector3 currentPosition = closestEnemy.Transform.Position;
                    Vector3 direction = (currentPosition - previousPosition).normalized;
                    Vector3 anticipatedPosition = currentPosition + direction * Vector3.Distance(currentPosition, bTree.myPlayerInfos.Transform.Position) * anticipationFactor;

                    bTree.actions.Add(new AIActionLookAtPosition(anticipatedPosition));
                    return EtatNoeud.Success;
                }
                else
                {
                    bTree.actions.Add(new AIActionLookAtPosition(closestEnemy.Transform.Position));
                    return EtatNoeud.Success;
                }
            }

            return EtatNoeud.Fail;
        }
    }

    public class NoeudsLookAtClosestEnemy : INoeud
    {
        EtatNoeud INoeud.Execute(ref BehaviourTree bTree)
        {
            List<PlayerInformations> playerInfos = bTree.gameWorld.GetPlayerInfosList();

            PlayerInformations closestEnemy = null;
            float closestDistance = float.MaxValue;

            foreach (var playerInfo in playerInfos)
            {
                if (playerInfo.PlayerId != bTree.myPlayerInfos.PlayerId && playerInfo.IsActive)
                {
                    float distance = Vector3.Distance(bTree.myPlayerInfos.Transform.Position, playerInfo.Transform.Position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = playerInfo;
                    }
                }
            }

            if (closestEnemy != null)
            {
                bTree.actions.Add(new AIActionLookAtPosition(closestEnemy.Transform.Position));
                return EtatNoeud.Success;
            }

            return EtatNoeud.Fail;
        }
    }

    public class NoeudsMoveToClosestEnemy : INoeud
    {
        EtatNoeud INoeud.Execute(ref BehaviourTree bTree)
        {
            List<PlayerInformations> playerInfos = bTree.gameWorld.GetPlayerInfosList();

            PlayerInformations closestEnemy = null;
            float closestDistance = float.MaxValue;

            foreach (var playerInfo in playerInfos)
            {
                if (playerInfo.PlayerId != bTree.myPlayerInfos.PlayerId && playerInfo.IsActive)
                {
                    float distance = Vector3.Distance(bTree.myPlayerInfos.Transform.Position, playerInfo.Transform.Position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = playerInfo;
                    }
                }
            }

            if (closestEnemy != null)
            {
                bTree.actions.Add(new AIActionMoveToDestination(closestEnemy.Transform.Position));
                return EtatNoeud.Success;
            }

            return EtatNoeud.Fail;
        }
    }

    public class NoeudsCheckDistance : INoeud
    {
        private float distanceThreshold;
        private bool checkInside; // false : out, true : in

        public NoeudsCheckDistance(float d, bool inside=true)
        {
            distanceThreshold = d;
            checkInside = inside;
        }

        EtatNoeud INoeud.Execute(ref BehaviourTree bTree)
        {
            PlayerInformations closestEnemy = GetClosestEnemy(bTree.myPlayerInfos, bTree.gameWorld.GetPlayerInfosList());

            if(checkInside)
            {
                if (closestEnemy != null)
                {
                    float distance = Vector3.Distance(bTree.myPlayerInfos.Transform.Position, closestEnemy.Transform.Position);
                    return distance <= distanceThreshold ? EtatNoeud.Fail : EtatNoeud.Success;
                }

                return EtatNoeud.Fail;
            }
            else
            {
                if (closestEnemy != null)
                {
                    float distance = Vector3.Distance(bTree.myPlayerInfos.Transform.Position, closestEnemy.Transform.Position);
                    return distance <= distanceThreshold ? EtatNoeud.Success : EtatNoeud.Fail;
                }

                return EtatNoeud.Success;
            }
        }

        private PlayerInformations GetClosestEnemy(PlayerInformations myPlayerInfos, List<PlayerInformations> playerInfosList)
        {
            PlayerInformations closestEnemy = null;
            float closestDistance = float.MaxValue;

            foreach (var playerInfo in playerInfosList)
            {
                if (playerInfo.PlayerId != myPlayerInfos.PlayerId && playerInfo.IsActive)
                {
                    float distance = Vector3.Distance(myPlayerInfos.Transform.Position, playerInfo.Transform.Position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = playerInfo;
                    }
                }
            }

            return closestEnemy;
        }
    }

    public class NoeudsMoveToBonus : INoeud
    {
        EtatNoeud INoeud.Execute(ref BehaviourTree bTree)
        {
            List<BonusInformations> bonusInfos = bTree.gameWorld.GetBonusInfosList();
            BonusInformations closestBonus = null;
            float closestDistance = float.MaxValue;

            foreach (var bonusInfo in bonusInfos)
            {
                float distance = Vector3.Distance(bTree.myPlayerInfos.Transform.Position, bonusInfo.Position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBonus = bonusInfo;
                }
            }

            if (closestBonus != null)
            {
                bTree.position = closestBonus.Position;
                NoeudsMoveTo move = new NoeudsMoveTo();
                return move.Execute(ref bTree);
            }

            return EtatNoeud.Fail;
        }
    }

    public class NoeudsDashIfProjectileClose : INoeud
    {
        private float _dangerDistance;
        private float _dashDistance;

        public NoeudsDashIfProjectileClose(float dangerDistance, float dashDistance)
        {
            _dangerDistance = dangerDistance;
            _dashDistance = dashDistance;
        }

        EtatNoeud INoeud.Execute(ref BehaviourTree bTree)
        {
            List<ProjectileInformations> projectileInfos = bTree.gameWorld.GetProjectileInfosList();
            List<BonusInformations> bonusInfos = bTree.gameWorld.GetBonusInfosList();
            Vector3 aiPosition = bTree.myPlayerInfos.Transform.Position;

            ProjectileInformations closestProjectile = null;
            float closestDistance = float.MaxValue;

            // Find the closest projectile from an enemy
            foreach (var projectileInfo in projectileInfos)
            {
                if (projectileInfo.PlayerId != bTree.myPlayerInfos.PlayerId)
                {
                    float distance = Vector3.Distance(aiPosition, projectileInfo.Transform.Position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestProjectile = projectileInfo;
                    }
                }
            }

            if (closestProjectile != null && closestDistance <= _dangerDistance)
            {
                // Calculate perpendicular dash direction
                Vector3 projectileDirection = (closestProjectile.Transform.Position - aiPosition).normalized;
                Vector3 dashDirection = Vector3.Cross(projectileDirection, Vector3.up).normalized;

                // Find the closest bonus in the dash direction
                BonusInformations closestBonus = null;
                float closestBonusDistance = float.MaxValue;

                foreach (var bonusInfo in bonusInfos)
                {
                    float distance = Vector3.Distance(aiPosition, bonusInfo.Position);
                    Vector3 bonusDirection = (bonusInfo.Position - aiPosition).normalized;

                    if (Vector3.Dot(bonusDirection, dashDirection) > 0.5f && distance < closestBonusDistance)
                    {
                        closestBonusDistance = distance;
                        closestBonus = bonusInfo;
                    }
                }

                if (closestBonus != null)
                {
                    dashDirection = (closestBonus.Position - aiPosition).normalized;
                }

                //Vector3 dashPosition = aiPosition + dashDirection * _dashDistance;
                bTree.actions.Add(new AIActionDash(dashDirection * _dashDistance));
                return EtatNoeud.Success;
            }

            return EtatNoeud.Fail;
        }
    }
}
