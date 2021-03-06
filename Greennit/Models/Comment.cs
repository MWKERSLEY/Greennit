//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Greennit.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Comment
    {
        public Comment()
        {
            this.UpVotes = 0;
            this.DownVotes = 0;
        }
        //[Required]
        [Display(Name = "Comment ID")]
        public int ID { get; set; }
        [Display(Name = "Comment ArticleID")]
        public int ArticleID { get; set; }
        [Display(Name = "Comment Replies To")]
        public Nullable<int> RepliesTo { get; set; }
        [Display(Name = "Comment Author")]
        [StringLength(50, MinimumLength = 2)]
        [RegularExpression(@"^[^\<\>]+$", ErrorMessage = "Please do not use tag symbols! ('<' and '>')")]
        public string Author { get; set; }
        [Display(Name = "Comment Text")]
        [Required(ErrorMessage = "You must enter a comment!")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Your comment must be between 1 and 500 characters long!")]
        [RegularExpression(@"^[^\<\>]+$", ErrorMessage = "Please do not use tag symbols! ('<' and '>')")]
        public string Text { get; set; }
        [Display(Name = "Comment Up Votes")]
        public int UpVotes { get; set; }
        [Display(Name = "Comment Down Votes")]
        public int DownVotes { get; set; }

        public virtual Article Article { get; set; }
    }
}
