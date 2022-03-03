using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using Ionic.Zip;
using System.IO;
using HappyRE.Core.Utils;
using log4net;
using System.Collections.ObjectModel;

namespace HappyRE.Static.Services
{
    public class FileBuilder
    {
        private static readonly ILog _log = LogManager.GetLogger("FileBuilder");
        string _folder = "";
        string _domain = "";
        string _contentHtml = "";

        private static readonly string SYS_FOLDER = ConfigSettings.Get("SYS_FOLDER", @"C:\Punnel\sys");
        private static readonly string SYS_FOLDER_CONFIG = ConfigSettings.Get("SYS_FOLDER", @"C:\Punnel\sys_config");
        private static readonly string PUBLISH_FOLDER_ROOT = ConfigSettings.Get("PUBLISH_FOLDER_ROOT", @"C:\Punnel\LANDING_PAGES");
        static readonly ReadOnlyCollection<string> FREE_DOMAIN = new ReadOnlyCollection<string>(
          new string[] {
            "HappyRE.vip",
            "HappyRE.link",
            "HappyRE.page",
            "HappyRE.shop",
            "HappyRE.store"
          }
        );
        private static readonly string htmlFileName = "index.html";
        public FileBuilder() { }

        public FileBuilder(string domain, string folder, string contentHtml)
        {
            _folder = folder == null ? "" : folder;
            _folder = _folder.Replace("/", "");
            _contentHtml = contentHtml;
            _domain = domain;
        }

        public string Create(string page_id)
        {
            if (string.IsNullOrEmpty(_domain)) return null;
            string domain_path = System.IO.Path.Combine(PUBLISH_FOLDER_ROOT, _domain);
            if (!Directory.Exists(domain_path))
            {
                System.IO.Directory.CreateDirectory(domain_path);
            }

            if (IsSysDomain(_domain) == false)
            {
                if (HasDefaultFile(domain_path) == false)
                {
                    CreateDefaultSystemFile(_domain, page_id);
                }
                else
                {
                    var destFolder = System.IO.Path.Combine(PUBLISH_FOLDER_ROOT, _domain.Replace("/", ""));
                }
            }

            string sfolder = System.IO.Path.Combine(PUBLISH_FOLDER_ROOT, _domain, _folder);
            if (!string.IsNullOrEmpty(_folder))
            {
                //Tao Folder 
                System.IO.Directory.CreateDirectory(sfolder);
            }
            if (!string.IsNullOrEmpty(_contentHtml))
            {
                var pathString = System.IO.Path.Combine(sfolder, htmlFileName);
                System.IO.File.WriteAllText(pathString, _contentHtml, Encoding.UTF8);
            }
            return string.IsNullOrEmpty(_folder) ? _domain : string.Format("{0}/{1}", _domain, _folder);
        }

        void CreateDefaultSystemFile(string domain, string page_id)
        {
            var destFolder = System.IO.Path.Combine(PUBLISH_FOLDER_ROOT, domain.Replace("/", ""));
            DirectoryCopy(SYS_FOLDER, destFolder, true);
        }

        public void CreateWebconfigFile()
        {
            string domain_path = System.IO.Path.Combine(PUBLISH_FOLDER_ROOT, _domain);
            if (!Directory.Exists(domain_path))
            {
                System.IO.Directory.CreateDirectory(domain_path);
            }

            if (IsSysDomain(_domain) == false)
            {
                if (HasSysFile(domain_path) == false)
                {
                    var destFolder = System.IO.Path.Combine(PUBLISH_FOLDER_ROOT, _domain.Replace("/", ""));
                    System.IO.File.Copy(System.IO.Path.Combine(SYS_FOLDER_CONFIG, "web.config"), System.IO.Path.Combine(destFolder, "web.config"), false);
                }
            }
        }

        public string ChangeFolder(string domain, string oldFolder, string newFolder, string contentHtml, string page_id)
        {
            oldFolder = oldFolder.Replace("/", "");
            newFolder = newFolder.Replace("/", "");
            if (oldFolder == newFolder) return newFolder;
            string ofolder = System.IO.Path.Combine(PUBLISH_FOLDER_ROOT, domain, oldFolder);
            string nfolder = System.IO.Path.Combine(PUBLISH_FOLDER_ROOT, domain, newFolder);
            if (System.IO.Directory.Exists(ofolder) && !string.IsNullOrEmpty(oldFolder) && !string.IsNullOrEmpty(newFolder))
            {
                System.IO.Directory.Move(ofolder, nfolder);
            }
            else
            {
                if (!System.IO.Directory.Exists(nfolder))
                {
                    _folder = newFolder;
                    _domain = domain;
                    _contentHtml = contentHtml;
                    return Create(page_id);
                }
                if (string.IsNullOrEmpty(oldFolder))
                {
                    System.IO.File.Delete(System.IO.Path.Combine(ofolder, "index.html"));
                }
                else
                {
                    System.IO.Directory.Delete(ofolder);
                }
            }
            return string.IsNullOrEmpty(newFolder) ? domain : string.Format("{0}/{1}", domain, newFolder);
        }

        public bool CanRemoveIIS(string domain)
        {
            DirectoryInfo dir = new DirectoryInfo(System.IO.Path.Combine(PUBLISH_FOLDER_ROOT, _domain));
            var folders = dir.GetDirectories().Where(x => x.Name != "bin");
            return folders.Count() == 0;
        }
        
        public void Remove()
        {
            string sfolder = System.IO.Path.Combine(PUBLISH_FOLDER_ROOT, _domain, _folder);
            if (System.IO.Directory.Exists(sfolder))
            {
                DirectoryInfo dir = new DirectoryInfo(sfolder);
                if (_folder.Length > 0)
                {
                    System.IO.Directory.Delete(sfolder, true);
                }
                else
                {
                    if (dir.GetDirectories().Length <= 1 && IsSysDomain(sfolder) == false)
                    {
                        System.IO.Directory.Delete(sfolder, true);
                    }
                    else
                    {
                        System.IO.File.Delete(System.IO.Path.Combine(sfolder, "index.html"));
                    }
                }
            }
        }

        public void changeIndexFileContent()
        {
            string sfolder = System.IO.Path.Combine(PUBLISH_FOLDER_ROOT, _domain, _folder);
            var pathString = System.IO.Path.Combine(sfolder, htmlFileName);
            if (!System.IO.Directory.Exists(sfolder) || !System.IO.File.Exists(pathString))
            {
                return;
            }
            string contentHtml = File.ReadAllText(pathString);
            contentHtml = contentHtml.Replace("title-site", "pn-title-site");
            contentHtml = contentHtml.Replace("pn-title-site", "punnel-title-site");
            System.IO.File.WriteAllText(pathString, contentHtml, Encoding.UTF8);
        }

        public bool setStartupPath(string path)
        {
            if (string.IsNullOrEmpty(_domain) || IsSysDomain(_domain) == true) return false;
            if (path == null) path = string.Empty;
            string sfolder_domain = System.IO.Path.Combine(PUBLISH_FOLDER_ROOT, _domain);
            string sfolder = System.IO.Path.Combine(PUBLISH_FOLDER_ROOT, _domain, path.Replace("/", ""));
            var pathString = System.IO.Path.Combine(sfolder_domain, htmlFileName);
            var pathString1 = System.IO.Path.Combine(sfolder_domain, htmlFileName);
            if ((!System.IO.Directory.Exists(sfolder_domain) || !System.IO.File.Exists(pathString)) || (!System.IO.Directory.Exists(sfolder) || !System.IO.File.Exists(pathString1)))
            {
                return false;
            }

            string contentHtml = File.ReadAllText(pathString);
            if (contentHtml.Contains("<title>COMMING SOON</title>") == false)
            {
                return false;
            }
            int b_script = contentHtml.IndexOf("<script>");
            int e_script = contentHtml.IndexOf("</script>");
            if (b_script > 0 && e_script > 0)
            {
                var scr = contentHtml.Substring(b_script, e_script - b_script + 9);
                contentHtml = contentHtml.Replace(scr, @"<script> var startupPath='[[" + path + "]]'; startupPath= startupPath.replaceAll('[[','').replaceAll(']]',''); if(startupPath.length>1) { location.replace(startupPath); }</script>");
            }
            else
            {
                contentHtml = contentHtml.Replace("<style>", @"<script> var startupPath='[[" + path + "]]'; startupPath= startupPath.replaceAll('[[','').replaceAll(']]',''); if(startupPath.length>1) { location.replace(startupPath); }</script> <style>");
            }
            System.IO.File.WriteAllText(pathString, contentHtml, Encoding.UTF8);
            return true;
        }

        public MemoryStream MakeDownLoad()
        {
            string sfolder = System.IO.Path.Combine(PUBLISH_FOLDER_ROOT, _folder);
            MemoryStream stream = new MemoryStream();
            stream.Seek(0, SeekOrigin.Begin);
            using (ZipFile zip = new ZipFile())
            {
                zip.AddDirectory(sfolder);
                zip.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                stream.Flush();
            }

            return stream;
        }

        public MemoryStream MakeDownLoadDefault()
        {
            string sfile = System.IO.Path.Combine(PUBLISH_FOLDER_ROOT, _domain, _folder, "index.html");
            MemoryStream stream = new MemoryStream();
            stream.Seek(0, SeekOrigin.Begin);
            using (ZipFile zip = new ZipFile())
            {
                zip.AddFile(sfile, "");
                zip.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                stream.Flush();
            }

            return stream;
        }

        #region function
        bool IsSysDomain(string domain)
        {
            return FREE_DOMAIN.Any(x => x == _domain);
        }
        bool HasSysFile(string moveToFolder)
        {
            return (File.Exists(System.IO.Path.Combine(moveToFolder, "web.config")));
        }
        bool HasDefaultFile(string moveToFolder)
        {
            return (File.Exists(System.IO.Path.Combine(moveToFolder, "index.html")));
        }
        void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }


            // Get the file contents of the directory to copy.
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                file.Attributes = FileAttributes.Normal;
                // Create the path to the new copy of the file.
                string temppath = Path.Combine(destDirName, file.Name);

                // Copy the file.
                file.CopyTo(temppath, true);
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
            {

                foreach (DirectoryInfo subdir in dirs)
                {
                    // Create the subdirectory.
                    string temppath = Path.Combine(destDirName, subdir.Name);

                    // Copy the subdirectories.
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
        void deleteFileInFolder(DirectoryInfo dir)
        {
            foreach (var file in dir.GetFiles())
            {
                try
                {
                    file.Attributes = FileAttributes.Normal;
                    System.IO.File.Delete(file.FullName);
                }
                catch (Exception ex)
                {
                    _log.ErrorFormat("{0} : {1}", file.Name, ex);
                }
            }
            foreach (var subDir in dir.GetDirectories())
            {
                deleteFileInFolder(subDir);
            }
        }

        void setAttributesNormal(DirectoryInfo dir)
        {
            foreach (var subDir in dir.GetDirectories())
            {
                setAttributesNormal(subDir);
            }
            foreach (var file in dir.GetFiles())
            {
                file.Attributes = FileAttributes.Normal;
            }
        }
        #endregion
    }
}