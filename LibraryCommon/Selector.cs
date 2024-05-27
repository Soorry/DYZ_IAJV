using AI_BehaviorTree_AIGameUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjetCommun
{
    public class Selector : INoeud
    {
        public List<INoeud> noeuds = new List<INoeud>();
        public etatNoeud etat = etatNoeud.NotExecuted;

        public List<AIAction> actions { get => actions; set => actions = value; }

        public Selector()
        {
            actions = new List<AIAction>();
        }

        public etatNoeud Execute(object o)
        {
            foreach (var n in noeuds)
            {
                etat = n.Execute(o);
                actions.AddRange(n.actions);
                if (etat == etatNoeud.Sucess)
                {
                    return etat;
                }
            }
            return etat;
        }
    }
}
