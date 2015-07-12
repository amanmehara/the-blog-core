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
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        public ActionResult Index()
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Blogs");
            }
            
            var comments = db.Comments.Include(c => c.Blog).OrderBy(c => c.DateTime);
            return View(comments.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Blogs");
            }

            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create
        public ActionResult Create()
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Blogs");
            }

            ViewBag.BlogID = new SelectList(db.Blogs, "BlogId", "Title");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CommentId,Content,BlogId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.CommentId = Guid.NewGuid();

                comment.UserId = User.Identity.GetUserId();
                comment.DateTime = DateTime.Now;
                comment.IsEdited = false;
                comment.EditedTime = comment.DateTime;

                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BlogID = new SelectList(db.Blogs, "BlogId", "Title", comment.BlogId);
            return View(comment);
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }

            if (!comment.Commentator.Id.Equals(User.Identity.GetUserId()))
            {
                return RedirectToAction("Index", "Blogs");
            }

            ViewBag.BlogID = new SelectList(db.Blogs, "BlogId", "Title", comment.BlogId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CommentId,Content")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                var commentFromDb = db.Comments.FirstOrDefault(c => c.CommentId == comment.CommentId);
                commentFromDb.Content = comment.Content;
                commentFromDb.IsEdited = true;
                commentFromDb.EditedTime = DateTime.Now;
                
                //db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                //return RedirectToAction("Index");

                return RedirectToAction("Details", "Blogs", new { id = commentFromDb.BlogId });
            }
            ViewBag.BlogID = new SelectList(db.Blogs, "BlogId", "Title", comment.BlogId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }

            if (!comment.Commentator.Id.Equals(User.Identity.GetUserId()))
            {
                return RedirectToAction("Index", "Blogs");
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
            db.SaveChanges();
            //return RedirectToAction("Index");

            return RedirectToAction("Details", "Blogs", new { id = comment.BlogId });
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
