//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ArcFMSilverlight
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices;
    using System.ServiceModel.DomainServices.Client;
    using System.ServiceModel.DomainServices.Client.ApplicationServices;
    using Miner.Server.Silverlight.Web;
    
    
    /// <summary>
    /// Context for the RIA application.
    /// </summary>
    /// <remarks>
    /// This context extends the base to make application services and types available
    /// for consumption from code and xaml.
    /// </remarks>
    public sealed partial class WebContext : WebContextBase
    {
        
        #region Extensibility Method Definitions

        /// <summary>
        /// This method is invoked from the constructor once initialization is complete and
        /// can be used for further object setup.
        /// </summary>
        partial void OnCreated();

        #endregion
        
        
        /// <summary>
        /// Initializes a new instance of the WebContext class.
        /// </summary>
        public WebContext()
        {
            this.OnCreated();
        }
        
        /// <summary>
        /// Gets the context that is registered as a lifetime object with the current application.
        /// </summary>
        /// <exception cref="InvalidOperationException"> is thrown if there is no current application,
        /// no contexts have been added, or more than one context has been added.
        /// </exception>
        /// <seealso cref="System.Windows.Application.ApplicationLifetimeObjects"/>
        public new static WebContext Current
        {
            get
            {
                return ((WebContext)(WebContextBase.Current));
            }
        }
        
        /// <summary>
        /// Gets a user representing the authenticated identity.
        /// </summary>
        public new User User
        {
            get
            {
                return ((User)(base.User));
            }
        }
    }
}
namespace Miner.Server.Silverlight.Web
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.DomainServices;
    using System.ServiceModel.DomainServices.Client;
    using System.ServiceModel.DomainServices.Client.ApplicationServices;
    using System.ServiceModel.Web;
    
    
    /// <summary>
    /// The DomainContext corresponding to the 'AuthenticationDomainService' DomainService.
    /// </summary>
    public sealed partial class AuthenticationDomainContext : global::System.ServiceModel.DomainServices.Client.ApplicationServices.AuthenticationDomainContextBase
    {
        
        #region Extensibility Method Definitions

        /// <summary>
        /// This method is invoked from the constructor once initialization is complete and
        /// can be used for further object setup.
        /// </summary>
        partial void OnCreated();

        #endregion
        
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationDomainContext"/> class.
        /// </summary>
        public AuthenticationDomainContext() : 
                this(new WebDomainClient<IAuthenticationDomainServiceContract>(new Uri("Miner-Server-Silverlight-Web-AuthenticationDomainService.svc", UriKind.Relative)))
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationDomainContext"/> class with the specified service URI.
        /// </summary>
        /// <param name="serviceUri">The AuthenticationDomainService service URI.</param>
        public AuthenticationDomainContext(Uri serviceUri) : 
                this(new WebDomainClient<IAuthenticationDomainServiceContract>(serviceUri))
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationDomainContext"/> class with the specified <paramref name="domainClient"/>.
        /// </summary>
        /// <param name="domainClient">The DomainClient instance to use for this DomainContext.</param>
        public AuthenticationDomainContext(DomainClient domainClient) : 
                base(domainClient)
        {
            this.OnCreated();
        }
        
        /// <summary>
        /// Gets the set of <see cref="User"/> entity instances that have been loaded into this <see cref="AuthenticationDomainContext"/> instance.
        /// </summary>
        public EntitySet<User> Users
        {
            get
            {
                return base.EntityContainer.GetEntitySet<User>();
            }
        }
        
        /// <summary>
        /// Gets an EntityQuery instance that can be used to load <see cref="User"/> entity instances using the 'GetUser' query.
        /// </summary>
        /// <returns>An EntityQuery that can be loaded to retrieve <see cref="User"/> entity instances.</returns>
        public EntityQuery<User> GetUserQuery()
        {
            this.ValidateMethod("GetUserQuery", null);
            return base.CreateQuery<User>("GetUser", null, false, false);
        }
        
        /// <summary>
        /// Gets an EntityQuery instance that can be used to load <see cref="User"/> entity instances using the 'Login' query.
        /// </summary>
        /// <param name="userName">The value for the 'userName' parameter of the query.</param>
        /// <param name="password">The value for the 'password' parameter of the query.</param>
        /// <param name="isPersistent">The value for the 'isPersistent' parameter of the query.</param>
        /// <param name="customData">The value for the 'customData' parameter of the query.</param>
        /// <returns>An EntityQuery that can be loaded to retrieve <see cref="User"/> entity instances.</returns>
        public EntityQuery<User> LoginQuery(string userName, string password, bool isPersistent, string customData)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userName", userName);
            parameters.Add("password", password);
            parameters.Add("isPersistent", isPersistent);
            parameters.Add("customData", customData);
            this.ValidateMethod("LoginQuery", parameters);
            return base.CreateQuery<User>("Login", parameters, true, false);
        }
        
        /// <summary>
        /// Gets an EntityQuery instance that can be used to load <see cref="User"/> entity instances using the 'Logout' query.
        /// </summary>
        /// <returns>An EntityQuery that can be loaded to retrieve <see cref="User"/> entity instances.</returns>
        public EntityQuery<User> LogoutQuery()
        {
            this.ValidateMethod("LogoutQuery", null);
            return base.CreateQuery<User>("Logout", null, true, false);
        }
        
        /// <summary>
        /// Creates a new EntityContainer for this DomainContext's EntitySets.
        /// </summary>
        /// <returns>A new container instance.</returns>
        protected override EntityContainer CreateEntityContainer()
        {
            return new AuthenticationDomainContextEntityContainer();
        }
        
        /// <summary>
        /// Service contract for the 'AuthenticationDomainService' DomainService.
        /// </summary>
        [ServiceContract()]
        public interface IAuthenticationDomainServiceContract
        {
            
            /// <summary>
            /// Asynchronously invokes the 'GetUser' operation.
            /// </summary>
            /// <param name="callback">Callback to invoke on completion.</param>
            /// <param name="asyncState">Optional state object.</param>
            /// <returns>An IAsyncResult that can be used to monitor the request.</returns>
            [FaultContract(typeof(DomainServiceFault), Action="http://tempuri.org/AuthenticationDomainService/GetUserDomainServiceFault", Name="DomainServiceFault", Namespace="DomainServices")]
            [OperationContract(AsyncPattern=true, Action="http://tempuri.org/AuthenticationDomainService/GetUser", ReplyAction="http://tempuri.org/AuthenticationDomainService/GetUserResponse")]
            [WebGet()]
            IAsyncResult BeginGetUser(AsyncCallback callback, object asyncState);
            
            /// <summary>
            /// Completes the asynchronous operation begun by 'BeginGetUser'.
            /// </summary>
            /// <param name="result">The IAsyncResult returned from 'BeginGetUser'.</param>
            /// <returns>The 'QueryResult' returned from the 'GetUser' operation.</returns>
            QueryResult<User> EndGetUser(IAsyncResult result);
            
            /// <summary>
            /// Asynchronously invokes the 'Login' operation.
            /// </summary>
            /// <param name="userName">The value for the 'userName' parameter of this action.</param>
            /// <param name="password">The value for the 'password' parameter of this action.</param>
            /// <param name="isPersistent">The value for the 'isPersistent' parameter of this action.</param>
            /// <param name="customData">The value for the 'customData' parameter of this action.</param>
            /// <param name="callback">Callback to invoke on completion.</param>
            /// <param name="asyncState">Optional state object.</param>
            /// <returns>An IAsyncResult that can be used to monitor the request.</returns>
            [FaultContract(typeof(DomainServiceFault), Action="http://tempuri.org/AuthenticationDomainService/LoginDomainServiceFault", Name="DomainServiceFault", Namespace="DomainServices")]
            [OperationContract(AsyncPattern=true, Action="http://tempuri.org/AuthenticationDomainService/Login", ReplyAction="http://tempuri.org/AuthenticationDomainService/LoginResponse")]
            IAsyncResult BeginLogin(string userName, string password, bool isPersistent, string customData, AsyncCallback callback, object asyncState);
            
            /// <summary>
            /// Completes the asynchronous operation begun by 'BeginLogin'.
            /// </summary>
            /// <param name="result">The IAsyncResult returned from 'BeginLogin'.</param>
            /// <returns>The 'QueryResult' returned from the 'Login' operation.</returns>
            QueryResult<User> EndLogin(IAsyncResult result);
            
            /// <summary>
            /// Asynchronously invokes the 'Logout' operation.
            /// </summary>
            /// <param name="callback">Callback to invoke on completion.</param>
            /// <param name="asyncState">Optional state object.</param>
            /// <returns>An IAsyncResult that can be used to monitor the request.</returns>
            [FaultContract(typeof(DomainServiceFault), Action="http://tempuri.org/AuthenticationDomainService/LogoutDomainServiceFault", Name="DomainServiceFault", Namespace="DomainServices")]
            [OperationContract(AsyncPattern=true, Action="http://tempuri.org/AuthenticationDomainService/Logout", ReplyAction="http://tempuri.org/AuthenticationDomainService/LogoutResponse")]
            IAsyncResult BeginLogout(AsyncCallback callback, object asyncState);
            
            /// <summary>
            /// Completes the asynchronous operation begun by 'BeginLogout'.
            /// </summary>
            /// <param name="result">The IAsyncResult returned from 'BeginLogout'.</param>
            /// <returns>The 'QueryResult' returned from the 'Logout' operation.</returns>
            QueryResult<User> EndLogout(IAsyncResult result);
            
            /// <summary>
            /// Asynchronously invokes the 'SubmitChanges' operation.
            /// </summary>
            /// <param name="changeSet">The change-set to submit.</param>
            /// <param name="callback">Callback to invoke on completion.</param>
            /// <param name="asyncState">Optional state object.</param>
            /// <returns>An IAsyncResult that can be used to monitor the request.</returns>
            [FaultContract(typeof(DomainServiceFault), Action="http://tempuri.org/AuthenticationDomainService/SubmitChangesDomainServiceFault", Name="DomainServiceFault", Namespace="DomainServices")]
            [OperationContract(AsyncPattern=true, Action="http://tempuri.org/AuthenticationDomainService/SubmitChanges", ReplyAction="http://tempuri.org/AuthenticationDomainService/SubmitChangesResponse")]
            IAsyncResult BeginSubmitChanges(IEnumerable<ChangeSetEntry> changeSet, AsyncCallback callback, object asyncState);
            
            /// <summary>
            /// Completes the asynchronous operation begun by 'BeginSubmitChanges'.
            /// </summary>
            /// <param name="result">The IAsyncResult returned from 'BeginSubmitChanges'.</param>
            /// <returns>The collection of change-set entry elements returned from 'SubmitChanges'.</returns>
            IEnumerable<ChangeSetEntry> EndSubmitChanges(IAsyncResult result);
        }
        
        internal sealed class AuthenticationDomainContextEntityContainer : EntityContainer
        {
            
            public AuthenticationDomainContextEntityContainer()
            {
                this.CreateEntitySet<User>(EntitySetOperations.Edit);
            }
        }
    }
    
    /// <summary>
    /// The 'User' entity class.
    /// </summary>
    [DataContract(Namespace="http://schemas.datacontract.org/2004/07/Miner.Server.Silverlight.Web")]
    public sealed partial class User : Entity, global::System.Security.Principal.IIdentity, global::System.Security.Principal.IPrincipal
    {
        
        private string _name = string.Empty;
        
        private IEnumerable<string> _roles;
        
        #region Extensibility Method Definitions

        /// <summary>
        /// This method is invoked from the constructor once initialization is complete and
        /// can be used for further object setup.
        /// </summary>
        partial void OnCreated();
        partial void OnNameChanging(string value);
        partial void OnNameChanged();
        partial void OnRolesChanging(IEnumerable<string> value);
        partial void OnRolesChanged();

        #endregion
        
        
        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        public User()
        {
            this.OnCreated();
        }
        
        /// <summary>
        /// Gets or sets the 'Name' value.
        /// </summary>
        [DataMember()]
        [Editable(false, AllowInitialValue=true)]
        [Key()]
        [RoundtripOriginal()]
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                if ((this._name != value))
                {
                    this.OnNameChanging(value);
                    this.ValidateProperty("Name", value);
                    this._name = value;
                    this.RaisePropertyChanged("Name");
                    this.OnNameChanged();
                    this.RaisePropertyChanged("IsAuthenticated");
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the 'Roles' value.
        /// </summary>
        [DataMember()]
        [Editable(false)]
        public IEnumerable<string> Roles
        {
            get
            {
                return this._roles;
            }
            set
            {
                if ((this._roles != value))
                {
                    this.OnRolesChanging(value);
                    this.ValidateProperty("Roles", value);
                    this._roles = value;
                    this.RaisePropertyChanged("Roles");
                    this.OnRolesChanged();
                }
            }
        }
        
        string global::System.Security.Principal.IIdentity.AuthenticationType
        {
            get
            {
                return string.Empty;
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether the identity is authenticated.
        /// </summary>
        /// <remarks>
        /// This value is <c>true</c> if <see cref="Name"/> is not <c>null</c> or empty.
        /// </remarks>
        public bool IsAuthenticated
        {
            get
            {
                return (true != string.IsNullOrEmpty(this.Name));
            }
        }
        
        string global::System.Security.Principal.IIdentity.Name
        {
            get
            {
                return this.Name;
            }
        }
        
        global::System.Security.Principal.IIdentity global::System.Security.Principal.IPrincipal.Identity
        {
            get
            {
                return this;
            }
        }
        
        /// <summary>
        /// Computes a value from the key fields that uniquely identifies this entity instance.
        /// </summary>
        /// <returns>An object instance that uniquely identifies this entity instance.</returns>
        public override object GetIdentity()
        {
            return this._name;
        }
        
        /// <summary>
        /// Return whether the principal is in the role.
        /// </summary>
        /// <remarks>
        /// Returns whether the specified role is contained in the roles.
        /// This implementation is case sensitive.
        /// </remarks>
        /// <param name="role">The name of the role for which to check membership.</param>
        /// <returns>Whether the principal is in the role.</returns>
        public bool IsInRole(string role)
        {
            if ((this.Roles == null))
            {
                return false;
            }
            return global::System.Linq.Enumerable.Contains(this.Roles, role);
        }
    }
}
