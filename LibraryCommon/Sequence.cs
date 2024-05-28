using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;
using UnityEngine;

namespace LibraryCommon
{
    public class Sequence : INoeud
    {
        List<INoeud> noeuds = new List<INoeud>();
        EtatNoeud etat = EtatNoeud.NotExecuted;

        public Sequence()
        {
            noeuds = new List<INoeud>();
        }


        EtatNoeud INoeud.Execute(ref GameWorldUtils gameWorld, List<AIAction> aIActions, Vector3 position)
        {
            foreach (var n in noeuds)
            {
                etat = n.Execute(ref gameWorld, aIActions, position);
                if (etat == EtatNoeud.Fail)
                {
                    return etat;
                }
            }
            return etat;
        }
    }
}
