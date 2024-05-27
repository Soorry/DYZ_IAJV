namespace ProjetCommun
{
    public interface INoeud
    {
        // tu es générique
        etatNoeud Execute();
    }
    public enum etatNoeud
    {
        Fail,
        Sucess,
        Running,
        NotExecuted,
    }
}