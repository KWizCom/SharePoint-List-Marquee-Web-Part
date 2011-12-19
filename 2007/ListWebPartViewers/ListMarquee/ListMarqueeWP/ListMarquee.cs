using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebPartPages;

namespace KWizCom.SharePoint.WebParts.ListMarquee
{
	[	ToolboxData("<{0}:ListMarquee runat=server></{0}:ListMarquee>"),
		XmlRoot(Namespace="http://www.KWizCom.com/ListMarquee") ]
	public class ListMarquee : BaseListWebPart
	{
		#region members
		#endregion

		#region controls
		/// <summary>
		/// This control will hold the html of the marquee after transformation.
		/// </summary>
		LiteralControl XmlTransformHTML;
		#endregion
		
		#region properties

		#region Marquee

		#region MarqueeAmount
		private const int defaultMarqueeAmount = 1;
		private int marqueeAmount = defaultMarqueeAmount;
		[Browsable(false),
//		Category("Marquee"),
		DefaultValue(defaultMarqueeAmount),
		WebPartStorage(Storage.Personal),
//		FriendlyName("Marquee Amount"),
//		Description("Marquee Amount")
		]
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
		[Browsable(false),
//		Category("Marquee"),
		DefaultValue(defaultMarqueeDelay),
		WebPartStorage(Storage.Personal),
//		FriendlyName("Marquee Delay"),
//		Description("Marquee Delay")
		]
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
		[Browsable(false),
//		Category("Marquee"),
		DefaultValue(defaultMarqueeDirection),
		WebPartStorage(Storage.Personal),
//		FriendlyName("Marquee Direction"),
//		Description("Marquee Direction")
		]
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
		[Browsable(false),
//		Category("Marquee"),
		DefaultValue(defaultLinkTarget),
		WebPartStorage(Storage.Personal),
//		FriendlyName("Link target"),
//		Description("Use '_self' to open links in same window, '_blank' to open items in new window etc...")
		]
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

		#region XSLUrl
		private const string defaultXSLUrl = "";
		private string xSLUrl = defaultXSLUrl;
		[Browsable(true),
		Category("Misc"),
		DefaultValue(defaultXSLUrl),
		WebPartStorage(Storage.Personal),
		FriendlyName("Alternate XSL file url (optional)"),
		Description("You can change the way this web part renders by having different XSL files to control the layout.")
		]
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

		#region List Definitions

		#region TitleFieldName
		private const string defaultTitleFieldName = "Title";
		private string titleFieldName = defaultTitleFieldName;
		[Browsable(false),
//		Category("List Definitions"),
		DefaultValue(defaultTitleFieldName),
		WebPartStorage(Storage.Personal),
//		FriendlyName("Title Field Name"),
//		Description("Title Field Name")
		]
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
		[Browsable(false),
//		Category("List Definitions"),
		DefaultValue(defaultBodyFieldName),
		WebPartStorage(Storage.Personal),
//		FriendlyName("Body Field Name"),
//		Description("Body Field Name")
		]
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

		#endregion

		#region Ctor
		public ListMarquee()
		{
			this.PreRender += new EventHandler(ListMarquee_PreRender);
		}
		#endregion

		#region Implementation ( overrides )

		/// <summary>
		/// Add a ListConnectionToolPart to enable users to easily select data sources.
		/// </summary>
		/// <returns></returns>
		public override ToolPart[] GetToolParts()
		{
			try
			{
				ToolPart[] toolparts = new ToolPart[4];
				ListConnectionToolPart listConnectionToolPart = new ListConnectionToolPart();
				MarqueeToolPart marqueeToolPart = new MarqueeToolPart();

				CustomPropertyToolPart custom = new CustomPropertyToolPart();
				WebPartToolPart wptp = new WebPartToolPart();
				
				toolparts[0] = listConnectionToolPart;
				toolparts[1] = marqueeToolPart;
				toolparts[2] = wptp;
				toolparts[3] = custom;
				return toolparts;
			}
			catch(Exception ex)
			{AddError(ex);}
			return null;
		}


		protected override void CreateChildControls()
		{
			XmlTransformHTML = new LiteralControl();
			this.Controls.Add(XmlTransformHTML);
		}

		protected override void RenderWebPart(HtmlTextWriter output)
		{
			try
			{
				this.EnsureChildControls();
				this.RenderChildren(output);

				//If user selected to show errors (base web part property)
				//this writes to screen all the errors that were trapped by base.AddError
				base.RenderErrors(output);
			}
			catch(Exception ex)
			{
				//Too late for trapping error... no one will display it now
				//Trace.Write it.
				Page.Trace.Write("Error", ex.ToString());
			}
		}
		#endregion

		#region Event Handlers
		private void ListMarquee_PreRender(object sender, EventArgs e)
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
				string viewFields = string.Format( "<FieldRef Name='{0}'/><FieldRef Name='{1}'/>",this.TitleFieldName, this.BodyFieldName );

				//If nothing was defined by the user so far - connect to Announcements list.
				if( base.ListName.Trim() == string.Empty ) base.ListName = "Announcements";

				//Add each item from the selected view to the XML
				foreach( SPListItem itm in base.GetListViewItems(viewFields) )
				{
					elmItem = xDoc.CreateElement("Item");
					elmItems.AppendChild(elmItem);

					string title = itm[this.TitleFieldName] == null ? "No title" : itm[this.TitleFieldName].ToString();
					string body = itm[this.BodyFieldName] == null ? "No body" : itm[this.BodyFieldName].ToString();

					elmItem.SetAttribute("Title", title );
					elmItem.SetAttribute("Body", body );
					elmItem.SetAttribute("ViewItemUrl", string.Format(base.TheListViewItemFormUrlFormat,itm.ID) );
				}

				//XSL parameters and transform. The return HTML is inserted to the literal control that writes it
				//to the page.
				System.Xml.Xsl.XsltArgumentList arguments = new System.Xml.Xsl.XsltArgumentList();
				arguments.AddParam("MarqueeAmount","",this.MarqueeAmount);
				arguments.AddParam("MarqueeDelay","",this.MarqueeDelay);
				arguments.AddParam("MarqueeDirection","",this.MarqueeDirection.ToString());
				arguments.AddParam("LinkTarget","",this.LinkTarget.ToString());
				XmlTransformHTML.Text = Utilities.XslTransform(Page, xDoc,arguments, getXSLFileLocation());
			}
			catch(Exception ex)
			{
				base.AddError(ex);
			}
		}

		string getXSLFileLocation()
		{
			if( this.XSLUrl.Equals(string.Empty) )
				return this.ClassResourcePath + "/Marquee.xslt";

			return new Uri(Page.Request.Url,this.XSLUrl,true).ToString();
		}
		#endregion

	}
}
