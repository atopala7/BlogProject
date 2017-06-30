using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNet.Identity;
using WebApplication4.Models;
using System.Data.Entity;
using System.Threading.Tasks;
using WebApplication4.ViewModels;
using AutoMapper;

namespace WebApplication4.Models
{
    public class BlogManager : VmManagerBase
    {
        // Returns the current user, including his/her posts
        public ApplicationUser getCurrentUser()
        {
            var id = HttpContext.Current.User.Identity.GetUserId();
            var currentUser = dc.Users.Include("Posts").FirstOrDefault(u => u.Id == id);

            return currentUser;
        }

        // Returns the given user, including his her posts
        public ApplicationUser getUser(string username)
        {
            var user = dc.Users.Include("Posts").FirstOrDefault(u => u.UserName == username);

            return user;
        }

        // Returns all the users who are bloggers
        public IEnumerable<ApplicationUser> getBloggers()
        {
            var bloggers = dc.Users.ToList().Where(u => u.Blogger);

            return bloggers;
        }

        // Returns the given post, including comments, its parent, and its owner
        public Post getPost(int? id)
        {
            Post post = dc.Posts.Include("Comments").Include("Parent").Include("PostUser").FirstOrDefault(p => p.Id == id);

            return post;
        }

        // Returns a list containing all the given user's posts
        public List<Post> getAllPosts(string username)
        {
            var user = dc.Users.Include("Posts").FirstOrDefault(u => u.UserName == username);

            List<Post> posts = new List<Post>();

            foreach (var item in user.Posts)
            {
                // Since the post may be a comment, include the parent and the owner of the parent
                posts.Add(dc.Posts.Include("Parent").Include("Parent.PostUser").FirstOrDefault(p => p.Id == item.Id));
            }

            return posts;
        }

        // Returns a list containing the given user's blog posts only
        public List<Post> getBlogPosts(string username)
        {
            var user = dc.Users.Include("Posts").FirstOrDefault(u => u.UserName == username);

            List<Post> posts = new List<Post>();

            foreach (var item in user.Posts)
            {
                if (item.BlogPost)
                {
                    posts.Add(dc.Posts.FirstOrDefault(p => p.Id == item.Id));
                }
            }

            return posts;
        }

        // Returns a list containing the comments of a given post
        public List<Post> getComments(int id)
        {
            Post post = dc.Posts.Include("Comments").FirstOrDefault(p => p.Id == id);

            List<Post> comments = new List<Post>();

            foreach (var item in post.Comments)
            {
                // Populate the list of comments and the UserName of each's owner so that it may be displayed
                comments.Add(dc.Posts.Include("PostUser").FirstOrDefault(c => c.Id == item.Id));
            }

            return comments;
        }

        public PostAddForm getPostAddForm()
        {
            var postForm = new PostAddForm();
            return postForm;
        }

        public CommentAddForm getCommentAddForm(int id)
        {
            var commentForm = new CommentAddForm();
            commentForm.ParentId = id;
            return commentForm;
        }

        // Adds the given post to the database
        public async Task<PostFull> addPost(PostAdd pst)
        {
            pst.BlogPost = true;
            pst.Date = DateTime.Now;
            Post post = Mapper.Map<Post>(pst);

            var currentUser = getCurrentUser();

            //post.BlogPost = true;
            //post.Date = DateTime.Now;
            // post.Parent = null;
            post.PostUser = currentUser;
            if (post.Message == null)
            {
                post.Message = "--empty--";
            }
            if (post.Title == null)
            {
                post.Title = "--empty--";
            }
            //post.Message = (pst.Message != null ? pst.Message : "--empty--");
            //post.Title = (pst.Title != null ? pst.Title : "--empty--");

            currentUser.Posts.Add(post);

            await dc.SaveChangesAsync();

            return getPostFull(post.Id);
        }

        // Adds the given comment to the database and sets its parent to the appropriate post
        public async Task<PostFull> addComment(CommentAdd cmnt)
        {
            cmnt.BlogPost = false;
            cmnt.Date = DateTime.Now;
            Post comment = Mapper.Map<Post>(cmnt);

            var currentUser = getCurrentUser();

            //comment.BlogPost = false;
            //comment.Date = DateTime.Now;
            comment.Parent = getPost(cmnt.ParentId);
            comment.PostUser = currentUser;
            if (comment.Message == null)
            {
                comment.Message = "--empty--";
            }
            if (comment.Title == null)
            {
                comment.Title = "--empty--";
            }
            //comment.Message = (cmnt.Message != null ? cmnt.Message : "--empty--");
            //comment.Title = (cmnt.Title != null ? cmnt.Title : "--empty--");

            currentUser.Posts.Add(comment);
            await dc.SaveChangesAsync();

            return getPostFull(comment.Id);
        }

        // Returns the full given post
        public PostFull getPostFull(int? id)
        {
            if (id != null)
            {
                // Include all the immediate information of the post
                var post = dc.Posts.Include("Comments").Include("Parent").Include("PostUser").FirstOrDefault(p => p.Id == id);

                if (post == null)
                {
                    return null;
                }

                PostFull pfull = new PostFull();

                pfull.Id = post.Id;
                pfull.BlogPost = post.BlogPost;
                pfull.Date = post.Date;
                pfull.Message = post.Message;
                if (post.Parent != null)
                {
                    pfull.ParentId = post.Parent.Id;
                }
                else
                {
                    pfull.ParentId = -1;
                }
                pfull.PostUserId = post.PostUser.Id;
                pfull.Title = post.Title;

                // Populate the list of children with the ids of the post's comments
                foreach (var item in post.Comments)
                {
                    pfull.ChildrenId.Add(item.Id);
                }

                return pfull;
            }
            else
            {
                return null;
            }
        }

        // Modifies the given post in the database
        public async Task<bool> modifyPost(Post post)
        {
            try
            {
                dc.Entry(post).State = EntityState.Modified;

                await dc.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            
            return true;
        }

        // Deletes the given post and all its comments
        public async Task<bool> deletePost(int id)
        {
            Post post = getPost(id);

            try
            {
                // Create temporary list of children to iterate over; without this, the foreach loop wouldn't work because the list would be changing with each iteration
                var children = post.Comments.ToList();

                // Call this method recursively for each child
                foreach (var child in children)
                {
                    await (deletePost(child.Id));
                }

                dc.Posts.Remove(post);
                await dc.SaveChangesAsync();
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}