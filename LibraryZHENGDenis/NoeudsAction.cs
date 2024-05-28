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
    public class NoeudsFire : INoeud
    {
        public EtatNoeud Execute(object param, List<AIAction> aIActions)
        {
            
            aIActions.Add(new AIActionFire());
            return EtatNoeud.Success;
        }
    }
}
