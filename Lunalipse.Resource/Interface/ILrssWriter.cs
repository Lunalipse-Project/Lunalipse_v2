namespace Lunalipse.Resource.Interface
{
    public interface ILrssWriter
    {
        bool Export();
        bool AppendResource(string path);
        bool AppendResourcesDir(string baseDir);
        bool AppendResources(params string[] pathes);
        void RemoveResource(int index);
    }
}