using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace ControlActive
{
    internal class ApiExplorerIgnores : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            if (action.Controller.ControllerName.Equals("Error"))
                action.ApiExplorer.IsVisible = false;
        }
    }
}