using AI_BehaviorTree_AIGameUtility;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace LibraryCommon
{
    public class NoeudsFire : INoeud
    {

        public EtatNoeud Execute(ref GameWorldUtils gameWorld, List<AIAction> aIActions, object info)
        {

            aIActions.Add(new AIActionFire());
            return EtatNoeud.Success;
        }
    }

    public class NoeudsDash : INoeud
    {
        public EtatNoeud Execute(ref GameWorldUtils gameWorld, List<AIAction> aIActions, object info)
        {
            aIActions.Add(new AIActionDash((Vector3)info));
            return EtatNoeud.Success;
        }
    }

    public class NoeudsLookAt : INoeud
    {
        public EtatNoeud Execute(ref GameWorldUtils gameWorld, List<AIAction> aIActions, object info)
        {
            aIActions.Add(new AIActionLookAtPosition((Vector3)info));
            return EtatNoeud.Success;
        }
    }

    public class NoeudsMoveTo : INoeud
    {

        public EtatNoeud Execute(ref GameWorldUtils gameWorld, List<AIAction> aIActions, object info)
        {
            aIActions.Add(new AIActionMoveToDestination((Vector3)info));
            return EtatNoeud.Success;
        }
    }

    public class NoeudsStop : INoeud
    {
        public EtatNoeud Execute(ref GameWorldUtils gameWorld, List<AIAction> aIActions, object info)
        {
            aIActions.Add(new AIActionStopMovement());
            return EtatNoeud.Success;
        }
    }
}
