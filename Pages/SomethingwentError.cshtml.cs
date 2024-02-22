using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dajeej.Pages
{
    public class SomethingwentErrorModel : PageModel
    {
        public string ErrorMessage { get; set; }
        public void OnGet(string Message)
        {
            ErrorMessage = Message;
        }
    }
}
