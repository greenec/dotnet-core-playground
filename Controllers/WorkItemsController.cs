using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vue2SpaSignalR.Models;

namespace Vue2Spa.Controllers
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
            var workItems = await GetDetailedWorkItems();

            return View(workItems);
        }

        // GET: WorkItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workItem = await GetWorkItemDetailed(id);

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
            ViewBag.Employees = new SelectList(employees, "ID", "Name");

            return View();
        }

        // POST: WorkItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,UserID,TaskName,Description")] WorkItem workItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workItem);
        }

        // GET: WorkItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workItem = await _context.WorkItem.SingleOrDefaultAsync(m => m.ID == id);

            if (workItem == null)
            {
                return NotFound();
            }

            var employees = await GetEmployees();
            ViewBag.Employees = new SelectList(employees, "ID", "Name");

            return View(workItem);
        }

        // POST: WorkItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,UserID,TaskName,Description")] WorkItem workItem)
        {
            if (id != workItem.ID)
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
                    if (!WorkItemExists(workItem.ID))
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
            return View(workItem);
        }

        // GET: WorkItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workItem = await GetWorkItemDetailed(id);

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
            var workItem = await _context.WorkItem.SingleOrDefaultAsync(m => m.ID == id);
            _context.WorkItem.Remove(workItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkItemExists(int id)
        {
            return _context.WorkItem.Any(e => e.ID == id);
        }

        private async Task<List<WorkItemDetailed>> GetDetailedWorkItems()
        {
            var workItemsQuery = from i in _context.WorkItem
                                join e in _context.Employee on i.UserID equals e.ID
                                select new WorkItemDetailed
                                {
                                    ID = i.ID,
                                    TaskName = i.TaskName,
                                    Description = i.Description,
                                    UserID = i.UserID,
                                    EmployeeName = e.Name
                                };

            return await workItemsQuery.ToListAsync();
        }

        private async Task<List<Employee>> GetEmployees()
        {
            IQueryable<Employee> employeeQuery = from e in _context.Employee
                                                 orderby e.Name
                                                 select e;

            return await employeeQuery.ToListAsync();
        }

        private async Task<WorkItemDetailed> GetWorkItemDetailed(int? id)
        {
            var workItemQuery = from i in _context.WorkItem
                                join e in _context.Employee on i.UserID equals e.ID
                                where i.ID == id
                                select new WorkItemDetailed
                                {
                                    ID = i.ID,
                                    TaskName = i.TaskName,
                                    Description = i.Description,
                                    UserID = i.UserID,
                                    EmployeeName = e.Name
                                };

            return await workItemQuery.SingleOrDefaultAsync();
        }
    }
}
