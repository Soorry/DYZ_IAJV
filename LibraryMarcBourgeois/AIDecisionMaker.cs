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

        // Elon Musk utilise LibraryCommonVS19
        public AIDecisionMaker()
        {
            /// Le behaviour tree ci-dessous correspond à ElonMuskBehaviourTreeDiagramNonOptimized.png
            /// Pour une version factorisée voir ElonMuskBehaviourTreeDiagramOptimized.png
            /// Les images sont situées à la source du dossier LibraryMarcBourgeois
            /// Note : Elon semble encore plus fort quand le temps est accéléré à x5 ou x10 sur des parties de plus longue durée

            Sequence startSequence = new Sequence();
            Selector startSelector = new Selector();

            Sequence rushBonus = new Sequence();
            rushBonus.Add(new NoeudsDashToClosestBonus());
            rushBonus.Add(new NoeudsMoveToClosestBonus());

            Sequence dodgeSequence = new Sequence();
            dodgeSequence.Add(new NoeudsDashIfProjectileClose(5.0f)); // Can fail

            Sequence targetAndBonusSequence = new Sequence();
            targetAndBonusSequence.Add(new NoeudsIsTarget()); // Condition
            targetAndBonusSequence.Add(new NoeudsIsBonus()); // Condition
            targetAndBonusSequence.Add(new NoeudsPredictAndLookAtEnemy(0.01f)); // Always success
            targetAndBonusSequence.Add(new NoeudsFireAtVisibleEnemy()); // Always success
            targetAndBonusSequence.Add(rushBonus);

            Sequence targetAndNoBonusSequence = new Sequence();
            targetAndNoBonusSequence.Add(new NoeudsIsTarget()); // Condition
            targetAndNoBonusSequence.Add(new NoeudsPredictAndLookAtEnemy(0.01f)); // Always success
            targetAndNoBonusSequence.Add(new NoeudsFireAtVisibleEnemy()); // Always success

            Selector ammoSelector = new Selector();
            Sequence seq1 = new Sequence();
            seq1.Add(new NoeudsAmmoHigherThan(0)); // Condition
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
            
            startSelector.Add(targetAndBonusSequence);
            startSelector.Add(targetAndNoBonusSequence);
            startSelector.Add(noTargetAndBonusSequence);
            startSelector.Add(nothingButBonusSpawnsSequence);
            startSelector.Add(defaultSequence);

            startSequence.Add(dodgeSequence); // Always success
            startSequence.Add(startSelector);

            behaviourTree = new BehaviourTree(startSequence, AIGameWorldUtils);
            behaviourTree.alliedNames = new List<string> { "Narutode" }; // Noms alliés
        }

        public List<AIAction> ComputeAIDecision()
        {
            List<PlayerInformations> playerInfos = AIGameWorldUtils.GetPlayerInfosList();
            PlayerInformations myPlayerInfos = GetPlayerInfos(AIId, playerInfos);

            behaviourTree.AIId = AIId;
            behaviourTree.myPlayerInfos = myPlayerInfos;

            behaviourTree.actions.Clear();
            behaviourTree.previousGameWorld = behaviourTree.gameWorld;
            behaviourTree.gameWorld = AIGameWorldUtils;

            SetAllBonusPositions(behaviourTree, behaviourTree.gameWorld); // Vérifie et ajoute les bonus dans les po
            behaviourTree.closestBonus = GetClosestBonus(myPlayerInfos.Transform.Position, behaviourTree.gameWorld);
            behaviourTree.closestTarget = GetClosestTarget(myPlayerInfos.Transform.Position, behaviourTree.gameWorld, behaviourTree.alliedNames);
            behaviourTree.start.Execute(ref behaviourTree);
            return behaviourTree.actions;
        }

        // Ajoute les bonus du gameWorld courant dans la liste des positions des bonus possibles
        // Répertorie la position de tout les bonus
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

        // Compute la position du bonus le plus proche si il en existe un
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

        // Compute la position du joueur le plus proche qui n'est pas un allié
        // Les alliés sont ignorés dans le cas où il y a seulement 2 joueurs dans la partie
        private PlayerInformations GetClosestTarget(Vector3 position, GameWorldUtils gameWorld, List<string> alliedNames)
        {
            List<PlayerInformations> playerInfos = gameWorld.GetPlayerInfosList();
            PlayerInformations closestEnemy = null;
            float closestDistance = float.MaxValue;

            foreach (PlayerInformations playerInfo in playerInfos)
            {
                if (playerInfo.PlayerId != AIId && playerInfo.IsActive)
                {
                    if (playerInfos.Count > 2) // Au moins 3 joueurs
                    {
                        if (!alliedNames.Contains(playerInfo.Name)) // Si ce n'est pas un allié
                        {
                            float distance = Vector3.Distance(position, playerInfo.Transform.Position);
                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                closestEnemy = playerInfo;
                            }
                        }
                    }
                    else // 1v1 : FFA (pas d'alliés)
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