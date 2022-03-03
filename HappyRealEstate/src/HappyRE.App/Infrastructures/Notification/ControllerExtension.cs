using System.Web.Mvc;

namespace HappyRE.App.Infrastructures.Notification
{
    public static class ControllerExtension
    {
        public static ToastMessage ShowSuccess(this Controller controller, string message, string title = "")
        {
            return ShowMessage(controller, title, message, ToastType.Success);
        }

        public static ToastMessage ShowError(this Controller controller, string message, string title = "")
        {
            return ShowMessage(controller, title, message, ToastType.Error);
        }

        public static ToastMessage ShowInfo(this Controller controller, string message, string title = "")
        {
            return ShowMessage(controller, title, message, ToastType.Info);
        }

        public static ToastMessage ShowWarning(this Controller controller, string message, string title = "")
        {
            return ShowMessage(controller, title, message, ToastType.Warning);
        }

        private static ToastMessage ShowMessage(Controller controller, string title, string message,
            ToastType toastType = ToastType.Info)
        {
            var toastr = controller.TempData["Toastr"] as Toastr;
            toastr = toastr ?? new Toastr();

            var toastMessage = toastr.AddToastMessage(title, message, toastType);
            controller.TempData["Toastr"] = toastr;
            return toastMessage;
        }
    }
}