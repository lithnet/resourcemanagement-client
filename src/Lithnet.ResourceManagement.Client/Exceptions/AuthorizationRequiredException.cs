using Microsoft.ResourceManagement.WebServices.WSAddressing;
using Microsoft.ResourceManagement.WebServices.WSResourceManagement;
using System;
using Microsoft.ResourceManagement.WebServices.Exceptions;
using Microsoft.ResourceManagement.WebServices.Faults;

namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// An exception thrown by the resource management service when permission to perform an action requires further authorization
    /// </summary>
    [Serializable]
    public class AuthorizationRequiredException : ResourceManagementException
    {
        /// <summary>
        /// The string used to construct the exception message
        /// </summary>
        private const string messageFormat = "Authorization required: Pending request ID: {0}";

        /// <summary>
        /// The internal representation of the failure
        /// </summary>
        private AuthorizationRequiredFault details;

        /// <summary>
        /// Gets the endpoint for the authorization request
        /// </summary>
        public string Endpoint => this.details.EndpointReference?.Address?.Value;

        /// <summary>
        /// Gets ID of the authorization reference
        /// </summary>
        public string ResourceReference => this.details.EndpointReference.ReferenceProperties.ResourceReferenceProperty.Text;

        /// <summary>
        /// Initializes a new instance of the AuthorizationRequiredException class
        /// </summary>
        /// <param name="failure">The object containing the details of the failure</param>
        public AuthorizationRequiredException(AuthorizationRequiredFault failure)
            : base(GetMessage(failure))
        {
            this.details = failure;
        }

        /// <summary>
        /// Gets a string representation of the exception
        /// </summary>
        /// <param name="failure">The object containing details of the failure</param>
        /// <returns>A string containing the details of the exception</returns>
        private static string GetMessage(AuthorizationRequiredFault failure)
        {

            if (failure != null)
            {
                return string.Format(
                    AuthorizationRequiredException.messageFormat,
                    failure.EndpointReference.ReferenceProperties?.ResourceReferenceProperty?.Text
                    );
            }
            else
            {
                return string.Format("A generic authorization failure was returned from the server that did not contain detailed information");
            }
        }
    }
}
