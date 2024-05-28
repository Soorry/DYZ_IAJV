using AI_BehaviorTree_AIGameUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace LibraryCommon
{
    public class NoeudsFire : INoeud
    {
       
        public EtatNoeud Execute(object param, List<AIAction> aIActions)
        {

            aIActions.Add(new AIActionFire());
            return EtatNoeud.Success;
        }
    }

    public class NoeudsDash : INoeud
    {


        public EtatNoeud Execute(object param, List<AIAction> aIActions)
        {
            if(param != null && param.GetType() == typeof(UnityEngine.Vector3))
            {
                aIActions.Add(new AIActionDash(new UnityEngine.Vector3(0,0,10)));
                return EtatNoeud.Success;
            }
            return EtatNoeud.Fail;
        }
    }

    public class NoeudsLookAt : INoeud
    {
       

        public EtatNoeud Execute(object param, List<AIAction> aIActions)
        {
            if (param != null && param.GetType() == typeof(UnityEngine.Vector3))
            {
                aIActions.Add(new AIActionLookAtPosition((UnityEngine.Vector3)param));
                return EtatNoeud.Success;
            }
            return EtatNoeud.Fail;
        }
    }

    public class NoeudsMoveTo : INoeud
    {
 
        public EtatNoeud Execute(object param, List<AIAction> aIActions)
        {
            if (param != null && param.GetType() == typeof(UnityEngine.Vector3))
            {
                aIActions.Add(new AIActionMoveToDestination((UnityEngine.Vector3)param));
                return EtatNoeud.Success;
            }
            return EtatNoeud.Fail;
        }
    }

    public class NoeudsStop : INoeud
    {
      
        public EtatNoeud Execute(object param, List<AIAction> aIActions)
        {
            aIActions.Add(new AIActionStopMovement());
            return EtatNoeud.Success;
        }
    }
}
