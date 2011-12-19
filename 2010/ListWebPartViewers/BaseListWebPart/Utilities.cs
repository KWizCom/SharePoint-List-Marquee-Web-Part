using System;
using System.Xml;
using System.Xml.Xsl;
using System.Text;
using System.IO;
using Microsoft.SharePoint;

namespace KWizCom.SharePoint.WebParts
{
	public class Utilities
	{
		/// <summary>
		/// transforms xml document with the xsl in rthe given path.
		/// </summary>
		/// <param name="page">Page (used for storing cache)</param>
		/// <param name="XmlDoc">XML To Transform</param>
		/// <param name="xslPath">XSL path to load</param>
		/// <returns>Transformed HTML</returns>
		public static string XslTransform(XmlDocument XmlDoc, string xslPath)
		{
			return XslTransform(XmlDoc, null, xslPath);
		}

		/// <summary>
		/// transforms xml document with the xsl in rthe given path.
		/// </summary>
		/// <param name="page">Page (used for storing cache)</param>
		/// <param name="XmlDoc">XML To Transform</param>
		/// <param name="argumentList">list of arguments</param>
		/// <param name="xslPath">XSL path to load</param>
		/// <returns>Transformed HTML</returns>
		public static string XslTransform(XmlDocument XmlDoc, XsltArgumentList argumentList, string xslPath)
		{
			XslTransform xslTransform = null;
			StringBuilder stmHtml = new StringBuilder(10000);
			TextWriter txtHtmlOutput = new StringWriter(stmHtml);

			XmlDocument xslDocument = new XmlDocument();

			try
			{
				using (SPSite site = new SPSite(xslPath))
				{
					using (SPWeb web = site.OpenWeb())
					{
						string xslText = web.GetFileAsString(xslPath);
						xslDocument.LoadXml(xslText);
					}
				}
			}
			catch (Exception ex)
			{
				throw new FileNotFoundException("The xsl file could not be loaded. The file must be uploaded into a library inside your current site collection", ex);
			}

			xslTransform = new XslTransform();
			xslTransform.Load(xslDocument.CreateNavigator());

			xslTransform.Transform(XmlDoc.CreateNavigator(), argumentList, txtHtmlOutput, null);

			txtHtmlOutput.Close();

			return stmHtml.ToString();
		}
	}
}
