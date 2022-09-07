<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<!-- Inventory_9_1_2.xsl :  This style sheet creates conflict reports for XML packets
It is applied to conflict data returned by EditLogMonitor translated into XML form.-->
	<xsl:decimal-format name="engUS" decimal-separator="." grouping-separator=","/>
	<xsl:param name="XSLDirectory" select="'D:\dev\Bin\Style Sheets'"/>
	<!-- Output method is set to xml so the MSXML 3.0 SP1 Parser won't insert an unclosed META tag -->
	<xsl:output method="xml" omit-xml-declaration="yes" encoding="UTF-8"/>
	<xsl:template match="CONFLICTS">
		<html>
			<head>
				<meta http-equiv="Content-Type" content="text/html; charset=UTF-8"/>
			</head>
			<body oncontextmenu="return true">
				<h2 align="center">Conflicts Report</h2>
				<p>Conflicts were detected between the currently open session or design and the Enterprise. 
				Save the session or design to overwrite the Enterprise data. Close the session or design without saving to maintain the existing Enterprise data. 
				The Edit Log tool allows you to view edits made in the session or design.
                </p>
	                        <table id="My CONFLICT table" style="behavior:url('{$XSLDirectory}\tableAct.htc');BORDER: black 1px solid; WIDTH: 99%; background-color:#eeeecc;" borderColor="#999999" cellSpacing="0" cellPadding="2" border="1" dragcolor="gray" slcolor="#ffffcc" hlcolor="#BEC5DE">
		                <thead>
			           <tr align="center">
				   <td width="300">
					<b>Table Name</b>
				   </td>
				   <td width="300">
					<b>Object Id</b>
				   </td>
				   <td width="100">
					<b>Field Name</b>
				   </td>
				   <td width="100">
					<b>Conflict Type</b>
				   </td>
			           </tr>
		                </thead>
				<xsl:apply-templates select="CONFLICT"/>
                                </table>
    		                <br/>
			</body>
		</html>
	</xsl:template>
	<xsl:template match="CONFLICT">
		<tr>
			<td width="300" align="left">
				<xsl:value-of select="TABLE_NAME"/>
			</td>
			<td width="300" align="left">
				<xsl:value-of select="OID"/>
			</td>
			<td width="150" align="center">
                                <xsl:value-of select="FIELD_NAME"/>
			</td>
			<td width="100" align="center">
                                <xsl:value-of select="CONFLICT_TYPE"/>
			</td>
		</tr>
	</xsl:template>

</xsl:stylesheet>
