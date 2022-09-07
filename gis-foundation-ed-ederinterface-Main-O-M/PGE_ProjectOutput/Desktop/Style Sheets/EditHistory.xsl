<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<!-- EditHistory.xsl :  This style sheet creates Edit History reports for XML mobile packets
It is applied to the edit log parts of xml mobile packets.-->

	<!-- Output method is set to xml so the MSXML 3.0 SP1 Parser won't insert an unclosed META tag -->
	<xsl:output method="xml" omit-xml-declaration="yes" encoding="UTF-8"/>
	<xsl:param name="XSLDirectory" select="'D:\dev\Bin\Style Sheets'"/>
	<xsl:param name="XSLImagePath" select="'D:\dev\Bin\Bitmaps'"/>
    <xsl:param name="PxNodeIsOpen" select="False"/>
	
	<xsl:template match="session">
		<html>
			<head>
				<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />				
			</head>
			<title>Edit History</title>
			<body oncontextmenu="return true">
				<h2 align="center">Edit Log</h2>
			    <p>The Edit Log displays edits in the currently open session or design. Use the Zoom and Highlight buttons to view new and modified features in the map.</p>
				<xsl:if test="$PxNodeIsOpen='False'">
					<p><strong>Note: You must open a session or design in order to zoom to or highlight individual edit entries</strong></p>
				</xsl:if>
				<table id="My CONFLICT table" style="behavior:url('{$XSLDirectory}\tableAct.htc');BORDER: black 1px solid; WIDTH: 99%; background-color:#eeeecc;" borderColor="#999999" cellSpacing="0" cellPadding="2" border="1" dragcolor="gray" slcolor="#ffffcc" hlcolor="#BEC5DE">
					<tr>
                        <xsl:if test="$PxNodeIsOpen='True'">
							<th align="center">Zoom</th>
							<th align="center">Highlight</th>
						</xsl:if>
						<th align="left">Table</th>
						<th align="center">Operation</th>
						<th>Fields</th>
					</tr>

					<xsl:apply-templates select="georow"/>

				</table>
			</body>
		</html>
	</xsl:template>
	
	<xsl:template match="georow">
			<xsl:if test="@table[.!='']">
			<xsl:variable name="OID"><xsl:value-of select="@oid"/></xsl:variable>
			<xsl:variable name="TABLENAME"><xsl:value-of select="@table"/></xsl:variable>
            <xsl:variable name="TABLEALIAS"><xsl:value-of select="@tablealias"/></xsl:variable>
			<xsl:variable name="SERVEROID"><xsl:value-of select="@svroid"/></xsl:variable>
			<xsl:variable name="OP"><xsl:value-of select="@op"/></xsl:variable>
			<tr valign="top">
                 <xsl:if test="$PxNodeIsOpen='True'">
					<td align="center">
						<xsl:if test="@op[.!='delete']">
						<xsl:if test="@op[.!='delete attributed relationship']">
						<xsl:if test="@op[.!='new relationship']">
						  	<a href="http://zoom?OID={$OID}&amp;TABLE={$TABLENAME}&amp;SERVEROID={$SERVEROID}&amp;OPERATION={$OP}">
						  	<img border="0" src="{$XSLImagePath}/ZoomIn.gif"/>
						  	</a>
						</xsl:if>
						</xsl:if>
						</xsl:if>
					</td>
					<td align="center">
						<xsl:if test="@op[.!='delete']">
						<xsl:if test="@op[.!='delete attributed relationship']">
						<xsl:if test="@op[.!='new relationship']">
							<a href="http://highlight?OID={$OID}&amp;TABLE={$TABLENAME}&amp;SERVEROID={$SERVEROID}&amp;OPERATION={$OP}">
							<img border="0" src="{$XSLImagePath}/Flash.gif"/>
							</a>
						</xsl:if>
						</xsl:if>
						</xsl:if>
					</td>
				</xsl:if>
				<td id="georow" class="Table" valign="top"><xsl:copy-of select="$TABLEALIAS" /></td>
				<td id="georow" class="Operation" align="center" valign="top"><xsl:copy-of select="$OP" /></td>
				<xsl:if test="@op[.='modify'] or @op[.='modify relationship origin'] or @op[.='modify relationship destination']">
					<td id="georow" class="Fields" valign="top"><xsl:apply-templates select="field"/></td>
				</xsl:if>
			</tr>
			</xsl:if>
	</xsl:template>
	
	<xsl:template match="field">
			<xsl:value-of select="@name"/>
			<xsl:if test="@name[.!='SHAPE']">=<xsl:value-of select="."/></xsl:if>
			<br/>
	</xsl:template>
</xsl:stylesheet>
