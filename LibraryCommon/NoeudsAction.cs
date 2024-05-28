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

        etatNoeud INoeud.Execute(ref GameWorldUtils gameWorld, List<AIAction> aIActions, Vector3 position)
        {

            aIActions.Add(new AIActionFire());
            return EtatNoeud.Success;
        }
    }

    public class NoeudsDash : INoeud
    {
        etatNoeud INoeud.Execute(ref GameWorldUtils gameWorld, List<AIAction> aIActions, Vector3 position)
        {
            aIActions.Add(new AIActionDash(position));
            return etatNoeud.Sucess;
        }
    }

    public class NoeudsLookAt : INoeud
    {
        etatNoeud INoeud.Execute(ref GameWorldUtils gameWorld, List<AIAction> aIActions, Vector3 position)
        {
            aIActions.Add(new AIActionLookAtPosition(position));
            return etatNoeud.Sucess;
        }
    }

    public class NoeudsMoveTo : INoeud
    {

        etatNoeud INoeud.Execute(ref GameWorldUtils gameWorld, List<AIAction> aIActions, Vector3 position)
        {
            aIActions.Add(new AIActionMoveToDestination(position));
            return etatNoeud.Sucess;
        }
    }

    public class NoeudsStop : INoeud
    {
        etatNoeud INoeud.Execute(ref GameWorldUtils gameWorld, List<AIAction> aIActions, Vector3 position)
        {
            aIActions.Add(new AIActionStopMovement());
            return EtatNoeud.Success;
        }
    }
}
