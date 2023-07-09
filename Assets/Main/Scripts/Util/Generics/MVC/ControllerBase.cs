using UnityEngine;

namespace Main.Scripts.Util.Generic_Helpers.MVC
{
    public abstract class ControllerBase<TModel, TView> : MonoBehaviour where TModel : ModelBase where TView : ViewBase 
    {
        protected TModel _model;
        protected TView _view;

        public ControllerBase(TModel model, TView view)
        {
            _model = model;
            _view = view;
        }
    }
}
