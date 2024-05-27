using AI_BehaviorTree_AIGameUtility;

namespace ProjetCommun
{
    public interface INoeud
    {
        List<AIAction> actions { get; set; }
        etatNoeud Execute(Object param);
    }
    public enum etatNoeud
    {
        Fail,
        Sucess,
        Running,
        NotExecuted,
    }
}