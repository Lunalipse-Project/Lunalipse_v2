namespace LunalipseInstaller.Procedure
{
    public interface IProcedure
    {
        void Main();
        object GetResult();
        string GetModuleName();
    }
}
