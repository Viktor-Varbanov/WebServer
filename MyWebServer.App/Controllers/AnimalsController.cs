namespace MyWebServer.App.Controllers
{
    using MyWebServer.App.Models.Animals;
    using MyWebServer.Controllers;
    using MyWebServer.Http;
    using MyWebServer.Results;

    public class AnimalsController : Controller
    {
        public AnimalsController(HttpRequest request)
            : base(request)
        {
        }

        public ActionResult Cats()
        {
            const string nameKey = "Name";
            const string ageKey = "Age";

            var query = this.Request.Query;

            var catName = query.ContainsKey(nameKey)
                ? query[nameKey]
                : "the cats";

            int catAge = query.ContainsKey(ageKey)
                ? int.Parse(query[ageKey])
                : 1;

            var result = $"<h1>Hello from {catName}!</h1>";

            return View(new CatViewModel(catName, catAge));
        }

        public ActionResult Dogs() => View();

        public ActionResult Bunnies() => View("Rabbits");

        public ActionResult Turtles() => View("Animals/Wild/Turtles");
    }
}
