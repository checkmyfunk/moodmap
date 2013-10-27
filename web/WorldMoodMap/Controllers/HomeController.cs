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

            Country country = Country.GetById(data.countryId);
            Country oldCountry = Country.GetById(user.CountryId);

            user.CountryId = data.countryId;
            user.Save();

            RecalculateScore(country, oldCountry);

            return Json(new { success = true });
        }

        public ActionResult Map()
        {
            MapModel model = new MapModel();
            var availableCountries = Logic.Entities.User.GetAll().Select(x => x.CountryId).Distinct();
            model.Countries = Country.GetAll().Where(x => availableCountries.Contains(x.Id)).ToList();

            return View(model);
        }

        private void RecalculateScore(Country country, Country oldCountry)
        {
            var countryMood = (
                from u in Logic.Entities.User.AsQueryable()
                group u by u.CountryId into g
                select new { g.Key.Value, Sum = g.Sum(u => u.Mood.Score), Count = g.Count() }
            ).ToList();

            if (oldCountry != null)
            {
                var oldCountryValues = countryMood.Where(x => x.Value == oldCountry.Id).FirstOrDefault();
                if (oldCountryValues != null)
                    oldCountry.Score = oldCountryValues.Sum / oldCountryValues.Count;
                else
                    oldCountry.Score = null;
            }
            oldCountry.Save();

            var countryValues = countryMood.Where(x => x.Value == country.Id).FirstOrDefault();
            country.Score = countryValues.Sum / countryValues.Count;
            country.Save();
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
                model.Friends = new List<UserFriend>();

                if (fbuser.Friends != null && fbuser.Friends.Data != null)
                {
                    foreach (MyAppUserFriend fbfriend in fbuser.Friends.Data)
                    {
                        UserFriend friend = new UserFriend();
                        friend.Name = fbfriend.Name;
                        friend.Link = fbfriend.Link;
                        friend.Picture = fbfriend.Picture;
                        var friendUser = Logic.Entities.User.GetFirst(x => x.FacebookId == fbfriend.Id);
                        if (friendUser != null)
                            friend.Mood = friendUser.Mood;
                        else
                            friend.Mood = Mood.GetFirst(x => x.Name == "meh");
                        model.Friends.Add(friend);
                    }
                }

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
