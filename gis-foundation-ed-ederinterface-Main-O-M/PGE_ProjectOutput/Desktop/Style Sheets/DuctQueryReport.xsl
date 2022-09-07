<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<html>
			<head>
				<meta http-equiv="Content-Type" content="text/html; charset=UTF-8"/>
				<style>
					<!-- Primary headings -->
					.ph1 {margin-left:0.2in; font-weight:bold; color:#0000FF}
					.ph2 {margin-left:0.25in; font-weight:normal; color:#2E8B57}
					<!-- Generated text and listitems -->
					.gt {margin-left:0.3in; font-weight:normal; font-style="normal"; color:#000000}
					.gbt {margin-left:0.3in; font-weight:bold; color:#000000}
				</style>
			</head>
			<body>
				<h2 align="center">Conduit Trace Report</h2>
				<b >
				<xsl:if test="DuctQueryXML/CriteriaSummary">
					Criteria Summary
					<xsl:apply-templates select="DuctQueryXML/CriteriaSummary"/>
				</xsl:if>
				</b>
				<br />
				<b >
				<xsl:choose>
				
					<xsl:when test="DuctQueryXML/FeatureList/*">
						Feature List
						<xsl:apply-templates select="DuctQueryXML/FeatureList"/>
					</xsl:when>
					<xsl:otherwise>
						No conduit features meet the criteria between the start and end points of the trace.
					</xsl:otherwise>
				</xsl:choose>
				</b>
			</body>
		</html>
	</xsl:template>
	<xsl:template match="DuctQueryXML/CriteriaSummary">
		<div style="margin-left: 0.25in" class="ph2" >
			<xsl:for-each select="Criterion">
				<xsl:value-of select="text()"/><br/>
				<div style="margin-left:0.2in" class="gt">
					<xsl:for-each select="./CriterionInfo">
						<xsl:value-of select="." />
						<br />
					</xsl:for-each>
				</div>
			</xsl:for-each>
		</div>
	</xsl:template>
	<xsl:template match="DuctQueryXML/FeatureList">
		<div style="margin-left: 0.25in" class="ph2" >
			<xsl:for-each select="Feature">
				<xsl:value-of select="text()"/><br/>
				<div style="margin-left:0.2in" class="gt">
					<xsl:if test="AttributeList" >
						<xsl:apply-templates select="AttributeList" />
						<br />
					</xsl:if>
					<xsl:if test="DuctList" >
						<xsl:apply-templates select="DuctList" />
						<br />
					</xsl:if>
				</div>
			</xsl:for-each>
		</div>
	</xsl:template>
	<xsl:template match="AttributeList|Attributes-Properties">
		<div style="margin-left: 0.25in" class="gbt" >
			Attributes
			<table border="0" cellpadding="5">
				<tr>
					<th>Field Name</th>
					<th>Field Value</th>
				</tr>
				<xsl:for-each select="./FieldValue">
					<xsl:if test="(not(text()='Blob') and not(text()='Geometry'))">
				

				<!--	<xsl:if test="not(text()='Geometry')">   -->
						<tr>
							<td><xsl:value-of select="preceding-sibling::FieldName[position()=1]"/></td>
							<td><xsl:value-of select="."/></td>
						</tr>
					</xsl:if>
				<!--	</xsl:if>  -->
				</xsl:for-each>
			</table>
		</div>
	</xsl:template>
	<xsl:template match="DuctList|Duct">
		
		<xsl:for-each select="./Duct">
			<xsl:choose >
				<xsl:when test="MeetsCriteria">
				
					<div style="margin-left: 0.25in" class="ph2">
						<xsl:if test="not(../../DuctList)">
							<xsl:value-of select="../text()"/>-
						</xsl:if>
						<xsl:value-of select="text()"/><br/>
						<div style="margin-left:0.2in" class="gt">
							<xsl:if test="Attributes-Properties" >
								<xsl:apply-templates select="Attributes-Properties" />
								<br />
							</xsl:if>
							<xsl:if test="Occupants" >
								<xsl:apply-templates select="Occupants" />
								<br />
							</xsl:if>
							<xsl:if test="Duct" >
								<xsl:apply-templates select="." />
								<br />
							</xsl:if>
						</div>
					</div>
				</xsl:when>
				<xsl:otherwise>
					
					<div style="margin-left: 0.25in" class="ph2">
						<!--<xsl:value-of select="text()"/><br/>-->
						<div style="margin-left:0.2in" class="gt">
							<xsl:if test="Duct" >
								<xsl:apply-templates select="." />
								<br />
							</xsl:if>
						</div>
					</div>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:for-each>
		
	</xsl:template>
	<xsl:template match="Occupants">
		<div style="margin-left: 0.25in" class="gbt">
		Occupants
		<br />
			<div class="gt">
			<xsl:for-each select="Occupant">
				<xsl:value-of select="."/>
				<br />
			</xsl:for-each>
			</div>
		</div>
	</xsl:template>
</xsl:stylesheet>
