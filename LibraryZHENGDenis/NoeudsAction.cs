using AI_BehaviorTree_AIGameUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using LibraryCommon;

namespace LibraryZHENGDenis
{
    class NoeudsFire : INoeud
    {
        public List<AIAction> actions { get => actions; set => actions = value; }

        public etatNoeud Execute(object param)
        {
            actions.Add(new AIActionFire());
            return etatNoeud.Sucess;
        }
    }
}
