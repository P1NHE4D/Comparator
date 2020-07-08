using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace Comparator.wwwroot {
    public class Landing : ControllerBase
    {
        private static String page;

        public Landing() {
            var assembly = Assembly.GetExecutingAssembly();

            var reader = new StreamReader(this.GetType().
                Assembly.GetManifestResourceStream("Comparator.wwwroot.landingpage.index.html"));
            page = reader.ReadToEnd();
        }
        [HttpGet("/")]
        public ContentResult Index() {
            return base.Content(
                page,
                "text/html"
            );
        }
    }
}