<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<!-- CostReport_8_2.xsl :  This style sheet creates cost reports for Workflow Manager 8.2
It is applied to design xmls that have the work request address, customer information,
and cost information added under the workrequest node.-->

	<xsl:param name="XSLDirectory" select="'D:\dev\Bin\Style Sheets'"/>
	<!-- Output method is set to xml so the MSXML 3.0 SP1 Parser won't insert an unclosed META tag -->
	<xsl:output method="xml" omit-xml-declaration="yes" encoding="UTF-8"/>
	
	<xsl:template match="WORKREQUESTTOPLEVEL">
		<html>
			<head>
				<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />				
			</head>
			<body oncontextmenu="return true">
				<h2 align="center">Design Cost Report</h2>	
					
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
		<xsl:apply-templates select="DESIGN/COST_SUMMARY/TOTAL"/>
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
	</xsl:template>

	
	<xsl:template match="COST_SUMMARY/TOTAL">
		<br/>
		<div style="margin-left:0.3in">
		<table id="My cost table" style="BORDER: black 1px solid; table-layout:fixed"
			width="670" borderColor="#999999" cellSpacing="0" cellPadding="2" border="1" slcolor="#ffffcc" hlcolor="#BEC5DE">
			<colgroup span="2">
				<col width="60"/>
				<col width="610"/>
			</colgroup>
			<tr>
				<th align="center" valign="middle">Debits:</th>
				<td align="center">
					<table id="My debit table" style="behavior:url('{$XSLDirectory}\tableAct.htc');BORDER: black 1px solid; background-color:#eeeecc; table-layout:fixed"
						width="600" borderColor="#999999" cellSpacing="0" cellPadding="2" border="1"  slcolor="#ffffcc" hlcolor="#BEC5DE">
						<tr align="center">		
							<td width="150"><b>Work Function</b></td>
							<td width="150"><b>Material</b></td>
							<td width="150"><b>Labor</b></td>
							<td width="150"><b>Total</b></td>
						</tr>
						<tbody>
							<xsl:apply-templates select="DEBITS"/>
						</tbody>
					</table>
				</td>
			</tr>
			<tr>
				<th width="50" align="center" valign="middle">Credits:</th>
				<td align="center">
					<table id="My credit table" style="behavior:url('{$XSLDirectory}\tableAct.htc');BORDER: black 1px solid; background-color:#eeeecc; table-layout:fixed"
						width="600" borderColor="#999999" cellSpacing="0" cellPadding="2" border="1" slcolor="#ffffcc" hlcolor="#BEC5DE">
						<tbody>
							<xsl:apply-templates select="CREDITS"/>
						</tbody>
					</table>
				</td>
			</tr>
			<tr>
				<th width="50" align="center" valign="middle">Totals:</th>
				<td align="center">
					<table id="My total table" style="behavior:url('{$XSLDirectory}\tableAct.htc');BORDER: black 1px solid; background-color:#eeeecc; table-layout:fixed"
						width="600" borderColor="#999999" cellSpacing="0" cellPadding="2" border="1" slcolor="#ffffcc" hlcolor="#BEC5DE">
						<tbody>
							<xsl:apply-templates select="BEAN"/>
						</tbody>
					</table>
				</td>
			</tr>
		</table>
		</div>
	</xsl:template>
		
	<xsl:template match="DEBITS">
		<xsl:apply-templates select="BEAN"/>
		<xsl:apply-templates select="TOTAL"/>
	</xsl:template>
	
	<xsl:template match="CREDITS|TOTAL">
		<xsl:apply-templates select="BEAN"/>		
	</xsl:template>

	<xsl:template match="BEAN">			
		<tr>
			<xsl:choose>
				<xsl:when test="NAME!='' and NAME!='Total'">
					<td>
						<xsl:value-of select="NAME"/>
					</td>
				</xsl:when>
				<xsl:otherwise>
					<td style="background-color:#999999">
						&#160;
					</td>
				</xsl:otherwise>
			</xsl:choose>
			
			<xsl:choose>
				<xsl:when test="MATERIAL_COST!=''">
					<td align="right">
						<xsl:value-of select="MATERIAL_COST"/>
					</td>
				</xsl:when>
				<xsl:otherwise>
					<td style="background-color:#999999">
						&#160;
					</td>		
				</xsl:otherwise>
			</xsl:choose>

			<xsl:choose>
				<xsl:when test="LABOR_COST!=''">
					<td align="right">
						<xsl:value-of select="LABOR_COST"/>
					</td>
				</xsl:when>
				<xsl:otherwise>
					<td style="background-color:#999999">
						&#160;
					</td>		
				</xsl:otherwise>
			</xsl:choose>

			<xsl:choose>
				<xsl:when test="AMOUNT!=''">
					<td align="right">
						<xsl:value-of select="AMOUNT"/>
					</td>
				</xsl:when>
				<xsl:otherwise>
					<td style="background-color:#999999">
						&#160;
					</td>		
				</xsl:otherwise>
			</xsl:choose>
		</tr>
	</xsl:template>

	<xsl:template match="ADDRESS">
		<div style="margin-left:0.3in">
		<table  border="0" width="500">
			<tr>
				<td valign="top" width="150">Address:</td>
				<td width="350">
					<xsl:if test="ADDRESS1!=''">
						<xsl:value-of select="ADDRESS1"/><br/>
					</xsl:if>
					<xsl:if test="ADDRESS2!=''">
						<xsl:value-of select="ADDRESS2"/><br/>
					</xsl:if>
					<xsl:choose>
						<xsl:when test="CITY!=''">
							<xsl:value-of select="CITY"/><xsl:if test="STATE!=''">, <xsl:value-of select="STATE"/></xsl:if>&#160;<xsl:value-of select="ZIP"/>
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
			<xsl:if test="FIRSTNAME!='' or LASTNAME!=''">
				<tr>
					<td width="150">Customer:</td>
					<td width="350">
						<xsl:value-of select="FIRSTNAME"/>&#160;<xsl:value-of select="LASTNAME"/>
					</td>
				</tr>
			</xsl:if>
			<xsl:if test="EMAIL!=''">
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
			</xsl:if>
		</table>
		</div>
		<xsl:apply-templates select="ADDRESS"/>
	</xsl:template>
	
</xsl:stylesheet>
