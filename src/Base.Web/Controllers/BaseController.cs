using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Base.Repository;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Base.Web.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Base.Common;
using Base.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using HTActive.Web.Helpers;
using HTActive.Common;

namespace Base.Web.Controllers
{
    public class BaseController : Controller
    {
        protected BaseDBRepository BaseDBRepository { get; private set; }

        public BaseController(BaseDBRepository repository)
        {
            this.BaseDBRepository = repository;
            Configuration = repository.ServiceProvider.GetService<IOptions<ConfigurationHelper>>()?.Value;
        }

        protected ConfigurationHelper Configuration { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.CurrentUser = this.CurrentUser;
            base.OnActionExecuting(context);
        }

        private UserModel currentUser;

        protected UserModel CurrentUser
        {
            get
            {
                if (currentUser == null)
                {
                    string jwt = this.HttpContext.Request.Cookies["auth"];

                    if (string.IsNullOrEmpty(jwt) || jwt == "null") return null;

                    var payloadString = JwtHelper.Decode(jwt);

                    if (string.IsNullOrEmpty(payloadString)) return null;

                    var payLoad = JsonConvert.DeserializeObject<Dictionary<string, string>>(payloadString);
                    var token = payLoad["token"];
                    if (string.IsNullOrEmpty(token)) return null;

                    var loginSession = BaseDBRepository.UserLoginTokenRepository.GetAll()
                        .Include("User.UserProfiles.Image")
                        .Include("User.UserRoles.Role")
                        .FirstOrDefault(x => x.Token == token);
                    if (loginSession == null || loginSession.User == null) return null;

                    //Check if user was banned
                    if (loginSession?.User?.UserStatusId == UserStatusEnums.Deactive) return null;

                    if (loginSession.ExpiredDated < DateTimeHelper.GetDateTimeNow())
                    {
                        return null;
                    }

                    currentUser = Mappers.Mapper.ToModel(loginSession.User);
                    if (currentUser != null)
                    {
                        currentUser.UserProfiles = loginSession.User.UserProfiles.Select(x =>
                        {
                            var profile = Mappers.Mapper.ToModel(x);
                            if (profile != null)
                            {
                                profile.Image = Mappers.Mapper.ToModel(x.Image);
                            }
                            return profile;
                        }).ToList();
                        currentUser.UserRoles = loginSession.User.UserRoles.Select(x =>
                        {
                            var role = Mappers.Mapper.ToModel(x);
                            if (role != null)
                            {
                                role.Role = Mappers.Mapper.ToModel(x.Role);
                            }
                            return role;
                        }).ToList();
                    }
                }
                return currentUser;
            }
        }
        
        protected string BuildTemplate(string template, Dictionary<string, string> tokens)
        {
            if (string.IsNullOrEmpty(template)) return template;
            foreach (var token in tokens)
            {
                template = template.Replace(token.Key, token.Value);
            }
            return template;
        }
    }
}
