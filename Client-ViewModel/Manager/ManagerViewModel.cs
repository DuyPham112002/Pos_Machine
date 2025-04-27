using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ViewModel.Manager
{
    public class ManagerViewModel 
    {
        public string AccId { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Creator { get; set; }
        public string Bio { get; set; }
        public bool IsActive { get; set; }
        public string ImageSetId { get; set; }
        public List<string>? Images { get; set; }
    }
}
