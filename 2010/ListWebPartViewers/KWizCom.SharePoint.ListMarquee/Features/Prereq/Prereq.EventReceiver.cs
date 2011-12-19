using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace KWizCom.SharePoint.WebParts.ListMarquee.Features.Prereq
{
	/// <summary>
	/// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
	/// </summary>
	/// <remarks>
	/// The GUID attached to this class may be used during packaging and should not be modified.
	/// </remarks>

	[Guid("1c685130-8443-415b-b3d9-791a922ed90c")]
	public class PrereqEventReceiver : SPFeatureReceiver
	{
		// Uncomment the method below to handle the event raised after a feature has been activated.

		public override void FeatureActivated(SPFeatureReceiverProperties properties)
		{
			try
			{
				(properties.Feature.Parent as SPSite).RootWeb.Lists.EnsureSiteAssetsLibrary();
			}
			catch { }
		}


		// Uncomment the method below to handle the event raised before a feature is deactivated.

		//public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
		//{
		//}


		// Uncomment the method below to handle the event raised after a feature has been installed.

		//public override void FeatureInstalled(SPFeatureReceiverProperties properties)
		//{
		//}


		// Uncomment the method below to handle the event raised before a feature is uninstalled.

		//public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
		//{
		//}

		// Uncomment the method below to handle the event raised when a feature is upgrading.

		//public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
		//{
		//}
	}
}
