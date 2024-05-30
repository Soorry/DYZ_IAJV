using AI_BehaviorTree_AIGameUtility;
using LibraryCommon;
using LibraryRemiDugenet;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


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
        public BehaviourTree behaviourTree;

        public AIDecisionMaker()
        {
            Selector start = new Selector();
            Sequence seq1 = new Sequence();
            Sequence seq2 = new Sequence();
            Sequence seq3 = new Sequence();

            AvoidBullet avoid = new AvoidBullet();
            seq1. noeuds.Add(avoid);
            start.noeuds.Add(seq1);

            MoveToBonus bonus = new MoveToBonus();
            seq2.noeuds.Add(bonus);
            start.noeuds.Add(seq2);

            //MoveToPlayer move = new MoveToPlayer();
            LookAtPlayer lookAt = new LookAtPlayer();
            CheckAngle angle = new CheckAngle();
            SetTarget target = new SetTarget();
            Fire fire = new Fire();
            seq3.noeuds.Add(target);
            seq3.noeuds.Add(lookAt);
            seq3.noeuds.Add(angle);
            seq3.noeuds.Add(fire);
            start.noeuds.Add(seq3);

            behaviourTree = new BehaviourTree(start, AIGameWorldUtils);
        }

        // Ne pas utiliser cette fonction, elle n'est utile que pour le jeu qui vous Set votre Id, si vous voulez votre Id utilisez AIId
        public void SetAIId(int parAIId) { AIId = parAIId; }

        // Vous pouvez modifier le contenu de cette fonction pour modifier votre nom en jeu
        public string GetName() { return "bot_bozo 3"; }

        public void OnMyAIDeath() { }

        public void SetAIGameWorldUtils(GameWorldUtils parGameWorldUtils) { AIGameWorldUtils = parGameWorldUtils; }

        //Fin du bloc de fonction nécessaire (Attention ComputeAIDecision en fait aussi partit)

        public List<AIAction> ComputeAIDecision()
        {
            List<PlayerInformations> playerInfos = AIGameWorldUtils.GetPlayerInfosList();
            PlayerInformations myPlayerInfos = GetPlayerInfos(AIId, playerInfos);

            List<Vector3> playerPos = new List<Vector3>();
            foreach (PlayerInformations playerInfo in playerInfos)
            {
                playerPos.Add(playerInfo.Transform.Position);
            }

            behaviourTree.actions.Clear();
            behaviourTree.gameWorld = AIGameWorldUtils;
            behaviourTree.myPlayer = myPlayerInfos;
            behaviourTree.start.Execute(ref behaviourTree);
            behaviourTree.PrevPlayersPos.Clear();
            behaviourTree.PrevPlayersPos.AddRange(playerPos);
            Debug.Log("count actions : " + behaviourTree.actions.Count);
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
