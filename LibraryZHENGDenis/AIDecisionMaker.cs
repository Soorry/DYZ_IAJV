using AI_BehaviorTree_AIGameUtility;
using LibraryCommon;
using LibraryZHENGDenis;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Windows.Services.Maps;


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
        public string GetName() { return "Narutode"; }

        public void SetAIGameWorldUtils(GameWorldUtils parGameWorldUtils) { AIGameWorldUtils = parGameWorldUtils; }

        public void OnMyAIDeath() { }
        //Fin du bloc de fonction nécessaire (Attention ComputeAIDecision en fait aussi partit)
        public BehaviourTree behaviourTree;

        public AIDecisionMaker()
        {
            Selector start = new Selector();

            //Custom noeuds
            LookAtPlayerClosest lookAtPlayerClosest = new LookAtPlayerClosest();
            NoeudFireWhenSeesPlayer noeudsFire = new NoeudFireWhenSeesPlayer();
            NoeudReloadIfNecessary noeudReload = new NoeudReloadIfNecessary();
            NoeudAlwaysFail alwaysFail = new NoeudAlwaysFail();

            MoveBonusClosest moveBonusClosest = new MoveBonusClosest();
            EchapProjectileMove echapProjectileMove = new EchapProjectileMove();
            MoveToPlayerClosest moveToPlayerClosest = new MoveToPlayerClosest();

            Sequence shootSequence = new Sequence();
            shootSequence.noeuds.Add(lookAtPlayerClosest);

            Selector ShootOrReload = new Selector();
            ShootOrReload.noeuds.Add(noeudsFire);
            ShootOrReload.noeuds.Add(noeudReload);

            shootSequence.noeuds.Add(ShootOrReload);
            shootSequence.noeuds.Add(alwaysFail);

            Sequence bonusSequence = new Sequence();
            bonusSequence.noeuds.Add(moveBonusClosest);

            Sequence escapeSequence = new Sequence();
            escapeSequence.noeuds.Add(echapProjectileMove);
           
            Sequence moveToPlayerSequence = new Sequence();
            moveToPlayerSequence.noeuds.Add(moveToPlayerClosest);

            start.noeuds.Add(shootSequence);
            start.noeuds.Add(bonusSequence);  
            start.noeuds.Add(escapeSequence);
            start.noeuds.Add(moveToPlayerSequence);
            List<string> Allies = new List<string> { "Elon Musk"};
            behaviourTree = new BehaviourTree(start, AIGameWorldUtils);
            behaviourTree.alliedNames = Allies;
        }


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
            behaviourTree.myplayerInformations = myPlayerInfos;
            behaviourTree.start.Execute(ref behaviourTree);
            behaviourTree.PrevPlayersPos.Clear();
            behaviourTree.PrevPlayersPos.AddRange(playerPos);
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
