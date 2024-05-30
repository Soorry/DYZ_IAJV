using AI_BehaviorTree_AIGameUtility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LibraryCommon
{
    public class BehaviourTree
    {
        public GameWorldUtils previousGameWorld;
        public GameWorldUtils gameWorld;
        public int AIId;

        public INoeud start;
        public List<AIAction> actions;
        public Vector3 position;
        public PlayerInformations myPlayerInfos;
        public List<string> alliedNames;
        public PlayerInformations closestTarget; // Enemy computed at each tick
        public BonusInformations closestBonus; // Bonus computed at each tick
        public List<Vector3> bonusPositions = new List<Vector3>(); // Known bonus positions

        public PlayerInformations myplayerInformations; // deprecated (use myPlayerInfos)
        public List<Vector3> PrevPlayersPos = new List<Vector3>(); // deprecated (use previousGameWorld.GetPlayerInfosList())

        public BehaviourTree(INoeud first, GameWorldUtils gw)
        {
            start = first;
            actions = new List<AIAction>();
            gameWorld = gw;
            previousGameWorld = gw;
        }

        public void AddAction()
        {
            // todo
        }
    }
}
