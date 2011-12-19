using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebPartPages;
using System.Collections;

namespace KWizCom.SharePoint.WebParts
{
	/// <summary>
	/// Description of the toolpart. Override the GetToolParts method in your WebPart
	/// class to invoke this toolpart. To establish a reference to the Web Part 
	/// the user has selected, use the ParentToolPane.SelectedWebPart property.
	/// </summary>
	public class ListConnectionToolPart: Microsoft.SharePoint.WebPartPages.ToolPart
	{
		#region members
		#endregion

		#region controls

		Image kWizComImage;

		/// <summary>
		/// text box for the site to look for lists in
		/// </summary>
		TextBox WebUrlTextBox;
		/// <summary>
		/// button to load all the lists from the site
		/// </summary>
		Button LoadListButton;
		/// <summary>
		/// combo with all lists in the site in WebUrlTextBox
		/// </summary>
		DropDownList AvailableListsDropDownList;
		/// <summary>
		/// combo with all views in the list
		/// </summary>
		DropDownList AvailableViewsDropDownList;
		#endregion

		#region Ctor
		/// <summary>
		/// Constructor for the class. A great place to set Set default values for
		/// additional base class properties here.
		/// <summary>
		public ListConnectionToolPart()
		{
			try
			{
				this.Title = "List Connection Tool Part";
				this.Load += new EventHandler(ListConnectionToolPart_Load);
				this.PreRender += new EventHandler(ListConnectionToolPart_PreRender);
			}
			catch(Exception ex){System.Diagnostics.Debug.Write(ex.ToString());}
		}
		#endregion

		#region overrides
		/// <summary>
		/// We have 2 sections: OWA and SPLists sources.
		/// Add controls to edit these sources.
		/// </summary>
		protected override void CreateChildControls()
		{
			try
			{
				kWizComImage = new Image();
				kWizComImage.BorderWidth = new Unit(0);
				kWizComImage.ToolTip = "KWizCom, Knowledge Worker Components";
				kWizComImage.AlternateText = "KWizCom, Knowledge Worker Components";
				kWizComImage.ImageUrl = SPContext.Current.Site.ServerRelativeUrl.TrimEnd('/') + "/SiteAssets/KWizCom/ListMarquee/logoKWizCom.gif";
				this.Controls.Add(kWizComImage);

				System.Web.UI.HtmlControls.HtmlGenericControl containerDiv = new System.Web.UI.HtmlControls.HtmlGenericControl("DIV");
				containerDiv.Attributes["class"] = "UserGeneric";
				this.Controls.Add(containerDiv);

				System.Web.UI.HtmlControls.HtmlGenericControl div;

				containerDiv.Controls.Add(
					GetNewHeaderDiv(LoadResource("ToolPart_Header_Text_SPLists"))
					);

				div = GetNewBodyDiv(LoadResource("ToolPart_Body_Text_SPListsUrl"));
				containerDiv.Controls.Add(div);

				WebUrlTextBox = new TextBox();
				WebUrlTextBox.ToolTip = LoadResource("ToolPart_Body_Text_SPListsUrl_ToolTip");
				WebUrlTextBox.CssClass = "UserInput";
				WebUrlTextBox.Style["width"] = "80%";
				LoadListButton = new Button();
				LoadListButton.CssClass = "ms-descriptiontext";
				LoadListButton.Text = LoadResource("ToolPart_Body_Text_SPListsUrlLoad");
				LoadListButton.ToolTip = LoadResource("ToolPart_Body_Text_SPListsUrlLoad_ToolTip");
				LoadListButton.Click += new EventHandler(LoadListButton_Click);
				div.Controls.Add(WrapInDiv(WebUrlTextBox, LoadListButton));

				div = GetNewBodyDiv(LoadResource("ToolPart_Body_Text_SPListsSelectList"));
				containerDiv.Controls.Add(div);

				AvailableListsDropDownList = new DropDownList();
				AvailableListsDropDownList.ID = "AvailableListsDropDownList";
				AvailableListsDropDownList.CssClass = "UserInput";
				AvailableListsDropDownList.AutoPostBack = true;
				AvailableListsDropDownList.SelectedIndexChanged += new EventHandler(AvailableListsDropDownList_SelectedIndexChanged);
				div.Controls.Add(WrapInDiv(AvailableListsDropDownList));

				div = GetNewBodyDiv(LoadResource("ToolPart_Body_Text_SPViewsSelectList"));
				containerDiv.Controls.Add(div);

				AvailableViewsDropDownList = new DropDownList();
				AvailableViewsDropDownList.ID = "AvailableViewsDropDownList";
				AvailableViewsDropDownList.CssClass = "UserInput";
				div.Controls.Add(WrapInDiv(AvailableViewsDropDownList));
			}
			catch(Exception ex){System.Diagnostics.Debug.Write(ex.ToString());}
		}

		System.Web.UI.HtmlControls.HtmlGenericControl GetNewHeaderDiv(string text)
		{
			System.Web.UI.HtmlControls.HtmlGenericControl div = new System.Web.UI.HtmlControls.HtmlGenericControl("DIV");
			div.Attributes["class"] = "UserSectionHead ms-bold";
			div.InnerText = text;
			return div;
		}
		System.Web.UI.HtmlControls.HtmlGenericControl GetNewBodyDiv(string text)
		{
			System.Web.UI.HtmlControls.HtmlGenericControl div = new System.Web.UI.HtmlControls.HtmlGenericControl("DIV");
			div.Attributes["class"] = "UserSectionBody";
			div.InnerText = text;
			return div;
		}

		System.Web.UI.HtmlControls.HtmlGenericControl WrapInDiv(Control ctrl)
		{
			System.Web.UI.HtmlControls.HtmlGenericControl div = new System.Web.UI.HtmlControls.HtmlGenericControl("DIV");
			div.Attributes["class"] = "UserControlGroup";
			div.Controls.Add(ctrl);
			return div;
		}
		System.Web.UI.HtmlControls.HtmlGenericControl WrapInDiv(Control ctrl1, Control ctrl2)
		{
			System.Web.UI.HtmlControls.HtmlGenericControl div = new System.Web.UI.HtmlControls.HtmlGenericControl("DIV");
			div.Attributes["class"] = "UserControlGroup";

			System.Web.UI.HtmlControls.HtmlGenericControl nobr = new System.Web.UI.HtmlControls.HtmlGenericControl("NOBR");

			nobr.Controls.Add(ctrl1);
			nobr.Controls.Add(new LiteralControl("&nbsp;"));
			nobr.Controls.Add(ctrl2);
			div.Controls.Add(nobr);

			return div;
		}

		///	<summary>
		///	Called by the tool pane to apply property changes to the selected Web Part. 
		///	</summary>
		public override void ApplyChanges()
		{
			try
			{
				EnsureChildControls();
				// apply property values here
				BaseListWebPart wp = this.ParentToolPane.SelectedWebPart as BaseListWebPart;

				wp.ListWebUrl = this.WebUrlTextBox.Text;
				wp.ListName = this.AvailableListsDropDownList.SelectedValue;
				wp.ListViewName = this.AvailableViewsDropDownList.SelectedValue;
			}
			catch(Exception ex){System.Diagnostics.Debug.Write(ex.ToString());}
		}

		/// <summary>
		///	If the ApplyChanges method succeeds, this method is called by the tool pane 
		///	to refresh the specified property values in the toolpart user interface.
		/// </summary>
		public override void SyncChanges()
		{
			// sync with the new property changes here
			try
			{
				this.LoadWebPartProperties(true);
			}
			catch(Exception ex){System.Diagnostics.Debug.Write(ex.ToString());}
		}
		
		/// <summary>
		///	Called by the tool pane if the user discards changes to the selected Web Part. 
		/// </summary>
		public override void CancelChanges()
		{
			try
			{
				this.EnsureChildControls();
				//Get rid of view state!!!
				ViewState["ListConnectionToolPartLoaded" + this.Qualifier] = false;
			}
			catch(Exception ex){System.Diagnostics.Debug.Write(ex.ToString());}
		}
		/// <summary>
		/// Render this Tool part to the output parameter specified.
		/// </summary>
		/// <param name="output"> The HTML writer to write out to </param>
		protected override void RenderToolPart(HtmlTextWriter output)
		{
			try
			{
				if( AvailableListsDropDownList.Items.Count < 1 )
				{
					//Add dummy item - this will not be
					//saved in the view state because its too late for that...
					AvailableListsDropDownList.Items.Add(LoadResource("ToolPart_Body_Text_SPListsListEmpty"));
				}
				if( AvailableViewsDropDownList.Items.Count < 1 )
				{
					//Add dummy item - this will not be
					//saved in the view state because its too late for that...
					AvailableViewsDropDownList.Items.Add(LoadResource("ToolPart_Body_Text_SPViewsListEmpty"));
				}

				this.EnsureChildControls();
				this.RenderChildren(output);
			}
			catch(Exception ex){System.Diagnostics.Debug.Write(ex.ToString());}
		}
		#endregion

		#region event handlers
		private void ListConnectionToolPart_Load(object sender, EventArgs e)
		{
			try
			{
			}
			catch(Exception ex){System.Diagnostics.Debug.Write(ex.ToString());}
		}

		/// <summary>
		/// Load the properties from the web part into the toolpart controls.
		/// do not override view state!
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListConnectionToolPart_PreRender(object sender, EventArgs e)
		{
			try
			{
				this.LoadWebPartProperties(false);
			}
			catch(Exception ex){System.Diagnostics.Debug.Write(ex.ToString());}
		}
		/// <summary>
		/// Load lists from the selected web into the available lists list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LoadListButton_Click(object sender, EventArgs e)
		{
			try
			{
				BaseListWebPart wp = this.ParentToolPane.SelectedWebPart as BaseListWebPart;

				EnsureChildControls();
				AvailableListsDropDownList.Items.Clear();

				SPWeb web;
				if( WebUrlTextBox.Text.Trim() == string.Empty )
				{
					web = Microsoft.SharePoint.WebControls.SPControl.GetContextWeb(Context);
				}
				else
				{
					SPSite st = new SPSite( new Uri(Page.Request.Url, WebUrlTextBox.Text, true).ToString() );
					st.CatchAccessDeniedException = false;
					web = st.OpenWeb();
				}

				WebUrlTextBox.Text = web.Url;
				web.Lists.ListsForCurrentUser = true;
				foreach(SPList list in web.Lists)
				{
					AvailableListsDropDownList.Items.Add(list.Title);

					if( wp.ListName == list.Title )
						AvailableListsDropDownList.SelectedIndex = AvailableListsDropDownList.Items.Count-1;
				}

				LoadViews();
			}
			catch(Exception ex){System.Diagnostics.Debug.Write(ex.ToString());}
		}

		private void AvailableListsDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				LoadViews();
			}
			catch(Exception ex){System.Diagnostics.Debug.Write(ex.ToString());}
		}

		#endregion

		#region General
		/// <summary>
		/// Loads the values fro mth eweb part.
		/// Use force=true to force replace of view state values.
		/// </summary>
		/// <param name="force">Override values</param>
		void LoadWebPartProperties(bool force)
		{
			this.EnsureChildControls();

			if( force || ViewState["ListConnectionToolPartLoaded" + this.Qualifier] == null  || (bool)ViewState["ListConnectionToolPartLoaded" + this.Qualifier]!= true )
			{
				ViewState["ListConnectionToolPartLoaded" + this.Qualifier] = true;

				BaseListWebPart wp = this.ParentToolPane.SelectedWebPart as BaseListWebPart;
				
				if( force || this.WebUrlTextBox.Text == string.Empty )
				{
					if( wp.ListWebUrl.Trim() == string.Empty )
						WebUrlTextBox.Text = Microsoft.SharePoint.WebControls.SPControl.GetContextWeb(Context).Url;
					else
						WebUrlTextBox.Text = wp.ListWebUrl;
					LoadListButton_Click(null,null);
				}
			}
		}

		string LoadResource(string ID)
		{
			switch(ID)
			{
				case "ToolPart_Header_Text_SPLists":
					return "SharePoint Lists";
				case "ToolPart_Body_Text_SPListsUrl":
					return "Enter SharePoint Site URL and click Load";
				case "ToolPart_Body_Text_SPListsUrlLoad":
					return "Load...";
				case "ToolPart_Body_Text_SPListsSelectList":
					return "Select a SharePoint List";
				case "ToolPart_Body_Text_SPViewsSelectList":
					return "Select a view";
				case "ToolPart_Body_Text_SPListsListEmpty":
					return "- No lists available -";
				case "ToolPart_Body_Text_SPViewsListEmpty":
					return "- No views available -";
				case "ToolPart_Body_Text_SPListsUrl_ToolTip":
					return "SharePoint Site Url";
				case "ToolPart_Body_Text_SPListsUrlLoad_ToolTip":
					return "Load all lists on the web site";
			}

			return ID;
		}

		void LoadViews()
		{
			try
			{
				BaseListWebPart wp = this.ParentToolPane.SelectedWebPart as BaseListWebPart;

				EnsureChildControls();
				AvailableViewsDropDownList.Items.Clear();

				SPWeb web;
				if( WebUrlTextBox.Text.Trim() == string.Empty )
				{
					web = Microsoft.SharePoint.WebControls.SPControl.GetContextWeb(Context);
				}
				else
				{
					SPSite st = new SPSite( new Uri(Page.Request.Url, WebUrlTextBox.Text, true).ToString() );
					st.CatchAccessDeniedException = false;
					web = st.OpenWeb();
				}

				WebUrlTextBox.Text = web.Url;
				web.Lists.ListsForCurrentUser = true;
				SPList list = web.Lists[this.AvailableListsDropDownList.SelectedValue];

				foreach( SPView vi in list.Views )
				{
					if( vi.Title.Trim() != string.Empty )
						AvailableViewsDropDownList.Items.Add(vi.Title);

					if( wp.ListViewName == vi.Title )
						AvailableViewsDropDownList.SelectedIndex = AvailableViewsDropDownList.Items.Count-1;
				}
			}
			catch(Exception ex){System.Diagnostics.Debug.Write(ex.ToString());}
		}
		#endregion


	}											
}
								
