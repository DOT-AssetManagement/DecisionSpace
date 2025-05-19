using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using Volo.Abp.Account.Settings;
using Volo.Abp;
using Volo.Abp.Account.Web;
using Volo.Abp.Account.Web.Pages.Account;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Settings;
using Volo.Abp.Validation;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;

namespace DSS.Web.Pages.Account
{
    public class CustomLoginModel : LoginModel
    {
        private readonly IDataFilter _dataFilter;
        public CustomLoginModel(IAuthenticationSchemeProvider schemeProvider, 
            IOptions<AbpAccountOptions> accountOptions, IOptions<IdentityOptions> identityOptions, 
            IdentityDynamicClaimsPrincipalContributorCache identityDynamicClaimsPrincipalContributorCache, 
            IDataFilter dataFilter) : base(schemeProvider, accountOptions, identityOptions, identityDynamicClaimsPrincipalContributorCache)
        {
            _dataFilter = dataFilter;
        }

        public override async Task<IActionResult> OnGetAsync()
        {
            LoginInput = new LoginInputModel();

            ExternalProviders = await GetExternalProviders();

            EnableLocalLogin = await SettingProvider.IsTrueAsync(AccountSettingNames.EnableLocalLogin);

            if (IsExternalLoginOnly)
            {
                return await OnPostExternalLogin(ExternalProviders.First().AuthenticationScheme);
            }

            return Page();
        }

        public override async Task<IActionResult> OnPostAsync(string action)
        {
            await CheckLocalLoginAsync();

            ValidateModel();

            ExternalProviders = await GetExternalProviders();

            EnableLocalLogin = await SettingProvider.IsTrueAsync(AccountSettingNames.EnableLocalLogin);

            await ReplaceEmailToUsernameOfInputIfNeeds();

            await IdentityOptions.SetAsync();

            var result = await SignInManager.PasswordSignInAsync(
                LoginInput.UserNameOrEmailAddress,
                LoginInput.Password,
                LoginInput.RememberMe,
                true
            );

            await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
            {
                Identity = IdentitySecurityLogIdentityConsts.Identity,
                Action = result.ToIdentitySecurityLogAction(),
                UserName = LoginInput.UserNameOrEmailAddress
            });

            if (result.RequiresTwoFactor)
            {
                return await TwoFactorLoginResultAsync();
            }

            if (result.IsLockedOut)
            {
                Alerts.Warning(L["UserLockedOutMessage"]);
                return Page();
            }

            if (result.IsNotAllowed)
            {
                Alerts.Warning(L["LoginIsNotAllowed"]);
                return Page();
            }

            if (!result.Succeeded)
            {
                Alerts.Danger(L["InvalidUserNameOrPassword"]);
                return Page();
            }

            //TODO: Find a way of getting user's id from the logged in user and do not query it again like that!
            var user = await UserManager.FindByNameAsync(LoginInput.UserNameOrEmailAddress) ??
                       await UserManager.FindByEmailAsync(LoginInput.UserNameOrEmailAddress);

            Debug.Assert(user != null, nameof(user) + " != null");

            // Clear the dynamic claims cache.
            await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(user.Id, user.TenantId);

            return await RedirectSafelyAsync(ReturnUrl, ReturnUrlHash);
        }

        /// <summary>
        /// Override this method to add 2FA for your application.
        /// </summary>
        protected override Task<IActionResult> TwoFactorLoginResultAsync()
        {
            throw new NotImplementedException();
        }

        protected override async Task<List<ExternalProviderModel>> GetExternalProviders()
        {
            var schemes = await SchemeProvider.GetAllSchemesAsync();

            return schemes
                .Where(x => x.DisplayName != null || x.Name.Equals(AccountOptions.WindowsAuthenticationSchemeName, StringComparison.OrdinalIgnoreCase))
                .Select(x => new ExternalProviderModel
                {
                    DisplayName = x.DisplayName,
                    AuthenticationScheme = x.Name
                })
                .ToList();
        }

        public override async Task<IActionResult> OnPostExternalLogin(string provider)
        {
            var redirectUrl = Url.Page("./Login", pageHandler: "ExternalLoginCallback", values: new { ReturnUrl, ReturnUrlHash });
            var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            properties.Items["scheme"] = provider;

            return await Task.FromResult(Challenge(properties, provider));
        }
        public override async Task<IActionResult> OnGetExternalLoginCallbackAsync(string returnUrl = "", string returnUrlHash = "", string remoteError = null)
        {
            //TODO: Did not implemented Identity Server 4 sample for this method (see ExternalLoginCallback in Quickstart of IDS4 sample)
            /* Also did not implement these:
             * - Logout(string logoutId)
             */
            using (_dataFilter.Disable<IMultiTenant>())
            {
                if (remoteError != null)
                {
                    Logger.LogWarning($"External login callback error: {remoteError}");
                    return RedirectToPage("./Login");
                }

                await IdentityOptions.SetAsync();

                var loginInfo = await SignInManager.GetExternalLoginInfoAsync();
                if (loginInfo == null)
                {
                    Logger.LogWarning("External login info is not available");
                    return RedirectToPage("./Login");
                }

                var result = await SignInManager.ExternalLoginSignInAsync(
                    loginInfo.LoginProvider,
                    loginInfo.ProviderKey,
                    isPersistent: false,
                    bypassTwoFactor: true
                );

                if (!result.Succeeded)
                {
                    await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
                    {
                        Identity = IdentitySecurityLogIdentityConsts.IdentityExternal,
                        Action = "Login" + result
                    });
                }

                if (result.IsLockedOut)
                {
                    Logger.LogWarning($"External login callback error: user is locked out!");
                    throw new UserFriendlyException("Cannot proceed because user is locked out!");
                }

                if (result.IsNotAllowed)
                {
                    Logger.LogWarning($"External login callback error: user is not allowed!");
                    throw new UserFriendlyException("Cannot proceed because user is not allowed!");
                }

                Volo.Abp.Identity.IdentityUser user;
                if (result.Succeeded)
                {
                    user = await UserManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);
                    if (user != null)
                    {
                        await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(user.Id, user.TenantId);
                    }
                    if (User.IsInRole("Guest"))
                    {
                        // Redirect to GuestIndex
                        return RedirectToPage("/GuestIndex");
                    }
                    else if (User.IsInRole("admin"))
                    {
                        return RedirectToPage("/Identity/Users");
                    }
                    return RedirectToPage("/Dashboard");
                }
                var email = loginInfo.Principal.FindFirstValue("emails");
                if (email.IsNullOrWhiteSpace())
                {
                    return RedirectToPage("./Register", new
                    {
                        IsExternalLogin = true,
                        ExternalLoginAuthSchema = loginInfo.LoginProvider,
                        ReturnUrl = returnUrl
                    });
                }

                user = await UserManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return RedirectToPage("./Register", new
                    {
                        IsExternalLogin = true,
                        ExternalLoginAuthSchema = loginInfo.LoginProvider,
                        ReturnUrl = returnUrl
                    });
                }
                await SignInManager.SignInAsync(user, false);

                await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
                {
                    Identity = IdentitySecurityLogIdentityConsts.IdentityExternal,
                    Action = result.ToIdentitySecurityLogAction(),
                    UserName = user.Name
                });
                await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(user.Id, user.TenantId);
                if (User.IsInRole("Guest"))
                {
                    // Redirect to GuestIndex
                    return RedirectToPage("/GuestIndex");
                }else if (User.IsInRole("admin"))
                {
                    return RedirectToPage("/Identity/Users");
                }
                return RedirectToPage("/Dashboard");
            }
        }

        protected virtual async Task ReplaceEmailToUsernameOfInputIfNeeds()
        {
            if (!ValidationHelper.IsValidEmailAddress(LoginInput.UserNameOrEmailAddress))
            {
                return;
            }

            var userByUsername = await UserManager.FindByNameAsync(LoginInput.UserNameOrEmailAddress);
            if (userByUsername != null)
            {
                return;
            }

            var userByEmail = await UserManager.FindByEmailAsync(LoginInput.UserNameOrEmailAddress);
            if (userByEmail == null)
            {
                return;
            }

            LoginInput.UserNameOrEmailAddress = userByEmail.UserName;
        }

        protected virtual async Task CheckLocalLoginAsync()
        {
            if (!await SettingProvider.IsTrueAsync(AccountSettingNames.EnableLocalLogin))
            {
                throw new UserFriendlyException(L["LocalLoginDisabledMessage"]);
            }
        }

    }
}
