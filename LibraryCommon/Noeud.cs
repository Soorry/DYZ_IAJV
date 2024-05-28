using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;
using UnityEngine;

namespace LibraryCommon
{
    public interface INoeud
    {
        EtatNoeud Execute(ref GameWorldUtils gameWorld,List<AIAction> aIActions, Vector3 position);
    }
    public enum EtatNoeud
    {
        Fail,
        Success,
        Running,
        NotExecuted,
    }
}