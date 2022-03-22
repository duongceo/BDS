using System.IO;
using System.Web.Mvc;
using System;

using MBN.Utils;

namespace HappyRE.Web.Models
{
    /// <summary>
    /// Based on FineUploader
    /// Thanks to FineUploader team.
    /// </summary>
    [ModelBinder(typeof(UploadFileBinder))]
    public class FileUploader : Core.MapModels.FileUploaderBase
    {
        public class UploadFileBinder : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var request = controllerContext.RequestContext.HttpContext.Request;
                var formUpload = request.Files.Count > 0;

                // find filename
                var xFileName = request.Headers["X-File-Name"];
                var qqFile = request["Filename"];//request["qqfile"];
                var fileType = formUpload ? request.Files[0].ContentType : null;
                var formFilename = formUpload ? request.Files[0].FileName : null;

                var upload = new FileUploader
                {
                    Filename = xFileName ?? qqFile ?? formFilename,
                    FileType = fileType,
                    InputStream = formUpload ? request.Files[0].InputStream : request.InputStream
                };

                return upload;
            }
        }

    }
}