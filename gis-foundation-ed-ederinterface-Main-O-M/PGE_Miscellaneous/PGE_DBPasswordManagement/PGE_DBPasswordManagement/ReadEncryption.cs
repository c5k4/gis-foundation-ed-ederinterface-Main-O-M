// ========================================================================
// Copyright © 2021 PGE.
// <history>
// PGE DB Password Management (Common functions to implement in Batch and Uc4 Jobs)
// TCS V3SF (EDGISREARC-638) 06/16/2021                             Created
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGE_DBPasswordManagement
{
    /// <summary>
    /// Class Library Class to Access Password Management Functions
    /// </summary>
    public static class ReadEncryption
    {
        /// <summary>
        /// Create Oracle Connection String
        /// </summary>
        /// <param name="Data_Source"></param>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        private static string CreateOracleConnString(string Data_Source, string user, string pass)
        {
            string connection = string.Empty;
            try
            {
                connection = String.Format("Data Source={0}; User Id={1}; Password={2};", Data_Source, user, pass);
            }
            catch (Exception exp)
            {
                throw new Exception("Error in creating oracle conn string : " + exp.Message);
            }
            return connection;
        }

        /// <summary>
        /// Get Connection Details and create connection string
        /// </summary>
        /// <param name="username_Instance">Provide USER@INSTANCE in Upper Case like EDER@EDGEM1D</param>
        /// <returns>Returns Oracle Connection String for given User and Instance</returns>
        public static string GetConnectionStr(string username_Instance)
        {
            string retStr = default;
            string Instance = default;
            string user = default;
            string pass = default;
            try
            {
                pass = GetPassword(username_Instance);
                if (!string.IsNullOrWhiteSpace(pass))
                {
                    user = username_Instance.Split('@')[0].ToUpper();
                    Instance = username_Instance.Split('@')[1].ToUpper();
                    retStr = CreateOracleConnString(Instance, user, pass);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error :: " + ex.Message + " " + ex.StackTrace);
            }

            return retStr;
        }

        /// <summary>
        /// Get Password from PGE_DBPasswordManagement Project
        /// </summary>
        /// <param name="username_Instance">Provide USER@INSTANCE in Upper Case like EDER@EDGEM1D</param>
        /// <returns>Returns Password for given User and Instance</returns>
        public static string GetPassword(string username_Instance)
        {
            string retStr = default;
            try
            {
                Program program = new Program();
                retStr = program.RetGETPASSWORD(username_Instance);
            }
            catch (Exception ex)
            {
                throw new Exception("Error :: " + ex.Message + " " + ex.StackTrace);
            }

            return retStr;
        }

        /// <summary>
        /// Get Sde Path for given username@Instance
        /// </summary>
        /// <param name="username_Instance">Provide USER@INSTANCE in Upper Case like EDER@EDGEM1D</param>
        /// <returns>Returns SDE Path for given User and Instance</returns>
        public static string GetSDEPath(string username_Instance)
        {
            string retStr = default;
            try
            {
                Program program = new Program();
                retStr = program.RetGETSDEFILE(username_Instance);
            }
            catch (Exception ex)
            {
                throw new Exception("Error :: " + ex.Message + " " + ex.StackTrace);
            }

            return retStr;
        }

        /// <summary>
        /// Generates Password 
        /// </summary>
        /// <param name="username_Instance">Provide USER@INSTANCE in Upper Case like EDER@EDGEM1D</param>
        /// <returns>Returns Generated Password for given User and Instance</returns>
        public static string GenPassword(string username_Instance)
        {
            string retStr = default;
            try
            {
                Program program = new Program();
                retStr = program.RetGENPASSWORD(username_Instance);
            }
            catch (Exception ex)
            {
                throw new Exception("Error :: " + ex.Message + " " + ex.StackTrace);
            }

            return retStr;
        }
    }

}
