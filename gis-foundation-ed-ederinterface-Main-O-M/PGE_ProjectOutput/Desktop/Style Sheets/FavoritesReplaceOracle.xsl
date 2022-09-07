<!-- replace.xsl:  Replaces a string with another string for an entire xml document-->
<xsl:transform
 xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
 version="1.0"
>

<xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

<!-- String to replace -->
<xsl:variable name="replace" select="'ARCFM8.'"/>
<!-- String to substitue for the "replace" sting -->
<xsl:variable name="by" select="''"/>

<!-- Replaces all occurrences of $replace with $by" -->
<xsl:template name="do-replace">
     <xsl:param name="text"/>
   <xsl:choose>
   <!-- $replace is at the beginning -->
   <xsl:when test="starts-with($text,$replace)">
   	<xsl:value-of select="$by"/>
   	<xsl:value-of select="substring-after($text,$replace)"/>
   </xsl:when>
   <!-- $replace is in the middle of the string -->
   <xsl:when test="contains($text, $replace)">
      <xsl:value-of select="substring-before($text, $replace)"/>
      <xsl:value-of select="$by"/>
      <xsl:call-template name="do-replace">
         <xsl:with-param name="text"
                         select="substring-after($text, $replace)"/>
      </xsl:call-template>
   </xsl:when>
   <xsl:otherwise>
      <xsl:value-of select="$text"/>
   </xsl:otherwise>
   </xsl:choose>
</xsl:template>

<!-- Copies all elements and attributes.  Applies templates. -->
<xsl:template match="*|@*">
    <xsl:copy>
       <xsl:apply-templates select="*|@*|text()"/>
    </xsl:copy>
</xsl:template>

<xsl:template match="@*">
	<xsl:attribute name="{name(.)}">
		<xsl:call-template name="do-replace">
			<xsl:with-param name="text" select="."/>
		</xsl:call-template>
	</xsl:attribute>
</xsl:template>
<!--
<xsl:template match="@TableName">
	<xsl:attribute name="TableName">
		<xsl:call-template name="do-replace">
			<xsl:with-param name="text" select="."/>
		</xsl:call-template>
	</xsl:attribute>
</xsl:template>

<xsl:template match="@ RELATIONSHIPCLASS_NAME">
	<xsl:attribute name="RELATIONSHIPCLASS_NAME">
		<xsl:call-template name="do-replace">
			<xsl:with-param name="text" select="."/>
		</xsl:call-template>
	</xsl:attribute>
</xsl:template>
-->
<!-- Calls do-replace for text node -->
<xsl:template match="text()">
	<xsl:if test=". != '&#10;'">
	    <xsl:call-template name="do-replace">
       	 <xsl:with-param name="text" select="."/>
	    </xsl:call-template>
    </xsl:if>
</xsl:template>

</xsl:transform>

