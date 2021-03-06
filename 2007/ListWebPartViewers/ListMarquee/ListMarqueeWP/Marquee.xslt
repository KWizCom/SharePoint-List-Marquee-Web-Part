<?xml version="1.0" encoding="UTF-8" ?>
<!--
Written by: Shai Ben Shooshan Shaibs@gmail.com
At:			23/04/2006
For:		KWizCom http://www.KWizCom.com

 -->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output encoding="utf-8" omit-xml-declaration="yes" />

<xsl:param name="MarqueeAmount"/>
<xsl:param name="MarqueeDelay"/>
<xsl:param name="MarqueeDirection"/>
<xsl:param name="LinkTarget"/>

<xsl:template match="Items">
	<script>
	function marqStop(marq)
	{
		marq.scrollAmount= 0;
	}
	function marqStart(marq)
	{
		marq.scrollAmount = <xsl:value-of select="$MarqueeAmount" />;
	}
	</script>
	<marquee onmouseover="marqStop(this);" onmouseout="marqStart(this);">
		<xsl:attribute name="ScrollAmount">
			<xsl:value-of select="$MarqueeAmount" />
		</xsl:attribute>
		<xsl:attribute name="ScrollDelay">
			<xsl:value-of select="$MarqueeDelay" />
		</xsl:attribute>
		<xsl:attribute name="Direction">
			<xsl:value-of select="$MarqueeDirection" />
		</xsl:attribute>

		<xsl:choose>
			<xsl:when test="$MarqueeDirection = 'up' or $MarqueeDirection = 'down' ">
				<table width="100%" border="0" cellpadding="0" cellspacing="0">
					<xsl:attribute name="height"><xsl:value-of select="100 * count(*)" />%</xsl:attribute>
					<xsl:for-each select=".">
						<xsl:apply-templates/>
					</xsl:for-each>
				</table>
			</xsl:when>
			<xsl:otherwise>
				<xsl:for-each select=".">
					<table height="100%" border="0" cellpadding="0" cellspacing="10">
						<xsl:attribute name="width"><xsl:value-of select="100 * count(*)" />%</xsl:attribute>
						<xsl:apply-templates/>
					</table>
				</xsl:for-each>
			</xsl:otherwise>
		</xsl:choose>

	</marquee>
</xsl:template>

<xsl:template match="Item">

	<xsl:choose>
		<xsl:when test="$MarqueeDirection = 'up' or $MarqueeDirection = 'down' ">
			<tr height="1%" valign="top">
				<td>
					<b>
						<A>
							<xsl:attribute name="Target">
								<xsl:value-of select="$LinkTarget" />
							</xsl:attribute>
							<xsl:attribute name="href">
								<xsl:value-of disable-output-escaping="yes" select="@ViewItemUrl"/>
							</xsl:attribute>
							<xsl:value-of disable-output-escaping="yes" select="@Title"/>
						</A>
					</b>
				</td>
			</tr>
			<tr height="1%" valign="top"><td><hr /></td></tr>
			<tr valign="top">
				<td>
					<xsl:value-of disable-output-escaping="yes" select="@Body"/>
				</td>
			</tr>
			<tr><td><br/></td></tr>
		</xsl:when>
		<xsl:otherwise>
			<td>
				<table width="100%" height="100%" border="0" cellpadding="0" cellspacing="0">
					<tr height="1%" valign="top">
						<td>
							<b>
								<A>
									<xsl:attribute name="Target">
										<xsl:value-of select="$LinkTarget" />
									</xsl:attribute>
									<xsl:attribute name="href">
										<xsl:value-of disable-output-escaping="yes" select="@ViewItemUrl"/>
									</xsl:attribute>
									<xsl:value-of disable-output-escaping="yes" select="@Title"/>
								</A>
							</b>
						</td>
					</tr>
					<tr height="1%" valign="top"><td><hr /></td></tr>
					<tr valign="top">
						<td>
							<xsl:value-of disable-output-escaping="yes" select="@Body"/>
						</td>
					</tr>
				</table>
			</td>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

</xsl:stylesheet>