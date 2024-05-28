using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;
using UnityEngine;

namespace LibraryCommon
{
    public interface INoeud
    {
        EtatNoeud Execute(ref BehaviourTree bTree);
    }
    public enum EtatNoeud
    {
        Fail,
        Success,
        Running,
        NotExecuted,
    }
}