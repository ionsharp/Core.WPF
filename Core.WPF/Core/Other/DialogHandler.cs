using System.Threading.Tasks;

namespace Imagin.Core
{
    public delegate Task DialogClosedHandler(int result);

    public delegate Task<int> DialogOpenedHandler();
}