﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RolesForAssessment.Data;
using dsd03Razor2020Assessment.Models;

namespace RolesForAssessment.Pages.Casts
{
    public class DeleteModel : PageModel
    {
        private readonly RolesForAssessment.Data.ApplicationDbContext _context;

        public DeleteModel(RolesForAssessment.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Cast Cast { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || _context.Cast == null)
            {
                return NotFound();
            }

            var cast = await _context.Cast.FirstOrDefaultAsync(m => m.Id == id);

            if (cast == null)
            {
                return NotFound();
            }
            else 
            {
                Cast = cast;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null || _context.Cast == null)
            {
                return NotFound();
            }
            var cast = await _context.Cast.FindAsync(id);

            if (cast != null)
            {
                Cast = cast;
                _context.Cast.Remove(Cast);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
