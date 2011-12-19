using System;
using System.ComponentModel;
using System.Web.UI;
using System.Collections;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebPartPages;

namespace KWizCom.SharePoint.WebParts
{
	[	ToolboxData("<{0}:BaseListWebPart runat=server></{0}:BaseListWebPart>"),
		XmlRoot(Namespace="KWizCom.SharePoint.WebParts")]
	public class BaseListWebPart : Microsoft.SharePoint.WebPartPages.WebPart
	{
		#region Members
		protected ArrayList ErrorsCollection = new ArrayList();
		#endregion

		#region Controls
		#endregion

		#region Properties
		private SPWeb theListWeb;
		/// <summary>
		/// The Selected List SPWeb Object
		/// Note: This points to the current web site if ListWebUrl is empty
		/// </summary>
		protected SPWeb TheListWeb
		{
			get
			{
				if( this.theListWeb == null )
				{
					try
					{
						if( this.ListWebUrl.Trim() != string.Empty )
						{
							try
							{
								SPSite st = new SPSite( new Uri(Page.Request.Url, this.ListWebUrl, true).ToString() );
								st.CatchAccessDeniedException = false;
								this.theListWeb = st.OpenWeb();
							}
							catch(Exception ex)
							{
								AddError(ex);
							}
						}

						if( this.theListWeb == null )
							this.theListWeb = SPControl.GetContextWeb(Context);

						this.theListWeb.AllowUnsafeUpdates = true;
						this.theListWeb.Lists.IncludeRootFolder = true;
						this.theListWeb.Lists.ListsForCurrentUser = true;
					}
					catch(Exception ex)
					{
						AddError(ex);
					}

				}

				return this.theListWeb;
			}
		}


		private SPList theList;
		/// <summary>
		/// The Selected List SPList Object
		/// </summary>
		protected SPList TheList
		{
			get
			{
				if( this.theList == null )
				{
					try
					{
						this.theList = this.TheListWeb.Lists[this.ListName];
					}
					catch(Exception ex)
					{
						AddError(ex);
					}
				}

				return this.theList;
			}
		}
		

		/// <summary>
		/// The Selected List View SPView Object
		/// Note: This points to the default view if ListViewName is empty
		/// </summary>
		private SPView theListView;
		protected SPView TheListView
		{
			get
			{
				if( this.theListView == null )
				{
					try
					{
						if( this.ListViewName.Trim() != string.Empty )
						{
							try
							{
								this.theListView = this.TheList.Views[this.ListViewName];
							}
							catch(Exception ex)
							{
								AddError(ex);
							}
						}

						if( this.theListView == null )
							this.theListView = this.TheList.DefaultView;
					}
					catch(Exception ex)
					{
						AddError(ex);
					}
				}

				return this.theListView;
			}
		}
		

		private string theListViewItemFormUrlFormat = null;
		/// <summary>
		/// View Item Format Link, use string.Format and pass the items's ID.
		/// </summary>
		protected string TheListViewItemFormUrlFormat
		{
			get
			{
				if( theListViewItemFormUrlFormat == null )
					this.theListViewItemFormUrlFormat = TheList.Forms[PAGETYPE.PAGE_DISPLAYFORM].Url + "?ID={0}&Source=" + Page.Request.Url.ToString();

				return this.theListViewItemFormUrlFormat;
			}
		}


		private string theListEditItemFormUrlFormat = null;
		/// <summary>
		/// Edit Item Format Link, use string.Format and pass the items's ID.
		/// </summary>
		protected string TheListEditItemFormUrlFormat
		{
			get
			{
				if( this.theListEditItemFormUrlFormat == null )
					this.theListEditItemFormUrlFormat = TheList.Forms[PAGETYPE.PAGE_EDITFORM].Url + "?ID={0}&Source=" + Page.Request.Url.ToString();

				return this.theListEditItemFormUrlFormat;
			}
		}


		private string theListNewItemFormUrl = null;
		/// <summary>
		/// New Item Link.
		/// </summary>
		protected string TheListNewItemFormUrl
		{
			get
			{
				if( this.theListNewItemFormUrl == null )
					this.theListNewItemFormUrl = TheList.Forms[PAGETYPE.PAGE_NEWFORM].Url + "?Source=" + Page.Request.Url.ToString();

				return this.theListNewItemFormUrl;
			}
		}


		#endregion
		
		#region WebPart Proeprties

		#region Category: ListConnectionToolPart

		#region ListWebUrl
		private const string defaultListWebUrl = "";
		private string listWebUrl = defaultListWebUrl;
		[Browsable(false),
		DefaultValue(defaultListWebUrl),
		WebPartStorage(Storage.Personal)]
		public string ListWebUrl
		{
			get
			{
				return listWebUrl;
			}

			set
			{
				listWebUrl = value;
			}
		}
		public bool ShouldSerializeListWebUrl()
		{
            if (this.listWebUrl == defaultListWebUrl)
                return false;
            return true;
		}
		#endregion

		#region ListName
		private const string defaultListName = "";
		private string listName = defaultListName;
		[Browsable(false),
		DefaultValue(defaultListName),
		WebPartStorage(Storage.Personal)]
		public string ListName
		{
			get
			{
				return listName;
			}

			set
			{
				listName = value;
			}
		}
		public bool ShouldSerializeListName()
		{
            if (this.listName == defaultListName)
                return false;
            return true;
        }
		#endregion

		#region ListViewName
		private const string defaultListViewName = "";
		private string listViewName = defaultListViewName;
		[Browsable(false),
		DefaultValue(defaultListViewName),
		WebPartStorage(Storage.Personal)]
		public string ListViewName
		{
			get
			{
				return listViewName;
			}

			set
			{
				listViewName = value;
			}
		}
		public bool ShouldSerializeListViewName()
		{
            if (this.listViewName == defaultListViewName)
                return false;
            return true;
        }
		#endregion

		#endregion

		#region Misc

		#region ShowErrors
		private const bool defaultShowErrors = false;
		private bool showErrors = defaultShowErrors;
		[Browsable(true),
		Category("Misc"),
		DefaultValue(defaultShowErrors),
		WebPartStorage(Storage.Personal),
		FriendlyName("Show Errors"),
		Description("Check this to for debug purposses")]
		public bool ShowErrors
		{
			get
			{
				return showErrors;
			}

			set
			{
				showErrors = value;
			}
		}

		#endregion

		#endregion

		#endregion
		
		#region Ctor
		public BaseListWebPart()
		{
			try
			{
				this.Load += new EventHandler(BaseListWebPart_Load);
				this.PreRender += new EventHandler(BaseListWebPart_PreRender);
			}
			catch(Exception ex)
			{
				AddError(ex);
			}
		}
		#endregion
		
		#region Implementation (overrides)
		public override void CreateWebPartMenu()
		{
			Microsoft.SharePoint.WebPartPages.MenuItem KWizComOnlineMenuItem = new Microsoft.SharePoint.WebPartPages.MenuItem("More from KWizCom...", "window.open('http://www.KWizCom.com');");
			this.WebPartMenu.MenuItems.Add(KWizComOnlineMenuItem);
		}

		/// <summary>
		/// Add a ListConnectionToolPart to enable users to easily select data sources.
		/// </summary>
		/// <returns></returns>
		public override ToolPart[] GetToolParts()
		{
			try
			{
				ToolPart[] toolparts = new ToolPart[3];
				ListConnectionToolPart listConnectionToolPart = new ListConnectionToolPart();

				CustomPropertyToolPart custom = new CustomPropertyToolPart();
				WebPartToolPart wptp = new WebPartToolPart();
				
				toolparts[0] = listConnectionToolPart;
				toolparts[1] = wptp;
				toolparts[2] = custom;
				return toolparts;
			}
			catch(Exception ex)
			{AddError(ex);}
			return null;
		}

		/// <summary>
		/// Render this Web Part to the output parameter specified.
		/// </summary>
		/// <param name="output"> The HTML writer to write out to </param>
		protected override void RenderWebPart(HtmlTextWriter output)
		{
			try
			{
			}
			catch(Exception ex)
			{
				AddError(ex);
			}
		}
		#endregion
		
		#region Event Handlers
		private void BaseListWebPart_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetResourceRequest();
			}
			catch(Exception ex)
			{
				AddError(ex);
			}
		}

		private void BaseListWebPart_PreRender(object sender, EventArgs e)
		{
			try
			{
			}
			catch(Exception ex)
			{
				AddError(ex);
			}
		}
		#endregion
		
		#region Other
		/// <summary>
		/// Trap an exception error.
		/// </summary>
		/// <param name="ex">Error</param>
		protected void AddError(Exception ex)
		{
			AddError(ex.ToString().Replace("\n","<BR>"));
		}
		/// <summary>
		/// Trap a custom-text error.
		/// </summary>
		/// <param name="error">Error</param>
		protected void AddError(string error)

		{
			this.ErrorsCollection.Add(error);
		}
		/// <summary>
		/// Draws the errors that were trapped using the AddError into the UI.
		/// If the user did not select the ShowErrors property nothing will be displayed.
		/// </summary>
		/// <param name="output"></param>
		protected void RenderErrors(HtmlTextWriter output)
		{
			if( this.ShowErrors )
			{
				foreach( string error in ErrorsCollection )
				{
					output.Write(error);
				}
			}
		}

		/// <summary>
		/// Get All Items From The Selected List
		/// </summary>
		/// <returns>List Items</returns>
		protected SPListItemCollection GetListItems()
		{
			try
			{
				return this.TheList.Items;
			}
			catch(Exception ex)
			{
				this.AddError(ex);
			}

			return null;
		}
		/// <summary>
		/// Get List Items According to the selected view definitions
		/// </summary>
		/// <returns>List View Items</returns>
		protected SPListItemCollection GetListViewItems()
		{
			return GetListViewItems(string.Empty);
		}
		/// <summary>
		/// Get List Items According to the selected view definitions.
		/// Allows overriding the retrieved fields collection.
		/// </summary>
		/// <param name="viewFields">ViewFields in CAML format</param>
		/// <returns>List View Items</returns>
		protected SPListItemCollection GetListViewItems(string viewFields)
		{
			try
			{
				SPQuery query = new SPQuery( this.TheListView );
				if( viewFields != null && viewFields.Trim() != string.Empty )
					query.ViewFields = viewFields;

				return this.TheList.GetItems( query );
			}
			catch(Exception ex)
			{
				this.AddError(ex);
			}

			return null;
		}

		#region resources
		/// <summary>
		/// Resource caching
		/// </summary>
		const string ResponseContentType_Gif = "Content-Type: image/gif";
		const string ResourceGifExtention = ".gif";
		private static Hashtable htResources = new Hashtable();
		private void GetResourceRequest()
		{
			try
			{
				//Load Resources from htResources hashtable (gif js css)
				string requestedResource = Page.Request.QueryString["BaseListWebPart"];
				if( requestedResource != null && requestedResource != string.Empty )
				{
					//Add to cache
					if( !htResources.ContainsKey(requestedResource) ||
						htResources[requestedResource] != null )
					{
						//get resource stream into string
						System.IO.Stream strm = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("KWizCom.SharePoint.WebParts." + requestedResource.ToLower());
						byte[] strmBytes = new byte[strm.Length];
						strm.Read( strmBytes, 0, (int)strm.Length );

						htResources[requestedResource] = strmBytes;
					}

					if( requestedResource.ToLower().EndsWith(ResourceGifExtention) )
					{
						Page.Response.ContentType = ResponseContentType_Gif;
					}

					Page.Response.Clear();
					Page.Response.BinaryWrite( (byte[])htResources[requestedResource] );
					Page.Response.End();
				}
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.Write(ex.ToString());
			}
		}
		#endregion

		#endregion
	}
}
