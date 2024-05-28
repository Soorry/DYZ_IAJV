using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;

namespace LibraryCommon
{
    public interface INoeud
    {
        EtatNoeud Execute(object param,List<AIAction> aIActions);
    }
    public enum EtatNoeud
    {
        Fail,
        Success,
        Running,
        NotExecuted,
    }
}