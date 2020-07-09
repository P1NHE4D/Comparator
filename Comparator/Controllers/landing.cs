using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Comparator.Controllers {
    public class Landing : ControllerBase
    {
        private String page;

        private String css;
        // TODO
        // Falls da wer Lust drauf hat ...
        // private String favicon;

        public Landing() {
            var reader = new StreamReader(this.GetType().
                                               Assembly.GetManifestResourceStream("Comparator.wwwroot.landingpage.index.html"));
            page = reader.ReadToEnd();
            reader = new StreamReader(this.GetType().
                                               Assembly.GetManifestResourceStream("Comparator.wwwroot.landingpage.index.css"));
            css = reader.ReadToEnd();
            // TODO 
            // reader = new StreamReader(this.GetType().
            //                               Assembly.GetManifestResourceStream("Comparator.wwwroot.landingpage.favicon.png"));
            // favicon = reader.ReadToEnd();
        }
        [HttpGet("/")]
        public ContentResult Index() {
            return base.Content(
                page,
                "text/html"
            );
        }
        [HttpGet("/index.css")]
        public ContentResult Css() {
            return base.Content(
                css,
                "text/css"
            );
        }
        // TODO
        /*[HttpGet("/favicon.png")]
        public ContentResult Favicon() {
            return Content(
                favicon,
                "image/png"
            );
        }*/
    }
}