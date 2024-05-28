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
        public List<AIAction> actions { get => actions; set => actions = value; }

        public etatNoeud Execute(object param)
        {
            actions.Add(new AIActionFire());
            return etatNoeud.Sucess;
        }
    }

    public class NoeudsDash : INoeud
    {
        public List<AIAction> actions { get => actions; set => actions = value; }

        public etatNoeud Execute(object param)
        {
            if(param != null && param.GetType() == typeof(UnityEngine.Vector3))
            {
                actions.Add(new AIActionDash(new UnityEngine.Vector3(0,0,10)));
                return etatNoeud.Sucess;
            }
            return etatNoeud.Fail;
        }
    }

    public class NoeudsLookAt : INoeud
    {
        public List<AIAction> actions { get => actions; set => actions = value; }

        public etatNoeud Execute(object param)
        {
            if (param != null && param.GetType() == typeof(UnityEngine.Vector3))
            {
                actions.Add(new AIActionLookAtPosition((UnityEngine.Vector3)param));
                return etatNoeud.Sucess;
            }
            return etatNoeud.Fail;
        }
    }

    public class NoeudsMoveTo : INoeud
    {
        public List<AIAction> actions { get => actions; set => actions = value; }

        public etatNoeud Execute(object param)
        {
            if (param != null && param.GetType() == typeof(UnityEngine.Vector3))
            {
                actions.Add(new AIActionMoveToDestination((UnityEngine.Vector3)param));
                return etatNoeud.Sucess;
            }
            return etatNoeud.Fail;
        }
    }

    public class NoeudsStop : INoeud
    {
        public List<AIAction> actions { get => actions; set => actions = value; }

        public etatNoeud Execute(object param)
        {
            actions.Add(new AIActionStopMovement());
            return etatNoeud.Sucess;
        }
    }
}
