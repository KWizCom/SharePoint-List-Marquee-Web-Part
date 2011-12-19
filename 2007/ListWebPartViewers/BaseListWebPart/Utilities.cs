using System;
using System.Xml;
using System.Xml.Xsl;
using System.Text;
using System.IO;

namespace KWizCom.SharePoint.WebParts
{
	/// <summary>
	/// Summary description for Utilities.
	/// </summary>
	public class Utilities
	{
		/// <summary>
		/// transforms xml document with the xsl in rthe given path.
		/// </summary>
		/// <param name="page">Page (used for storing cache)</param>
		/// <param name="XmlDoc">XML To Transform</param>
		/// <param name="xslPath">XSL path to load</param>
		/// <returns>Transformed HTML</returns>
		public static string XslTransform(System.Web.UI.Page page, XmlDocument XmlDoc, string xslPath)
		{
			XslTransform xslTransform = null;
			StringBuilder stmHtml = new StringBuilder(10000);
			TextWriter txtHtmlOutput = new StringWriter(stmHtml);

			try
			{
				if( page.Cache.Get(xslPath) is XslTransform )
					xslTransform = page.Cache.Get(xslPath) as XslTransform;
			}
			catch{}

			if( xslTransform == null )//no cache
			{
				XmlDocument	xslDocument = new XmlDocument();
				XmlUrlResolver urlResolver = new XmlUrlResolver();
				//Set default credentials
				urlResolver.Credentials = System.Net.CredentialCache.DefaultCredentials;
				xslDocument.XmlResolver = urlResolver;
				xslDocument.Load(xslPath);

				xslTransform = new XslTransform();
				xslTransform.Load(xslDocument.CreateNavigator(),urlResolver, null);

				try
				{
					//save to cache
					page.Cache.Insert(xslPath, xslTransform);
				}
				catch{}
			}

			xslTransform.Transform(XmlDoc.CreateNavigator(), null, txtHtmlOutput, null);

			txtHtmlOutput.Close();

			return stmHtml.ToString();
		}

		/// <summary>
		/// transforms xml document with the xsl in rthe given path.
		/// </summary>
		/// <param name="page">Page (used for storing cache)</param>
		/// <param name="XmlDoc">XML To Transform</param>
		/// <param name="argumentList">list of arguments</param>
		/// <param name="xslPath">XSL path to load</param>
		/// <returns>Transformed HTML</returns>
		public static string XslTransform(System.Web.UI.Page page, XmlDocument XmlDoc, XsltArgumentList argumentList, string xslPath)
		{
			XslTransform xslTransform = null;
			StringBuilder stmHtml = new StringBuilder(10000);
			TextWriter txtHtmlOutput = new StringWriter(stmHtml);

			try
			{
				if( page.Cache.Get(xslPath) is XslTransform )
					xslTransform = page.Cache.Get(xslPath) as XslTransform;
			}
			catch{}

			if( xslTransform == null )//no cache
			{
				XmlDocument	xslDocument = new XmlDocument();
				XmlUrlResolver urlResolver = new XmlUrlResolver();
				//Set default credentials
				urlResolver.Credentials = System.Net.CredentialCache.DefaultCredentials;
				xslDocument.XmlResolver = urlResolver;
				xslDocument.Load(xslPath);

				xslTransform = new XslTransform();
				xslTransform.Load(xslDocument.CreateNavigator(),urlResolver, null);

				try
				{
					//save to cache
					page.Cache.Insert(xslPath, xslTransform);
				}
				catch{}
			}

			xslTransform.Transform(XmlDoc.CreateNavigator(), argumentList, txtHtmlOutput, null);

			txtHtmlOutput.Close();

			return stmHtml.ToString();
		}


	}
}
