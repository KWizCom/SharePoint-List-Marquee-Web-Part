using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using codeplex.spsl;
using Microsoft.SharePoint.WebPartPages;
using System.Collections.Generic;
using System.Xml;

namespace KWizCom.SharePoint.ListMarquee.ListMarquee
{
	[ToolboxItemAttribute(false)]
	public class ListMarquee : System.Web.UI.WebControls.WebParts.WebPart
	{
		#region Members
		protected List<string> ErrorsCollection = new List<string>();
		Logging logger;
		bool shouldDisposeTheListWeb = false;
		#endregion

		#region Controls
		/// <summary>
		/// This control will hold the html of the marquee after transformation.
		/// </summary>
		LiteralControl XmlTransformHTML;
		Label errorsLabel;
		#endregion

		#region Properties
		private SPSite theListSite;
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
								theListSite = new SPSite(new Uri(Page.Request.Url, this.ListWebUrl, true).ToString());
								shouldDisposeTheListWeb = true;
								theListSite.CatchAccessDeniedException = false;
								this.theListWeb = theListSite.OpenWeb();
							}
							catch(Exception ex)
							{
								AddError(ex);
							}
						}

						if (this.theListWeb == null)
							this.theListWeb = SPContext.Current.Web;

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
					//this.theListViewItemFormUrlFormat = TheList.Forms[PAGETYPE.PAGE_DISPLAYFORM].ServerRelativeUrl + "?ID={0}&Source=" + Page.Request.Url.ToString();
					this.theListViewItemFormUrlFormat = TheList.ParentWeb.ServerRelativeUrl.TrimEnd('/') + "/_layouts/listform.aspx?PageType=4&ID={0}&ListId="+TheList.ID.ToString()+"&Source=" + Page.Request.Url.ToString();

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
					//this.theListEditItemFormUrlFormat = TheList.Forms[PAGETYPE.PAGE_EDITFORM].Url + "?ID={0}&Source=" + Page.Request.Url.ToString();
					this.theListViewItemFormUrlFormat = TheList.ParentWeb.ServerRelativeUrl.TrimEnd('/') + "/_layouts/listform.aspx?PageType=6&ID={0}&ListId=" + TheList.ID.ToString() + "&Source=" + Page.Request.Url.ToString();

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
					//this.theListNewItemFormUrl = TheList.Forms[PAGETYPE.PAGE_NEWFORM].Url + "?Source=" + Page.Request.Url.ToString();
					this.theListViewItemFormUrlFormat = TheList.ParentWeb.ServerRelativeUrl.TrimEnd('/') + "/_layouts/listform.aspx?PageType=8&ID={0}&ListId=" + TheList.ID.ToString() + "&Source=" + Page.Request.Url.ToString();

				return this.theListNewItemFormUrl;
			}
		}

		#endregion
		
		#region WebPart Proeprties

		#region Category: ListConnectionToolPart

		#region ListWebUrl
		private const string defaultListWebUrl = "";
		private string listWebUrl = defaultListWebUrl;
#if EditorParts
		[WebBrowsable(false), Personalizable(PersonalizationScope.User)]
#else
		[WebBrowsable(true),
		Personalizable(PersonalizationScope.User),
		SPWebCategoryName("List Connection v" + Constants.Version),
		WebDisplayName("List web url"),
		WebDescription("Url to the web site containing the list")]
#endif
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
		private const string defaultListName = "Announcements";
		private string listName = defaultListName;
#if EditorParts
		[WebBrowsable(false), Personalizable(PersonalizationScope.User)]
#else
		[WebBrowsable(true),
		Personalizable(PersonalizationScope.User),
		SPWebCategoryName("List Connection v" + Constants.Version),
		WebDisplayName("List name"),
		WebDescription("The list name to connect to")]
#endif
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
#if EditorParts
		[WebBrowsable(false), Personalizable(PersonalizationScope.User)]
#else
		[WebBrowsable(true),
		Personalizable(PersonalizationScope.User),
		SPWebCategoryName("List Connection v" + Constants.Version),
		WebDisplayName("List view name"),
		WebDescription("The list view name to connect to. Leave empty to use the default view.")]
#endif
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

		#region Marquee

		#region MarqueeAmount
		private const int defaultMarqueeAmount = 1;
		private int marqueeAmount = defaultMarqueeAmount;
#if EditorParts
		[WebBrowsable(false), Personalizable(PersonalizationScope.User)]
#else
		[WebBrowsable(true),
		Personalizable(PersonalizationScope.User),
		SPWebCategoryName("Marquee settings v" + Constants.Version),
		WebDisplayName("Marquee jump pixle"),
		WebDescription("The number of pixles to jupt each step. Use lower values for smoother experience.")]
#endif
		public int MarqueeAmount
		{
			get
			{
				return marqueeAmount;
			}

			set
			{
				marqueeAmount = value;
			}
		}
		public bool ShouldSerializeMarqueeAmount()
		{
			if (this.marqueeAmount == defaultMarqueeAmount)
				return false;
			return true;
		}

		#endregion

		#region MarqueeDelay
		private const int defaultMarqueeDelay = 50;
		private int marqueeDelay = defaultMarqueeDelay;
#if EditorParts
		[WebBrowsable(false), Personalizable(PersonalizationScope.User)]
#else
		[WebBrowsable(true),
		Personalizable(PersonalizationScope.User),
		SPWebCategoryName("Marquee settings v" + Constants.Version),
		WebDisplayName("Marquee jump delay"),
		WebDescription("The delay between jumps. Use lower values for slower scrolling.")]
#endif
		public int MarqueeDelay
		{
			get
			{
				return marqueeDelay;
			}

			set
			{
				marqueeDelay = value;
			}
		}
		public bool ShouldSerializeMarqueeDelay()
		{
			if (this.marqueeDelay == defaultMarqueeDelay)
				return false;
			return true;
		}

		#endregion

		#region MarqueeDirection
		private const MarqueeDirections defaultMarqueeDirection = MarqueeDirections.up;
		private MarqueeDirections marqueeDirection = defaultMarqueeDirection;
#if EditorParts
		[WebBrowsable(false), Personalizable(PersonalizationScope.User)]
#else
		[WebBrowsable(true),
		Personalizable(PersonalizationScope.User),
		SPWebCategoryName("Marquee settings v" + Constants.Version),
		WebDisplayName("Marquee scrolling direction"),
		WebDescription("The direction content will scroll in the mequee")]
#endif
		public MarqueeDirections MarqueeDirection
		{
			get
			{
				return marqueeDirection;
			}

			set
			{
				marqueeDirection = value;
			}
		}
		public bool ShouldSerializeMarqueeDirection()
		{
			if (this.marqueeDirection == defaultMarqueeDirection)
				return false;
			return true;
		}

		#endregion

		#region LinkTarget
		private const LinkTargets defaultLinkTarget = LinkTargets._self;
		private LinkTargets linkTarget = defaultLinkTarget;
#if EditorParts
		[WebBrowsable(false), Personalizable(PersonalizationScope.User)]
#else
		[WebBrowsable(true),
		Personalizable(PersonalizationScope.User),
		SPWebCategoryName("Marquee settings v" + Constants.Version),
		WebDisplayName("Title link target"),
		WebDescription("The target for the title link. _self will open the form in a popup.")]
#endif
		public LinkTargets LinkTarget
		{
			get
			{
				return linkTarget;
			}

			set
			{
				linkTarget = value;
			}
		}
		public bool ShouldSerializeLinkTarget()
		{
			if (this.linkTarget == defaultLinkTarget)
				return false;
			return true;
		}

		#endregion

		#endregion

		#region List Definitions

		#region TitleFieldName
		private const string defaultTitleFieldName = "Title";
		private string titleFieldName = defaultTitleFieldName;
#if EditorParts
		[WebBrowsable(false), Personalizable(PersonalizationScope.User)]
#else
		[WebBrowsable(true),
		Personalizable(PersonalizationScope.User),
		SPWebCategoryName("List Definitions v" + Constants.Version),
		WebDisplayName("Title field name"),
		WebDescription("The name of the field to use as the title for each scrolling item.")]
#endif
		public string TitleFieldName
		{
			get
			{
				return titleFieldName;
			}

			set
			{
				titleFieldName = value;
			}
		}
		public bool ShouldSerializeTitleFieldName()
		{
			if (this.titleFieldName == defaultTitleFieldName)
				return false;
			return true;
		}

		#endregion

		#region BodyFieldName
		private const string defaultBodyFieldName = "Body";
		private string bodyFieldName = defaultBodyFieldName;
#if EditorParts
		[WebBrowsable(false), Personalizable(PersonalizationScope.User)]
#else
		[WebBrowsable(true),
		Personalizable(PersonalizationScope.User),
		SPWebCategoryName("List Definitions v" + Constants.Version),
		WebDisplayName("Content field name"),
		WebDescription("The name of the field to use as the content for each scrolling item.")]
#endif
		public string BodyFieldName
		{
			get
			{
				return bodyFieldName;
			}

			set
			{
				bodyFieldName = value;
			}
		}
		public bool ShouldSerializeBodyFieldName()
		{
			if (this.bodyFieldName == defaultBodyFieldName)
				return false;
			return true;
		}

		#endregion

		#endregion

		#region Category: Misc

		#region ShowErrors
		private const bool defaultShowErrors = false;
		private bool showErrors = defaultShowErrors;
		[WebBrowsable(true),
		Personalizable(PersonalizationScope.User),
		SPWebCategoryName("Misc"),
		WebDisplayName("Show Errors"),
		WebDescription("Check this to for debug purposses")]
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

		#region XSLUrl
		private const string defaultXSLUrl = "";
		private string xSLUrl = defaultXSLUrl;
		[WebBrowsable(true),
		Personalizable(PersonalizationScope.User),
		SPWebCategoryName("Misc"),
		WebDisplayName("Alternate XSL file url (optional)"),
		WebDescription("You can change the way this web part renders by having different XSL files to control the layout.")]
		public string XSLUrl
		{
			get
			{
				return xSLUrl;
			}

			set
			{
				xSLUrl = value;
			}
		}
		#endregion

		#endregion

		#endregion
		
		#region Ctor
		public ListMarquee()
		{
			try
			{
				logger = new Logging(this.GetType());
				this.PreRender += new EventHandler(BaseListWebPart_PreRender);
			}
			catch(Exception ex)
			{
				AddError(ex);
			}
		}
		#endregion
		
		#region Implementation (overrides)
#if EditorParts
		public override EditorPartCollection CreateEditorParts()
		{
			try
			{
				List<EditorPart> parts = new List<EditorPart>();
				parts.Add(new ListConnectionToolPart());
				parts.Add(new MarqueeToolPart());
				foreach (EditorPart part in base.CreateEditorParts())
					parts.Add(part);

				return new EditorPartCollection(parts);
			}
			catch (Exception ex) { logger.LogError(ex); }

			//if error occured - return empty
			return EditorPartCollection.Empty;
		}
#endif

		protected override void CreateChildControls()
		{
			XmlTransformHTML = new LiteralControl();
			this.Controls.Add(XmlTransformHTML);

			errorsLabel = new Label();
			errorsLabel.Visible = false;
			errorsLabel.EnableViewState = false;
			this.Controls.Add(errorsLabel);
		}

		public override void Dispose()
		{
			if (shouldDisposeTheListWeb)
			{
				if (this.theListWeb != null)
				{
					this.theListWeb.Dispose();
					this.theListWeb = null;
				}
				if (this.theListSite != null)
				{
					this.theListSite.Dispose();
					this.theListSite = null;
				}
			}
			base.Dispose();
		}
		#endregion
		
		#region Event Handlers

		private void BaseListWebPart_PreRender(object sender, EventArgs e)
		{
			try
			{
				this.EnsureChildControls();

				//Create XML Document that will hold list items in XML suitable for marquee XSL
				XmlDocument xDoc = new XmlDocument();
				XmlElement elmItems = xDoc.CreateElement("Items");
				xDoc.AppendChild(elmItems);

				XmlElement elmItem;

				//Add the Title and Body fields here to make sure they're values return regardless of
				//selected view's ViewFields definitions.
				//This solves a case when the selected view does not return the fields selected for the marquee.
				string viewFields = string.Format("<FieldRef Name='{0}'/><FieldRef Name='{1}'/>", this.TitleFieldName, this.BodyFieldName);

				//If nothing was defined by the user so far - connect to Announcements list.
				if (this.ListName.Trim() == string.Empty) this.ListName = "Announcements";

				//Add each item from the selected view to the XML
				foreach (SPListItem itm in this.GetListViewItems(viewFields))
				{
					try
					{
						elmItem = xDoc.CreateElement("Item");

						string title = itm[this.TitleFieldName] == null ? "No title" : itm[this.TitleFieldName].ToString();
						string body = itm[this.BodyFieldName] == null ? "No body" : itm[this.BodyFieldName].ToString();

						elmItem.SetAttribute("Title", title);
						elmItem.SetAttribute("Body", body);
						elmItem.SetAttribute("ViewItemUrl", string.Format(this.TheListViewItemFormUrlFormat, itm.ID));

						elmItems.AppendChild(elmItem);
					}
					catch { }
				}

				//XSL parameters and transform. The return HTML is inserted to the literal control that writes it
				//to the page.
				System.Xml.Xsl.XsltArgumentList arguments = new System.Xml.Xsl.XsltArgumentList();
				arguments.AddParam("MarqueeAmount", "", this.MarqueeAmount);
				arguments.AddParam("MarqueeDelay", "", this.MarqueeDelay);
				arguments.AddParam("MarqueeDirection", "", this.MarqueeDirection.ToString());
				arguments.AddParam("LinkTarget", "", this.LinkTarget.ToString());
				XmlTransformHTML.Text = Utilities.XslTransform(xDoc, arguments, getXSLFileLocation());
			}
			catch (Exception ex)
			{
				this.AddError(ex);
			}

			try
			{
				RenderErrors();
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
			this.ErrorsCollection.Add(ex.ToString().Replace("\n", "<BR>"));
			logger.LogError(ex);
		}
		/// <summary>
		/// Trap a custom-text error.
		/// </summary>
		/// <param name="error">Error</param>
		protected void AddError(string error)
		{
			this.ErrorsCollection.Add(error);
			logger.LogError(error);
		}
		/// <summary>
		/// Draws the errors that were trapped using the AddError into the UI.
		/// If the user did not select the ShowErrors property nothing will be displayed.
		/// </summary>
		protected void RenderErrors()
		{
			if (this.ShowErrors && ErrorsCollection.Count > 0)
			{
				foreach( string error in ErrorsCollection )
				{
					errorsLabel.Text += error + "<br />";
				}
				errorsLabel.Visible = true;
			}
			else
				errorsLabel.Visible = false;
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

		string getXSLFileLocation()
		{
			string fileUrl = (this.XSLUrl.Equals(string.Empty)) ?
				SPContext.Current.Site.ServerRelativeUrl.TrimEnd('/') + "/SiteAssets/KWizCom/ListMarqueeSandbox/Marquee.xslt" :
				this.XSLUrl;

			return new Uri(Page.Request.Url, fileUrl, true).ToString();
		}

		#endregion

	}
}
