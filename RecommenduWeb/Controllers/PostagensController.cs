using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecommenduWeb.Data;
using RecommenduWeb.Models;
using RecommenduWeb.Models.ViewModels;
using RecommenduWeb.Services;

namespace RecommenduWeb.Controllers
{
    [Authorize]
    public class PostagensController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PostService _postService;
        private readonly UserManager<Usuario> _userManager;
        private readonly IWebHostEnvironment _environment;

        public PostagensController(ApplicationDbContext context, PostService postagemService, UserManager<Usuario> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _postService = postagemService;
            _userManager = userManager;
            _environment = environment;
        }

        // GET: Postagens
        public async Task<IActionResult> Index()
        {
            //var user = await _userManager.GetUserAsync(User);
            var listaPost = await _postService.TodasPostagensAsync();

            return _context.Postagem != null ?
                        View(listaPost) :
                        Problem("Entity set 'ApplicationDbContext.Postagem'  is null.");
        }

        // GET: Postagens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Postagem == null)
            {
                return NotFound();
            }

            var postagem = await _context.Postagem
                .FirstOrDefaultAsync(m => m.PostagemId == id);
            if (postagem == null)
            {
                return NotFound();
            }

            return View(postagem);
        }

        // GET: Postagens/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Postagens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Categoria,Descricao,PublicoAlvo,ImgPostagem,Modelo,Fabricante,LinkProduto,TempoUso,NomeServico,Endereco,Contato")] PostagemViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var webRoot = _environment.WebRootPath + @"\Resources\PostImages";
                await _postService.PublicarAsync(vm, user, webRoot);

                return RedirectToAction(nameof(Index));
            }
            return View(vm);
        }

        // GET: Postagens/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Postagem == null)
            {
                return NotFound();
            }

            var postagem = await _context.Postagem.FindAsync(id);
            if (postagem == null)
            {
                return NotFound();
            }
            return View(postagem);
        }

        // POST: Postagens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostagemId,Categoria,Descricao,PublicoAlvo,DtPostagem,Curtidas")] Postagem postagem)
        {
            if (id != postagem.PostagemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(postagem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostagemExists(postagem.PostagemId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(postagem);
        }

        // GET: Postagens/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Postagem == null)
            {
                return NotFound();
            }

            var postagem = await _context.Postagem
                .FirstOrDefaultAsync(m => m.PostagemId == id);
            if (postagem == null)
            {
                return NotFound();
            }

            return View(postagem);
        }

        // POST: Postagens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Postagem == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Postagem'  is null.");
            }
            var postagem = await _context.Postagem.FindAsync(id);
            if (postagem != null)
            {
                _context.Postagem.Remove(postagem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Curtir(int id)
        {
            var post = await _postService.PostagemPorPostagemIdAsync(id);
            if (post != null)
            {
                await _postService.AtualizarCurtidasAsync(post.PostagemId, 1);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PostagemExists(int id)
        {
            return (_context.Postagem?.Any(e => e.PostagemId == id)).GetValueOrDefault();
        }
    }
}
