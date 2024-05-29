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
            Selector start = new Selector();

            Sequence dodgeSequence = new Sequence();
            dodgeSequence.noeuds.Add(new NoeudsDashIfProjectileClose(1.0f, 1000.0f));

            Sequence moveToBonusSequence = new Sequence();
            //moveToBonusSequence.noeuds.Add(new NoeudsLookAtClosestEnemy());
            moveToBonusSequence.noeuds.Add(new NoeudsAnticipateAndLookAtEnemy(0.005f));
            moveToBonusSequence.noeuds.Add(new NoeudsFire());
            moveToBonusSequence.noeuds.Add(new NoeudsMoveToBonus());

            Sequence moveToEnemySequence = new Sequence();
            //moveToEnemySequence.noeuds.Add(new NoeudsLookAtClosestEnemy());
            moveToEnemySequence.noeuds.Add(new NoeudsAnticipateAndLookAtEnemy(0.005f));
            moveToBonusSequence.noeuds.Add(new NoeudsFire());
            //moveToEnemySequence.noeuds.Add(new NoeudsCheckDistance(10.0f, false));
            moveToEnemySequence.noeuds.Add(new NoeudsMoveToClosestEnemy());

            Sequence defaultSequence = new Sequence();
            defaultSequence.noeuds.Add(new NoeudsStop());
            
            start.noeuds.Add(dodgeSequence);
            start.noeuds.Add(moveToBonusSequence);
            start.noeuds.Add(moveToEnemySequence);
            start.noeuds.Add(defaultSequence);

            behaviourTree = new BehaviourTree(start, AIGameWorldUtils);
        }

        public List<AIAction> ComputeAIDecision()
        {
            List<PlayerInformations> playerInfos = AIGameWorldUtils.GetPlayerInfosList();
            behaviourTree.AIId = AIId;
            behaviourTree.myPlayerInfos = GetPlayerInfos(AIId, playerInfos);

            behaviourTree.actions.Clear();
            behaviourTree.previousGameWorld = behaviourTree.gameWorld;
            behaviourTree.gameWorld = AIGameWorldUtils;
            behaviourTree.start.Execute(ref behaviourTree);
            return behaviourTree.actions;
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