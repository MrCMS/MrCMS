# Mr CMS

**Mr CMS is an ASP.NET MVC 5 C# open source content management framework**

Mr CMS wraps up a lot of the time consuming aspects of webside creation. Mr CMS is a framework for developers create bespoke website whilst at the same time giving content editors an easy to use CMS.

## Apps
Apps in Mr CMS contain all the functionality your website requires. For example, you might want a Blog App which contains page definitions for a list of blogs and a blog page itself. 

Mr CMS comes with 3 basic apps, 'Core', 'Articles' and 'Galleries'. The CoreApp contains basic concepts such as login and registration. It also has some basic page types such as 'TextPage' which can be used as a base page type for other pages.

Ultimately though, you can throw these three apps out and start building your own if you want.

### Creating your first Mr CMS App
The first step in creating an app is to create a folder in the Apps folder. For example, 'Blog'. In here create a file called BlogApp.cs.

	public class BlogApp : MrCMSApp
    {
        public override string AppName
        {
            get { return "Blog"; }
        }

        public override string Version
        {
            get { return "0.1"; }
        }

		protected override void RegisterServices(IKernel kernel)
        {
            
       }

        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
        }
    }

This registers your App with Mr CMS. Note that your App definition must inherit from MrCMSApp.

Next create a folder called 'Apps\BlogApp\Pages' and create a file called Blog.cs

	public class Blog : Webpage
    {
		[DisplayName("Featured Image")]
        public virtual string FeatureImage { get; set; }

        [AllowHtml]
        [StringLength(160, ErrorMessage = "Abstract cannot be longer than 500 characters.")]
        public virtual string Abstract { get; set; }
    }

Note in this instance we are inheriting from Webpage. This tells Mr CMS that this page can be added to the CMS. Webpage has a lot of properties which are shared across all webpages, such as Page Tile and Meta Description. It also has BodyContent for the main body of text of any webpage.

Finally create a folder called Views\Pages and within that folder add a page to display the Blog.

	<article>
		<div class="row">
			<div class="col-md-12">
				<h1 class="margin-top-0">@Editable(Model, p => p.Name, false)</h1>
				<a href="/@Model.Parent.UrlSegment" class="btn btn-default">Back</a>
				@Model.CreatedOn.Day @Model.CreatedOn.ToString("MMMMM") @Model.CreatedOn.Year
				<br />
				@if (!String.IsNullOrEmpty(Model.FeatureImage))
				{
					<a href="/@Model.LiveUrlSegment" class="margin-top-0">@RenderImage(Model.FeatureImage)</a>
				}
				@Editable(Model, page => page.BodyContent, true)
			</div>
		</div>
	</article>

Note here we use @Editable - this allows inline editing of content. Passing in True/False argument will enable or disable HTML editing.

Finally you need to add a page in Apps\BlogApp\Areas\Admin\Views\Webpage\ to add the additional editable fields we defined earlier.

	<div class="form-group">
		@Html.LabelFor(model => model.FeatureImage, "Article Image") <br />
		@Html.TextBoxFor(model => model.FeatureImage, new { data_type = "media-selector" })
	</div>
	<div class="form-group">
		@Html.LabelFor(model => model.Abstract)
		@Html.TextAreaFor(model => model.Abstract, new { @rows = "2", @class = "form-control" })
	</div>

This is the page Mr CMS uses to edit your custom page fields.

At this point you now have a new page type which can be added to Mr CMS. You can have a lot more control over your pages and how they act by using DocumentMetadataMap<> - this class allows you to specify page behaviour, for example if you have custom Controller you'd like to use rather than the standard Mr CMS one you can specify that here.

The Mr CMS ECommerce App listed [on GitHub](https://github.com/MrCMS/Ecommerce) has a lot more functionality to it that just a simple page, so if you'd like to see how more complicated apps are built check this project out.

## Feature list

*   Unlimited document types
*   Lucene based search architecture - easily create search indexes for super fast content filtering and searching of '000's of items.
*   Layouts & Layout areas give complete control over widgets on each page
*   Bulk media upload and management
*   Azure or file based media storage
*   Inline content editing
*   Error logging
*   Task management
*   SEO - Control over meta details
*   URL History table - keep track of URL changes. When a URL changes create 301 redirect to new location
*   URL History - add URLs at will to create redirects or if importing from another system add in the original URLs here
*   Form builder - create forms on the fly and collect their data
*   Enforce user login to page
*   SSL per page
*   User management & roles
*   Document version control
*   Complete control of page meta data - I.E when creating a page type (e.g BlogPage) control what type of pages can be added below it, say weather the page can maintain url hierarchy, say how many children should show in the page tree in item (if there are 000's of pages you might only show the top 5 pages) and lots more
*   Content managed into apps
*   Themes - override standard views
*   Multi-site capability - run multiple domains through one set of files & admin system
*   ACL
*   Webpage import and export�
*   Site duplication button to duplicate a website quickly
*   Azure support for SQL, Caching and Lucene

## Release History

### 0.5.1 - February 2016

*   Better Azure Support - swapped out old Azure caching for Redis cache
*   Brought in the concept of a staging URL and staging Robots.txt - this allows us to use Azure Deployment Slots
*   Rewrite of the task execution system so that tasks are system wide rather than site specific. Tasks can now be enabled or disabled. This is a breaking change and tasks should be updated to disable site filters for any multisite websites.
*   Message queue now allows attachments
*   The Mr CMS admin logo can now be swapped out through System Settings - Site Settings
*   Removed Self Execute Tasks which uses HostingEnvironment.QueueBackgroundWorkItem due to unreliability of execution. Instead use external task system to poll the task executor URL.
*   Fixed: Empty writes to the universal search index
*   Fixed: Issue with the close icon on the inline widget editing
*   Fixed: 404s were returning 404 status but with a blank response. Fixed to return 404 HTML.


## 0.5.1 - September 2016
#### Required changes in upgrade from 0.5.0.*

*   Files are moved off disk for use with Azure websites scaling and deployment slots
*   Settings are migrated back to the database. Note: Remove unused .json files from App_Data/Settings. Settings will auto migrate but issues may occur if unused .json files are in the directory
*   Message templates moved back to database
*   Connection string now in ConnectionStrings.Config
*   Sitemaps now output to disk and are scheduled for rebuild via tasks
*   Favicons can be rendered at different sizes using  @Html.RenderFavicon(size: new Size(16, 16))
*   Better media management and uploading notifications
*   Azure probe settings for using traffic manager
*   Added more ACL rules for core functionality
*   Performance: Nhibernate configured for non-eager loading of entities to improve database performance
*   Performance: Limited URL lengths to 450 to allow for indexes to be applied in SQL Server
*   Performance: Whitespace filter now compiled 
*   Fixed: Daylight savings date/time issue
*   Breaking Change: Localised scripts now rendered separately to other scripts using @{ @Html.RenderLocalisedScripts();}
*   Breaking Change: Tasks now run at the System level and do not need to be set up on each website. 
*   Breaking Change: Mail settings are set at the system level rather than site level. Mail settings will need to be reconfigured
*   Breaking Change: System settings now stored in mrcms.config - the new location for hosting specific settings
*   Breaking Change: Remove Azure Directory for Lucene (We are slowly moving away from Lucene and looking at other solutions which work better with cloud based hosting and scaling)

