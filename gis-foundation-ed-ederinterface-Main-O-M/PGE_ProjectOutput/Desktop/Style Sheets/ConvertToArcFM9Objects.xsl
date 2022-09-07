<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:fo="http://www.w3.org/1999/XSL/Format">


<!-- ProgID attribute - change mmCore to mmGeoDatabase -->
<xsl:template match="@ProgID" priority="1">

	<xsl:attribute name="ProgID">
		<xsl:value-of select="concat('mmGeoDatabase.',substring-after(.,'mmCore.'))"/>
	</xsl:attribute>
	
</xsl:template>

<!-- recursive copy -->
<xsl:template match="*|@*|text()"  priority="0">

	<xsl:copy>
		<xsl:apply-templates select="*|@*|text()"/>
	</xsl:copy>		

</xsl:template>

</xsl:stylesheet>