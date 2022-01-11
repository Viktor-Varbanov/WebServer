namespace MyWebServer.App.Controllers
{
    using MyWebServer.Controllers;
    using MyWebServer.Http;
    using MyWebServer.Results;
    using System;

    public class HomeController : Controller
    {
        public HomeController(HttpRequest request)
            : base(request)
        {
        }

        public ActionResult Index() => Text("Hello from Ivo!");

        public ActionResult LocalRedirect() => Redirect("/Cats");

        public ActionResult ToSoftUni() => Redirect("https://softuni.bg");

        public HttpResponse Error() => throw new InvalidOperationException();
    }
}
