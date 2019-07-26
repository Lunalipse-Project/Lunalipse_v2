namespace Lunalipse.Resource.Generic
{
    public class Delegates
    {
        public delegate void ChuckOperated(int operated, int total);
        public delegate void EndpointReached(params object[] args);
        public delegate void SingleEndpointReached(string name);

        public static ChuckOperated OnChuckOperated;
        public static EndpointReached OnEndpointReached;
        public static SingleEndpointReached OnSingleEndpointReached;
    }
}
