<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:fo="http://www.w3.org/1999/XSL/Format">

<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

<!-- Strings to append -->
<xsl:variable name="prefix" select="'ARCFM8.'"/>
<xsl:variable name="prefixsys" select="'sde.'"/>
<xsl:variable name="prefixobject" select="'ARCFM8.'"/>
<xsl:variable name="prefixrelclass" select="'ARCFM8.'"/>

<!-- Match the root node -->
<xsl:template match="/">
	<xsl:apply-templates select="*"/>
</xsl:template>

<!-- Match everything else -->
   <xsl:template match="*|@*|text()">
      <xsl:copy><xsl:apply-templates select="*|@*|text()"/></xsl:copy>
   </xsl:template>

<xsl:template match="MODELNAMES/OBJECTCLASSNAME">
	<xsl:choose>
		<xsl:when test="../CLASSNAMES/MODELNAME='ARCFMSYSTEMTABLE'">
			<xsl:copy><xsl:value-of select="concat($prefixsys, .)"/></xsl:copy>			
		</xsl:when>
		<xsl:when test="../OBJECT">
			<xsl:copy><xsl:value-of select="concat($prefixobject, .)"/></xsl:copy>			
		</xsl:when>
		<xsl:otherwise>
			<xsl:copy><xsl:value-of select="concat($prefix, .)"/></xsl:copy>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="MODELNAMES/CLASSNAMES/NAME">
	<xsl:choose>
		<xsl:when test="../MODELNAME='ARCFMSYSTEMTABLE'">
			<xsl:copy><xsl:value-of select="concat($prefixsys, .)"/></xsl:copy>			
		</xsl:when>
		<xsl:when test="../OBJECT">
			<xsl:copy><xsl:value-of select="concat($prefixobject, .)"/></xsl:copy>			
		</xsl:when>
		<xsl:otherwise>
			<xsl:copy><xsl:value-of select="concat($prefix, .)"/></xsl:copy>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<!-- Prefix for Relationships -->
<xsl:template match="RELATIONSHIPCLASS/NAME">
	<xsl:copy><xsl:value-of select="concat($prefixrelclass, .)"/></xsl:copy>
</xsl:template>

<!-- Prefix for ObjectClassInfo/ObjClassName -->
<xsl:template match="OBJECTCLASSINFO/OBJCLASSNAME">
	<xsl:copy><xsl:value-of select="concat($prefixobject, .)"/></xsl:copy>
</xsl:template>


<!-- Prefix for Abandoned - related tables -->
<xsl:template match="FEATURECLASS/SUBTYPE/SIMPLEVARIANT/VARIANTPROPVALUE">
	<xsl:choose>
		<xsl:when test="../VARIANTPROPTYPE='Abandon Feature Class'">
			<xsl:copy><xsl:value-of select="concat($prefix, .)"/></xsl:copy>
		</xsl:when>
		<xsl:when test="../VARIANTPROPTYPE='Remove Feature Class'">
			<xsl:copy><xsl:value-of select="concat($prefix, .)"/></xsl:copy>
		</xsl:when>
	</xsl:choose>
</xsl:template>

<!-- Prefix for EDM - related tables -->
<xsl:template match="FEATURECLASS/SUBTYPE/AUTOVALUE/AVEDMTABLENAME">
	<xsl:copy><xsl:value-of select="concat($prefixobject, .)"/></xsl:copy>
</xsl:template>

<!-- Prefix for tables defined in snapping rules -->
<xsl:template match="SNAPELEMENT/SNAPENTRY/SNAPFEATURECLASS | SNAPELEMENT/SNAPENTRY/SNAPTOFEATURECLASS">
	<xsl:copy><xsl:value-of select="concat($prefix, .)"/></xsl:copy>
</xsl:template>

<xsl:template match="FEATURECLASS/FEATURENAME">
	<xsl:choose>
		<xsl:when test="../OBJECT">
			<xsl:copy><xsl:value-of select="concat($prefixobject, .)"/></xsl:copy> 			
		</xsl:when>
		<xsl:otherwise>
			<xsl:copy><xsl:value-of select="concat($prefix, .)"/></xsl:copy> 
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template match="FEATURECLASS/SUBTYPE/FIELD/FIELDNAME">
	<xsl:copy><xsl:value-of select="translate(., 'abcdefghijklmnopqrstuvwxyz', 'ABCDEFGHIJKLMNOPQRSTUVWXYZ')"/></xsl:copy>
</xsl:template>

<xsl:template match="MODELNAMES/FIELDNAMES/NAME">
	<xsl:copy><xsl:value-of select="translate(., 'abcdefghijklmnopqrstuvwxyz', 'ABCDEFGHIJKLMNOPQRSTUVWXYZ')"/></xsl:copy>
</xsl:template>

<xsl:template match="FEATURECLASS/PRIMARYDISPLAYFIELD">
	<xsl:copy><xsl:value-of select="translate(., 'abcdefghijklmnopqrstuvwxyz', 'ABCDEFGHIJKLMNOPQRSTUVWXYZ')"/></xsl:copy>
</xsl:template>

</xsl:stylesheet>

