using System.Threading.Tasks;

namespace ControlActive.Services
{
    public interface IHttpClientServiceImplementation
    {
        Task Execute(string ca_num);
    }
}
