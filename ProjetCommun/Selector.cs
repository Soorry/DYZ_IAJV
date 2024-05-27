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

        bool Execute()
        {
            return true;
        }

    }
}
