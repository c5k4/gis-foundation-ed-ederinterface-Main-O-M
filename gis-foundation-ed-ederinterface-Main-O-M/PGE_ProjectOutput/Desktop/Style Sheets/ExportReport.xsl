<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<!-- ExportReport.xsl - creates an html report when it is applied by the ArcFM XML Exporter. 
The user should save the target file as an html file.-->

<!-- Output method is set to xml so the MSXML 3.0 SP1 Parser won't insert an unclosed META tag -->
<xsl:output method="xml" omit-xml-declaration="yes" encoding="UTF-8"/>

<xsl:template match="/">
	<html>
		<head>
			<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
			<style>
				<!-- Primary Headings -->
      				.ph1 {margin-left:0.2in; font-weight:bold; color:#0000FF; cursor:hand}
      				.ph2 {margin-left:0.25in; font-weight:normal; color:#0000FF; cursor:hand}
				<!-- General text and list items -->
				.gt {margin-left:0.3in; font-weight:normal; font-style="normal"; color:#000000}
				.gbt {margin-left:0.3in; font-weight:bold; color:#000000}
				.li {font-weight:normal; color:#191970}
				.li2 {font-weight:normal; font-style:italic; color:#191970}
    			</style>

       		<script type="text/javascript" language="JavaScript"><xsl:comment><![CDATA[

			       function blocking(e)
			      {
				  var Group = e.children[0];
				  current = (Group.style.display == 'block') ? 'none' : 'block';
				  Group.style.display = current;
				  window.event.cancelBubble = true;
			       }
			
				function Highlight()
				{
				  var elem=window.event.srcElement;
				  color=elem.style.color;
				  elem.style.color="#696969";
				}
				
				function UnHighlight()
				{
				  var elem=window.event.srcElement;
				  elem.style.color=color;
				}

			
			]]></xsl:comment></script>	
		</head>

		<body>
			<h2 align="center">ArcFM Properties Report</h2>
			<xsl:if test="GXXML/DBPROPERTIES">
				<div style="margin-left:0.2in" class="ph1" onclick="blocking(this)" onmouseover="Highlight()" onmouseout="UnHighlight()">
					Database Properties
					<div style="display:none">
						<xsl:apply-templates select="GXXML/DBPROPERTIES"/>
					</div>
				</div>
			</xsl:if>

			<xsl:if test="GXXML/FEATURECLASS">
				<br />
				<div style="margin-left:0.2in" class="ph1" onclick="blocking(this)" onmouseover="Highlight()" onmouseout="UnHighlight()">
					Properties
					<div style="display:none">
						<xsl:apply-templates select="GXXML/FEATURECLASS">
							<xsl:sort select="FEATURENAME"/>
						</xsl:apply-templates>
					</div>
				</div>
			</xsl:if>
			
			<xsl:if test="GXXML/MODELNAMES">
				<br />
				<div style="margin-left:0.2in" class="ph1" onclick="blocking(this)" onmouseover="Highlight()" onmouseout="UnHighlight()">
					Model Names
					<div style="display:none">
						<xsl:apply-templates select="GXXML/MODELNAMES">
							<xsl:sort select="OBJECTCLASSNAME"/>
						</xsl:apply-templates>
					</div>
				</div>
			</xsl:if>

			<xsl:if test="GXXML/SNAPELEMENT">
				<br />
				<div style="margin-left:0.2in" class="ph1" onclick="blocking(this)" onmouseover="Highlight()" onmouseout="UnHighlight()">
					Snapping
					<div style="display:none">
						<xsl:apply-templates select="GXXML/SNAPELEMENT">
							<xsl:sort select="SNAPENTRY/SNAPFEATURECLASS"/>
						</xsl:apply-templates>
					</div>
				</div>
			</xsl:if>

			<xsl:if test="GXXML/DOMAINS">
				<br />
				<div style="margin-left:0.2in" class="ph1" onclick="blocking(this)" onmouseover="Highlight()" onmouseout="UnHighlight()">
					Domains
					<div style="display:none">
						<xsl:apply-templates select="GXXML/DOMAINS"/>
					</div>
				</div>
			</xsl:if>
		
			<xsl:if test="GXXML/RELATIONSHIPCLASS">
				<br />
				<div style="margin-left:0.2in" class="ph1" onclick="blocking(this)" onmouseover="Highlight()" onmouseout="UnHighlight()">
					Relationship Classes
					<div style="display:none">
						<xsl:apply-templates select="GXXML/RELATIONSHIPCLASS"/>
					</div>
				</div>
			</xsl:if>
			
			<xsl:if test="GXXML/XML_NetWorks">
				<br />
				<div style="margin-left:0.2in" class="ph1" onclick="blocking(this)" onmouseover="Highlight()" onmouseout="UnHighlight()">
					Geometric Networks
					<div style="display:none">
						<xsl:apply-templates select="GXXML/XML_NetWorks"/>
					</div>
				</div>
			</xsl:if>
			
			<xsl:if test="GXXML/OBJECTCLASSINFO">
				<br />
				<div style="margin-left:0.2in" class="ph1" onclick="blocking(this)" onmouseover="Highlight()" onmouseout="UnHighlight()">
					Custom Object Types
					<div style="display:none">
						<xsl:apply-templates select="GXXML/OBJECTCLASSINFO"/>
					</div>
				</div>
			</xsl:if>
		
		</body>
	</html>
</xsl:template>


<xsl:template match="GXXML/DBPROPERTIES">				
	<div style="margin-left:0.25in" class="gt">
		<i>Non-GIS CU Extended Data Definition Table:  </i><xsl:value-of select="EDMDBPROPERTIES/@EDMTABLENAME"/>	
	</div>
</xsl:template>


<xsl:template match="GXXML/FEATURECLASS">				
	<div style="margin-left:0.25in" class="ph2" onclick="blocking(this)">
		<xsl:value-of select="FEATURENAME"/>
		<div style="display:none" class="gt">
			<i class="li">ArcFM Display Field:  </i><xsl:value-of select="PRIMARYDISPLAYFIELD"/>
			<br />
			
			<!-- Field aliases -->
			<div class="li2" onclick="blocking(this)">
				Field Aliases
				<div style="display:none" class="gt" >
					<table border="1" cellpadding="5">
						<caption class="gbt">Field Aliases</caption>  
						<tr>
							<th>Field Name</th>
							<th>Field Alias</th>
						</tr>
						<xsl:for-each select="SUBTYPE[1]/FIELD">
							<tr>
								<td><xsl:value-of select="FIELDNAME"/></td>
								<td><xsl:value-of select="FIELDALIAS"/></td>
							</tr>
						</xsl:for-each>	
					</table>
					<br />
				</div>
			</div>
			
			<!-- Subtypes -->
			<div class="li2" onclick="blocking(this)">
				Subtypes
				<div style="display:none" class="gt">
					<xsl:for-each select="SUBTYPE">
						<xsl:sort data-type="number" select="SUBTYPECODE"/>
						<div class="gt" onclick="blocking(this)">
							Subtype Code:  <xsl:value-of select="SUBTYPECODE"/>
							<div style="display:none" class="gt">
								<i>Create Edit Task: </i><xsl:value-of select="EDITTASKNAME"/><br />
								<xsl:for-each select="AUTOVALUE">
									<xsl:choose>
										<xsl:when test="@EDITEVENT[.='mmEventFeatureCreate']">
											<i>On Create Event:  </i><xsl:value-of select="AUTOVALUENAME"/><br />
										</xsl:when>
										<xsl:when test="@EDITEVENT[.='mmEventFeatureUpdate']">
											<i>On Update Event:  </i><xsl:value-of select="AUTOVALUENAME"/><br />
										</xsl:when>
										<xsl:when test="@EDITEVENT[.='mmEventFeatureDelete']">
											<i>On Delete Event:  </i><xsl:value-of select="AUTOVALUENAME"/><br />
										</xsl:when>
										<xsl:when test="@EDITEVENT[.='mmEventFeatureSplit']">
											<i>On Split Event:  </i><xsl:value-of select="AUTOVALUENAME"/><br />
										</xsl:when>
										<xsl:when test="@EDITEVENT[.='mmEventRelatedObjectChanged']">
											<i>Related Object Changed Event:  </i><xsl:value-of select="AUTOVALUENAME"/><br />
										</xsl:when>
										<xsl:when test="@EDITEVENT[.='mmEventRelatedObjectMoved']">
											<i>Related Object Moved Event:  </i><xsl:value-of select="AUTOVALUENAME"/><br />
										</xsl:when>
										<xsl:when test="@EDITEVENT[.='mmEventRelatedObjectRotated']">
											<i>Related Object Rotated Event:  </i><xsl:value-of select="AUTOVALUENAME"/><br />
										</xsl:when>
										<xsl:when test="@EDITEVENT[.='mmEventNetworkFeatureConnect']">
											<i>Network Feature Connect Event:  </i><xsl:value-of select="AUTOVALUENAME"/><br />
										</xsl:when>
										<xsl:when test="@EDITEVENT[.='mmEventNetworkFeatureDisconnect']">
											<i>Network Feature Disconnect Event:  </i><xsl:value-of select="AUTOVALUENAME"/><br />
										</xsl:when>
										<xsl:when test="@EDITEVENT[.='mmEventEditMetadata']">
											<i>Metadata Editor:  </i><xsl:value-of select="AUTOVALUENAME"/><br />
										</xsl:when>
										<xsl:when test="@EDITEVENT[.='mmEventAbandon']">
											<i>On Abandon Event:  </i><xsl:value-of select="AUTOVALUENAME"/><br />
										</xsl:when>
										<xsl:when test="@EDITEVENT[.='mmEventCustomEditor']">
											<i>Custom Configuration Editor:  </i><xsl:value-of select="AUTOVALUENAME"/><br />
										</xsl:when>
										<xsl:when test="@EDITEVENT[.='mmEventBeforeFeatureSplit']">
											<i>Before Split Event:  </i><xsl:value-of select="AUTOVALUENAME"/><br />
										</xsl:when>
										<xsl:when test="@EDITEVENT[.='mmEventAfterFeatureSplit']">
											<i>After Split Event:  </i><xsl:value-of select="AUTOVALUENAME"/><br />
										</xsl:when>
										<xsl:when test="@EDITEVENT[.='mmEventExtendedDataDefinitionTable']">
											<i>Extended Data Definition Table:  </i><xsl:value-of select="AVEDMTABLENAME"/><br />
										</xsl:when>
									</xsl:choose>
								</xsl:for-each>
								<xsl:for-each select="SIMPLEVARIANT">
									<i><xsl:value-of select="VARIANTPROPTYPE"/>:  </i><xsl:value-of select="VARIANTPROPVALUE"/><br />
								</xsl:for-each>	
								
								<table border="1" cellpadding="5">
									<tr>
							        		<th>Field Alias</th>
							        		<th>Visible</th>
								  		<th>Editable</th><br />
							        		<th>Allow Null Values</th>
							        		<th>CU Defining</th>
										<th>Clear After Create</th>
										<th>On Feature Create</th>
										<th>On Feature Update</th>
							      		</tr>
									<xsl:for-each select="FIELD">
										<tr>
											<td><xsl:value-of select="FIELDALIAS"/></td>
											
											<td>
												<xsl:for-each select="SIMPLESETTING">
													<xsl:if test="@SETTINGTYPE[.='mmFSVisible']">
														<xsl:value-of select="SETTINGVALUE"/>
													</xsl:if>
												</xsl:for-each>
												&#160;	
											</td>
											
											<td>
												<xsl:for-each select="SIMPLESETTING">
													<xsl:if test="@SETTINGTYPE[.='mmFSEditable']">
														<xsl:value-of select="SETTINGVALUE"/>
													</xsl:if>
												</xsl:for-each>
												&#160;
											</td>
											
											<td>
												<xsl:for-each select="SIMPLESETTING">
													<xsl:if test="@SETTINGTYPE[.='mmFSAllowNulls']">
														<xsl:value-of select="SETTINGVALUE"/>
													</xsl:if>
												</xsl:for-each>
												&#160;
											</td>
											
											<td>
												<xsl:for-each select="SIMPLESETTING">
													<xsl:if test="@SETTINGTYPE[.='mmFSCUDefined']">
														<xsl:value-of select="SETTINGVALUE"/>
													</xsl:if>											
												</xsl:for-each>
												&#160;
											</td>
											
											<td>
												<xsl:for-each select="SIMPLESETTING">
													<xsl:if test="@SETTINGTYPE[.='mmFSClearOnCreate']">
														<xsl:value-of select="SETTINGVALUE"/>
													</xsl:if>											
												</xsl:for-each>
												&#160;
											</td>
											
											<td>
												<xsl:for-each select="AUTOVALUE ">
													<xsl:if test="@EDITEVENT[.='mmEventFeatureCreate']">
														<xsl:value-of select="AUTOVALUENAME"/>
													</xsl:if>											
												</xsl:for-each>
												&#160;
											</td>
											
											<td>
												<xsl:for-each select="AUTOVALUE ">
													<xsl:if test="@EDITEVENT[.='mmEventFeatureUpdate']">
														<xsl:value-of select="AUTOVALUENAME"/>
													</xsl:if>											
												</xsl:for-each>
												&#160;
											</td>																	
										</tr>
									</xsl:for-each>
								</table>
								<br></br>
								<br />
							</div>
						</div>
					</xsl:for-each>			
				</div>
				<br />
			</div>
		</div>
	</div>
</xsl:template>


<xsl:template match="GXXML/MODELNAMES">				
	<div style="margin-left:0.25in" class="ph2" onclick="blocking(this)">
		<xsl:if test="DATABASE">
			Database
			<div style="display:none" class="gt">
				<table border="1" cellpadding="5"> 
					<tr>
						<th>Model Names</th>
					</tr>
					<xsl:for-each select="DATABASE/MODELNAME">
						<tr>
							<td><xsl:value-of select="."/></td>
						</tr>
					</xsl:for-each>	
				</table>
				<br />
			</div>
		</xsl:if>
		
		<xsl:if test="OBJECTCLASSNAME">
			<xsl:value-of select="OBJECTCLASSNAME"/>
			<div style="display:none" class="gt">
				<table border="1" cellpadding="5">
				<caption class="gbt">Object Class Model Names</caption>  
					<tr>
						<th>Object Class</th>
						<th>Model Name</th>
					</tr>
					<xsl:for-each select="CLASSNAMES/NAME">
						<tr>
							<td><xsl:value-of select="."/></td>
							<td><xsl:value-of select="following-sibling::MODELNAME[1]"/></td>
						</tr>
					</xsl:for-each>	
				</table>
				<br />
				<table border="1" cellpadding="5">
				<caption class="gbt">Field Model Names</caption>  
					<tr>
						<th>Field</th>
						<th>Model Name</th>
					</tr>
					<xsl:for-each select="FIELDNAMES/NAME">
						<tr>
							<td><xsl:value-of select="."/></td>
							<td><xsl:value-of select="following-sibling::MODELNAME[1]"/></td>
						</tr>
					</xsl:for-each>	
				</table>
			</div>
		</xsl:if>
		<br />
	</div>
</xsl:template>


<xsl:template match="GXXML/SNAPELEMENT">
	<xsl:variable name="unique-snapfeature" select="SNAPENTRY/SNAPFEATURECLASS[not(.=preceding::SNAPENTRY/SNAPFEATURECLASS)]"/>
	<div style="margin-left:0.25in" class="ph2" onclick="blocking(this)">
		<xsl:for-each select="$unique-snapfeature">
			<xsl:value-of select="."/>
			<div style="display:none" class="gt">
				<table border="1" cellpadding="5">
					<tr>
						<th>Subtype</th>
						<th>Snap To Feature</th>
						<th>Snap To Subtype</th>
						<th>Hit Type</th>
						<th>Tolerance</th>
					</tr>
					<xsl:for-each select="//GXXML/SNAPELEMENT/SNAPENTRY/SNAPFEATURECLASS[.=current()]">
						<tr>
							<td><xsl:value-of select="following-sibling::SNAPSUBTYPE"/>&#160;</td>
							<td><xsl:value-of select="following-sibling::SNAPTOFEATURECLASS"/>&#160;</td>
							<td><xsl:value-of select="following-sibling::SNAPTOSUBTYPE"/>&#160;</td>
							<td><xsl:value-of select="following-sibling::HITTYPE"/>&#160;</td>
							<td><xsl:value-of select="following-sibling::TOLERANCE"/>&#160;</td>
						</tr>
					</xsl:for-each>
				</table>
			</div>
		</xsl:for-each>
		<br />
	</div>
</xsl:template>


<xsl:template match="GXXML/DOMAINS">
	<xsl:for-each select="DOMAIN">	
		<xsl:sort select="NAME"/>			
		<div style="margin-left:0.25in" class="ph2" onclick="blocking(this)">
			<xsl:value-of select="NAME"/>
			<div style="display:none" class="gt">
				<i class="li">Domain ID:  </i><xsl:value-of select="DOMAINID"/><br />
				<i class="li">Type:  </i><xsl:value-of select="TYPE"/><br />
				<i class="li">Field Type:  </i><xsl:value-of select="FIELDTYPE"/><br />
				<i class="li">Merge Policy:  </i><xsl:value-of select="MERGEPOLICY"/><br />
				<i class="li">Split Policy:  </i><xsl:value-of select="SPLITPOLICY"/><br />
				<br />
				<table border="1" cellpadding="5">
				<caption class="gbt">Domain Values</caption>  
					<tr>
						<th>Name</th>
						<th>Value</th>
					</tr>
					<xsl:for-each select="CODEDVALUEELEMENT">
						<tr>
							<td><xsl:value-of select="CODENAME"/></td>
							<td><xsl:value-of select="CODEVALUE"/></td>
						</tr>
					</xsl:for-each>	
				</table>
			</div>
			<br />
		</div>
	</xsl:for-each>
</xsl:template>

<!-- Relationship Classes added by J. Johnson7-18-02... right before the release :P -->
<xsl:template match="GXXML/RELATIONSHIPCLASS">				
	<div style="margin-left:0.25in" class="ph2" onclick="blocking(this)">
		<xsl:value-of select="NAME"/>
		<div style="display:none" class="gt">
			<xsl:for-each select="AUTOVALUE">
				<xsl:choose>
					<xsl:when test="@EDITEVENT[.='mmEventRelationshipCreated']">
						<i>On Relationship Created Event:  </i><xsl:value-of select="AUTOVALUENAME"/><br />
					</xsl:when>
					<xsl:when test="@EDITEVENT[.='mmEventRelationshipUpdated']">
						<i>On Relationship Updated Event:  </i><xsl:value-of select="AUTOVALUENAME"/><br />
					</xsl:when>
					<xsl:when test="@EDITEVENT[.='mmEventRelationshipDeleted']">
						<i>On Relationship Deleted Event:  </i><xsl:value-of select="AUTOVALUENAME"/><br />
					</xsl:when>
				</xsl:choose>
			</xsl:for-each>
			<br />				
		</div>
	</div>
</xsl:template>

<xsl:template match="GXXML/XML_NetWorks">
	<xsl:for-each select="NETWORKNAME">	
		<xsl:sort select="NETWORKNAME"/>			
		<div style="margin-left:0.25in" class="ph2" onclick="blocking(this)">
			<xsl:value-of select="text()[1]"/>
			<div style="display:none" class="gt">
				<i class="li">Feature Dataset Name:  </i><xsl:value-of select="FEATUREDATASETNAME"/><br />
				<div  class="li2" onclick="blocking(this)">
					Feature Classes
					<div style="display:none"  class="gt">
						<table border="1" cellpadding="5" style="text-align:center">
							<tr>
								<th>Feature Class</th>
								<th>Role</th>
								<th>Enabled</th>
								<th>Ancillary Role</th>
								<th>Ancillary Field Name</th>
							</tr>
							<xsl:for-each select="FEATURECLASSES/FEATURCLASSESPROPERTIES/FEATURECLASSNAME">
								<tr>
									<td><xsl:value-of select="text()[1]"/></td>
									<td><xsl:value-of select="FEATURECLASSTYPE"/>&#160;</td>							
									<xsl:choose>
										<xsl:when test="ENABLEDDISABLEDFIELD='ENABLED'">
											<td>True</td>
										</xsl:when>
										<xsl:otherwise>
											<td>False</td>
										</xsl:otherwise>
									</xsl:choose>
									<td><xsl:value-of select="ANCILLARYROLE"/>&#160;</td>
									<td><xsl:value-of select="ANCILLARYFIELDNAME"/>&#160;</td>
								</tr>
							</xsl:for-each>	
						</table>
					</div>
					<br />
				</div>
				
				<div  class="li2" onclick="blocking(this)">
					Weights
					<div style="display:none"  class="gt">
						<xsl:for-each select="NUMBEROFWEIGHTS/WEIGHT">
							<div class="gt" onclick="blocking(this)">
								<xsl:value-of select="text()[1]"/>
								<div style="display:none"  class="gt">
									Type:  <xsl:value-of select="WEIGHTTYPE"/><br />
									Bitgate Size:  <xsl:value-of select="BITGATESIZE"/>
									<table border="1" cellpadding="5" style="text-align:center">
										<tr>
											<th>Feature Class</th>
											<th>Associated Field</th>
										</tr>
										<xsl:for-each select="TABLES_FIELDS/NETWORKTABLE">
											<tr>
												<td><xsl:value-of select="text()[1]"/></td>
												<td><xsl:value-of select="NETWORKFIELD"/>&#160;</td>							
											</tr>
										</xsl:for-each>	
									</table>
								</div>
							</div>
						</xsl:for-each>
					</div>
					<br />
				</div>
								
				<div  class="li2" onclick="blocking(this)">
					Connectivity Rules
					<div style="display:none"  class="gt">
						<xsl:apply-templates select="CONNECTIVITIES/ConnectivityRules/Edge-Edge"/>
						<xsl:apply-templates select="CONNECTIVITIES/ConnectivityRules/Edge-Junction"/>
					</div>
				</div>
				<br />
			</div>
			<br />
		</div>
	</xsl:for-each>
</xsl:template>

<xsl:template match="Edge-Edge">
	<div class="gt" onclick="blocking(this)">
		<xsl:value-of select="name(.)"/>:  <xsl:value-of select="FromClass"/>-<xsl:value-of select="ToClass"/>
		<div style="display:none" class="gt">
			<i>From Class:  </i><xsl:value-of select="FromClass"/><br />
			<i>From Subtype:  </i><xsl:value-of select="FromST"/><br />
			<i>To Class:  </i><xsl:value-of select="ToClass"/><br />
			<i>To Subtype:  </i><xsl:value-of select="ToST"/><br />
			<i>Default Class:  </i><xsl:value-of select="DefaultCls"/><br />
			<i>Default Subtype:  </i><xsl:value-of select="DefaultST"/><br />
			<i>Category:  </i><xsl:value-of select="Category"/><br />
			<table border="1" cellpadding="5" style="text-align:center">
				<tr>
					<th>Junction Class</th>
					<th>Junction Subtype</th>
				</tr>
				<xsl:for-each select="JunctionCls">
					<tr>
						<td><xsl:value-of select="text()[1]"/></td>
						<td><xsl:value-of select="following-sibling::JunctionST"/>&#160;</td>							
					</tr>
				</xsl:for-each>	
			</table>
			<br />
		</div>
	</div>
</xsl:template>

<xsl:template match="Edge-Junction">
	<div class="gt" onclick="blocking(this)">
		<xsl:value-of select="name(.)"/>:  <xsl:value-of select="EdgeCls"/>-<xsl:value-of select="JunctionCls"/>
		<div style="display:none" class="gt">
			<i>Edge Class:  </i><xsl:value-of select="EdgeCls"/><br />
			<i>Edge Subtype:  </i><xsl:value-of select="EdgeST"/><br />
			<i>Junction Class:  </i><xsl:value-of select="JunctionCls"/><br />
			<i>Junction Subtype:  </i><xsl:value-of select="JunctionST"/><br />
			<i>Edge Maximum:  </i><xsl:value-of select="EdgeMax"/><br />
			<i>Edge Minimum:  </i><xsl:value-of select="EdgeMin"/><br />
			<i>Junction Maximum:  </i><xsl:value-of select="JunctionMax"/><br />
			<i>Junction Minimum:  </i><xsl:value-of select="JunctionMin"/><br />
			<i>Category:  </i><xsl:value-of select="Category"/><br />
			<i>Default:  </i><xsl:value-of select="Default"/><br />
			<br />
		</div>
	</div>
</xsl:template>

<xsl:template match="GXXML/OBJECTCLASSINFO">	
	<div style="margin-left:0.25in" class="ph2" onclick="blocking(this)">
		<xsl:value-of select="OBJCLASSNAME"/>
		<div style="display:none" class="gt">
			<i class="li">Object Class ID Name:  </i><xsl:value-of select="OBJCLASSIDNAME"/><br />
			<i class="li">Object Class ID:  </i><xsl:value-of select="OBJCLASSID"/><br />
			<i class="li">Extension Name:  </i><xsl:value-of select="OBJCLASSEXTENSIONNAME"/><br />
			<i class="li">Extension ID:  </i><xsl:value-of select="OBJCLASSEXTENSION"/><br />
			<br />
		</div>
	</div>
</xsl:template>

</xsl:stylesheet>