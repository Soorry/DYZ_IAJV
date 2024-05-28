using AI_BehaviorTree_AIGameUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public EtatNoeud Execute(object o, List<AIAction> aIActions)
        {
            foreach (var n in noeuds)
            {
                etat = n.Execute(o, aIActions);
                if (etat == EtatNoeud.Fail)
                {
                    return etat;
                }
            }
            return etat;
        }
    }
}
