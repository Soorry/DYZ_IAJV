using AI_BehaviorTree_AIGameUtility;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using LibraryCommon;
using LibraryMarcBourgeois;

namespace AI_BehaviorTree_AIImplementation
{
    public class AIDecisionMaker
    {
        /// <summary>
        /// Ne pas supprimer des fonctions, ou changer leur signature sinon la DLL ne fonctionnera plus
        /// Vous pouvez unitquement modifier l'intérieur des fonctions si nécessaire (par exemple le nom)
        /// ComputeAIDecision en fait partit
        /// </summary>
        private int AIId = -1;
        public GameWorldUtils AIGameWorldUtils = new GameWorldUtils();

        // Ne pas utiliser cette fonction, elle n'est utile que pour le jeu qui vous Set votre Id, si vous voulez votre Id utilisez AIId
        public void SetAIId(int parAIId) { AIId = parAIId; }

        // Vous pouvez modifier le contenu de cette fonction pour modifier votre nom en jeu
        public string GetName() { return "Elon Musk"; }

        public void SetAIGameWorldUtils(GameWorldUtils parGameWorldUtils) { AIGameWorldUtils = parGameWorldUtils; }

        public void OnMyAIDeath() { }

        //Fin du bloc de fonction nécessaire (Attention ComputeAIDecision en fait aussi partie)

        public BehaviourTree behaviourTree;

        public AIDecisionMaker()
        {
            Sequence startSequence = new Sequence();
            Selector startSelector = new Selector();

            Sequence rushBonus = new Sequence();
            rushBonus.Add(new NoeudsDashToClosestBonus());
            rushBonus.Add(new NoeudsMoveToBonus());

            Sequence dodgeSequence = new Sequence();
            dodgeSequence.Add(new NoeudsDashIfProjectileClose(5.0f)); // Can fail

            Sequence targetAndBonusSequence = new Sequence();
            targetAndBonusSequence.Add(new NoeudsIsTarget()); // Condition
            targetAndBonusSequence.Add(new NoeudsIsBonus()); // Condition
            targetAndBonusSequence.Add(new NoeudsPredictAndLookAtEnemy()); // Always success
            targetAndBonusSequence.Add(new NoeudsFireAtVisibleEnemy()); // Always success
            targetAndBonusSequence.Add(rushBonus);

            Sequence targetAndNoBonusSequence = new Sequence();
            targetAndNoBonusSequence.Add(new NoeudsIsTarget()); // Condition
            targetAndNoBonusSequence.Add(new NoeudsPredictAndLookAtEnemy()); // Always success
            targetAndNoBonusSequence.Add(new NoeudsFireAtVisibleEnemy()); // Always success

            Selector ammoSelector = new Selector();
            Sequence seq1 = new Sequence();
            seq1.Add(new NoeudsAmmoHigherThan(4)); // Condition
            seq1.Add(new NoeudsMoveToClosestEnemy());
            ammoSelector.Add(seq1);
            ammoSelector.Add(new NoeudsMoveToClosestBonusLastKnownPosition()); // Can fail
            ammoSelector.Add(new NoeudsStop()); // Always success

            targetAndNoBonusSequence.Add(ammoSelector);

            Sequence noTargetAndBonusSequence = new Sequence();
            noTargetAndBonusSequence.Add(new NoeudsIsBonus()); // Condition
            noTargetAndBonusSequence.Add(new NoeudsReload()); // Always success
            noTargetAndBonusSequence.Add(rushBonus);

            Sequence nothingButBonusSpawnsSequence = new Sequence();
            nothingButBonusSpawnsSequence.Add(new NoeudsReload()); // Always success
            nothingButBonusSpawnsSequence.Add(new NoeudsMoveToClosestBonusLastKnownPosition()); // Can fail

            Sequence defaultSequence = new Sequence();
            defaultSequence.Add(new NoeudsReload());
            defaultSequence.Add(new NoeudsStop());

            //startSelector.Add(dodgeSequence);
            startSelector.Add(targetAndBonusSequence);
            startSelector.Add(targetAndNoBonusSequence);
            startSelector.Add(noTargetAndBonusSequence);
            startSelector.Add(nothingButBonusSpawnsSequence);
            startSelector.Add(defaultSequence);

            startSequence.Add(dodgeSequence); // Always success
            startSequence.Add(startSelector);


            /*Sequence dodgeSequence = new Sequence();
            //dodgeSequence.noeuds.Add(new NoeudsReloadIfEnemyBehind()); // Always success
            dodgeSequence.noeuds.Add(new NoeudsDashIfProjectileClose(5.0f));

            // is enemy, is bonus
            Sequence moveToBonusSequence = new Sequence();
            moveToBonusSequence.noeuds.Add(new NoeudsPredictAndLookAtEnemy(0.017f));
            moveToBonusSequence.noeuds.Add(new NoeudsFireAtVisibleEnemy()); // Always true
            moveToBonusSequence.noeuds.Add(new NoeudsDashToClosestBonus());
            moveToBonusSequence.noeuds.Add(new NoeudsMoveToBonus());
            
            // is enemy, no bonus
            Sequence moveToEnemySequence = new Sequence();
            moveToEnemySequence.noeuds.Add(new NoeudsPredictAndLookAtEnemy(0.017f));
            moveToEnemySequence.noeuds.Add(new NoeudsFireAtVisibleEnemy()); // Always true

            Selector moveToBonusOrTarget = new Selector();
            Sequence sq = new Sequence();
            sq.noeuds.Add(new NoeudsAmmoHigherThan(4));
            sq.noeuds.Add(new NoeudsMoveToClosestEnemy());
            moveToBonusOrTarget.noeuds.Add(new NoeudsMoveToClosestBonusLastKnowPosition());

            moveToEnemySequence.noeuds.Add(moveToBonusOrTarget);
            //moveToEnemySequence.noeuds.Add(new NoeudsAmmoHigherThan(5));
            //moveToEnemySequence.noeuds.Add(new NoeudsMoveToClosestEnemy());
            //moveToEnemySequence.noeuds.Add(new NoeudsMoveToClosestBonusLastKnowPosition());

            moveToBonusOrTarget.noeuds.Add(sq);
            moveToBonusOrTarget.noeuds.Add(new NoeudsMoveToClosestBonusLastKnowPosition());


            // no enemy
            Sequence noEnemySequence = new Sequence();
            noEnemySequence.noeuds.Add(new NoeudsReload());
            noEnemySequence.noeuds.Add(new NoeudsMoveToBonus());
            noEnemySequence.noeuds.Add(new NoeudsDashToClosestBonus());

            // no bonus, no enemy
            Sequence defaultSequence = new Sequence();
            defaultSequence.noeuds.Add(new NoeudsStop());
            
            start.noeuds.Add(dodgeSequence);
            start.noeuds.Add(moveToBonusSequence);
            start.noeuds.Add(moveToEnemySequence);
            start.noeuds.Add(noEnemySequence);
            start.noeuds.Add(defaultSequence);*/

            behaviourTree = new BehaviourTree(startSequence, AIGameWorldUtils);
        }

        public List<AIAction> ComputeAIDecision()
        {
            List<PlayerInformations> playerInfos = AIGameWorldUtils.GetPlayerInfosList();
            PlayerInformations myPlayerInfos = GetPlayerInfos(AIId, playerInfos);

            behaviourTree.AIId = AIId;
            behaviourTree.myPlayerInfos = myPlayerInfos;
            behaviourTree.alliedNames = new List<string> { "Narutode" };

            behaviourTree.actions.Clear();
            behaviourTree.previousGameWorld = behaviourTree.gameWorld;
            behaviourTree.gameWorld = AIGameWorldUtils;

            SetAllBonusPositions(behaviourTree, behaviourTree.gameWorld); // In theory should be executed only once
            behaviourTree.closestBonus = GetClosestBonus(myPlayerInfos.Transform.Position, behaviourTree.gameWorld);
            behaviourTree.closestTarget = GetClosestTarget(myPlayerInfos.Transform.Position, behaviourTree.gameWorld, behaviourTree.alliedNames);
            behaviourTree.start.Execute(ref behaviourTree);
            return behaviourTree.actions;
        }

        private void SetAllBonusPositions(BehaviourTree bTree, GameWorldUtils gameWorld)
        {
            List<BonusInformations> bonusInfos = gameWorld.GetBonusInfosList();

            foreach (BonusInformations bonusInfo in bonusInfos)
            {
                if(!bTree.bonusPositions.Contains(bonusInfo.Position)) { 
                    bTree.bonusPositions.Add(bonusInfo.Position);
                }
            }
        }

        private BonusInformations GetClosestBonus(Vector3 position, GameWorldUtils gameWorld)
        {
            List<BonusInformations> bonusInfos = gameWorld.GetBonusInfosList();
            BonusInformations closestBonus = null;
            float closestDistance = float.MaxValue;

            foreach (BonusInformations bonusInfo in bonusInfos)
            {
                float distance = Vector3.Distance(position, bonusInfo.Position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBonus = bonusInfo;
                }
            }

            return closestBonus;
        }

        private PlayerInformations GetClosestTarget(Vector3 position, GameWorldUtils gameWorld, List<string> alliedNames)
        {
            List<PlayerInformations> playerInfos = gameWorld.GetPlayerInfosList();
            PlayerInformations closestEnemy = null;
            float closestDistance = float.MaxValue;

            foreach (PlayerInformations playerInfo in playerInfos)
            {
                if (playerInfo.PlayerId != AIId && playerInfo.IsActive)
                {
                    // Ally mode
                    if (playerInfos.Count > 2)
                    {
                        if (!alliedNames.Contains(playerInfo.Name))
                        {
                            float distance = Vector3.Distance(position, playerInfo.Transform.Position);
                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                closestEnemy = playerInfo;
                            }
                        }
                    }
                    else // FFA
                    {
                        float distance = Vector3.Distance(position, playerInfo.Transform.Position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestEnemy = playerInfo;
                        }
                    }
                }
            }
            return closestEnemy;
        }

        public PlayerInformations GetPlayerInfos(int parPlayerId, List<PlayerInformations> parPlayerInfosList)
        {
            foreach (PlayerInformations playerInfo in parPlayerInfosList)
            {
                if (playerInfo.PlayerId == parPlayerId)
                    return playerInfo;
            }

            Assert.IsTrue(false, "GetPlayerInfos : PlayerId not Found");
            return null;
        }
    }
}