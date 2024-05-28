﻿using AI_BehaviorTree_AIGameUtility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LibraryCommon
{
    public class BehaviourTree
    {
        public GameWorldUtils gameWorld;
        public Selector start;
        public List<AIAction> actions;
        public Vector3 position;

        public BehaviourTree(Selector first, GameWorldUtils gw)
        {
            start = first;
            actions = new List<AIAction>();
            gameWorld = gw;
        }
    }
}