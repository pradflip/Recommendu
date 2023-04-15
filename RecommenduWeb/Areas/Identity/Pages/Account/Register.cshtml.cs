// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using RecommenduWeb.Models;
using RecommenduWeb.Services;

namespace RecommenduWeb.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UserManager<Usuario> _userManager;
        private readonly IUserStore<Usuario> _userStore;
        private readonly IUserEmailStore<Usuario> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly LocalidadeService _localidadeService;

        public RegisterModel(
            UserManager<Usuario> userManager,
            IUserStore<Usuario> userStore,
            SignInManager<Usuario> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            LocalidadeService localidadeService)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _localidadeService = localidadeService;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            /// Nome Completo
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "O usuário deve ter entre {2} a {1} caracteres.", MinimumLength = 10)]
            [DataType(DataType.Text)]
            [Display(Name = "Nome Completo")]
            public string NomeCompleto { get; set; }

            /// <summary>
            /// Username
            /// </summary>
            //[Required (ErrorMessage = "{0} requerido;")]
            [Required]
            [StringLength(20, ErrorMessage = "O usuário deve ter entre {2} a {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Usuário")]
            public string UserName { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(50, ErrorMessage = "A senha deve ter entre {2} a {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Senha")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirmar Senha")]
            [Compare("Password", ErrorMessage = "A senha verificada não combina.")]
            public string ConfirmPassword { get; set; }

            // Estado
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Estado")]
            public string Estado { get; set; }

            // Cidade
            [Required]
            [StringLength(100, ErrorMessage = "A cidade deve ter entre {2} a {1} caracteres.", MinimumLength = 4)]
            [DataType(DataType.Text)]
            [Display(Name = "Cidade")]
            public string Cidade { get; set; }

        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Alimenta viewdata com a lista de estados para o metodo GET
            // ViewData precisa ter o mesmo nome da propriedade
            ViewData["Input.Estado"] = await GetEstadoAsync();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            // Alimenta viewdata com a lista de estados para o metodo POST
            // ViewData precisa ter o mesmo nome da propriedade
            var estados = await GetEstadoAsync();
            ViewData["Input.Estado"] = estados;

            // Valida se um estado foi selecionado
            if (Input.Estado == "0")
            {
                ModelState.AddModelError(string.Empty, "Selecione o Estado.");
                return Page();
            }

            // Atualiza o Input.Estado buscando a sigla do estado de acordo com o ID recebido do proprio Input.Estado
            Input.Estado = estados.Where(p => p.Value == Input.Estado).First().Text;

            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                Random random = new Random();
                int num = random.Next(1, 3);
                string profileImage = $"default-profile-image-{num}.png";
                Input.UserName = Input.UserName.ToLower();
                var user = new Usuario { NomeCompleto = Input.NomeCompleto.Titleize(), UserName = Input.UserName, Email = Input.Email, Cidade = Input.Cidade, Estado = Input.Estado, ImagemPerfil = profileImage };
                //var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.UserName, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<Usuario> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<Usuario>)_userStore;
        }

        public async Task<IEnumerable<SelectListItem>> GetEstadoAsync()
        {
            DataTable dt = new DataTable();
            
            // Consulta o serviço que realiza um request na API do IBGE e retorna um DataTable
            dt = await _localidadeService.ListaEstadoAsync();

            // Adiciona cada linha (DataRow) em uma lista
            var dtItem = new List<DataRow>();
            foreach (DataRow dr in dt.Rows)
            {
                dtItem.Add(dr);
            }

            // Cria a lista do DropDown com um valor padrão
            List<SelectListItem> listaEstados = new List<SelectListItem>
            {
                new SelectListItem { Text = "Selecione...", Value = "0" }
            };

            // Pega cada item da lista extraída do DataTable e adiciona na lista do DropDown
            int count = 0;
            foreach (DataRow dr in dtItem)
            {
                listaEstados.Add(new SelectListItem { Text = dtItem[count].ItemArray[1].ToString(), Value = dtItem[count].ItemArray[0].ToString() });
                count++;
            }

            return listaEstados;
        }
    }
}
