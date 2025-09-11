using GrpcServiceClient;

using ObjectsManager.Helpers;
using ObjectsManager.Interfaces;

namespace ObjectsManager.ViewModels;

public partial class MainViewModel : ViewModelBase, IMainViewModel
{
    public MainViewModel(MainService service)
    {
        Service = service;
    }

    #region Properties

    private MainService Service { get; set; }

    #endregion

    #region Commands

    #endregion
}
