using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KWizCom.SharePoint.ListMarquee
{
	public enum MarqueeDirections { left, right, up, down }
	public enum LinkTargets
	{
		_blank,// Load the linked document into a new blank window. This window is not named.  
		_media,// Load the linked document into the HTML content area of the Media Bar. Available in Internet Explorer 6 or later. 
		_parent,// Load the linked document into the immediate parent of the document the link is in.  
		_search,// Load the linked document into the browser search pane. Available in Internet Explorer 5 or later.  
		_self,// Default. Load the linked document into the window in which the link was clicked (the active window).  
		_top,//Load the linked document into the topmost window. 
	}

	public class Constants
	{
		public const string Version = "11.0.04";

		public class ToolPart
		{
			public const string Header_Text_ListFields = "Marquee List Fields";
			public const string Body_Text_ListTitleField = "Title List Field";
			public const string Body_Text_ListBodyField = "Body List Field";
			public const string Body_Text_ListTitleField_ToolTip = "Select the list field for the marquee title";
			public const string Body_Text_ListBodyField_ToolTip = "Select the list field for the marquee body";

			public const string Header_Text_MarqueeDefinitions = "Marquee Definitions";
			public const string Body_Text_MarqueeDelay = "Marquee Scroll Speed";
			public const string Body_Text_MarqueeDelayError = "Please Enter a Number Between 0 to 999.";
			public const string Body_Text_MarqueeStep = "Marquee Scroll Step";
			public const string Body_Text_MarqueeStepError = "Please Enter a Number Between 0 to 50.";

			public const string Body_Text_MarqueeDirection = "Merquee Scroll Direction";
			public const string Body_Text_MarqueeLinkTarget = "Open Item Location";

		}
	}
}
