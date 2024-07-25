using Jint;

namespace Tendeos.Modding
{
    public interface IModScript
    {
        object invoke(string name, params object[] args);
        IModMethod function(string name);
        object get(string name);
        bool has(string name);

        void Init();

        void Add(string name, object obj);
    }
}