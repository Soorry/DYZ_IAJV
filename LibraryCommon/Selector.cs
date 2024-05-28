using AI_BehaviorTree_AIGameUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LibraryCommon
{
    public class Selector : INoeud
    {
        public List<INoeud> noeuds { get; set; }
        public EtatNoeud etat { get; set; }


        public Selector()
        {
            noeuds = new List<INoeud>();
            etat = EtatNoeud.NotExecuted;
        }

        etatNoeud INoeud.Execute(ref GameWorldUtils gameWorld, List<AIAction> aIActions, Vector3 position)
        {
            foreach (var n in noeuds)
            {
                etat = n.Execute(ref gameWorld, aIActions, position);
                if (etat == etatNoeud.Sucess)
                {
                    return etat;
                }
            }
            return etat;
        }
    }
}
