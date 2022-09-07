namespace ROBC_Testing
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.cmdECustCount = new System.Windows.Forms.Button();
            this.bPCPGetCount = new System.Windows.Forms.Button();
            this.bParentChildIDs = new System.Windows.Forms.Button();
            this.bHasEssentialCustomerPartialCurtailmentPoint = new System.Windows.Forms.Button();
            this.bCheckCircuitForEssentialCustomer = new System.Windows.Forms.Button();
            this.bCustomerCountforPCP = new System.Windows.Forms.Button();
            this.bCustomerCountforCircuit = new System.Windows.Forms.Button();
            this.bInvalidPCPs = new System.Windows.Forms.Button();
            this.cmdSCADA = new System.Windows.Forms.Button();
            this.cmdLoadInfo = new System.Windows.Forms.Button();
            this.tbCircuitIDToTest = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbDeviceGuid = new System.Windows.Forms.TextBox();
            this.tbOPNumber = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.bFindDevice = new System.Windows.Forms.Button();
            this.cmdCreatePCP = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(117, 240);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Get Session";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cmdECustCount
            // 
            this.cmdECustCount.Location = new System.Drawing.Point(443, 226);
            this.cmdECustCount.Name = "cmdECustCount";
            this.cmdECustCount.Size = new System.Drawing.Size(115, 50);
            this.cmdECustCount.TabIndex = 1;
            this.cmdECustCount.Text = "Get unassigned CircuitSource without Loading";
            this.cmdECustCount.UseVisualStyleBackColor = true;
            this.cmdECustCount.Click += new System.EventHandler(this.cmdECustCount_Click);
            // 
            // bPCPGetCount
            // 
            this.bPCPGetCount.Location = new System.Drawing.Point(12, 82);
            this.bPCPGetCount.Name = "bPCPGetCount";
            this.bPCPGetCount.Size = new System.Drawing.Size(115, 52);
            this.bPCPGetCount.TabIndex = 2;
            this.bPCPGetCount.Text = "UnusedButton";
            this.bPCPGetCount.UseVisualStyleBackColor = true;
            this.bPCPGetCount.Click += new System.EventHandler(this.bPCPGetCount_Click);
            // 
            // bParentChildIDs
            // 
            this.bParentChildIDs.Location = new System.Drawing.Point(12, 14);
            this.bParentChildIDs.Name = "bParentChildIDs";
            this.bParentChildIDs.Size = new System.Drawing.Size(115, 52);
            this.bParentChildIDs.TabIndex = 3;
            this.bParentChildIDs.Text = "Get Parent/Child CircuitIDs for circuit";
            this.bParentChildIDs.UseVisualStyleBackColor = true;
            this.bParentChildIDs.Click += new System.EventHandler(this.bParentChildIDs_Click);
            // 
            // bHasEssentialCustomerPartialCurtailmentPoint
            // 
            this.bHasEssentialCustomerPartialCurtailmentPoint.Location = new System.Drawing.Point(274, 82);
            this.bHasEssentialCustomerPartialCurtailmentPoint.Name = "bHasEssentialCustomerPartialCurtailmentPoint";
            this.bHasEssentialCustomerPartialCurtailmentPoint.Size = new System.Drawing.Size(115, 52);
            this.bHasEssentialCustomerPartialCurtailmentPoint.TabIndex = 4;
            this.bHasEssentialCustomerPartialCurtailmentPoint.Text = "Check Partial Curtailment Point  for Essential Customer";
            this.bHasEssentialCustomerPartialCurtailmentPoint.UseVisualStyleBackColor = true;
            this.bHasEssentialCustomerPartialCurtailmentPoint.Click += new System.EventHandler(this.bHasEssentialCustomerPartialCurtailmentPoint_Click);
            // 
            // bCheckCircuitForEssentialCustomer
            // 
            this.bCheckCircuitForEssentialCustomer.Location = new System.Drawing.Point(276, 12);
            this.bCheckCircuitForEssentialCustomer.Name = "bCheckCircuitForEssentialCustomer";
            this.bCheckCircuitForEssentialCustomer.Size = new System.Drawing.Size(115, 52);
            this.bCheckCircuitForEssentialCustomer.TabIndex = 5;
            this.bCheckCircuitForEssentialCustomer.Text = "Check CircuitID  for Essential Customer";
            this.bCheckCircuitForEssentialCustomer.UseVisualStyleBackColor = true;
            this.bCheckCircuitForEssentialCustomer.Click += new System.EventHandler(this.bCheckCircuitForEssentialCustomer_Click);
            // 
            // bCustomerCountforPCP
            // 
            this.bCustomerCountforPCP.Location = new System.Drawing.Point(144, 82);
            this.bCustomerCountforPCP.Name = "bCustomerCountforPCP";
            this.bCustomerCountforPCP.Size = new System.Drawing.Size(115, 52);
            this.bCustomerCountforPCP.TabIndex = 6;
            this.bCustomerCountforPCP.Text = "Get Partial Curtailment Point ALL Customer Count";
            this.bCustomerCountforPCP.UseVisualStyleBackColor = true;
            this.bCustomerCountforPCP.Click += new System.EventHandler(this.bCustomerCountforPCP_Click);
            // 
            // bCustomerCountforCircuit
            // 
            this.bCustomerCountforCircuit.Location = new System.Drawing.Point(144, 14);
            this.bCustomerCountforCircuit.Name = "bCustomerCountforCircuit";
            this.bCustomerCountforCircuit.Size = new System.Drawing.Size(115, 50);
            this.bCustomerCountforCircuit.TabIndex = 7;
            this.bCustomerCountforCircuit.Text = "Get CircuitID ALL Customer Count";
            this.bCustomerCountforCircuit.UseVisualStyleBackColor = true;
            this.bCustomerCountforCircuit.Click += new System.EventHandler(this.bCustomerCountforCircuit_Click);
            // 
            // bInvalidPCPs
            // 
            this.bInvalidPCPs.Location = new System.Drawing.Point(298, 224);
            this.bInvalidPCPs.Name = "bInvalidPCPs";
            this.bInvalidPCPs.Size = new System.Drawing.Size(115, 52);
            this.bInvalidPCPs.TabIndex = 8;
            this.bInvalidPCPs.Text = "Get Invalid PCPs";
            this.bInvalidPCPs.UseVisualStyleBackColor = true;
            this.bInvalidPCPs.Click += new System.EventHandler(this.bInvalidPCPs_Click);
            // 
            // cmdSCADA
            // 
            this.cmdSCADA.Location = new System.Drawing.Point(12, 235);
            this.cmdSCADA.Name = "cmdSCADA";
            this.cmdSCADA.Size = new System.Drawing.Size(72, 36);
            this.cmdSCADA.TabIndex = 9;
            this.cmdSCADA.Text = "Is SCADA";
            this.cmdSCADA.UseVisualStyleBackColor = true;
            this.cmdSCADA.Click += new System.EventHandler(this.cmdSCADA_Click);
            // 
            // cmdLoadInfo
            // 
            this.cmdLoadInfo.Location = new System.Drawing.Point(12, 182);
            this.cmdLoadInfo.Name = "cmdLoadInfo";
            this.cmdLoadInfo.Size = new System.Drawing.Size(86, 47);
            this.cmdLoadInfo.TabIndex = 10;
            this.cmdLoadInfo.Text = "Load Info";
            this.cmdLoadInfo.UseVisualStyleBackColor = true;
            this.cmdLoadInfo.Click += new System.EventHandler(this.cmdLoadInfo_Click);
            // 
            // tbCircuitIDToTest
            // 
            this.tbCircuitIDToTest.Location = new System.Drawing.Point(457, 29);
            this.tbCircuitIDToTest.Name = "tbCircuitIDToTest";
            this.tbCircuitIDToTest.Size = new System.Drawing.Size(113, 20);
            this.tbCircuitIDToTest.TabIndex = 11;
            this.tbCircuitIDToTest.Text = "182742102";
            this.tbCircuitIDToTest.TextChanged += new System.EventHandler(this.tbCircuitIDToTest_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(417, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Circuit :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(417, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "DeviceGUID:";
            // 
            // tbDeviceGuid
            // 
            this.tbDeviceGuid.Location = new System.Drawing.Point(491, 61);
            this.tbDeviceGuid.Name = "tbDeviceGuid";
            this.tbDeviceGuid.Size = new System.Drawing.Size(232, 20);
            this.tbDeviceGuid.TabIndex = 13;
            this.tbDeviceGuid.Text = "{043F4855-19E7-40C7-B521-9D0CC3373315}";
            // 
            // tbOPNumber
            // 
            this.tbOPNumber.Location = new System.Drawing.Point(610, 31);
            this.tbOPNumber.Name = "tbOPNumber";
            this.tbOPNumber.Size = new System.Drawing.Size(113, 20);
            this.tbOPNumber.TabIndex = 15;
            this.tbOPNumber.Text = "6513";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(576, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "op#:";
            // 
            // bFindDevice
            // 
            this.bFindDevice.Location = new System.Drawing.Point(608, 224);
            this.bFindDevice.Name = "bFindDevice";
            this.bFindDevice.Size = new System.Drawing.Size(115, 52);
            this.bFindDevice.TabIndex = 17;
            this.bFindDevice.Text = "Find Device";
            this.bFindDevice.UseVisualStyleBackColor = true;
            this.bFindDevice.Click += new System.EventHandler(this.bFindDevice_Click);
            // 
            // cmdCreatePCP
            // 
            this.cmdCreatePCP.Location = new System.Drawing.Point(175, 168);
            this.cmdCreatePCP.Name = "cmdCreatePCP";
            this.cmdCreatePCP.Size = new System.Drawing.Size(83, 56);
            this.cmdCreatePCP.TabIndex = 18;
            this.cmdCreatePCP.Text = "Create PCP";
            this.cmdCreatePCP.UseVisualStyleBackColor = true;
            this.cmdCreatePCP.Click += new System.EventHandler(this.cmdCreatePCP_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(399, 97);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(89, 52);
            this.button2.TabIndex = 19;
            this.button2.Text = "Get Loading Information";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(495, 97);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(85, 52);
            this.button3.TabIndex = 20;
            this.button3.Text = "Get PCP Customer Count";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(735, 289);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.cmdCreatePCP);
            this.Controls.Add(this.bFindDevice);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbOPNumber);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbDeviceGuid);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbCircuitIDToTest);
            this.Controls.Add(this.cmdLoadInfo);
            this.Controls.Add(this.cmdSCADA);
            this.Controls.Add(this.bInvalidPCPs);
            this.Controls.Add(this.bCustomerCountforCircuit);
            this.Controls.Add(this.bCustomerCountforPCP);
            this.Controls.Add(this.bCheckCircuitForEssentialCustomer);
            this.Controls.Add(this.bHasEssentialCustomerPartialCurtailmentPoint);
            this.Controls.Add(this.bParentChildIDs);
            this.Controls.Add(this.bPCPGetCount);
            this.Controls.Add(this.cmdECustCount);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button cmdECustCount;
        private System.Windows.Forms.Button bPCPGetCount;
        private System.Windows.Forms.Button bParentChildIDs;
        private System.Windows.Forms.Button bHasEssentialCustomerPartialCurtailmentPoint;
        private System.Windows.Forms.Button bCheckCircuitForEssentialCustomer;
        private System.Windows.Forms.Button bCustomerCountforPCP;
        private System.Windows.Forms.Button bCustomerCountforCircuit;
        private System.Windows.Forms.Button bInvalidPCPs;
        private System.Windows.Forms.Button cmdSCADA;
        private System.Windows.Forms.Button cmdLoadInfo;
        private System.Windows.Forms.TextBox tbCircuitIDToTest;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbDeviceGuid;
        private System.Windows.Forms.TextBox tbOPNumber;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button bFindDevice;
        private System.Windows.Forms.Button cmdCreatePCP;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}

