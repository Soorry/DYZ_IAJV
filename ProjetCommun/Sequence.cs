using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetCommun
{
    internal class Sequence : INoeud
    {
        List<INoeud> noeuds = new List<INoeud>();
        public etatNoeud etat = etatNoeud.NotExecuted;


        public etatNoeud Execute()
        {
            foreach (var o in noeuds)
            {
                etat = o.Execute();
                if (etat == etatNoeud.Fail)
                {
                    return etat;
                }
            }
            return etat;
        }
    }
}
