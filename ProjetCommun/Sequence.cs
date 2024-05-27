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
        public bool Execute()
        {
            return true;
        }
    }
}
