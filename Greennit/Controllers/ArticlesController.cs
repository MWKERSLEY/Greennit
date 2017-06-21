using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Greennit.Models;

namespace Greennit.Controllers
{
    public class ArticlesController : Controller
    {
        private GreennitEntities2 db = new GreennitEntities2();

        // GET: Articles
        //public ActionResult Index()
        //{
        //    return View(db.Articles.ToList());
        //}

        public ActionResult Index(string articleType, string searchString, string user)
        {
            //type dropdown
            var TypeList = new List<string>();
            var TypeQuery = from t in db.ContentTypes
                           orderby t.ContentTypeString
                           select t.ContentTypeString;
            TypeList.AddRange(TypeQuery.Distinct());
            ViewBag.articleType = new SelectList(TypeList);

            //title search
            var articles = from s in db.Articles
                         select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                articles = articles.Where(s => s.Title.Contains(searchString));
            }

            //type search
            if (!String.IsNullOrEmpty(articleType))
            {
                int articleID = 0;
                foreach (var type in db.ContentTypes.ToList())
                {
                    if (type.ContentTypeString == articleType)
                    {
                        articleID = type.ID;
                    }
                }
                articles = articles.Where(x => x.ContentTypeID == articleID);
            }

            //sort and return
            return View(articles.ToList().OrderByDescending(o=>o.UpVotes).ToList().OrderByDescending(o=>(o.UpVotes - o.DownVotes)).ToList());
        }
        // GET: Articles/Details/5
        public ActionResult Details(int? id, string user, string deleteMessage)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            if (deleteMessage == "failed")
            {
                ViewBag.DeleteMessage = "This isn't yours to delete!";
            }
            var commentsList = (from t in db.Comments
                            where t.ArticleID == article.ID
                            orderby t.ArticleID
                            select t).ToList().OrderBy(o => o.ID).ToList();

            var articleAndComments = new ArticleAndComments { CommentsList = commentsList, Article = article, Comment = new Comment()};

            return View(articleAndComments);
        }

        // GET: Articles/Create
        public ActionResult Create(string user)
        {
            //var p = new Article();
            //p.ContentType = db.C.ToList();
            //return View(p);
            ViewBag.articleTypeCreate = db.ContentTypes.ToList();
            //return View();

            Article article = new Article();
            article.Author = user;

            return View(article);
        }

        // POST: Articles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Article article, string user)
        {
            if (ModelState.IsValid)
            {
                db.Articles.Add(article);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.articleTypeCreate = db.ContentTypes.ToList();
            return View(article);
        }

        // GET: Articles/Edit/5
        public ActionResult Edit(int? id, string user)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            ViewBag.articleTypeCreate = db.ContentTypes.ToList();
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ContentTypeID,Title,Description,Author,Image,Website,Video,Text,UpVotes,DownVotes")] Article article, string user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(article).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = article.ID, user=user });
            }
            ViewBag.articleTypeCreate = db.ContentTypes.ToList();
            return View(article);
        }

        // GET: Articles/Delete/5
        public ActionResult Delete(int? id, string user)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            //if allowed to delete
            if ((user == article.Author) || string.IsNullOrWhiteSpace(article.Author) || article.Author == "Anonymous")
            {
                ArticleAndComments articleAndComments = new ArticleAndComments();
                articleAndComments.Article = article;
                return View(articleAndComments);
            }
            //if not allowed to delete
            return RedirectToAction("Details", new { id = article.ID, user = user, deleteMessage = "failed" });
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string user)
        {
            //get and delete all related comments
            var commentList = (from c in db.Comments
                            where c.ArticleID == id
                            select c).ToList();
            foreach (var c in commentList)
            {
                db.Comments.Remove(c);
            }

            //delete article
            Article article = db.Articles.Find(id);
            db.Articles.Remove(article);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult UpMain(int id, string user)
        {
            Article Article = db.Articles.Find(id);

            int currentUps = Convert.ToInt32(Article.UpVotes);
            Article.UpVotes = currentUps + 1;

            if (ModelState.IsValid)
            {
                db.Entry(Article).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index", new { });
        }
        public ActionResult DownMain(int id, string user)
        {
            Article Article = db.Articles.Find(id);

            int currentDowns = Convert.ToInt32(Article.DownVotes);
            Article.DownVotes = currentDowns + 1;

            if (ModelState.IsValid)
            {
                db.Entry(Article).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index", new { });
        }
        public ActionResult UpDetails(int id, string user)
        {
            Article Article = db.Articles.Find(id);

            int currentUps = Convert.ToInt32(Article.UpVotes);
            Article.UpVotes = currentUps + 1;

            if (ModelState.IsValid)
            {
                db.Entry(Article).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Details", new { ID=Article.ID });
        }
        public ActionResult DownDetails(int id, string user)
        {
            Article Article = db.Articles.Find(id);

            int currentDowns = Convert.ToInt32(Article.DownVotes);
            Article.DownVotes = currentDowns + 1;

            if (ModelState.IsValid)
            {
                db.Entry(Article).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Details", new { ID = Article.ID });
        }


        public ActionResult About(string user)
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact(string user)
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult LogIn(string user)
        {
            ViewBag.Message = "What's Your Name?";
            return View();
        }
        
        [HttpPost, ActionName("LogIn")]
        public ActionResult LogInConfirmed(string user)
        {
            return RedirectToAction("Index", new { user = user });
        }



        // GET: Articles/Create
        //public ActionResult _NewComment(int repliesTo, int articleID, string user)
        //{
        //    //var p = new Article();
        //    //p.ContentType = db.C.ToList();
        //    //return View(p);
        //    //return View();

        //    ArticleAndComments articleAndComments = new ArticleAndComments();
        //    articleAndComments.Comment.Author = user;
        //    articleAndComments.Comment.UpVotes = 0;
        //    articleAndComments.Comment.DownVotes = 0;
        //    articleAndComments.Comment.RepliesTo = repliesTo;
        //    articleAndComments.Comment.ArticleID = articleID;

        //    return View(articleAndComments.Comment);
        //}

        // POST: Articles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(ArticleAndComments articleAndComments, string user)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(articleAndComments.Comment.Author))
                {
                    articleAndComments.Comment.Author = "Anonymous";
                }
                db.Comments.Add(articleAndComments.Comment);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = articleAndComments.Comment.ArticleID, user = user, deleteMessage = "" });
            }

            return RedirectToAction("Details", new { id = articleAndComments.Comment.ArticleID, user = user, deleteMessage = "" });
            //return View(articleAndComments);
        }

        public ActionResult UpComment(int id, string user)
        {
            Comment comment = db.Comments.Find(id);

            int currentUps = Convert.ToInt32(comment.UpVotes);
            comment.UpVotes = currentUps + 1;
            //comment.UpVotes++;

            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Details", new { id = comment.ArticleID });
        }
        public ActionResult DownComment(int id, string user)
        {
            Comment comment = db.Comments.Find(id);

            int currentDowns = Convert.ToInt32(comment.DownVotes);
            comment.DownVotes = currentDowns + 1;
            //comment.DownVotes++;

            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Details", new { id = comment.ArticleID });
        }

        //public ActionResult _Comments(ArticleAndComments articleAndComments, string user, int repliesTo, int articleID)
        //{
        //    //var p = new Article();
        //    //p.ContentType = db.C.ToList();
        //    //return View(p);
        //    //return View();

        //    return View(articleAndComments);
        //}

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
