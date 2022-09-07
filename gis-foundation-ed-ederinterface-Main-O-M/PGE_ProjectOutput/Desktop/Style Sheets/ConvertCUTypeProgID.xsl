<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:fo="http://www.w3.org/1999/XSL/Format">

<!-- Used programmatically by ArcFM Favorites Imports for 9.1.2 SP1 and beyond.-->
<!-- ProgID attribute - change mmDesktop.D8CuType to mmFramework.D8CuType.-->

<xsl:template match="@ProgID[. = 'mmDesktop.D8CuType.1']" priority="1">
	<xsl:attribute name="ProgID">
		<xsl:text>mmFramework.D8CuType.1</xsl:text>
	</xsl:attribute>
</xsl:template>

<!-- recursive copy -->
<xsl:template match="*|@*|text()"  priority="0">
	<xsl:copy>
		<xsl:apply-templates select="*|@*|text()"/>
	</xsl:copy>		
</xsl:template>

</xsl:stylesheet>