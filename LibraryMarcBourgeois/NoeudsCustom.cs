using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;
using UnityEngine;
using LibraryCommon;

namespace LibraryMarcBourgeois
{
    public class NoeudsPredictAndLookAtEnemy : INoeud
    {
        private float anticipationFactor; //

        public NoeudsPredictAndLookAtEnemy(float f = 0.01f)
        {
            anticipationFactor = f;
        }

        EtatNoeud INoeud.Execute(ref BehaviourTree bTree)
        {
            List<PlayerInformations> playerInfos = bTree.gameWorld.GetPlayerInfosList();
            List<PlayerInformations> previousPlayerInfos = bTree.previousGameWorld.GetPlayerInfosList();
            
            PlayerInformations closestEnemy = bTree.closestTarget;
            PlayerInformations previousClosestEnemy = null;

            if (closestEnemy != null)
            {
                // Get previous position of current closest enemy
                foreach (PlayerInformations prevPlayerInfo in previousPlayerInfos)
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

            // Always success
            return EtatNoeud.Success;
        }
    }

    public class NoeudsIsTarget : INoeud
    {
        EtatNoeud INoeud.Execute(ref BehaviourTree bTree)
        {
            if (bTree.closestTarget != null)
            {
                return EtatNoeud.Success;
            }
            return EtatNoeud.Fail;
        }
    }

    public class NoeudsIsBonus : INoeud
    {
        EtatNoeud INoeud.Execute(ref BehaviourTree bTree)
        {
            if (bTree.closestBonus != null)
            {
                return EtatNoeud.Success;
            }
            return EtatNoeud.Fail;
        }
    }

    public class NoeudsMoveToClosestEnemy : INoeud
    {
        EtatNoeud INoeud.Execute(ref BehaviourTree bTree)
        {
            if (bTree.closestTarget != null)
            {
                bTree.actions.Add(new AIActionMoveToDestination(bTree.closestTarget.Transform.Position));
                return EtatNoeud.Success;
            }

            return EtatNoeud.Fail;
        }
    }

    public class NoeudsMoveToClosestBonus : INoeud
    {
        EtatNoeud INoeud.Execute(ref BehaviourTree bTree)
        {
            if (bTree.closestBonus != null)
            {
                bTree.actions.Add(new AIActionMoveToDestination(bTree.closestBonus.Position));
                return EtatNoeud.Success;
            }

            return EtatNoeud.Fail;
        }
    }

    public class NoeudsDashToClosestBonus : INoeud
    {
        EtatNoeud INoeud.Execute(ref BehaviourTree bTree)
        {
            if(bTree.closestBonus != null)
            {
                Vector3 direction = (bTree.closestBonus.Position - bTree.myPlayerInfos.Transform.Position).normalized;
                bTree.actions.Add(new AIActionDash(direction));
                return EtatNoeud.Success;
            }

            return EtatNoeud.Fail;
        }
    }

    // Utilise la position de tout les bonus connus qu'ils aient spawn ou non
    public class NoeudsMoveToClosestBonusLastKnownPosition : INoeud
    {
        public EtatNoeud Execute(ref BehaviourTree bTree)
        {
            if (bTree.bonusPositions.Count == 0)
            {
                return EtatNoeud.Fail;
            }

            Vector3 closestBonusPosition = Vector3.zero;
            float closestDistance = float.MaxValue;

            foreach(Vector3 bonusPosition in bTree.bonusPositions)
            {
                float distance = Vector3.Distance(bTree.myPlayerInfos.Transform.Position, bonusPosition);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBonusPosition = bonusPosition;
                }
            }

            bTree.actions.Add(new AIActionMoveToDestination(closestBonusPosition));
            return EtatNoeud.Success;
        }
    }

    // Spider man dodge (big node)
    public class NoeudsDashIfProjectileClose : INoeud
    {
        private float dangerDistance;

        public NoeudsDashIfProjectileClose(float dangerDistance)
        {
            this.dangerDistance = dangerDistance;
        }

        EtatNoeud INoeud.Execute(ref BehaviourTree bTree)
        {
            List<ProjectileInformations> projectileInfos = bTree.gameWorld.GetProjectileInfosList();
            List<BonusInformations> bonusInfos = bTree.gameWorld.GetBonusInfosList();
            Vector3 myPosition = bTree.myPlayerInfos.Transform.Position;

            ProjectileInformations closestProjectile = null;
            float closestDistance = float.MaxValue;

            // Find the closest projectile from an enemy
            foreach (var projectileInfo in projectileInfos)
            {
                if (projectileInfo.PlayerId != bTree.myPlayerInfos.PlayerId)
                {
                    float distance = Vector3.Distance(myPosition, projectileInfo.Transform.Position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestProjectile = projectileInfo;
                    }
                }
            }

            if (closestProjectile != null && closestDistance <= dangerDistance)
            {
                Vector3 projectileDirection = (closestProjectile.Transform.Position - myPosition).normalized;
                Vector3 dashDirection = Vector3.Cross(projectileDirection, Vector3.up).normalized; // Perpendiculaire

                float wallMaxDistance = 5;
                RaycastHit hit;
                if (Physics.Raycast(bTree.myPlayerInfos.Transform.Position, dashDirection.normalized, out hit, wallMaxDistance))
                {
                    // Wall
                    if (hit.collider.gameObject.layer != bTree.gameWorld.PlayerLayerMask &&
                        hit.collider.gameObject.layer != bTree.gameWorld.ProjectileLayerMask &&
                        hit.collider.gameObject.layer != bTree.gameWorld.BonusLayerMask)
                    {
                        dashDirection = -dashDirection;
                    }
                }

                BonusInformations closestBonus = null;
                float closestBonusDistance = float.MaxValue;

                foreach (BonusInformations bonusInfo in bonusInfos)
                {
                    float bonusDistance = Vector3.Distance(myPosition, bonusInfo.Position);
                    Vector3 bonusDirection = (bonusInfo.Position - myPosition).normalized;

                    // Dirige vers le bonus si l'angle par rapport à la perpendiculaire n'est pas trop obtu
                    if (Vector3.Dot(bonusDirection, dashDirection) > 0.5f && bonusDistance < closestBonusDistance)
                    {
                        closestBonusDistance = bonusDistance;
                        closestBonus = bonusInfo;
                    }
                }

                if (closestBonus != null)
                {
                    dashDirection = (closestBonus.Position - myPosition).normalized;
                }

                bTree.actions.Add(new AIActionDash(dashDirection));
                return EtatNoeud.Success;
            }

            // Always success
            return EtatNoeud.Success;
        }
    }

    public class NoeudsFireAtVisibleEnemy : INoeud
    {
        public EtatNoeud Execute(ref BehaviourTree bTree)
        {
            if (bTree.closestTarget != null)
            {
                if(IsEnemyInFieldOfView(bTree.myPlayerInfos, bTree.closestTarget, 30))
                {
                    bTree.actions.Add(new AIActionFire()); // Fire at target
                    return EtatNoeud.Success;
                }

                // Detection de mur (pas 100% fonctionnel)

                /*Vector3 directionToEnemy = closestEnemy.Transform.Position - bTree.myPlayerInfos.Transform.Position;
                RaycastHit hit;
                if (Physics.Raycast(bTree.myPlayerInfos.Transform.Position, directionToEnemy, out hit, 500f))
                {
                    float hitDistance = Vector3.Distance(bTree.myPlayerInfos.Transform.Position, hit.point);
                    // Wall behind target
                    if (hit.collider.gameObject.layer == bTree.gameWorld.PlayerLayerMask || hitDistance+1 > closestDistance)
                    {
                        bTree.actions.Add(new AIActionFire()); // Fire at target
                    }
                }*/
            }

            // Always Success
            return EtatNoeud.Success;
        }

        private bool IsEnemyInFieldOfView(PlayerInformations myPlayerInfos, PlayerInformations enemy, float fieldOfView)
        {
            Vector3 directionToEnemy = (enemy.Transform.Position - myPlayerInfos.Transform.Position).normalized;
            float angle = Vector3.Angle(myPlayerInfos.Transform.Rotation * Vector3.forward, directionToEnemy);

            return angle <= fieldOfView / 2f;
        }
    }

    public class NoeudsAmmoLessThan : INoeud
    {
        private float count;

        public NoeudsAmmoLessThan(float ammoCount)
        {
            this.count = ammoCount;
        }

        public EtatNoeud Execute(ref BehaviourTree bTree)
        {
            if(bTree.myPlayerInfos.SalvoRemainingAmount < count)
            {
                return EtatNoeud.Success;
            }
            return EtatNoeud.Fail;
        }
    }

    public class NoeudsAmmoHigherThan : INoeud
    {
        private float count;

        public NoeudsAmmoHigherThan(float ammoCount)
        {
            this.count = ammoCount;
        }

        public EtatNoeud Execute(ref BehaviourTree bTree)
        {
            if (bTree.myPlayerInfos.SalvoRemainingAmount > count)
            {
                return EtatNoeud.Success;
            }
            return EtatNoeud.Fail;
        }
    }

    // Not used
    public class NoeudsReloadIfEnemyBehind : INoeud
    {
        public EtatNoeud Execute(ref BehaviourTree bTree)
        {
            if (bTree.closestTarget != null && IsEnemyBehind(bTree.myPlayerInfos, bTree.closestTarget, 180f))
            {
                bTree.actions.Add(new AIActionReload());
                return EtatNoeud.Success;
            }

            // Always true
            return EtatNoeud.Success;
        }

        private bool IsEnemyBehind(PlayerInformations myPlayerInfos, PlayerInformations enemy, float fieldOfView)
        {
            Vector3 directionToEnemy = (enemy.Transform.Position - myPlayerInfos.Transform.Position).normalized;
            Vector3 oppositeDirection = -(myPlayerInfos.Transform.Rotation * Vector3.forward); // Direction opposée
            float angle = Vector3.Angle(oppositeDirection, directionToEnemy);

            return angle <= fieldOfView / 2f;
        }
    }
}
