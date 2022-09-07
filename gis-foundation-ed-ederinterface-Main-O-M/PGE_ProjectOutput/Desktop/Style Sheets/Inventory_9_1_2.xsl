<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<!-- Inventory_9_1_2.xsl :  This style sheet creates inventory reports for Workflow Manager
It is applied to design xmls that have the work request address and customer information added
under the workrequest node.-->
	<xsl:decimal-format name="engUS" decimal-separator="." grouping-separator=","/>
	<xsl:param name="XSLDirectory" select="'D:\dev\Bin\Style Sheets'"/>
	<!-- Output method is set to xml so the MSXML 3.0 SP1 Parser won't insert an unclosed META tag -->
	<xsl:output method="xml" omit-xml-declaration="yes" encoding="UTF-8"/>
	<xsl:key name="cu-by-wmscode-workfunction" match="CU" use="concat(generate-id(..),'_',WMS_CODE,'_',WORK_FUNCTION)"/>
	<xsl:template match="WORKREQUESTTOPLEVEL">
		<html>
			<head>
				<meta http-equiv="Content-Type" content="text/html; charset=UTF-8"/>
			</head>
			<body oncontextmenu="return true">
				<h2 align="center">Design Inventory Report</h2>
				<xsl:apply-templates select="WORKREQUEST"/>
			</body>
		</html>
	</xsl:template>
	<xsl:template match="WORKREQUEST">
		<b>Work Request Information:</b>
		<div style="margin-left:0.3in">
			<table border="0" width="500">
				<tr>
					<td width="150">Work Request Name:</td>
					<td width="350">
						<xsl:value-of select="DESCRIPTION"/>
					</td>
				</tr>
				<tr>
					<td width="150">Work Request Number:</td>
					<td width="350">
						<xsl:value-of select="ID"/>
					</td>
				</tr>
				<tr>
					<td width="150">Work Request Status:</td>
					<td width="350">
						<xsl:choose>
							<xsl:when test="WF_STATUS='1'">Initiated</xsl:when>
							<xsl:when test="WF_STATUS='2'">In Design</xsl:when>
							<xsl:when test="WF_STATUS='3'">In Construction</xsl:when>
							<xsl:when test="WF_STATUS='4'">Complete</xsl:when>
						</xsl:choose>
					</td>
				</tr>
			</table>
		</div>
		<xsl:apply-templates select="ADDRESS"/>
		<xsl:apply-templates select="CUSTOMER"/>
		<xsl:apply-templates select="DESIGN"/>
	</xsl:template>
	<xsl:template match="DESIGN">
		<br/>
		<b>Design Information:</b>
		<div style="margin-left:0.3in">
			<table border="0" width="500">
				<tr>
					<td width="150">Design Name:</td>
					<td width="350">
						<xsl:value-of select="DESCRIPTION"/>
					</td>
				</tr>
				<tr>
					<td width="150">Design Number:</td>
					<td width="350">
						<xsl:value-of select="DESIGN_NUMBER"/>
					</td>
				</tr>
				<tr>
					<td width="150">Design Status:</td>
					<td width="350">
						<xsl:choose>
							<xsl:when test="WF_STATUS='1'">In Design</xsl:when>
							<xsl:when test="WF_STATUS='2'">Pending Approval</xsl:when>
							<xsl:when test="WF_STATUS='3'">Approved</xsl:when>
							<xsl:when test="WF_STATUS='4'">As-Built Editing</xsl:when>
							<xsl:when test="WF_STATUS='5'">As-Built Verified</xsl:when>
							<xsl:when test="WF_STATUS='6'">Verified</xsl:when>
						</xsl:choose>
					</td>
				</tr>
			</table>
		</div>
		<xsl:if test="CU">
			<br/>
			<b>Compatible Units Not Associated to a Work Location</b>
			<table id="My CU table" style="behavior:url('{$XSLDirectory}\tableAct.htc');BORDER: black 1px solid; WIDTH: 99%; background-color:#eeeecc;" borderColor="#999999" cellSpacing="0" cellPadding="2" border="1" dragcolor="gray" slcolor="#ffffcc" hlcolor="#BEC5DE">
				<xsl:apply-templates select="CU[1]" mode="heading"/>
				<tbody>
					<!--xsl:for-each select="CU"-->
					<xsl:for-each select="CU[count(. | key('cu-by-wmscode-workfunction', concat(generate-id(..),'_',WMS_CODE,'_',WORK_FUNCTION))[1]) = 1]">
						<xsl:sort order="ascending" data-type="text" select="WMS_CODE"/>
						<xsl:apply-templates select="." mode="rows"/>
					</xsl:for-each>
				</tbody>
			</table>
		</xsl:if>
		<xsl:apply-templates select="WORKLOCATION">
			<xsl:sort data-type="number" select="ID"/>
		</xsl:apply-templates>
	</xsl:template>
	<xsl:template match="WORKLOCATION">
		<br/>
		<br/>
		<xsl:choose>
			<xsl:when test="DESCRIPTION!=''">
				<b>Work Location <xsl:value-of select="ID"/>:  <xsl:value-of select="DESCRIPTION"/>
				</b>
			</xsl:when>
			<xsl:otherwise>
				<b>Work Location <xsl:value-of select="ID"/>
				</b>
			</xsl:otherwise>
		</xsl:choose>
		<xsl:if test="CU">
			<table id="My CU table" style="behavior:url('{$XSLDirectory}\tableAct.htc');BORDER: black 1px solid; WIDTH: 99%; background-color:#eeeecc;" borderColor="#999999" cellSpacing="0" cellPadding="2" border="1" dragcolor="gray" slcolor="#ffffcc" hlcolor="#BEC5DE">
				<xsl:apply-templates select="CU[1]" mode="heading"/>
				<tbody>
					<!--xsl:for-each select="CU"-->
					<xsl:for-each select="CU[count(. | key('cu-by-wmscode-workfunction', concat(generate-id(..),'_',WMS_CODE,'_',WORK_FUNCTION))[1]) = 1]">
						<xsl:sort order="ascending" data-type="text" select="WMS_CODE"/>
						<xsl:apply-templates select="." mode="rows"/>
					</xsl:for-each>
				</tbody>
			</table>
		</xsl:if>
	</xsl:template>
	<xsl:template match="CU" mode="heading">
		<thead>
			<tr align="center">
				<td width="300">
					<b>Compatible Unit Code</b>
				</td>
				<td width="300">
					<b>Compatible Unit Description</b>
				</td>
				<td width="100">
					<b>Work Function</b>
				</td>
				<td width="100">
					<b>Quantity</b>
				</td>
				<td width="100">
					<b>Length</b>
				</td>
			</tr>
		</thead>
	</xsl:template>
	
	<xsl:template match="CU" mode="rows">
		<!--xsl:for-each select=".[count(. | key('cu-by-wmscode-workfunction', concat(WMS_CODE,'_',WORK_FUNCTION))[1]) = 1]"-->
		<!--xsl:sort select="WMS_CODE" /-->
		<tr>
			<td width="300" align="left">
				<xsl:value-of select="WMS_CODE"/>
				&#160;
			</td>
			<td width="300" align="left">
				<xsl:value-of select="DESCRIPTION"/>
				&#160;
			</td>
			<td width="150" align="center">
				<xsl:choose>
					<xsl:when test="WORK_FUNCTION='0'">None</xsl:when>
					<xsl:when test="WORK_FUNCTION='1'">Install</xsl:when>
					<xsl:when test="WORK_FUNCTION='2'">Remove</xsl:when>
					<xsl:when test="WORK_FUNCTION='16'">Retire</xsl:when>
					<xsl:when test="WORK_FUNCTION='32'">Abandon</xsl:when>
				</xsl:choose>
				&#160;
			</td>
			<td width="100" align="center">
				<xsl:choose>
					<xsl:when test="UNIT_OF_MEASURE='0'">
						<xsl:value-of select="sum(key('cu-by-wmscode-workfunction', concat(generate-id(..),'_',WMS_CODE,'_',WORK_FUNCTION))/QUANTITY)"/>			
					</xsl:when>
					<xsl:otherwise>
						1
					</xsl:otherwise>
				</xsl:choose>
			</td>
			<td width="100" align="center">
				<xsl:if test="LENGTH!=''">
					<xsl:value-of select="format-number(sum(key('cu-by-wmscode-workfunction', concat(generate-id(..),'_',WMS_CODE,'_',WORK_FUNCTION))/LENGTH),'#.##','engUS')"/>
					<xsl:choose>
						<xsl:when test="UNIT_OF_MEASURE='0'"> -- </xsl:when>
						<xsl:when test="UNIT_OF_MEASURE='50'"> mi.</xsl:when>
						<xsl:when test="UNIT_OF_MEASURE='51'"> ft.</xsl:when>
						<xsl:when test="UNIT_OF_MEASURE='52'"> in.</xsl:when>
						<xsl:when test="UNIT_OF_MEASURE='100'"> km.</xsl:when>
						<xsl:when test="UNIT_OF_MEASURE='101'"> m.</xsl:when>
						<xsl:when test="UNIT_OF_MEASURE='102'"> cm.</xsl:when>
						<xsl:otherwise> unknown units </xsl:otherwise>
					</xsl:choose>
				</xsl:if>
				&#160;
			</td>
		</tr>
	</xsl:template>
	
	<xsl:template match="ADDRESS">
		<div style="margin-left:0.3in">
			<table border="0" width="500">
				<tr>
					<td valign="top" width="150">Address:</td>
					<td width="350">
						<xsl:if test="ADDRESS1!=''">
							<xsl:value-of select="ADDRESS1"/>
							<br/>
						</xsl:if>
						<xsl:if test="ADDRESS2!=''">
							<xsl:value-of select="ADDRESS2"/>
							<br/>
						</xsl:if>
						<xsl:choose>
							<xsl:when test="CITY!=''">
								<xsl:value-of select="CITY"/>
								<xsl:if test="STATE!=''">, <xsl:value-of select="STATE"/>
								</xsl:if>&#160;<xsl:value-of select="ZIP"/>
							</xsl:when>
							<xsl:otherwise>
								<xsl:value-of select="STATE"/>&#160;<xsl:value-of select="ZIP"/>
							</xsl:otherwise>
						</xsl:choose>
					</td>
				</tr>
			</table>
		</div>
	</xsl:template>
	
	<xsl:template match="CUSTOMER">
		<br/>
		<b>Customer Information:</b>
		<div style="margin-left:0.3in">
			<table border="0" width="500">
				<tr>
					<td width="150">Customer:</td>
					<td width="350">
						<xsl:value-of select="FIRSTNAME"/>&#160;<xsl:value-of select="LASTNAME"/>
					</td>
				</tr>
				<tr>
					<td width="150">Contact Email:</td>
					<td width="350">
						<a>
							<xsl:attribute name="href">
					mailto:<xsl:value-of select="EMAIL"/></xsl:attribute>
							<xsl:value-of select="EMAIL"/>
						</a>
					</td>
				</tr>
			</table>
		</div>
		<xsl:apply-templates select="ADDRESS"/>
	</xsl:template>
</xsl:stylesheet>
