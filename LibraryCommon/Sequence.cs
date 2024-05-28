using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;
using UnityEngine;

namespace LibraryCommon
{
    public class Sequence : INoeud
    {
        public List<INoeud> noeuds = new List<INoeud>();
        EtatNoeud etat = EtatNoeud.NotExecuted;

        public Sequence()
        {
            noeuds = new List<INoeud>();
        }


        public EtatNoeud Execute(ref GameWorldUtils gameWorld, List<AIAction> aIActions, object info)
        {
            foreach (var n in noeuds)
            {
                etat = n.Execute(ref gameWorld, aIActions, info);
                if (etat == EtatNoeud.Fail)
                {
                    return etat;
                }
            }
            return etat;
        }
    }
}
