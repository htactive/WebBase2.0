using Base.Common;
using Base.Entities;
using Base.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Base.Web.Controllers
{
    [Route("role")]
    public class RoleController : BaseController
    {
        public RoleController(Repository.BaseDBRepository _BaseDBRepository) : base(_BaseDBRepository)
        {
        }

        [HttpGet, Route("init-roles")]
        [AllowAnonymous]
        public bool InitRoles()
        {
            if (!this.BaseDBRepository.RoleRepository.GetAll().Any(x => x.RoleType == RoleTypeEnums.School))
            {
                this.BaseDBRepository.RoleRepository.Save(new Role()
                {
                    DisplayName = "Trường học",
                    Id = 0,
                    Name = "School",
                    RoleType = RoleTypeEnums.School
                });
            }

            if (!this.BaseDBRepository.RoleRepository.GetAll().Any(x => x.RoleType == RoleTypeEnums.CommitteeDivision))
            {
                this.BaseDBRepository.RoleRepository.Save(new Role()
                {
                    DisplayName = "Phòng",
                    Id = 0,
                    Name = "CommitteeDivision",
                    RoleType = RoleTypeEnums.CommitteeDivision
                });
            }
            if (!this.BaseDBRepository.RoleRepository.GetAll().Any(x => x.RoleType == RoleTypeEnums.Department))
            {
                this.BaseDBRepository.RoleRepository.Save(new Role()
                {
                    DisplayName = "Sở",
                    Id = 0,
                    Name = "Department",
                    RoleType = RoleTypeEnums.Department
                });
            }
            if (!this.BaseDBRepository.RoleRepository.GetAll().Any(x => x.RoleType == RoleTypeEnums.Ministry))
            {
                this.BaseDBRepository.RoleRepository.Save(new Role()
                {
                    DisplayName = "Bộ",
                    Id = 0,
                    Name = "Ministry",
                    RoleType = RoleTypeEnums.Ministry
                });
            }
            if (!this.BaseDBRepository.RoleRepository.GetAll().Any(x => x.RoleType == RoleTypeEnums.Administrator))
            {
                this.BaseDBRepository.RoleRepository.Save(new Role()
                {
                    DisplayName = "Người quản trị",
                    Id = 0,
                    Name = "Administrator",
                    RoleType = RoleTypeEnums.Administrator
                });
            }
            this.BaseDBRepository.Commit();

            return true;
        }

        [HttpGet, Route("init-role-claims")]
        [AllowAnonymous]
        public bool InitRoleClaims()
        {
            var roleClaims = this.BaseDBRepository.RoleClaimRepository.GetAll().ToList();
            this.BaseDBRepository.RoleClaimRepository.Delete(roleClaims);
            this.BaseDBRepository.Commit();
            
            this.BaseDBRepository.Commit();
            return true;
        }

        private void AddClaimToRoles(string claim, List<RoleTypeEnums> roleTypes)
        {
            var claimEntity = this.BaseDBRepository.ClaimRepository.GetAll()
                .Include(x => x.RoleClaims)
                .FirstOrDefault(x => x.ClaimName == claim);
            if (claimEntity == null)
            {
                claimEntity = new Claim()
                {
                    ClaimName = claim,
                    RoleClaims = new List<RoleClaim>()
                };
            }
            var roleIds = this.BaseDBRepository.RoleRepository.GetAll()
                .Where(x => roleTypes.Contains(x.RoleType)).Select(x => x.Id).ToList();
            foreach (var roleId in roleIds)
            {
                if (!claimEntity.RoleClaims.Any(x => x.RoleId == roleId))
                {
                    claimEntity.RoleClaims.Add(new RoleClaim()
                    {
                        RoleId = roleId
                    });
                }
            }
            this.BaseDBRepository.ClaimRepository.Save(claimEntity);
        }

        [HttpGet, Route("get-all-user-roles")]
        [AllowAnonymous]
        public List<RoleModel> GetAllUserRoles()
        {
            var entities = this.BaseDBRepository.RoleRepository.GetAll().ToList();

            return Mappers.Mapper.ToModel(entities);
        }

    }
}
