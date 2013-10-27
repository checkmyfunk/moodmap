using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Mvc.Facebook;
using Microsoft.AspNet.Mvc.Facebook.Client;
using WorldMoodMap.Models;
using Logic.Entities;

namespace WorldMoodMap.Controllers
{
    public class HomeController : Controller
    {
        [FacebookAuthorize("email", "user_photos")]
        public async Task<ActionResult> Index(FacebookContext context)
        {
            return await LoadView(context);
        }

        [HttpPost]
        public ActionResult SaveMood(MoodDTO data)
        {
            User user = Logic.Entities.User.GetById(data.userId);
            user.MoodId = data.moodId;
            user.CountryId = data.countryId;
            user.Save();

            return Json(new { success = true });

            //return await LoadView(context);
            //return await LoadView(context, moodId, countryId);
        }



        private async Task<ActionResult> LoadView(FacebookContext context, string moodId = null, string countryId = null)
        {
            if (ModelState.IsValid)
            {
                var fbuser = await context.Client.GetCurrentUserAsync<MyAppUser>();

                User user = Logic.Entities.User.GetFirst(x => x.FacebookId == fbuser.Id);

                if (user == null)
                {
                    user = new User();
                    user.FacebookId = fbuser.Id;
                    user.Mood = Mood.GetFirst(x => x.Name == "excited");
                    user.Country = Country.GetFirst(x => x.Name == "Germany");
                    user.Save();
                }

                if (!string.IsNullOrEmpty(moodId))
                {
                    user.Mood = Mood.GetById(int.Parse(moodId));
                    user.Country = Country.GetById(int.Parse(countryId));
                    user.Save();
                }

                HomeModel model = new HomeModel();
                model.User = fbuser;
                model.Mood = user.Mood;
                model.Country = user.Country;
                model.UserId = user.Id;

                return View(model);
            }

            return View("Error");
        }

        // This action will handle the redirects from FacebookAuthorizeFilter when 
        // the app doesn't have all the required permissions specified in the FacebookAuthorizeAttribute.
        // The path to this action is defined under appSettings (in Web.config) with the key 'Facebook:AuthorizationRedirectPath'.
        public ActionResult Permissions(FacebookRedirectContext context)
        {
            if (ModelState.IsValid)
            {
                return View(context);
            }

            return View("Error");
        }
    }
}
