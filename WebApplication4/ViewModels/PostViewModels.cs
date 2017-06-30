using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.ViewModels
{
    public class PostList
    {
        [Key]
        public int PostId;
        public string PostTitle;
    }

    public class PostAddForm
    {
        [Key]
        [HiddenInput]
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }

    public class PostAdd
    {
        [Key]
        public int PostId { get; set; }
        public DateTime Date { get; set; }
        public bool BlogPost { get; set; }
        public int ParentId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }


    public class CommentAddForm
    {
        [Key]
        [HiddenInput]
        public int PostId { get; set; }
        [HiddenInput]
        public int ParentId { get; set; }
        [HiddenInput]
        public string Title { get; set; }
        public string Message { get; set; }
    }

    public class CommentAdd
    {
        [Key]
        public int PostId { get; set; }
        public DateTime Date { get; set; }
        public bool BlogPost { get; set; }
        public int ParentId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }

    public class PostFull
    {
        public PostFull()
        {
            ChildrenId = new List<int>();
        }

        [HiddenInput]
        public int Id { get; set; }
        [HiddenInput]
        public DateTime Date { get; set; }
        [HiddenInput]
        public bool BlogPost { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }
        [HiddenInput]
        public string PostUserId { get; set; }
        [HiddenInput]
        public List<int> ChildrenId { get; set; }
        [HiddenInput]
        public int ParentId { get; set; }
    }
}