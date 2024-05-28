using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;
using UnityEngine;
using LibraryCommon;

namespace LibraryMarcBourgeois
{
    public class NoeudsLookAtClosestEnemy : INoeud
    {
        EtatNoeud INoeud.Execute(ref BehaviourTree bTree)
        {
            List<PlayerInformations> playerInfos = bTree.gameWorld.GetPlayerInfosList();
            PlayerInformations myPlayerInfos = GetPlayerInfos(bTree.AIId, playerInfos);

            PlayerInformations closestEnemy = null;
            float closestDistance = float.MaxValue;

            foreach (var playerInfo in playerInfos)
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

            if (closestEnemy != null)
            {
                bTree.actions.Add(new AIActionLookAtPosition(closestEnemy.Transform.Position));
                return EtatNoeud.Success;
            }

            return EtatNoeud.Fail;
        }

        private PlayerInformations GetPlayerInfos(int parPlayerId, List<PlayerInformations> parPlayerInfosList)
        {
            foreach (PlayerInformations playerInfo in parPlayerInfosList)
            {
                if (playerInfo.PlayerId == parPlayerId)
                    return playerInfo;
            }
            return null;
        }
    }

    public class NoeudsCheckDistance : INoeud
    {
        private float _distanceThreshold;

        public NoeudsCheckDistance(float distanceThreshold)
        {
            _distanceThreshold = distanceThreshold;
        }

        EtatNoeud INoeud.Execute(ref BehaviourTree bTree)
        {
            PlayerInformations myPlayerInfos = GetPlayerInfos(bTree.AIId, bTree.gameWorld.GetPlayerInfosList());
            PlayerInformations closestEnemy = GetClosestEnemy(myPlayerInfos, bTree.gameWorld.GetPlayerInfosList());

            if (closestEnemy != null)
            {
                float distance = Vector3.Distance(myPlayerInfos.Transform.Position, closestEnemy.Transform.Position);
                return distance <= _distanceThreshold ? EtatNoeud.Success : EtatNoeud.Fail;
            }

            return EtatNoeud.Fail;
        }

        private PlayerInformations GetPlayerInfos(int playerId, List<PlayerInformations> playerInfosList)
        {
            foreach (PlayerInformations playerInfo in playerInfosList)
            {
                if (playerInfo.PlayerId == playerId)
                    return playerInfo;
            }

            return null;
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

    public class NoeudsLookAtBonus : INoeud
    {
        EtatNoeud INoeud.Execute(ref BehaviourTree bTree)
        {
            List<PlayerInformations> playerInfos = bTree.gameWorld.GetPlayerInfosList();
            List<BonusInformations> bonusInfos = bTree.gameWorld.GetBonusInfosList();
            if (bonusInfos.Count > 0)
            {
                bTree.position = bonusInfos[0].Position;
                NoeudsLookAt look = new NoeudsLookAt();
                return look.Execute(ref bTree);
            }
            return EtatNoeud.Fail;
        }
    }

    public class NoeudsMoveToBonus : INoeud
    {
        EtatNoeud INoeud.Execute(ref BehaviourTree bTree)
        {
            List<PlayerInformations> playerInfos = bTree.gameWorld.GetPlayerInfosList();
            List<BonusInformations> bonusInfos = bTree.gameWorld.GetBonusInfosList();
            if(bonusInfos.Count > 0) { 
                bTree.position = bonusInfos[0].Position;
                NoeudsMoveTo move = new NoeudsMoveTo();
                return move.Execute(ref bTree);
            }
            return EtatNoeud.Fail;

            /*List<PlayerInformations> playerInfos = bTree.gameWorld.GetPlayerInfosList();
            PlayerInformations myPlayerInfos = GetPlayerInfos(bTree.AIId, playerInfos);

            List<BonusInformations> bonusInfos = bTree.gameWorld.GetBonusInfosList();
            BonusInformations closestBonus = null;
            float closestDistance = float.MaxValue;

            foreach (var bonusInfo in bonusInfos)
            {
                float distance = Vector3.Distance(myPlayerInfos.Transform.Position, bonusInfo.Position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBonus = bonusInfo;
                }
            }

            if (closestBonus != null)
            {
                bTree.actions.Add(new AIActionMoveToDestination(closestBonus.Position));
                return EtatNoeud.Success;
            }

            return EtatNoeud.Fail;*/
        }

        private PlayerInformations GetPlayerInfos(int parPlayerId, List<PlayerInformations> parPlayerInfosList)
        {
            foreach (PlayerInformations playerInfo in parPlayerInfosList)
            {
                if (playerInfo.PlayerId == parPlayerId)
                    return playerInfo;
            }
            return null;
        }
    }
}
