using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;

namespace ProjetCommun
{
    public interface INoeud
    {
        List<AIAction> actions { get; set; }
        etatNoeud Execute(object param);
    }
    public enum etatNoeud
    {
        Fail,
        Sucess,
        Running,
        NotExecuted,
    }
}