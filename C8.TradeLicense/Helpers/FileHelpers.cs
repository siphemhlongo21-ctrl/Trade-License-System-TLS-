using C8.TradeLicense.Constansts;
using C8.TradeLicense.DataAccessLayer;
using C8.TradeLicense.Keys;
using C8.TradeLicense.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace C8.TradeLicense.Helpers
{
    public class FileHelpers
    {
        private readonly TradeLicenseDbContext _db;
        private string SharePointSite { get; set; }
        private string SharePointLibrary { get; set; }
        private string SharePointApiBaseUrl { get; set; }
        private string UserName { get; set; }
        private string Password { get; set; }
        public FileHelpers(TradeLicenseDbContext db)
        {
            _db = db;
            Initialise();
        }
        public FileHelpers()
        {
            _db = new TradeLicenseDbContext();
            Initialise();
        }
        private void Initialise()
        {
            AppSetting appSetting = _db.AppSettings.FirstOrDefault(a => a.Key == AppSettingConstants.SharePointBaseUrl);
            if (appSetting != null && !string.IsNullOrEmpty(appSetting.Value))
                SharePointApiBaseUrl = appSetting.Value;
            appSetting = _db.AppSettings.FirstOrDefault(a => a.Key == AppSettingConstants.SharePointLibraryName);
            if (appSetting != null && !string.IsNullOrEmpty(appSetting.Value))
                SharePointLibrary = appSetting.Value;
            appSetting = _db.AppSettings.FirstOrDefault(a => a.Key == AppSettingConstants.SharePointSite);
            if (appSetting != null && !string.IsNullOrEmpty(appSetting.Value))
                SharePointSite = appSetting.Value;
            appSetting = _db.AppSettings.FirstOrDefault(a => a.Key == AppSettingConstants.SharePointUsername);
            if (appSetting != null && !string.IsNullOrEmpty(appSetting.Value))
                UserName = appSetting.Value;
            appSetting = _db.AppSettings.FirstOrDefault(a => a.Key == AppSettingConstants.SharePointPassword);
            if (appSetting != null && !string.IsNullOrEmpty(appSetting.Value))
                Password = appSetting.Value;

            if (string.IsNullOrEmpty(SharePointApiBaseUrl) || string.IsNullOrEmpty(SharePointSite) || string.IsNullOrEmpty(SharePointLibrary)
                || string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
                throw new Exception("SharePoint configuration is missing. Please check application settings.");
        }


        public async Task<SharePointDocument> DownloadFile(string address)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    address = HttpUtility.UrlEncode(address, Encoding.UTF8);

                    string url = $"{SharePointApiBaseUrl}/api/dms-engine/download?sharePointAddress={address}";

                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<SharePointDocument>(json);
                }
            }
            catch (Exception ex)
            {
                return new SharePointDocument { ErrorMessage = ex.Message };
            }
        }





        //public async Task<SharePointDocument> DownloadFile(string address)
        //{
        //    try
        //    {
        //        HttpClient client = new HttpClient();
        //        //L.M.20240606 - Encoded Url to handle special charecters on the filename.
        //        address = HttpUtility.UrlEncode(address, Encoding.UTF8);
        //        string url = $"{SharePointApiBaseUrl}/api/dms-engine/download?sharePointAddress={address}";

        //        var request = new HttpRequestMessage(HttpMethod.Post, url);

        //        var result = await client.SendAsync(request);
        //        result.EnsureSuccessStatusCode();

        //        StreamReader loResponseStream = new StreamReader(result.Content.ReadAsStreamAsync().Result, Encoding.UTF8);
        //        string response_from_server = loResponseStream.ReadToEnd();

        //        var response = JsonConvert.DeserializeObject<SharePointDocument>(response_from_server);
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        //TODO: Log Error
        //        return new SharePointDocument { ErrorMessage = ex.Message };
        //    }
        //}
        public SharePointDocument UploadFileToSharepoint(byte[] oFile, string fileName, string referenceId)
        {
            try
            {
                DocumentViewModel documentViewModel = new DocumentViewModel
                {
                    SharePointSite = SharePointSite,
                    SharePointLibrary = SharePointLibrary,
                    FileContent = oFile,
                    ReferenceId = referenceId,
                    FileName = fileName,
                    Application = "TLS",
                    UserName = UserName,
                    Password = Password
                };

                var client = new HttpClient();
                string documentViewModelJson = JsonConvert.SerializeObject(documentViewModel);
                StringContent content = new StringContent(documentViewModelJson, Encoding.UTF8, "application/json");

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{SharePointApiBaseUrl}/api/dms-engine/upload")
                {
                    Content = content
                };

                client.Timeout = new TimeSpan(0, 20, 0);

                HttpResponseMessage result = client.SendAsync(request).Result;
                result.EnsureSuccessStatusCode();

                StreamReader loResponseStream = new StreamReader(result.Content.ReadAsStreamAsync().Result, Encoding.UTF8);
                string response_from_server = loResponseStream.ReadToEnd();

                var response = JsonConvert.DeserializeObject<SharePointDocument>(response_from_server);
                response.FileName = fileName;
                return response;
            }
            catch (Exception ex)
            {
                //TODO: Log Error
                return new SharePointDocument { ErrorMessage = ex.Message };
            }
        }
        public void SaveClientFiles(IEnumerable<HttpPostedFileBase> files, string clientComment, int clientId, string clientFullname, string uploadedby, int createdByUserId)
        {
            int documentTypeId = _db.DocumentTypes.FirstOrDefault(dt => dt.DocumentTypeKey == DocumentTypeKeys.Clientdocument)?.DocumentTypeId ?? 0;
            List<Document> docsupload = _db.Documents.Where(d => d.DocumentTypeId == documentTypeId && d.IsActive && d.IsDeleted == false).Include(d => d.DocumentType).ToList();

            int doccount = docsupload.Count();
            int counter = 0;
            if (files != null && files.Any())
            {
                foreach (HttpPostedFileBase file in files)
                {
                    if (file != null && counter < doccount)
                    {
                        int position = file.FileName.LastIndexOf(".", StringComparison.Ordinal);
                        string filename = file.FileName.Substring(0, position);
                        string extension = file.FileName.Substring(position + 1);

                        //L.M 20141119 - Exclude special characters by replacing with the underscore from the filename
                        const string regExp = @"[^\w\d]";
                        string uploadName = Regex.Replace(filename, regExp, "_") + "." + extension;
                        string name = Regex.Replace(filename, regExp, "_");
                        string contentType = file.ContentType;

                        int fileLen = file.ContentLength;
                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(file.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(file.ContentLength);
                        }
                        string comment = string.Empty;
                        if (!string.IsNullOrEmpty(clientComment))
                        {
                            char[] spearator = { '*' };

                            String[] Commentlist = clientComment.Split(spearator, StringSplitOptions.None);
                            Commentlist = Commentlist.Where(t => t != ",").ToArray();
                            if (Commentlist[counter].ToString() != "N/A")
                            {
                                comment = Commentlist[counter].ToString();
                            }
                        }
                        SharePointDocument sharePointDocument = UploadFileToSharepoint(fileData, file.FileName, "Client");
                        SaveFile(sharePointDocument.FileName, sharePointDocument.DocumentUrl, clientId
                        , clientFullname, docsupload[counter].DocumentId, clientId, comment, uploadedby);
                        counter++;
                    }
                    else
                    {
                        counter++;
                    }
                }
            }
        }
        public void SaveBusinessFiles(IEnumerable<HttpPostedFileBase> files, string BussinessComment, int clientId, int businessId
            , string clientFullname, string uploadedby, int createdByUserId, int? itemType)
        {
            if (files == null)
                return;

            int documentTypeId = _db.DocumentTypes.FirstOrDefault(dt => dt.DocumentTypeKey == DocumentTypeKeys.Bussinessdocument)?.DocumentTypeId ?? 0;
            // load documents once and materialize to list
            string itemtypeval = _db.ItemTypes.FirstOrDefault(c => c.ItemTypeId == itemType)?.ItemTypeName ?? string.Empty;
            List<Document> docs2;
            if (itemtypeval == "Item2")
            {
                docs2 = _db.Documents
                    .Where(d => d.DocumentTypeId == documentTypeId && d.IsActive && !d.IsDeleted)
                    .Include(d => d.DocumentType)
                    .ToList();
            }
            else
            {
                docs2 = _db.Documents
                    .Where(d => d.DocumentTypeId == documentTypeId && d.IsActive && !d.IsDeleted
                        && d.DocumentKey != TLKeys.fingerprint_Verification && d.DocumentKey != TLKeys.Events_Management)
                    .Include(d => d.DocumentType)
                    .ToList();
            }

            if (docs2 == null || docs2.Count == 0)
                return;

            // Prepare comments array (if any)
            char[] separator = { '*' };
            string[] commentList = Array.Empty<string>();
            if (!string.IsNullOrEmpty(BussinessComment))
            {
                commentList = BussinessComment.Split(separator, StringSplitOptions.None)
                    .Where(t => t != ",")
                    .ToArray();
            }

            int docCount = docs2.Count;
            int counter = 0;

            foreach (HttpPostedFileBase file in files)
            {
                if (file == null)
                {
                    counter++;
                    continue;
                }
                try
                {
                    // Map file to document template: use same index if available, otherwise fallback to first
                    Document targetDoc = counter < docCount ? docs2[counter] : docs2[0];

                    // Safe filename/extraction
                    string originalFileName = Path.GetFileName(file.FileName) ?? string.Empty;
                    string filenameWithoutExt = Path.GetFileNameWithoutExtension(originalFileName) ?? string.Empty;
                    string extension = Path.GetExtension(originalFileName) ?? string.Empty;
                    if (extension.StartsWith("."))
                        extension = extension.Substring(1);

                    // sanitize filename to remove special characters
                    const string regExp = @"[^\w\d]";
                    string safeName = Regex.Replace(filenameWithoutExt, regExp, "_");
                    string uploadName = string.IsNullOrEmpty(extension) ? safeName : $"{safeName}.{extension}";

                    // read file bytes
                    byte[] fileData;
                    using (var binaryReader = new BinaryReader(file.InputStream))
                    {
                        fileData = binaryReader.ReadBytes(file.ContentLength);
                    }

                    // determine comment for this file (if provided)
                    string comment = string.Empty;
                    if (commentList.Length > 0)
                    {
                        // prefer matching index, else use first non-empty item
                        if (counter < commentList.Length && !string.Equals(commentList[counter], "N/A", StringComparison.OrdinalIgnoreCase))
                        {
                            comment = commentList[counter];
                        }
                        else if (!string.Equals(commentList[0], "N/A", StringComparison.OrdinalIgnoreCase))
                        {
                            comment = commentList[0];
                        }
                    }

                    // upload and persist
                    SharePointDocument sharePointDocument = UploadFileToSharepoint(fileData, originalFileName, "Business");
                    if (sharePointDocument != null && string.IsNullOrEmpty(sharePointDocument.ErrorMessage))
                    {
                        SaveFile(sharePointDocument.FileName, sharePointDocument.DocumentUrl, clientId,
                            clientFullname, targetDoc.DocumentId, businessId, comment, uploadedby);
                    }
                    else
                    {
                        // TODO: log sharePointDocument.ErrorMessage for diagnostics
                    }
                }
                catch (Exception)
                {
                    // TODO: log exception; swallow so other files can continue
                }
                finally
                {
                    counter++;
                }
            }
        }

        public void SaveFile(string fileName, string url, int clientId, string clientName, int docId, int refId, string comment, string userUploaded)
        {
            FileUpload fileupload = new FileUpload
            {
                FileName = fileName,
                FilePath = url,
                ClientId = clientId,
                ClientName = clientName,
                DocumentId = docId,
                referenceId = refId,
                IsActive = true,
                IsDeleted = false,
                IsLocked = false,
                Uploadeddate = DateTime.Now.ToString(),
                Comments = comment
                ,
                Uploadedby = userUploaded
            };
            _db.FileUploads.Add(fileupload);
            _db.SaveChanges();
        }
    }
}