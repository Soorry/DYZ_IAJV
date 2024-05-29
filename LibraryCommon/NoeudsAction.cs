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
        public EtatNoeud Execute(ref BehaviourTree bTree)
        {
            bTree.actions.Add(new AIActionFire());
            return EtatNoeud.Success;
        }
    }

    public class NoeudsDash : INoeud
    {
        public EtatNoeud Execute(ref BehaviourTree bTree)
        {
            bTree.actions.Add(new AIActionDash(bTree.position));
            return EtatNoeud.Success;
        }
    }

    public class NoeudsLookAt : INoeud
    {
        public EtatNoeud Execute(ref BehaviourTree bTree)
        {
            bTree.actions.Add(new AIActionLookAtPosition(bTree.position));
            return EtatNoeud.Success;
        }
    }

    public class NoeudsMoveTo : INoeud
    {
        public EtatNoeud Execute(ref BehaviourTree bTree)
        {
            bTree.actions.Add(new AIActionMoveToDestination(bTree.position));
            return EtatNoeud.Success;
        }
    }

    public class NoeudsStop : INoeud
    {
        public EtatNoeud Execute(ref BehaviourTree bTree)
        {
            bTree.actions.Add(new AIActionStopMovement());
            return EtatNoeud.Success;
        }
    }
}
