<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<!-- PxWorkflow.xsl - creates an html report when it is applied by the Px Workflow XML Exporter. 
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
			<h2 align="center">Process Framework Workflow Report</h2>
			
			<xsl:if test="/PXXML/PX_WORKFLOW/SUPPORTED_EXTENSIONS">
				<div style="margin-left:0.2in" class="ph1" onclick="blocking(this)" onmouseover="Highlight()" onmouseout="UnHighlight()">
					Supported Extensions
					<div style="display:none">
						<xsl:apply-templates select="/PXXML/PX_WORKFLOW/SUPPORTED_EXTENSIONS/EXTENSION"/>
					</div>
				</div>
			</xsl:if>

			<xsl:if test="/PXXML/PX_WORKFLOW/NODE_TYPES">
				<br />
				<div style="margin-left:0.2in" class="ph1" onclick="blocking(this)" onmouseover="Highlight()" onmouseout="UnHighlight()">
					Node Types
					<div style="display:none">
						<xsl:apply-templates select="/PXXML/PX_WORKFLOW/NODE_TYPES/NODE_TYPE">
							<xsl:sort select="EXTENSION_ID"/>
							<xsl:sort select="NAME"/>
						</xsl:apply-templates>
					</div>
				</div>
			</xsl:if>
			
			<xsl:if test="/PXXML/PX_WORKFLOW/ROLES">
				<br />
				<div style="margin-left:0.2in" class="ph1" onclick="blocking(this)" onmouseover="Highlight()" onmouseout="UnHighlight()">
					Roles
					<div style="display:none">
						<xsl:apply-templates select="/PXXML/PX_WORKFLOW/ROLES/ROLE">
							<xsl:sort select="DISPLAY_NAME"/>
						</xsl:apply-templates>
					</div>
				</div>
			</xsl:if>
			
			<xsl:if test="/PXXML/PX_WORKFLOW/STATES">
				<br />
				<div style="margin-left:0.2in" class="ph1" onclick="blocking(this)" onmouseover="Highlight()" onmouseout="UnHighlight()">
					States
					<div style="display:none">
						<xsl:apply-templates select="/PXXML/PX_WORKFLOW/STATES/STATE">
							<xsl:sort select="NODE_TYPE_ID"/>
							<xsl:sort select="NAME"/>
						</xsl:apply-templates>
					</div>
				</div>
			</xsl:if>

			<xsl:if test="/PXXML/PX_WORKFLOW/TRANSITIONS">
				<br />
				<div style="margin-left:0.2in" class="ph1" onclick="blocking(this)" onmouseover="Highlight()" onmouseout="UnHighlight()">
					Transitions
					<div style="display:none">
						<xsl:apply-templates select="/PXXML/PX_WORKFLOW/TRANSITIONS/TRANSITION">
							<xsl:sort select="DISPLAY_NAME"/>
						</xsl:apply-templates>
					</div>
				</div>
			</xsl:if>
			
			<xsl:if test="/PXXML/PX_WORKFLOW/TASKS">
				<br />
				<div style="margin-left:0.2in" class="ph1" onclick="blocking(this)" onmouseover="Highlight()" onmouseout="UnHighlight()">
					Tasks
					<div style="display:none">
						<xsl:apply-templates select="PXXML/PX_WORKFLOW/TASKS/TASK">
							<xsl:sort select="NODE_TYPE_ID"/>
							<xsl:sort select="NAME"/>
							<xsl:sort select="DISPLAY_ORDER"/>
						</xsl:apply-templates>
					</div>
				</div>
			</xsl:if>
			
			<xsl:if test="/PXXML/PX_WORKFLOW/FILTERS">
				<br />
				<div style="margin-left:0.2in" class="ph1" onclick="blocking(this)" onmouseover="Highlight()" onmouseout="UnHighlight()">
					Filters
					<div style="display:none">
						<xsl:apply-templates select="/PXXML/PX_WORKFLOW/FILTERS/FILTER"/>
					</div>
				</div>
			</xsl:if>
					
		</body>
	</html>
</xsl:template>


<xsl:template match="/PXXML/PX_WORKFLOW/SUPPORTED_EXTENSIONS/EXTENSION">				
	<div style="margin-left:0.25in" class="ph2" onclick="blocking(this)">
		<xsl:value-of select="NAME"/>
		<div style="display:none" class="gt">
			<i class="li">ID:  </i><xsl:value-of select="ID"/>
			<br />	
		</div>
	</div>
</xsl:template>


<xsl:template match="/PXXML/PX_WORKFLOW/NODE_TYPES/NODE_TYPE">				
	<div style="margin-left:0.25in" class="ph2" onclick="blocking(this)">
		<xsl:value-of select="NAME"/>
		<div style="display:none" class="gt">
			<i class="li">ID:  </i><xsl:value-of select="ID"/><br />
			<i class="li">Description:  </i><xsl:value-of select="DESCRIPTION"/><br />
			<i class="li">Extension:  </i><xsl:value-of select="/PXXML/PX_WORKFLOW/SUPPORTED_EXTENSIONS/EXTENSION[ID=current()/EXTENSION_ID]/NAME"/><br />
			<i class="li">Deleter:  </i><xsl:value-of select="DELETER_PROGID"/><br />
			<i class="li">Version Namer:  </i><xsl:value-of select="VERSION_NAMER_PROGID"/><br />
			<i class="li">Hidden:  </i><xsl:value-of select="HIDDEN"/><br />
			<br />	
		</div>
	</div>
</xsl:template>


<xsl:template match="/PXXML/PX_WORKFLOW/ROLES/ROLE">
	<div style="margin-left:0.25in" class="ph2" onclick="blocking(this)">
		<xsl:value-of select="DISPLAY_NAME"/>
		<div style="display:none" class="gt">
			<i class="li">ID:  </i><xsl:value-of select="ID"/><br />
			<i class="li">Name:  </i><xsl:value-of select="NAME"/><br />
			<i class="li">Description:  </i><xsl:value-of select="DESCRIPTION"/><br />
			<br />	
		</div>
	</div>
</xsl:template>


<xsl:template match="/PXXML/PX_WORKFLOW/STATES/STATE">
	<div style="margin-left:0.25in" class="ph2" onclick="blocking(this)">
		<xsl:value-of select="NAME"/>
		<div style="display:none" class="gt">
			<i class="li">ID:  </i><xsl:value-of select="ID"/><br />
			<i class="li">Description:  </i><xsl:value-of select="DESCRIPTION"/><br />
			<i class="li">Node Type:  </i><xsl:value-of select="/PXXML/PX_WORKFLOW/NODE_TYPES/NODE_TYPE[ID=current()/NODE_TYPE_ID]/NAME"/><br />
			<i class="li">State Number:  </i><xsl:value-of select="STATE_NUMBER"/><br />
			<i class="li">Control:  </i><xsl:value-of select="CONTROL_PROGID"/><br />		
			<br />	
		</div>
	</div>
</xsl:template>


<xsl:template match="/PXXML/PX_WORKFLOW/TRANSITIONS/TRANSITION">
	<div style="margin-left:0.25in" class="ph2" onclick="blocking(this)">
		<xsl:value-of select="DISPLAY_NAME"/>
		<div style="display:none" class="gt">
			<i class="li">ID:  </i><xsl:value-of select="ID"/><br />
			<i class="li">Name:  </i><xsl:value-of select="NAME"/><br />
			<i class="li">Description:  </i><xsl:value-of select="DESCRIPTION"/><br />
			<table style="text-align:left" >
				<tr>
					<td valign="top"><i class="li">Role(s):  </i></td>
					<td>
						<table style="text-align:left">
					      	<xsl:for-each select="/PXXML/PX_WORKFLOW/TRANSITION_ROLES/TRANSITION_ROLE[TRANSITION_ID=current()/ID]">
								<tr>
					  				<td><xsl:value-of select="/PXXML/PX_WORKFLOW/ROLES/ROLE[ID=current()/ROLE_ID]/DISPLAY_NAME"/>&#160;</td>
								</tr>
							</xsl:for-each>
					      </table>
					 </td>
				</tr>
			</table>
			<table style="text-align:left" >
				<tr>
					<td valign="top"><i class="li">From State(s):  </i></td>
					<td>
						<table style="text-align:left">
					      	<xsl:for-each select="/PXXML/PX_WORKFLOW/TRANSITION_FROM_STATES/TRANSITION_FROM_STATE[TRANSITION_ID=current()/ID]">
								<tr>
					  				<td><xsl:value-of select="/PXXML/PX_WORKFLOW/STATES/STATE[ID=current()/STATE_ID]/NAME"/>&#160;</td>
								</tr>
							</xsl:for-each>
					      </table>
					 </td>
				</tr>
			</table>
			<i class="li">To State:  </i><xsl:value-of select="/PXXML/PX_WORKFLOW/STATES/STATE[ID=/PXXML/PX_WORKFLOW/TRANSITION_TO_STATES/TRANSITION_TO_STATE[TRANSITION_ID=current()/ID]/STATE_ID]/NAME"/><br />
			<br />	
		</div>
	</div>
</xsl:template>


<xsl:template match="/PXXML/PX_WORKFLOW/TASKS/TASK">
	<div style="margin-left:0.25in" class="ph2" onclick="blocking(this)">
		<xsl:value-of select="NAME"/>
		<div style="display:none" class="gt">
			<i class="li">ID:  </i><xsl:value-of select="ID"/><br />
			<i class="li">Description:  </i><xsl:value-of select="DESCRIPTION"/><br />
			<i class="li">Node Type:  </i><xsl:value-of select="/PXXML/PX_WORKFLOW/NODE_TYPES/NODE_TYPE[ID=current()/NODE_TYPE_ID]/NAME"/><br />
			<i class="li">Transition:  </i><xsl:value-of select="/PXXML/PX_WORKFLOW/TRANSITIONS/TRANSITION[ID=current()/TRANSITION_ID]/DISPLAY_NAME"/><br />
			<i class="li">Display Order:  </i><xsl:value-of select="DISPLAY_ORDER"/><br />
			<xsl:choose>
				<xsl:when test="VISIBLE='0'">
					<i class="li">Visible:  </i>No<br />				
				</xsl:when>
				<xsl:otherwise>
					<i class="li">Visible:  </i>Yes<br />
				</xsl:otherwise>
			</xsl:choose>
			<table style="text-align:left" >
				<tr>
					<td valign="top"><i class="li">Task Role(s):  </i></td>
					<td>
						<table style="text-align:left">
					      	<xsl:for-each select="/PXXML/PX_WORKFLOW/TASK_ROLES/TASK_ROLE[TASK_ID=current()/ID]">
								<tr>
					  				<td><xsl:value-of select="/PXXML/PX_WORKFLOW/ROLES/ROLE[ID=current()/ROLE_ID]/DISPLAY_NAME"/>&#160;</td>
								</tr>
							</xsl:for-each>
					      </table>
					 </td>
				</tr>
			</table>
			<table style="text-align:left" >
				<tr>
					<td valign="top"><i class="li">Subsask(s):  </i></td>
					<td>
						<table style="text-align:left">
					      	<xsl:for-each select="/PXXML/PX_WORKFLOW/SUBTASKS/SUBTASK[TASK_ID=current()/ID]">
					      		<xsl:sort data-type="number" select="SUBTASK_ORDER"/>
								<tr>
					  				<td>
					  					<xsl:value-of select="SUBTASK_OBJECT"/>
					  					<xsl:if test="SUBTASK_DESCRIPTION!=''">
											-  <xsl:value-of select="SUBTASK_DESCRIPTION"/>
										</xsl:if>
					  					&#160;
					  				</td>
								</tr>
							</xsl:for-each>
					      </table>
					 </td>
				</tr>
			</table>
			<br />	
		</div>
	</div>
</xsl:template>


<xsl:template match="/PXXML/PX_WORKFLOW/FILTERS/FILTER">
	<div style="margin-left:0.25in" class="ph2" onclick="blocking(this)">
		<xsl:value-of select="PROGID"/>
		<div style="display:none" class="gt">
			<i class="li">ID:  </i><xsl:value-of select="ID"/><br />
			<i class="li">Description:  </i><xsl:value-of select="DESCRIPTION"/><br />
			<i class="li">State:  </i><xsl:value-of select="/PXXML/PX_WORKFLOW/STATES/STATE[ID=current()/STATE_ID]/NAME"/><br />
			<table style="text-align:left" >
				<tr>
					<td valign="top"><i class="li">Filter Role(s):  </i></td>
					<td>
						<table style="text-align:left">
					      	<xsl:for-each select="/PXXML/PX_WORKFLOW/FILTER_ROLES/FILTER_ROLE[FILTER_ID=current()/ID]">
								<tr>
					  				<td valign="top"><xsl:value-of select="/PXXML/PX_WORKFLOW/ROLES/ROLE[ID=current()/ROLE_ID]/DISPLAY_NAME"/>&#160;</td>
								</tr>
							</xsl:for-each>
					      </table>
					 </td>
				</tr>
			</table>
			<br />	
		</div>
	</div>
</xsl:template>


</xsl:stylesheet>