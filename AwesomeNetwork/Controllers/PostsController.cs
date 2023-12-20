﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AwesomeNetwork.Data;
using AwesomeNetwork.Models.Users;

namespace AwesomeNetwork.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> AllPosts(string? search,DateTime? from,DateTime? to, string? PostType="Всі")
        {
           if (PostType.Equals("Всі"))
            {
                if (search != null)
                    return View(await _context.Posts.
                        Where(p => (p.Title.Contains(search) || p.Description.Contains(search)) &&
                        ((p.CreationDate >= from) && (p.CreationDate <= to))).
                        Include("User").
                        Include("Comments").
                        Include("User").
                        OrderByDescending(p => p.CreationDate).ToListAsync());
                else
                    return View(await _context.Posts.
                    Where(p => (p.CreationDate >= from && p.CreationDate <= to)).
                    Include("User").
                    Include("Comments").
                    Include("User").
                    OrderByDescending(p => p.CreationDate).ToListAsync());
            }
            //    PostType = " ";
            ////PostType = "";
                if (search != null)
                    return View(await _context.Posts.
                        Where(p => (p.Title.Contains(search) || p.Description.Contains(search)) &&
                        ((p.CreationDate >= from) && (p.CreationDate <= to)) &&
                        p.PostType.Contains(PostType)).
                        Include("User").
                        Include("Comments").
                        Include("User").
                        OrderByDescending(p => p.CreationDate).ToListAsync());
                else
                    return View(await _context.Posts.
                    Where(p => (p.CreationDate >= from && p.CreationDate <= to) &&
                    p.PostType.Contains(PostType)).
                    Include("User").
                    Include("Comments").
                    Include("User").
                    OrderByDescending(p => p.CreationDate).ToListAsync());
            
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
              return View(await _context.Posts.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.
                Include(f=>f.User).
                Include(p=>p.Comments).
                ThenInclude(c=>c.User).
                FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,CreationDate")] Post post)
        {
            if (ModelState.IsValid)
            {
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,CreationDate")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
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
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Posts'  is null.");
            }
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
          return _context.Posts.Any(e => e.Id == id);
        }
    }
}
