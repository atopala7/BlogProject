using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WebApplication4.Models;
using System.Threading.Tasks;
using System.Net;
using System.Data.Entity;
using WebApplication4.ViewModels;

namespace WebApplication4.Controllers
{
    [Authorize]
    public class BlogsController : Controller
    {
        // Manager for posts; also contains methods for getting a user by the UserName or the current user
        BlogManager man = new BlogManager();

        // GET: Blogs
        // If the user is logged in as a blogger, lists all of his/her posts with options for editing and deleting
        public ActionResult Index()
        {
            var identity = User as ClaimsPrincipal;

            var currentUser = man.getCurrentUser();

            var userPosts = man.getAllPosts(currentUser.UserName).OrderByDescending(p => p.Date);

            // If the user is a blogger, show the blogger page; otherwise, go to the Index page
            if (identity.HasClaim("http://http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "blogger"))
            {
                return View(userPosts);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Blogs/Display?uname=Achilles
        // Lists all the blog posts of a given user
        [AllowAnonymous]
        public ActionResult Display(string uname)
        {
            var user = man.getUser(uname);

            if (user == null)
            {
                return HttpNotFound();
            }
            else
            {
                var blogPosts = man.getBlogPosts(uname).OrderByDescending(p => p.Date);
                ViewBag.UserName = user.UserName;

                return View(blogPosts);
            }
        }

        // GET: Blogs/Publish
        // Returns a view for a form to add a new blog post
        public ActionResult Publish()
        {
            return View(man.getPostAddForm());
        }

        // POST: Blogs/Publish
        // Adds a new post to the current user
        [HttpPost]
        public ActionResult Publish(PostAdd newPost)
        {
            if (ModelState.IsValid)
            {
                var addedPost = man.addPost(newPost);
                if (addedPost == null)
                {
                    return View("Error");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View("Error");
            }
        }

        // GET: Blogs/Edit/5
        // Returns a form to edit the post with the given id
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PostFull post = man.getPostFull(id);
            var currentUser = man.getCurrentUser();

            if (post == null)
            {
                return HttpNotFound();
            }
            // Only a post's owner can edit that post
            else if (post.PostUserId != currentUser.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            return View(post);
        }

        // POST: Blogs/Edit
        // Modifies the given post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id, Date, BlogPost, Title, Message")] Post post)
        {
            if (ModelState.IsValid)
            {
                await(man.modifyPost(post));
                return RedirectToAction("Index");
            }

            // If the ModelState is invalid, display the form again
            return View(post);
        }

        // GET: Blogs/PostComment/5
        // Returns a form to add a comment to a given post
        public ActionResult PostComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var identity = User as ClaimsPrincipal;

            if (identity.HasClaim("http://http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "commenter"))
            {
                Post parent = man.getPost(id);

                if (parent == null)
                {
                    return HttpNotFound();
                }

                // Pass the parent's id as parameter
                CommentAddForm comment = man.getCommentAddForm((int)id);

                // The view will say "Comment on USERNAME's post"
                ViewBag.UserName = parent.PostUser.UserName;

                return View(comment);
            }
            else
            {
                // If a non-commenter gets here for whatever reason, redirect to Register
                // Will need to rewrite Register method to incorporate ReturnUrl
                return RedirectToAction("Register", "Account", new { ReturnUrl = "/Blogs/PostComment/"+id });
            }
        }

        // POST: Blogs/PostComment
        // Adds the comment to the parent post
        [HttpPost]
        public ActionResult PostComment(CommentAdd newComment)
        {
            if (ModelState.IsValid)
            {
                var addedComment = man.addComment(newComment);
                if (addedComment == null)
                {
                    return View("Error");
                }
                else
                {
                    // Go the the view that lists the comments of the post that was just commented on
                    return RedirectToAction("ViewComments", new { id = newComment.ParentId });
                }
            }
            else
            {
                return View("Error");
            }
        }

        //GET: Blogs/Details/5
        // Displays the title and message of a given post
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Post post = man.getPost(id);

            if (post == null)
            {
                return HttpNotFound();
            }

            var identity = User as ClaimsPrincipal;

            var currentUser = man.getCurrentUser();

            // If the current user is the post's owner or an admin, give him/her the option to delete the post in the view
            ViewBag.delete = false;
            if (currentUser == post.PostUser || identity.HasClaim("http://http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "administrator"))
            {
                ViewBag.delete = true;
            }

            // Pass the owner's username, to be used for display purposes and to return to a list of that user's posts
            ViewBag.UserName = post.PostUser.UserName;

            // If it's a blog post, pass its title to the view; if it's a comment, pass its parent's id
            // The parent id will be used for display purposes and to link to a view of all its comments
            if (post.BlogPost)
            {
                ViewBag.PostTitle = post.Title;
            }
            else
            {
                ViewBag.ParentId = post.Parent.Id;
                ViewBag.PostTitle = "comment";
            }

            return View(post);
        }

        // GET: Blogs/Delete/5
        // Asks for confirmation to delete the given post
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Post post = man.getPost(id);
            var currentUser = man.getCurrentUser();
            var identity = User as ClaimsPrincipal;

            if (post == null)
            {
                return HttpNotFound();
            }
            // A post can only be deleted by its owner or by an admin
            else if (post.PostUser != currentUser && !(identity.HasClaim("http://http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "administrator")))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            return View(post);
        }

        // POST: Blogs/Delete
        // Deletes the post and all its comments
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await (man.deletePost(id));

            return RedirectToAction("Index", "Home");
        }

        // GET: Blogs/ViewComments/5
        // Lists all the comments on a given post
        [AllowAnonymous]
        public ActionResult ViewComments(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Post post = man.getPost(id);

            if (post == null)
            {
                return HttpNotFound();
            }

            // If the post has a title, pass it to the view; otherwise, just call it "comment"
            if (post.Title != null && post.Title != "--empty--")
            {
                ViewBag.PostTitle = post.Title;
            }
            else
            {
                ViewBag.PostTitle = "comment";
            }

            ViewBag.UserName = post.PostUser.UserName;
            // PostId will be used to link back to the Details view of the given post
            ViewBag.PostId = post.Id;

            var comments = man.getComments(post.Id);

            return View(comments.OrderByDescending(p => p.Date));
        }
    }
}