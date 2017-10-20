using Base.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Base.Entities;
using Microsoft.AspNetCore.Http.Authentication;
using System.Threading.Tasks;
using System.Security.Claims;
using Base.Web.Authentication;
using System.Collections.Generic;
using Newtonsoft.Json;
using Base.Common;
using System;
using Base.Repository;
using HTActive.Web.Helpers;
using HTActive.Common;

namespace Base.Web.Controllers
{
    [Route("account")]
    public class AccountController : BaseController
    {
        public AccountController(BaseDBRepository _BasedbRepository) : base(_BasedbRepository)
        {

        }

        private const string LoginProviderKey = "LoginProvider";
        private const string XsrfKey = "XsrfId";

        [HttpGet, Route("reset-password-hash")]
        [HTAuthorize]
        private bool ResetPasswordHash()
        {
            var entities = this.BaseDBRepository.UserRepository.GetAll().ToList();
            //entities.Select(x => x.Password = MD5Helper.Encode(x.Password));

            foreach (var entity in entities)
            {
                if (entity.Password != null)
                {
                    entity.Password = MD5Helper.Encode(entity.Password);
                }
                this.BaseDBRepository.UserRepository.Save(entity);
            }
            this.BaseDBRepository.Commit();
            return true;
        }

        [HttpGet, Route("login")]
        [AllowAnonymous]
        public IActionResult Login(string r = null)
        {
            ViewBag.Title = "Login";
            var model = new LoginViewModel();
            if (!string.IsNullOrEmpty(r))
            {
                model.RedirectUrl = r;
            }
            return View(model);
        }

        [HttpPost, Route("login")]
        [AllowAnonymous]
        public IActionResult PostLogin(LoginViewModel request)
        {
            ViewBag.Title = "Login";
            var passwordHashed = MD5Helper.Encode(request.Password);
            var user = this.BaseDBRepository.UserRepository.GetAll()
                .FirstOrDefault(x => x.UserName.ToLower().Equals(request.UserName.ToLower())
                && x.Password == passwordHashed);
            if (user == null)
            {
                request.IsError = true;
                request.ErrorMessage = "Sai tên đăng nhập hoặc mật khẩu.";
                return View("Login", request);
            }

            if (user.UserStatusId == UserStatusEnums.Deactive)
            {
                request.ErrorMessage = "Tài khoản bị khóa.";
                request.IsError = true;
                return View("Login", request);
            }

            var token = UserLogin(user, request.IsRememberMe);

            if (string.IsNullOrEmpty(token))
            {
                request.IsError = true;
                request.ErrorMessage = "Đã xảy ra lỗi. Vui lòng thử lại.";
                return View("Login", request);
            }
            return Redirect(string.IsNullOrEmpty(request.RedirectUrl) ? "/" : request.RedirectUrl);
        }
        
        private string UserLogin(User user, bool isRememberMe)
        {
            var token = new Entities.UserLoginToken()
            {
                Id = 0,
                UserId = user.Id
            };
            token.LastLoginDated = DateTimeHelper.GetDateTimeNow();

            token.ExpiredDated = token.LastLoginDated.AddDays(isRememberMe ? 14 : 1);

            token.Token = System.Guid.NewGuid().ToString().Replace("-", "");

            BaseDBRepository.UserLoginTokenRepository.Save(token);
            BaseDBRepository.Commit();

            // Delete user's expired tokens 
            var expiredTokens = BaseDBRepository.UserLoginTokenRepository.GetAll()
                .Where(t => t.UserId == user.Id &&
                t.ExpiredDated < DateTimeHelper.GetDateTimeNow()).ToList();
            BaseDBRepository.UserLoginTokenRepository.Delete(expiredTokens);
            BaseDBRepository.Commit();

            // @TODO : Merge anonymous data to logged in user and delete current anonymous user & its tokens
            var tokenString = JwtHelper.CreateJwtToken(token.Token, token.ExpiredDated);

            if (!string.IsNullOrEmpty(tokenString))
            {
                this.Response.Cookies.Delete("auth");
                this.Response.Cookies.Append("auth", tokenString, new Microsoft.AspNetCore.Http.CookieOptions()
                {
                    Path = "/",
                    Expires = new DateTimeOffset(DateTimeHelper.GetDateTimeNow().AddYears(2))
                });
            }

            return tokenString;
        }
        
        [HttpGet, Route("get-my-profile")]
        [HTAuthorize]
        public UserModel GetMyProfile()
        {
            var currentUserId = this.CurrentUser.Id;

            var entity = this.BaseDBRepository.UserRepository.GetAll()
                .Include("UserProfiles.Image")
                .Include("UserRoles.Role")
                .FirstOrDefault(x => x.Id == currentUserId);

            var model = Mappers.Mapper.ToModel(entity);
            if (model != null)
            {
                model.UserProfiles = entity.UserProfiles.Select(x =>
                {

                    var profile = Mappers.Mapper.ToModel(x);
                    if (profile != null)
                    {
                        profile.Image = Mappers.Mapper.ToModel(x.Image);
                    }
                    return profile;
                }).ToList();
                model.UserRoles = entity.UserRoles.Select(x =>
                {
                    var ur = Mappers.Mapper.ToModel(x);
                    if (ur != null)
                    {
                        ur.Role = Mappers.Mapper.ToModel(x.Role);
                    }
                    return ur;
                }).ToList();
            }

            return model;
        }
        
        [HttpGet, Route("log-out")]
        [AllowAnonymous]
        public IActionResult LogOut()
        {
            string jwt = this.HttpContext.Request.Cookies["auth"];

            if (string.IsNullOrEmpty(jwt)) return Redirect("/");

            var payloadString = JwtHelper.Decode(jwt);

            if (string.IsNullOrEmpty(payloadString)) return Redirect("/");

            var payLoad = JsonConvert.DeserializeObject<Dictionary<string, string>>(payloadString);
            var token = payLoad["token"];
            if (string.IsNullOrEmpty(token)) return Redirect("/");

            var loginSession = BaseDBRepository.UserLoginTokenRepository.GetAll()
                .Include(x => x.User)
                .FirstOrDefault(x => x.Token == token);

            if (loginSession == null) return Redirect("/");

            this.BaseDBRepository.UserLoginTokenRepository.Delete(loginSession);
            this.BaseDBRepository.Commit();
            this.Response.Cookies.Delete("auth");
            return Redirect("/");
        }
    }
}
