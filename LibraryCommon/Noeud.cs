using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;

namespace LibraryCommon
{
    public interface INoeud
    {
        etatNoeud Execute(object param,List<AIAction> aIActions);
    }
    public enum etatNoeud
    {
        Fail,
        Sucess,
        Running,
        NotExecuted,
    }
}