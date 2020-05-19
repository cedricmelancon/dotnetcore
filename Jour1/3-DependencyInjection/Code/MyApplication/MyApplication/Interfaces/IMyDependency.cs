using System.Threading.Tasks;

namespace MyApplication.Interfaces
{
    public interface IMyDependency
    {
        Task WriteMessage(string message);
    }
}
