// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using RecommenduWeb.Models;
using RecommenduWeb.Services;

namespace RecommenduWeb.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly EnvioEmailService _envioEmailService;

        public ResendEmailConfirmationModel(UserManager<Usuario> userManager, EnvioEmailService envioEmailService)
        {
            _userManager = userManager;
            _envioEmailService = envioEmailService;
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
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email informado não foi encontrado.");
                return Page();
            }

            if (user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Esta conta já foi confirmada.");
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);
            var mensagem = $"<h1>Seja bem-vindo!</h1><p>Caro usuário, <br><br><br> para finalizar sua inscrição você deve ativar sua conta.</p><a id=\"confirm-link\" href=\"{callbackUrl}\">Clique aqui para confirmar sua conta.</a>";
            var envio = _envioEmailService.EnvioEmail(Input.Email, "Confirmação de registro", mensagem);
            if (envio == false)
            {
                return NotFound("Problemas ao tentar enviar email.");
            }

            ModelState.AddModelError(string.Empty, "Confirmação de email enviada. Favor verifique sua caixa de entrada ou lixo eletrônico.");
            return Page();
        }
    }
}
