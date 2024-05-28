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

        EtatNoeud INoeud.Execute(ref GameWorldUtils gameWorld, List<AIAction> aIActions, Vector3 position)
        {

            aIActions.Add(new AIActionFire());
            return EtatNoeud.Success;
        }
    }

    public class NoeudsDash : INoeud
    {
        EtatNoeud INoeud.Execute(ref GameWorldUtils gameWorld, List<AIAction> aIActions, Vector3 position)
        {
            aIActions.Add(new AIActionDash(position));
            return EtatNoeud.Success;
        }
    }

    public class NoeudsLookAt : INoeud
    {
        EtatNoeud INoeud.Execute(ref GameWorldUtils gameWorld, List<AIAction> aIActions, Vector3 position)
        {
            aIActions.Add(new AIActionLookAtPosition(position));
            return EtatNoeud.Success;
        }
    }

    public class NoeudsMoveTo : INoeud
    {

        EtatNoeud INoeud.Execute(ref GameWorldUtils gameWorld, List<AIAction> aIActions, Vector3 position)
        {
            aIActions.Add(new AIActionMoveToDestination(position));
            return EtatNoeud.Success;
        }
    }

    public class NoeudsStop : INoeud
    {
        EtatNoeud INoeud.Execute(ref GameWorldUtils gameWorld, List<AIAction> aIActions, Vector3 position)
        {
            aIActions.Add(new AIActionStopMovement());
            return EtatNoeud.Success;
        }
    }
}
