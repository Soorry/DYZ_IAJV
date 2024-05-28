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
        public etatNoeud etat { get; set; }


        public Selector()
        {
            noeuds = new List<INoeud>();
            etat = etatNoeud.NotExecuted;
        }

        public etatNoeud Execute(object o, List<AIAction> aIActions)
        {
            foreach (var n in noeuds)
            {
                etat = n.Execute(o, aIActions);
                if (etat == etatNoeud.Sucess)
                {
                    return etat;
                }
            }
            return etat;
        }
    }
}
