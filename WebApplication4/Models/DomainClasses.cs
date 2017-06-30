using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class ToDo
    {

        public int Id { get; set; }

        public string Description { get; set; }

        public bool IsDone { get; set; }

        public virtual ApplicationUser ToDoUser { get; set; }

    }

    public class Post
    {
        public Post()
        {
            Comments = new List<Post>();
        }

        public int Id { get; set; }

        public DateTime Date { get; set; }

        public bool BlogPost { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public virtual ApplicationUser PostUser { get; set; }

        public List<Post> Comments { get; set; }

        public virtual Post Parent { get; set; }
    }
}