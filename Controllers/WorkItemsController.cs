using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vue2SpaSignalR.Models;

namespace Vue2SpaSignalR.Controllers
{
    public class WorkItemsController : Controller
    {
        private readonly Vue2SpaContext _context;

        public WorkItemsController(Vue2SpaContext context)
        {
            _context = context;
        }

        // GET: WorkItems
        public async Task<IActionResult> Index()
        {
            var workItems = await GetWorkItems();

            return View(workItems);
        }

        // GET: WorkItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workItem = await GetWorkItem(id);

            if (workItem == null)
            {
                return NotFound();
            }

            return View(workItem);
        }

        // GET: WorkItems/Create
        public async Task<IActionResult> Create()
        {
            var employees = await GetEmployees();
            ViewBag.Employees = new SelectList(employees, "Id", "Name");

            return View();
        }

        // POST: WorkItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployeeId,TaskName,Description")] WorkItem workItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var employees = await GetEmployees();
            ViewBag.Employees = new SelectList(employees, "Id", "Name");

            return View(workItem);
        }

        // GET: WorkItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workItem = await _context.WorkItem.SingleOrDefaultAsync(m => m.Id == id);

            if (workItem == null)
            {
                return NotFound();
            }

            var employees = await GetEmployees();
            ViewBag.Employees = new SelectList(employees, "Id", "Name");

            return View(workItem);
        }

        // POST: WorkItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeId,TaskName,Description")] WorkItem workItem)
        {
            if (id != workItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkItemExists(workItem.Id))
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

            var employees = await GetEmployees();
            ViewBag.Employees = new SelectList(employees, "Id", "Name");

            return View(workItem);
        }

        // GET: WorkItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workItem = await GetWorkItem(id);

            if (workItem == null)
            {
                return NotFound();
            }

            return View(workItem);
        }

        // POST: WorkItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workItem = await _context.WorkItem.SingleOrDefaultAsync(m => m.Id == id);
            _context.WorkItem.Remove(workItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Validate Task Name
        [AcceptVerbs("Get", "Post")]
        public IActionResult ValidateTaskName(string taskName)
        {
            Regex rgx = new Regex("[^a-z0-9]");
            taskName = rgx.Replace(taskName.ToLower(), string.Empty);

            string reversed = new string(taskName.ToCharArray().Reverse().ToArray());

            if (taskName != reversed)
            {
                return Json("All task names must be palindromes");
            }

            return Json(true);
        }

        private bool WorkItemExists(int id)
        {
            return _context.WorkItem.Any(e => e.Id == id);
        }

        private async Task<List<WorkItem>> GetWorkItems()
        {
            return await _context.WorkItem.Include("Employee").ToListAsync();
        }

        private async Task<List<Employee>> GetEmployees()
        {
            return await _context.Employee.OrderBy(x => x.Name).ToListAsync();
        }

        private async Task<WorkItem> GetWorkItem(int? id)
        {
            return await _context.WorkItem.Include("Employee").Where(x => x.Id == id).SingleOrDefaultAsync();
        }
    }
}
