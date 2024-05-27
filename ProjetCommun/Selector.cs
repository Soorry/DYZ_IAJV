using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetCommun
{
    internal class Selector : INoeud
    {
        List<INoeud> noeuds = new List<INoeud>();
        public etatNoeud etat = etatNoeud.NotExecuted;

        public etatNoeud Execute()
        {
            foreach (var o in noeuds)
            {
                if (o.Execute()==etatNoeud.Sucess)
                {
                    etat = etatNoeud.Sucess;
                    return etat;
                }
            }
            etat = etatNoeud.Fail;
            return etat;
        }

    }
}
