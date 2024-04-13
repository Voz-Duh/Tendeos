using Jint;

namespace Tendeos.Modding
{
    public interface IModScript
    {
        object invoke(string name, params object[] args);
        IModMethod function(string name);
        object get(string name);
        bool has(string name);

        internal void Init();

        internal void Add(string name, object obj);
    }
}