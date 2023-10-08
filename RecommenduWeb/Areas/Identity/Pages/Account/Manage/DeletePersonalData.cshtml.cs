// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecommenduWeb.Models;
using RecommenduWeb.Services;

namespace RecommenduWeb.Areas.Identity.Pages.Account.Manage
{
    public class DeletePersonalDataModel : PageModel
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly ILogger<DeletePersonalDataModel> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly PostService _postService;

        public DeletePersonalDataModel(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            ILogger<DeletePersonalDataModel> logger,
            IWebHostEnvironment environment,
            PostService postService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _environment = environment;
            _postService = postService;
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
            [Required(ErrorMessage = "Campo Senha é obrigatório")]
            [DisplayName("Senha")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool RequirePassword { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"ID não encontrado '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"ID não encontrado '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            if (RequirePassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, Input.Password))
                {
                    ModelState.AddModelError(string.Empty, "Senha incorreta.");
                    return Page();
                }
            }

            var produtos = await _postService.BuscarProdutoPorUsuarioAsync(user.Id);
            var servicos = await _postService.BuscarServicoPorUsuarioAsync(user.Id);

            // deletar todas publicações de produtos
            if (produtos.Count > 0)
            {
                foreach (var prod in produtos)
                {
                    await _postService.DeletarPostagemAsync(prod.Categoria, _environment.WebRootPath, prod, null);
                }
            }

            // deletar todas publicações de serviços
            if (servicos.Count > 0)
            {
                foreach (var serv in servicos)
                {
                    await _postService.DeletarPostagemAsync(serv.Categoria, _environment.WebRootPath, null, serv);
                }
            }

            var result = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Erro inesperado.");
            }

            await _signInManager.SignOutAsync();

            _logger.LogInformation("Usuario ID: '{UserId}' deletou a conta.", userId);

            return Redirect("~/");
        }
    }
}
