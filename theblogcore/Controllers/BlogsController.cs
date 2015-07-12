using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using theblogcore.Models;
using Microsoft.AspNet.Identity;

namespace theblogcore.Controllers
{
    [Authorize]
    public class BlogsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Blogs
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(db.Blogs.OrderBy(b=>b.DateTime).ToList());
        }

        // GET: Blogs/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }

            BlogViewModel blogViewModel = new BlogViewModel();
            blogViewModel.Blog = blog;
            blogViewModel.Comment = new Comment();
            
            //return View(blog);
            return View(blogViewModel);
        }

        // POST: Blogs/Details/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(Guid? id, [Bind(Include = "CommentId,Content,BlogId")] Comment comment)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                comment.CommentId = Guid.NewGuid();
                comment.BlogId = blog.BlogId;
                comment.UserId = User.Identity.GetUserId();
                comment.DateTime = DateTime.Now;
                comment.IsEdited = false;
                comment.EditedTime = DateTime.Now;

                db.Comments.Add(comment);
                db.SaveChanges();
            }

            return RedirectToAction("Details", new { id = id });
        }

        // GET: Blogs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BlogId,Title,Content")] Blog blog)
        {
            if (ModelState.IsValid)
            {
                blog.BlogId = Guid.NewGuid();

                blog.UserId = User.Identity.GetUserId();
                blog.DateTime = DateTime.Now;
                blog.IsEdited = false;
                blog.EditedTime = blog.DateTime;

                db.Blogs.Add(blog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blog);
        }

        // GET: Blogs/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }

            if (!blog.Author.Id.Equals(User.Identity.GetUserId()))
            {
                return RedirectToAction("Index");
            }

            return View(blog);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BlogId,Title,Content")] Blog blog)
        {
            if (ModelState.IsValid)
            {
                var blogFromDb = db.Blogs.FirstOrDefault(b => b.BlogId == blog.BlogId);

                blogFromDb.Title = blog.Title;
                blogFromDb.Content = blog.Content;
                blogFromDb.IsEdited = true;
                blogFromDb.EditedTime = DateTime.Now;

                //db.Entry(blog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blog);
        }

        // GET: Blogs/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }

            if (!blog.Author.Id.Equals(User.Identity.GetUserId()))
            {
                return RedirectToAction("Index");
            }

            return View(blog);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Blog blog = db.Blogs.Find(id);
            db.Blogs.Remove(blog);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
