<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<!-- QAQCReport_8_1_2.xsl :  This style sheet creates QAQC reports for ArcFM 8.1.2-->
	<xsl:output encoding="UTF-8"/>
	<xsl:template match="QAQCTOPLEVEL">
		<html>
		<head>
			<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
			<style>
				<!-- Primary Headings -->
      				.ph1 {font-weight:bold; color:#0000FF; cursor:hand}
      				.ph2 {font-weight:bold; color:#2E8B57}
				<!-- General text and indentation-->
				.gtb {font-weight:bold; font-style="normal"; color:#000000}
				.gt {font-weight:normal; font-style="normal"; color:#000000}
				.indent {margin-left:0.3in}
			</style>

       		<script type="text/javascript" language="JavaScript"><xsl:comment>
					   <![CDATA[

			       
				   
				   function HideByImage(id)
				   {
					//alert(id);
					var elem = document.getElementById(id);
					var imgElem = document.getElementById(id + '_img');
					if(elem != null && imgElem != null)
					{
						if(elem.style.display == 'block')
						{
							elem.style.display = 'none';
							imgElem.src = 'PlusSign.bmp';
						}
						else
						{
							elem.style.display = 'block';
							imgElem.src = 'MinusSign.bmp';
						}
					}
				   }
						
				function Highlight()
				{
				  var elem=window.event.srcElement;
				  if (elem.className == "ph1") {
          			  color=elem.style.color;
				  elem.style.color="#FF0000";
				
					window.event.cancelBubble = true;
				  }
				  
				 }
				
				function UnHighlight()
				{
				  var elem=window.event.srcElement;
				  if (elem.className == "ph1") {
          			  elem.style.color=color;
				
				 	window.event.cancelBubble = true;			  
				 }
				 
			       }

			]]></xsl:comment></script>	
		</head>
		<body>			
			<h2 align="center">ArcFM QA/QC Report</h2>
			<h5 align="center"> <xsl:value-of select="DATE"/></h5>
			
			<xsl:if test="LAYER">
				<div class="ph1" onmouseover="Highlight()" onmouseout="UnHighlight()">
					<xsl:variable name="currentFeaturesId">
						<xsl:value-of select="generate-id(.)"/>
					</xsl:variable>
					<img id="{$currentFeaturesId}_img" src="MinusSign.bmp" onclick="HideByImage('{$currentFeaturesId}')" /> Features
					<div id="{$currentFeaturesId}" style="display:block">
						<br/>
						<xsl:apply-templates select="LAYER">
							<xsl:sort select="ALIAS"/>
						</xsl:apply-templates>
						<xsl:apply-templates select="FEATURE">
							<xsl:sort select="ALIASTABLE"/>
						</xsl:apply-templates>
					</div>
				</div>
			</xsl:if>
						
									
			<xsl:if test="D8TABLE">
				<div class="ph1" onmouseover="Highlight()" onmouseout="UnHighlight()">
					<xsl:variable name="currentTablesId">
						<xsl:value-of select="generate-id(.)"/>
					</xsl:variable>
					<img id="{$currentTablesId}_img" src="MinusSign.bmp" onclick="HideByImage('{$currentTablesId}')" /> Tables
					<div id="{$currentTablesId}" style="display:block">
						<br/>
						<xsl:apply-templates select="D8TABLE"/>
					</div>
				</div>
			</xsl:if>
			
			
			<xsl:if test="WORKREQUEST">
				<div class="ph1" onmouseover="Highlight()" onmouseout="UnHighlight()">
					<xsl:variable name="currentWRsId">
						<xsl:value-of select="generate-id(.)"/>
					</xsl:variable>
					<img id="{$currentWRsId}_img" src="MinusSign.bmp" onclick="HideByImage('{$currentWRsId}')" /> Work Requests
					<div id="{$currentWRsId}" style="display:block">
						<br/>
						<xsl:apply-templates select="WORKREQUEST">
							<xsl:sort select="ID"/>
						</xsl:apply-templates>
					</div>
				</div>
			</xsl:if>

			
			<xsl:if test="DESIGN">
				<div class="ph1" onmouseover="Highlight()" onmouseout="UnHighlight()">
					<xsl:variable name="currentDesignsId">
						<xsl:value-of select="generate-id(.)"/>
					</xsl:variable>
					<img id="{$currentDesignsId}_img" src="MinusSign.bmp" onclick="HideByImage('{$currentDesignsId}')" /> Designs
					<div id="{$currentDesignsId}" style="display:block">
						<br/>
						<xsl:apply-templates select="DESIGN">
							<xsl:with-param name="ClassString" select="'ph2'"/>
							<xsl:sort select="ID"/>
						</xsl:apply-templates>
					</div>
				</div>
			</xsl:if>


			<xsl:if test="ERROR">
				<div class="ph1" onmouseover="Highlight()" onmouseout="UnHighlight()">
					<xsl:variable name="currentErrorsId">
						<xsl:value-of select="generate-id(.)"/>
					</xsl:variable>
					<img id="{$currentErrorsId}_img" src="MinusSign.bmp" onclick="HideByImage('{$currentErrorsId}')" /> Errors
					<div id="{$currentErrorsId}" style="display:block">
						<br/>
						<div class="indent">	
							<span class="gt"><xsl:apply-templates select="ERROR"/></span>
						</div>
					</div>
				</div>
			</xsl:if>
			
					
			<xsl:if test="VERSIONDIFF">
				<div class="ph1" onmouseover="Highlight()" onmouseout="UnHighlight()">
					<xsl:variable name="currentVerDiffsId">
						<xsl:value-of select="generate-id(.)"/>
					</xsl:variable>
					<img id="{$currentVerDiffsId}_img" src="MinusSign.bmp" onclick="HideByImage('{$currentVerDiffsId}')" /> Version Difference
					<div id="{$currentVerDiffsId}" style="display:block">
						<br/>
						<xsl:apply-templates select="VERSIONDIFF"/>
					</div>
				</div>
			</xsl:if>

			<br/><br/>			
			<small>Generated by:</small>
			<br/>
			<b>ArcFM</b>
			<br/>
			<a href="https://myarcfm.schneider-electric.com/myarcfm/s/">
				<span style="font-family:Frutiger-Roman,Arial,Helvetica,Sans-Serif;color:009B3E"><b>Schneider Electric</b></span>
			</a>
			<br/>
		</body>
		</html>
	</xsl:template>	
	
	
	<xsl:template match="LAYER">
		<span class="ph2">		
			<div class="indent">
				<xsl:variable name="currentLayerId">
					<xsl:value-of select="generate-id(.)"/>
				</xsl:variable>
				<img id="{$currentLayerId}_img" src="MinusSign.bmp" onclick="HideByImage('{$currentLayerId}')" /> &#32; <xsl:value-of select="ALIAS"/>
				<div id="{$currentLayerId}" style="display:block">
					<xsl:apply-templates select="DATABASE"/>
					<xsl:apply-templates select="FEATURE"/>
					<xsl:apply-templates select="LAYER"/>
					<hr/>
				</div>
			</div>
		</span>
		<br/>
	</xsl:template>
	
	
	<xsl:template match="D8TABLE">
		<span class="ph2">		
			<div class="indent" >
				<xsl:variable name="currentTableId">
					<xsl:value-of select="generate-id(.)"/>
				</xsl:variable>
				<img id="{$currentTableId}_img" src="MinusSign.bmp" onclick="HideByImage('{$currentTableId}')" /> &#32; <xsl:value-of select="TABLENAME"/>
				<div id="{$currentTableId}" style="display:block">
					<xsl:if test="COMPARINGVERSION">
						<b>Comparing Version:  </b><xsl:value-of select="COMPARINGVERSION"/><br/>
					</xsl:if>
					<xsl:apply-templates select="DATABASE"/>		
					<xsl:apply-templates select="FEATURE"/>
					<hr/>
				</div>
			</div>
		</span>
		<br/>
	</xsl:template>
	
		
	<xsl:template match="FEATURE">
		<div class="indent">
			<xsl:choose>
				<xsl:when test="count(./ancestor::node())=3">
					<span class="gt"><b>(<xsl:value-of select="OID"/>):  </b><xsl:value-of select="PRIMARYDISPLAYFIELD"/>  =  <xsl:value-of select="PRIMARYDISPLAYFIELDVALUE"/></span>
				</xsl:when>
				<xsl:otherwise>
					<span class="gt"><b><xsl:value-of select="ALIASTABLE"/> (<xsl:value-of select="OID"/>):  </b><xsl:value-of select="PRIMARYDISPLAYFIELD"/>  =  <xsl:value-of select="PRIMARYDISPLAYFIELDVALUE"/></span>
				</xsl:otherwise>
			</xsl:choose>	
			<xsl:if test="ERROR or ERRORMSG or RELATIONSHIP/*">
        <div class="indent">
          <div class="gt">	
            <table cellpadding="1" style="text-align:left">
              <xsl:if test="ERROR or ERRORMSG">

                <tr>
                  <th valign="top">Errors:</th>
                  <td><xsl:value-of select="ERRORMSG"/></td>
                </tr>
                <tr>	
                  <td></td>
                  <td><ul><xsl:apply-templates select="ERROR"/></ul></td>
                </tr>
              </xsl:if>
              <xsl:if test="RELATIONSHIP/*">
                <tr>
                  <th valign="top">Relationships:</th>
                  <td><xsl:apply-templates select="RELATIONSHIP"/></td>
                </tr>
              </xsl:if>
            </table>
          </div>
        </div>
			</xsl:if>
			<xsl:if test=" (count(./ancestor::node())=3) and (position()!=last())"><hr/></xsl:if>
		</div>
		<br/>
	</xsl:template>
	
	
	<xsl:template match="ERROR">
		<li><xsl:value-of select="MESSAGE"/></li>
	</xsl:template>
	
	<xsl:template match="RELATIONSHIP">
    <xsl:if test="normalize-space(.)">
      <xsl:apply-templates select="FEATURE"/>
    </xsl:if>
	</xsl:template>
	
	
	<xsl:template match="WORKREQUEST">
		<span class="ph2">		
			<div class="indent">
				<xsl:variable name="currentWRId">
					<xsl:value-of select="generate-id(.)"/>
				</xsl:variable>
				<img id="{$currentWRId}_img" src="MinusSign.bmp" onclick="HideByImage('{$currentWRId}')" /> Work Request ID <xsl:value-of select="ID"/>:  <xsl:value-of select="DESCRIPTION"/>
				<div id="{$currentWRId}" style="display:block">
					<div class="indent">
						<table cellpadding="1" style="text-align:left">				
							<xsl:if test="ERROR">
								<tr>
									<th valign="top">Errors:</th>
							 	 	<td><ul><xsl:apply-templates select="ERROR"/></ul></td>
								</tr>
							</xsl:if>
						</table>
					</div>
					<xsl:apply-templates select="DESIGN">
						<xsl:with-param name="ClassString" select="'gtb'"/>
						<xsl:with-param name="onClickString"/>			
					</xsl:apply-templates>
					<xsl:if test="(count(./ancestor::node())=2)"><hr/></xsl:if>
				</div>
			</div>
		</span>
		<br/>
	</xsl:template>
	
	
	<xsl:template match="DESIGN">
		<xsl:param name="ClassString"/>
		<span class="{$ClassString}">		
			<div class="indent">
				<xsl:variable name="currentDesignId">
					<xsl:value-of select="generate-id(.)"/>
				</xsl:variable>
				<img id="{$currentDesignId}_img" src="MinusSign.bmp" onclick="HideByImage('{$currentDesignId}')" /> Design ID <xsl:value-of select="ID"/>:  <xsl:value-of select="DESCRIPTION"/>
				<div id="{$currentDesignId}" style="display:block">
					<xsl:apply-templates select="DATABASE"/>			
					<xsl:apply-templates select="WORKLOCATION"/>
					<xsl:apply-templates select="GISUNIT"/>
					<div class="indent">
						<table cellpadding="1" style="text-align:left">
							<xsl:if test="ERROR">
								<tr>
									<th valign="top">Errors:</th>
							 	 	<td><ul><xsl:apply-templates select="ERROR"/></ul></td>
								</tr>
							</xsl:if>
							<xsl:if test="CU">
								<tr>
									<th valign="top">Compatible Units:</th>
								</tr>
								<tr>
									<th/>
									<td><xsl:apply-templates select="CU"/></td>							
								</tr>
							</xsl:if>
						</table>
					</div>
					<xsl:if test="(count(./ancestor::node())=2)"><hr/></xsl:if>
				</div>
			</div>
		</span>
		<br/>
	</xsl:template>

	
	
	<xsl:template match="WORKLOCATION">
		<div class="indent">
			<span class="gt"><b>Work Location ID:  </b><xsl:value-of select="ID"/></span>
			<br/>
			<div class="indent">
				<table cellpadding="1" style="text-align:left">
					<xsl:if test="ERROR">
						<tr>
							<th valign="top">Errors:</th>
					 	 	<td><ul><xsl:apply-templates select="ERROR"/></ul></td>
						</tr>
					</xsl:if>
					<xsl:if test="CU">
						<tr>
							<th valign="top">Compatible Units:</th>
						</tr>
						<tr>
							<th/>
							<td><xsl:apply-templates select="CU"/></td>							
						</tr>
					</xsl:if>
				</table>
			</div>
			<xsl:apply-templates select="GISUNIT"/>
			<!--xsl:if test="(count(./ancestor::node())=2)"><hr/></xsl:if-->
		</div>
		<br/>
	</xsl:template>
	
	
	<xsl:template match="GISUNIT">
		<div class="indent">
			<span class="gt"><b>GIS Unit:  </b><xsl:value-of select="DESCRIPTION"/></span>			
			<br/>
			<div class="indent">
				<div class="gt">	
					<table cellpadding="1" style="text-align:left">
						<tr>
							<th valign="top">Table:</th>
							<td><xsl:value-of select="TABLENAME"/></td>
						</tr>
						<xsl:if test="ERROR">
							<tr>
								<th valign="top">Errors:</th>
				 	 			<td><ul><xsl:apply-templates select="ERROR"/></ul></td>
							</tr>
						</xsl:if>
						<xsl:if test="CU">
							<tr>
								<th valign="top">Compatible Units:</th>
							</tr>
							<tr>
								<th/>
								<td><xsl:apply-templates select="CU"/></td>							
							</tr>
						</xsl:if>
					</table>
					<xsl:apply-templates select="GISUNIT"/>
				</div>
			</div>
			<!--xsl:if test="(count(./ancestor::node())=2)"><hr/></xsl:if-->
		</div>
		<br/>
	</xsl:template>
	
	
	<xsl:template match="CU">
		<div class="gt">	
			<table cellpadding="1" style="text-align:left">
				<tr>
					<th valign="top">CU Description:</th>
					<td><xsl:value-of select="DESCRIPTION"/></td>
				</tr>
				<tr>
					<td/>
					<td>							
						<table cellpadding="1" style="text-align:left">
							<tr>
								<th valign="top">Table:</th>
								<td><xsl:value-of select="TABLENAME"/></td>
							</tr>
							<tr>
								<th valign="top">WMS Code:</th>
								<td><xsl:value-of select="WMS_CODE"/></td>
							</tr>
							<tr>
								<th valign="top">Work Function:</th>
								<td><xsl:value-of select="WORK_FUNCTION"/></td>
							</tr>
							<tr>
								<th valign="top">Work Function Status:</th>
								<td><xsl:value-of select="WF_STATUS"/></td>
							</tr>
							<xsl:if test="ERROR">
								<tr>
									<th valign="top">Errors:</th>
				 	 				<td><ul><xsl:apply-templates select="ERROR"/></ul></td>
								</tr>						
							</xsl:if>
						</table>
					</td>
				</tr>
			</table>
		</div>
	</xsl:template>
	
	
	<xsl:template match="VERSIONDIFF">
		<div class="indent">
			<span class="ph2"><b>Description:  </b><xsl:value-of select="DESCRIPTION"/></span><br/>			
			<xsl:apply-templates select="D8TABLE"/>
			<xsl:if test="(count(./ancestor::node())=2) and (position()!=last())"><hr/></xsl:if>
		</div>
		<br/>
	</xsl:template>
	
	
	<xsl:template match="DATABASE">
			<span class="gt">
				<xsl:choose>
					<xsl:when test="@LOCATION='Local'">
						<b>Database Name:  </b><xsl:value-of select="DBNAME"/><br/>
					</xsl:when>
					<xsl:when test="@LOCATION='Remote'">
						<b>Server:  </b><xsl:value-of select="SERVER"/><br/>
						<b>Version:  </b><xsl:value-of select="DBVERSION"/><br/>
						<b>User:  </b><xsl:value-of select="USER"/><br/>
					</xsl:when>
				</xsl:choose>
			</span>			
		<br/>
	</xsl:template>

</xsl:stylesheet>
