﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using codeplex.spsl;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;

namespace KWizCom.SharePoint.ListMarquee.ListMarquee
{
#if EditorParts
	public class MarqueeToolPart : EditorPart
	{
		#region members
		Logging logger;
		#endregion

		#region controls
		/// <summary>
		/// Text box for the Title Field Name
		/// </summary>
		TextBox TitleFieldNameTextBox;
		/// <summary>
		/// Text box for the Body Field Name
		/// </summary>
		TextBox BodyFieldNameTextBox;
		/// <summary>
		/// Marquee Delay Parameter text box
		/// </summary>
		TextBox MarqueeSpeedTextBox;
		/// <summary>
		/// Validate numeric value
		/// </summary>
		RangeValidator MarqueeSpeedValidator;

		/// <summary>
		/// Marquee Amount Parameter text box
		/// </summary>
		TextBox MarqueeStepSizeTextBox;
		/// <summary>
		/// Validate numeric value
		/// </summary>
		RangeValidator MarqueeStepSizeValidator;

		/// <summary>
		/// Select Marquee Direction
		/// </summary>
		ListBox MarqueeDirectionListBox;

		/// <summary>
		/// Select Marquee Links target
		/// </summary>
		ListBox MarqueeLinkTargetListBox;

		#endregion

		#region Ctor
		/// <summary>
		/// Constructor for the class. A great place to set Set default values for
		/// additional base class properties here.
		/// <summary>
		public MarqueeToolPart()
		{
			try
			{
				logger = new Logging(this.GetType());
				this.Title = "Marquee Tool Part V" + Constants.Version;

				this.Load += new EventHandler(ListConnectionToolPart_Load);
				this.PreRender += new EventHandler(ListConnectionToolPart_PreRender);
			}
			catch (Exception ex) { if (logger != null && logger.IsLoggingEnabled) logger.LogError(ex); }
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
				System.Web.UI.HtmlControls.HtmlGenericControl containerDiv = new System.Web.UI.HtmlControls.HtmlGenericControl("DIV");
				containerDiv.Attributes["class"] = "UserGeneric";
				this.Controls.Add(containerDiv);

				System.Web.UI.HtmlControls.HtmlGenericControl div;

				containerDiv.Controls.Add(
					GetNewHeaderDiv(Constants.ToolPart.Header_Text_ListFields)
					);

				div = GetNewBodyDiv(Constants.ToolPart.Body_Text_ListTitleField);
				containerDiv.Controls.Add(div);

				TitleFieldNameTextBox = new TextBox();
				TitleFieldNameTextBox.ToolTip = Constants.ToolPart.Body_Text_ListTitleField_ToolTip;
				TitleFieldNameTextBox.CssClass = "UserInput";
				div.Controls.Add(WrapInDiv(TitleFieldNameTextBox));

				div = GetNewBodyDiv(Constants.ToolPart.Body_Text_ListBodyField);
				containerDiv.Controls.Add(div);

				BodyFieldNameTextBox = new TextBox();
				BodyFieldNameTextBox.ToolTip = Constants.ToolPart.Body_Text_ListBodyField_ToolTip;
				BodyFieldNameTextBox.CssClass = "UserInput";
				div.Controls.Add(WrapInDiv(BodyFieldNameTextBox));

				containerDiv.Controls.Add(
					GetNewHeaderDiv(Constants.ToolPart.Header_Text_MarqueeDefinitions)
					);

				div = GetNewBodyDiv(Constants.ToolPart.Body_Text_MarqueeDelay);
				containerDiv.Controls.Add(div);

				MarqueeSpeedTextBox = new TextBox();
				MarqueeSpeedTextBox.ToolTip = Constants.ToolPart.Body_Text_MarqueeDelay;
				MarqueeSpeedTextBox.CssClass = "UserInput";
				MarqueeSpeedTextBox.ID = "MarqueeSpeedTextBox";
				MarqueeSpeedValidator = new RangeValidator();
				MarqueeSpeedValidator.ControlToValidate = MarqueeSpeedTextBox.ID;
				MarqueeSpeedValidator.EnableClientScript = true;
				MarqueeSpeedValidator.ErrorMessage = Constants.ToolPart.Body_Text_MarqueeDelayError;
				MarqueeSpeedValidator.ForeColor = System.Drawing.Color.Red;
				MarqueeSpeedValidator.MaximumValue = "999";
				MarqueeSpeedValidator.MinimumValue = "0";
				MarqueeSpeedValidator.Text = MarqueeSpeedValidator.ErrorMessage;
				MarqueeSpeedValidator.Type = ValidationDataType.Integer;
				MarqueeSpeedValidator.Display = ValidatorDisplay.Dynamic;
				div.Controls.Add(WrapInDiv(MarqueeSpeedTextBox));
				div.Controls.Add(WrapInDiv(MarqueeSpeedValidator));

				div = GetNewBodyDiv(Constants.ToolPart.Body_Text_MarqueeStep);
				containerDiv.Controls.Add(div);

				MarqueeStepSizeTextBox = new TextBox();
				MarqueeStepSizeTextBox.ToolTip = Constants.ToolPart.Body_Text_MarqueeStep;
				MarqueeStepSizeTextBox.CssClass = "UserInput";
				MarqueeStepSizeTextBox.ID = "MarqueeStepSizeTextBox";
				MarqueeStepSizeValidator = new RangeValidator();
				MarqueeStepSizeValidator.ControlToValidate = MarqueeStepSizeTextBox.ID;
				MarqueeStepSizeValidator.EnableClientScript = true;
				MarqueeStepSizeValidator.ErrorMessage = Constants.ToolPart.Body_Text_MarqueeStepError;
				MarqueeStepSizeValidator.ForeColor = System.Drawing.Color.Red;
				MarqueeStepSizeValidator.MaximumValue = "50";
				MarqueeStepSizeValidator.MinimumValue = "0";
				MarqueeStepSizeValidator.Text = MarqueeStepSizeValidator.ErrorMessage;
				MarqueeStepSizeValidator.Type = ValidationDataType.Integer;
				MarqueeStepSizeValidator.Display = ValidatorDisplay.Dynamic;
				div.Controls.Add(WrapInDiv(MarqueeStepSizeTextBox));
				div.Controls.Add(WrapInDiv(MarqueeStepSizeValidator));

				div = GetNewBodyDiv(Constants.ToolPart.Body_Text_MarqueeDirection);
				containerDiv.Controls.Add(div);

				MarqueeDirectionListBox = new ListBox();
				MarqueeDirectionListBox.CssClass = "UserInput";
				div.Controls.Add(WrapInDiv(MarqueeDirectionListBox));

				div = GetNewBodyDiv(Constants.ToolPart.Body_Text_MarqueeLinkTarget);
				containerDiv.Controls.Add(div);

				MarqueeLinkTargetListBox = new ListBox();
				MarqueeLinkTargetListBox.CssClass = "UserInput";
				div.Controls.Add(WrapInDiv(MarqueeLinkTargetListBox));
			}
			catch (Exception ex) { logger.LogError(ex); }
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
		public override bool ApplyChanges()
		{
			try
			{
				EnsureChildControls();
				// apply property values here
				ListMarquee wp = this.WebPartToEdit as ListMarquee;

				wp.TitleFieldName = this.TitleFieldNameTextBox.Text;
				wp.BodyFieldName = this.BodyFieldNameTextBox.Text;
				wp.MarqueeDelay = int.Parse(this.MarqueeSpeedTextBox.Text);
				wp.MarqueeAmount = int.Parse(this.MarqueeStepSizeTextBox.Text);
				wp.MarqueeDirection = (MarqueeDirections)Enum.Parse(typeof(MarqueeDirections), this.MarqueeDirectionListBox.SelectedValue, true);
				wp.LinkTarget = (LinkTargets)Enum.Parse(typeof(LinkTargets), this.MarqueeLinkTargetListBox.SelectedValue, true);
				return true;
			}
			catch (Exception ex) { logger.LogError(ex); }
			return false;
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
			catch (Exception ex) { logger.LogError(ex); }
		}
		#endregion

		#region event handlers
		private void ListConnectionToolPart_Load(object sender, EventArgs e)
		{
			try
			{
			}
			catch (Exception ex) { logger.LogError(ex); }
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
			catch (Exception ex) { logger.LogError(ex); }
		}
		#endregion

		#region General
		/// <summary>
		/// Loads the values from the web part.
		/// Use force=true to force replace of view state values.
		/// </summary>
		/// <param name="force">Override values</param>
		void LoadWebPartProperties(bool force)
		{
			this.EnsureChildControls();

			if (force || ViewState["MarqueeToolPartLoaded" + this.WebPartToEdit.UniqueID] == null || (bool)ViewState["MarqueeToolPartLoaded" + this.WebPartToEdit.UniqueID] != true)
			{
				ViewState["MarqueeToolPartLoaded" + this.WebPartToEdit.UniqueID] = true;

				ListMarquee wp = this.WebPartToEdit as ListMarquee;

				if (force || this.TitleFieldNameTextBox.Text == string.Empty)
				{
					this.TitleFieldNameTextBox.Text = wp.TitleFieldName;
				}
				if (force || this.BodyFieldNameTextBox.Text == string.Empty)
				{
					this.BodyFieldNameTextBox.Text = wp.BodyFieldName;
				}
				if (force || this.MarqueeSpeedTextBox.Text == string.Empty)
				{
					this.MarqueeSpeedTextBox.Text = wp.MarqueeDelay.ToString();
				}
				if (force || this.MarqueeStepSizeTextBox.Text == string.Empty)
				{
					this.MarqueeStepSizeTextBox.Text = wp.MarqueeAmount.ToString();
				}
				if (force || this.MarqueeDirectionListBox.Items.Count == 0)
				{
					LoadMarqueeDirectionListBox(wp.MarqueeDirection);
				}
				if (force || this.MarqueeLinkTargetListBox.Items.Count == 0)
				{
					LoadMarqueeLinkTargetListBox(wp.LinkTarget);
				}
			}
		}

		void LoadMarqueeDirectionListBox(MarqueeDirections selected)
		{
			MarqueeDirectionListBox.Items.Clear();

			foreach (string name in Enum.GetNames(typeof(MarqueeDirections)))
			{
				ListItem itm = new ListItem(name, name);
				if (name == selected.ToString())
					itm.Selected = true;

				MarqueeDirectionListBox.Items.Add(itm);
			}
		}
		void LoadMarqueeLinkTargetListBox(LinkTargets selected)
		{
			MarqueeLinkTargetListBox.Items.Clear();

			foreach (string name in Enum.GetNames(typeof(LinkTargets)))
			{
				string Title = string.Empty;

				if (name == "_blank")
					Title = "New window";
				if (name == "_self")
					Title = "Current window";
				if (name == "_top")
					Title = "Top window";

				if (Title != string.Empty)
				{
					ListItem itm = new ListItem(Title, name);
					if (name == selected.ToString())
						itm.Selected = true;

					MarqueeLinkTargetListBox.Items.Add(itm);
				}
			}
		}
		#endregion
	}
#endif
}
