using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;

using EnterprisePics.Web.Model;

namespace EnterprisePics.Web.Controllers
{
    public class PicturesController : Controller
    {
        public ActionResult Overview()
        {
            var model = new PicturesOverviewModel();

            try
            {
                var path = Config.GetPicturesPath();
                var directory = new DirectoryInfo(path);
                model.Path = path;
                model.Items = directory.GetFiles("*", SearchOption.AllDirectories).OrderByDescending(f => f.LastWriteTimeUtc);
            }
            catch (Exception ex)
            {
                model.ErrorMessage = ex.Message;
            }

            return PartialView(model);
        }

        public ActionResult Display(string fileName, string directory)
        {
            var model = new PictureDisplayModel();
            model.Filename = fileName;
            model.ImageUrl = Url.Action("Render", new {fileName, directory});
            return PartialView(model);
        }

        public ActionResult Render(string fileName, string directory)
        {
            if (fileName.EndsWith("jpg") || fileName.EndsWith("jpeg"))
                return File(Path.Combine(Config.GetPicturesPath(), directory, fileName), "image/jpeg");
            return File(Path.Combine(Config.GetPicturesPath(), directory, fileName), "image/png");
        }

        public ActionResult Upload()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult Upload(PictureUploadModel model)
        {
            try
            {
                var directory = Path.Combine(Config.GetPicturesPath(), model.DirectoryName);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                var filename = Path.GetFileName(model.File.FileName);
                model.File.SaveAs(Path.Combine(directory, filename));

                return RedirectToAction("Index", "Home", new { message = String.Format("The file {0} has been uploaded.", filename)});
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Home", new { errorMessage = String.Format("Error uploading file. {0}", ex.Message) });
            }
        }
    }
}