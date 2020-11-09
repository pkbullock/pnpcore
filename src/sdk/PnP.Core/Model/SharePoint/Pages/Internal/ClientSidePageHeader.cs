﻿using AngleSharp.Html.Parser;
using PnP.Core.Services;
using System;
using System.Linq;
using System.Net;
using System.Text.Json;

namespace PnP.Core.Model.SharePoint
{
    /// <summary>
    /// Class that implements the client side page header
    /// </summary>
    internal class ClientSidePageHeader : IClientSidePageHeader
    {
        private const string NoPageHeader = "<div><div data-sp-canvascontrol=\"\" data-sp-canvasdataversion=\"1.4\" data-sp-controldata=\"&#123;&quot;id&quot;&#58;&quot;cbe7b0a9-3504-44dd-a3a3-0e5cacd07788&quot;,&quot;instanceId&quot;&#58;&quot;cbe7b0a9-3504-44dd-a3a3-0e5cacd07788&quot;,&quot;title&quot;&#58;&quot;Title Region&quot;,&quot;description&quot;&#58;&quot;Title Region Description&quot;,&quot;serverProcessedContent&quot;&#58;&#123;&quot;htmlStrings&quot;&#58;&#123;&#125;,&quot;searchablePlainTexts&quot;&#58;&#123;&#125;,&quot;imageSources&quot;&#58;&#123;&#125;,&quot;links&quot;&#58;&#123;&#125;&#125;,&quot;dataVersion&quot;&#58;&quot;1.4&quot;,&quot;properties&quot;&#58;&#123;&quot;title&quot;&#58;&quot;@@title@@&quot;,&quot;imageSourceType&quot;&#58;4,&quot;layoutType&quot;&#58;&quot;NoImage&quot;,&quot;textAlignment&quot;&#58;&quot;@@textalignment@@&quot;,&quot;showTopicHeader&quot;&#58;@@showtopicheader@@,&quot;showPublishDate&quot;&#58;@@showpublishdate@@,&quot;topicHeader&quot;&#58;&quot;@@topicheader@@&quot;&#125;&#125;\"></div></div>";
        private const string DefaultPageHeader = "<div><div data-sp-canvascontrol=\"\" data-sp-canvasdataversion=\"1.4\" data-sp-controldata=\"&#123;&quot;id&quot;&#58;&quot;cbe7b0a9-3504-44dd-a3a3-0e5cacd07788&quot;,&quot;instanceId&quot;&#58;&quot;cbe7b0a9-3504-44dd-a3a3-0e5cacd07788&quot;,&quot;title&quot;&#58;&quot;Title Region&quot;,&quot;description&quot;&#58;&quot;Title Region Description&quot;,&quot;serverProcessedContent&quot;&#58;&#123;&quot;htmlStrings&quot;&#58;&#123;&#125;,&quot;searchablePlainTexts&quot;&#58;&#123;&#125;,&quot;imageSources&quot;&#58;&#123;&#125;,&quot;links&quot;&#58;&#123;&#125;&#125;,&quot;dataVersion&quot;&#58;&quot;1.4&quot;,&quot;properties&quot;&#58;&#123;&quot;title&quot;&#58;&quot;@@title@@&quot;,&quot;imageSourceType&quot;&#58;4,&quot;layoutType&quot;&#58;&quot;@@layouttype@@&quot;,&quot;textAlignment&quot;&#58;&quot;@@textalignment@@&quot;,&quot;showTopicHeader&quot;&#58;@@showtopicheader@@,&quot;showPublishDate&quot;&#58;@@showpublishdate@@,&quot;topicHeader&quot;&#58;&quot;@@topicheader@@&quot;,&quot;authorByline&quot;&#58;[@@authorbyline@@],&quot;authors&quot;&#58;[@@authors@@]&#125;&#125;\"></div></div>";
        private const string CustomPageHeader = "<div><div data-sp-canvascontrol=\"\" data-sp-canvasdataversion=\"1.4\" data-sp-controldata=\"&#123;&quot;id&quot;&#58;&quot;cbe7b0a9-3504-44dd-a3a3-0e5cacd07788&quot;,&quot;instanceId&quot;&#58;&quot;cbe7b0a9-3504-44dd-a3a3-0e5cacd07788&quot;,&quot;title&quot;&#58;&quot;Title Region&quot;,&quot;description&quot;&#58;&quot;Title Region Description&quot;,&quot;serverProcessedContent&quot;&#58;&#123;&quot;htmlStrings&quot;&#58;&#123;&#125;,&quot;searchablePlainTexts&quot;&#58;&#123;&#125;,&quot;imageSources&quot;&#58;&#123;&quot;imageSource&quot;&#58;&quot;@@imageSource@@&quot;&#125;,&quot;links&quot;&#58;&#123;&#125;,&quot;customMetadata&quot;&#58;&#123;&quot;imageSource&quot;&#58;&#123;&quot;siteId&quot;&#58;&quot;@@siteId@@&quot;,&quot;webId&quot;&#58;&quot;@@webId@@&quot;,&quot;listId&quot;&#58;&quot;@@listId@@&quot;,&quot;uniqueId&quot;&#58;&quot;@@uniqueId@@&quot;&#125;&#125;&#125;,&quot;dataVersion&quot;&#58;&quot;1.4&quot;,&quot;properties&quot;&#58;&#123;&quot;title&quot;&#58;&quot;@@title@@&quot;,&quot;imageSourceType&quot;&#58;2,&quot;layoutType&quot;&#58;&quot;@@layouttype@@&quot;,&quot;textAlignment&quot;&#58;&quot;@@textalignment@@&quot;,&quot;showTopicHeader&quot;&#58;@@showtopicheader@@,&quot;showPublishDate&quot;&#58;@@showpublishdate@@,&quot;topicHeader&quot;&#58;&quot;@@topicheader@@&quot;,&quot;authorByline&quot;&#58;[@@authorbyline@@],&quot;authors&quot;&#58;[@@authors@@],&quot;altText&quot;&#58;&quot;@@alternativetext@@&quot;,&quot;webId&quot;&#58;&quot;@@webId@@&quot;,&quot;siteId&quot;&#58;&quot;@@siteId@@&quot;,&quot;listId&quot;&#58;&quot;@@listId@@&quot;,&quot;uniqueId&quot;&#58;&quot;@@uniqueId@@&quot;@@focalPoints@@&#125;&#125;\"></div></div>";
        private PageHeaderType pageHeaderType;
        private string imageServerRelativeUrl;
        private readonly PnPContext clientContext;
        private bool headerImageResolved;
        private Guid siteId = Guid.Empty;
        private Guid webId = Guid.Empty;
        private Guid listId = Guid.Empty;
        private Guid uniqueId = Guid.Empty;

        /// <summary>
        /// Returns the type of header
        /// </summary>
        public PageHeaderType Type
        {
            get
            {
                return this.pageHeaderType;
            }
        }

        /// <summary>
        /// Server relative link to page header image, set to null for default header image.
        /// Note: image needs to reside in the current site
        /// </summary>
        public string ImageServerRelativeUrl
        {
            get
            {
                return this.imageServerRelativeUrl;
            }
            set
            {
                this.imageServerRelativeUrl = value;
                this.headerImageResolved = false;
            }
        }

        /// <summary>
        /// Image focal point X coordinate
        /// </summary>
        public double? TranslateX { get; set; }

        /// <summary>
        /// Image focal point Y coordinate
        /// </summary>
        public double? TranslateY { get; set; }

        /// <summary>
        /// Type of layout used inside the header
        /// </summary>
        public PageHeaderLayoutType LayoutType { get; set; }

        /// <summary>
        /// Alignment of the title in the header
        /// </summary>
        public PageHeaderTitleAlignment TextAlignment { get; set; }

        /// <summary>
        /// Show the topic header in the title region
        /// </summary>
        public bool ShowTopicHeader { get; set; }

        /// <summary>
        /// Show the page publication date in the title region
        /// </summary>
        public bool ShowPublishDate { get; set; }

        /// <summary>
        /// The topic header text to show if ShowTopicHeader is set to true
        /// </summary>
        public string TopicHeader { get; set; }

        /// <summary>
        /// Alternative text for the header image
        /// </summary>
        public string AlternativeText { get; set; }

        /// <summary>
        /// Page author(s) to be displayed
        /// </summary>
        public string Authors { get; set; }

        /// <summary>
        /// Page author byline
        /// </summary>
        public string AuthorByLine { get; set; }

        /// <summary>
        /// Id of the page author
        /// </summary>
        public int AuthorByLineId { get; set; }

        #region construction
        /// <summary>
        /// Creates a custom header with a custom image
        /// </summary>
        /// <param name="cc">ClientContext of the site hosting the image</param>
        /// <param name="pageHeaderType">Type of page header</param>
        /// <param name="imageServerRelativeUrl">Server relative image url</param>
        public ClientSidePageHeader(PnPContext cc, PageHeaderType pageHeaderType, string imageServerRelativeUrl)
        {
            this.imageServerRelativeUrl = imageServerRelativeUrl;
            this.clientContext = cc;
            this.pageHeaderType = pageHeaderType;
            this.LayoutType = PageHeaderLayoutType.FullWidthImage;
            this.TextAlignment = PageHeaderTitleAlignment.Left;
            this.ShowTopicHeader = false;
            this.TopicHeader = "";
            this.Authors = "";
            this.AlternativeText = "";
            this.ShowPublishDate = false;
            this.AuthorByLineId = -1;
        }

        /// <summary>
        /// Creates a custom header with a custom image + custom image offset
        /// </summary>
        /// <param name="cc">ClientContext of the site hosting the image</param>
        /// <param name="pageHeaderType">Type of page header</param>
        /// <param name="imageServerRelativeUrl">Server relative image url</param>
        /// <param name="translateX">X offset coordinate</param>
        /// <param name="translateY">Y offset coordinate</param>
        public ClientSidePageHeader(PnPContext cc, PageHeaderType pageHeaderType, string imageServerRelativeUrl, double translateX, double translateY) : this(cc, pageHeaderType, imageServerRelativeUrl)
        {
            TranslateX = translateX;
            TranslateY = translateY;
        }
        #endregion

        /// <summary>
        /// Returns the header value to set a "no header"
        /// </summary>
        /// <param name="pageTitle">Title of the page</param>
        /// <param name="titleAlignment">Left align or center the title</param>
        /// <returns>Header html value that indicates "no header"</returns>
        private static string NoHeader(string pageTitle, PageHeaderTitleAlignment titleAlignment)
        {
            if (pageTitle == null)
            {
                pageTitle = "";
            }
            else
            {
                pageTitle = EncodePageTitle(pageTitle);
            }

            string header = Replace1point4Defaults(NoPageHeader);

            return header.Replace("@@title@@", pageTitle).Replace("@@textalignment@@", titleAlignment.ToString());
        }

        /// <summary>
        /// Returns the header value to set a "no header"
        /// </summary>
        /// <param name="pageTitle">Title of the page</param>
        /// <returns>Header html value that indicates "no header"</returns>
        public static string NoHeader(string pageTitle)
        {
            return NoHeader(pageTitle, PageHeaderTitleAlignment.Left);
        }

        /// <summary>
        /// Load the PageHeader object from the given html
        /// </summary>
        /// <param name="pageHeaderHtml">Page header html</param>
        public void FromHtml(string pageHeaderHtml)
        {
            // select all control div's
            if (String.IsNullOrEmpty(pageHeaderHtml))
            {
                this.pageHeaderType = PageHeaderType.Default;
                return;
            }

            HtmlParser parser = new HtmlParser(new HtmlParserOptions() { IsEmbedded = true });
            using (var document = parser.ParseDocument(pageHeaderHtml))
            {
                var pageHeaderControl = document.All.Where(m => m.HasAttribute(CanvasControl.ControlDataAttribute)).FirstOrDefault();
                if (pageHeaderControl != null)
                {
                    string pageHeaderData = pageHeaderControl.GetAttribute(ClientSideWebPart.ControlDataAttribute);
                    string decoded = "";

                    if (pageHeaderData.Contains("%7B") && pageHeaderData.Contains("%22") && pageHeaderData.Contains("%7D"))
                    {
                        decoded = WebUtility.UrlDecode(pageHeaderData);
                    }
                    else
                    {
                        decoded = WebUtility.HtmlDecode(pageHeaderData);
                    }

                    //JObject wpJObject = JObject.Parse(decoded);
                    var wpJObject = JsonDocument.Parse(decoded).RootElement;

                    // Store the server processed content as that's needed for full fidelity
                    //if (wpJObject["serverProcessedContent"] != null)
                    if (wpJObject.TryGetProperty("serverProcessedContent", out JsonElement serverProcessedContent))
                    {
                        //if (wpJObject["serverProcessedContent"]["imageSources"] != null && wpJObject["serverProcessedContent"]["imageSources"]["imageSource"] != null)
                        //{
                        //    this.imageServerRelativeUrl = wpJObject["serverProcessedContent"]["imageSources"]["imageSource"].ToString();
                        //}

                        if (serverProcessedContent.TryGetProperty("imageSources", out JsonElement imageSources) && imageSources.TryGetProperty("imageSource", out JsonElement imageSource))
                        {
                            this.imageServerRelativeUrl = imageSource.GetString();
                        }

                        // Properties that apply to all header configurations
                        if (wpJObject.TryGetProperty("properties", out JsonElement properties))
                        {
                            //if (wpJObject["properties"]["layoutType"] != null)
                            //{
                            //    this.LayoutType = (PageHeaderLayoutType)Enum.Parse(typeof(PageHeaderLayoutType), wpJObject["properties"]["layoutType"].ToString());
                            //}
                            if (properties.TryGetProperty("layoutType", out JsonElement layoutType))
                            {
                                this.LayoutType = (PageHeaderLayoutType)Enum.Parse(typeof(PageHeaderLayoutType), layoutType.GetString());
                            }
                            //if (wpJObject["properties"]["textAlignment"] != null)
                            //{
                            //    this.TextAlignment = (PageHeaderTitleAlignment)Enum.Parse(typeof(PageHeaderTitleAlignment), wpJObject["properties"]["textAlignment"].ToString());
                            //}
                            if (properties.TryGetProperty("textAlignment", out JsonElement textAlignment))
                            {
                                this.TextAlignment = (PageHeaderTitleAlignment)Enum.Parse(typeof(PageHeaderTitleAlignment), textAlignment.GetString());
                            }
                            //if (wpJObject["properties"]["showTopicHeader"] != null)
                            //{
                            //    bool showTopicHeader = false;
                            //    bool.TryParse(wpJObject["properties"]["showTopicHeader"].ToString(), out showTopicHeader);
                            //    this.ShowTopicHeader = showTopicHeader;
                            //}
                            if (properties.TryGetProperty("showTopicHeader", out JsonElement showTopicHeader))
                            {
                                this.ShowTopicHeader = showTopicHeader.GetBoolean();
                            }
                            //if (wpJObject["properties"]["showPublishDate"] != null)
                            //{
                            //    bool showPublishDate = false;
                            //    bool.TryParse(wpJObject["properties"]["showPublishDate"].ToString(), out showPublishDate);
                            //    this.ShowPublishDate = showPublishDate;
                            //}
                            if (properties.TryGetProperty("showPublishDate", out JsonElement showPublishDate))
                            {
                                this.ShowPublishDate = showPublishDate.GetBoolean();
                            }
                            //if (wpJObject["properties"]["topicHeader"] != null)
                            //{
                            //    this.TopicHeader = wpJObject["properties"]["topicHeader"].ToString();
                            //}
                            if (properties.TryGetProperty("topicHeader", out JsonElement topicHeader))
                            {
                                this.TopicHeader = topicHeader.GetString();
                            }
                            //if (wpJObject["properties"]["authors"] != null)
                            //{
                            //    this.Authors = wpJObject["properties"]["authors"].ToString();
                            //}
                            if (properties.TryGetProperty("authors", out JsonElement authors))
                            {
                                this.Authors = authors.ToString();
                            }
                            //if (wpJObject["properties"]["authorByline"] != null)
                            //{
                            //    this.AuthorByLine = wpJObject["properties"]["authorByline"].ToString();
                            //}
                            if (properties.TryGetProperty("authorByline", out JsonElement authorByline))
                            {
                                this.AuthorByLine = authorByline.ToString();
                            }

                            // Specific properties that only apply when the header has a custom image
                            if (!string.IsNullOrEmpty(this.imageServerRelativeUrl))
                            {
                                this.pageHeaderType = PageHeaderType.Custom;
                                //if (wpJObject["properties"] != null)
                                //{
                                Guid result = new Guid();
                                //if (wpJObject["properties"]["siteId"] != null && Guid.TryParse(wpJObject["properties"]["siteId"].ToString(), out result))
                                //{
                                //    this.siteId = result;
                                //}
                                if (properties.TryGetProperty("siteId", out JsonElement siteId) && siteId.TryGetGuid(out Guid siteIdGuid))
                                {
                                    this.siteId = siteIdGuid;
                                }
                                //if (wpJObject["properties"]["webId"] != null && Guid.TryParse(wpJObject["properties"]["webId"].ToString(), out result))
                                //{
                                //    this.webId = result;
                                //}
                                if (properties.TryGetProperty("webId", out JsonElement webId) && webId.TryGetGuid(out Guid webIdGuid))
                                {
                                    this.webId = webIdGuid;
                                }
                                //if (wpJObject["properties"]["listId"] != null && Guid.TryParse(wpJObject["properties"]["listId"].ToString(), out result))
                                //{
                                //    this.listId = result;
                                //}
                                if (properties.TryGetProperty("listId", out JsonElement listId) && listId.TryGetGuid(out Guid listIdGuid))
                                {
                                    this.listId = listIdGuid;
                                }
                                //if (wpJObject["properties"]["uniqueId"] != null && Guid.TryParse(wpJObject["properties"]["uniqueId"].ToString(), out result))
                                //{
                                //    this.uniqueId = result;
                                //}
                                if (properties.TryGetProperty("uniqueId", out JsonElement uniqueId) && uniqueId.TryGetGuid(out Guid uniqueIdGuid))
                                {
                                    this.uniqueId = uniqueIdGuid;
                                }
                                if (this.siteId != Guid.Empty && this.webId != Guid.Empty && this.listId != Guid.Empty && this.uniqueId != Guid.Empty)
                                {
                                    this.headerImageResolved = true;
                                }
                                //}

                                System.Globalization.CultureInfo usCulture = new System.Globalization.CultureInfo("en-US");
                                System.Globalization.CultureInfo europeanCulture = new System.Globalization.CultureInfo("nl-BE");

                                //if (wpJObject["properties"]["translateX"] != null)
                                if (properties.TryGetProperty("translateX", out JsonElement translateXElement))
                                {
                                    //var translateXEN = wpJObject["properties"]["translateX"].ToString();
                                    var translateXEN = translateXElement.GetDecimal().ToString();

                                    System.Globalization.CultureInfo cultureToUse;
                                    if (translateXEN.Contains("."))
                                    {
                                        cultureToUse = usCulture;
                                    }
                                    else if (translateXEN.Contains(","))
                                    {
                                        cultureToUse = europeanCulture;
                                    }
                                    else
                                    {
                                        cultureToUse = usCulture;
                                    }

                                    double.TryParse(translateXEN, System.Globalization.NumberStyles.Float, cultureToUse, out double translateX);
                                    this.TranslateX = translateX;
                                }
                                //if (wpJObject["properties"]["translateY"] != null)
                                if (properties.TryGetProperty("translateY", out JsonElement translateYElement))
                                {
                                    //var translateYEN = wpJObject["properties"]["translateY"].ToString();
                                    var translateYEN = translateYElement.GetDecimal().ToString();

                                    System.Globalization.CultureInfo cultureToUse;
                                    if (translateYEN.Contains("."))
                                    {
                                        cultureToUse = usCulture;
                                    }
                                    else if (translateYEN.Contains(","))
                                    {
                                        cultureToUse = europeanCulture;
                                    }
                                    else
                                    {
                                        cultureToUse = usCulture;
                                    }

                                    double.TryParse(translateYEN, System.Globalization.NumberStyles.Float, cultureToUse, out double translateY);
                                    this.TranslateY = translateY;
                                }
                                //if (wpJObject["properties"]["altText"] != null)
                                if (properties.TryGetProperty("altText", out JsonElement altText))
                                {
                                    //this.AlternativeText = wpJObject["properties"]["altText"].ToString();
                                    this.AlternativeText = altText.GetString();
                                }
                            }
                            else
                            {
                                this.pageHeaderType = PageHeaderType.Default;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the header html representation
        /// </summary>
        /// <param name="pageTitle">Title of the page</param>
        /// <returns>Header html value</returns>
        public string ToHtml(string pageTitle)
        {
            if (pageTitle == null)
            {
                pageTitle = "";
            }
            else
            {
                pageTitle = EncodePageTitle(pageTitle);
            }

            // Get the web part properties
            if (!string.IsNullOrEmpty(this.ImageServerRelativeUrl) && this.clientContext != null)
            {
                if (!headerImageResolved)
                {
                    ResolvePageHeaderImage();
                }

                if (headerImageResolved)
                {
                    string focalPoints = "";
                    if (TranslateX.HasValue || TranslateY.HasValue)
                    {
                        System.Globalization.CultureInfo usCulture = new System.Globalization.CultureInfo("en-US");
                        var translateX = TranslateX.Value.ToString(usCulture);
                        var translateY = TranslateY.Value.ToString(usCulture);
                        focalPoints = $",&quot;translateX&quot;&#58;{translateX},&quot;translateY&quot;&#58;{translateY}";
                    }

                    // Populate default properties
                    var header = FillDefaultProperties(CustomPageHeader);
                    // Populate custom header specific properties
                    return header.Replace("@@siteId@@", this.siteId.ToString()).Replace("@@webId@@", this.webId.ToString()).Replace("@@listId@@", this.listId.ToString()).Replace("@@uniqueId@@", this.uniqueId.ToString()).Replace("@@focalPoints@@", focalPoints).Replace("@@title@@", pageTitle).Replace("@@imageSource@@", this.ImageServerRelativeUrl).Replace("@@alternativetext@@", this.AlternativeText == null ? "" : this.AlternativeText);
                }
            }

            // in case nothing worked out...
            // Populate default properties
            var defaultHeader = FillDefaultProperties(DefaultPageHeader);
            // Populate title
            return defaultHeader.Replace("@@title@@", pageTitle);
        }

        private string FillDefaultProperties(string header)
        {
            if (!string.IsNullOrEmpty(this.Authors))
            {
                string data = this.Authors.Replace("\r", "").Replace("\n", "").TrimStart(new char[] { '[' }).TrimEnd(new char[] { ']' });
                var jsonencoded = WebUtility.HtmlEncode(data).Replace(":", "&#58;"); //.Replace("@", "%40");
                header = header.Replace("@@authors@@", jsonencoded);
            }
            else
            {
                header = header.Replace("@@authors@@", "");
            }

            if (!string.IsNullOrEmpty(this.AuthorByLine))
            {
                string data = this.AuthorByLine.Replace("\r", "").Replace("\n", "").Replace(" ", "").TrimStart(new char[] { '[' }).TrimEnd(new char[] { ']' });
                var jsonencoded = WebUtility.HtmlEncode(data).Replace(":", "&#58;");
                header = header.Replace("@@authorbyline@@", jsonencoded);

                int userId = -1;
                try
                {
                    /* TODO
                    var user = this.clientContext.Web.EnsureUser(data.Replace("\"", "").Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries)[2]);
                    this.clientContext.Load(user);
                    this.clientContext.ExecuteQueryRetry();
                    userId = user.Id;
                    */
                }
                catch (Exception)
                {

                }

                this.AuthorByLineId = userId;
            }
            else
            {
                header = header.Replace("@@authorbyline@@", "");
            }

            return header.Replace("@@showtopicheader@@", this.ShowTopicHeader.ToString().ToLower()).Replace("@@showpublishdate@@", this.ShowPublishDate.ToString().ToLower()).Replace("@@topicheader@@", this.TopicHeader == null ? "" : this.TopicHeader).Replace("@@textalignment@@", this.TextAlignment.ToString()).Replace("@@layouttype@@", this.LayoutType.ToString());
        }

        private static string Replace1point4Defaults(string header)
        {
            return header.Replace("@@showtopicheader@@", "false").Replace("@@showpublishdate@@", "false").Replace("@@topicheader@@", "");
        }

        private static string EncodePageTitle(string pageTitle)
        {
            string result = pageTitle;

            if (result.Contains("\""))
            {
                result = result.Replace("\"", "\\&quot;");
            }

            return result;
        }

        private void ResolvePageHeaderImage()
        {
            /* TODO
            try
            {
                this.siteId = this.clientContext.Site.EnsureProperty(p => p.Id);
                this.webId = this.clientContext.Web.EnsureProperty(p => p.Id);

                if (!ImageServerRelativeUrl.StartsWith("/_LAYOUTS", StringComparison.OrdinalIgnoreCase))
                {
                    var pageHeaderImage = this.clientContext.Web.GetFileByServerRelativePath(ResourcePath.FromDecodedUrl(ImageServerRelativeUrl));
                    this.clientContext.Load(pageHeaderImage, p => p.UniqueId, p => p.ListId);
                    this.clientContext.ExecuteQueryRetry();

                    this.listId = pageHeaderImage.ListId;
                    this.uniqueId = pageHeaderImage.UniqueId;
                }

                this.headerImageResolved = true;
            }
            catch (ServerException ex)
            {
                if (ex.ServerErrorTypeName == "System.IO.FileNotFoundException")
                {
                    // provided file link does not exist...we're eating the exception and the page will end up with a default page header
                    Log.Warning(Constants.LOGGING_SOURCE, CoreResources.ClientSidePageHeader_ImageNotFound, ImageServerRelativeUrl);
                }
                else if (ex.Message.Contains("SPWeb.ServerRelativeUrl"))
                {
                    // image resides in a different site collection context, we will simply allow it to be referred in the page header section.                    
                    Log.Warning(Constants.LOGGING_SOURCE, CoreResources.ClientSidePageHeader_ImageInDifferentWeb, imageServerRelativeUrl);
                    this.headerImageResolved = true;
                }
                else
                {
                    // the image can also refer to a path outside SharePoint, that is also allowed, so we will mark it as resolved and move ahead.
                    Log.Warning(Constants.LOGGING_SOURCE, CoreResources.ClientSidePageHeader_ImageInDifferentWeb, imageServerRelativeUrl);
                    this.headerImageResolved = true;
                    //throw;
                }
            }
            */
        }

    }
}