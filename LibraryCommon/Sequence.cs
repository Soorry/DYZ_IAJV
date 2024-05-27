using AI_BehaviorTree_AIGameUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetCommun
{
    public class Sequence : INoeud
    {
        List<INoeud> noeuds = new List<INoeud>();
        List<AIAction> actions = new List<AIAction>();
        etatNoeud etat = etatNoeud.NotExecuted;

        List<AIAction> INoeud.actions { get => actions; set => actions = value; }

        public etatNoeud Execute(object o)
        {
            foreach (var n in noeuds)
            {
                etat = n.Execute(o);
                actions.AddRange(n.actions);
                if (etat == etatNoeud.Fail)
                {
                    return etat;
                }
            }
            return etat;
        }
    }
}
